using Assets;
using Library.Engine;
using Library.Engine.Types;
using Library.Types;
using Library.Native;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ScenarioEdit.Engine;
using ScenarioEdit.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static ScenarioEdit.UndoItem;

namespace ScenarioEdit
{
    public partial class FormMain : Form
    {
        /// <summary>"
        /// The action that will be performed when clicking the left mouse button.
        /// </summary>
        public enum PrimaryMode
        {
            Insert,
            Select,
            Shape
        }

        public enum ShapeFillMode
        {
            Insert,
            Delete,
            Select,
        }

        public UndoBuffer _undoBuffer { get; set; }
        private bool _snapToGrid = false;
        private ActorBase _mostRecentlySelectedTile = null;
        private Control drawingsurface = new Control();
        private bool _firstShown = true;
        private EngineCore _core;
        private bool _fullScreen = false;
        private bool _hasBeenModified = false;
        private string _currentMapFilename = string.Empty;
        private int _newFilenameIncrement = 1;
        private ToolTip _interrogationTip = new ToolTip();
        private Rectangle? _shapeSelectionRect = null;
        private Rectangle<double> _snapToGridRect = null;
        private ImageList _assetBrowserImageList = new ImageList();
        private Point _lastMouseLocation = new Point();
        private string _partialTilesPath = "Tiles\\";
        private ActorBase _lastPropertiesTabClicked = null;
        private Point<double> _moveTilesStartPosition = null;

        #region Settings.

        private int tilePaintOverlap = 5;

        #endregion

        /// <summary>
        /// Where the mouse was when the user started dragging the map.
        /// </summary>
        private Point<double> dragStartMouse = new Point<double>();
        /// <summary>
        /// Where the mouse was when the user started drawing
        /// </summary>
        private Point<double> insertStartMousePosition = new Point<double>();
        /// <summary>
        /// Where the mouse was when the user started drawing a fill shape.
        /// </summary>
        private Point<double> shapeInsertStartMousePosition = new Point<double>();
        /// <summary>
        /// What the background offset was then the user started dragging the map.
        /// </summary>
        private Point<double> dragStartOffset = new Point<double>();
        /// <summary>
        /// The location that the last block was placed (real world location).
        /// </summary>
        private Point<double> drawLastLocation = new Point<double>();
        /// <summary>
        /// Whether the user is dragging with the right or left mouse buttons when in shape fill mode
        /// </summary>
        private ShapeFillMode shapeFillMode = ShapeFillMode.Insert;
        private Size lastPlacedItemSize = new Size(0, 0);
        private ActorBase lastHoverTile = null;
        private PrimaryMode CurrentPrimaryMode { get; set; } = PrimaryMode.Select;

        //This really shouldn't be necessary! :(
        protected override CreateParams CreateParams
        {
            get
            {
                //Paints all descendants of a window in bottom-to-top painting order using double-buffering.
                // For more information, see Remarks. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000; //WS_EX_COMPOSITED       
                return handleParam;
            }
        }

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            _core = new EngineCore(drawingsurface, new Size(drawingsurface.Width, drawingsurface.Height));

            DoubleBuffered = true;
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            if (_fullScreen)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.Width = Screen.PrimaryScreen.Bounds.Width;
                this.Height = Screen.PrimaryScreen.Bounds.Height;
                this.ShowInTaskbar = true;
                //this.TopMost = true;
                this.WindowState = FormWindowState.Maximized;
            }

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.UserPaint, true);

            System.Reflection.PropertyInfo controlProperty = typeof(Control)
                    .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            controlProperty.SetValue(drawingsurface, true, null);

            this.Shown += FormMain_Shown;

            splitContainerBody.Panel1.Controls.Add(drawingsurface);

            drawingsurface.Dock = DockStyle.Fill;
            drawingsurface.BackColor = Color.FromArgb(60, 60, 60);
            drawingsurface.PreviewKeyDown += drawingsurface_PreviewKeyDown;
            drawingsurface.Paint += new PaintEventHandler(drawingsurface_Paint);
            drawingsurface.MouseClick += new MouseEventHandler(drawingsurface_MouseClick);
            drawingsurface.MouseDoubleClick += new MouseEventHandler(drawingsurface_MouseDoubleClick);
            drawingsurface.MouseDown += new MouseEventHandler(drawingsurface_MouseDown);
            drawingsurface.MouseMove += new MouseEventHandler(drawingsurface_MouseMove);
            drawingsurface.MouseUp += new MouseEventHandler(drawingsurface_MouseUp);

            drawingsurface.Select();
            drawingsurface.Focus();

            PreviewKeyDown += drawingsurface_PreviewKeyDown;
            listViewProperties.PreviewKeyDown += drawingsurface_PreviewKeyDown;
            toolStrip.PreviewKeyDown += drawingsurface_PreviewKeyDown;
            treeViewTiles.PreviewKeyDown += drawingsurface_PreviewKeyDown;

            toolStripButtonCollapseAll.Click += ToolStripButtonCollapseAll_Click;
            toolStripButtonFindSelectedTile.Click += ToolStripButtonFindSelectedTile_Click;

            editSelectionToolStripMenuItem.Click += editSelectionStripMenuItem_Click;
            toolStripButtonInsertMode.Click += ToolStripButtonInsertMode_Click;
            toolStripButtonSelectMode.Click += ToolStripButtonSelectMode_Click;
            saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            newToolStripMenuItem.Click += NewToolStripMenuItem_Click;
            saveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;
            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            toolStripButtonSave.Click += ToolStripButtonSave_Click;
            toolStripButtonOpen.Click += ToolStripButtonOpen_Click;
            toolStripButtonClose.Click += ToolStripButtonClose_Click;
            toolStripButtonNew.Click += ToolStripButtonNew_Click;
            toolStripButtonMoveTileUp.Click += ToolStripButtonMoveTileUp_Click;
            toolStripButtonMoveTileDown.Click += ToolStripButtonMoveTileDown_Click;
            toolStripButtonShapeMode.Click += ToolStripButtonShapeMode_Click;
            toolStripMenuItemResetAllTileMeta.Click += ToolStripMenuItemResetAllTileMeta_Click;
            treeViewTiles.MouseDown += TreeViewTiles_MouseDown;
            treeViewTiles.MouseUp += TreeViewTiles_MouseUp;
            treeViewTiles.BeforeExpand += TreeViewTiles_BeforeExpand;
            toolStripMenuItemScenarioProperties.Click += ToolStripMenuItemScenarioProperties_Click;
            toolStripButtonInsertSwatch.Click += ToolStripButtonInsertSwatch_Click;
            cutToolStripMenuItem.Click += CutToolStripMenuItem_Click;
            copyToolStripMenuItem.Click += CopyToolStripMenuItem_Click;
            pasteToolStripMenuItem.Click += PasteToolStripMenuItem_Click;
            toolStripMenuItemAddLevel.Click += ToolStripMenuItemAddLevel_Click;
            toolStripMenuItemChangeLevel.Click += ToolStripMenuItemChangeLevel_Click;
            toolStripMenuItemSetDefaultLevel.Click += ToolStripMenuItemSetDefaultLevel_Click;
            toolStripMenuItemDeleteLevel.Click += ToolStripMenuItemDeleteLevel_Click;
            toolStripMenuItemViewWorldItems.Click += ToolStripMenuItemViewWorldItems_Click;
            undoToolStripMenuItem.Click += UndoToolStripMenuItem_Click;
            toolStripButtonUndo.Click += UndoToolStripMenuItem_Click;
            redoToolStripMenuItem.Click += RedoToolStripMenuItem_Click;
            toolStripButtonRedo.Click += RedoToolStripMenuItem_Click;

            this.MouseWheel += FormMain_MouseWheel;

            _undoBuffer = new UndoBuffer(_core);

            PopulateMaterials();

            ToolStripButtonSelectMode_Click(new object(), new EventArgs());

            NewToolStripMenuItem_Click(null, null);
            snapToGridToolStripMenuItem_Click(null, null);
        }

        private void FormMain_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                switch (CurrentPrimaryMode)
                {
                    case PrimaryMode.Insert:
                        CurrentPrimaryMode = PrimaryMode.Select;
                        break;
                    case PrimaryMode.Select:
                        CurrentPrimaryMode = PrimaryMode.Shape;
                        break;
                    case PrimaryMode.Shape:
                        CurrentPrimaryMode = PrimaryMode.Insert;
                        break;

                }
            }
            else if (e.Delta < 0)
            {
                switch (CurrentPrimaryMode)
                {
                    case PrimaryMode.Shape:
                        CurrentPrimaryMode = PrimaryMode.Select;
                        break;
                    case PrimaryMode.Select:
                        CurrentPrimaryMode = PrimaryMode.Insert;
                        break;
                    case PrimaryMode.Insert:
                        CurrentPrimaryMode = PrimaryMode.Shape;
                        break;

                }
            }

            toolStripButtonInsertMode.Checked = CurrentPrimaryMode == PrimaryMode.Insert;
            toolStripButtonSelectMode.Checked = CurrentPrimaryMode == PrimaryMode.Select;
            toolStripButtonShapeMode.Checked = CurrentPrimaryMode == PrimaryMode.Shape;
        }

        private void TreeViewTiles_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "<dummy>")
            {
                e.Node.Nodes.Clear();
                PopChildNodes(e.Node, e.Node.FullPath);
            }
        }

        void PopulateMaterials()
        {
            _assetBrowserImageList.Images.Add("<folder>", Resources.AssetTreeView_Folder);

            treeViewTiles.ImageList = _assetBrowserImageList;

            foreach (string d in Directory.GetDirectories(Constants.BaseAssetPath + _partialTilesPath))
            {
                if (Utility.IgnoreFileName(d))
                {
                    continue;
                }
                var directory = Path.GetFileName(d);

                var directoryNode = treeViewTiles.Nodes.Add(_partialTilesPath + directory, directory, "<folder>");
                directoryNode.Nodes.Add("<dummy>");
            }
        }

        public void PopChildNodes(TreeNode parent, string partialPath)
        {
            foreach (string d in Directory.GetDirectories(Constants.BaseAssetPath + _partialTilesPath + partialPath))
            {
                var directory = Path.GetFileName(d);
                if (Utility.IgnoreFileName(directory) || directory.ToLower() == "player")
                {
                    continue;
                }

                var directoryNode = parent.Nodes.Add(_partialTilesPath + directory, directory, "<folder>");
                directoryNode.Nodes.Add("<dummy>");
            }

            foreach (var f in Directory.GetFiles(Constants.BaseAssetPath + _partialTilesPath + partialPath, "*.png"))
            {
                if (Utility.IgnoreFileName(f))
                {
                    continue;
                }
                var file = new FileInfo(f);

                string fileKey = $"{_partialTilesPath}{partialPath}\\{Path.GetFileNameWithoutExtension(file.Name)}";

                _assetBrowserImageList.Images.Add(fileKey, SpriteCache.GetBitmapCached(file.FullName));

                parent.Nodes.Add(fileKey, Path.GetFileNameWithoutExtension(file.Name), fileKey, fileKey);
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            if (_firstShown)
            {
                using (var form = new FormWelcome())
                {
                    var result = form.ShowDialog();

                    if (result == DialogResult.Yes)
                    {
                        NewToolStripMenuItem_Click(null, null);


                        _core.Display.BackgroundOffset.X = -0;
                        _core.Display.BackgroundOffset.Y = -0;

                    }
                    else if (result == DialogResult.OK)
                    {
                        _currentMapFilename = form.SelectedFileName;
                        _core.LoadLevlesAndPopCurrent(_currentMapFilename);

                        if (_core.Levels.Collection[_core.State.CurrentLevel].LastEditBackgroundOffset != null)
                        {
                            _core.Display.BackgroundOffset = _core.Levels.Collection[_core.State.CurrentLevel].LastEditBackgroundOffset;
                        }

                        FormWelcome.AddToRecentList(_currentMapFilename);
                        _hasBeenModified = false;
                    }
                }

                _firstShown = false;
            }
        }

        /// <summary>
        /// Ensures that the dependencies of the tile are also removed.
        /// </summary>
        /// <param name="tile"></param>
        void DeleteTile(ActorBase tile)
        {
            if (tile.Meta != null && tile.Meta.UID != null && tile.Meta.IsContainer == true)
            {
                //If this is a container, then remove the items from the scenario collection.
                _core.State.Items.RemoveAll(o => o.ContainerId == tile.Meta.UID);
            }

            tile.QueueForDelete();
        }


        TreeNode GetRandomChildNode(TreeNode node)
        {
            if (node.Nodes.Count > 0)
            {
                int nodeIndex = MathUtility.RandomNumber(0, node.Nodes.Count);

                if (node.Nodes[nodeIndex].Nodes.Count > 0)
                {
                    return GetRandomChildNode(node.Nodes[nodeIndex]);
                }
                else
                {
                    return node.Nodes[nodeIndex];
                }
            }

            return node;
        }

        private ActorBase PlaceSelectedItem(double x, double y)
        {
            ActorBase insertedTile = null;

            _hasBeenModified = true;

            if (treeViewTiles.SelectedNode == null)
            {
                return null;
            }

            if (treeViewTiles.SelectedNode.Nodes.Count == 1 && treeViewTiles.SelectedNode.Nodes[0].Text == "<dummy>")
            {
                treeViewTiles.SelectedNode.Expand();
            }

            var selectedItem = GetRandomChildNode(treeViewTiles.SelectedNode);
            if (selectedItem != null && selectedItem.Text != "<dummy>")
            {
                insertedTile = _core.Actors.AddNew<ActorBase>(x, y, _partialTilesPath + selectedItem.FullPath);
                insertedTile.RefreshMetadata(false);

                _undoBuffer.Record(insertedTile, ActionPerformed.Created);

                if (insertedTile.Meta.ActorClass == ActorClassName.ActorSpawner)
                {
                    //We do not default thses in the meta data files becaue a refresh of meta data would wipe them out. :(
                    if (insertedTile.Meta.MinLevel == null)
                    {
                        insertedTile.Meta.MinLevel = 0;
                    }
                    if (insertedTile.Meta.MaxLevel == null)
                    {
                        insertedTile.Meta.MaxLevel = 1;
                    }
                }
                else if (insertedTile.Meta.ActorClass == ActorClassName.ActorSpawnPoint)
                {
                    insertedTile.DrawOrder = _core.Actors.Tiles.Max(o => o.DrawOrder ?? 0) + 1;

                    var otherSpawnPoints = _core.Actors.Tiles.Where(o =>
                        o.Meta.ActorClass == ActorClassName.ActorSpawnPoint && o.Meta.UID != insertedTile.Meta.UID).ToList();

                    _undoBuffer.Record(otherSpawnPoints, ActionPerformed.Deleted);

                    otherSpawnPoints.ForEach(o => o.QueueForDelete());
                    _core.PurgeAllDeletedTiles();
                }

                //No need to create GUIDs for every terrain tile.
                if (insertedTile.Meta.ActorClass != ActorClassName.ActorTerrain)
                {
                    insertedTile.Meta.UID = Guid.NewGuid();
                }

                if (insertedTile.Meta.CanTakeDamage != null && ((bool)insertedTile.Meta.CanTakeDamage) == true)
                {
                    if (insertedTile.Meta.HitPoints == null)
                    {
                        insertedTile.Meta.HitPoints = 10;
                    }

                    if (insertedTile.Meta.Experience == null)
                    {
                        insertedTile.Meta.Experience = 10;
                    }
                }

                if (insertedTile.Meta.ActorClass == ActorClassName.ActorSpawner && insertedTile.Meta.SpawnType == ActorClassName.ActorItem)
                {
                    insertedTile.Meta.SpawnSubTypes = (ActorSubType[])Utility.RandomDropSubTypes.Clone();
                }

                if (insertedTile.Meta.CanStack == true && (insertedTile.Meta.Quantity ?? 0) == 0)
                {
                    insertedTile.Meta.Quantity = 1;
                }

                lastPlacedItemSize = insertedTile.Size;
                drawLastLocation = new Point<double>(x, y);
            }

            return insertedTile;
        }

        private void EditorContainer(Guid containerId)
        {
            using (var form = new FormEditContainer(_core, containerId))
            {
                form.ShowDialog();
            }
        }

        bool TrySave()
        {
            //If we already have an open file, then just save it.
            if (string.IsNullOrWhiteSpace(_currentMapFilename) == false)
            {
                _core.PushLevelAndSave(_currentMapFilename);
                FormWelcome.AddToRecentList(_currentMapFilename);
                _hasBeenModified = false;
                return true;
            }
            else //If we do not have a current open file, then we need to "Save As".
            {
                return SaveAs();
            }
        }

        bool SaveAs()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.FileName = $"Newfile {_newFilenameIncrement++}";
                dialog.Filter = "Rougue Quest Scenario (*.rqs)|*.rqs|All files (*.*)|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _core.Materials = EnumFlatMaterials();

                    _currentMapFilename = dialog.FileName;
                    _core.PushLevelAndSave(_currentMapFilename);
                    FormWelcome.AddToRecentList(_currentMapFilename);
                    _hasBeenModified = false;
                    return true;
                }
            }
            return false;
        }

        private List<TileIdentifier> EnumFlatMaterials()
        {
            List<TileIdentifier> materials = new List<TileIdentifier>();

            foreach (string d in Directory.GetDirectories(Constants.BaseAssetPath + _partialTilesPath))
            {
                if (Utility.IgnoreFileName(d))
                {
                    continue;
                }

                materials.AddRange(EnumFlatMaterials(Path.GetFileName(d)));
            }

            return materials;
        }

        private List<TileIdentifier> EnumFlatMaterials(string partialPath)
        {
            List<TileIdentifier> materials = new List<TileIdentifier>();

            foreach (string d in Directory.GetDirectories(Constants.BaseAssetPath + _partialTilesPath + partialPath))
            {
                if (Utility.IgnoreFileName(d))
                {
                    continue;
                }

                materials.AddRange(EnumFlatMaterials(partialPath + "\\" + Path.GetFileName(d)));
            }

            foreach (var f in Directory.GetFiles(Constants.BaseAssetPath + _partialTilesPath + partialPath, "*.png"))
            {
                if (Utility.IgnoreFileName(f))
                {
                    continue;
                }
                var file = new FileInfo(f);

                string tilePath = $"{_partialTilesPath}{partialPath}\\{Path.GetFileNameWithoutExtension(file.Name)}";

                var meta = TileMetadata.GetFreshMetadata(tilePath);

                meta.UID = null; //We do not need unique ids for materials. We reference these by path and the UIDs could change which would be baaaaad.

                materials.Add(new TileIdentifier(tilePath, meta));
            }

            return materials;
        }

        #region Menu Clicks.

        bool _haveAllMaterialsBeenLoaded = false;

        private void ToolStripButtonFindSelectedTile_Click(object sender, EventArgs e)
        {
            if (_haveAllMaterialsBeenLoaded == false)
            {
                var result = MessageBox.Show("Finding the selected tile will require that all materials be populated. This could take seconds to minutes depending on the speed of your machine. Continue?",
                    "Populate all metadata?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                treeViewTiles.BeginUpdate();
                var needToBeLoaded = treeViewTiles.Descendants().Where(o => o.Text == "<dummy>").ToList();
                while (needToBeLoaded.Count > 0)
                {
                    needToBeLoaded.ForEach(o => o.Parent.Expand());
                    needToBeLoaded = treeViewTiles.Descendants().Where(o => o.Text == "<dummy>").ToList();
                }

                _haveAllMaterialsBeenLoaded = true;
                treeViewTiles.CollapseAll();
                treeViewTiles.EndUpdate();
            }

            if (_mostRecentlySelectedTile != null)
            {
                var result = treeViewTiles.Descendants()
                    .Where(x => x.ImageKey == _mostRecentlySelectedTile.TilePath).FirstOrDefault();

                if (result != null)
                {
                    var expandNode = result;
                    while (expandNode != null)
                    {
                        if (expandNode.IsExpanded == false)
                        {
                            expandNode.Expand();
                        }
                        expandNode = expandNode.Parent;
                    }

                    treeViewTiles.SelectedNode = result;
                    result.EnsureVisible();
                }
            }
        }

        private void ToolStripButtonCollapseAll_Click(object sender, EventArgs e)
        {
            treeViewTiles.CollapseAll();
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _undoBuffer.Rollforward();
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _undoBuffer.Rollback();
        }

        private void snapToGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _snapToGrid = !_snapToGrid;
            snapToGridToolStripMenuItem.Checked = _snapToGrid;

            if (_snapToGridRect != null)
            {
                drawingsurface.Invalidate(new Rectangle(
                    (int)_snapToGridRect.X - 2, (int)_snapToGridRect.Y - 2,
                    (int)_snapToGridRect.Width + 4, (int)_snapToGridRect.Height + 4));
            }
        }

        private void ToolStripMenuItemScenarioProperties_Click(object sender, EventArgs e)
        {
            using (var form = new FormScenarioProperties(_core))
            {
                form.ShowDialog();
            }
        }

        private void ToolStripMenuItemViewWorldItems_Click(object sender, EventArgs e)
        {
            if (CheckForNeededSave())
            {
                using (var form = new FormScenarioItems(_core))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                    }
                }
            }
        }

        private void ToolStripMenuItemDeleteLevel_Click(object sender, EventArgs e)
        {
            using (var form = new FormDeleteLevel(_core))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    int levelIndex = form.SelectedLevelIndex;
                    _core.DeleteLevel(levelIndex);
                }
            }
        }

        private void ToolStripMenuItemSetDefaultLevel_Click(object sender, EventArgs e)
        {
            using (var form = new FormSetDefaultLevel(_core))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    int levelIndex = form.SelectedLevelIndex;
                    _core.SetDefaultLevel(levelIndex);
                }
            }
        }

        private void ToolStripMenuItemChangeLevel_Click(object sender, EventArgs e)
        {
            using (var form = new FormSelectLevel(_core))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    int levelIndex = form.SelectedLevelIndex;
                    _core.SelectLevel(levelIndex);

                    if (_core.Levels.Collection[levelIndex].LastEditBackgroundOffset != null)
                    {
                        _core.Display.BackgroundOffset = _core.Levels.Collection[levelIndex].LastEditBackgroundOffset;
                    }
                }
            }
        }

        private void ToolStripMenuItemAddLevel_Click(object sender, EventArgs e)
        {
            using (var form = new FormAddNewLevel(_core))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    int levelIndex = _core.Levels.AddNew(form.LevelName);
                    _core.SelectLevel(levelIndex);
                }
            }
        }

        private void ToolStripMenuItemResetAllTileMeta_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Reset the meta data of all tiles on this map with their default values?",
                "Reset all metadata?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _core.Materials = EnumFlatMaterials();
                _core.ResetAllTilesMetadata();
                MessageBox.Show("Complete.");
            }
        }

        private void ToolStripButtonMoveTileDown_Click(object sender, EventArgs e)
        {
            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
            foreach (var tile in selectedTiles)
            {
                tile.DrawOrder = (tile.DrawOrder ?? 0) - 1;
            }
        }

        private void ToolStripButtonMoveTileUp_Click(object sender, EventArgs e)
        {
            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
            foreach (var tile in selectedTiles)
            {
                tile.DrawOrder = (tile.DrawOrder ?? 0) + 1;
            }
        }

        /// <summary>
        /// Do not continue if this returns false.
        /// </summary>
        /// <returns></returns>
        bool CheckForNeededSave()
        {
            if (_hasBeenModified)
            {
                var result = MessageBox.Show("The current file has been modified. Save it first?",
                    "Save before continuing?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    return TrySave();
                }
                else if (result == DialogResult.Cancel)
                {
                    return false;
                }
            }

            return true;
        }

        private void ToolStripButtonNew_Click(object sender, EventArgs e)
        {
            NewToolStripMenuItem_Click(sender, e);
        }

        private void ToolStripButtonClose_Click(object sender, EventArgs e)
        {
            NewToolStripMenuItem_Click(sender, e);
        }

        private void ToolStripButtonOpen_Click(object sender, EventArgs e)
        {
            OpenToolStripMenuItem_Click(sender, e);
        }

        private void ToolStripButtonSave_Click(object sender, EventArgs e)
        {
            SaveToolStripMenuItem_Click(sender, e);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckForNeededSave() == false)
            {
                return;
            }

            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Rogue Quest Scenario (*.rqs)|*.rqs|All files (*.*)|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _currentMapFilename = dialog.FileName;
                    _core.LoadLevlesAndPopCurrent(_currentMapFilename);

                    if (_core.Levels.Collection[_core.State.CurrentLevel].LastEditBackgroundOffset != null)
                    {
                        _core.Display.BackgroundOffset = _core.Levels.Collection[_core.State.CurrentLevel].LastEditBackgroundOffset;
                    }

                    FormWelcome.AddToRecentList(_currentMapFilename);
                    _hasBeenModified = false;
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrySave();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckForNeededSave() == false)
            {
                return;
            }

            _currentMapFilename = string.Empty;

            _core.Reset();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckForNeededSave() == false)
            {
                return;
            }
        }

        private void ToolStripButtonShapeMode_Click(object sender, EventArgs e)
        {
            CurrentPrimaryMode = PrimaryMode.Shape;
            toolStripButtonInsertMode.Checked = false;
            toolStripButtonSelectMode.Checked = false;
            toolStripButtonShapeMode.Checked = true;

            ClearMultiSelection();
        }

        private void ToolStripButtonSelectMode_Click(object sender, EventArgs e)
        {
            CurrentPrimaryMode = PrimaryMode.Select;
            toolStripButtonInsertMode.Checked = false;
            toolStripButtonSelectMode.Checked = true;
            toolStripButtonShapeMode.Checked = false;

            ClearMultiSelection();
        }

        private void ToolStripButtonInsertMode_Click(object sender, EventArgs e)
        {
            CurrentPrimaryMode = PrimaryMode.Insert;
            toolStripButtonInsertMode.Checked = true;
            toolStripButtonSelectMode.Checked = false;
            toolStripButtonShapeMode.Checked = false;

            ClearMultiSelection();
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //e.ClickedItem
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CheckForNeededSave() == false)
            {
                e.Cancel = true;
                return;
            }

            _core.Stop();
        }

        #endregion

        #region TreeViewTiles.

        private void TreeViewTiles_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _interrogationTip.Hide(sender as Control);
            }
        }

        private void TreeViewTiles_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var lv = sender as TreeView;
                var selectedItem = lv.GetNodeAt(e.X, e.Y);

                if (selectedItem == null)
                {
                    return;
                }

                var meta = TileMetadata.GetFreshMetadata(_partialTilesPath + selectedItem.FullPath);
                if (meta.ActorClass == ActorClassName.ActorItem)
                {
                    string text = meta.Name;

                    if (meta.SubType == ActorSubType.MeleeWeapon || meta.SubType == ActorSubType.RangedWeapon)
                    {
                        text += "\r\n" + $"Damage: {meta.DamageDice:N0}d{meta.DamageDiceFaces:N0}";
                        if (meta.DamageAdditional > 0)
                        {
                            text += $" +{meta.DamageAdditional:N0}";
                        }
                    }
                    else if (meta.SubType == ActorSubType.Armor || meta.SubType == ActorSubType.Boots
                        || meta.SubType == ActorSubType.Garment || meta.SubType == ActorSubType.Gauntlets
                        || meta.SubType == ActorSubType.Helment || meta.SubType == ActorSubType.Shield
                        || meta.SubType == ActorSubType.Necklace || meta.SubType == ActorSubType.Belt
                        || meta.SubType == ActorSubType.Bracers)
                    {
                        text += "\r\n" + $"Armor Class: {meta.AC:N0}";
                    }
                    else if (meta.SubType == ActorSubType.Chest || meta.SubType == ActorSubType.Pack)
                    {
                        text += "\r\n" + $"Max Weight: {meta.WeightCapacity:N0}";
                        text += "\r\n" + $"Bulk Weight: {meta.BulkCapacity:N0}";
                    }

                    text += "\r\n" + $"Weight: {meta.Bulk:N0}";
                    text += "\r\n" + $"Bulk: {meta.Weight:N0}";


                    if (string.IsNullOrWhiteSpace(text) == false)
                    {
                        var location = new Point(e.X + 10, e.Y - 25);
                        _interrogationTip.Show(text, lv, location, 5000);
                    }
                }
            }
        }

        #endregion

        #region drawingsurface.

        private void drawingsurface_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            _hasBeenModified = true;

            if (e.KeyCode == Keys.Oemplus)
            {
                ToolStripButtonMoveTileUp_Click(null, null);
            }
            else if (e.KeyCode == Keys.OemMinus)
            {
                ToolStripButtonMoveTileDown_Click(null, null);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (CurrentPrimaryMode == PrimaryMode.Select)
                {
                    var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();

                    _undoBuffer.Record(selectedTiles, ActionPerformed.Deleted);

                    foreach (var tile in selectedTiles)
                    {
                        DeleteTile(tile);
                    }
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                if (CurrentPrimaryMode == PrimaryMode.Select)
                {
                    var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
                    foreach (var tile in selectedTiles)
                    {
                        tile.X--;
                    }
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (CurrentPrimaryMode == PrimaryMode.Select)
                {
                    var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
                    foreach (var tile in selectedTiles)
                    {
                        tile.X++;
                    }
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (CurrentPrimaryMode == PrimaryMode.Select)
                {
                    var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
                    foreach (var tile in selectedTiles)
                    {
                        tile.Y--;
                    }
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (CurrentPrimaryMode == PrimaryMode.Select)
                {
                    var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
                    foreach (var tile in selectedTiles)
                    {
                        tile.Y++;
                    }
                }
            }
        }

        private void ClearMultiSelection()
        {
            _core.Actors.Tiles.Where(o => o.SelectedHighlight == true)
                .ToList().ForEach(o => o.SelectedHighlight = false);
        }


        private void drawingsurface_MouseDown(object sender, MouseEventArgs e)
        {
            drawingsurface.Select();
            drawingsurface.Focus();

            double ex = e.X;
            double ey = e.Y;

            if (_snapToGrid)
            {
                //ex -= 16;
                //ey -= 16;
            }

            double x = ex + _core.Display.BackgroundOffset.X;
            double y = ey + _core.Display.BackgroundOffset.Y;

            var hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1)).OrderBy(o => o.DrawOrder ?? 0).LastOrDefault();

            if (e.Button == MouseButtons.Middle)
            {
                dragStartOffset = new Point<double>(_core.Display.BackgroundOffset.X, _core.Display.BackgroundOffset.Y);
                dragStartMouse = new Point<double>(ex, ey);
            }

            //We have a bunch of tiles selected but now are clicking on a different tile while not holding shift or control.
            //Since this is not adding to the selection (holsing shift) or removing from the selection (holding control) and not
            //dragging (since we arent over a selected tile, then assume this is a new operation and deslect all tiles.
            if (IsShiftDown() == false && IsControlDown() == false && (hoverTile == null || hoverTile?.SelectedHighlight == false))
            {
                if (e.Button != MouseButtons.Middle) //Still allow panning while items are selected.
                {
                    ClearMultiSelection();
                }
            }

            if (
                (CurrentPrimaryMode == PrimaryMode.Select && e.Button == MouseButtons.Right)
                || (CurrentPrimaryMode == PrimaryMode.Shape && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right))
                || (e.Button == MouseButtons.Left && CurrentPrimaryMode == PrimaryMode.Insert)
                )
            {
                if (_snapToGrid && _snapToGridRect != null &&
                    (CurrentPrimaryMode == PrimaryMode.Insert || CurrentPrimaryMode == PrimaryMode.Shape))
                {
                    shapeInsertStartMousePosition = new Point<double>(_snapToGridRect.X, _snapToGridRect.Y);
                }
                else
                {
                    shapeInsertStartMousePosition = new Point<double>(ex, ey);
                }
            }

            if (e.Button == MouseButtons.Left && CurrentPrimaryMode == PrimaryMode.Insert)
            {
                //Single item placement with left button.

                ClearMultiSelection();

                double placeX = x;
                double placeY = y;

                if (_snapToGrid && _snapToGridRect != null &&
                    (CurrentPrimaryMode == PrimaryMode.Insert || CurrentPrimaryMode == PrimaryMode.Shape))
                {
                    placeX = (_snapToGridRect.X + _core.Display.BackgroundOffset.X) + 16;
                    placeY = (_snapToGridRect.Y + _core.Display.BackgroundOffset.Y) + 16;
                }

                var placedItem = PlaceSelectedItem(placeX, placeY);
                if (placedItem != null)
                {
                    placedItem.SelectedHighlight = true;
                    _mostRecentlySelectedTile = placedItem;
                }
            }

            if (e.Button == MouseButtons.Left && CurrentPrimaryMode == PrimaryMode.Select)
            {
                if (hoverTile != null)
                {
                    //Dragging items:
                    _moveTilesStartPosition = new Point<double>(hoverTile.X, hoverTile.ScreenY);

                    //Place the mouse over the exact center of the tile.
                    Win32.POINT p = new Win32.POINT((int)hoverTile.ScreenX, (int)hoverTile.ScreenY);
                    Win32.ClientToScreen(drawingsurface.Handle, ref p);
                    Win32.SetCursorPos(p.x, p.y);

                    hoverTile.SelectedHighlight = true;
                    _mostRecentlySelectedTile = hoverTile;

                    //Dont populate properties with every click. Its slow.
                    if (listViewProperties.Tag != _lastPropertiesTabClicked)
                    {
                        PopulateSelectedItemProperties();
                        _lastPropertiesTabClicked = hoverTile;
                    }
                }
            }
        }

        public static bool IsShiftDown()
        {
            return (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
        }

        public static bool IsControlDown()
        {
            return (Control.ModifierKeys & Keys.Control) == Keys.Control;
        }

        private void drawingsurface_MouseClick(object sender, MouseEventArgs e)
        {
            double x = e.X + _core.Display.BackgroundOffset.X;
            double y = e.Y + _core.Display.BackgroundOffset.Y;

            var hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1)).OrderBy(o => o.DrawOrder ?? 0).LastOrDefault();

            if (hoverTile != null)
            {
                listViewProperties.Tag = hoverTile;
            }

            //Dont populate properties with every click. Its slow.
            if (listViewProperties.Tag != _lastPropertiesTabClicked)
            {
                PopulateSelectedItemProperties();
                _lastPropertiesTabClicked = hoverTile;
            }

            //Single item deletion with right button.
            if (e.Button == MouseButtons.Right && CurrentPrimaryMode == PrimaryMode.Insert)
            {
                if (hoverTile != null)
                {
                    _undoBuffer.Record(hoverTile, ActionPerformed.Deleted);
                    DeleteTile(hoverTile);
                    _hasBeenModified = true;
                }
            }
        }

        private void drawingsurface_Paint(object sender, PaintEventArgs e)
        {
            var image = _core.Render();

            e.Graphics.DrawImage(image, 0, 0);

            if (_snapToGrid && _snapToGridRect != null &&
                (CurrentPrimaryMode == PrimaryMode.Insert || CurrentPrimaryMode == PrimaryMode.Shape))
            {
                Pen pen = new Pen(Color.Pink, 2);

                var rect = new Rectangle((int)_snapToGridRect.X, (int)_snapToGridRect.Y,
                    (int)_snapToGridRect.Width, (int)_snapToGridRect.Height);

                e.Graphics.DrawRectangle(pen, rect);
            }

            if (_shapeSelectionRect != null)
            {
                Pen pen = new Pen(Color.Yellow, 2);

                if (shapeFillMode == ShapeFillMode.Delete)
                {
                    pen = new Pen(Color.Red, 2);
                }

                e.Graphics.DrawRectangle(pen, (Rectangle)_shapeSelectionRect);
            }
        }

        private void drawingsurface_MouseMove(object sender, MouseEventArgs e)
        {
            double ex = e.X;
            double ey = e.Y;

            double x = ex + _core.Display.BackgroundOffset.X;
            double y = ey + _core.Display.BackgroundOffset.Y;

            _lastMouseLocation.X = (int)ex;
            _lastMouseLocation.Y = (int)ey;

            if (_snapToGrid && (CurrentPrimaryMode == PrimaryMode.Insert || CurrentPrimaryMode == PrimaryMode.Shape))
            {
                if (_snapToGridRect != null)
                {
                    drawingsurface.Invalidate(new Rectangle(
                        (int)_snapToGridRect.X - 2, (int)_snapToGridRect.Y - 2,
                        (int)_snapToGridRect.Width + 4, (int)_snapToGridRect.Height + 4));
                }

                double placeX = (x - 16);
                double placeY = (y - 16);

                //I really wish I knew why this was necessary.
                if (placeX > 0)
                {
                    placeX += 32;
                }
                if (placeY > 0)
                {
                    placeY += 32;
                }

                _snapToGridRect = new Rectangle<double>(
                    ((placeX - (placeX % 32)) - _core.Display.BackgroundOffset.X) - 16,
                    ((placeY - (placeY % 32)) - _core.Display.BackgroundOffset.Y) - 16, 32, 32);

                toolStripStatusLabelDebug.Text = $"{_snapToGridRect.X:N0},{_snapToGridRect.Y:N0}";

                drawingsurface.Invalidate(new Rectangle(
                    (int)_snapToGridRect.X - 2, (int)_snapToGridRect.Y - 2,
                    (int)_snapToGridRect.Width + 4, (int)_snapToGridRect.Height + 4));
            }
            else if (_snapToGrid && (CurrentPrimaryMode == PrimaryMode.Insert))
            {
                double placeX = (x - 16);
                double placeY = (y - 16);

                //I really wish I knew why this was necessary.
                if (placeX > 0)
                {
                    placeX += 32;
                }
                if (placeY > 0)
                {
                    placeY += 32;
                }

                _snapToGridRect = new Rectangle<double>(
                    ((placeX - (placeX % 32)) - _core.Display.BackgroundOffset.X) - 16,
                    ((placeY - (placeY % 32)) - _core.Display.BackgroundOffset.Y) - 16, 32, 32);

            }

            toolStripStatusLabelMouseXY.Text = $"{x}x,{y}y";

            if (e.Button == MouseButtons.Middle)
            {
                _core.Display.BackgroundOffset.X = dragStartOffset.X - (ex - dragStartMouse.X);
                _core.Display.BackgroundOffset.Y = dragStartOffset.Y - (ey - dragStartMouse.Y);
                _core.Display.DrawingSurface.Invalidate();
            }
            else
            {
                var hoverTile = _core.Actors.Intersections(new Point<double>(x, y),
                    new Point<double>(1, 1)).OrderBy(o => o.DrawOrder ?? 0).LastOrDefault();

                if (e.Button == MouseButtons.None)
                {
                    if (e.Button != MouseButtons.Left) //Dont change the selection based on hover location while dragging.
                    {
                        if (lastHoverTile != null && hoverTile != lastHoverTile)
                        {
                            lastHoverTile.HoverHighlight = false;
                        }
                        lastHoverTile = hoverTile;
                    }
                }

                if (e.Button != MouseButtons.Left) //Dont change the selection based on hover location while dragging.
                {
                    if (lastHoverTile != null)
                    {
                        string hoverText = $"[{lastHoverTile.TilePath}]";

                        if (lastHoverTile.Meta?.CanStack == true)
                        {
                            hoverText += $" ({lastHoverTile.Meta.Quantity:N0})";
                        }

                        toolStripStatusLabelHoverObject.Text = hoverText;

                        if (CurrentPrimaryMode != PrimaryMode.Shape)
                        {
                            lastHoverTile.HoverHighlight = true;
                        }
                    }
                    else
                    {
                        toolStripStatusLabelHoverObject.Text = "";
                    }
                }

                if ((CurrentPrimaryMode == PrimaryMode.Shape && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right))
                    || (CurrentPrimaryMode == PrimaryMode.Select && e.Button == MouseButtons.Right))
                {
                    if (_snapToGrid)
                    {
                        if (ex - shapeInsertStartMousePosition.X < 0)
                        {
                            ex -= 32;
                        }
                        if (ey - shapeInsertStartMousePosition.Y < 0)
                        {
                            ey -= 32;
                        }

                        double modX = (ex - shapeInsertStartMousePosition.X) % 32;
                        double modY = (ey - shapeInsertStartMousePosition.Y) % 32;


                        _shapeSelectionRect = GraphicsUtility.SortRectangle(new Rectangle(
                                            (int)shapeInsertStartMousePosition.X,
                                            (int)shapeInsertStartMousePosition.Y,
                                            ((int)(ex - shapeInsertStartMousePosition.X) - (int)modX) + 32,
                                            ((int)(ey - shapeInsertStartMousePosition.Y) - (int)modY) + 32));
                    }
                    else
                    {
                        _shapeSelectionRect = GraphicsUtility.SortRectangle(new Rectangle(
                                            (int)shapeInsertStartMousePosition.X,
                                            (int)shapeInsertStartMousePosition.Y,
                                            (int)(ex - shapeInsertStartMousePosition.X),
                                            (int)(ey - shapeInsertStartMousePosition.Y)));
                    }

                    var rc = (Rectangle)_shapeSelectionRect;
                    drawingsurface.Invalidate();
                    toolStripStatusLabelDebug.Text = $"{rc.X}x,{rc.Y}y->{rc.Width}x,{rc.Height}y";

                    if (CurrentPrimaryMode == PrimaryMode.Shape)
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            shapeFillMode = ShapeFillMode.Insert;
                        }
                        if (e.Button == MouseButtons.Right)
                        {
                            shapeFillMode = ShapeFillMode.Delete;
                        }
                    }
                    else if (CurrentPrimaryMode == PrimaryMode.Select)
                    {
                        shapeFillMode = ShapeFillMode.Select;
                    }
                }

                //Paint with left button.
                if (e.Button == MouseButtons.Left && CurrentPrimaryMode == PrimaryMode.Insert)
                {
                    double drawDeltaX = ex - (drawLastLocation.X - _core.Display.BackgroundOffset.X);
                    double drawDeltaY = ey - (drawLastLocation.Y - _core.Display.BackgroundOffset.Y);

                    if (lastPlacedItemSize.Width > 0)
                    {
                        if (Math.Abs(drawDeltaX) > lastPlacedItemSize.Width - tilePaintOverlap
                            || Math.Abs(drawDeltaY) > lastPlacedItemSize.Height - tilePaintOverlap)
                        {
                            double placeX = x;
                            double placeY = y;

                            if (_snapToGrid && _snapToGridRect != null &&
                                (CurrentPrimaryMode == PrimaryMode.Insert || CurrentPrimaryMode == PrimaryMode.Shape))
                            {
                                placeX = _snapToGridRect.X + _core.Display.BackgroundOffset.X + 16;
                                placeY = _snapToGridRect.Y + _core.Display.BackgroundOffset.Y + 16;
                            }

                            PlaceSelectedItem(placeX, placeY);
                        }
                    }
                }

                //Drag item.
                if (e.Button == MouseButtons.Left && CurrentPrimaryMode == PrimaryMode.Select)
                {
                    var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();

                    if (selectedTiles.Count == 0 && lastHoverTile != null)
                    {
                        selectedTiles.Add(lastHoverTile);
                    }

                    if (selectedTiles.Count > 0 && lastHoverTile != null)
                    {
                        int deltaX = 0;
                        int deltaY = 0;

                        if (_snapToGrid)
                        {
                            var lastX = lastHoverTile.X;
                            var lastY = lastHoverTile.Y;

                            lastHoverTile.X = (x - (x % 32));
                            lastHoverTile.Y = (y - (y % 32));

                            toolStripStatusLabelDebug.Text = $"{lastHoverTile.X:N0},{lastHoverTile.Y:N0}";

                            if (lastHoverTile.X != lastX)
                            {
                                deltaX = (int)(lastX - lastHoverTile.X);
                            }
                            if (lastHoverTile.Y != lastY)
                            {
                                deltaY = (int)(lastY - lastHoverTile.Y);
                            }
                        }
                        else
                        {
                            lastHoverTile.X = x;
                            lastHoverTile.Y = y;

                            deltaX = (int)(lastHoverTile.X - x);
                            deltaY = (int)(lastHoverTile.Y - y);
                        }

                        foreach (var tile in selectedTiles)
                        {
                            if (tile != lastHoverTile)
                            {
                                tile.X -= deltaX;
                                tile.Y -= deltaY;
                            }
                        }

                    }
                }

                //Paint deletion with right button.
                if (e.Button == MouseButtons.Right && CurrentPrimaryMode == PrimaryMode.Insert)
                {
                    if (lastHoverTile != null)
                    {
                        _undoBuffer.Record(lastHoverTile, ActionPerformed.Deleted);
                        DeleteTile(lastHoverTile);
                        _hasBeenModified = true;
                    }
                }
            }
        }

        private void drawingsurface_MouseUp(object sender, MouseEventArgs e)
        {
            //Dont populate properties with every click. Its slow.
            if (listViewProperties.Tag != _lastPropertiesTabClicked)
            {
                PopulateSelectedItemProperties();
            }

            if (_moveTilesStartPosition != null && lastHoverTile != null)
            {
                var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();

                if (selectedTiles.Count == 0 && lastHoverTile != null)
                {
                    selectedTiles.Add(lastHoverTile);
                }

                Point<double> offset = new Point<double>(
                    _moveTilesStartPosition.X - lastHoverTile.X,
                    _moveTilesStartPosition.Y - lastHoverTile.ScreenY);

                _undoBuffer.Record(selectedTiles, ActionPerformed.Moved, offset);
            }

            if (shapeFillMode == ShapeFillMode.Select && (IsShiftDown() || IsControlDown()))
            {
                double x = e.X + _core.Display.BackgroundOffset.X;
                double y = e.Y + _core.Display.BackgroundOffset.Y;

                var hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1)).OrderBy(o => o.DrawOrder ?? 0).LastOrDefault();

                if (hoverTile != null)
                {
                    if (IsShiftDown())
                    {
                        hoverTile.SelectedHighlight = true;
                        _mostRecentlySelectedTile = hoverTile;
                    }
                    else if (IsControlDown())
                    {
                        hoverTile.SelectedHighlight = false;
                    }
                }
            }

            if (
                _shapeSelectionRect != null &&
                   (
                        (CurrentPrimaryMode == PrimaryMode.Shape && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right))
                        || (CurrentPrimaryMode == PrimaryMode.Select && e.Button == MouseButtons.Right)
                   )
                )
            {
                if (shapeFillMode == ShapeFillMode.Select)
                {
                    var rc = (Rectangle)_shapeSelectionRect;
                    double x = rc.X + _core.Display.BackgroundOffset.X;
                    double y = rc.Y + _core.Display.BackgroundOffset.Y;

                    var intersections = _core.Actors.Intersections(x, y, rc.Width, rc.Height);

                    if (IsControlDown() == true)
                    {
                        foreach (var obj in intersections)
                        {
                            obj.SelectedHighlight = false;
                        }
                    }
                    else
                    {
                        foreach (var obj in intersections)
                        {
                            obj.SelectedHighlight = true;
                            _mostRecentlySelectedTile = obj;
                        }
                    }

                }
                else if (shapeFillMode == ShapeFillMode.Insert)
                {
                    var rc = (Rectangle)_shapeSelectionRect;

                    if (_snapToGrid == true)
                    {
                        rc.X += 16;
                        rc.Y += 16;
                    }

                    double x = rc.X + _core.Display.BackgroundOffset.X;
                    double y = rc.Y + _core.Display.BackgroundOffset.Y;

                    if (rc.Height > 0 && rc.Width > 0)
                    {
                        int minHeight = 0;

                        for (int py = 0; py < rc.Height;)
                        {
                            for (int px = 0; px < rc.Width;)
                            {
                                var placedTile = PlaceSelectedItem(x + px, y + py);
                                if (placedTile == null) //No tiles are selected in the explorer.
                                {
                                    _shapeSelectionRect = null;
                                    drawingsurface.Invalidate();
                                    return;
                                }

                                px += placedTile.Size.Width;
                                //We keep track of the min height because I perfer overlap over gaps if the tiles are irregular.
                                if (placedTile.Size.Height > minHeight)
                                {
                                    minHeight = placedTile.Size.Height;
                                }
                            }
                            py += minHeight;
                        }
                    }
                }
                else if (shapeFillMode == ShapeFillMode.Delete)
                {
                    var rc = (Rectangle)_shapeSelectionRect;
                    double x = rc.X + _core.Display.BackgroundOffset.X;
                    double y = rc.Y + _core.Display.BackgroundOffset.Y;

                    var intersections = _core.Actors.Intersections(x, y, rc.Width, rc.Height);

                    _undoBuffer.Record(intersections, ActionPerformed.Deleted);

                    foreach (var obj in intersections)
                    {
                        DeleteTile(obj);
                    }
                }

                _shapeSelectionRect = null;
                drawingsurface.Invalidate();
            }
        }

        private void drawingsurface_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            double x = e.X + _core.Display.BackgroundOffset.X;
            double y = e.Y + _core.Display.BackgroundOffset.Y;

            var hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1)).OrderBy(o => o.DrawOrder ?? 0).LastOrDefault();

            if (hoverTile == null)
            {
                return;
            }

            if (e.Button == MouseButtons.Left && CurrentPrimaryMode == PrimaryMode.Select)
            {
                if (hoverTile.Meta?.IsContainer == true)
                {
                    EditorContainer((Guid)hoverTile.Meta.UID);
                }
                else if (hoverTile.Meta?.ActorClass == ActorClassName.ActorLevelWarpHidden
                    || hoverTile.Meta?.ActorClass == ActorClassName.ActorLevelWarpVisible)
                {
                    using (var form = new FormSelectLevelTile(_core, new ActorClassName[] { ActorClassName.ActorWarpTarget }))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            hoverTile.Meta.LevelWarpName = form.LevelName;
                            hoverTile.Meta.LevelWarpTargetTileUID = (Guid)form.SelectedTile.Meta.UID;
                            PopulateSelectedItemProperties();
                        }
                    }
                }
            }
        }

        #endregion

        #region Form Events.

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            if (_core != null)
            {
                _core.ResizeDrawingSurface(new Size(drawingsurface.Width, drawingsurface.Height));
            }
        }

        private void splitContainerBody_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (_core != null)
            {
                _core.ResizeDrawingSurface(new Size(drawingsurface.Width, drawingsurface.Height));
            }
        }

        #endregion

        #region ListViewProperties.

        private void listViewProperties_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                var selectedItems = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();

                var selectedTile = selectedItems.First();

                listViewProperties.Tag = selectedTile;

                if (listViewProperties.SelectedItems?.Count > 0)
                {
                    var selectedRow = listViewProperties.SelectedItems[0];

                    if (selectedRow.Text == "Contents" && selectedItems.Count == 1)
                    {
                        if (selectedTile.Meta?.IsContainer == true)
                        {
                            EditorContainer((Guid)selectedTile.Meta.UID);
                        }
                    }
                    else if (selectedRow.Text == "Warp to Tile" && selectedItems.Count == 1)
                    {
                        using (var form = new FormSelectLevelTile(_core, new ActorClassName[] { ActorClassName.ActorWarpTarget }))
                        {
                            if (form.ShowDialog() == DialogResult.OK)
                            {
                                selectedTile.Meta.LevelWarpName = form.LevelName;
                                selectedTile.Meta.LevelWarpTargetTileUID = (Guid)form.SelectedTile.Meta.UID;
                                PopulateSelectedItemProperties();
                            }
                        }
                    }
                    else if (selectedRow.Text == "Spawn Type" && selectedItems.Count == 1)
                    {
                    }
                    else if (selectedRow.Text == "Spawn Sub-Types" && selectedItems.Count == 1)
                    {
                        using var form = new FormEditItemSpawner(_core, selectedTile.Meta.SpawnSubTypes?.ToList());
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            selectedTile.Meta.SpawnSubTypes = form.SelectedSubTypes.ToArray();
                        }
                    }

                    else if (selectedRow.Text == "Enchantment" && selectedItems.Count == 1)
                    {
                        var items = new List<ComboItem<EnchantmentType>>();

                        foreach (var item in Enum.GetValues<EnchantmentType>())
                        {
                            items.Add(new ComboItem<EnchantmentType>(item));
                        }

                        using (var form = new FormEditComboBox("Enchantment"))
                        {
                            if (form.ShowDialog(items, selectedTile.Meta.Enchantment?.ToString()) == DialogResult.OK)
                            {
                                selectedTile.Meta.Enchantment = form.GetSelection<EnchantmentType>();
                            }
                        }
                    }
                    else if (selectedRow.Text == "Effects" && selectedItems.Count == 1)
                    {
                        using var form = new FormEditEffects(_core, selectedTile.Meta.Effects);
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            selectedTile.Meta.Effects = form.Effects;
                        }
                    }
                    //listViewProperties.Items.Add("Is Identified").SubItems.Add(selectedTile.Meta.IsIdentified.ToString());
                    //listViewProperties.Items.Add("Enchantment").SubItems.Add(selectedTile.Meta.Enchantment.ToString());
                    //listViewProperties.Items.Add("Effects").SubItems.Add(Utility.GetEffectsText(selectedTile.Meta.Effects));
                    else
                    {
                        if (selectedRow.Text == "Dialog")
                        {
                            using (var dialog = new FormEditText(selectedRow.Text, selectedRow.SubItems[1].Text.ToString()))
                            {
                                if (selectedRow.Text == "Dialog" && selectedItems.Count == 1)
                                {
                                    if (dialog.ShowDialog() == DialogResult.OK)
                                    {
                                        selectedTile.Meta.Dialog = dialog.PropertyValue;
                                        if (selectedTile.Meta.OnlyDialogOnce == null)
                                        {
                                            selectedTile.Meta.OnlyDialogOnce = true;
                                        }
                                    }
                                }
                            }
                        }
                        else if (selectedRow.Text == "Can Take Damage"
                            || selectedRow.Text == "Can Walk On"
                            || selectedRow.Text == "Only Dialog Once"
                            || selectedRow.Text == "Is Identified")
                        {
                            if (bool.TryParse(selectedRow.SubItems[1].Text.ToString(), out bool inputValue) == false)
                            {
                                return;
                            }

                            using (var dialog = new FormEditBoolean(selectedRow.Text, inputValue))
                            {
                                if (dialog.ShowDialog() == DialogResult.OK)
                                {
                                    if (selectedRow.Text == "Can Take Damage")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.CanTakeDamage = dialog.PropertyValue;
                                        }
                                    }
                                    else if (selectedRow.Text == "Can Walk On")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.CanWalkOn = dialog.PropertyValue;
                                        }
                                    }
                                    else if (selectedRow.Text == "Only Dialog Once")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.OnlyDialogOnce = dialog.PropertyValue;
                                        }
                                    }
                                    else if (selectedRow.Text == "Is Identified")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.IsIdentified = dialog.PropertyValue;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var dialog = new FormEditString(selectedRow.Text, selectedRow.SubItems[1].Text.ToString()))
                            {
                                if (dialog.ShowDialog() == DialogResult.OK)
                                {
                                    if (selectedRow.Text == "Location")
                                    {
                                        var coords = dialog.PropertyValue.Split(",");
                                        if (coords.Length == 2)
                                        {
                                            var x = double.Parse(coords[0]);
                                            var y = double.Parse(coords[1]);

                                            foreach (var tile in selectedItems)
                                            {
                                                tile.X = x;
                                                tile.Y = y;
                                            }
                                        }
                                    }
                                    else if (selectedRow.Text == "Name" && selectedItems.Count == 1)
                                    {
                                        selectedTile.Meta.Name = dialog.PropertyValue;
                                    }
                                    else if (selectedRow.Text == "Min level")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.MinLevel = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Max level")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.MaxLevel = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Hit Points")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.HitPoints = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Level")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.Level = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Rarity")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.Rarity = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Experience")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.Experience = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Quantity")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.Quantity = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Charges")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.Charges = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Angle")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Velocity.Angle.Degrees = double.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "z-Order")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.DrawOrder = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Strength")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.Strength = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Dexterity")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.Dexterity = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Armor Class")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.AC = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Damage Dice")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.DamageDice = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Damage Dice Faces")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.DamageDiceFaces = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Damage Additional")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.DamageAdditional = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Bulk")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.Bulk = double.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Weight")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.Weight = double.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Bulk Capacity")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.BulkCapacity = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                    else if (selectedRow.Text == "Weight Capacity")
                                    {
                                        foreach (var tile in selectedItems)
                                        {
                                            tile.Meta.WeightCapacity = int.Parse(dialog.PropertyValue);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                PopulateSelectedItemProperties();

                foreach (var tile in selectedItems)
                {
                    tile.Invalidate();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void PopulateSelectedItemProperties()
        {
            var selectedTile = listViewProperties.Tag as ActorBase;

            string selectedRowText = string.Empty;

            if (listViewProperties.SelectedItems?.Count == 1)
            {
                selectedRowText = listViewProperties.SelectedItems[0].Text;
            }

            listViewProperties.Items.Clear();

            if (selectedTile != null && selectedTile.Visible)
            {
                listViewProperties.Items.Add("Name").SubItems.Add(selectedTile.Meta?.Name);
                listViewProperties.Items.Add("Tile").SubItems.Add(selectedTile.TilePath);
                listViewProperties.Items.Add("Actor Class").SubItems.Add(selectedTile.Meta?.ActorClass.ToString());
                listViewProperties.Items.Add("Sub Type").SubItems.Add(selectedTile.Meta?.SubType.ToString());
                listViewProperties.Items.Add("z-Order").SubItems.Add(selectedTile.DrawOrder.ToString());
                listViewProperties.Items.Add("Size").SubItems.Add(selectedTile.Size.ToString());

                if (selectedTile.Meta.ActorClass != ActorClassName.ActorSpawner
                    && selectedTile.Meta.ActorClass != ActorClassName.ActorSpawnPoint
                    && selectedTile.Meta.ActorClass != ActorClassName.ActorLevelWarpHidden
                    && selectedTile.Meta.ActorClass != ActorClassName.ActorLevelWarpVisible)
                {
                    listViewProperties.Items.Add("Can Walk On").SubItems.Add(selectedTile.Meta?.CanWalkOn.ToString());
                    listViewProperties.Items.Add("Angle").SubItems.Add(selectedTile.Velocity.Angle.Degrees.ToString());
                }

                if (selectedTile.Meta.ActorClass == ActorClassName.ActorSpawner)
                {
                    listViewProperties.Items.Add("Min level").SubItems.Add(selectedTile.Meta?.MinLevel.ToString());
                    listViewProperties.Items.Add("Max level").SubItems.Add(selectedTile.Meta?.MaxLevel.ToString());
                    listViewProperties.Items.Add("Spawn Type").SubItems.Add(selectedTile.Meta.SpawnType.ToString());

                    string subTypes = string.Empty;

                    if (selectedTile.Meta.SpawnSubTypes != null)
                    {
                        subTypes = String.Join(',', selectedTile.Meta.SpawnSubTypes);
                    }

                    listViewProperties.Items.Add("Spawn Sub-Types").SubItems.Add(subTypes);
                }

                if (selectedTile.Meta.ActorClass == ActorClassName.ActorFriendyBeing
                    || selectedTile.Meta.ActorClass == ActorClassName.ActorHostileBeing
                    || selectedTile.Meta.ActorClass == ActorClassName.ActorItem
                    || selectedTile.Meta.ActorClass == ActorClassName.ActorLevelWarpVisible
                    || selectedTile.Meta.ActorClass == ActorClassName.ActorLevelWarpHidden
                    || selectedTile.Meta.ActorClass == ActorClassName.ActorBlockaid
                    || selectedTile.Meta.ActorClass == ActorClassName.ActorBlockadeHidden
                    || selectedTile.Meta.ActorClass == ActorClassName.ActorHiddenMessage
                    || selectedTile.Meta.ActorClass == ActorClassName.ActorStore
                    || selectedTile.Meta.ActorClass == ActorClassName.ActorBuilding)
                {
                    listViewProperties.Items.Add("Dialog").SubItems.Add(selectedTile.Meta.Dialog);
                    listViewProperties.Items.Add("Only Dialog Once").SubItems.Add(selectedTile.Meta.OnlyDialogOnce.ToString());
                }

                if (selectedTile.Meta.ActorClass == ActorClassName.ActorItem)
                {
                    listViewProperties.Items.Add("Is Identified").SubItems.Add(selectedTile.Meta.IsIdentified.ToString());
                    listViewProperties.Items.Add("Enchantment").SubItems.Add(selectedTile.Meta.Enchantment.ToString());
                    listViewProperties.Items.Add("Effects").SubItems.Add(Utility.GetEffectsText(selectedTile.Meta.Effects));
                }

                if (selectedTile.Meta.ActorClass == ActorClassName.ActorLevelWarpHidden
                    || selectedTile.Meta.ActorClass == ActorClassName.ActorLevelWarpVisible)
                {
                    string spawnTargetDisplay = "<orphaned>";

                    int levelIndex = _core.Levels.GetIndex(selectedTile.Meta.LevelWarpName);
                    if (levelIndex >= 0)
                    {
                        var levelChunks = _core.Levels.GetChunks(levelIndex);

                        var spawnTarget = levelChunks.Where(o => o.Meta.UID == selectedTile.Meta?.LevelWarpTargetTileUID).FirstOrDefault();
                        if (spawnTarget != null)
                        {
                            spawnTargetDisplay = $"Level: {_core.Levels.ByIndex(levelIndex).Name}, Tile: {spawnTarget.Meta?.Name}";
                        }
                    }

                    listViewProperties.Items.Add("Warp to Tile").SubItems.Add(spawnTargetDisplay);
                }

                if (selectedTile.Meta.ActorClass == ActorClassName.ActorHostileBeing)
                {
                    listViewProperties.Items.Add("Can Take Damage").SubItems.Add(selectedTile.Meta?.CanTakeDamage.ToString());
                    listViewProperties.Items.Add("Hit Points").SubItems.Add(selectedTile.Meta?.HitPoints.ToString());
                    listViewProperties.Items.Add("Experience").SubItems.Add(selectedTile.Meta?.Experience.ToString());
                    listViewProperties.Items.Add("Strength").SubItems.Add(selectedTile.Meta?.Strength.ToString());
                    listViewProperties.Items.Add("Dexterity").SubItems.Add(selectedTile.Meta?.Dexterity.ToString());
                    listViewProperties.Items.Add("Armor Class").SubItems.Add(selectedTile.Meta?.AC.ToString());
                }

                if (selectedTile.Meta.SubType == ActorSubType.Armor || selectedTile.Meta.SubType == ActorSubType.Gauntlets
                    || selectedTile.Meta.SubType == ActorSubType.Helment || selectedTile.Meta.SubType == ActorSubType.Bracers
                    || selectedTile.Meta.SubType == ActorSubType.Boots || selectedTile.Meta.SubType == ActorSubType.Shield
                    || selectedTile.Meta.SubType == ActorSubType.Ring || selectedTile.Meta.SubType == ActorSubType.Garment
                    || selectedTile.Meta.SubType == ActorSubType.Belt || selectedTile.Meta.SubType == ActorSubType.Necklace)
                {
                    listViewProperties.Items.Add("Armor Class").SubItems.Add(selectedTile.Meta?.AC.ToString());
                }

                if (selectedTile.Meta.SubType == ActorSubType.Wand
                    || selectedTile.Meta.SubType == ActorSubType.MeleeWeapon
                    || selectedTile.Meta.SubType == ActorSubType.RangedWeapon)
                {
                    listViewProperties.Items.Add("Damage Dice").SubItems.Add(selectedTile.Meta?.DamageDice.ToString());
                    listViewProperties.Items.Add("Damage Dice Faces").SubItems.Add(selectedTile.Meta?.DamageDiceFaces.ToString());
                    listViewProperties.Items.Add("Damage Additional").SubItems.Add(selectedTile.Meta?.DamageAdditional.ToString());
                }

                if (selectedTile.Meta.SubType == ActorSubType.Pack)
                {
                    listViewProperties.Items.Add("BulkCapacity").SubItems.Add(selectedTile.Meta?.BulkCapacity.ToString());
                    listViewProperties.Items.Add("WeightCapacity").SubItems.Add(selectedTile.Meta?.WeightCapacity.ToString());
                }

                listViewProperties.Items.Add("Bulk").SubItems.Add(selectedTile.Meta?.Bulk.ToString());
                listViewProperties.Items.Add("Weight").SubItems.Add(selectedTile.Meta?.Weight.ToString());
                listViewProperties.Items.Add("Level").SubItems.Add(selectedTile.Meta?.Level.ToString());

                listViewProperties.Items.Add("Location").SubItems.Add($"{selectedTile.Location.X},{selectedTile.Location.Y}");

                if (selectedTile.Meta?.IsContainer == true)
                {
                    listViewProperties.Items.Add("Contents").SubItems.Add("<open>");
                }
                if (selectedTile.Meta?.CanStack == true)
                {
                    listViewProperties.Items.Add("Quantity").SubItems.Add(selectedTile.Meta?.Quantity.ToString());
                }
                if (selectedTile.Meta.SubType == ActorSubType.Wand)
                {
                    listViewProperties.Items.Add("Charges").SubItems.Add(selectedTile.Meta?.Charges.ToString());
                }

                listViewProperties.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

                //If there was an item selected before, select it again and scroll it into view.
                if (string.IsNullOrEmpty(selectedRowText) == false)
                {
                    var item = listViewProperties.FindItemWithText(selectedRowText);
                    if (item != null)
                    {
                        item.Selected = true;
                        item.EnsureVisible();
                    }
                }
            }
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearMultiSelection();

            if (Singletons.ClipboardTiles.Count > 0)
            {
                var firstTile = Singletons.ClipboardTiles.First();

                int deltaX = (int)(_core.Display.BackgroundOffset.X + _lastMouseLocation.X - firstTile.X);
                int deltaY = (int)(_core.Display.BackgroundOffset.Y + _lastMouseLocation.Y - firstTile.Y);

                foreach (var tile in Singletons.ClipboardTiles)
                {
                    tile.SelectedHighlight = false;

                    var clone = tile.Clone();
                    clone.X += deltaX;
                    clone.Y += deltaY;

                    clone.SelectedHighlight = true;
                    _mostRecentlySelectedTile = clone;

                    _core.Actors.Add(clone);
                }
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Singletons.ClipboardTiles.Clear();

            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
            if (selectedTiles.Count > 0)
            {
                Singletons.ClipboardTiles.AddRange(selectedTiles.Select(o => o.Clone()));
            }
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Singletons.ClipboardTiles.Clear();

            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
            if (selectedTiles.Count > 0)
            {
                Singletons.ClipboardTiles.AddRange(selectedTiles.Select(o => o.Clone()));
            }

            _undoBuffer.Record(selectedTiles, ActionPerformed.Deleted);

            selectedTiles.ForEach(o => o.QueueForDelete());
            _core.PurgeAllDeletedTiles();
        }

        private void ToolStripButtonInsertSwatch_Click(object sender, EventArgs e)
        {
            using (var form = new FormSelectSwatch())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ClearMultiSelection();

                    CurrentPrimaryMode = PrimaryMode.Select;

                    var chunks = Levels.LoadChunks(form.SelectedSwatchFileName, 0);

                    double deltaX = 0;
                    double deltaY = 0;

                    double x = _core.Display.BackgroundOffset.X + 16;
                    double y = _core.Display.BackgroundOffset.Y + 16;

                    if (_snapToGrid)
                    {
                        _snapToGridRect = new Rectangle<double>(
                            ((x - (x % 32)) - _core.Display.BackgroundOffset.X) - 16,
                            ((y - (y % 32)) - _core.Display.BackgroundOffset.Y) - 16, 32, 32);

                        x = (_snapToGridRect.X + _core.Display.BackgroundOffset.X) + 16;
                        y = (_snapToGridRect.Y + _core.Display.BackgroundOffset.Y) + 16;
                    }

                    var firstChunk = chunks.FirstOrDefault();
                    if (firstChunk != null)
                    {
                        deltaX = firstChunk.X - x;
                        deltaY = firstChunk.Y - y;
                    }

                    foreach (var chunk in chunks)
                    {
                        var tile = new ActorBase(_core)
                        {
                            X = chunk.X - deltaX,
                            Y = chunk.Y - deltaY,
                            TilePath = chunk.TilePath,
                            DrawOrder = chunk.DrawOrder,
                            Meta = chunk.Meta
                        };

                        tile.Velocity.Angle.Degrees = chunk.Angle ?? 0;
                        tile.SetImage(Constants.GetAssetPath($"{chunk.TilePath}.png"));

                        tile.SelectedHighlight = true;
                        _mostRecentlySelectedTile = tile;

                        _core.Actors.Add(tile);
                    }
                }
            }
        }

        private void editSelectionStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new FormEditSelection(_core))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                }
            }
        }

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
            if (selectedTiles.Count > 0)
            {
                var firstTile = selectedTiles.First();

                int deltaX = (int)(_core.Display.BackgroundOffset.X + _lastMouseLocation.X - firstTile.X);
                int deltaY = (int)(_core.Display.BackgroundOffset.Y + _lastMouseLocation.Y - firstTile.Y);

                foreach (var tile in selectedTiles)
                {
                    tile.SelectedHighlight = false;

                    var clone = tile.Clone();
                    clone.X += deltaX;
                    clone.Y += deltaY;

                    clone.SelectedHighlight = true;
                    _mostRecentlySelectedTile = clone;

                    _core.Actors.Add(clone);
                }
            }
        }

        private void deselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearMultiSelection();
        }

        private void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _core.Actors.Tiles.ForEach(o => o.SelectedHighlight = !o.SelectedHighlight);
            _mostRecentlySelectedTile = _core.Actors.Tiles.FirstOrDefault(o => o.SelectedHighlight == true);
        }

        private void expandSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
            foreach (var tile in _core.Actors.Tiles)
            {
                foreach (var selection in selectedTiles)
                {
                    if (tile.SelectedHighlight == false && tile.Intersects(selection, 1))
                    {
                        tile.SelectedHighlight = true;
                        _mostRecentlySelectedTile = tile;
                    }
                }
            }
        }

        private void verticalCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActorBase alignWith = _mostRecentlySelectedTile;
            if (alignWith == null)
            {
                alignWith = _core.Actors.Tiles.FirstOrDefault(o => o.SelectedHighlight == true);
            }

            if (alignWith == null)
            {
                return;
            }

            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true && o != alignWith).ToList();
            foreach (var tile in selectedTiles)
            {
                tile.X = alignWith.X;
            }
        }

        private void horizontalCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActorBase alignWith = _mostRecentlySelectedTile;
            if (alignWith == null)
            {
                alignWith = _core.Actors.Tiles.FirstOrDefault(o => o.SelectedHighlight == true);
            }

            if (alignWith == null)
            {
                return;
            }

            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true && o != alignWith).ToList();
            foreach (var tile in selectedTiles)
            {
                tile.Y = alignWith.Y;
            }
        }

        private void topsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActorBase alignWith = _mostRecentlySelectedTile;
            if (alignWith == null)
            {
                alignWith = _core.Actors.Tiles.FirstOrDefault(o => o.SelectedHighlight == true);
            }

            if (alignWith == null)
            {
                return;
            }

            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true && o != alignWith).ToList();
            foreach (var tile in selectedTiles)
            {
                tile.Y = (alignWith.BoundLocation.Y - (tile.Size.Height / 2)) + tile.Size.Height;
            }
        }

        private void bottomsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActorBase alignWith = _mostRecentlySelectedTile;
            if (alignWith == null)
            {
                alignWith = _core.Actors.Tiles.FirstOrDefault(o => o.SelectedHighlight == true);
            }

            if (alignWith == null)
            {
                return;
            }

            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true && o != alignWith).ToList();
            foreach (var tile in selectedTiles)
            {
                tile.Y = (alignWith.BoundLocation.Y + alignWith.Size.Height) - (tile.Size.Height / 2);
            }
        }

        private void leftSidesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActorBase alignWith = _mostRecentlySelectedTile;
            if (alignWith == null)
            {
                alignWith = _core.Actors.Tiles.FirstOrDefault(o => o.SelectedHighlight == true);
            }

            if (alignWith == null)
            {
                return;
            }

            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true && o != alignWith).ToList();
            foreach (var tile in selectedTiles)
            {
                tile.X = (alignWith.BoundLocation.X - (tile.Size.Width / 2)) + tile.Size.Width;
            }
        }

        private void rightSidesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActorBase alignWith = _mostRecentlySelectedTile;
            if (alignWith == null)
            {
                alignWith = _core.Actors.Tiles.FirstOrDefault(o => o.SelectedHighlight == true);
            }

            if (alignWith == null)
            {
                return;
            }

            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true && o != alignWith).ToList();
            foreach (var tile in selectedTiles)
            {
                tile.X = (alignWith.BoundLocation.X + alignWith.Size.Width) - (tile.Size.Width / 2);
            }
        }

        #endregion

        private void exportMaterialManifestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string saveFilename;

                using (var dialog = new SaveFileDialog())
                {
                    dialog.FileName = $"Item Manifest";
                    dialog.Filter = "Excel (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    dialog.OverwritePrompt = true;

                    if (dialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    saveFilename = dialog.FileName;
                }

                var materials = EnumFlatMaterials();

                var workbook = new XSSFWorkbook();
                var weapons = workbook.CreateSheet("Weapons");
                var projectiles = workbook.CreateSheet("Projectiles");
                var scrolls = workbook.CreateSheet("Scrolls");
                var books = workbook.CreateSheet("Books");
                var potions = workbook.CreateSheet("Potions");
                var wands = workbook.CreateSheet("Wands");
                var equipment = workbook.CreateSheet("Equipment");
                var containers = workbook.CreateSheet("Containers");
                var hostiles = workbook.CreateSheet("Hostiles");

                void CreateHeader(ISheet sheet, string nameCsv)
                {
                    var boldStyle = workbook.CreateFont();
                    var boldCellStyle = workbook.CreateCellStyle();
                    boldStyle.IsBold = true;
                    boldCellStyle.SetFont(boldStyle);

                    IRow header = sheet.CreateRow(sheet.LastRowNum);
                    var names = nameCsv.Split(',');
                    int cellCount = 0;

                    foreach (var name in names)
                    {
                        var cell = header.CreateCell(cellCount++);
                        cell.CellStyle = boldCellStyle;
                        cell.SetCellValue(name);
                    }
                }

                foreach (var item in materials)
                {
                    if (item.Meta.SubType == ActorSubType.MeleeWeapon || item.Meta.SubType == ActorSubType.RangedWeapon)
                    {
                        if (weapons.PhysicalNumberOfRows == 0) CreateHeader(weapons, "Name,Level,Value,Bulk,Weight,Rarity,Type,Damage Type,Damage");
                        IRow body = weapons.CreateRow(weapons.PhysicalNumberOfRows);
                        int cell = 0;

                        if (item.Meta.Name == "Abaddon")
                        {
                        }

                        /*
                        double value = ((((item.Meta.DamageDice ?? 0) * 2) * (item.Meta.DamageDiceFaces ?? 0)) * 2);
                        if ((item.Meta.DamageAdditional ?? 0) > 0)
                        {
                            value *= ((item.Meta.DamageAdditional ?? 0) * 10);
                        }
                        */

                        body.CreateCell(cell++).SetCellValue(item.Meta.Name);
                        body.CreateCell(cell++).SetCellValue((item.Meta.Level ?? 0));
                        //body.CreateCell(cell++).SetCellValue(value);
                        body.CreateCell(cell++).SetCellValue((item.Meta.Value ?? 0));
                        body.CreateCell(cell++).SetCellValue((item.Meta.Bulk ?? 0));
                        body.CreateCell(cell++).SetCellValue((item.Meta.Weight ?? 0));
                        body.CreateCell(cell++).SetCellValue(item.Meta.RarityText);

                        body.CreateCell(cell++).SetCellValue(item.Meta.SubType?.ToString());
                        body.CreateCell(cell++).SetCellValue(item.Meta.DamageType?.ToString());
                        body.CreateCell(cell++).SetCellValue(item.Meta.DndDamageText);
                    }

                    if (item.Meta.ActorClass == ActorClassName.ActorHostileBeing)
                    {
                        if (hostiles.PhysicalNumberOfRows == 0) CreateHeader(hostiles, "Name,Level,Hit Points,AC,Dexterity,Strength,Damage,Damage Type,Weakness,Experience");
                        IRow body = hostiles.CreateRow(hostiles.PhysicalNumberOfRows);
                        int cell = 0;
                        body.CreateCell(cell++).SetCellValue(item.Meta.Name);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Level ?? 0);

                        body.CreateCell(cell++).SetCellValue(item.Meta.HitPoints ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.AC ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Dexterity ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Strength ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.DndDamageText);
                        body.CreateCell(cell++).SetCellValue(item.Meta.DamageType?.ToString());
                        body.CreateCell(cell++).SetCellValue((Utility.GetOppositeOfDamageType(item.Meta.DamageType ?? DamageType.Unspecified)).ToString());
                        body.CreateCell(cell++).SetCellValue(item.Meta.Experience ?? 0);
                    }

                    if (item.Meta.SubType == ActorSubType.Projectile)
                    {
                        if (projectiles.PhysicalNumberOfRows == 0) CreateHeader(projectiles, "Name,Level,Value,Bulk,Weight,Rarity,Type,Damage Type,Damage");
                        IRow body = projectiles.CreateRow(projectiles.PhysicalNumberOfRows);
                        int cell = 0;
                        body.CreateCell(cell++).SetCellValue(item.Meta.Name);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Level ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Value ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Bulk ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Weight ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.RarityText);

                        body.CreateCell(cell++).SetCellValue(item.Meta.SubType?.ToString());
                        body.CreateCell(cell++).SetCellValue(item.Meta.DamageType?.ToString());
                        body.CreateCell(cell++).SetCellValue(item.Meta.DndDamageText);
                    }

                    if (item.Meta.SubType == ActorSubType.Armor
                        || item.Meta.SubType == ActorSubType.Boots
                        || item.Meta.SubType == ActorSubType.Bracers
                        || item.Meta.SubType == ActorSubType.Garment
                        || item.Meta.SubType == ActorSubType.Gauntlets
                        || item.Meta.SubType == ActorSubType.Helment
                        || item.Meta.SubType == ActorSubType.Necklace
                        || item.Meta.SubType == ActorSubType.Ring
                        || item.Meta.SubType == ActorSubType.Shield)
                    {
                        if (equipment.PhysicalNumberOfRows == 0) CreateHeader(equipment, "Name,Level,Value,Bulk,Weight,Rarity,Type,Damage Type,AC");
                        IRow body = equipment.CreateRow(equipment.PhysicalNumberOfRows);
                        int cell = 0;
                        body.CreateCell(cell++).SetCellValue(item.Meta.Name);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Level ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Value ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Bulk ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Weight ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.RarityText);

                        body.CreateCell(cell++).SetCellValue(item.Meta.SubType?.ToString());
                        body.CreateCell(cell++).SetCellValue(item.Meta.DamageType?.ToString());
                        body.CreateCell(cell++).SetCellValue(item.Meta.AC ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.RarityText);
                    }

                    if (item.Meta.SubType == ActorSubType.Belt
                        || item.Meta.SubType == ActorSubType.Chest
                        || item.Meta.SubType == ActorSubType.Pack)
                    {
                        if (containers.PhysicalNumberOfRows == 0) CreateHeader(containers, "Name,Level,Value,Bulk,Weight,Rarity,Type,Item Capacity,Bulk Capacity,Weight Capacity");
                        IRow body = containers.CreateRow(containers.PhysicalNumberOfRows);
                        int cell = 0;
                        body.CreateCell(cell++).SetCellValue(item.Meta.Name);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Level ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Value ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Bulk ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Weight ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.RarityText);

                        body.CreateCell(cell++).SetCellValue(item.Meta.SubType?.ToString());
                        body.CreateCell(cell++).SetCellValue(item.Meta.ItemCapacity ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.BulkCapacity ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.WeightCapacity ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.RarityText);
                    }

                    if (item.Meta.SubType == ActorSubType.Book)
                    {
                        if (books.PhysicalNumberOfRows == 0) CreateHeader(books, "Name,Level,Value,Bulk,Weight,Rarity,Mana,Effect");

                        IRow body = books.CreateRow(books.PhysicalNumberOfRows);
                        int cell = 0;
                        body.CreateCell(cell++).SetCellValue(item.Meta.Name);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Level ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Value ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Bulk ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Weight ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.RarityText);

                        body.CreateCell(cell++).SetCellValue(item.Meta.Mana ?? 0);
                        body.CreateCell(cell++).SetCellValue($"Permanently learn {item.Meta.SpellName}");
                    }

                    if (item.Meta.SubType == ActorSubType.Scroll)
                    {
                        if (scrolls.PhysicalNumberOfRows == 0) CreateHeader(scrolls, "Name,Level,Value,Bulk,Weight,Rarity,Mana,Effect,Damage,Cast Time");

                        IRow body = scrolls.CreateRow(scrolls.PhysicalNumberOfRows);
                        int cell = 0;
                        body.CreateCell(cell++).SetCellValue(item.Meta.Name);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Level ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Value ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Bulk ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Weight ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.RarityText);

                        body.CreateCell(cell++).SetCellValue(item.Meta.Mana ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.EffectText);
                        body.CreateCell(cell++).SetCellValue(item.Meta.DndDamageText);
                        body.CreateCell(cell++).SetCellValue(item.Meta.CastTime ?? 0);
                    }

                    if (item.Meta.SubType == ActorSubType.Wand)
                    {
                        if (wands.PhysicalNumberOfRows == 0) CreateHeader(wands, "Name,Level,Value,Bulk,Weight,Rarity,Effect,Damage,Cast Time");

                        IRow body = wands.CreateRow(wands.PhysicalNumberOfRows);
                        int cell = 0;
                        body.CreateCell(cell++).SetCellValue(item.Meta.Name);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Level ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Value ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Bulk ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Weight ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.RarityText);

                        body.CreateCell(cell++).SetCellValue(item.Meta.EffectText);
                        body.CreateCell(cell++).SetCellValue(item.Meta.DndDamageText);
                        body.CreateCell(cell++).SetCellValue(item.Meta.CastTime ?? 0);
                    }

                    if (item.Meta.SubType == ActorSubType.Potion)
                    {
                        if (potions.PhysicalNumberOfRows == 0) CreateHeader(potions, "Name,Level,Value,Bulk,Weight,Rarity,Effect");

                        IRow body = potions.CreateRow(potions.PhysicalNumberOfRows);
                        int cell = 0;
                        body.CreateCell(cell++).SetCellValue(item.Meta.Name);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Level ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Value ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Bulk ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.Weight ?? 0);
                        body.CreateCell(cell++).SetCellValue(item.Meta.RarityText);

                        body.CreateCell(cell++).SetCellValue(item.Meta.EffectText);
                    }
                }

                FileStream sw = File.Create(saveFilename);
                workbook.Write(sw);
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void toolStripButtonToggleMinimap_Click(object sender, EventArgs e)
        {
            _core.DrawMinimap = toolStripButtonToggleMinimap.Checked;
            drawingsurface.Invalidate();
        }
    }
}
