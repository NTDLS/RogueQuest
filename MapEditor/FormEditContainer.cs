﻿using Assets;
using Library.Engine;
using MapEditor.Engine;
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

namespace MapEditor
{
    public partial class FormEditContainer : Form
    {
        const int QTY_COLUMN = 1;

        public EngineCore Core { get; set; }
        public Guid ContainerId { get; set; }

        private Library.Engine.Container _container { get; set; }

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

            _container = Core.Actors.Containers.GetContainer(ContainerId);

            foreach (var obj in _container.Chunks)
            {
                AddItemToContainer(obj.TilePath, obj.Meta.Quantity);
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

        private void AddItemToContainer(string tilePath, int quantity)
        {
            var metaData = TileMetadata.GetFreshMetadata(tilePath);

            ListViewItem item = new ListViewItem(metaData.Name);
            item.SubItems.Add(quantity.ToString());
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
            CreateImageListAndAssets(imageList, null, Assets.Constants.BasePath, "Tiles/Items");
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

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            _container.Clear();

            foreach (ListViewItem obj in listViewContainer.Items)
            {
                var metaData = TileMetadata.GetFreshMetadata(obj.ImageKey);

                metaData.Quantity = Int32.Parse(obj.SubItems[QTY_COLUMN].Text);

                _container.Add(new LevelChunk()
                {
                    TilePath = obj.ImageKey,
                    Meta = metaData,
                });
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
