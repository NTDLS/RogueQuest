using Library.Engine;
using System;
using System.IO;
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
            PopulateSpellsEx(@"\");
        }

        private void PopulateSpellsEx(string childFolder)
        {
            string spellsPath = Assets.Constants.GetAssetPath();
            string partialPath = @$"Tiles\Items\Equipment\Scrolls\{childFolder}".Replace(@"\\", @"\");

            foreach (var f in Directory.GetFiles(Path.Combine(spellsPath, partialPath), "*.txt"))
            {
                if (Path.GetFileName(f).StartsWith("@"))
                {
                    continue;
                }
                string relativePath = Path.GetRelativePath(spellsPath, f);
                string tilePath = relativePath.Substring(0, relativePath.Length - 4);
                TileIdentifier tile = new TileIdentifier(tilePath, true);

                if (tile.Meta.Level <= _core.State.Character.Level)
                {
                    ListViewItem lvItem = new ListViewItem(new string[]
                    {
                        tile.Meta.Name,
                        tile.Meta.Level.ToString(),
                        tile.Meta.Mana.ToString()
                    });

                    lvItem.Tag = tile;

                    listViewSpells.Items.Add(lvItem);
                }
            }

            foreach (var d in Directory.GetDirectories(Path.Combine(spellsPath, partialPath)))
            {
                if (Path.GetFileName(d).StartsWith("@") || Path.GetFileName(d).StartsWith("."))
                {
                    continue;
                }
                PopulateSpellsEx(Path.Combine(childFolder, d));
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
