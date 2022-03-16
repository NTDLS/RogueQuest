using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace ScenarioEdit
{
    public partial class FormEditComboBox : Form
    {
        public FormEditComboBox()
        {
            InitializeComponent();
        }

        private void FormEditComboBox_Load(object sender, EventArgs e)
        {
            this.AcceptButton = buttonSave;
            this.CancelButton = buttonCancel;
        }

        public FormEditComboBox(string propertyName)
        {
            InitializeComponent();
            labelPropertyName.Text = propertyName;
        }

        public DialogResult ShowDialog<T>(List<ComboItem<T>> items, string selectedText = null)
        {
            comboBoxItems.Items.AddRange(items.ToArray());
            if (selectedText != null)
            {
                int index = comboBoxItems.FindString(selectedText);
                if (index >= 0)
                {
                    comboBoxItems.SelectedIndex = index;
                }
            }
            return this.ShowDialog();
        }

        public T GetSelection<T>()
        {
            if (comboBoxItems.SelectedItem != null)
            {
                return ((ComboItem<T>)comboBoxItems.SelectedItem).Value;
            }
            return default;
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
