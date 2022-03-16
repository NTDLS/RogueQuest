using System;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormEditString : Form
    {
        public FormEditString()
        {
            InitializeComponent();
        }

        private void FormEditString_Load(object sender, EventArgs e)
        {
            this.AcceptButton = buttonSave;
            this.CancelButton = buttonCancel;
        }

        public FormEditString(string propertyName, string propertyValue)
        {
            InitializeComponent();
            labelPropertyName.Text = propertyName;
            textBoxPropertyValue.Text = propertyValue;
        }

        public string PropertyValue
        {
            get
            {
                return textBoxPropertyValue.Text;
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
