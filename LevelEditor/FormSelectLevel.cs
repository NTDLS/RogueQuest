using LevelEditor.Engine;
using Library.Engine;
using System;
using System.Windows.Forms;

namespace LevelEditor
{
    public partial class FormSelectLevel : Form
    {
        public EngineCore Core  { get; set; }
        public FormSelectLevel()
        {
            InitializeComponent();
        }

        private void FormSelectLevel_Load(object sender, EventArgs e)
        {
            this.AcceptButton = buttonOk;
            this.CancelButton = buttonCancel;

            listBoxLevels.DisplayMember = "Name";

            foreach (var level in Core.Levels.Collection)
            {
                listBoxLevels.Items.Add(level);
            }
        }

        public FormSelectLevel(EngineCore core)
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
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
