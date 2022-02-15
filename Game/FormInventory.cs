using Assets;
using Game.Actors;
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
        private TileIdentifier _currentlySelectedPack = null;
        private ToolTip _interrogationTip = new ToolTip();

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

            listViewSelectedContainer.SmallImageList = _imageList;
            listViewSelectedContainer.LargeImageList = _imageList;
            listViewSelectedContainer.ItemDrag += ListViewSelectedContainer_ItemDrag;
            listViewSelectedContainer.DragEnter += ListViewSelectedContainer_DragEnter;
            listViewSelectedContainer.DragDrop += ListViewSelectedContainer_DragDrop;
            listViewSelectedContainer.AllowDrop = true;
            listViewSelectedContainer.MouseDoubleClick += ListViewSelectedContainer_MouseDoubleClick;
            listViewSelectedContainer.MouseUp += ListView_Shared_MouseUp;
            listViewSelectedContainer.MouseDown += ListView_Shared_MouseDown;

            listViewGround.SmallImageList = _imageList;
            listViewGround.LargeImageList = _imageList;
            listViewGround.ItemDrag += ListViewGround_ItemDrag;
            listViewGround.DragEnter += ListViewGround_DragEnter;
            listViewGround.DragDrop += ListViewGround_DragDrop;
            listViewGround.AllowDrop = true;
            listViewGround.MouseDoubleClick += ListViewGround_MouseDoubleClick;
            listViewGround.MouseUp += ListView_Shared_MouseUp;
            listViewGround.MouseDown += ListView_Shared_MouseDown;

            listViewPlayerPack.SmallImageList = _imageList;
            listViewPlayerPack.LargeImageList = _imageList;
            listViewPlayerPack.ItemDrag += ListViewPlayerPack_ItemDrag;
            listViewPlayerPack.DragEnter += ListViewPlayerPack_DragEnter;
            listViewPlayerPack.DragDrop += ListViewPlayerPack_DragDrop;
            listViewPlayerPack.AllowDrop = true;
            listViewPlayerPack.MouseDoubleClick += ListViewPlayerPack_MouseDoubleClick;
            listViewPlayerPack.MouseUp += ListView_Shared_MouseUp;
            listViewPlayerPack.MouseDown += ListView_Shared_MouseDown;

            InitEquipSlot(listViewArmor, ActorSubType.Armor, EquipSlot.Armor);
            InitEquipSlot(listViewBracers, ActorSubType.Bracers, EquipSlot.Bracers);
            InitEquipSlot(listViewWeapon, ActorSubType.Weapon, EquipSlot.Weapon);
            InitEquipSlot(listViewPack, ActorSubType.Pack, EquipSlot.Pack);
            InitEquipSlot(listViewBelt, ActorSubType.Belt, EquipSlot.Belt);
            InitEquipSlot(listViewRightRing, ActorSubType.Ring, EquipSlot.RightRing);
            InitEquipSlot(listViewNecklace, ActorSubType.Necklace, EquipSlot.Necklace);
            InitEquipSlot(listViewHelment, ActorSubType.Helment, EquipSlot.Helment);
            InitEquipSlot(listViewGarment, ActorSubType.Garment, EquipSlot.Garment);
            InitEquipSlot(listViewPurse, ActorSubType.Purse, EquipSlot.Purse);
            InitEquipSlot(listViewBoots, ActorSubType.Boots, EquipSlot.Boots);
            InitEquipSlot(listViewLeftRing, ActorSubType.Ring, EquipSlot.LeftRing);
            InitEquipSlot(listViewFreeHand, ActorSubType.Unspecified, EquipSlot.FreeHand);
            InitEquipSlot(listViewGauntlets, ActorSubType.Gauntlets, EquipSlot.Gauntlets);
            InitEquipSlot(listViewShield, ActorSubType.Shield, EquipSlot.Shield);

            //If we are wearing a pack, go ahead and show its contents.
            var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack);
            if (pack.Tile != null)
            {
                PopulateContainerFromPack(listViewPlayerPack, pack.Tile);
            }

            PopulateContainerFromGround(listViewGround);
        }

        private bool UseItem(TileIdentifier item)
        {
            string text = $"Use {item.Meta.Name}?";

            if (item.Meta.Charges > 0)
            {
                text += $"\r\n{item.Meta.Charges} charges remaining.";
            }

            if (MessageBox.Show(text, $"RougeQuest :: Use Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var inventoryItem = Core.State.Items.Where(o => o.Tile.Meta.UID == item.Meta.UID).First();

                if (inventoryItem != null && inventoryItem.Tile.Meta.UID != null)
                {
                    return Core.Tick.UseConsumableItem((Guid)inventoryItem.Tile.Meta.UID, null);
                }
            }

            return false;
        }

        #region ListViewGround

        private void ListViewGround_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewGround.SelectedItems?.Count != 1)
            {
                return;
            }

            var item = listViewGround.SelectedItems[0].Tag as EquipTag;
            if (item.Tile.Meta.SubType == ActorSubType.Pack
                || item.Tile.Meta.SubType == ActorSubType.Belt
                || item.Tile.Meta.SubType == ActorSubType.Chest
                || item.Tile.Meta.SubType == ActorSubType.Purse)
            {
                OpenPack(item);
            }
            else
            {
                if (item.Tile.Meta.IsConsumable == true)
                {
                    //We cant use items on the ground because they dont exist in the world inventory collection.
                    //UseItem(item.Tile);
                }
            }
        }

        private void ListViewGround_DragDrop(object sender, DragEventArgs e)
        {
            var destination = sender as ListView;
            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            var draggedItemTag = draggedItem.Tag as EquipTag;

            if (destination == draggedItem.ListView)
            {
                return;
            }

            //If we are draging from the primary pack slot, then close the pack.
            if (draggedItem.ListView == listViewPack)
            {
                listViewPlayerPack.Items.Clear();
            }

            //Find the item in the players inventory and change its container id to that of the selected open pack.
            var inventoryItem = Core.State.Items.Where(o => o.Tile.Meta.UID == draggedItemTag.Tile.Meta.UID).First();

            bool wasStacked = false;

            if (inventoryItem.Tile.Meta.CanStack == true)
            {
                var itemUnderfoot = Core.Actors.Intersections(Core.Player)
                    .Where(o => o.Meta.ActorClass == ActorClassName.ActorItem && o.TilePath == draggedItemTag.Tile.TilePath)
                    .Cast<ActorItem>().FirstOrDefault();

                if (itemUnderfoot != null)
                {
                    itemUnderfoot.Meta.Quantity = (itemUnderfoot.Meta.Quantity ?? 0) + inventoryItem.Tile.Meta.Quantity;

                    var listViewItem = FindListViewObjectByUid(listViewGround, (Guid)itemUnderfoot.Meta.UID);
                    if (listViewItem != null)
                    {
                        string text = itemUnderfoot.Meta.Name;

                        if (itemUnderfoot.Meta.CanStack == true && itemUnderfoot.Meta.Quantity > 0)
                        {
                            text += $" ({itemUnderfoot.Meta.Quantity})";
                        }

                        listViewItem.Text = text;
                    }

                    wasStacked = true;
                }
            }

            if (wasStacked == false)
            {
                var droppedItem = Core.Actors.AddDynamic(inventoryItem.Tile.Meta.ActorClass.ToString(),
                    Core.Player.X, Core.Player.Y, inventoryItem.Tile.TilePath);
                droppedItem.Meta = inventoryItem.Tile.Meta;

                AddItemToListView(listViewGround, draggedItemTag.Tile.TilePath, draggedItemTag.Tile.Meta);
            }

            Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == draggedItemTag.Tile.Meta.UID);

            if (draggedItem.ListView == listViewSelectedContainer)
            {
                draggedItem.ListView.Items.Remove(draggedItem);
            }
            else if (draggedItem.ListView == listViewPlayerPack)
            {
                draggedItem.ListView.Items.Remove(draggedItem);
            }
            else
            {
                var slotToVacate = Core.State.Character.GetEquipSlot(draggedItemTag.Slot);
                slotToVacate.Tile = null;

                //We dont remove the items from equip slots, we just clear their text and image.
                draggedItem.ImageKey = null;
                draggedItem.Text = "";
                draggedItemTag.Tile = null;
            }
        }

        private void ListViewGround_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void ListViewGround_ItemDrag(object sender, ItemDragEventArgs e)
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

        #region ListViewSelectedContainer.

        private void ListViewSelectedContainer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewSelectedContainer.SelectedItems?.Count != 1)
            {
                return;
            }

            var selectedItem = listViewSelectedContainer.SelectedItems[0];
            var item = selectedItem.Tag as EquipTag;
            if (item.Tile.Meta.SubType == ActorSubType.Pack
                || item.Tile.Meta.SubType == ActorSubType.Belt
                || item.Tile.Meta.SubType == ActorSubType.Chest
                || item.Tile.Meta.SubType == ActorSubType.Purse)
            {
                OpenPack(item);
            }
            else if (item.Tile.Meta.SubType == ActorSubType.Scroll
                         || item.Tile.Meta.SubType == ActorSubType.Potion)
            {
                if (item.Tile.Meta.IsConsumable == true)
                {
                    if (UseItem(item.Tile))
                    {
                        if ((item.Tile.Meta.Quantity ?? 0) == 0)
                        {
                            listViewPlayerPack.Items.Remove(selectedItem);
                        }
                        else
                        {
                            string text = item.Tile.Meta.Name;
                            if (item.Tile.Meta.CanStack == true && item.Tile.Meta.Quantity > 0)
                            {
                                text += $" ({item.Tile.Meta.Quantity})";
                            }
                            selectedItem.Text = text;
                        }
                    }
                }
            }
        }

        private void ListViewSelectedContainer_DragDrop(object sender, DragEventArgs e)
        {
            if (_currentlySelectedPack == null)
            {
                Constants.Alert("You havn't opened a container yet.");
                return;
            }

            var destination = sender as ListView;
            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            if (destination == draggedItem.ListView)
            {
                return;
            }

            //If we are draging from the primary pack slot, then close the pack.
            if (draggedItem.ListView == listViewPack)
            {
                listViewPlayerPack.Items.Clear();
            }

            var draggedItemTag = draggedItem.Tag as EquipTag;

            if (_currentlySelectedPack.Meta.SubType == ActorSubType.Purse
                && draggedItemTag.Tile.Meta.SubType != ActorSubType.Money)
            {
                Constants.Alert("You can only store coins in the purse");
                return;
            }

            //Find the item in the players inventory and change its container id to that of the selected open pack.
            var inventoryItem = Core.State.Items.Where(o => o.Tile.Meta.UID == draggedItemTag.Tile.Meta.UID).First();
            int maxBulk = (int)_currentlySelectedPack.Meta.BulkCapacity;
            int maxWeight = (int)_currentlySelectedPack.Meta.WeightCapacity;
            int? maxItems = _currentlySelectedPack.Meta.ItemCapacity;

            //Do weight/bulk math.
            var currentPackWeight = Core.State.Items.Where(o => o.ContainerId == _currentlySelectedPack.Meta.UID).Sum(o => o.Tile.Meta.Weight);
            if (inventoryItem.Tile.Meta.Weight + currentPackWeight > maxWeight)
            {
                Constants.Alert($"{inventoryItem.Tile.Meta.Name} is too bulky for your {_currentlySelectedPack.Meta.Name}. Drop something or move to free hand?");
                return;
            }

            var currentPackBulk = Core.State.Items.Where(o => o.ContainerId == _currentlySelectedPack.Meta.UID).Sum(o => o.Tile.Meta.Bulk);
            if (inventoryItem.Tile.Meta.Bulk + currentPackBulk > maxBulk)
            {
                Constants.Alert($"{inventoryItem.Tile.Meta.Name} is too heavy for your {_currentlySelectedPack.Meta.Name}. Drop something or move to free hand?");
                return;
            }

            if (maxItems != null)
            {
                var currentPackItems = Core.State.Items.Where(o => o.ContainerId == _currentlySelectedPack.Meta.UID).Count();
                if (currentPackItems + 1 > (int)maxItems)
                {
                    Constants.Alert($"{_currentlySelectedPack.Meta.Name} can only carry {maxItems} items. Drop something or move to free hand?");
                    return;
                }
            }

            ActorItem itemOnGround = null;

            if (draggedItem.ListView == listViewGround)
            {
                itemOnGround = Core.Actors.Intersections(Core.Player)
                    .Where(o => o.Meta.ActorClass == ActorClassName.ActorItem && o.TilePath == draggedItemTag.Tile.TilePath)
                    .Cast<ActorItem>().FirstOrDefault();

                Core.State.Items.Add(new CustodyItem()
                {
                    Tile = itemOnGround.CloneIdentifier()
                });
            }

            if (_currentlySelectedPack.Meta.UID == draggedItemTag.Tile.Meta.UID)
            {
                //A container canot contain itsself.
                Constants.Alert($"A {_currentlySelectedPack.Meta.Name} cannot contain itself.");
                return;
            }
            if (inventoryItem.ContainerId == (Guid)_currentlySelectedPack.Meta.UID)
            {
                //No need to do anything if we are dragging to the same container.
                return;
            }

            bool wasStacked = false;

            if (inventoryItem.Tile.Meta.CanStack == true)
            {
                //If we are dragging to a container and the container already contains some of the stackable stuff, then stack!
                var existingInventoryItem = Core.State.Items
                    .Where(o => o.Tile.TilePath == draggedItemTag.Tile.TilePath
                    && o.ContainerId == _currentlySelectedPack.Meta.UID).FirstOrDefault();

                if (existingInventoryItem != null)
                {
                    existingInventoryItem.Tile.Meta.Quantity = (existingInventoryItem.Tile.Meta.Quantity ?? 0) + inventoryItem.Tile.Meta.Quantity;
                    Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == draggedItemTag.Tile.Meta.UID);

                    var listViewItem = FindListViewObjectByUid(listViewSelectedContainer, (Guid)existingInventoryItem.Tile.Meta.UID);
                    if (listViewItem != null)
                    {
                        string text = existingInventoryItem.Tile.Meta.Name;

                        if (existingInventoryItem.Tile.Meta.CanStack == true && existingInventoryItem.Tile.Meta.Quantity > 0)
                        {
                            text += $" ({existingInventoryItem.Tile.Meta.Quantity})";
                        }

                        listViewItem.Text = text;
                    }

                    wasStacked = true;
                }
            }

            if (wasStacked == false)
            {
                AddItemToListView(listViewSelectedContainer, draggedItemTag.Tile.TilePath, draggedItemTag.Tile.Meta);
                inventoryItem.ContainerId = (Guid)_currentlySelectedPack.Meta.UID;
            }

            if (draggedItem.ListView == listViewGround)
            {
                itemOnGround.QueueForDelete();
                draggedItem.ListView.Items.Remove(draggedItem);
            }
            else if (draggedItem.ListView == listViewPlayerPack)
            {
                draggedItem.ListView.Items.Remove(draggedItem);
            }
            else
            {
                var slotToVacate = Core.State.Character.GetEquipSlot(draggedItemTag.Slot);
                slotToVacate.Tile = null;

                //We dont remove the items from equip slots, we just clear their text and image.
                draggedItem.ImageKey = null;
                draggedItem.Text = "";
                draggedItemTag.Tile = null;
            }
        }

        private void ListViewSelectedContainer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void ListViewSelectedContainer_ItemDrag(object sender, ItemDragEventArgs e)
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

        #region ListViewPlayerPack.

        private void ListViewPlayerPack_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewPlayerPack.SelectedItems?.Count != 1)
            {
                return;
            }

            var selectedItem = listViewPlayerPack.SelectedItems[0];
            var item = selectedItem.Tag as EquipTag;
            if (item.Tile.Meta.SubType == ActorSubType.Pack
                || item.Tile.Meta.SubType == ActorSubType.Belt
                || item.Tile.Meta.SubType == ActorSubType.Chest
                || item.Tile.Meta.SubType == ActorSubType.Purse)
            {
                OpenPack(item);
            }
            else if (item.Tile.Meta.SubType == ActorSubType.Scroll
                         || item.Tile.Meta.SubType == ActorSubType.Potion)
            {
                if (item.Tile.Meta.IsConsumable == true)
                {
                    if (UseItem(item.Tile))
                    {
                        if ((item.Tile.Meta.Quantity ?? 0) == 0)
                        {
                            listViewPlayerPack.Items.Remove(selectedItem);
                        }
                        else
                        {
                            string text = item.Tile.Meta.Name;
                            if (item.Tile.Meta.CanStack == true && item.Tile.Meta.Quantity > 0)
                            {
                                text += $" ({item.Tile.Meta.Quantity})";
                            }
                            selectedItem.Text = text;
                        }
                    }
                }
            }
        }

        private void ListViewPlayerPack_DragDrop(object sender, DragEventArgs e)
        {
            var playersPack = Core.State.Character.GetEquipSlot(EquipSlot.Pack);
            if (playersPack.Tile == null)
            {
                Constants.Alert("You need to equip a pack.");
                return;
            }

            var destination = sender as ListView;
            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            var draggedItemTag = draggedItem.Tag as EquipTag;

            if (destination == draggedItem.ListView)
            {
                return;
            }

            //Find the item in the players inventory and change its container id to that of the selected open pack.
            var inventoryItem = Core.State.Items.Where(o => o.Tile.Meta.UID == draggedItemTag.Tile.Meta.UID).First();
            int maxBulk = (int)playersPack.Tile.Meta.BulkCapacity;
            int maxWeight = (int)playersPack.Tile.Meta.WeightCapacity;
            int? maxItems = playersPack.Tile.Meta.ItemCapacity;

            //Do weight/bulk math.
            var currentPackWeight = Core.State.Items.Where(o => o.ContainerId == playersPack.Tile.Meta.UID).Sum(o => o.Tile.Meta.Weight);
            if (inventoryItem.Tile.Meta.Weight + currentPackWeight > maxWeight)
            {
                Constants.Alert($"{inventoryItem.Tile.Meta.Name} is too bulky for your {playersPack.Tile.Meta.Name}. Drop something or move to free hand?");
                return;
            }

            var currentPackBulk = Core.State.Items.Where(o => o.ContainerId == playersPack.Tile.Meta.UID).Sum(o => o.Tile.Meta.Bulk);
            if (inventoryItem.Tile.Meta.Bulk + currentPackBulk > maxBulk)
            {
                Constants.Alert($"{inventoryItem.Tile.Meta.Name} is too heavy for your {playersPack.Tile.Meta.Name}. Drop something or move to free hand?");
                return;
            }

            if (maxItems != null)
            {
                var currentPackItems = Core.State.Items.Where(o => o.ContainerId == playersPack.Tile.Meta.UID).Count();
                if (currentPackItems + 1 > (int)maxItems)
                {
                    Constants.Alert($"{playersPack.Tile.Meta.Name} can only carry {maxItems} items. Drop something or move to free hand?");
                    return;
                }
            }

            //If we are draging from the primary pack slot, then close the pack.
            if (draggedItem.ListView == listViewPack)
            {
                listViewPlayerPack.Items.Clear();
            }

            ActorItem itemOnGround = null;

            if (draggedItem.ListView == listViewGround)
            {
                itemOnGround = Core.Actors.Intersections(Core.Player)
                    .Where(o => o.Meta.ActorClass == ActorClassName.ActorItem && o.Meta.UID == (Guid)draggedItemTag.Tile.Meta.UID)
                    .Cast<ActorItem>().FirstOrDefault();

                Core.State.Items.Add(new CustodyItem()
                {
                    Tile = itemOnGround.CloneIdentifier()
                });
            }


            if (playersPack.Tile.Meta.UID == draggedItemTag.Tile.Meta.UID)
            {
                //A container canot contain itsself.
                Constants.Alert($"A {playersPack.Tile.Meta.Name} cannot contain itself.");
                return;
            }
            if (inventoryItem.ContainerId == (Guid)playersPack.Tile.Meta.UID)
            {
                //No need to do anything if we are dragging to the same container.
                return;
            }

            bool wasStacked = false;

            if (inventoryItem.Tile.Meta.CanStack == true)
            {
                //If we are dragging to a container and the container already contains some of the stackable stuff, then stack!
                var existingInventoryItem = Core.State.Items
                    .Where(o => o.Tile.TilePath == draggedItemTag.Tile.TilePath
                    && o.ContainerId == playersPack.Tile.Meta.UID).FirstOrDefault();

                if (existingInventoryItem != null)
                {
                    existingInventoryItem.Tile.Meta.Quantity = (existingInventoryItem.Tile.Meta.Quantity ?? 0) + inventoryItem.Tile.Meta.Quantity;
                    Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == draggedItemTag.Tile.Meta.UID);

                    var listViewItem = FindListViewObjectByUid(listViewPlayerPack, (Guid)existingInventoryItem.Tile.Meta.UID);
                    if (listViewItem != null)
                    {
                        string text = existingInventoryItem.Tile.Meta.Name;

                        if (existingInventoryItem.Tile.Meta.CanStack == true && existingInventoryItem.Tile.Meta.Quantity > 0)
                        {
                            text += $" ({existingInventoryItem.Tile.Meta.Quantity})";
                        }

                        listViewItem.Text = text;
                    }

                    wasStacked = true;
                }
            }

            if (wasStacked == false)
            {
                AddItemToListView(listViewPlayerPack, draggedItemTag.Tile.TilePath, draggedItemTag.Tile.Meta);
                inventoryItem.ContainerId = (Guid)playersPack.Tile.Meta.UID;
            }

            if (draggedItem.ListView == listViewGround)
            {
                itemOnGround.QueueForDelete();
                draggedItem.ListView.Items.Remove(draggedItem);
            }
            else if (draggedItem.ListView == listViewSelectedContainer)
            {
                draggedItem.ListView.Items.Remove(draggedItem);
            }
            else
            {
                var slotToVacate = Core.State.Character.GetEquipSlot(draggedItemTag.Slot);
                slotToVacate.Tile = null;

                //We dont remove the items from equip slots, we just clear their text and image.
                draggedItem.ImageKey = null;
                draggedItem.Text = "";
                draggedItemTag.Tile = null;
            }
        }

        private void ListViewPlayerPack_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void ListViewPlayerPack_ItemDrag(object sender, ItemDragEventArgs e)
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

            if (slot != EquipSlot.Purse)
            {
                lv.AllowDrop = true;
                lv.DragDrop += ListView_EquipSlot_DragDrop;
                lv.DragEnter += ListView_EquipSlot_DragEnter;
                lv.ItemDrag += ListView_EquipSlot_ItemDrag;
                lv.MouseDown += Lv_MouseDown;
                lv.MouseUp += Lv_MouseUp;
            }

            lv.MouseDoubleClick += Lv_MouseDoubleClick;

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

        private void Lv_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var lv = sender as ListView;
                var item = lv.Items[0].Tag as EquipTag;

                string text = item.Tile.Meta.Name;
                if (item.Tile.Meta.Weight != null) text += "\r\n" + $"Weight: {item.Tile.Meta.Weight:N0}";
                if (item.Tile.Meta.Bulk != null) text += "\r\n" + $"Bulk: {item.Tile.Meta.Bulk:N0}";
                if (item.Tile.Meta.AC != null) text += "\r\n" + $"AC: {item.Tile.Meta.AC:N0}";

                if (item.Tile.Meta.SubType == ActorSubType.Weapon)
                    text += "\r\n" + $"Stats: {item.Tile.Meta.DndDamageText}";
                else if (item.Tile.Meta.SubType == ActorSubType.Money)
                    text += "\r\n" + $"Value: {((int)(item.Tile.Meta.Quantity * item.Tile.Meta.Value)):N0} gold";

                if (string.IsNullOrWhiteSpace(text) == false)
                {
                    var location = new Point(e.X + 10, e.Y - 25);
                    _interrogationTip.Show(text, lv, location, 5000);
                }
            }
        }

        private void Lv_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _interrogationTip.Hide(sender as Control);
            }
        }

        private void Lv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var listView = sender as ListView;

            if (listView.SelectedItems?.Count != 1)
            {
                return;
            }

            if (listView == listViewPack)
            {
                //Only open the worn pack to the main container view.
                var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack);
                if (pack.Tile != null)
                {
                    PopulateContainerFromPack(listViewPlayerPack, pack.Tile);
                }
            }
            else
            {
                var selectedItem = listView.SelectedItems[0];
                var item = selectedItem.Tag as EquipTag;
                if (item.Tile.Meta.SubType == ActorSubType.Pack
                    || item.Tile.Meta.SubType == ActorSubType.Belt
                    || item.Tile.Meta.SubType == ActorSubType.Chest
                    || item.Tile.Meta.SubType == ActorSubType.Purse)
                {
                    OpenPack(item);
                }
                else if (item.Tile.Meta.SubType == ActorSubType.Scroll
                        || item.Tile.Meta.SubType == ActorSubType.Potion)
                {
                    if (item.Tile.Meta.IsConsumable == true)
                    {
                        if (UseItem(item.Tile))
                        {
                            if ((item.Tile.Meta.Quantity ?? 0) == 0)
                            {
                                listViewPlayerPack.Items.Remove(selectedItem);
                            }
                            else
                            {
                                string text = item.Tile.Meta.Name;
                                if (item.Tile.Meta.CanStack == true && item.Tile.Meta.Quantity > 0)
                                {
                                    text += $" ({item.Tile.Meta.Quantity})";
                                }
                                selectedItem.Text = text;
                            }
                        }
                    }
                }
            }
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

            //If we are draging from the primary pack slot, then close the pack.
            if (draggedItem.ListView == listViewPack)
            {
                listViewPlayerPack.Items.Clear();
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

                ActorItem itemOnGround = null;

                if (draggedItem.ListView == listViewGround)
                {
                    itemOnGround = Core.Actors.Intersections(Core.Player)
                        .Where(o => o.Meta.ActorClass == ActorClassName.ActorItem && o.TilePath == draggedItemTag.Tile.TilePath)
                        .Cast<ActorItem>().FirstOrDefault();

                    Core.State.Items.Add(new CustodyItem()
                    {
                        Tile = itemOnGround.CloneIdentifier()
                    });
                }

                //If the item is in a container, find the item and set its container to NULL.
                var inventoryItem = Core.State.Items.Where(o => o.Tile.Meta.UID == draggedItemTag.Tile.Meta.UID).FirstOrDefault();
                if (inventoryItem != null)
                {
                    inventoryItem.ContainerId = null; //find the item in inventory and set its container id to null.
                }

                if (draggedItem.ListView == listViewGround)
                {
                    itemOnGround.QueueForDelete();
                    draggedItem.ListView.Items.Remove(draggedItem);
                }
                else if (draggedItem.ListView == listViewPlayerPack)
                {
                    draggedItem.ListView.Items.Remove(draggedItem);
                }
                else if (draggedItem.ListView == listViewSelectedContainer)
                {
                    draggedItem.ListView.Items.Remove(draggedItem);
                }
                else
                {
                    var slotToVacate = Core.State.Character.GetEquipSlot(draggedItemTag.Slot);
                    slotToVacate.Tile = null;

                    //We dont remove the items from equip slots, we just clear their text and image.
                    draggedItem.ImageKey = null;
                    draggedItem.Text = "";
                    draggedItemTag.Tile = null;
                }

                if (destination == listViewPack)
                {
                    var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack);
                    if (pack.Tile != null)
                    {
                        if (pack.Tile.Meta.UID == _currentlySelectedPack?.Meta?.UID)
                        {
                            listViewSelectedContainer.Items.Clear();
                        }

                        PopulateContainerFromPack(listViewPlayerPack, pack.Tile);
                    }
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

        #region Form Events.

        private void ListView_Shared_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var lv = sender as ListView;
                var selection = lv.GetItemAt(e.X, e.Y);

                if (selection == null)
                {
                    return;
                }

                var item = selection.Tag as EquipTag;

                string text = item.Tile.Meta.Name;
                if (item.Tile.Meta.Weight != null) text += "\r\n" + $"Weight: {item.Tile.Meta.Weight:N0}";
                if (item.Tile.Meta.Bulk != null) text += "\r\n" + $"Bulk: {item.Tile.Meta.Bulk:N0}";
                if (item.Tile.Meta.AC != null) text += "\r\n" + $"AC: {item.Tile.Meta.AC:N0}";

                if (item.Tile.Meta.SubType == ActorSubType.Weapon)
                    text += "\r\n" + $"Stats: {item.Tile.Meta.DndDamageText}";
                else if (item.Tile.Meta.SubType == ActorSubType.Money)
                    text += "\r\n" + $"Value: {((int)(item.Tile.Meta.Quantity * item.Tile.Meta.Value)):N0} gold";

                if (string.IsNullOrWhiteSpace(text) == false)
                {
                    var location = new Point(e.X + 10, e.Y - 25);
                    _interrogationTip.Show(text, lv, location, 5000);
                }
            }
        }

        private void ListView_Shared_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _interrogationTip.Hide(sender as Control);
            }
        }

        private void _buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Utility.

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

        void PopulateContainerFromGround(ListView listView)
        {
            listView.Items.Clear();

            var itemUnderfoot = Core.Actors.Intersections(Core.Player)
                .Where(o => o.Meta.ActorClass == ActorClassName.ActorItem)
                .Cast<ActorItem>();

            foreach (var item in itemUnderfoot)
            {
                AddItemToListView(listView, item.TilePath, item.Meta);
            }
        }

        void PopulateContainerFromPack(ListView listView, TileIdentifier containerTile)
        {
            listView.Items.Clear();

            if (listView == listViewPlayerPack)
            {
                labelPack.Text = $"Pack: ({containerTile.Meta.Name})";
            }

            foreach (var item in Core.State.Items.Where(o => o.ContainerId == containerTile.Meta.UID))
            {
                AddItemToListView(listView, item.Tile.TilePath, item.Tile.Meta);
            }
        }

        private void AddItemToListView(ListView listView, string tilePath, TileMetadata meta)
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
                Tile = new TileIdentifier(tilePath, meta)
            };
            listView.Items.Add(item);
        }

        private void OpenPack(EquipTag item)
        {
            if (item.Tile.Meta.SubType == ActorSubType.Pack
                || item.Tile.Meta.SubType == ActorSubType.Belt
                || item.Tile.Meta.SubType == ActorSubType.Chest
                || item.Tile.Meta.SubType == ActorSubType.Purse)
            {
                _currentlySelectedPack = item.Tile;
                labelSelectedContainer.Text = $"Selected Container: ({item.Tile.Meta.Name})";
                PopulateContainerFromPack(listViewSelectedContainer, item.Tile);
            }
        }

        private ListViewItem FindListViewObjectByUid(ListView listView, Guid uid)
        {
            foreach (ListViewItem obj in listView.Items)
            {
                var item = obj.Tag as EquipTag;

                if (item.Tile.Meta.UID == uid)
                {
                    return obj;
                }
            }

            return null;
        }


        #endregion

    }
}
