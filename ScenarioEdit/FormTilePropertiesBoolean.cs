using System;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormTilePropertiesBoolean : Form
    {
        public FormTilePropertiesBoolean()
        {
            InitializeComponent();
        }

        private void FormTilePropertiesBoolean_Load(object sender, EventArgs e)
        {
            this.AcceptButton = buttonSave;
            this.CancelButton = buttonCancel;
        }

        public FormTilePropertiesBoolean(string propertyName, bool propertyValue)
        {
            InitializeComponent();
            checkBoxValue.Text = propertyName;
            checkBoxValue.Checked = propertyValue;
        }

        public bool PropertyValue
        {
            get
            {
                return checkBoxValue.Checked;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
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
