using Assets;
using LevelEditor.Engine;
using Library.Engine;
using Library.Types;
using Library.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelEditor
{
    public partial class FormMain : Form
    {
        /// <summary>
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
            Delete
        }

        private EngineCore _core;
        private bool _fullScreen = false;
        private bool _hasBeenModified = false;
        private string _currentMapFilename = string.Empty;
        private int _newFilenameIncrement = 1;

        #region Settings.

        private int tilePaintOverlap = 5;
        private bool highlightSelectedTile = true;
        private bool highlightHoverTile = true;

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
        private TerrainBase selectedTile = null;
        private TerrainBase previousHoverTile = null;
        private PrimaryMode CurrentPrimaryMode { get; set; } = PrimaryMode.Insert;

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
            _core = new EngineCore(pictureBox, new Size(pictureBox.Width, pictureBox.Height));

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

            pictureBox.BackColor = Color.FromArgb(60, 60, 60);

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

            PopulateMaterials();

            ToolStripButtonInsertMode_Click(new object(), new EventArgs());

            MapPersistence.Load(_core, Assets.Constants.GetAssetPath(@"Maps\Test.rqm"), true);
        }

        #region Menu Clicks.

        private void ToolStripButtonMoveTileDown_Click(object sender, EventArgs e)
        {
            if (selectedTile != null)
            {
                selectedTile.DrawOrder--;
                PopulateSelectedItemProperties();
            }
        }

        private void ToolStripButtonMoveTileUp_Click(object sender, EventArgs e)
        {
            if (selectedTile != null)
            {
                selectedTile.DrawOrder++;
                PopulateSelectedItemProperties();
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
                dialog.Filter = "RogueQuest Maps (*.qrm)|*.rqm|All files (*.*)|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _currentMapFilename = dialog.FileName;
                    MapPersistence.Load(_core, _currentMapFilename);
                    _hasBeenModified = false;
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //If we already have an open file, then just save it.
            if (string.IsNullOrWhiteSpace(_currentMapFilename) == false)
            {
                MapPersistence.Save(_core, _currentMapFilename);
                _hasBeenModified = false;
            }
            else //If we do not have a current open file, then we need to "Save As".
            {
                TrySave();
            }
        }

        bool TrySave()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.FileName = $"Newfile {_newFilenameIncrement++}";
                dialog.Filter = "RQ Map (*.qrm)|*.rqm|All files (*.*)|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _currentMapFilename = dialog.FileName;

                    MapPersistence.Save(_core, _currentMapFilename);
                    _hasBeenModified = false;
                    return true;
                }
            }
            return false;
        }


        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrySave();
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckForNeededSave() == false)
            {
                return;
            }

            _core.QueueAllForDelete();
            _core.Display.BackgroundOffset = new Point<double>();
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

            if (selectedTile != null) selectedTile.SelectedHighlight = false;
        }

        private void ToolStripButtonSelectMode_Click(object sender, EventArgs e)
        {
            CurrentPrimaryMode = PrimaryMode.Select;
            toolStripButtonInsertMode.Checked = false;
            toolStripButtonSelectMode.Checked = true;
            toolStripButtonShapeMode.Checked = false;

            if (selectedTile != null) selectedTile.SelectedHighlight = false;
        }

        private void ToolStripButtonInsertMode_Click(object sender, EventArgs e)
        {
            CurrentPrimaryMode = PrimaryMode.Insert;
            toolStripButtonInsertMode.Checked = true;
            toolStripButtonSelectMode.Checked = false;
            toolStripButtonShapeMode.Checked = false;

            if(selectedTile != null) selectedTile.SelectedHighlight = false;
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
        }

        #endregion

        public TreeNode CreateImageListAndAssets(ImageList imageList, TreeNode parent, string basePath, string partialPath)
        {
            TreeNode node = null;

            if (parent == null)
            {
                node = treeViewTiles.Nodes.Add(Path.GetFileName(partialPath));
            }
            else
            {
                node = parent.Nodes.Add(Path.GetFileName(partialPath));
            }

            foreach (var f in Directory.GetFiles(basePath + partialPath, "*.png"))
            {
                if (Path.GetFileName(f).StartsWith("@"))
                {
                    continue;
                }
                var file = new FileInfo(f);

                string fileKey = $"{partialPath}\\{Path.GetFileNameWithoutExtension(file.Name)}";

                imageList.Images.Add(fileKey, SpriteCache.GetBitmapCached(file.FullName));

                node.Nodes.Add(fileKey, Path.GetFileNameWithoutExtension(file.Name), fileKey, fileKey);
            }
            foreach (string d in Directory.GetDirectories(basePath + partialPath))
            {
                var directory = Path.GetFileName(d);
                if (directory.StartsWith("@"))
                {
                    continue;
                }
                var addedNode = CreateImageListAndAssets(imageList, node, basePath, partialPath + "\\" + directory);

                //Set the folder image to the first image in the children.
                TreeNode imageFind = addedNode;
                while (String.IsNullOrWhiteSpace(imageFind.ImageKey))
                {
                    if (imageFind.Nodes.Count > 0)
                    {
                        imageFind = imageFind.Nodes[0];
                    }
                    else
                    {
                        break;
                    }
                }
                if (imageFind != null)
                {
                    addedNode.ImageKey = imageFind.ImageKey;
                    addedNode.SelectedImageKey = imageFind.SelectedImageKey;
                }
            }

            return node;
        }

        void PopulateMaterials()
        {
            ImageList imageList = new ImageList();
            treeViewTiles.ImageList = imageList;
            CreateImageListAndAssets(imageList, null, Assets.Constants.BasePath, "Terrain");
            if (treeViewTiles.Nodes.Count > 0)
            {
                treeViewTiles.Nodes[0].Expand();
            }
        }

        Rectangle? shapeSelectionRect = null;


        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            double x = e.X + _core.Display.BackgroundOffset.X;
            double y = e.Y + _core.Display.BackgroundOffset.Y;

            toolStripStatusLabelMouseXY.Text = $"{x}x,{y}y";

            if (e.Button == MouseButtons.Middle)
            {
                _core.Display.BackgroundOffset.X = dragStartOffset.X - (e.X - dragStartMouse.X);
                _core.Display.BackgroundOffset.Y = dragStartOffset.Y - (e.Y - dragStartMouse.Y);
                _core.Display.DrawingSurface.Invalidate();
            }
            else
            {
                var hoverTile = _core.Terrain.Intersections(new Point<double>(x, y),
                    new Point<double>(1, 1)).OrderBy(o => o.DrawOrder).LastOrDefault();

                if (e.Button != MouseButtons.Left) //Dont change the selection based on hover location while dragging.
                {
                    if (previousHoverTile != null && highlightHoverTile)
                    {
                        previousHoverTile.HoverHighlight = false;
                    }

                    if (hoverTile != null)
                    {
                        toolStripStatusLabelHoverObject.Text = $"[{hoverTile.TileTypeKey}]";

                        if (highlightHoverTile && CurrentPrimaryMode != PrimaryMode.Shape)
                        {
                            hoverTile.HoverHighlight = true;
                        }

                        previousHoverTile = hoverTile;
                    }
                    else
                    {
                        toolStripStatusLabelHoverObject.Text = "";
                    }
                }

                if (CurrentPrimaryMode == PrimaryMode.Shape)
                {
                    if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                    {
                        shapeSelectionRect = GraphicsUtility.SortRectangle(new Rectangle(
                                            (int)shapeInsertStartMousePosition.X,
                                            (int)shapeInsertStartMousePosition.Y,
                                            (int)(e.X - shapeInsertStartMousePosition.X),
                                            (int)(e.Y - shapeInsertStartMousePosition.Y)));

                        var rc = (Rectangle)shapeSelectionRect;
                        pictureBox.Invalidate();
                        toolStripStatusLabelDebug.Text = $"{rc.X}x,{rc.Y}y->{rc.Width}x,{rc.Height}y";
                    }

                    if (e.Button == MouseButtons.Left)
                    {
                        shapeFillMode = ShapeFillMode.Insert;
                    }
                    if (e.Button == MouseButtons.Right)
                    {
                        shapeFillMode = ShapeFillMode.Delete;
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
                            PlaceSelectedItem(x, y);
                        }
                    }
                }

                //Drag item.
                if (e.Button == MouseButtons.Left && CurrentPrimaryMode == PrimaryMode.Select)
                {
                    if (selectedTile == null)
                    {
                        selectedTile = hoverTile;
                    }

                    if (selectedTile != null)
                    {
                        selectedTile.X = x;
                        selectedTile.Y = y;
                        _hasBeenModified = true;
                    }
                }

                //Paint deletion with right button.
                if (e.Button == MouseButtons.Right && CurrentPrimaryMode == PrimaryMode.Insert)
                {
                    if (hoverTile != null)
                    {
                        hoverTile.QueueForDelete();
                        _hasBeenModified = true;
                    }
                }
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            PopulateSelectedItemProperties();

            if (CurrentPrimaryMode == PrimaryMode.Shape && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) && shapeSelectionRect != null)
            {
                if (shapeFillMode == ShapeFillMode.Insert)
                {
                    var rc = (Rectangle)shapeSelectionRect;
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
                                    shapeSelectionRect = null;
                                    pictureBox.Invalidate();
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
                    var rc = (Rectangle)shapeSelectionRect;
                    double x = rc.X + _core.Display.BackgroundOffset.X;
                    double y = rc.Y + _core.Display.BackgroundOffset.Y;

                    var intersections = _core.Terrain.Intersections(x, y, rc.Width, rc.Height);

                    foreach (var obj in intersections)
                    {
                        obj.QueueForDelete();
                    }
                }

                shapeSelectionRect = null;
                pictureBox.Invalidate();
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            double x = e.X + _core.Display.BackgroundOffset.X;
            double y = e.Y + _core.Display.BackgroundOffset.Y;

            var hoverTile = _core.Terrain.Intersections(new Point<double>(x, y), new Point<double>(1, 1)).OrderBy(o => o.DrawOrder).LastOrDefault();

            if (e.Button == MouseButtons.Middle)
            {
                dragStartOffset = new Point<double>(_core.Display.BackgroundOffset.X, _core.Display.BackgroundOffset.Y);
                dragStartMouse = new Point<double>(e.X, e.Y);
            }

            if (CurrentPrimaryMode == PrimaryMode.Shape && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right))
            {
                shapeInsertStartMousePosition = new Point<double>(e.X, e.Y);
            }

            if (e.Button == MouseButtons.Left && CurrentPrimaryMode == PrimaryMode.Insert)
            {
                //Single item placement with left button.
                insertStartMousePosition = new Point<double>(e.X, e.Y);
                PlaceSelectedItem(x, y);
            }

            if (e.Button == MouseButtons.Left && CurrentPrimaryMode == PrimaryMode.Select)
            {
                if (selectedTile != hoverTile)
                {
                    if (selectedTile != null && highlightSelectedTile)
                    {
                        selectedTile.SelectedHighlight = false;
                    }

                    selectedTile = hoverTile;
                    PopulateSelectedItemProperties();
                }

                if (selectedTile != null && highlightSelectedTile)
                {
                    selectedTile.SelectedHighlight = true;
                }
            }
        }

        void PopulateSelectedItemProperties()
        {
            listViewProperties.Items.Clear();

            if (selectedTile != null && selectedTile.Visible)
            {
                listViewProperties.Items.Add("Type").SubItems.Add(selectedTile.TileTypeKey).Tag = new TPTag { ReadOnly = true };

                listViewProperties.Items.Add("Location").SubItems.Add(
                    $"{selectedTile.Location.X},{selectedTile.Location.Y}").Tag = new TPTag { ReadOnly = false };

                listViewProperties.Items.Add("Angle").SubItems.Add(selectedTile.Angle.Degrees.ToString()).Tag = new TPTag { ReadOnly = false };
                listViewProperties.Items.Add("z-Order").SubItems.Add(selectedTile.DrawOrder.ToString()).Tag = new TPTag { ReadOnly = false };
                listViewProperties.Items.Add("Size").SubItems.Add(selectedTile.Size.ToString()).Tag = new TPTag { ReadOnly = true };
                listViewProperties.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        private void listViewProperties_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (listViewProperties.SelectedItems?.Count > 0)
                {
                    var selectedRow = listViewProperties.SelectedItems[0];

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
                            if (selectedRow.Text == "Angle")
                            {
                                var degrees = double.Parse(dialog.PropertyValue);
                                selectedTile.Angle.Degrees = degrees;
                            }
                            if (selectedRow.Text == "z-Order")
                            {
                                var order = int.Parse(dialog.PropertyValue);
                                selectedTile.DrawOrder = order;
                            }

                            PopulateSelectedItemProperties();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public class TPTag //Tile properties tag
        {
            public bool ReadOnly { get; set; } = true;
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            double x = e.X + _core.Display.BackgroundOffset.X;
            double y = e.Y + _core.Display.BackgroundOffset.Y;

            var hoverTile = _core.Terrain.Intersections(new Point<double>(x, y), new Point<double>(1, 1)).OrderBy(o => o.DrawOrder).LastOrDefault();

            //Single item deletion with right button.
            if (e.Button == MouseButtons.Right && CurrentPrimaryMode == PrimaryMode.Insert)
            {
                if(hoverTile != null)
                {
                    hoverTile.QueueForDelete();
                    _hasBeenModified = true;
                }
            }
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


        private TerrainBase PlaceSelectedItem(double x, double y)
        {
            TerrainBase insertedTile = null;

            _hasBeenModified = true;

            if (treeViewTiles.SelectedNode == null)
            {
                return null;
            }

            var selectedItem = GetRandomChildNode(treeViewTiles.SelectedNode);
            if (selectedItem != null)
            {
                insertedTile = _core.Terrain.AddNew<TerrainBase>(x, y, selectedItem.FullPath);
                insertedTile.RefreshMetadata();
                lastPlacedItemSize = insertedTile.Size;
                drawLastLocation = new Point<double>(x, y);
            }

            return insertedTile;
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            var image = _core.Render();

            e.Graphics.DrawImage(image, 0, 0);

            if (shapeSelectionRect != null)
            {
                Pen pen = new Pen(Color.Yellow, 2);

                if (shapeFillMode == ShapeFillMode.Delete)
                {
                    pen = new Pen(Color.Red, 2);
                }

                e.Graphics.DrawRectangle(pen, (Rectangle)shapeSelectionRect);
            }
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            _core.ResizeDrawingSurface(new Size(pictureBox.Width, pictureBox.Height));
        }
    }
}
