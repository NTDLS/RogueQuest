using Assets;
using Game.Engine;
using Library.Engine;
using Library.Engine.Types;
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

        public class EquipTag
        {
            public TileIdentifier Tile { get; set; }
            public ActorSubType AcceptType { get; set; }
            public EquipSlot Slot { get; set; }
        }

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
            listViewContainer.DragDrop += ListViewContainer_DragDrop;
            listViewContainer.AllowDrop = true;

            InitEquipSlot(listViewArmor, ActorSubType.Armor, EquipSlot.Armor);
            InitEquipSlot(listViewBracers, ActorSubType.Bracers, EquipSlot.Bracers);
            InitEquipSlot(listViewWeapon, ActorSubType.Weapon, EquipSlot.Weapon);
            InitEquipSlot(listViewPack, ActorSubType.Pack, EquipSlot.Pack);
            InitEquipSlot(listViewBelt, ActorSubType.Belt, EquipSlot.Belt);
            InitEquipSlot(listViewRightRing, ActorSubType.Ring, EquipSlot.RightRing);
            InitEquipSlot(listViewNecklace, ActorSubType.Necklace, EquipSlot.Necklace);
            InitEquipSlot(listViewHelment, ActorSubType.Helment, EquipSlot.Helment);
            InitEquipSlot(listViewGarment, ActorSubType.Garment, EquipSlot.Garment);
            //InitEquipSlot(listViewPurse, ActorSubType.Purse, EquipSlot.Purse);
            InitEquipSlot(listViewBoots, ActorSubType.Boots, EquipSlot.Boots);
            InitEquipSlot(listViewLeftRing, ActorSubType.Ring, EquipSlot.LeftRing);
            InitEquipSlot(listViewFreeHand, ActorSubType.Unspecified, EquipSlot.FreeHand);
            InitEquipSlot(listViewGauntlets, ActorSubType.Gauntlets, EquipSlot.Gauntlets);
            InitEquipSlot(listViewShield, ActorSubType.Shield, EquipSlot.Shield);

            //If we are wearing a pack, go ahead and show its contents.
            var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack);
            if (pack.Tile != null)
            {
                PopulateContainerFromPack((Guid)pack.Tile.Meta.UID);
            }
        }

        #region Container.

        private void ListViewContainer_DragDrop(object sender, DragEventArgs e)
        {
            var destination = sender as ListView;
            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            if (destination == draggedItem.ListView)
            {
                return;
            }

            var draggedItemTag = draggedItem.Tag as EquipTag;

            AddItemToContainer(draggedItemTag.Tile.TilePath, draggedItemTag.Tile.Meta);

            draggedItem.ImageKey = null;
            draggedItem.Text = "";
            draggedItemTag.Tile = null;
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

        #endregion


        #region Equip Slot.

        private void InitEquipSlot(ListView lv, ActorSubType acceptType, EquipSlot slot)
        {
            lv.HideSelection = true;
            lv.LargeImageList = _imageList;
            lv.SmallImageList = _imageList;
            lv.Scrollable = false;
            lv.AllowDrop = true;

            lv.DragDrop += ListView_EquipSlot_DragDrop;
            lv.DragEnter += ListView_EquipSlot_DragEnter;
            lv.ItemDrag += ListView_EquipSlot_ItemDrag;

            ListViewItem item = new ListViewItem("");
            item.Tag = new EquipTag()
            {
                AcceptType = acceptType,
                Slot = slot
            };

            var equipSlot = Core.State.Character.GetEquipSlot(slot);
            if (equipSlot.Tile != null)
            {
                string text = equipSlot.Tile.Meta.Name;

                if (equipSlot.Tile.Meta.CanStack == true && equipSlot.Tile.Meta.Quantity > 0)
                {
                    text += $" ({equipSlot.Tile.Meta.Quantity})";
                }

                item.Text = text;
                item.ImageKey = GetImageKey(equipSlot.Tile.TilePath);
                (item.Tag as EquipTag).Tile = equipSlot.Tile;
            }

            lv.Items.Add(item);
        }

        private void ListView_EquipSlot_DragEnter(object sender, DragEventArgs e)
        {
            var destination = sender as ListView;
            var destinationTag = destination.Items[0].Tag as EquipTag;

            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            var draggedItemTag = draggedItem.Tag as EquipTag;

            if (destination == draggedItem.ListView || destinationTag.Tile != null)
            {
                return;
            }

            if (destinationTag.AcceptType == draggedItemTag.Tile.Meta.SubType
                || destinationTag.AcceptType == ActorSubType.Unspecified)
            {
                e.Effect = e.AllowedEffect;
            }
        }

        private void ListView_EquipSlot_DragDrop(object sender, DragEventArgs e)
        {
            var destination = sender as ListView;
            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            if (destination == draggedItem.ListView)
            {
                return;
            }

            var destinationTag = destination.Items[0].Tag as EquipTag;

            var draggedItemTag = draggedItem.Tag as EquipTag;

            if (destinationTag.AcceptType == draggedItemTag.Tile.Meta.SubType
                || destinationTag.AcceptType == ActorSubType.Unspecified)
            {
                destination.Items[0].ImageKey = draggedItem.ImageKey;
                destination.Items[0].Text = draggedItem.Text;
                destinationTag.Tile = draggedItemTag.Tile;

                var equipSlot = Core.State.Character.GetEquipSlot(destinationTag.Slot);
                equipSlot.Tile = draggedItemTag.Tile;

                if (draggedItem.ListView == listViewContainer || draggedItem.ListView == listViewGround)
                {
                    draggedItem.ListView.Items.Remove(draggedItem);
                }
                else
                {
                    //Re cant remove the "empty items" from equip slots.
                    draggedItem.ImageKey = null;
                    draggedItem.Text = "";
                    draggedItemTag.Tile = null;
                }
            }
        }

        private void ListView_EquipSlot_ItemDrag(object sender, ItemDragEventArgs e)
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

        #endregion


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

        void PopulateContainerFromPack(Guid containerId)
        {
            foreach (var item in Core.State.Character.Inventory.Where(o => o.ContainerId == containerId))
            {
                AddItemToContainer(item.Tile.TilePath, item.Tile.Meta);
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
            item.Tag = new EquipTag()
            {
                AcceptType = (ActorSubType)meta.SubType,
                Tile = new TileIdentifier(tilePath)
                {
                    Meta = meta
                }
            };
            listViewContainer.Items.Add(item);
        }
    }
}
