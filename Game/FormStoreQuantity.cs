using Game.Classes;
using Library.Engine;
using System;
using System.Windows.Forms;

namespace Game
{
    public partial class FormStoreQuantity : Form
    {
        private EngineCoreBase _core;
        private TileIdentifier _tile;
        public FormStoreQuantity()
        {
            InitializeComponent();
        }

        public int QuantityToBuy
        {
            get
            {
                if (int.TryParse(textBoxBuyAmount.Text, out int value))
                {
                    return value;
                }
                return 0;
            }
        }

        public FormStoreQuantity(EngineCoreBase core, TileIdentifier tile)
        {
            InitializeComponent();

            _core = core;
            _tile = tile;

            labelItem.Text = $"{tile.Meta.DisplayName}";
            textBoxBuyAmount.Minimum = 1;
            textBoxBuyAmount.Maximum = (int)tile.Meta.Quantity;
            textBoxBuyAmount.Text = tile.Meta.Quantity.ToString();
            labelStoreQuantity.Text = $"{tile.Meta.Quantity:N0}";

            textBoxBuyAmount.Focus();

            var timer = new Timer()
            {
                Interval = 500,
            };

            timer.Tick += Timer_Tick;

            timer.Start();

            Timer_Tick(this, null);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            labelTotalPrice.Text = $"{StoreAndInventory.AskingPrice(_core, _tile, QuantityToBuy):N0}";
        }

        private void FormStoreQuantity_Load(object sender, EventArgs e)
        {
            this.CancelButton = buttonCancel;
            this.AcceptButton = buttonBuy;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonBuy_Click(object sender, EventArgs e)
        {
            int splitQty = QuantityToBuy;
            int totalQty = 0;

            if (_tile.Meta.Charges > 0)
            {
                totalQty = (int)_tile.Meta.Charges;
            }
            else if (_tile.Meta.Quantity > 0)
            {
                totalQty = (int)_tile.Meta.Quantity;
            }

            if (splitQty > 0 && splitQty <= totalQty)
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
