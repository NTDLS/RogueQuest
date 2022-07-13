using Library.Engine;
using Library.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public partial class FormSelectUnidentifiedItem : Form
    {
        private EngineCoreBase _core;

        public TileIdentifier SelectedItem
        {
            get
            {
                if (listViewEquip.SelectedItems?.Count > 0)
                {
                    return listViewEquip.SelectedItems[0].Tag as TileIdentifier;
                }
                return null;
            }
        }

        public FormSelectUnidentifiedItem()
        {
            this.CancelButton = buttonCancel;
            this.AcceptButton = buttonOk;
        }

        public FormSelectUnidentifiedItem(EngineCoreBase core)
        {
            InitializeComponent();
            _core = core;

            List<TileIdentifier> equipment = new List<TileIdentifier>();

            var equipItem = _core.State.Character.GetEquipSlot(EquipSlot.FreeHand);
            if (equipItem != null && equipItem.Tile != null && (equipItem.Tile.Meta.IsIdentified ?? false) == false)
            {
                equipment.Add(equipItem.Tile);
            }

            equipItem = _core.State.Character.GetEquipSlot(EquipSlot.Weapon);
            if (equipItem != null && equipItem.Tile != null && (equipItem.Tile.Meta.IsIdentified ?? false) == false)
            {
                equipment.Add(equipItem.Tile);
            }

            equipItem = _core.State.Character.GetEquipSlot(EquipSlot.Belt);
            if (equipItem != null && equipItem.Tile != null)
            {
                var equipItems = _core.State.Items
                    .Where(o => o.Tile != null && (o.Tile.Meta.IsIdentified ?? false) == false && o.ContainerId == equipItem.Tile.Meta.UID).ToList();
                equipment.AddRange(equipItems.Select(o => o.Tile));
            }

            equipItem = _core.State.Character.GetEquipSlot(EquipSlot.Pack);
            if (equipItem != null && equipItem.Tile != null)
            {
                var equipItems = _core.State.Items
                    .Where(o => o.Tile != null && (o.Tile.Meta.IsIdentified ?? false) == false && o.ContainerId == equipItem.Tile.Meta.UID).ToList();
                equipment.AddRange(equipItems.Select(o => o.Tile));
            }

            foreach (var tile in equipment)
            {
                var lvItem = new ListViewItem(tile.Meta.DisplayName)
                {
                    Tag = tile
                };
                listViewEquip.Items.Add(lvItem);
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (SelectedItem == null)
            {
                Constants.Alert("Select an unidentifed item, or cancel.");
                return;
            }
            this.Close();
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.Cancel;
        }

        private void FormSelectUnidentifiedItem_Load(object sender, EventArgs e)
        {
        }
    }
}
