using Library.Engine;
using Library.Engine.Types;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public partial class FormPickCursedItem : Form
    {
        private EngineCoreBase _core;

        public TileIdentifier SelectedCursedItem
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

        public FormPickCursedItem()
        {
            this.CancelButton = buttonCancel;
            this.AcceptButton = buttonOk;
        }

        public FormPickCursedItem(EngineCoreBase core)
        {
            InitializeComponent();
            _core = core;

            var cursedEquipment = _core.State.Character.Equipment.Where(o => o.Tile != null && o.Tile.Meta.Enchantment == EnchantmentType.Cursed).ToList();

            foreach (var cursedEquip in cursedEquipment)
            {
                var lvItem = new ListViewItem(cursedEquip.Tile.Meta.Name)
                {
                    Tag = cursedEquip.Tile
                };
                listViewEquip.Items.Add(lvItem);
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (SelectedCursedItem == null)
            {
                Constants.Alert("Select a cursed item, or cancel.");
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
    }
}
