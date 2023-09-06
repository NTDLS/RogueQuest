using Library.Engine;
using ScenarioEdit.Engine;
using System;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormDeleteLevel : Form
    {
        private ContextMenuStrip _menu = new ContextMenuStrip();

        public EngineCore Core { get; set; }

        public FormDeleteLevel()
        {
            InitializeComponent();
        }
        public FormDeleteLevel(EngineCore core)
        {
            InitializeComponent();
            Core = core;
        }

        private void FormDeleteLevel_Load(object sender, EventArgs e)
        {
            this.AcceptButton = buttonOk;
            this.CancelButton = buttonCancel;

            listBoxLevels.DisplayMember = "Name";
            listBoxLevels.MouseUp += ListBoxLevels_MouseUp;

            _menu.Items.Clear();
            _menu.Items.Add("Rename").Click += FormSetDefaultLevel_Click;

            PopulateLevels();
        }

        private void PopulateLevels()
        {
            listBoxLevels.Items.Clear();
            foreach (var level in Core.Levels.Collection)
            {
                listBoxLevels.Items.Add(level);
            }
        }

        private void ListBoxLevels_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var index = listBoxLevels.IndexFromPoint(e.Location);
                if (index >= 0)
                {
                    listBoxLevels.SelectedIndex = index;
                }

                if (listBoxLevels.SelectedItem == null)
                {
                    return;
                }

                _menu.Show(listBoxLevels, e.Location);
            }
        }

        private void FormSetDefaultLevel_Click(object sender, EventArgs e)
        {
            if (listBoxLevels.SelectedItem == null)
            {
                return;
            }

            string oldName = (listBoxLevels.SelectedItem as Level).Name;

            using (var dialog = new FormEditString("Level Name", oldName))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Core.Levels.RenameLevel(oldName, dialog.PropertyValue);
                    PopulateLevels();
                }
            }
        }

        public int SelectedLevelIndex
        {
            get
            {
                if (listBoxLevels.SelectedItem != null)
                {
                    string levelName = (listBoxLevels.SelectedItem as Level).Name;
                    return Core.Levels.GetIndex(levelName);
                }
                return -1;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (listBoxLevels.SelectedItem != null)
            {
                string levelName = (listBoxLevels.SelectedItem as Level).Name;
                if (MessageBox.Show($"Are you sure you want to delete the level '{levelName}'?",
                    "Delete Level?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void listBoxLevels_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBoxLevels.SelectedItem != null)
            {
                string levelName = (listBoxLevels.SelectedItem as Level).Name;
                if (MessageBox.Show($"Are you sure you want to delete the level '{levelName}'?",
                    "Delete Level?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
