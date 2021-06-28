using Game.Properties;
using Library.Utility;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public partial class FormNewCharacter : Form
    {
        public int SelectedAvatar { get; set; }
        public int Constitution => progressBarConstitution.Value;
        public int Dexterity => progressBarDexterity.Value;
        public int Intelligence => progressBarIntelligence.Value;
        public int Strength => progressBarStrength.Value;
        public string CharacterName => textBoxName.Text;

        public FormNewCharacter()
        {
            InitializeComponent();
        }

        private void FormNewCharacter_Load(object sender, EventArgs e)
        {
            progressBarConstitution.Minimum = 0;
            progressBarDexterity.Minimum = 0;
            progressBarIntelligence.Minimum = 0;
            progressBarStrength.Minimum = 0;

            progressBarConstitution.Maximum = Constants.MaxAvailableStatsPool;
            progressBarDexterity.Maximum = Constants.MaxAvailableStatsPool;
            progressBarIntelligence.Maximum = Constants.MaxAvailableStatsPool;
            progressBarStrength.Maximum = Constants.MaxAvailableStatsPool;

            progressBarConstitution.Value = Constants.StartingStatLevel;
            progressBarDexterity.Value = Constants.StartingStatLevel;
            progressBarIntelligence.Value = Constants.StartingStatLevel;
            progressBarStrength.Value = Constants.StartingStatLevel;

            progressBarAvailable.Minimum = 0;
            progressBarAvailable.Maximum = Constants.MaxAvailableStatsPool;
            progressBarAvailable.Value = RemainPool;

            var names = Resources.Names.Split("\r\n").ToList();
            textBoxName.Text = names[MathUtility.RandomNumber(0, names.Count - 1)];

            Image img;

            img = Bitmap.FromFile(Assets.Constants.GetAssetPath(@"Tiles\Player\1\Front 1.png"));
            pictureBoxPlayer1.Image = img;
            img = Bitmap.FromFile(Assets.Constants.GetAssetPath(@"Tiles\Player\2\Front 1.png"));
            pictureBoxPlayer2.Image = img;
            img = Bitmap.FromFile(Assets.Constants.GetAssetPath(@"Tiles\Player\3\Front 1.png"));
            pictureBoxPlayer3.Image = img;
            img = Bitmap.FromFile(Assets.Constants.GetAssetPath(@"Tiles\Player\4\Front 1.png"));
            pictureBoxPlayer4.Image = img;


            pictureBoxPlayer1.MouseClick += PictureBoxPlayer_MouseClick;
            pictureBoxPlayer2.MouseClick += PictureBoxPlayer_MouseClick;
            pictureBoxPlayer3.MouseClick += PictureBoxPlayer_MouseClick;
            pictureBoxPlayer4.MouseClick += PictureBoxPlayer_MouseClick;

            PictureBoxPlayer_MouseClick(pictureBoxPlayer3, null);

            this.AcceptButton = buttonOk;
            this.CancelButton = buttonCancel;
        }

        private void PictureBoxPlayer_MouseClick(object sender, MouseEventArgs e)
        {
            pictureBoxPlayer1.BackColor = Color.Transparent;
            pictureBoxPlayer2.BackColor = Color.Transparent;
            pictureBoxPlayer3.BackColor = Color.Transparent;
            pictureBoxPlayer4.BackColor = Color.Transparent;

            pictureBoxPlayer1.BorderStyle = BorderStyle.None;
            pictureBoxPlayer2.BorderStyle = BorderStyle.None;
            pictureBoxPlayer3.BorderStyle = BorderStyle.None;
            pictureBoxPlayer4.BorderStyle = BorderStyle.None;

            (sender as PictureBox).BackColor = Color.LightSkyBlue;
            (sender as PictureBox).BorderStyle = BorderStyle.Fixed3D;

            if (sender == pictureBoxPlayer1) SelectedAvatar = 1;
            if (sender == pictureBoxPlayer2) SelectedAvatar = 2;
            if (sender == pictureBoxPlayer3) SelectedAvatar = 3;
            if (sender == pictureBoxPlayer4) SelectedAvatar = 4;
        }

        int UsedPool
        {
            get
            {
                return progressBarConstitution.Value
                    + progressBarDexterity.Value
                    + progressBarIntelligence.Value
                    + progressBarStrength.Value;
            }
        }

        int RemainPool
        {
            get
            {
                return progressBarAvailable.Maximum - UsedPool;
            }
        }

        private void buttonStrengthDown_Click(object sender, EventArgs e)
        {
            if (progressBarStrength.Value > Constants.MinStartingStatLevel)
            {
                progressBarStrength.Value--;
            }
            progressBarAvailable.Value = RemainPool;
        }

        private void buttonStrengthUp_Click(object sender, EventArgs e)
        {
            if (progressBarStrength.Value < progressBarStrength.Maximum && RemainPool > 0)
            {
                progressBarStrength.Value++;
            }
            progressBarAvailable.Value = RemainPool;
        }

        private void buttonIntelligenceDown_Click(object sender, EventArgs e)
        {
            if (progressBarIntelligence.Value > Constants.MinStartingStatLevel)
            {
                progressBarIntelligence.Value--;
            }
            progressBarAvailable.Value = RemainPool;
        }

        private void buttonIntelligenceUp_Click(object sender, EventArgs e)
        {
            if (progressBarIntelligence.Value < progressBarIntelligence.Maximum && RemainPool > 0)
            {
                progressBarIntelligence.Value++;
            }
            progressBarAvailable.Value = RemainPool;
        }

        private void buttonConstitutionDown_Click(object sender, EventArgs e)
        {
            if (progressBarConstitution.Value > Constants.MinStartingStatLevel)
            {
                progressBarConstitution.Value--;
            }
            progressBarAvailable.Value = RemainPool;
        }

        private void buttonConstitutionUp_Click(object sender, EventArgs e)
        {
            if (progressBarConstitution.Value < progressBarConstitution.Maximum && RemainPool > 0)
            {
                progressBarConstitution.Value++;
            }
            progressBarAvailable.Value = RemainPool;
        }

        private void buttonDexterityDown_Click(object sender, EventArgs e)
        {
            if (progressBarDexterity.Value > Constants.MinStartingStatLevel)
            {
                progressBarDexterity.Value--;
            }
            progressBarAvailable.Value = RemainPool;
        }

        private void buttonDexterityUp_Click(object sender, EventArgs e)
        {
            if (progressBarDexterity.Value < progressBarDexterity.Maximum && RemainPool > 0)
            {
                progressBarDexterity.Value++;
            }
            progressBarAvailable.Value = RemainPool;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
