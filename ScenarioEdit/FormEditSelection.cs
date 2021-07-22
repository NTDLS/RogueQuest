using Assets;
using Library.Engine;
using ScenarioEdit.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormEditSelection : Form
    {
        public EngineCore Core { get; set; }

        private bool _ignoreCheckedEvent = false;
        private ImageList _imageList = new ImageList();
        private ActorBase _previousHighlightedTile = null;
        private Button _buttonCancel = new Button();
        private bool _checkedByDefault = true;

        public FormEditSelection(EngineCore core)
        {
            InitializeComponent();

            Core = core;
        }

        public FormEditSelection()
        {
            InitializeComponent();
        }

        private void FormEditSelection_Load(object sender, EventArgs e)
        {
            CancelButton = _buttonCancel;
            treeViewTiles.ImageList = _imageList;

            _buttonCancel.Click += _ButtonCancel_Click;
            treeViewTiles.AfterSelect += TreeViewTiles_AfterSelect;
            FormClosed += FormEditSelection_FormClosed;

            var selections = Core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();

            if (selections.Count == 0)
            {
                selections = Core.Actors.Tiles;
                _checkedByDefault = false;
            }

            var groups = selections.GroupBy(o => o.TilePath).ToList();

            _ignoreCheckedEvent = true;

            TreeNode root = treeViewTiles.Nodes.Add("Selected Tiles");
            root.Checked = _checkedByDefault;

            foreach (var group in groups)
            {
                var groupNode = AddParsedNode(root, group.Key);

                foreach (var item in group)
                {
                    var node = groupNode.Nodes.Add(item.TilePath);
                    node.Tag = item;
                    node.Checked = _checkedByDefault;
                    node.ImageKey = GetImageKey(group.Key);
                    node.SelectedImageKey = GetImageKey(group.Key);
                }

                ExpandAllParents(groupNode.Parent);
            }

            root.Expand();

            _ignoreCheckedEvent = false;
        }

        void ExpandAllParents(TreeNode node)
        {
            while (node != null)
            {
                if (node.IsExpanded == false)
                {
                    node.Expand();
                }
                node = node.Parent;
            }
        }

        private TreeNode AddParsedNode(TreeNode root, string tilePath)
        {
            var tileParts = tilePath.Split("\\");

            TreeNode groupNode = root;

            string imageKey = GetImageKey(tilePath);

            foreach (var part in tileParts)
            {
                if (groupNode.Nodes.ContainsKey(part))
                {
                    groupNode = groupNode.Nodes[part];
                }
                else
                {
                    groupNode = groupNode.Nodes.Add(part, part);
                    groupNode.ImageKey = GetImageKey(imageKey);
                    groupNode.SelectedImageKey = GetImageKey(imageKey);
                    groupNode.Checked = _checkedByDefault;
                }
            }

            groupNode.Expand();

            return groupNode;
        }

        private void FormEditSelection_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_previousHighlightedTile != null)
            {
                _previousHighlightedTile.HoverHighlight = false;
                _previousHighlightedTile = null;
            }
        }

        private void TreeViewTiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var tile = e.Node.Tag as ActorBase;
            if (tile != null)
            {
                if (_previousHighlightedTile != null && _previousHighlightedTile != tile)
                {
                    _previousHighlightedTile.HoverHighlight = false;
                    _previousHighlightedTile = null;
                }

                tile.HoverHighlight = true;
                _previousHighlightedTile = tile;
            }
        }

        public string GetImageKey(string tilePath)
        {
            if (_imageList.Images.ContainsKey(tilePath))
            {
                return tilePath;
            }

            string imageFile = Constants.BaseAssetPath + @$"{tilePath}.png";

            _imageList.Images.Add(tilePath, SpriteCache.GetBitmapCached(imageFile));

            return tilePath;
        }

        private void _ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        void CheckNodesRecursive(TreeNode node)
        {
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode n in node.Nodes)
                {
                    n.Checked = node.Checked;

                    var tile = node.Tag as ActorBase;
                    if (tile != null)
                    {
                        tile.SelectedHighlight = n.Checked;
                    }

                    CheckNodesRecursive(n);
                }
            }
            else
            {
                var tile = node.Tag as ActorBase;
                if (tile != null)
                {
                    tile.SelectedHighlight = node.Checked;
                }
            }
        }

        private void treeViewTiles_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_ignoreCheckedEvent == false)
            {
                _ignoreCheckedEvent = true;
                CheckNodesRecursive(e.Node);
                _ignoreCheckedEvent = false;
            }
        }

        private void toolStripButtonMoveTilesUp_Click(object sender, EventArgs e)
        {
            var selectedTiles = Core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
            foreach (var tile in selectedTiles)
            {
                tile.DrawOrder = (tile.DrawOrder ?? 0) + 1;
            }
        }

        private void toolStripButtonMoveTilesDown_Click(object sender, EventArgs e)
        {
            var selectedTiles = Core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
            foreach (var tile in selectedTiles)
            {
                tile.DrawOrder = (tile.DrawOrder ?? 0) - 1;
            }
        }

        private void toolStripButtonDeleteTiles_Click(object sender, EventArgs e)
        {
            var selectedTiles = Core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
            if (MessageBox.Show($"Delete selected {selectedTiles.Count:N0} tiles?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (var tile in selectedTiles)
                {
                    tile.QueueForDelete();
                    Core.PurgeAllDeletedTiles();
                }
            }
        }

        private void toolStripButtonCopyToClipboard_Click(object sender, EventArgs e)
        {
            Singletons.ClipboardTiles.Clear();

            var selectedTiles = Core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
            if (selectedTiles.Count > 0)
            {
                Singletons.ClipboardTiles.AddRange(selectedTiles.Select(o => o.Clone()));
            }
        }
    }
}
