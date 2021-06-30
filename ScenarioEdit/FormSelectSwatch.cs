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
        private string BaseAssetPath => Assets.Constants.BaseAssetPath;
        private string PartialSwatchPath => "Swatches\\";
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

            this.Shown += FormSwatch_Shown; ;

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

            PopulateSwatches();
        }

        private void TreeViewSwatches_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.ImageKey == "<swatch>")
            {
                string fileName = $"{BaseAssetPath}{PartialSwatchPath}\\{e.Node.FullPath}.rqs";

                SelectedSwatchFileName = fileName;

                _core.Reset();
                _core.Load(fileName);
                _core.PopCurrentLevel();
            }
        }

        private void TreeViewSwatches_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SelectedSwatchFileName) == false)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
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

            foreach (string d in Directory.GetDirectories(BaseAssetPath + PartialSwatchPath))
            {
                var directory = Path.GetFileName(d);
                if (directory.StartsWith("@") || directory.ToLower() == "player")
                {
                    continue;
                }

                var directoryNode = treeViewSwatches.Nodes.Add(PartialSwatchPath + directory, directory, "<folder>");
                directoryNode.Nodes.Add("<dummy>");
            }
        }

        public void PopChildNodes(TreeNode parent, string partialPath)
        {
            foreach (string d in Directory.GetDirectories(BaseAssetPath + PartialSwatchPath + partialPath))
            {
                var directory = Path.GetFileName(d);
                if (directory.StartsWith("@") || directory.ToLower() == "player")
                {
                    continue;
                }

                var directoryNode = parent.Nodes.Add(PartialSwatchPath + directory, directory, "<folder>");
                directoryNode.Nodes.Add("<dummy>");
            }

            foreach (var f in Directory.GetFiles(BaseAssetPath + PartialSwatchPath + partialPath, "*.rqs"))
            {
                if (Path.GetFileName(f).StartsWith("@"))
                {
                    continue;
                }
                var file = new FileInfo(f);

                string fileKey = $"{PartialSwatchPath}{partialPath}\\{Path.GetFileNameWithoutExtension(file.Name)}";

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
    }
}
