using Assets;
using Game.Classes;
using Game.Engine;
using Library.Engine;
using Library.Engine.Types;
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
        private ImageList _imageList = new ImageList();
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

            listViewSelectedContainer.SmallImageList = _imageList;
            listViewSelectedContainer.LargeImageList = _imageList;
            listViewSelectedContainer.View = View.List;
            listViewSelectedContainer.ItemDrag += Generic_ItemDrag;
            listViewSelectedContainer.DragEnter += Generic_DragEnter;
            listViewSelectedContainer.DragDrop += Generic_DragDrop;
            listViewSelectedContainer.AllowDrop = true;
            listViewSelectedContainer.MouseDoubleClick += Generic_MouseDoubleClick;
            listViewSelectedContainer.MouseUp += ListView_Shared_MouseUp;
            listViewSelectedContainer.MouseDown += ListView_Shared_MouseDown;

            listViewStore.SmallImageList = _imageList;
            listViewStore.LargeImageList = _imageList;
            listViewStore.View = View.List;
            listViewStore.ItemDrag += ListViewStore_ItemDrag;
            listViewStore.DragEnter += ListViewStore_DragEnter;
            listViewStore.DragDrop += ListViewStore_DragDrop;
            listViewStore.AllowDrop = true;
            listViewStore.MouseDoubleClick += ListViewStore_MouseDoubleClick;
            listViewStore.MouseUp += ListView_Shared_MouseUp;
            listViewStore.MouseDown += ListView_Store_MouseDown;

            listViewPlayerPack.SmallImageList = _imageList;
            listViewPlayerPack.LargeImageList = _imageList;
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

            this.Text = _storeTileMeta.Name;
        }

        #region ListViewStore

        private void ListViewStore_MouseDoubleClick(object sender, MouseEventArgs e)
        {
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

            //We do nto stack items when sold to the store.

            AddItemToListView(listViewStore, draggedItemTag.Tile.TilePath, draggedItemTag.Tile.Meta);
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
            var listView = listViewSelectedContainer as ListView;

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

            var item = listView.SelectedItems[0].Tag as EquipTag;
            if (item.Tile.Meta.SubType == ActorSubType.Pack
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

            int maxBulk = (int)pack.Meta.BulkCapacity;
            int maxWeight = (int)pack.Meta.WeightCapacity;
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

            //Do weight/bulk math.
            var currentPackWeight = Core.State.Items.Where(o => o.ContainerId == pack.Meta.UID).Sum(o => o.Tile.Meta.Weight * (o.Tile.Meta.Quantity ?? 1));
            if ((draggedItemTag.Tile.Meta.Weight * quantityToMove) + currentPackWeight > maxWeight)
            {
                Constants.Alert($"{draggedItemTag.Tile.Meta.Name} is too heavy for your {pack.Meta.Name}. Drop something or move to free hand?");
                return;
            }

            var currentPackBulk = Core.State.Items.Where(o => o.ContainerId == pack.Meta.UID).Sum(o => o.Tile.Meta.Bulk * (o.Tile.Meta.Quantity ?? 1));
            if((draggedItemTag.Tile.Meta.Bulk * quantityToMove) + currentPackBulk > maxBulk)
            {
                Constants.Alert($"{draggedItemTag.Tile.Meta.Name} is too bulky for your {pack.Meta.Name}. Drop something or move to free hand?");
                return;
            }

            if (maxItems != null)
            {
                var currentPackItems = Core.State.Items.Where(o => o.ContainerId == pack.Meta.UID).Count();
                if (currentPackItems + 1 > (int)maxItems)
                {
                    Constants.Alert($"{pack.Meta.Name} can only carry {maxItems} items. Drop something or move to free hand?");
                    return;
                }
            }

            //If we are draging from the primary pack slot, then close the pack.
            if (draggedItem.ListView == listViewPack)
            {
                listViewPlayerPack.Items.Clear();
            }

            if (pack.Meta.UID == draggedItemTag.Tile.Meta.UID)
            {
                //A container cannot contain itsself.
                Constants.Alert($"A {pack.Meta.Name} cannot contain itself.");
                return;
            }
            if (Core.State.GetInventoryItem(draggedItemTag.Tile)?.ContainerId == (Guid)pack.Meta.UID)
            {
                //No need to do anything if we are dragging to the same container.
                return;
            }

            bool wasStacked = false;

            if (draggedItemTag.Tile.Meta.CanStack == true)
            {
                //If we are dragging to a container and the container already contains some of the stackable stuff, then stack!
                var existingItemInDestinationContainer = Core.State.Items
                    .Where(o => o.Tile.TilePath == draggedItemTag.Tile.TilePath
                    && o.ContainerId == pack.Meta.UID).FirstOrDefault();

                if (existingItemInDestinationContainer != null)
                {
                    existingItemInDestinationContainer.Tile.Meta.Quantity = (existingItemInDestinationContainer.Tile.Meta.Quantity ?? 0) + quantityToMove;
                    draggedItemTag.Tile.Meta.Quantity = (draggedItemTag.Tile.Meta.Quantity ?? 0) - quantityToMove;

                    if ((draggedItemTag.Tile.Meta.Quantity ?? 0) == 0)
                    {
                        Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == draggedItemTag.Tile.Meta.UID);
                    }
                    var listViewItem = FindListViewObjectByUid((ListView)sender, (Guid)existingItemInDestinationContainer.Tile.Meta.UID);
                    if (listViewItem != null)
                    {
                        string text = existingItemInDestinationContainer.Tile.Meta.Name;

                        if (existingItemInDestinationContainer.Tile.Meta.CanStack == true && existingItemInDestinationContainer.Tile.Meta.Quantity > 0)
                        {
                            text += $" ({existingItemInDestinationContainer.Tile.Meta.Quantity})";
                        }
                        else if (existingItemInDestinationContainer.Tile.Meta.CanStack == false && existingItemInDestinationContainer.Tile.Meta.Charges > 0)
                        {
                            text += $" ({existingItemInDestinationContainer.Tile.Meta.Charges})";
                        }

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
                    AddItemToListView((ListView)sender, clonedItem.TilePath, clonedItem.Meta);

                    var newInventoryItem = Core.State.GetOrCreateInventoryItem(clonedItem);
                    newInventoryItem.ContainerId = (Guid)pack.Meta.UID;

                    draggedItemTag.Tile.Meta.Quantity = draggedItemTag.Tile.Meta.Quantity - quantityToMove;
                }
                else
                {
                    //Move the whole stack.
                    var itemInPlayerInventory = Core.State.GetInventoryItem(draggedItemTag.Tile);
                    AddItemToListView((ListView)sender, draggedItemTag.Tile.TilePath, draggedItemTag.Tile.Meta);
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
                    string text = draggedItemTag.Tile.Meta.Name;
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
                AcceptTypes = acceptTypes.ToList(),
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
                else if (equipSlot.Tile.Meta.CanStack == false && equipSlot.Tile.Meta.Charges > 0)
                {
                    text += $" ({equipSlot.Tile.Meta.Charges})";
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

                string text = item.Tile.Meta.Name + $" ({StoreAndInventory.RarityText(item.Tile)})";

                text += "\r\n" + $"Type: {item.Tile.Meta.SubType}";

                if (item.Tile.Meta.CanStack == true && item.Tile.Meta.Quantity > 0)
                    text += "\r\n" + $"Quantity: {item.Tile.Meta.Quantity}";
                if (item.Tile.Meta.CanStack == false && item.Tile.Meta.Charges > 0)
                    text += "\r\n" + $"Charges: {item.Tile.Meta.Charges}";

                if (item.Tile.Meta.Weight != null) text += "\r\n" + $"Weight: {item.Tile.Meta.Weight:N0}";
                if (item.Tile.Meta.Bulk != null) text += "\r\n" + $"Bulk: {item.Tile.Meta.Bulk:N0}";
                if (item.Tile.Meta.AC != null) text += "\r\n" + $"AC: {item.Tile.Meta.AC:N0}";

                if (item.Tile.Meta.SubType == ActorSubType.MeleeWeapon || item.Tile.Meta.SubType == ActorSubType.RangedWeapon)
                    text += "\r\n" + $"Stats: {item.Tile.Meta.DndDamageText}";
                else if (item.Tile.Meta.SubType == ActorSubType.Money)
                    text += "\r\n" + $"Value: {((int)(item.Tile.Meta.Quantity * item.Tile.Meta.Value)):N0} gold";

                if (string.IsNullOrWhiteSpace(text) == false)
                {
                    var location = new Point(e.X + 10, e.Y - 25);
                    _interrogationTip.Show(text, lv, location, 5000);
                }

                text += "\r\n" + $"Offer: {StoreAndInventory.OfferPrice(Core, item.Tile):N0}";

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
            var destination = sender as ListView;
            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            if (destination == draggedItem.ListView)
            {
                return;
            }

            var draggedItemTag = draggedItem.Tag as EquipTag;
            var clonedItem = draggedItemTag.Tile.Clone(true);
            int askingPrice = StoreAndInventory.AskingPrice(Core, draggedItemTag.Tile);

            if (draggedItem.ListView == listViewStore)
            {
                var result = MessageBox.Show($"Item will cost you {askingPrice:N0} gold.\r\nPay the store?",
                    "Buy the item?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    return;
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

            _persistentStore.Items.RemoveAll(o => o.Meta.UID == draggedItemTag.Tile.Meta.UID);

            var destinationTag = destination.Items[0].Tag as EquipTag;

            if ((draggedItemTag.Tile.Meta.SubType != null && destinationTag.AcceptTypes.Contains((ActorSubType)draggedItemTag.Tile.Meta.SubType))
                || destinationTag.AcceptTypes.Contains(ActorSubType.Unspecified))
            {
                destination.Items[0].ImageKey = draggedItem.ImageKey;
                destination.Items[0].Text = draggedItem.Text;
                destinationTag.Tile = draggedItemTag.Tile.Clone();

                var equipSlot = Core.State.Character.GetEquipSlot(destinationTag.Slot);
                equipSlot.Tile = draggedItemTag.Tile;

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

                string text = item.Tile.Meta.Name + $" ({StoreAndInventory.RarityText(item.Tile)})";

                text += "\r\n" + $"Type: {item.Tile.Meta.SubType}";

                if (item.Tile.Meta.CanStack == true && item.Tile.Meta.Quantity > 0)
                    text += "\r\n" + $"Quantity: {item.Tile.Meta.Quantity}";
                if (item.Tile.Meta.CanStack == false && item.Tile.Meta.Charges > 0)
                    text += "\r\n" + $"Charges: {item.Tile.Meta.Charges}";

                if (item.Tile.Meta.Weight != null) text += "\r\n" + $"Weight: {item.Tile.Meta.Weight:N0}";
                if (item.Tile.Meta.Bulk != null) text += "\r\n" + $"Bulk: {item.Tile.Meta.Bulk:N0}";
                if (item.Tile.Meta.AC != null) text += "\r\n" + $"AC: {item.Tile.Meta.AC:N0}";

                if (item.Tile.Meta.SubType == ActorSubType.MeleeWeapon || item.Tile.Meta.SubType == ActorSubType.RangedWeapon)
                    text += "\r\n" + $"Stats: {item.Tile.Meta.DndDamageText}";
                else if (item.Tile.Meta.SubType == ActorSubType.Money)
                    text += "\r\n" + $"Value: {((int)((item.Tile.Meta.Quantity ?? 0) * item.Tile.Meta.Value)):N0} gold";

                text += "\r\n" + $"Offer: {StoreAndInventory.OfferPrice(Core, item.Tile):N0}";

                if (string.IsNullOrWhiteSpace(text) == false)
                {
                    var location = new Point(e.X + 10, e.Y - 25);
                    _interrogationTip.Show(text, lv, location, 5000);
                }
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

                string text = item.Tile.Meta.Name + $" ({StoreAndInventory.RarityText(item.Tile)})";

                text += "\r\n" + $"Type: {item.Tile.Meta.SubType}";

                if (item.Tile.Meta.CanStack == true && item.Tile.Meta.Quantity > 0)
                    text += "\r\n" + $"Quantity: {item.Tile.Meta.Quantity}";
                if (item.Tile.Meta.CanStack == false && item.Tile.Meta.Charges > 0)
                    text += "\r\n" + $"Charges: {item.Tile.Meta.Charges}";

                if (item.Tile.Meta.Weight != null) text += "\r\n" + $"Weight: {item.Tile.Meta.Weight:N0}";
                if (item.Tile.Meta.Bulk != null) text += "\r\n" + $"Bulk: {item.Tile.Meta.Bulk:N0}";
                if (item.Tile.Meta.AC != null) text += "\r\n" + $"AC: {item.Tile.Meta.AC:N0}";

                if (item.Tile.Meta.SubType == ActorSubType.MeleeWeapon || item.Tile.Meta.SubType == ActorSubType.RangedWeapon)
                    text += "\r\n" + $"Stats: {item.Tile.Meta.DndDamageText}";
                else if (item.Tile.Meta.SubType == ActorSubType.Money)
                    text += "\r\n" + $"Value: {((int)(item.Tile.Meta.Quantity * item.Tile.Meta.Value)):N0} gold";

                text += "\r\n" + $"Asking Price: {StoreAndInventory.AskingPrice(Core, item.Tile):N0}";

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
                subtypes.Add(ActorSubType.Boots);
                subtypes.Add(ActorSubType.Belt);
                subtypes.Add(ActorSubType.Garment);
                subtypes.Add(ActorSubType.Helment);
                subtypes.Add(ActorSubType.Gauntlets);
                subtypes.Add(ActorSubType.Shield);
            }
            else if (_storeTileMeta.SubType == ActorSubType.AlchemistStore)
            {
                subtypes.Add(ActorSubType.Potion);
            }
            else if (_storeTileMeta.SubType == ActorSubType.MageStore)
            {
                subtypes.Add(ActorSubType.Scroll);
                subtypes.Add(ActorSubType.Wand);
                subtypes.Add(ActorSubType.Necklace);
                subtypes.Add(ActorSubType.Ring);
            }

            return subtypes;
        }

        PersistentStore PopulateContainerFromStore(ListView listView)
        {
            listView.Items.Clear();

            List<ActorSubType> subtypes = GetStoreItemTypes();

            var persistentStore = Core.State.Stores.Where(o => o.StoreID == _storeTileMeta.UID).FirstOrDefault();

            //Populate the store once and then again every 1 game day.
            if (persistentStore == null || Core.State.TimePassed - persistentStore.GameTime > 1440)
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

                var itemsInStore = Core.Materials
                    .Where(o => o.Meta.ActorClass == ActorClassName.ActorItem
                        && o.Meta.Value > 0
                        && persistentStore.Items.Where(i => i.TilePath == o.TilePath).FirstOrDefault() == null //Don't add duplicate items.
                        && subtypes.Contains(o.Meta.SubType ?? ActorSubType.Unspecified)
                        && (o.Meta.Rarity ?? 0) > 0
                        && (o.Meta.Rarity == 100 || Library.Utility.MathUtility.ChanceIn(100 - (int)o.Meta.Rarity))
                        && (o.Meta.IsUnique ?? false) == false
                        && o.Meta.Level <= Core.State.Character.Level)
                    .Cast<TileIdentifier>();

                foreach (var item in itemsInStore)
                {
                    var newItem = item.DeriveCopy();

                    int max = (int)newItem.Meta.Rarity / 4;
                    if (max < 5) max = 5;
                    if (max > 20) max = 20;

                    if (newItem.Meta.CanStack == true)
                    {
                        newItem.Meta.Quantity = Library.Utility.MathUtility.RandomNumber(4, max);
                    }
                    else if (newItem.Meta.SubType == ActorSubType.Wand)
                    {
                        newItem.Meta.Charges = Library.Utility.MathUtility.RandomNumber(4, max);
                    }

                    persistentStore.Items.Add(newItem);
                }

                Core.State.Stores.Add(persistentStore);
            }

            foreach (var item in persistentStore.Items)
            {
                AddItemToListView(listView, item.TilePath, item.Meta);
            }

            listView.Sorting = SortOrder.Ascending;
            listView.Sort();

            return persistentStore;
        }

        void PopulateContainerFromPack(ListView listView, TileIdentifier containerTile)
        {
            listView.Items.Clear();

            labelPack.Text = $"Pack: ({containerTile.Meta.Name})";

            foreach (var item in Core.State.Items.Where(o => o.ContainerId == containerTile.Meta.UID))
            {
                AddItemToListView(listView, item.Tile.TilePath, item.Tile.Meta);
            }

            listView.Sorting = SortOrder.Ascending;
            listView.Sort();
        }

        private void AddItemToListView(ListView listView, string tilePath, TileMetadata meta)
        {
            string text = meta.Name;

            if (meta.CanStack == true && meta.Quantity > 0)
            {
                text += $" ({meta.Quantity})";
            }
            else if (meta.CanStack == false && meta.Charges > 0)
            {
                text += $" ({meta.Charges})";
            }

            var equipTag = new EquipTag()
            {
                Tile = new TileIdentifier(tilePath, meta)
            };

            equipTag.AcceptTypes.Add((ActorSubType)meta.SubType);

            ListViewItem item = new ListViewItem(text);
            item.ImageKey = GetImageKey(tilePath);
            item.Tag = equipTag;
            listView.Items.Add(item);
        }

        private void OpenPack(EquipTag item)
        {
            if (item.Tile.Meta.SubType == ActorSubType.Pack
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
