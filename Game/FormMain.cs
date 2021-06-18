using Game.Actors;
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

            _core = new EngineCore(this.pictureBox, new Size(this.pictureBox.Width, this.pictureBox.Height));

            _core.OnStop += _core_OnStop;
            _core.OnStart += _core_OnStart;

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

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey) _core.Input.KeyStateChanged(Library.Types.PlayerKey.SpeedBoost, Library.Types.KeyPressState.Up);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Forward, Library.Types.KeyPressState.Up);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(Library.Types.PlayerKey.RotateCounterClockwise, Library.Types.KeyPressState.Up);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Reverse, Library.Types.KeyPressState.Up);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(Library.Types.PlayerKey.RotateClockwise, Library.Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(Library.Types.PlayerKey.PrimaryFire, Library.Types.KeyPressState.Up);
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(Library.Types.PlayerKey.SecondaryFire, Library.Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Escape, Library.Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Left, Library.Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Right, Library.Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Up, Library.Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Down, Library.Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Enter, Library.Types.KeyPressState.Up);
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey) _core.Input.KeyStateChanged(Library.Types.PlayerKey.SpeedBoost, Library.Types.KeyPressState.Down);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Forward, Library.Types.KeyPressState.Down);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(Library.Types.PlayerKey.RotateCounterClockwise, Library.Types.KeyPressState.Down);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Reverse, Library.Types.KeyPressState.Down);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(Library.Types.PlayerKey.RotateClockwise, Library.Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(Library.Types.PlayerKey.PrimaryFire, Library.Types.KeyPressState.Down);
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(Library.Types.PlayerKey.SecondaryFire, Library.Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Escape, Library.Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Left, Library.Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Right, Library.Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Up, Library.Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Down, Library.Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(Library.Types.PlayerKey.Enter, Library.Types.KeyPressState.Down);

            _core.Input.HandleSingleKeyPress(e.KeyCode);
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_core.Render(), 0, 0);
        }
    }
}
