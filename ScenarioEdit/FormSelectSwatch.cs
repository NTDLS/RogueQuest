using Library.Engine;
using Library.Types;
using ScenarioEdit.Engine;
using ScenarioEdit.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormSelectSwatch : Form
    {
        private Point<double> dragStartOffset = new Point<double>();
        private Point<double> dragStartMouse = new Point<double>();

        private Control drawingsurface = new Control();
        private EngineCore _core;
        private bool _firstShown = true;
        private ImageList _assetBrowserImageList = new ImageList();

        public string SelectedSwatchFileName { get; set; }

        public FormSelectSwatch()
        {
            InitializeComponent();
        }

        private void FormSwatch_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.UserPaint, true);

            System.Reflection.PropertyInfo controlProperty = typeof(Control)
                    .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            controlProperty.SetValue(drawingsurface, true, null);

            this.Shown += FormSwatch_Shown;

            splitContainerBody.Panel1.Controls.Add(drawingsurface);
            drawingsurface.Dock = DockStyle.Fill;

            drawingsurface.BackColor = Color.FromArgb(60, 60, 60);
            drawingsurface.Paint += new PaintEventHandler(drawingsurface_Paint);

            treeViewSwatches.BeforeExpand += TreeViewSwatches_BeforeExpand;
            treeViewSwatches.MouseDoubleClick += TreeViewSwatches_MouseDoubleClick;
            treeViewSwatches.AfterSelect += TreeViewSwatches_AfterSelect;

            _core = new EngineCore(drawingsurface, new Size(drawingsurface.Width, drawingsurface.Height));

            drawingsurface.Select();
            drawingsurface.Focus();

            drawingsurface.MouseMove += Drawingsurface_MouseMove;
            drawingsurface.MouseDown += Drawingsurface_MouseDown;
            drawingsurface.MouseUp += Drawingsurface_MouseUp;

            PopulateSwatches();
        }

        private void Drawingsurface_MouseUp(object sender, MouseEventArgs e)
        {
            drawingsurface.Invalidate();
        }

        private void Drawingsurface_MouseDown(object sender, MouseEventArgs e)
        {
            drawingsurface.Select();
            drawingsurface.Focus();

            double ex = e.X;
            double ey = e.Y;

            if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                dragStartOffset = new Point<double>(_core.Display.BackgroundOffset.X, _core.Display.BackgroundOffset.Y);
                dragStartMouse = new Point<double>(ex, ey);
            }
        }

        private void Drawingsurface_MouseMove(object sender, MouseEventArgs e)
        {
            double ex = e.X;
            double ey = e.Y;

            if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                _core.Display.BackgroundOffset.X = dragStartOffset.X - (ex - dragStartMouse.X);
                _core.Display.BackgroundOffset.Y = dragStartOffset.Y - (ey - dragStartMouse.Y);
                _core.Display.DrawingSurface.Invalidate();
            }
        }

        private void TreeViewSwatches_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.ImageKey == "<swatch>")
            {
                string fileName = Assets.Constants.GetUserAssetPath($"Swatches\\{e.Node.FullPath}.rqs");

                SelectedSwatchFileName = fileName;

                _core.Reset();
                _core.Load(fileName);
                _core.PopCurrentLevel();

                textBoxInfo.Text = $"Loaded {_core.Actors.Tiles.Count:N0} tiles.";

                if (_core.Levels.FailedToLoadTilesCount != 0)
                {
                    textBoxInfo.Text = $"Failed to load {_core.Levels.FailedToLoadTilesCount:N0} tiles.\r\nWarnings:\r\n" +
                        String.Join("\r\n", _core.Levels.LoadErrors);
                }
            }
        }

        private void TreeViewSwatches_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SelectedSwatchFileName) == false && treeViewSwatches.SelectedNode != null)
            {
                if (treeViewSwatches.SelectedNode.ImageKey != " <folder>")
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void TreeViewSwatches_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "<dummy>")
            {
                e.Node.Nodes.Clear();
                PopChildNodes(e.Node, e.Node.FullPath);
            }
        }

        private void PopulateSwatches()
        {
            _assetBrowserImageList.Images.Add("<folder>", Resources.SwatchTreeView_Folder);
            _assetBrowserImageList.Images.Add("<swatch>", Resources.SwatchTreeView_Swatch);

            treeViewSwatches.ImageList = _assetBrowserImageList;

            foreach (string d in Directory.GetDirectories(Assets.Constants.GetUserAssetPath($"Swatches")))
            {
                if (Utility.IgnoreFileName(d))
                {
                    continue;
                }

                var directory = Path.GetFileName(d);
                var directoryNode = treeViewSwatches.Nodes.Add(directory, directory, "<folder>");
                directoryNode.Nodes.Add("<dummy>");
            }
        }

        public void PopChildNodes(TreeNode parent, string partialPath)
        {
            foreach (string d in Directory.GetDirectories(Assets.Constants.GetUserAssetPath($"Swatches\\{partialPath}")))
            {
                if (Utility.IgnoreFileName(d))
                {
                    continue;
                }

                var directory = Path.GetFileName(d);
                var directoryNode = parent.Nodes.Add(directory, directory, "<folder>");
                directoryNode.Nodes.Add("<dummy>");
            }

            foreach (var f in Directory.GetFiles(Assets.Constants.GetUserAssetPath($"Swatches\\{partialPath}"), "*.rqs"))
            {
                if (Utility.IgnoreFileName(f))
                {
                    continue;
                }
                var file = new FileInfo(f);

                string fileKey = $"{partialPath}\\{Path.GetFileNameWithoutExtension(file.Name)}";

                parent.Nodes.Add(fileKey, Path.GetFileNameWithoutExtension(file.Name), "<swatch>", "<swatch>");
            }
        }

        private void FormSwatch_Shown(object sender, EventArgs e)
        {
            if (_firstShown)
            {
                _firstShown = false;
            }
        }

        private void drawingsurface_Paint(object sender, PaintEventArgs e)
        {
            var image = _core.Render();
            e.Graphics.DrawImage(image, 0, 0);
        }

        private void FormSelectSwatch_SizeChanged(object sender, EventArgs e)
        {
            if (_core != null)
            {
                _core.ResizeDrawingSurface(new Size(drawingsurface.Width, drawingsurface.Height));
            }
        }
    }
}
