using Assets;
using Library.Engine;
using Library.Engine.Types;
using ScenarioEdit.Engine;
using ScenarioEdit.Properties;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormSelectLevelTile : Form
    {
        public string LevelName { get; set; }
        public LevelChunk SelectedTile { get; set; }

        private ActorClassName[] _tileTypesToShow;
        private ImageList _imageList = new ImageList();

        public EngineCore Core { get; set; }

        public FormSelectLevelTile()
        {
            InitializeComponent();
        }

        public FormSelectLevelTile(EngineCore core, ActorClassName[] tileTypesToShow)
        {
            InitializeComponent();
            Core = core;
            _tileTypesToShow = tileTypesToShow;
        }

        private void FormSelectLevelTile_Load(object sender, EventArgs e)
        {
            this.AcceptButton = buttonOk;
            this.CancelButton = buttonCancel;

            treeViewTiles.BeforeExpand += TreeViewTiles_BeforeExpand;
            treeViewTiles.NodeMouseDoubleClick += TreeViewTiles_NodeMouseDoubleClick;
            treeViewTiles.ImageList = _imageList;

            PopulateLevels();
        }

        private void TreeViewTiles_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.Tag is LevelChunk)
            {
                LevelName = e.Node.Parent.Text;
                SelectedTile = e.Node.Tag as LevelChunk;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void TreeViewTiles_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes?.Count == 1 && e.Node.Nodes[0].Text == "<dummy>")
            {
                e.Node.Nodes.Clear();

                int levelIndex = Core.Levels.GetIndex(e.Node.Text);
                if (levelIndex >= 0)
                {
                    var chunks = Core.Levels.GetChunks(levelIndex);
                    var desiredTiles = chunks.Where(o => o.Meta?.ActorClass != null && _tileTypesToShow.Contains((ActorClassName)o.Meta?.ActorClass));

                    foreach (var chunk in desiredTiles)
                    {
                        if (_imageList.Images.ContainsKey(chunk.TilePath) == false)
                        {
                            var fullTilePath = Path.Combine(Constants.BaseCommonAssetPath, $"{chunk.TilePath}.png");
                            _imageList.Images.Add(chunk.TilePath, SpriteCache.GetBitmapCached(fullTilePath));
                        }

                        var node = e.Node.Nodes.Add(chunk.Meta.Name);
                        node.Tag = chunk;
                        node.SelectedImageKey = chunk.TilePath;
                        node.ImageKey = chunk.TilePath;
                    }
                }
            }
        }

        private void PopulateLevels()
        {

            if (_imageList.Images.ContainsKey("<Folder>") == false)
            {
                _imageList.Images.Add("<Folder>", Resources.SwatchTreeView_Folder);
            }

            treeViewTiles.Nodes.Clear();
            foreach (var level in Core.Levels.Collection)
            {
                var parentNode = treeViewTiles.Nodes.Add(level.Name, level.Name, "<Folder>", "<Folder>");
                parentNode.Nodes.Add("<dummy>");
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (treeViewTiles.SelectedNode != null && treeViewTiles.SelectedNode.Tag is LevelChunk)
            {
                LevelName = treeViewTiles.SelectedNode.Parent.Text;
                SelectedTile = treeViewTiles.SelectedNode.Tag as LevelChunk;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
