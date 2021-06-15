using Assets;
using LevelEditor.Engine;
using Library.Engine;
using Library.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

            PopulateMaterials();

            toolStripStatusLabelPrimaryMode.Text = $"Mode: {CurrentPrimaryMode.ToString()}";

            MapPersistence.Load(_core, @"C:\\map.txt");
        }

        #region Menu Clicks.
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapPersistence.Save(_core, @"C:\\map.txt");
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

        #endregion

        void PopulateMaterials()
        {
            ImageList imageList = new ImageList();

            imageList.Images.Add(@"Terrain\BoderTree", SpriteCache.GetBitmapCached(@"Terrain\BoderTree.png"));
            imageList.Images.Add(@"Terrain\Bush", SpriteCache.GetBitmapCached(@"Terrain\Bush.png"));
            imageList.Images.Add(@"Terrain\Dirt", SpriteCache.GetBitmapCached(@"Terrain\Dirt.png"));
            imageList.Images.Add(@"Terrain\FallenTreeStump", SpriteCache.GetBitmapCached(@"Terrain\FallenTreeStump.png"));
            imageList.Images.Add(@"Terrain\Flowers", SpriteCache.GetBitmapCached(@"Terrain\Flowers.png"));
            imageList.Images.Add(@"Terrain\RiverFlowFromBottomToRight", SpriteCache.GetBitmapCached(@"Terrain\RiverFlowFromBottomToRight.png"));
            imageList.Images.Add(@"Terrain\RiverFlowFromLeftToBottom", SpriteCache.GetBitmapCached(@"Terrain\RiverFlowFromLeftToBottom.png"));
            imageList.Images.Add(@"Terrain\RiverFlowFromLeftToRight", SpriteCache.GetBitmapCached(@"Terrain\RiverFlowFromLeftToRight.png"));
            imageList.Images.Add(@"Terrain\RiverFlowFromLeftToTop", SpriteCache.GetBitmapCached(@"Terrain\RiverFlowFromLeftToTop.png"));
            imageList.Images.Add(@"Terrain\RiverFlowFromToBottom", SpriteCache.GetBitmapCached(@"Terrain\RiverFlowFromToBottom.png"));
            imageList.Images.Add(@"Terrain\RiverFlowFromTopToRight", SpriteCache.GetBitmapCached(@"Terrain\RiverFlowFromTopToRight.png"));
            imageList.Images.Add(@"Terrain\ShortTree", SpriteCache.GetBitmapCached(@"Terrain\ShortTree.png"));
            imageList.Images.Add(@"Terrain\SmallEntrance", SpriteCache.GetBitmapCached(@"Terrain\SmallEntrance.png"));
            imageList.Images.Add(@"Terrain\TallTree", SpriteCache.GetBitmapCached(@"Terrain\TallTree.png"));
            imageList.Images.Add(@"Terrain\TreeStump", SpriteCache.GetBitmapCached(@"Terrain\TreeStump.png"));
            imageList.Images.Add(@"Terrain\WaterPuddle", SpriteCache.GetBitmapCached(@"Terrain\WaterPuddle.png"));
            imageList.Images.Add(@"Terrain\WideTree", SpriteCache.GetBitmapCached(@"Terrain\WideTree.png"));

            listViewTiles.LargeImageList = imageList;
            listViewTiles.SmallImageList = imageList;
            listViewTiles.View = View.Details;

            listViewTiles.Columns.Add("Item");
            listViewTiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            listViewTiles.HideSelection = false;
            listViewTiles.Columns[0].Width = 250;

            foreach (var key in imageList.Images.Keys)
            {
                listViewTiles.Items.Add(key, key);

                if (key == @"Terrain\Dirt")
                {
                    listViewTiles.Items[listViewTiles.Items.Count - 1].Selected = true;
                }
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

            var hoverObjects = _core.TerrainIntersections(new Point<double>(x, y), new Point<double>(1, 1));
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
                }
            }

            //Paint deletion with right button.
            if (e.Button == MouseButtons.Right)
            {
                if (singleHoverItem != null)
                {
                    singleHoverItem.QueueForDelete();
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

                var objs = _core.TerrainIntersections(new Point<double>(x, y), new Point<double>(1, 1));
                foreach (var obj in objs)
                {
                    obj.QueueForDelete();
                }
            }
        }

        void PlaceSelectedItem(double x, double y)
        {
            if (listViewTiles.SelectedItems?.Count != 1)
            {
                return;
            }

            var selectedItem = listViewTiles.SelectedItems[0];

            _core.AddNewTerrain<TerrainBase>(x, y, selectedItem.ImageKey);

            drawLastLocation = new Point<double>(x, y);
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_core.Render(), 0, 0);
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //e.ClickedItem
        }
    }
}
