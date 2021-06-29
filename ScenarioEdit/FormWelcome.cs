﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormWelcome : Form
    {
        public string SelectedFileName { get; set; }

        public static string RecentSaveFilename
        {
            get
            {
                return Assets.Constants.GetAssetPath(@"Scenario\Recent.txt");
            }
        }

        public FormWelcome()
        {
            InitializeComponent();
        }

        public static void AddToRecentList(string fileName)
        {
            if (System.IO.File.Exists(RecentSaveFilename) == false)
            {
                System.IO.File.WriteAllText(RecentSaveFilename, string.Empty);
            }

            string json = System.IO.File.ReadAllText(RecentSaveFilename);

            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json);
            if (list == null)
            {
                list = new List<string>();
            }

            list.RemoveAll(o => o.ToLower() == fileName.ToLower());

            list.Insert(0, fileName);

            json = Newtonsoft.Json.JsonConvert.SerializeObject(list);

            System.IO.File.WriteAllText(RecentSaveFilename, json);
        }

        public static void RemoveFromList(string fileName)
        {
            if (System.IO.File.Exists(RecentSaveFilename) == false)
            {
                System.IO.File.WriteAllText(RecentSaveFilename, string.Empty);
            }

            string json = System.IO.File.ReadAllText(RecentSaveFilename);

            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json);
            if (list == null)
            {
                list = new List<string>();
            }

            list.RemoveAll(o => o.ToLower() == fileName.ToLower());

            json = Newtonsoft.Json.JsonConvert.SerializeObject(list);

            System.IO.File.WriteAllText(RecentSaveFilename, json);
        }

        public static void ClearList()
        {
            System.IO.File.WriteAllText(RecentSaveFilename, string.Empty);
        }

        private void PopulateList()
        {
            listBoxSaves.Items.Clear();
            if (System.IO.File.Exists(RecentSaveFilename) == false)
            {
                System.IO.File.WriteAllText(RecentSaveFilename, string.Empty);
            }

            string json = System.IO.File.ReadAllText(RecentSaveFilename);

            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json);
            if (list == null)
            {
                list = new List<string>();
            }

            listBoxSaves.Items.AddRange(list.ToArray());
        }

        private void FormWelcome_Load(object sender, EventArgs e)
        {
            this.CancelButton = buttonCancel;

            listBoxSaves.MouseDoubleClick += ListBoxSaves_MouseDoubleClick;

            PopulateList();
        }

        private void ListBoxSaves_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBoxSaves.SelectedItem != null)
            {
                SelectedFileName = listBoxSaves.SelectedItem.ToString();

                if (System.IO.File.Exists(SelectedFileName) == false)
                {
                    MessageBox.Show("This file no longer exists.", "Missing File");

                    RemoveFromList(SelectedFileName);
                    PopulateList();
                    SelectedFileName = string.Empty;
                    return;
                }

                SelectedFileName = listBoxSaves.SelectedItem.ToString();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonNewGame_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Rogue Quest Scenario (*.rqs)|*.rqs|All files (*.*)|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SelectedFileName = dialog.FileName;
                    this.DialogResult = DialogResult.OK;
                    AddToRecentList(SelectedFileName);
                    this.Close();
                }
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Clear all recent games??",
                "Clear list?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                listBoxSaves.Items.Clear();
                ClearList();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }
    }
}