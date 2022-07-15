using Game.Actors;
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
    public partial class FormInventory : Form
    {
        private Button _buttonClose = new Button();
        private TileIdentifier _currentlySelectedPack = null;
        private ToolTip _interrogationTip = new ToolTip();

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

            listViewSelectedContainer.SmallImageList = StoreAndInventory.ImageList;
            listViewSelectedContainer.LargeImageList = StoreAndInventory.ImageList;
            listViewSelectedContainer.View = View.List;
            listViewSelectedContainer.ItemDrag += ListViewSelectedContainer_ItemDrag;
            listViewSelectedContainer.DragEnter += ListViewSelectedContainer_DragEnter;
            listViewSelectedContainer.DragDrop += ListViewSelectedContainer_DragDrop;
            listViewSelectedContainer.AllowDrop = true;
            listViewSelectedContainer.MouseDoubleClick += ListViewSelectedContainer_MouseDoubleClick;
            listViewSelectedContainer.MouseUp += ListView_Shared_MouseUp;
            listViewSelectedContainer.MouseDown += ListView_Shared_MouseDown;

            listViewGround.SmallImageList = StoreAndInventory.ImageList;
            listViewGround.LargeImageList = StoreAndInventory.ImageList;
            listViewGround.View = View.List;
            listViewGround.ItemDrag += ListViewGround_ItemDrag;
            listViewGround.DragEnter += ListViewGround_DragEnter;
            listViewGround.DragDrop += ListViewGround_DragDrop;
            listViewGround.AllowDrop = true;
            listViewGround.MouseDoubleClick += ListViewGround_MouseDoubleClick;
            listViewGround.MouseUp += ListView_Shared_MouseUp;
            listViewGround.MouseDown += ListView_Shared_MouseDown;

            listViewPlayerPack.SmallImageList = StoreAndInventory.ImageList;
            listViewPlayerPack.LargeImageList = StoreAndInventory.ImageList;
            listViewPlayerPack.View = View.List;
            listViewPlayerPack.ItemDrag += ListViewPlayerPack_ItemDrag;
            listViewPlayerPack.DragEnter += ListViewPlayerPack_DragEnter;
            listViewPlayerPack.DragDrop += ListViewPlayerPack_DragDrop;
            listViewPlayerPack.AllowDrop = true;
            listViewPlayerPack.MouseDoubleClick += ListViewPlayerPack_MouseDoubleClick;
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

            RefreshInventory();
        }

        private void RefreshInventory()
        {
            RefreshDialogEquipmentSlots();

            //If we are wearing a pack, go ahead and show its contents.
            var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack);
            if (pack.Tile != null)
            {
                PopulateContainerFromPack(listViewPlayerPack, pack.Tile);
            }

            PopulateContainerFromGround(listViewGround);

            if (_currentlySelectedPack != null)
            {
                PopulateContainerFromPack(listViewSelectedContainer, _currentlySelectedPack);
            }
        }

        private bool UseItem(TileIdentifier item, bool promptForUse)
        {
            if ((item.Meta.IsIdentified ?? false) == false)
            {
                Constants.Alert("You don't know how to use this item because you cant identify it. Identify it using a spell or find a shop.", "Use Item");
                return false;
            }

            if (promptForUse)
            {
                string text = $"Use {item.Meta.DisplayName}?";

                if (item.Meta.Charges > 0)
                {
                    text += $"\r\n{item.Meta.Charges} charges remaining.";
                }

                if (item.Meta.Quantity > 0)
                {
                    text += $"\r\n{item.Meta.Quantity} remaining.";
                }

                if (MessageBox.Show(text, $"RougeQuest :: Use Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return false;
                }
            }

            var inventoryItem = Core.State.GetOrCreateInventoryItem(item);
            if (inventoryItem != null && inventoryItem.Tile.Meta.UID != null)
            {
                var result = Core.Tick.UseConsumableItem((Guid)inventoryItem.Tile.Meta.UID, null);

                if (result == true && inventoryItem.Tile.Meta?.EffectText == "Identify")
                {
                    RefreshInventory();
                }
                else RefreshDialogEquipmentSlots();

                return result;
            }

            return false;
        }

        #region ListViewGround

        private void ListViewGround_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

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
                    //We cant use items on the ground because they dont exist in Core.State.Items
                    //UseItem(item.Tile);
                }
            }
        }

        private void ListViewGround_DragDrop(object sender, DragEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

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
            var inventoryItem = Core.State.GetOrCreateInventoryItem(draggedItemTag.Tile);
            if (inventoryItem == null)
            {
                return;
            }

            bool wasStacked = false;

            if (inventoryItem.Tile.Meta.CanStack == true)
            {
                var itemUnderfoot = Core.Actors.Intersections(Core.Player)
                    .Where(o => o.Meta.ActorClass == ActorClassName.ActorItem
                    && StoreAndInventory.IsStackMatch(((ActorItem)o), draggedItemTag.Tile)
                    ).Cast<ActorItem>().FirstOrDefault();

                if (itemUnderfoot != null)
                {
                    itemUnderfoot.Meta.Quantity = (itemUnderfoot.Meta.Quantity ?? 0) + inventoryItem.Tile.Meta.Quantity;

                    var listViewItem = StoreAndInventory.FindListViewObjectByUid(listViewGround, (Guid)itemUnderfoot.Meta.UID);
                    if (listViewItem != null)
                    {
                        string text = itemUnderfoot.Meta.DisplayName;

                        if (itemUnderfoot.Meta.CanStack == true && itemUnderfoot.Meta.Quantity > 0)
                        {
                            text += $" ({itemUnderfoot.Meta.Quantity})";
                        }
                        else if (itemUnderfoot.Meta.CanStack == false && itemUnderfoot.Meta.Charges > 0)
                        {
                            text += $" ({itemUnderfoot.Meta.Charges})";
                        }

                        (listViewItem.Tag as EquipTag).Tile.Meta.Quantity = itemUnderfoot.Meta.Quantity;
                        listViewItem.Text = text;
                    }

                    wasStacked = true;
                }
            }

            if (wasStacked == false)
            {
                var droppedItem = Core.Actors.AddDynamic(inventoryItem.Tile, Core.Player.X, Core.Player.Y);
                StoreAndInventory.AddItemToListView(Core, listViewGround, draggedItemTag.Tile);
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

        private void ListViewSelectedContainer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

            if (listViewSelectedContainer.SelectedItems?.Count != 1)
            {
                return;
            }

            var selectedItem = listViewSelectedContainer.SelectedItems[0];
            var item = selectedItem.Tag as EquipTag;

            if (item.Tile.Meta.SubType == ActorSubType.Book)
            {
                string message = $"Read {item.Tile.Meta.DisplayName} to learn new spell?";

                if (MessageBox.Show(message, $"RougeQuest :: Use Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Core.State.Character.AddKnownSpell(item.Tile);
                    Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == item.Tile.Meta.UID);

                    var slotToVacate = Core.State.Character.FindEquipSlotByItemId(item.Tile.Meta.UID);
                    if (slotToVacate != null)
                    {
                        slotToVacate.Tile = null;
                    }

                    listViewSelectedContainer.Items.Remove(selectedItem);
                    Core.LogLine($"You learned a new spell, {item.Tile.Meta.SpellName}!");
                }
            }
            else if (item.Tile.Meta.SubType == ActorSubType.Pack
                || item.Tile.Meta.SubType == ActorSubType.Belt
                || item.Tile.Meta.SubType == ActorSubType.Chest
                || item.Tile.Meta.SubType == ActorSubType.Purse)
            {
                OpenPack(item);
            }
            else if (item.Tile.Meta.IsConsumable == true || item.Tile.Meta.CanStack == true)
            {
                using (var useAction = new FormUseAction(item.Tile))
                {
                    bool promptBeforeUse = true;

                    if (useAction.AvailableActions.Count > 1)
                    {
                        useAction.ShowDialog();
                        promptBeforeUse = false;
                    }

                    if (useAction.UseActionResult == FormUseAction.UseAction.Use
                        || (useAction.AvailableActions.Count == 1 && useAction.AvailableActions.First() == FormUseAction.UseAction.Use))
                    {
                        if (UseItem(item.Tile, promptBeforeUse))
                        {
                            if ((item.Tile.Meta.Quantity ?? 0) == 0)
                            {
                                listViewSelectedContainer.Items.Remove(selectedItem);
                            }
                            else
                            {
                                string text = item.Tile.Meta.DisplayName;
                                if (item.Tile.Meta.CanStack == true && item.Tile.Meta.Quantity > 0)
                                {
                                    text += $" ({item.Tile.Meta.Quantity})";
                                }
                                else if (item.Tile.Meta.CanStack == false && item.Tile.Meta.Charges > 0)
                                {
                                    text += $" ({item.Tile.Meta.Charges})";
                                }
                                selectedItem.Text = text;
                            }
                        }
                    }
                    else if (useAction.UseActionResult == FormUseAction.UseAction.Split
                        || (useAction.AvailableActions.Count == 1 && useAction.AvailableActions.First() == FormUseAction.UseAction.Split))
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
                                newInventoryItem.ContainerId = _currentlySelectedPack.Meta.UID;
                            }
                        }
                    }

                    PopulateContainerFromPack(listViewSelectedContainer, _currentlySelectedPack);
                }
            }
        }

        private void ListViewSelectedContainer_DragDrop(object sender, DragEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

            TileIdentifier pack = _currentlySelectedPack;

            if (pack == null)
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
            if (draggedItemTag.Tile == null)
            {
                return;
            }

            if (pack.Meta.SubType == ActorSubType.Purse && draggedItemTag.Tile.Meta.SubType != ActorSubType.Money)
            {
                Constants.Alert("You can only store coins in the purse");
                return;
            }

            //Find the item in the players inventory and change its container id to that of the selected open pack.
            var inventoryItem = Core.State.GetOrCreateInventoryItem(draggedItemTag.Tile);
            double maxBulk = pack.Meta.BulkCapacity ?? 0;
            double maxWeight = pack.Meta.WeightCapacity ?? 0;
            int? maxItems = pack.Meta.ItemCapacity;
            int quantityToMove = (draggedItemTag.Tile.Meta.Quantity ?? 1);

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

            if (pack.Meta.UID == draggedItemTag.Tile.Meta.UID)
            {
                //A container canot contain itsself.
                Constants.Alert($"A {pack.Meta.DisplayName} cannot contain itself.");
                return;
            }
            if (inventoryItem.ContainerId == (Guid)pack.Meta.UID)
            {
                //No need to do anything if we are dragging to the same container.
                return;
            }

            bool wasStacked = false;

            if (inventoryItem.Tile.Meta.CanStack == true)
            {
                //If we are dragging to a container and the container already contains some of the stackable stuff, then stack!
                var existingInventoryItem = Core.State.Items
                    .Where(o => StoreAndInventory.IsStackMatch(o.Tile, draggedItemTag.Tile)
                    && o.ContainerId == pack.Meta.UID).FirstOrDefault();

                if (existingInventoryItem != null)
                {
                    existingInventoryItem.Tile.Meta.Quantity = (existingInventoryItem.Tile.Meta.Quantity ?? 0) + inventoryItem.Tile.Meta.Quantity;
                    Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == draggedItemTag.Tile.Meta.UID);

                    var listViewItem = StoreAndInventory.FindListViewObjectByUid(listViewSelectedContainer, (Guid)existingInventoryItem.Tile.Meta.UID);
                    if (listViewItem != null)
                    {
                        string text = existingInventoryItem.Tile.Meta.DisplayName;

                        if (existingInventoryItem.Tile.Meta.CanStack == true && existingInventoryItem.Tile.Meta.Quantity > 0)
                        {
                            text += $" ({existingInventoryItem.Tile.Meta.Quantity})";
                        }
                        else if (existingInventoryItem.Tile.Meta.CanStack == false && existingInventoryItem.Tile.Meta.Charges > 0)
                        {
                            text += $" ({existingInventoryItem.Tile.Meta.Charges})";
                        }

                        (listViewItem.Tag as EquipTag).Tile.Meta.Quantity = existingInventoryItem.Tile.Meta.Quantity;
                        listViewItem.Text = text;
                    }

                    wasStacked = true;
                }
            }

            if (wasStacked == false)
            {
                StoreAndInventory.AddItemToListView(Core, listViewSelectedContainer, draggedItemTag.Tile);
                inventoryItem.ContainerId = (Guid)pack.Meta.UID;
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

        private void ListViewPlayerPack_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

            if (listViewPlayerPack.SelectedItems?.Count != 1)
            {
                return;
            }

            var selectedItem = listViewPlayerPack.SelectedItems[0];
            var item = selectedItem.Tag as EquipTag;

            if (item.Tile.Meta.SubType == ActorSubType.Book)
            {
                string message = $"Read {item.Tile.Meta.DisplayName} to learn new spell?";

                if (MessageBox.Show(message, $"RougeQuest :: Use Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Core.State.Character.AddKnownSpell(item.Tile);
                    Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == item.Tile.Meta.UID);

                    var slotToVacate = Core.State.Character.FindEquipSlotByItemId(item.Tile.Meta.UID);
                    if (slotToVacate != null)
                    {
                        slotToVacate.Tile = null;
                    }

                    listViewPlayerPack.Items.Remove(selectedItem);
                    Core.LogLine($"You learned a new spell, {item.Tile.Meta.SpellName}!");
                }
            }
            else if (item.Tile.Meta.SubType == ActorSubType.Pack
                || item.Tile.Meta.SubType == ActorSubType.Belt
                || item.Tile.Meta.SubType == ActorSubType.Chest
                || item.Tile.Meta.SubType == ActorSubType.Purse)
            {
                OpenPack(item);
            }
            else if (item.Tile.Meta.IsConsumable == true || item.Tile.Meta.CanStack == true)
            {
                var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack)?.Tile;
                if (pack == null)
                {
                    return;
                }

                using (var useAction = new FormUseAction(item.Tile))
                {
                    bool promptBeforeUse = true;

                    if (useAction.AvailableActions.Count > 1)
                    {
                        useAction.ShowDialog();
                        promptBeforeUse = false;
                    }

                    if (useAction.UseActionResult == FormUseAction.UseAction.Use
                        || (useAction.AvailableActions.Count == 1 && useAction.AvailableActions.First() == FormUseAction.UseAction.Use))
                    {
                        if (UseItem(item.Tile, promptBeforeUse))
                        {
                            if ((item.Tile.Meta.Quantity ?? 0) == 0)
                            {
                                listViewPlayerPack.Items.Remove(selectedItem);
                            }
                            else
                            {
                                string text = item.Tile.Meta.DisplayName;
                                if (item.Tile.Meta.CanStack == true && item.Tile.Meta.Quantity > 0)
                                {
                                    text += $" ({item.Tile.Meta.Quantity})";
                                }
                                else if (item.Tile.Meta.CanStack == false && item.Tile.Meta.Charges > 0)
                                {
                                    text += $" ({item.Tile.Meta.Charges})";
                                }
                                selectedItem.Text = text;
                            }
                        }
                    }
                    else if (useAction.UseActionResult == FormUseAction.UseAction.Split
                        || (useAction.AvailableActions.Count == 1 && useAction.AvailableActions.First() == FormUseAction.UseAction.Split))
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
                    }
                }

                PopulateContainerFromPack(listViewPlayerPack, pack);
            }
        }

        private void ListViewPlayerPack_DragDrop(object sender, DragEventArgs e)
        {
            if (Core.Tick.IsEngineBusy) return;

            var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack)?.Tile;
            if (pack == null)
            {
                Constants.Alert("You need to equip a pack.");
                return;
            }

            var destination = sender as ListView;
            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            var draggedItemTag = draggedItem.Tag as EquipTag;

            if (draggedItemTag?.Tile == null)
            {
                return;
            }

            if (destination == draggedItem.ListView)
            {
                return;
            }

            //Find the item in the players inventory and change its container id to that of the selected open pack.
            var inventoryItem = Core.State.GetOrCreateInventoryItem(draggedItemTag.Tile);
            double maxBulk = pack.Meta.BulkCapacity ?? 0;
            double maxWeight = pack.Meta.WeightCapacity ?? 0;
            int? maxItems = pack.Meta.ItemCapacity;
            int quantityToMove = (draggedItemTag.Tile.Meta.Quantity ?? 1);

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


            if (pack.Meta.UID == draggedItemTag.Tile.Meta.UID)
            {
                //A container canot contain itsself.
                Constants.Alert($"A {pack.Meta.DisplayName} cannot contain itself.");
                return;
            }
            if (inventoryItem.ContainerId == (Guid)pack.Meta.UID)
            {
                //No need to do anything if we are dragging to the same container.
                return;
            }

            bool wasStacked = false;

            if (inventoryItem.Tile.Meta.CanStack == true)
            {
                //If we are dragging to a container and the container already contains some of the stackable stuff, then stack!
                var existingInventoryItem = Core.State.Items
                    .Where(o => StoreAndInventory.IsStackMatch(o.Tile, draggedItemTag.Tile)
                    && o.ContainerId == pack.Meta.UID).FirstOrDefault();

                if (existingInventoryItem != null)
                {
                    existingInventoryItem.Tile.Meta.Quantity = (existingInventoryItem.Tile.Meta.Quantity ?? 0) + inventoryItem.Tile.Meta.Quantity;
                    Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == draggedItemTag.Tile.Meta.UID);

                    var listViewItem = StoreAndInventory.FindListViewObjectByUid(listViewPlayerPack, (Guid)existingInventoryItem.Tile.Meta.UID);
                    if (listViewItem != null)
                    {
                        string text = existingInventoryItem.Tile.Meta.DisplayName;

                        if (existingInventoryItem.Tile.Meta.CanStack == true && existingInventoryItem.Tile.Meta.Quantity > 0)
                        {
                            text += $" ({existingInventoryItem.Tile.Meta.Quantity})";
                        }
                        else if (existingInventoryItem.Tile.Meta.CanStack == false && existingInventoryItem.Tile.Meta.Charges > 0)
                        {
                            text += $" ({existingInventoryItem.Tile.Meta.Charges})";
                        }

                        (listViewItem.Tag as EquipTag).Tile.Meta.Quantity = existingInventoryItem.Tile.Meta.Quantity;
                        listViewItem.Text = text;
                    }

                    wasStacked = true;
                }
            }

            if (wasStacked == false)
            {
                StoreAndInventory.AddItemToListView(Core, listViewPlayerPack, draggedItemTag.Tile);
                inventoryItem.ContainerId = (Guid)pack.Meta.UID;
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

            lv.Items.Add(item);
        }

        private void RefreshDialogEquipmentSlots()
        {
            RefreshDialogEquipmentSlot(listViewArmor);
            RefreshDialogEquipmentSlot(listViewBracers);
            RefreshDialogEquipmentSlot(listViewWeapon);
            RefreshDialogEquipmentSlot(listViewPack);
            RefreshDialogEquipmentSlot(listViewBelt);
            RefreshDialogEquipmentSlot(listViewRightRing);
            RefreshDialogEquipmentSlot(listViewNecklace);
            RefreshDialogEquipmentSlot(listViewHelment);
            RefreshDialogEquipmentSlot(listViewGarment);
            RefreshDialogEquipmentSlot(listViewPurse);
            RefreshDialogEquipmentSlot(listViewBoots);
            RefreshDialogEquipmentSlot(listViewLeftRing);
            RefreshDialogEquipmentSlot(listViewFreeHand);
            RefreshDialogEquipmentSlot(listViewGauntlets);
            RefreshDialogEquipmentSlot(listViewShield);
            RefreshDialogEquipmentSlot(listViewQuiver1);
            RefreshDialogEquipmentSlot(listViewQuiver2);
            RefreshDialogEquipmentSlot(listViewQuiver3);
            RefreshDialogEquipmentSlot(listViewQuiver4);
            RefreshDialogEquipmentSlot(listViewQuiver5);
        }

        private void RefreshDialogEquipmentSlot(ListView lv)
        {
            var listViewItem = lv.Items[0] as ListViewItem;
            var equipTag = listViewItem.Tag as EquipTag;

            var equipSlot = Core.State.Character.GetEquipSlot(equipTag.Slot);
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

                listViewItem.Text = text;
                listViewItem.ImageKey = StoreAndInventory.GetImageKey(equipSlot.Tile.ImagePath);
                (listViewItem.Tag as EquipTag).Tile = equipSlot.Tile;
            }
            else
            {
                //We dont remove the items from equip slots, we just clear their text and image.
                listViewItem.ImageKey = null;
                listViewItem.Text = "";
                (listViewItem.Tag as EquipTag).Tile = null;
            }

        }

        private void ListView_EquipSlot_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var lv = sender as ListView;
                var item = lv.Items[0].Tag as EquipTag;
                var location = new Point(e.X + 10, e.Y - 25);
                _interrogationTip.Show(StoreAndInventory.GetItemTip(Core, item.Tile), lv, location, 5000);
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

                if (item.Tile == null)
                {
                    return;
                }

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
                        if (UseItem(item.Tile, true))
                        {
                            if (item.Tile != null)
                            {
                                string text = item.Tile.Meta.DisplayName;
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
            if (Core.Tick.IsEngineBusy) return;

            var destination = sender as ListView;
            var destinationTag = destination.Items[0].Tag as EquipTag;

            var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            var draggedItemTag = draggedItem.Tag as EquipTag;

            if (destination == draggedItem.ListView || destinationTag.Tile != null)
            {
                return;
            }

            if ((draggedItemTag?.Tile?.Meta?.SubType != null && destinationTag.AcceptTypes.Contains((ActorSubType)draggedItemTag.Tile.Meta.SubType))
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

            //If we are draging from the primary pack slot, then close the pack.
            if (draggedItem.ListView == listViewPack)
            {
                listViewPlayerPack.Items.Clear();
            }

            var destinationTag = destination.Items[0].Tag as EquipTag;

            var draggedItemTag = draggedItem.Tag as EquipTag;

            if ((draggedItemTag.Tile.Meta.SubType != null && destinationTag.AcceptTypes.Contains((ActorSubType)draggedItemTag.Tile.Meta.SubType))
                || destinationTag.AcceptTypes.Contains(ActorSubType.Unspecified))
            {
                if (draggedItemTag.Tile.Meta.SubType != ActorSubType.Wand
                    && draggedItemTag.Tile.Meta.SubType != ActorSubType.Potion
                    && draggedItemTag.Tile.Meta.SubType != ActorSubType.Scroll
                    && draggedItemTag.Tile.Meta.SubType != ActorSubType.Book)
                {
                    if ((draggedItemTag.Tile.Meta.IsIdentified ?? false) == false)
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
                var inventoryItem = Core.State.GetOrCreateInventoryItem(draggedItemTag.Tile);
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
            if (Core.Tick.IsEngineBusy) return;

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                var draggedItem = e.Item as ListViewItem;
                var draggedItemTag = draggedItem.Tag as EquipTag;

                if (draggedItemTag?.Tile?.Meta?.Enchantment == EnchantmentType.Cursed)
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
                _interrogationTip.Show(StoreAndInventory.GetItemTip(Core, item.Tile), lv, location, 5000);
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

        void PopulateContainerFromGround(ListView listView)
        {
            if (Core.Tick.IsEngineBusy) return;

            listView.Items.Clear();

            var itemUnderfoot = Core.Actors.Intersections(Core.Player)
                .Where(o => o.Meta.ActorClass == ActorClassName.ActorItem)
                .Cast<ActorItem>();

            foreach (var item in itemUnderfoot)
            {
                StoreAndInventory.AddItemToListView(Core, listView, item);
            }

            listView.Sorting = SortOrder.Ascending;
            listView.Sort();
        }

        void PopulateContainerFromPack(ListView listView, TileIdentifier containerTile)
        {
            if (Core.Tick.IsEngineBusy) return;

            listView.Items.Clear();

            if (listView == listViewPlayerPack)
            {
                labelPack.Text = $"Pack: ({containerTile.Meta.DisplayName})";
            }

            foreach (var item in Core.State.Items.Where(o => o.ContainerId == containerTile.Meta.UID))
            {
                StoreAndInventory.AddItemToListView(Core, listView, item.Tile);
            }

            listView.Sorting = SortOrder.Ascending;
            listView.Sort();
        }

        private void OpenPack(EquipTag item)
        {
            if (Core.Tick.IsEngineBusy) return;

            if (item.Tile.Meta.SubType == ActorSubType.Pack
                || item.Tile.Meta.SubType == ActorSubType.Belt
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
