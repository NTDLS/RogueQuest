using Game.Classes;
using Game.Engine;
using Library.Engine;
using Library.Engine.Types;
using Library.Native;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public partial class FormStore : Form
    {
        private Button _buttonClose = new Button();
        private TileIdentifier _currentlySelectedPack = null;
        private ToolTip _interrogationTip = new ToolTip();
        private TileMetadata _storeTileMeta;
        private PersistentStore _persistentStore;

        public EngineCore Core { get; set; }
        public FormStore()
        {
            InitializeComponent();
        }

        public FormStore(EngineCore core, TileMetadata storeTileMeta)
        {
            InitializeComponent();
            Core = core;
            _storeTileMeta = storeTileMeta;
        }

        private void FormStore_Load(object sender, EventArgs e)
        {
            _buttonClose.Click += _buttonClose_Click;
            this.CancelButton = _buttonClose;

            listViewSelectedContainer.SmallImageList = StoreAndInventory.ImageList;
            listViewSelectedContainer.LargeImageList = StoreAndInventory.ImageList;
            listViewSelectedContainer.View = View.List;
            listViewSelectedContainer.ItemDrag += Generic_ItemDrag;
            listViewSelectedContainer.DragEnter += Generic_DragEnter;
            listViewSelectedContainer.DragDrop += Generic_DragDrop;
            listViewSelectedContainer.AllowDrop = true;
            listViewSelectedContainer.MouseDoubleClick += Generic_MouseDoubleClick;
            listViewSelectedContainer.MouseUp += ListView_Shared_MouseUp;
            listViewSelectedContainer.MouseDown += ListView_Shared_MouseDown;

            listViewStore.SmallImageList = StoreAndInventory.ImageList;
            listViewStore.LargeImageList = StoreAndInventory.ImageList;
            listViewStore.View = View.List;
            listViewStore.ItemDrag += ListViewStore_ItemDrag;
            listViewStore.DragEnter += ListViewStore_DragEnter;
            listViewStore.DragDrop += ListViewStore_DragDrop;
            listViewStore.AllowDrop = true;
            listViewStore.MouseDoubleClick += ListViewStore_MouseDoubleClick;
            listViewStore.MouseUp += ListView_Shared_MouseUp;
            listViewStore.MouseDown += ListView_Store_MouseDown;

            listViewPlayerPack.SmallImageList = StoreAndInventory.ImageList;
            listViewPlayerPack.LargeImageList = StoreAndInventory.ImageList;
            listViewPlayerPack.View = View.List;
            listViewPlayerPack.ItemDrag += Generic_ItemDrag;
            listViewPlayerPack.DragEnter += Generic_DragEnter;
            listViewPlayerPack.DragDrop += Generic_DragDrop;
            listViewPlayerPack.AllowDrop = true;
            listViewPlayerPack.MouseDoubleClick += Generic_MouseDoubleClick;
            listViewPlayerPack.MouseUp += ListView_Shared_MouseUp;
            listViewPlayerPack.MouseDown += ListView_Shared_MouseDown;

            InitEquipSlot(listViewArmor, ActorSubType.Armor, EquipSlot.Armor);
            InitEquipSlot(listViewBracers, ActorSubType.Bracers, EquipSlot.Bracers);
            InitEquipSlot(listViewWeapon, new ActorSubType[] { ActorSubType.MeleeWeapon, ActorSubType.RangedWeapon }, EquipSlot.Weapon);
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
            InitEquipSlot(listViewQuiver1, ActorSubType.Projectile, EquipSlot.Projectile1);
            InitEquipSlot(listViewQuiver2, ActorSubType.Projectile, EquipSlot.Projectile2);
            InitEquipSlot(listViewQuiver3, ActorSubType.Projectile, EquipSlot.Projectile3);
            InitEquipSlot(listViewQuiver4, ActorSubType.Projectile, EquipSlot.Projectile4);
            InitEquipSlot(listViewQuiver5, ActorSubType.Projectile, EquipSlot.Projectile5);

            //If we are wearing a pack, go ahead and show its contents.
            var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack);
            if (pack.Tile != null)
            {
                PopulateContainerFromPack(listViewPlayerPack, pack.Tile);
            }

            _persistentStore = PopulateContainerFromStore(listViewStore);

            this.Text = _storeTileMeta.DisplayName;
        }

        #region ListViewStore

        private void ListViewStore_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

            if (listViewStore.SelectedItems?.Count != 1)
            {
                return;
            }

            var item = listViewStore.SelectedItems[0].Tag as EquipTag;
            if (item.Tile.Meta.SubType == ActorSubType.Pack
                || item.Tile.Meta.SubType == ActorSubType.Chest
                || item.Tile.Meta.SubType == ActorSubType.Purse)
            {
                OpenPack(item);
            }
        }

        private void ListViewStore_DragDrop(object sender, DragEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

            var destination = sender as ListView;
            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            var draggedItemTag = draggedItem.Tag as EquipTag;

            if (destination == draggedItem.ListView)
            {
                return;
            }

            var inventoryItem = Core.State.GetOrCreateInventoryItem(draggedItemTag.Tile);

            if (inventoryItem.Tile.Meta.SubType == ActorSubType.Money)
            {
                Constants.Alert("You probably want to keep that!");
                return;
            }

            List<ActorSubType> subtypes = GetStoreItemTypes();

            if (subtypes.Contains(inventoryItem.Tile.Meta.SubType ?? ActorSubType.Unspecified) == false)
            {
                Constants.Alert("I'm sorry, we don't buy those.");
                return;
            }

            //If we are draging from the primary pack slot, then close the pack.
            if (inventoryItem.Tile.Meta.SubType == ActorSubType.Pack
                || inventoryItem.Tile.Meta.SubType == ActorSubType.Chest)
            {
                var currentPackItems = Core.State.Items.Where(o => o.ContainerId == inventoryItem.Tile.Meta.UID).Count();

                if (currentPackItems > 0)
                {
                    Constants.Alert("We only buy packs if they are empty!");
                    return;
                }
            }

            int offerPrice = StoreAndInventory.OfferPrice(Core, draggedItemTag.Tile);

            var result = MessageBox.Show($"I'll give you {offerPrice:N0} gold for that.\r\nTake the offer?",
                "Sell the item?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            Core.State.Character.AddMoney(offerPrice);
            if (_currentlySelectedPack != null && _currentlySelectedPack.Meta.SubType == ActorSubType.Purse)
            {
                var purse = Core.State.Character.GetEquipSlot(EquipSlot.Purse);
                if (purse.Tile != null)
                {
                    PopulateContainerFromPack(listViewSelectedContainer, purse.Tile);
                }
            }

            if (draggedItemTag.Tile.Meta.SubType != ActorSubType.Wand
                 && draggedItemTag.Tile.Meta.SubType != ActorSubType.Potion
                 && draggedItemTag.Tile.Meta.SubType != ActorSubType.Scroll
                 && draggedItemTag.Tile.Meta.SubType != ActorSubType.Book)
            {
                if (draggedItemTag.Tile.Meta.Enchantment != null && (draggedItemTag.Tile.Meta.IsIdentified ?? false) == false)
                {
                    draggedItemTag.Tile.Meta.Identify(Core);

                    if (draggedItemTag.Tile.Meta.Enchantment == EnchantmentType.Cursed)
                    {
                        MessageBox.Show($"This {draggedItemTag.Tile.Meta.SubType} is cursed, the shop owner gives you a scowl!", $"RougeQuest", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (draggedItemTag.Tile.Meta.Enchantment == EnchantmentType.Enchanted)
                    {
                        MessageBox.Show($"This {draggedItemTag.Tile.Meta.SubType} has been enchanted, the shop is delighted!", $"RougeQuest", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (draggedItemTag.Tile.Meta.Enchantment == EnchantmentType.Normal)
                    {
                        MessageBox.Show($"This is just a normal {draggedItemTag.Tile.Meta.SubType}!", $"RougeQuest", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                draggedItemTag.Tile.Meta.Identify(Core);
            }

            //We do not stack items when sold to the store.

            StoreAndInventory.AddItemToListView(listViewStore, draggedItemTag.Tile);
            _persistentStore.Items.Add(draggedItemTag.Tile);

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

        private void ListViewStore_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void ListViewStore_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

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

        private void Generic_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

            var listView = sender as ListView;

            TileIdentifier pack = _currentlySelectedPack;

            if (sender == listViewPlayerPack)
            {
                pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack)?.Tile;
                if (pack == null)
                {
                    return;
                }
            }

            if (listView.SelectedItems?.Count != 1)
            {
                return;
            }

            var selectedListviewItem = listView.SelectedItems[0];
            var item = selectedListviewItem.Tag as EquipTag;

            string message = $"Read {item.Tile.Meta.DisplayName} to learn new spell?";

            if (MessageBox.Show(message, $"RougeQuest :: Use Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Core.State.Character.AddKnownSpell(item.Tile);
                Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == item.Tile.Meta.UID);
                listView.Items.Remove(selectedListviewItem);

                var slotToVacate = Core.State.Character.FindEquipSlotByItemId(item.Tile.Meta.UID);
                if (slotToVacate != null)
                {
                    slotToVacate.Tile = null;
                }
                Core.LogLine($"You learned a new spell, {item.Tile.Meta.SpellName}!");
            }
            else if (item.Tile.Meta.SubType == ActorSubType.Pack
                || item.Tile.Meta.SubType == ActorSubType.Chest
                || item.Tile.Meta.SubType == ActorSubType.Purse)
            {
                OpenPack(item);
            }
            else if (item.Tile.Meta.CanStack == true)
            {
                using (var splitForm = new FormSplitQuantity(item.Tile))
                {
                    if (splitForm.ShowDialog() == DialogResult.OK)
                    {
                        var newTile = item.Tile.DeriveCopy();

                        if (item.Tile.Meta.Quantity > 0)
                        {
                            newTile.Meta.Quantity = splitForm.SplitQuantity;
                            item.Tile.Meta.Quantity = ((int)(item.Tile.Meta.Quantity ?? 0)) - splitForm.SplitQuantity;
                        }

                        var newInventoryItem = Core.State.GetOrCreateInventoryItem(newTile);
                        newInventoryItem.ContainerId = pack.Meta.UID;
                    }
                }

                PopulateContainerFromPack(listView, pack);
            }
        }

        private void Generic_DragDrop(object sender, DragEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

            TileIdentifier pack;

            if (sender == listViewSelectedContainer)
            {
                if (_currentlySelectedPack == null)
                {
                    Constants.Alert("You havn't opened a container yet.");
                    return;
                }
                pack = _currentlySelectedPack;
            }
            else if (sender == listViewPlayerPack)
            {
                pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack)?.Tile;
                if (pack == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }

            var destination = sender as ListView;
            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            var draggedItemTag = draggedItem.Tag as EquipTag;

            if (destination == draggedItem.ListView)
            {
                return;
            }

            if (pack.Meta.SubType == ActorSubType.Purse && draggedItemTag.Tile.Meta.SubType != ActorSubType.Money)
            {
                Constants.Alert("You can only store coins in the purse");
                return;
            }
            if (pack.Meta.UID == draggedItemTag.Tile.Meta.UID)
            {
                //A container cannot contain itsself.
                Constants.Alert($"A {pack.Meta.DisplayName} cannot contain itself.");
                return;
            }
            if (Core.State.GetInventoryItem(draggedItemTag.Tile)?.ContainerId == (Guid)pack.Meta.UID)
            {
                //No need to do anything if we are dragging to the same container.
                return;
            }

            double maxBulk = pack.Meta.BulkCapacity ?? 0;
            double maxWeight = pack.Meta.WeightCapacity ?? 0;
            int? maxItems = pack.Meta.ItemCapacity;

            int askingPrice = StoreAndInventory.AskingPrice(Core, draggedItemTag.Tile);
            int quantityToMove = (draggedItemTag.Tile.Meta.Quantity ?? 1);

            if (draggedItem.ListView == listViewStore)
            {
                if (draggedItemTag.Tile.Meta.CanStack == true && draggedItemTag.Tile.Meta.Quantity > 0)
                {
                    using (var formQty = new FormStoreQuantity(Core, draggedItemTag.Tile))
                    {
                        if (formQty.ShowDialog() != DialogResult.OK)
                        {
                            return;
                        }

                        quantityToMove = formQty.QuantityToBuy;
                        askingPrice = StoreAndInventory.AskingPrice(Core, draggedItemTag.Tile, quantityToMove);
                    }
                }
                else
                {
                    var result = MessageBox.Show($"Item will cost you {askingPrice:N0} gold.\r\nPay the store?",
                        "Buy the item?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }
                }

                if (askingPrice > Core.State.Character.Money)
                {
                    Constants.Alert("You don't have enough money!");
                    return;
                }
            }

            //Do weight/bulk math.
            var currentPackWeight = Core.State.Items.Where(o => o.ContainerId == pack.Meta.UID).Sum(o => (o.Tile.Meta.Weight ?? 0) * (o.Tile.Meta.Quantity ?? 1));
            if ((draggedItemTag.Tile.Meta.Weight * quantityToMove) + currentPackWeight > maxWeight)
            {
                Constants.Alert($"{draggedItemTag.Tile.Meta.DisplayName} is too heavy for your {pack.Meta.DisplayName}. Drop something or move to free hand?");
                return;
            }

            var currentPackBulk = Core.State.Items.Where(o => o.ContainerId == pack.Meta.UID).Sum(o => (o.Tile.Meta.Bulk ?? 0) * (o.Tile.Meta.Quantity ?? 1));
            if ((draggedItemTag.Tile.Meta.Bulk * quantityToMove) + currentPackBulk > maxBulk)
            {
                Constants.Alert($"{draggedItemTag.Tile.Meta.DisplayName} is too bulky for your {pack.Meta.DisplayName}. Drop something or move to free hand?");
                return;
            }

            if (maxItems != null)
            {
                var currentPackItems = Core.State.Items.Where(o => o.ContainerId == pack.Meta.UID).Count();
                if (currentPackItems + 1 > (int)maxItems)
                {
                    Constants.Alert($"{pack.Meta.DisplayName} can only carry {maxItems} items. Drop something or move to free hand?");
                    return;
                }
            }

            //If we are draging from the primary pack slot, then close the pack.
            if (draggedItem.ListView == listViewPack)
            {
                listViewPlayerPack.Items.Clear();
            }

            if (askingPrice > 0)
            {
                Core.State.Character.DeductMoney(askingPrice);
                if (pack != null && pack.Meta.SubType == ActorSubType.Purse)
                {
                    var purse = Core.State.Character.GetEquipSlot(EquipSlot.Purse);
                    if (purse.Tile != null)
                    {
                        PopulateContainerFromPack(destination, purse.Tile);
                    }
                }
            }

            bool wasStacked = false;

            if (draggedItemTag.Tile.Meta.CanStack == true)
            {
                //If we are dragging to a container and the container already contains some of the stackable stuff, then stack!
                var existingItemInDestinationContainer = Core.State.Items
                    .Where(o => StoreAndInventory.IsStackMatch(o, draggedItemTag.Tile)
                    && o.ContainerId == pack.Meta.UID).FirstOrDefault();

                if (existingItemInDestinationContainer != null)
                {
                    existingItemInDestinationContainer.Tile.Meta.Quantity = (existingItemInDestinationContainer.Tile.Meta.Quantity ?? 0) + quantityToMove;
                    draggedItemTag.Tile.Meta.Quantity = (draggedItemTag.Tile.Meta.Quantity ?? 0) - quantityToMove;

                    if ((draggedItemTag.Tile.Meta.Quantity ?? 0) == 0)
                    {
                        Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == draggedItemTag.Tile.Meta.UID);
                    }
                    var listViewItem = StoreAndInventory.FindListViewObjectByUid((ListView)sender, (Guid)existingItemInDestinationContainer.Tile.Meta.UID);
                    if (listViewItem != null)
                    {
                        string text = existingItemInDestinationContainer.Tile.Meta.DisplayName;

                        if (existingItemInDestinationContainer.Tile.Meta.CanStack == true && existingItemInDestinationContainer.Tile.Meta.Quantity > 0)
                        {
                            text += $" ({existingItemInDestinationContainer.Tile.Meta.Quantity})";
                        }
                        else if (existingItemInDestinationContainer.Tile.Meta.CanStack == false && existingItemInDestinationContainer.Tile.Meta.Charges > 0)
                        {
                            text += $" ({existingItemInDestinationContainer.Tile.Meta.Charges})";
                        }

                        (listViewItem.Tag as EquipTag).Tile.Meta.Quantity = existingItemInDestinationContainer.Tile.Meta.Quantity;
                        listViewItem.Text = text;
                    }

                    wasStacked = true;
                }
            }

            if (wasStacked == false)
            {
                //Are we moving the whole stack or only part of it?
                if (draggedItemTag.Tile.Meta.CanStack == true || quantityToMove != draggedItemTag.Tile.Meta.Quantity)
                {
                    //Move part of the stack.
                    var clonedItem = draggedItemTag.Tile.DeriveCopy(); //Create a copy of the item with a new ID
                    clonedItem.Meta.Quantity = quantityToMove;
                    StoreAndInventory.AddItemToListView((ListView)sender, clonedItem);

                    var newInventoryItem = Core.State.GetOrCreateInventoryItem(clonedItem);
                    newInventoryItem.ContainerId = (Guid)pack.Meta.UID;

                    draggedItemTag.Tile.Meta.Quantity = draggedItemTag.Tile.Meta.Quantity - quantityToMove;
                }
                else
                {
                    //Move the whole stack.
                    var itemInPlayerInventory = Core.State.GetInventoryItem(draggedItemTag.Tile);
                    StoreAndInventory.AddItemToListView((ListView)sender, draggedItemTag.Tile);
                    itemInPlayerInventory.ContainerId = (Guid)pack.Meta.UID;
                }
            }

            if (draggedItem.ListView == listViewStore)
            {
                if ((draggedItemTag.Tile.Meta.CanStack ?? false) == false
                    || (draggedItemTag.Tile.Meta.CanStack == true && (draggedItemTag.Tile.Meta.Quantity ?? 0) == 0))
                {
                    draggedItem.ListView.Items.Remove(draggedItem);
                    _persistentStore.Items.RemoveAll(o => o.Meta.UID == draggedItemTag.Tile.Meta.UID);
                }
                else
                {
                    string text = draggedItemTag.Tile.Meta.DisplayName;
                    text += $" ({draggedItemTag.Tile.Meta.Quantity})";
                    draggedItem.Text = text;
                }
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

        private void Generic_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void Generic_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

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


        #endregion

        #region Equip Slot.

        private void InitEquipSlot(ListView lv, ActorSubType acceptType, EquipSlot slot)
        {
            var acceptTypeList = new List<ActorSubType>();
            acceptTypeList.Add(acceptType);
            InitEquipSlot(lv, acceptTypeList.ToArray(), slot);
        }

        private void InitEquipSlot(ListView lv, ActorSubType[] acceptTypes, EquipSlot slot)
        {
            lv.HideSelection = true;
            lv.LargeImageList = StoreAndInventory.ImageList;
            lv.SmallImageList = StoreAndInventory.ImageList;
            lv.Scrollable = false;

            if (slot != EquipSlot.Purse)
            {
                lv.AllowDrop = true;
                lv.DragDrop += ListView_EquipSlot_DragDrop;
                lv.DragEnter += ListView_EquipSlot_DragEnter;
                lv.ItemDrag += ListView_EquipSlot_ItemDrag;
                lv.MouseDown += ListView_EquipSlot_MouseDown;
                lv.MouseUp += ListView_EquipSlot_MouseUp;
            }

            lv.MouseDoubleClick += ListView_EquipSlot_MouseDoubleClick;

            ListViewItem item = new ListViewItem("");
            item.Tag = new EquipTag()
            {
                AcceptTypes = acceptTypes.ToList(),
                Slot = slot
            };

            var equipSlot = Core.State.Character.GetEquipSlot(slot);
            if (equipSlot.Tile != null)
            {
                string text = equipSlot.Tile.Meta.DisplayName;

                if (equipSlot.Tile.Meta.CanStack == true && equipSlot.Tile.Meta.Quantity > 0)
                {
                    text += $" ({equipSlot.Tile.Meta.Quantity})";
                }
                else if (equipSlot.Tile.Meta.CanStack == false && equipSlot.Tile.Meta.Charges > 0)
                {
                    text += $" ({equipSlot.Tile.Meta.Charges})";
                }

                item.Text = text;
                item.ImageKey = StoreAndInventory.GetImageKey(equipSlot.Tile.ImagePath);
                (item.Tag as EquipTag).Tile = equipSlot.Tile;
            }

            lv.Items.Add(item);
        }

        private void ListView_EquipSlot_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var lv = sender as ListView;
                var item = lv.Items[0].Tag as EquipTag;
                var location = new Point(e.X + 10, e.Y - 25);
                _interrogationTip.Show(StoreAndInventory.GetItemTip(Core, item.Tile, true), lv, location, 5000);
            }
        }

        private void ListView_EquipSlot_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _interrogationTip.Hide(sender as Control);
            }
        }

        private void ListView_EquipSlot_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;
            var listView = sender as ListView;

            if (listView.SelectedItems?.Count != 1)
            {
                return;
            }

            if (listView == listViewPack)
            {
                //Only open the worm pack to the main container view.
                var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack);
                if (pack.Tile != null)
                {
                    PopulateContainerFromPack(listViewPlayerPack, pack.Tile);
                }
            }
            else
            {
                var item = listView.SelectedItems[0].Tag as EquipTag;
                if (item.Tile.Meta.SubType == ActorSubType.Pack
                    || item.Tile.Meta.SubType == ActorSubType.Chest
                    || item.Tile.Meta.SubType == ActorSubType.Purse)
                {
                    OpenPack(item);
                }
            }
        }

        private void ListView_EquipSlot_DragEnter(object sender, DragEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

            var destination = sender as ListView;
            var destinationTag = destination.Items[0].Tag as EquipTag;

            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            var draggedItemTag = draggedItem.Tag as EquipTag;

            if (destination == draggedItem.ListView || destinationTag.Tile != null)
            {
                return;
            }

            if ((draggedItemTag.Tile.Meta.SubType != null && destinationTag.AcceptTypes.Contains((ActorSubType)draggedItemTag.Tile.Meta.SubType))
                || destinationTag.AcceptTypes.Contains(ActorSubType.Unspecified))
            {
                e.Effect = e.AllowedEffect;
            }
        }

        private void ListView_EquipSlot_DragDrop(object sender, DragEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

            var destination = sender as ListView;
            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            if (destination == draggedItem.ListView)
            {
                return;
            }

            var draggedItemTag = draggedItem.Tag as EquipTag;
            var clonedItem = draggedItemTag.Tile.Clone(true);
            int askingPrice = StoreAndInventory.AskingPrice(Core, draggedItemTag.Tile);
            int quantityToMove = (draggedItemTag.Tile.Meta.Quantity ?? 1);

            if (draggedItem.ListView == listViewStore)
            {
                if (draggedItemTag.Tile.Meta.CanStack == true && draggedItemTag.Tile.Meta.Quantity > 0)
                {
                    using (var formQty = new FormStoreQuantity(Core, draggedItemTag.Tile))
                    {
                        if (formQty.ShowDialog() != DialogResult.OK)
                        {
                            return;
                        }

                        quantityToMove = formQty.QuantityToBuy;
                        askingPrice = StoreAndInventory.AskingPrice(Core, draggedItemTag.Tile, quantityToMove);
                    }
                }
                else
                {
                    var result = MessageBox.Show($"Item will cost you {askingPrice:N0} gold.\r\nPay the store?",
                        "Buy the item?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }
                }

                if (askingPrice > Core.State.Character.Money)
                {
                    Constants.Alert("You don't have enough money!");
                    return;
                }

                Core.State.Character.DeductMoney(askingPrice);
                if (_currentlySelectedPack != null && _currentlySelectedPack.Meta.SubType == ActorSubType.Purse)
                {
                    var purse = Core.State.Character.GetEquipSlot(EquipSlot.Purse);
                    if (purse.Tile != null)
                    {
                        PopulateContainerFromPack(listViewSelectedContainer, purse.Tile);
                    }
                }
            }

            //If we are draging from the primary pack slot, then close the pack.
            if (draggedItem.ListView == listViewPack)
            {
                listViewPlayerPack.Items.Clear();
            }

            var destinationTag = destination.Items[0].Tag as EquipTag;

            if ((draggedItemTag.Tile.Meta.SubType != null && destinationTag.AcceptTypes.Contains((ActorSubType)draggedItemTag.Tile.Meta.SubType))
                || destinationTag.AcceptTypes.Contains(ActorSubType.Unspecified))
            {
                if (draggedItemTag.Tile.Meta.SubType != ActorSubType.Wand
                     && draggedItemTag.Tile.Meta.SubType != ActorSubType.Potion
                     && draggedItemTag.Tile.Meta.SubType != ActorSubType.Scroll
                     && draggedItemTag.Tile.Meta.SubType != ActorSubType.Book)
                {
                    if (draggedItemTag.Tile.Meta.Enchantment != null && (draggedItemTag.Tile.Meta.IsIdentified ?? false) == false)
                    {
                        draggedItemTag.Tile.Meta.Identify(Core);

                        if (draggedItemTag.Tile.Meta.Enchantment == EnchantmentType.Cursed)
                        {
                            MessageBox.Show($"This {draggedItemTag.Tile.Meta.SubType} is cursed and cannot be removed by conventional means!", $"RougeQuest", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (draggedItemTag.Tile.Meta.Enchantment == EnchantmentType.Enchanted)
                        {
                            MessageBox.Show($"This {draggedItemTag.Tile.Meta.SubType} has been enchanted!", $"RougeQuest", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (draggedItemTag.Tile.Meta.Enchantment == EnchantmentType.Normal)
                        {
                            MessageBox.Show($"This is just a normal {draggedItemTag.Tile.Meta.SubType}!", $"RougeQuest", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

                destination.Items[0].ImageKey = StoreAndInventory.GetImageKey(draggedItemTag.Tile.ImagePath);
                destination.Items[0].Text = draggedItemTag.Tile.Meta.DisplayName;
                destinationTag.Tile = draggedItemTag.Tile.Clone();

                var equipSlot = Core.State.Character.GetEquipSlot(destinationTag.Slot);
                equipSlot.Tile = destinationTag.Tile;
                equipSlot.Tile.Meta.Quantity = quantityToMove;

                if (draggedItem.ListView == listViewStore)
                {
                    Core.State.Items.Add(new CustodyItem()
                    {
                        Tile = clonedItem
                    });
                }

                //If the item is in a container, find the item and set its container to NULL.
                var inventoryItem = Core.State.GetOrCreateInventoryItem(draggedItemTag.Tile);
                if (inventoryItem != null)
                {
                    inventoryItem.ContainerId = null; //find the item in inventory and set its container id to null.
                }

                if (draggedItem.ListView == listViewStore)
                {
                    if (draggedItemTag.Tile.Meta.CanStack == true)
                    {
                        draggedItemTag.Tile.Meta.Quantity -= quantityToMove;
                    }

                    //If we moved all of the product from a store, then remove the item from the store.
                    if ((draggedItemTag.Tile.Meta.Quantity ?? 0) == 0)
                    {
                        draggedItem.ListView.Items.Remove(draggedItem);
                        _persistentStore.Items.RemoveAll(o => o.Meta.UID == draggedItemTag.Tile.Meta.UID);
                    }
                    else
                    {
                        string text = draggedItemTag.Tile.Meta.DisplayName;
                        text += $" ({draggedItemTag.Tile.Meta.Quantity})";
                        draggedItem.Text = text;
                    }
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
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                var draggedItem = e.Item as ListViewItem;
                var draggedItemTag = draggedItem.Tag as EquipTag;

                if (draggedItemTag?.Tile?.Meta.Enchantment == EnchantmentType.Cursed)
                {
                    MessageBox.Show($"This {draggedItemTag.Tile.Meta.SubType} is cursed and cannot be removed by conventional means!", $"RougeQuest", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DoDragDrop(e.Item, DragDropEffects.Move);
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
                var location = new Point(e.X + 10, e.Y - 25);
                _interrogationTip.Show(StoreAndInventory.GetItemTip(Core, item.Tile, true), lv, location, 5000);
            }
        }

        private void ListView_Store_MouseDown(object sender, MouseEventArgs e)
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

                var location = new Point(e.X + 10, e.Y - 25);
                _interrogationTip.Show(StoreAndInventory.GetItemTip(Core, item.Tile, false, true), lv, location, 5000);
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
            if (Core.Tick.IsEngineBusy) return;
            this.Close();
        }

        #endregion

        #region Utility.

        private List<ActorSubType> GetStoreItemTypes()
        {
            var subtypes = new List<ActorSubType>();

            if (_storeTileMeta.SubType == ActorSubType.WeaponSmithStore)
            {
                subtypes.Add(ActorSubType.MeleeWeapon);
                subtypes.Add(ActorSubType.RangedWeapon);
                subtypes.Add(ActorSubType.Projectile);
            }
            else if (_storeTileMeta.SubType == ActorSubType.ArmorSmithStore)
            {
                subtypes.Add(ActorSubType.Armor);
                subtypes.Add(ActorSubType.Helment);
                subtypes.Add(ActorSubType.Bracers);
                subtypes.Add(ActorSubType.Gauntlets);
                subtypes.Add(ActorSubType.Shield);
            }
            else if (_storeTileMeta.SubType == ActorSubType.GeneralStore)
            {
                subtypes.Add(ActorSubType.Boots);
                subtypes.Add(ActorSubType.Belt);
                subtypes.Add(ActorSubType.Garment);
                subtypes.Add(ActorSubType.Chest);
                subtypes.Add(ActorSubType.Pack);
                subtypes.Add(ActorSubType.Necklace);
                subtypes.Add(ActorSubType.Ring);
            }
            else if (_storeTileMeta.SubType == ActorSubType.AlchemistStore)
            {
                subtypes.Add(ActorSubType.Potion);
            }
            else if (_storeTileMeta.SubType == ActorSubType.MageStore)
            {
                subtypes.Add(ActorSubType.Book);
                subtypes.Add(ActorSubType.Scroll);
                subtypes.Add(ActorSubType.Wand);
            }

            return subtypes;
        }

        PersistentStore PopulateContainerFromStore(ListView listView)
        {
            listView.Items.Clear();

            List<ActorSubType> subtypes = GetStoreItemTypes();

            var persistentStore = Core.State.Stores.Where(o => o.StoreID == _storeTileMeta.UID).FirstOrDefault();

            //Populate the store once now, then again every 1 game hour.
            if (persistentStore == null || Core.State.TimePassed - persistentStore.GameTime > 3600)
            {
                if (persistentStore == null)
                {
                    persistentStore = new PersistentStore()
                    {
                        StoreID = (Guid)_storeTileMeta.UID,
                        GameTime = Core.State.TimePassed,
                        Items = new List<TileIdentifier>()
                    };
                }

                List<TileIdentifier> itemsInStore = new List<TileIdentifier>();

                for (int i = 0; i < 2; i++)
                {
                    itemsInStore.AddRange(Core.Materials
                        .Where(o =>
                            o.Meta.ActorClass == ActorClassName.ActorItem //We don't sell terrain blocks or buildings. :D
                            && o.Meta.Value > 0 //We don't sell free items.
                            && (
                                (persistentStore.Items.Where(i => i.TilePath == o.TilePath).Count() < 3 && (o.Meta.CanStack ?? false) == false) //Don't add too many duplicate items.
                                || (persistentStore.Items.Where(i => i.TilePath == o.TilePath).Any() == false && o.Meta.CanStack == true) //Don't add duplicate stackable items.
                            )
                            && (
                                (itemsInStore.Where(i => i.TilePath == o.TilePath).Count() < 3 && (o.Meta.CanStack ?? false) == false) //Don't add too many duplicate items.
                                || (itemsInStore.Where(i => i.TilePath == o.TilePath).Any() == false && o.Meta.CanStack == true) //Don't add duplicate stackable items.
                            )
                            && subtypes.Contains(o.Meta.SubType ?? ActorSubType.Unspecified) //Only show the items that this store type sells.
                            && o.Meta.Enchantment != EnchantmentType.Cursed //We dont sell cursed items.
                            && (o.Meta.Prevalence ?? 0) > 0 //Items with a prevalence of 0 are impossible to find or buy. They have to be placed by the map creator.
                            && (o.Meta.Prevalence == 100 || MathUtility.Random.Next(1, 100) <= (int)o.Meta.Prevalence) //Apply the prevalence lottery.
                            && o.Meta.Level <= Core.State.Character.Level //Only show items that are appropriate for the character level.
                        ).Cast<TileIdentifier>().Select(o=>o.DeriveCopy()));

                    itemsInStore.ForEach(o => o.Meta.Identify(Core));
                    itemsInStore.RemoveAll(o => o.Meta.Enchantment == EnchantmentType.Cursed);
                }

                foreach (var item in itemsInStore)
                {
                    int max = (int)item.Meta.Prevalence / 4;
                    if (max < 5) max = 5;
                    if (max > 20) max = 20;

                    if (item.Meta.CanStack == true)
                    {
                        item.Meta.Quantity = MathUtility.RandomNumber(4, max);
                    }
                    else if (item.Meta.SubType == ActorSubType.Wand)
                    {
                        item.Meta.Charges = MathUtility.RandomNumber(4, max);
                    }

                    persistentStore.Items.Add(item);
                }

                Core.State.Stores.Add(persistentStore);
            }

            foreach (var item in persistentStore.Items)
            {
                StoreAndInventory.AddItemToListView(listView, item);
            }

            listView.Sorting = SortOrder.Ascending;
            listView.Sort();

            return persistentStore;
        }

        void PopulateContainerFromPack(ListView listView, TileIdentifier containerTile)
        {
            listView.Items.Clear();

            labelPack.Text = $"Pack: ({containerTile.Meta.DisplayName})";

            foreach (var item in Core.State.Items.Where(o => o.ContainerId == containerTile.Meta.UID))
            {
                StoreAndInventory.AddItemToListView(listView, item.Tile);
            }

            listView.Sorting = SortOrder.Ascending;
            listView.Sort();
        }

        private void OpenPack(EquipTag item)
        {
            if (item.Tile.Meta.SubType == ActorSubType.Pack
                || item.Tile.Meta.SubType == ActorSubType.Chest
                || item.Tile.Meta.SubType == ActorSubType.Purse)
            {
                _currentlySelectedPack = item.Tile;
                labelSelectedContainer.Text = $"Selected Container: ({item.Tile.Meta.DisplayName})";
                PopulateContainerFromPack(listViewSelectedContainer, item.Tile);
            }
        }

        #endregion

    }
}
