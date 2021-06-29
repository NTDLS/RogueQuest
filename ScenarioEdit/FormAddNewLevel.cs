using ScenarioEdit.Engine;
using System;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormAddNewLevel : Form
    {
        public EngineCore Core { get; set; }

        public FormAddNewLevel()
        {
            InitializeComponent();
        }

        public FormAddNewLevel(EngineCore core)
        {
            InitializeComponent();
            Core = core;

            this.AcceptButton = buttonOk;
            this.CancelButton = buttonCancel;
        }

        public string LevelName
        {
            get
            {
                return textBoxName.Text.Trim();
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (Core.Levels.GetIndex(LevelName) >= 0)
            {
                MessageBox.Show("A level with name already exists.");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
