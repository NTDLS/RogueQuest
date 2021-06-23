using Assets;
using Game.Engine;
using Library.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    public partial class FormInventory : Form
    {
        private ImageList _imageList = new ImageList();
        private Button _buttonClose = new Button();


        public EngineCore Core { get; set; }
        public FormInventory()
        {
            InitializeComponent();
        }

        public FormInventory(EngineCore core)
        {
            InitializeComponent();
            Core = core;
        }

        private void FormInventory_Load(object sender, EventArgs e)
        {
            _buttonClose.Click += _buttonClose_Click;
            this.CancelButton = _buttonClose;

            listViewContainer.SmallImageList = _imageList;
            listViewContainer.LargeImageList = _imageList;
            listViewContainer.ItemDrag += ListViewContainer_ItemDrag;
            listViewContainer.DragEnter += ListViewContainer_DragEnter;
            listViewContainer.AllowDrop = true;

            InitEquipSlot(listViewArmor);
            InitEquipSlot(listViewBracers);
            InitEquipSlot(listViewWeapon);
            InitEquipSlot(listViewPack);
            InitEquipSlot(listViewBelt);
            InitEquipSlot(listViewRightRing);
            InitEquipSlot(listViewNecklace);
            InitEquipSlot(listViewHelment);
            InitEquipSlot(listViewGarment);
            InitEquipSlot(listViewPurse);
            InitEquipSlot(listViewBoots);
            InitEquipSlot(listViewLeftRing);
            InitEquipSlot(listViewFreeHand);
            InitEquipSlot(listViewGauntlets);
            InitEquipSlot(listViewShield);

            PopulateContainerFromPack();
        }

        private void InitEquipSlot(ListView lv)
        {
            lv.AllowDrop = true;
            lv.DragDrop += lv_DragDrop;
            lv.DragEnter += lv_DragEnter;
            lv.ItemDrag += lv_ItemDrag;

            lv.Items.Add("");

            lv.LargeImageList = _imageList;
            lv.SmallImageList = _imageList;
            lv.Scrollable = false;
        }
        private void lv_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void lv_DragDrop(object sender, DragEventArgs e)
        {
            var destination = sender as ListView;

            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            destination.Items[0].ImageKey = draggedItem.ImageKey;
            destination.Items[0].Text = draggedItem.Text;

            e.Effect = e.AllowedEffect;
        }

        private void lv_ItemDrag(object sender, ItemDragEventArgs e)
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



        private void ListViewContainer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void ListViewContainer_ItemDrag(object sender, ItemDragEventArgs e)
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

        private void _buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        string GetImageKey(string tilePath)
        {
            if (_imageList.Images.Keys.Contains(tilePath))
            {
                return tilePath;
            }

            var bitmap = SpriteCache.GetBitmapCached(Assets.Constants.GetAssetPath($"{tilePath}.png"));
            _imageList.Images.Add(tilePath, bitmap);

            return tilePath;
        }

        void PopulateContainerFromPack()
        {
            foreach (var item in Core.State.Character.Inventory)
            {
                AddItemToContainer(item.TilePath, item.Meta);
            }
        }

        private void AddItemToContainer(string tilePath, TileMetadata meta)
        {
            string text = meta.Name;

            if (meta.CanStack == true && meta.Quantity > 0)
            {
                text += $" ({meta.Quantity})";
            }

            ListViewItem item = new ListViewItem(text);
            item.ImageKey = GetImageKey(tilePath);
            item.Tag = meta;
            listViewContainer.Items.Add(item);
        }
    }
}
