﻿using Assets;
using LevelEditor.Engine;
using Library.Engine;
using Library.Types;
using Library.Utility;
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
            Select
        }

        private EngineCore _core;
        private bool _fullScreen = false;
        private bool _hasBeenModified = false;
        private string _currentMapFilename = string.Empty;
        private int _newFilenameIncrement = 1;

        /// <summary>
        /// Where the mouse was when the user started dragging the map.
        /// </summary>
        Point<double> dragStartMouse = new Point<double>();
        /// <summary>
        /// What the background offset was then the user started dragging the map.
        /// </summary>
        Point<double> dragStartOffset = new Point<double>();
        /// <summary>
        /// Where the mouse was when the user started drawing
        /// </summary>
        Point<double> drawStartMouse = new Point<double>();
        /// <summary>
        /// The location that the last block was placed (real world location).
        /// </summary>
        Point<double> drawLastLocation = new Point<double>();
        public PrimaryMode CurrentPrimaryMode { get; set; } = PrimaryMode.Insert;

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

            PopulateMaterials();

            toolStripStatusLabelPrimaryMode.Text = $"Mode: {CurrentPrimaryMode.ToString()}";

            //MapPersistence.Load(_core, Assets.Constants.GetAssetPath(@"Maps\Meadow.rqm"));
        }

        #region Menu Clicks.

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
                    SaveToolStripMenuItem_Click(new object(), new EventArgs());
                }
                else if (result == DialogResult.Cancel)
                {
                    return false;
                }
            }

            return true;
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
                SaveAsToolStripMenuItem_Click(sender, e);
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
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
                }
            }
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckForNeededSave() == false)
            {
                return;
            }

            _core.QueueAllForDelete();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckForNeededSave() == false)
            {
                return;
            }
        }

        private void ToolStripButtonSelectMode_Click(object sender, EventArgs e)
        {
            CurrentPrimaryMode = PrimaryMode.Select;
            toolStripStatusLabelPrimaryMode.Text = $"Mode: {CurrentPrimaryMode.ToString()}";
        }

        private void ToolStripButtonInsertMode_Click(object sender, EventArgs e)
        {
            CurrentPrimaryMode = PrimaryMode.Insert;
            toolStripStatusLabelPrimaryMode.Text = $"Mode: {CurrentPrimaryMode.ToString()}";
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

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                _core.Display.BackgroundOffset.X = dragStartOffset.X - dragStartMouse.X;
                _core.Display.BackgroundOffset.Y = dragStartOffset.Y - dragStartMouse.Y;
            }

            double x = _core.Display.BackgroundOffset.X + e.X;
            double y = _core.Display.BackgroundOffset.Y + e.Y;

            toolStripStatusLabelMouseXY.Text = $"Mouse: x{e.X},y{e.Y} World: x{x},y{y}";

            var hoverObjects = _core.Terrain.Intersections(new Point<double>(x, y), new Point<double>(1, 1));
            var singleHoverItem = hoverObjects?.LastOrDefault();

            if (hoverObjects.Count > 0)
            {
                var firstObj = hoverObjects.Last();
                toolStripStatusLabelHoverObject.Text = $"[{firstObj.TileTypeKey}]";
            }
            else
            {
                toolStripStatusLabelHoverObject.Text = "";
            }

            //Paint with left button.
            if (e.Button == MouseButtons.Left && CurrentPrimaryMode == PrimaryMode.Insert)
            {
                double drawDeltaX = e.X - drawLastLocation.X;
                double drawDeltaY = e.Y - drawLastLocation.Y;

                if (Math.Abs(drawDeltaX) > 10 || Math.Abs(drawDeltaY) > 10)
                {
                    PlaceSelectedItem(x, y);
                }
            }

            //Drag item.
            if (e.Button == MouseButtons.Left && CurrentPrimaryMode == PrimaryMode.Select)
            {
                if (singleHoverItem != null)
                {
                    singleHoverItem.X = x;
                    singleHoverItem.Y = y;
                    _hasBeenModified = true;
                }
            }

            //Paint deletion with right button.
            if (e.Button == MouseButtons.Right)
            {
                if (singleHoverItem != null)
                {
                    singleHoverItem.QueueForDelete();
                    _hasBeenModified = true;
                }
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                dragStartMouse = new Point<double>(e.X, e.Y);
                dragStartOffset = new Point<double>(_core.Display.BackgroundOffset);
            }

            if (CurrentPrimaryMode == PrimaryMode.Insert)
            {
                //Single item placement with left button.
                if (e.Button == MouseButtons.Left)
                {
                    drawStartMouse = new Point<double>(e.X, e.Y);

                    double x = _core.Display.BackgroundOffset.X + e.X;
                    double y = _core.Display.BackgroundOffset.Y + e.Y;
                    PlaceSelectedItem(x, y);
                }
            }

            //Single item deletion with right button.
            if (e.Button == MouseButtons.Right)
            {
                double x = _core.Display.BackgroundOffset.X + e.X;
                double y = _core.Display.BackgroundOffset.Y + e.Y;

                var objs = _core.Terrain.Intersections(new Point<double>(x, y), new Point<double>(1, 1));
                foreach (var obj in objs)
                {
                    obj.QueueForDelete();
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
            }

            return null;
        }

        void PlaceSelectedItem(double x, double y)
        {
            _hasBeenModified = true;

            if (treeViewTiles.SelectedNode == null)
            {
                return;
            }

            var selectedItem = GetRandomChildNode(treeViewTiles.SelectedNode);
            if (selectedItem != null)
            {
                _core.Terrain.AddNew<TerrainBase>(x, y, selectedItem.FullPath);
                drawLastLocation = new Point<double>(x, y);
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_core.Render(), 0, 0);
        }
    }
}
