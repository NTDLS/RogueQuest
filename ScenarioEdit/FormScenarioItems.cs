using Library.Engine;
using ScenarioEdit.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormScenarioItems : Form
    {
        private List<List<LevelChunk>> _allLevelChunks;
        private Button buttonCancel = new Button();

        public EngineCore Core { get; set; }
        public FormScenarioItems()
        {
            InitializeComponent();
        }

        private void FormWorldItems_Load(object sender, EventArgs e)
        {
            buttonCancel.Click += ButtonCancel_Click;
            this.CancelButton = buttonCancel;

            _allLevelChunks = Core.Levels.GetAllLevelsChunks();

            foreach (var item in Core.State.Items)
            {
                var lvi = new ListViewItem(item.Tile.Meta.Name);

                lvi.Tag = item;

                bool found = false;

                for (int level = 0; level < _allLevelChunks.Count; level++)
                {
                    var parent = _allLevelChunks[level].Where(o => o.Meta.UID == item.ContainerId).FirstOrDefault();

                    if (parent != null)
                    {
                        lvi.SubItems.Add(parent.Meta.Name);
                        lvi.SubItems.Add(Core.Levels.Collection[level].Name);
                        lvi.SubItems.Add($"{level:N0}");
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    lvi.SubItems.Add("<not found>");
                }

                listViewItems.Items.Add(lvi);
            }

            listViewItems.MouseDoubleClick += ListViewItems_MouseDoubleClick;
        }


        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ListViewItems_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewItems.SelectedItems?.Count != 1)
            {
                return;
            }

            var selectedItem = listViewItems.SelectedItems[0];

            if (selectedItem.SubItems.Count != 3)
            {
                return;
            }

            var selectedTag = selectedItem.Tag as CustodyItem;

            int levelIndex = Int32.Parse(selectedItem.SubItems[3].Text);
            if (levelIndex != Core.State.CurrentLevel)
            {
                Core.SelectLevel(levelIndex);
            }

            var hilightedTiles = Core.Actors.Tiles.Where(o => o.SelectedHighlight == true).ToList();
            foreach (var tile in hilightedTiles)
            {
                tile.SelectedHighlight = false;
            }


            var levelTile = Core.Actors.Tiles.Where(o => o.Meta.UID == selectedTag.ContainerId).FirstOrDefault();
            if (levelTile != null)
            {
                levelTile.SelectedHighlight = true;

                Core.Display.BackgroundOffset.X = levelTile.X / 2;
                Core.Display.BackgroundOffset.Y = levelTile.Y / 2;
                Core.Display.DrawingSurface.Invalidate();
            }
        }

        public FormScenarioItems(EngineCore core)
        {
            InitializeComponent();
            Core = core;
        }
    }
}
