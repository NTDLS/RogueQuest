using Assets;
using Library.Engine;
using ScenarioEdit.Engine;
using ScenarioEdit.Properties;
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
    public partial class FormEditContainer : Form
    {
        const int QTY_COLUMN = 1;

        public EngineCore Core { get; set; }
        public Guid ContainerId { get; set; }

        public FormEditContainer(EngineCore core, Guid containerId)
        {
            InitializeComponent();

            ContainerId = containerId;
            Core = core;
        }

        public FormEditContainer()
        {
            InitializeComponent();
        }

        private void FormEditContainer_Load(object sender, EventArgs e)
        {
            PopulateMaterials();

            listViewContainer.LargeImageList = treeViewTiles.ImageList;
            listViewContainer.SmallImageList = treeViewTiles.ImageList;

            treeViewTiles.DragEnter += TreeViewTiles_DragEnter;
            treeViewTiles.ItemDrag += TreeViewTiles_ItemDrag;

            listViewContainer.AllowDrop = true;
            listViewContainer.DragDrop += ListViewContainer_DragDrop;
            listViewContainer.DragEnter += ListViewContainer_DragEnter;
            listViewContainer.MouseDoubleClick += ListViewContainer_MouseDoubleClick;
            listViewContainer.KeyDown += ListViewContainer_KeyDown;

            this.AcceptButton = buttonSave;
            this.CancelButton = buttonCancel;

            var objs = Core.State.Items.Where(o=> o.ContainerId == ContainerId);

            foreach (var obj in objs)
            {
                AddItemToContainer(obj.Tile.TilePath, obj.Tile.Meta.Quantity);
            }
        }

        private void ListViewContainer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (listViewContainer.SelectedItems?.Count <= 0)
                {
                    return;
                }

                var items = new List<ListViewItem>();

                foreach (ListViewItem o in listViewContainer.SelectedItems)
                {
                    items.Add(o);
                }

                foreach (var o in items)
                {

                    listViewContainer.Items.Remove(o);
                }
            }
        }

        private void ListViewContainer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewContainer.SelectedItems?.Count != 1)
            {
                return;
            }

            var selectedItem = listViewContainer.SelectedItems[0];

            using (var form = new FormEditQuantity("Quantity", selectedItem.SubItems[QTY_COLUMN].Text))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    selectedItem.SubItems[QTY_COLUMN].Text = form.PropertyValue;
                }
            }
        }

        private void TreeViewTiles_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if ((e.Item as TreeNode).Nodes.Count == 0) //We onyl allow to drag child nodes
            {
                if (e.Button == MouseButtons.Left)
                {
                    DoDragDrop(e.Item, DragDropEffects.Move);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    DoDragDrop(e.Item, DragDropEffects.Copy);
                }
            }
        }

        private void AddItemToContainer(string tilePath, int ?quantity)
        {
            var metaData = TileMetadata.GetFreshMetadata(tilePath);

            ListViewItem item = new ListViewItem(metaData.Name);
            if (quantity != null)
            {
                item.SubItems.Add(quantity.ToString());
            }
            item.ImageKey = tilePath;
            item.Tag = tilePath;
            listViewContainer.Items.Add(item);
        }

        private void ListViewContainer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void ListViewContainer_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            AddItemToContainer(draggedNode.ImageKey, 1);
        }

        private void TreeViewTiles_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        void PopulateMaterials()
        {
            ImageList imageList = new ImageList();
            treeViewTiles.ImageList = imageList;

            imageList.Images.Add("<Folder>", Resources.SwatchTreeView_Folder);

            CreateImageListAndAssets(imageList, null, Constants.BaseAssetPath, "Tiles\\Items");
            if (treeViewTiles.Nodes.Count > 0)
            {
                treeViewTiles.Nodes[0].Expand();
            }
        }

        public TreeNode CreateImageListAndAssets(ImageList imageList, TreeNode parent, string basePath, string partialPath)
        {
            TreeNode node = null;

            if (parent == null)
            {
                node = treeViewTiles.Nodes.Add(Path.GetFileName(partialPath), Path.GetFileName(partialPath), "<Folder>", "<Folder>");
            }
            else
            {
                node = parent.Nodes.Add(Path.GetFileName(partialPath));
            }

            foreach (string d in Directory.GetDirectories(basePath + partialPath))
            {
                var directory = Path.GetFileName(d);
                if (directory.StartsWith("@"))
                {
                    continue;
                }

                var addedNode = CreateImageListAndAssets(imageList, node, basePath, partialPath + "\\" + directory);
                addedNode.ImageKey = "<Folder>";
                addedNode.SelectedImageKey = "<Folder>";
            }

            foreach (var f in Directory.GetFiles(basePath + partialPath, "*.png"))
            {
                if (Path.GetFileName(f).StartsWith("@"))
                {
                    continue;
                }
                var file = new FileInfo(f);

                string fileKey = $"{partialPath}\\{Path.GetFileNameWithoutExtension(file.Name)}";

                if (imageList.Images.ContainsKey(fileKey) == false)
                {
                    imageList.Images.Add(fileKey, SpriteCache.GetBitmapCached(file.FullName));
                }

                node.Nodes.Add(fileKey, Path.GetFileNameWithoutExtension(file.Name), fileKey, fileKey);
            }

            return node;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Core.State.Items.RemoveAll(o => o.ContainerId == ContainerId);

            foreach (ListViewItem obj in listViewContainer.Items)
            {
                var newItem = new CustodyItem()
                {
                    ContainerId = ContainerId
                };

                var meta = TileMetadata.GetFreshMetadata(obj.ImageKey);
                meta.Quantity = Int32.Parse(obj.SubItems[QTY_COLUMN].Text);
                newItem.Tile = new TileIdentifier(obj.ImageKey, meta);

                Core.State.Items.Add(newItem);
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
