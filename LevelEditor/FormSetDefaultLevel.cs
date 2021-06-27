using LevelEditor.Engine;
using Library.Engine;
using System;
using System.Windows.Forms;

namespace LevelEditor
{
    public partial class FormSetDefaultLevel : Form
    {
        public EngineCore Core  { get; set; }
        public FormSetDefaultLevel()
        {
            InitializeComponent();
        }

        private void FormSetDefaultLevel_Load(object sender, EventArgs e)
        {
            this.AcceptButton = buttonOk;
            this.CancelButton = buttonCancel;

            listBoxLevels.DisplayMember = "Name";

            foreach (var level in Core.Levels.Collection)
            {
                listBoxLevels.Items.Add(level);
            }
        }

        public FormSetDefaultLevel(EngineCore core)
        {
            InitializeComponent();
            Core = core;
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
                if (MessageBox.Show($"Are you sure you want set '{levelName}' as the default level?",
                    "Set Default Level?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
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
                if (MessageBox.Show($"Are you sure you want set '{levelName}' as the default level?",
                    "Set Default Level?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
