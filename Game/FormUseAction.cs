using Library.Engine;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Game
{
    public partial class FormUseAction : Form
    {
        public enum UseAction
        {
            None,
            Use,
            Split
        }

        private TileIdentifier _tile;
        public UseAction UseActionResult { get; set; }
        public List<UseAction> AvailableActions { get; set; } = new List<UseAction>();

        public FormUseAction()
        {
            InitializeComponent();
        }

        public FormUseAction(TileIdentifier tile)
        {
            InitializeComponent();

            _tile = tile;

            labelItem.Text = $"{tile.Meta.Name}";

            if (tile.Meta.Quantity > 0)
            {
                labelItem.Text += $" ({tile.Meta.Quantity})";
            }

            buttonUse.Enabled = (tile.Meta.IsConsumable ?? false) && tile.Meta.TargetType == Library.Engine.Types.TargetType.Self;
            buttonSplit.Enabled = (tile.Meta.CanStack ?? false) && tile.Meta.Quantity > 1;

            if (buttonUse.Enabled)
            {
                AvailableActions.Add(UseAction.Use);
            }
            if (buttonSplit.Enabled)
            {
                AvailableActions.Add(UseAction.Split);
            }
        }

        private void FormUseAction_Load(object sender, EventArgs e)
        {
            CancelButton = buttonCancel;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            UseActionResult = UseAction.None;
            DialogResult = DialogResult.Cancel;
        }

        private void buttonUse_Click(object sender, EventArgs e)
        {
            UseActionResult = UseAction.Use;
            DialogResult = DialogResult.OK;
        }

        private void buttonSplit_Click(object sender, EventArgs e)
        {
            UseActionResult = UseAction.Split;
            DialogResult = DialogResult.OK;
        }
    }
}
