using ScenarioEdit.Engine;
using System;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormScenarioProperties : Form
    {
        public EngineCore Core { get; set; }

        public FormScenarioProperties()
        {
            InitializeComponent();
        }

        private void FormScenarioProperties_Load(object sender, EventArgs e)
        {
            this.AcceptButton = buttonSave;
            this.CancelButton = buttonCancel;
        }

        public FormScenarioProperties(EngineCore core)
        {
            InitializeComponent();
            Core = core;

            textBoxCreatedBy.Text = core.ScenarioMeta.CreatedBy;
            textBoxCreatedDate.Text = core.ScenarioMeta.CreatedDate.ToString();
            textBoxModifiedDate.Text = core.ScenarioMeta.ModifiedDate.ToString();
            textBoxDescription.Text = core.ScenarioMeta.Description;
            textBoxName.Text = core.ScenarioMeta.Name;
            textBoxLevels.Text = $"{core.Levels.Collection.Count}:N0";
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Core.ScenarioMeta.CreatedBy = textBoxCreatedBy.Text.Trim();
            Core.ScenarioMeta.Description = textBoxDescription.Text.Trim();
            Core.ScenarioMeta.Name = textBoxName.Text.Trim();

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
