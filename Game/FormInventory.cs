using Game.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    public partial class FormInventory : Form
    {
        public EngineCore Core { get; set; }
        public FormInventory()
        {
            InitializeComponent();
        }

        public FormInventory(EngineCore core)
        {
            Core = core;
        }
    }
}
