using Assets;
using Library.Engine;
using Library.Engine.Types;
using Library.Types;
using Library.Utility;
using ScenarioEdit.Engine;
using ScenarioEdit.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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
        private Rectangle? _snapToGridRect = null;
        private ImageList _assetBrowserImageList = new ImageList();
        private Point _lastMouseLocation = new Point();
        private string _partialTilesPath = "Tiles\\";
        private ActorBase _lastPropertiesTabClicked = null;


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
            toolStripButtonPlayMap.Click += ToolStripButtonPlayMap_Click;
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

            PopulateMaterials();

            ToolStripButtonSelectMode_Click(new object(), new EventArgs());

            NewToolStripMenuItem_Click(null, null);
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
                var directory = Path.GetFileName(d);
                if (directory.StartsWith("@") || directory.ToLower() == "player")
                {
                    continue;
                }

                var directoryNode = treeViewTiles.Nodes.Add(_partialTilesPath + directory, directory, "<folder>");
                directoryNode.Nodes.Add("<dummy>");
            }
        }

        public void PopChildNodes(TreeNode parent, string partialPath)
        {
            foreach (string d in Directory.GetDirectories(Constants.BaseAssetPath + _partialTilesPath + partialPath))
            {
                var directory = Path.GetFileName(d);
                if (directory.StartsWith("@") || directory.ToLower() == "player")
                {
                    continue;
                }

                var directoryNode = parent.Nodes.Add(_partialTilesPath + directory, directory, "<folder>");
                directoryNode.Nodes.Add("<dummy>");
            }

            foreach (var f in Directory.GetFiles(Constants.BaseAssetPath + _partialTilesPath + partialPath, "*.png"))
            {
                if (Path.GetFileName(f).StartsWith("@"))
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
                    insertedTile.DrawOrder = _core.Actors.Tiles.Max(o => o.DrawOrder) + 1;

                    var otherSpawnPoints = _core.Actors.Tiles.Where(o =>
                        o.Meta.ActorClass == ActorClassName.ActorSpawnPoint && o.Meta.UID != insertedTile.Meta.UID).ToList();

                    _core.Actors.Tiles.ForEach(o => o.QueueForDelete());
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

                if (insertedTile.Meta.CanStack == true && insertedTile.Meta.Quantity == 0)
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
                var directory = Path.GetFileName(d);
                if (directory.StartsWith("@") || directory.ToLower() == "player")
                {
                    continue;
                }

                materials.AddRange(EnumFlatMaterials(directory));
            }

            return materials;
        }

        private List<TileIdentifier> EnumFlatMaterials(string partialPath)
        {
            List<TileIdentifier> materials = new List<TileIdentifier>();

            foreach (string d in Directory.GetDirectories(Constants.BaseAssetPath + _partialTilesPath + partialPath))
            {
                var directory = Path.GetFileName(d);
                if (directory.StartsWith("@") || directory.ToLower() == "player")
                {
                    continue;
                }

                materials.AddRange(EnumFlatMaterials(partialPath + "\\" + directory));
            }

            foreach (var f in Directory.GetFiles(Constants.BaseAssetPath + _partialTilesPath + partialPath, "*.png"))
            {
                if (Path.GetFileName(f).StartsWith("@"))
                {
                    continue;
                }
                var file = new FileInfo(f);

                string tilePath = $"{_partialTilesPath}{partialPath}\\{Path.GetFileNameWithoutExtension(file.Name)}";

                var meta = TileMetadata.GetFreshMetadata(tilePath);

                materials.Add(new TileIdentifier(tilePath, meta));
            }

            return materials;
        }

        #region Menu Clicks.

        private void snapToGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _snapToGrid = !_snapToGrid;
            snapToGridToolStripMenuItem.Checked = _snapToGrid;

            if (_snapToGridRect != null)
            {
                drawingsurface.Invalidate(new Rectangle(
                    ((Rectangle)_snapToGridRect).X - 2,
                    ((Rectangle)_snapToGridRect).Y - 2,
                    ((Rectangle)_snapToGridRect).Width + 4,
                    ((Rectangle)_snapToGridRect).Height + 4));
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

        private void ToolStripButtonPlayMap_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentMapFilename))
            {
                _hasBeenModified = true;
            }

            if (CheckForNeededSave())
            {
                var gameApp = new System.Diagnostics.Process();
                gameApp.StartInfo.FileName = @"..\..\..\..\Game\bin\Debug\net5.0-windows\Game.exe";
                gameApp.StartInfo.Arguments = $"/levelfile:\"{_currentMapFilename}\" /levelindex:{_core.State.CurrentLevel}";
                gameApp.Start();
            }
        }

        private void ToolStripButtonMoveTileDown_Click(object sender, EventArgs e)
        {
            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
            foreach (var tile in selectedTiles)
            {
                tile.DrawOrder--;
            }
        }

        private void ToolStripButtonMoveTileUp_Click(object sender, EventArgs e)
        {
            var selectedTiles = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
            foreach (var tile in selectedTiles)
            {
                tile.DrawOrder++;
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

                    if (meta.SubType == ActorSubType.Weapon)
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

            double x = e.X + _core.Display.BackgroundOffset.X;
            double y = e.Y + _core.Display.BackgroundOffset.Y;

            var hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1)).OrderBy(o => o.DrawOrder).LastOrDefault();

            if (e.Button == MouseButtons.Middle)
            {
                dragStartOffset = new Point<double>(_core.Display.BackgroundOffset.X, _core.Display.BackgroundOffset.Y);
                dragStartMouse = new Point<double>(e.X, e.Y);
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
                double modX = e.X % 32;
                double modY = e.Y % 32;
                _snapToGridRect = new Rectangle((int)(e.X - modX), (int)(e.Y - modY), 32, 32);

                if (_snapToGrid && _snapToGridRect != null &&
                    (CurrentPrimaryMode == PrimaryMode.Insert || CurrentPrimaryMode == PrimaryMode.Shape))
                {
                    shapeInsertStartMousePosition = new Point<double>(((Rectangle)_snapToGridRect).X, ((Rectangle)_snapToGridRect).Y);
                }
                else
                {
                    shapeInsertStartMousePosition = new Point<double>(e.X, e.Y);
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
                    placeX = ((Rectangle)_snapToGridRect).X + 16;
                    placeY = ((Rectangle)_snapToGridRect).Y + 16;
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

            var hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1)).OrderBy(o => o.DrawOrder).LastOrDefault();

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

                e.Graphics.DrawRectangle(pen, (Rectangle)_snapToGridRect);
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
            double x = e.X + _core.Display.BackgroundOffset.X;
            double y = e.Y + _core.Display.BackgroundOffset.Y;

            _lastMouseLocation.X = (int)e.X;
            _lastMouseLocation.Y = (int)e.Y;

            if (_snapToGrid && (CurrentPrimaryMode == PrimaryMode.Insert || CurrentPrimaryMode == PrimaryMode.Shape))
            {
                double modX = e.X % 32;
                double modY = e.Y % 32;

                if (_snapToGridRect != null)
                {
                    drawingsurface.Invalidate(new Rectangle(
                        ((Rectangle)_snapToGridRect).X - 2,
                        ((Rectangle)_snapToGridRect).Y - 2,
                        ((Rectangle)_snapToGridRect).Width + 4,
                        ((Rectangle)_snapToGridRect).Height + 4));
                }

                _snapToGridRect = new Rectangle((int)(e.X - modX), (int)(e.Y - modY), 32, 32);

                drawingsurface.Invalidate(new Rectangle(
                    ((Rectangle)_snapToGridRect).X - 2,
                    ((Rectangle)_snapToGridRect).Y - 2,
                    ((Rectangle)_snapToGridRect).Width + 4,
                    ((Rectangle)_snapToGridRect).Height + 4));
            }

            toolStripStatusLabelMouseXY.Text = $"{x}x,{y}y";

            if (e.Button == MouseButtons.Middle)
            {
                _core.Display.BackgroundOffset.X = dragStartOffset.X - (e.X - dragStartMouse.X);
                _core.Display.BackgroundOffset.Y = dragStartOffset.Y - (e.Y - dragStartMouse.Y);
                _core.Display.DrawingSurface.Invalidate();
            }
            else
            {
                var hoverTile = _core.Actors.Intersections(new Point<double>(x, y),
                    new Point<double>(1, 1)).OrderBy(o => o.DrawOrder).LastOrDefault();

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
                        double modX = (e.X - shapeInsertStartMousePosition.X) % 32;
                        double modY = (e.Y - shapeInsertStartMousePosition.Y) % 32;

                        _shapeSelectionRect = GraphicsUtility.SortRectangle(new Rectangle(
                                            (int)shapeInsertStartMousePosition.X,
                                            (int)shapeInsertStartMousePosition.Y,
                                            ((int)(e.X - shapeInsertStartMousePosition.X) - (int)modX) + 32,
                                            ((int)(e.Y - shapeInsertStartMousePosition.Y) - (int)modY) + 32));
                    }
                    else
                    {
                        _shapeSelectionRect = GraphicsUtility.SortRectangle(new Rectangle(
                                            (int)shapeInsertStartMousePosition.X,
                                            (int)shapeInsertStartMousePosition.Y,
                                            (int)(e.X - shapeInsertStartMousePosition.X),
                                            (int)(e.Y - shapeInsertStartMousePosition.Y)));
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
                    double drawDeltaX = e.X - (drawLastLocation.X - _core.Display.BackgroundOffset.X);
                    double drawDeltaY = e.Y - (drawLastLocation.Y - _core.Display.BackgroundOffset.Y);

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
                                placeX = ((Rectangle)_snapToGridRect).X + 16;
                                placeY = ((Rectangle)_snapToGridRect).Y + 16;
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
                        int deltaX = (int)(lastHoverTile.X - x);
                        int deltaY = (int)(lastHoverTile.Y - y);

                        lastHoverTile.X = x;
                        lastHoverTile.Y = y;

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

            if (shapeFillMode == ShapeFillMode.Select && (IsShiftDown() || IsControlDown()))
            {
                double x = e.X + _core.Display.BackgroundOffset.X;
                double y = e.Y + _core.Display.BackgroundOffset.Y;

                var hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1)).OrderBy(o => o.DrawOrder).LastOrDefault();

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

            var hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1)).OrderBy(o => o.DrawOrder).LastOrDefault();

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
            _core.ResizeDrawingSurface(new Size(drawingsurface.Width, drawingsurface.Height));
        }

        #endregion

        #region ListViewProperties.

        private void listViewProperties_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                var selectedItems = _core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();

                if (selectedItems.Count != 1)
                {
                    return;
                }

                var selectedTile = selectedItems.First();

                listViewProperties.Tag = selectedTile;

                if (listViewProperties.SelectedItems?.Count > 0)
                {
                    var selectedRow = listViewProperties.SelectedItems[0];

                    if (selectedRow.Text == "Contents")
                    {
                        if (selectedTile.Meta?.IsContainer == true)
                        {
                            EditorContainer((Guid)selectedTile.Meta.UID);
                        }
                    }
                    else if (selectedRow.Text == "Warp to Tile")
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
                    else
                    {
                        using (var dialog = new FormTileProperties(selectedRow.Text, selectedRow.SubItems[1].Text.ToString()))
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

                                        selectedTile.X = x;
                                        selectedTile.Y = y;
                                    }
                                }
                                else if (selectedRow.Text == "Can Take Damage")
                                {
                                    bool result = false;

                                    if (bool.TryParse(dialog.PropertyValue, out result) == false)
                                    {
                                        result = int.Parse(dialog.PropertyValue) != 0;
                                    }

                                    selectedTile.Meta.CanTakeDamage = result;
                                }
                                else if (selectedRow.Text == "Can Walk On")
                                {
                                    bool result = false;

                                    if (bool.TryParse(dialog.PropertyValue, out result) == false)
                                    {
                                        result = int.Parse(dialog.PropertyValue) != 0;
                                    }

                                    selectedTile.Meta.CanWalkOn = result;
                                }
                                else if (selectedRow.Text == "Name")
                                {
                                    selectedTile.Meta.Name = dialog.PropertyValue;
                                }
                                else if (selectedRow.Text == "Min level")
                                {
                                    selectedTile.Meta.MinLevel = int.Parse(dialog.PropertyValue);
                                }
                                else if (selectedRow.Text == "Max level")
                                {
                                    selectedTile.Meta.MaxLevel = int.Parse(dialog.PropertyValue);
                                }
                                //else if (selectedRow.Text == "Spawn Type")
                                //{
                                //    selectedTile.Meta.SpawnType = dialog.PropertyValue;
                                //}
                                //else if (selectedRow.Text == "Spawn Sub-Type")
                                //{
                                //    selectedTile.Meta.SpawnSubType = dialog.PropertyValue;
                                //}
                                else if (selectedRow.Text == "Tag")
                                {
                                    selectedTile.Meta.Tag = dialog.PropertyValue;
                                }
                                else if (selectedRow.Text == "Hit Points")
                                {
                                    selectedTile.Meta.HitPoints = int.Parse(dialog.PropertyValue);
                                }
                                else if (selectedRow.Text == "Experience")
                                {
                                    selectedTile.Meta.Experience = int.Parse(dialog.PropertyValue);
                                }
                                else if (selectedRow.Text == "Quantity")
                                {
                                    selectedTile.Meta.Quantity = int.Parse(dialog.PropertyValue);
                                }
                                else if (selectedRow.Text == "Angle")
                                {
                                    selectedTile.Velocity.Angle.Degrees = double.Parse(dialog.PropertyValue);
                                }
                                else if (selectedRow.Text == "z-Order")
                                {
                                    selectedTile.DrawOrder = int.Parse(dialog.PropertyValue);
                                }
                                else if (selectedRow.Text == "Only Dialog Once")
                                {
                                    bool result = false;

                                    if (bool.TryParse(dialog.PropertyValue, out result) == false)
                                    {
                                        result = int.Parse(dialog.PropertyValue) != 0;
                                    }

                                    selectedTile.Meta.OnlyDialogOnce = result;
                                }
                                else if (selectedRow.Text == "Dialog")
                                {
                                    selectedTile.Meta.Dialog = dialog.PropertyValue;
                                    if (selectedTile.Meta.OnlyDialogOnce == null)
                                    {
                                        selectedTile.Meta.OnlyDialogOnce = true;
                                    }
                                }
                                else if (selectedRow.Text == "Armor Class")
                                {
                                    selectedTile.Meta.AC = int.Parse(dialog.PropertyValue);
                                }
                                else if (selectedRow.Text == "Damage Dice")
                                {
                                    selectedTile.Meta.DamageDice = int.Parse(dialog.PropertyValue);
                                }
                                else if (selectedRow.Text == "Damage Dice Faces")
                                {
                                    selectedTile.Meta.DamageDiceFaces = int.Parse(dialog.PropertyValue);
                                }
                                else if (selectedRow.Text == "Damage Additional")
                                {
                                    selectedTile.Meta.DamageAdditional = int.Parse(dialog.PropertyValue);
                                }
                                else if (selectedRow.Text == "Bulk")
                                {
                                    selectedTile.Meta.Bulk = int.Parse(dialog.PropertyValue);
                                }
                                else if (selectedRow.Text == "Bulk Capacity")
                                {
                                    selectedTile.Meta.BulkCapacity = int.Parse(dialog.PropertyValue);
                                }
                                else if (selectedRow.Text == "Weight Capacity")
                                {
                                    selectedTile.Meta.WeightCapacity = int.Parse(dialog.PropertyValue);
                                }

                                PopulateSelectedItemProperties();
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        void PopulateSelectedItemProperties()
        {
            listViewProperties.Items.Clear();

            var selectedTile = listViewProperties.Tag as ActorBase;

            if (selectedTile != null && selectedTile.Visible)
            {
                listViewProperties.Items.Add("Name").SubItems.Add(selectedTile.Meta?.Name);
                listViewProperties.Items.Add("Tile").SubItems.Add(selectedTile.TilePath);
                listViewProperties.Items.Add("Tag").SubItems.Add(selectedTile.Meta?.Tag);
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
                    //listViewProperties.Items.Add("Spawn Sub-Type").SubItems.Add(selectedTile.Meta.SpawnSubType.ToString());
                }

                if (selectedTile.Meta.ActorClass == ActorClassName.ActorFriendyBeing
                    || selectedTile.Meta.ActorClass == ActorClassName.ActorHostileBeing
                    || selectedTile.Meta.ActorClass == ActorClassName.ActorItem
                    || selectedTile.Meta.ActorClass == ActorClassName.ActorBuilding)
                {
                    listViewProperties.Items.Add("Dialog").SubItems.Add(selectedTile.Meta.Dialog);
                    listViewProperties.Items.Add("Only Dialog Once").SubItems.Add(selectedTile.Meta.OnlyDialogOnce.ToString());
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
                }

                if (selectedTile.Meta.SubType == ActorSubType.Armor || selectedTile.Meta.SubType == ActorSubType.Gauntlets
                    || selectedTile.Meta.SubType == ActorSubType.Helment || selectedTile.Meta.SubType == ActorSubType.Bracers
                    || selectedTile.Meta.SubType == ActorSubType.Boots || selectedTile.Meta.SubType == ActorSubType.Shield
                    || selectedTile.Meta.SubType == ActorSubType.Ring || selectedTile.Meta.SubType == ActorSubType.Garment
                    || selectedTile.Meta.SubType == ActorSubType.Belt || selectedTile.Meta.SubType == ActorSubType.Necklace)
                {
                    listViewProperties.Items.Add("Armor Class").SubItems.Add(selectedTile.Meta?.AC.ToString());
                }

                if (selectedTile.Meta.SubType == ActorSubType.Wand || selectedTile.Meta.SubType == ActorSubType.Weapon)
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

                listViewProperties.Items.Add("Location").SubItems.Add($"{selectedTile.Location.X},{selectedTile.Location.Y}");

                if (selectedTile.Meta?.IsContainer == true)
                {
                    listViewProperties.Items.Add("Contents").SubItems.Add("<open>");
                }
                if (selectedTile.Meta?.CanStack == true)
                {
                    listViewProperties.Items.Add("Quantity").SubItems.Add(selectedTile.Meta?.Quantity.ToString());
                }

                listViewProperties.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
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

                    double x = _core.Display.BackgroundOffset.X + 20;
                    double y = _core.Display.BackgroundOffset.Y + 20;

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

                        tile.Velocity.Angle.Degrees = chunk.Angle;
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
            _mostRecentlySelectedTile = _core.Actors.Tiles.FirstOrDefault(o=>o.SelectedHighlight == true);
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
    }
}

