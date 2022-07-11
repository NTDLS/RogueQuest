using Assets;
using Library.Engine;
using Library.Engine.Types;
using ScenarioEdit.Engine;
using ScenarioEdit.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormEditContainer : Form
    {
        const int DATA_COLUMN = 1;
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

            var objs = Core.State.Items.Where(o => o.ContainerId == ContainerId);

            foreach (var obj in objs)
            {
                if (obj.Tile.Meta.SubType == ActorSubType.Wand)
                {
                    AddItemToContainer(obj.Tile.TilePath, obj.Tile.Meta.Charges);
                }
                else if (obj.Tile.Meta.ActorClass == ActorClassName.ActorSpawner)
                {
                    if (obj.Tile.Meta.SpawnSubTypes == null)
                    {
                        obj.Tile.Meta.SpawnSubTypes = new ActorSubType[0];
                    }

                    AddItemToContainer(obj.Tile.TilePath, String.Join(",", obj.Tile.Meta.SpawnSubTypes));
                }
                else if (obj.Tile.Meta.CanStack == true)
                {
                    AddItemToContainer(obj.Tile.TilePath, obj.Tile.Meta.Quantity);
                }
                else
                {
                    AddItemToContainer(obj.Tile.TilePath);
                }
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

            var item = selectedItem.Tag as TileIdentifier;

            if (item.Meta.ActorClass == ActorClassName.ActorSpawner)
            {
                var selectedTypes = new List<ActorSubType>();

                var values = selectedItem.SubItems[DATA_COLUMN].Text.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var value in values)
                {
                    selectedTypes.Add((ActorSubType)Enum.Parse(typeof(ActorSubType), value));
                }

                using var form = new FormEditItemSpawner(Core, selectedTypes);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    selectedItem.SubItems[DATA_COLUMN].Text = String.Join(',', form.SelectedSubTypes);
                }
            }
            else if (item.Meta.CanStack == true || item.Meta.SubType == ActorSubType.Wand)
            {
                if (item.Meta.SubType == ActorSubType.Wand)
                {
                    using var form = new FormEditInteger("Charges", selectedItem.SubItems[DATA_COLUMN].Text);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        selectedItem.SubItems[DATA_COLUMN].Text = form.PropertyValue;
                    }
                }
                else
                {
                    using var form = new FormEditInteger("Quantity", selectedItem.SubItems[DATA_COLUMN].Text);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        selectedItem.SubItems[DATA_COLUMN].Text = form.PropertyValue;
                    }
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

        private void AddItemToContainer(string tilePath, int ?quantity = null)
        {
            var metaData = TileMetadata.GetFreshMetadata(tilePath);

            ListViewItem item = new ListViewItem(metaData.Name);
            if (quantity != null)
            {
                item.SubItems.Add(quantity.ToString());
            }
            else
            {
                item.SubItems.Add("-");
            }
            item.ImageKey = tilePath;
            item.Tag = new TileIdentifier(tilePath, metaData);
            listViewContainer.Items.Add(item);
        }

        private void AddItemToContainer(string tilePath, string value)
        {
            var metaData = TileMetadata.GetFreshMetadata(tilePath);
            var item = new ListViewItem(metaData.Name);
            item.SubItems.Add(value);
            item.ImageKey = tilePath;
            item.Tag = new TileIdentifier(tilePath, metaData);
            listViewContainer.Items.Add(item);
        }

        private void ListViewContainer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void ListViewContainer_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            var metaData = TileMetadata.GetFreshMetadata(draggedNode.ImageKey);

            if (metaData.ActorClass == ActorClassName.ActorSpawner)
            {
                AddItemToContainer(draggedNode.ImageKey, String.Join(',', Utility.RandomDropSubTypes));
            }
            else
            {

                AddItemToContainer(draggedNode.ImageKey, 1);
            }
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

            CreateImageListAndAssets(imageList, null, Constants.BaseCommonAssetPath, "Tiles\\Items");
            if (treeViewTiles.Nodes.Count > 0)
            {
                treeViewTiles.Nodes[0].Expand();
            }
        }

        public TreeNode CreateImageListAndAssets(ImageList imageList, TreeNode parent, string basePath, string partialPath)
        {
            TreeNode node;

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
                if (Utility.IgnoreFileName(d))
                {
                    continue;
                }

                var addedNode = CreateImageListAndAssets(imageList, node, basePath, partialPath + "\\" + Path.GetFileName(d));
                addedNode.ImageKey = "<Folder>";
                addedNode.SelectedImageKey = "<Folder>";
            }

            foreach (var f in Directory.GetFiles(basePath + partialPath, "*.png"))
            {
                if (Utility.IgnoreFileName(f))
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

                var tile = obj.Tag as TileIdentifier;

                if (tile.Meta.SubType == ActorSubType.Wand)
                {
                    tile.Meta.Charges = Int32.Parse(obj.SubItems[DATA_COLUMN].Text);
                }
                else if (tile.Meta.ActorClass == ActorClassName.ActorSpawner)
                {
                    var selectedValues = new List<ActorSubType>();

                    var values = obj.SubItems[DATA_COLUMN].Text.Split(',');
                    foreach (var value in values)
                    {
                        selectedValues.Add((ActorSubType)Enum.Parse(typeof(ActorSubType), value));
                    }

                    tile.Meta.SpawnSubTypes = selectedValues.ToArray();
                }
                else
                {
                    if (obj.SubItems[DATA_COLUMN].Text == "-")
                    {
                        tile.Meta.Quantity = null;
                    }
                    else
                    {
                        if (Int32.TryParse(obj.SubItems[DATA_COLUMN].Text, out int value))
                        {
                            tile.Meta.Quantity = value;
                        }
                        else
                        {
                            tile.Meta.Quantity = 1;
                        }
                    }
                }

                newItem.Tile = tile;

                Core.State.Items.Add(newItem);
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
