using Library.Engine;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public partial class FormPickNewSpell : Form
    {
        private EngineCoreBase _core;
        private bool _Validated = false;

        public TileIdentifier SelectedSpell
        {
            get
            {
                if (listViewSpells.SelectedItems?.Count > 0)
                {
                    return listViewSpells.SelectedItems[0].Tag as TileIdentifier;
                }
                return null;
            }
        }

        public FormPickNewSpell()
        {
        }

        public FormPickNewSpell(EngineCoreBase core)
        {
            InitializeComponent();
            _core = core;

            PopulateSpells();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (SelectedSpell == null)
            {
                Constants.Alert("Select a new level-up spell.");
                return;
            }
            _Validated = true;
            this.Close();
            this.DialogResult = DialogResult.OK;
        }

        private void PopulateSpells()
        {
            var tiles = _core.Materials.Where(o =>
                o.Meta.SubType == Library.Engine.Types.ActorSubType.Book
                && o.Meta.Level <= _core.State.Character.Level).ToList();

            foreach (var tile in tiles)
            {
                if ((tile.Meta.Mana ?? 0) > 0 && tile.Meta.Level <= _core.State.Character.Level)
                {
                    var lvItem = new ListViewItem(new string[]
                    {
                        tile.Meta.DisplayName,
                        tile.Meta.Level.ToString(),
                        tile.Meta.Mana.ToString()
                    });

                    lvItem.Tag = tile;

                    listViewSpells.Items.Add(lvItem);
                }
            }
        }

        private void FormPickNewSpell_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_Validated == false)
            {
                Constants.Alert("Select a new level-up spell.");
                e.Cancel = true;
            }
        }
    }
}
