using ScenarioEdit.Engine;
using Library.Engine;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormSetDefaultLevel : Form
    {
        private ContextMenuStrip _menu = new ContextMenuStrip();
        public EngineCore Core  { get; set; }

        public FormSetDefaultLevel()
        {
            InitializeComponent();
        }

        public FormSetDefaultLevel(EngineCore core)
        {
            InitializeComponent();
            Core = core;
        }

        private void FormSetDefaultLevel_Load(object sender, EventArgs e)
        {
            this.AcceptButton = buttonOk;
            this.CancelButton = buttonCancel;

            listBoxLevels.DisplayMember = "Name";

            listBoxLevels.DrawItem += ListBoxLevels_DrawItem;
            listBoxLevels.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxLevels.MouseUp += ListBoxLevels_MouseUp;

            _menu.Items.Clear();
            _menu.Items.Add("Rename").Click += FormSetDefaultLevel_Click;

            PopulateLevels();
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

        private void PopulateLevels()
        {
            listBoxLevels.Items.Clear();
            foreach (var level in Core.Levels.Collection)
            {
                listBoxLevels.Items.Add(level);
            }
        }

        private void ListBoxLevels_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index == Core.State.DefaultLevel)
            {
                e.Graphics.DrawString((listBoxLevels.Items[e.Index] as Level).Name, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, e.Bounds);
            }
            else
            {
                e.Graphics.DrawString((listBoxLevels.Items[e.Index] as Level).Name, new Font("Arial", 10, FontStyle.Regular), Brushes.Black, e.Bounds);
            }
            e.DrawFocusRectangle();
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
