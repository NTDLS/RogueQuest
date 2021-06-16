using Game.Engine;
using Game.Maps;
using Library.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RougueQuest
{
    public partial class FormMain : Form
    {
        private EngineCore _core;
        private bool _fullScreen = false;

        //This really shouldn't be necessary! :(
        protected override CreateParams CreateParams
        {
            get
            {
                //Paints all descendants of a window in bottom-to-top painting order using double-buffering.
                // For more information, see Remarks. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000; //WS_EX_COMPOSITED       
                return handleParam;
            }
        }


        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            if (_fullScreen)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.Width = Screen.PrimaryScreen.Bounds.Width;
                this.Height = Screen.PrimaryScreen.Bounds.Height;
                this.ShowInTaskbar = true;
                //this.TopMost = true;
                this.WindowState = FormWindowState.Maximized;
            }

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);

            this.BackColor = Color.FromArgb(60, 60, 60);

            _core = new EngineCore(this, new Size(this.Width, this.Height));

            _core.OnStop += _core_OnStop;
            _core.OnStart += _core_OnStart;

            MapPersistence.Load(_core, Assets.Constants.GetAssetPath(@"Maps\Meadow.rqm"));
        }

        private void _core_OnStart(EngineCoreBase sender)
        {
            sender.AddNewMap<MapHome>();
        }

        private void _core_OnStop(EngineCoreBase sender)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _core.Stop();

        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            _core.Start();
        }

        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_core.Render(), 0, 0);
        }
    }
}
