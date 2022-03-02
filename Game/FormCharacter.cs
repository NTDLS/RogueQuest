using Library.Engine;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public partial class FormCharacter : Form
    {
        private EngineCoreBase _core;

        public FormCharacter()
        {
        }

        public FormCharacter(EngineCoreBase core)
        {
            InitializeComponent();

            _core = core;

            labelCharacterExpereience.Text = $"{core.State.Character.Experience:N0}";
            labelNextLevelAt.Text = $"{core.State.Character.NextLevelExperience:N0}";
            labelCharacterLevel.Text = $"{core.State.Character.Level:N0}";
            labelArmorValue.Text = $"{core.State.Character.AC:N0}";
            labelManaPoints.Text = $"{core.State.Character.Mana:N0}";
            labelHitPoints.Text = $"{core.State.Character.Hitpoints:N0}";
            labelGold.Text = $"{core.State.Character.Money:N0}";
            labelBulk.Text = $"{core.State.Character.Bulk:N0}";
            labelWeight.Text = $"{core.State.Character.Weight:N0}";

            int[] maxOf = new int[]
                {
                    core.State.Character.Intelligence, core.State.Character.Strength,
                    core.State.Character.Dexterity, core.State.Character.Constitution,
                    core.State.Character.StartingIntelligence, core.State.Character.StartingStrength,
                    core.State.Character.StartingDexterity, core.State.Character.StartingConstitution
                };

            int maxPropValue = maxOf.Max() + 5;

            progressCurrentIntelligence.Maximum = maxPropValue;
            progressCurrentStrength.Maximum = maxPropValue;
            progressCurrentDexterity.Maximum = maxPropValue;
            progressCurrentConstitution.Maximum = maxPropValue;
            progressOriginalIntelligence.Maximum = maxPropValue;
            progressOriginalStrength.Maximum = maxPropValue;
            progressOriginalDexterity.Maximum = maxPropValue;
            progressOriginalConstitution.Maximum = maxPropValue;

            progressCurrentIntelligence.Value = core.State.Character.Intelligence;
            progressCurrentStrength.Value = core.State.Character.Strength;
            progressCurrentDexterity.Value = core.State.Character.Dexterity;
            progressCurrentConstitution.Value = core.State.Character.Constitution;
            progressOriginalIntelligence.Value = core.State.Character.StartingIntelligence;
            progressOriginalStrength.Value = core.State.Character.StartingStrength;
            progressOriginalDexterity.Value = core.State.Character.StartingDexterity;
            progressOriginalConstitution.Value = core.State.Character.StartingConstitution;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
