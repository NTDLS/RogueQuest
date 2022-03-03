using Library.Engine;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public partial class FormPickNewSpell : Form
    {
        private EngineCoreBase _core;

        public FormPickNewSpell()
        {
        }

        public FormPickNewSpell(EngineCoreBase core)
        {
            InitializeComponent();
            _core = core;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
