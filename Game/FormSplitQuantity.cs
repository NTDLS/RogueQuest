using Library.Engine;
using System;
using System.Windows.Forms;

namespace Game
{
    public partial class FormSplitQuantity : Form
    {
        private TileIdentifier _tile;
        public FormSplitQuantity()
        {
            InitializeComponent();
        }

        public int SplitQuantity
        {
            get
            {
                if (int.TryParse(textBoxSplitAmount.Text, out int value))
                {
                    return value;
                }
                return 0;
            }
        }

        public FormSplitQuantity(TileIdentifier tile)
        {
            InitializeComponent();

            _tile = tile;

            labelItem.Text = $"{tile.Meta.Name}";

            if (tile.Meta.Charges > 0)
            {
                labelTotal.Text = $"{tile.Meta.Charges:N0}";
            }
            else if (tile.Meta.Quantity > 0)
            {
                labelTotal.Text = $"{tile.Meta.Quantity:N0}";
            }

            textBoxSplitAmount.Focus();
        }

        private void FormSplitQuantity_Load(object sender, EventArgs e)
        {
            this.CancelButton = buttonCancel;
            this.AcceptButton = buttonOk;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            int splitQty = SplitQuantity;
            int totalQty = 0;

            if (_tile.Meta.Charges > 0)
            {
                totalQty = (int)_tile.Meta.Charges;
            }
            else if (_tile.Meta.Quantity > 0)
            {
                totalQty = (int)_tile.Meta.Quantity;
            }

            if (splitQty > 0 && splitQty < totalQty)
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
