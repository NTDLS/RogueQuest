using Game.Actors;
using Game.Engine;
using Game.Extensions;
using Game.Maps;
using Library.Engine;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RougueQuest
{
    public partial class FormMain : Form
    {
        private EngineCore _core;
        private bool _fullScreen = false;

        /// <summary>
        /// Really just for debugging.
        /// </summary>
        public string _mapPathPassedToGame { get; set; }

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

        public FormMain(string mapPath)
        {
            InitializeComponent();
            _mapPathPassedToGame = mapPath;
        }

        public FormMain()
        {
            InitializeComponent();
        }

        private Control drawingsurface = new Control();

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

            _core = new EngineCore(this.drawingsurface, new Size(this.drawingsurface.Width, this.drawingsurface.Height));
            _core.OnStop += _core_OnStop;
            _core.OnStart += _core_OnStart;
            _core.AfterTick += _core_AfterTick;
            _core.Tick.OnLog += _core_OnLog;

            //Yea, this is stupid but the richtextbox steals the keyboard focus from the form. :(
            System.Reflection.PropertyInfo controlProperty = typeof(System.Windows.Forms.Control)
                    .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            controlProperty.SetValue(drawingsurface, true, null);

            splitContainerHoriz.Panel1.Controls.Add(drawingsurface);
            drawingsurface.Dock = DockStyle.Fill;

            drawingsurface.KeyUp += drawingsurface_KeyUp;
            richTextBoxLog.Click += RichTextBoxLog_Click;
            drawingsurface.KeyDown += drawingsurface_KeyDown;
            drawingsurface.Paint += drawingsurface_Paint;
            drawingsurface.SizeChanged += drawingsurface_SizeChanged;

            drawingsurface.Select();
            drawingsurface.Focus();
        }

        private void RichTextBoxLog_Click(object sender, EventArgs e)
        {
            drawingsurface.Focus();
        }

        private void drawingsurface_SizeChanged(object sender, EventArgs e)
        {
            _core.ResizeDrawingSurface(new Size(drawingsurface.Width, drawingsurface.Height));
        }

        private void _core_OnLog(EngineCore core, string text, Color color)
        {
            richTextBoxLog.AppendText(text, color);
            richTextBoxLog.HideSelection = true;
            richTextBoxLog.SelectionStart = richTextBoxLog.Text.Length;
            richTextBoxLog.ScrollToCaret();
        }

        private void _core_AfterTick(EngineCore core, Types.TickInput input, Library.Types.Point<double> offsetApplied)
        {
            var time = TimeSpan.FromMinutes(core.Tick.TimePassed);

            //toolStripStatusLabelDebug.Text = $"{_core.Player.X},{_core.Player.Y} {_core.Player.Meta.HitPoints}hp"
                //+ " " + time.ToString(@"dd\:hh\:mm\:ss");
        }

        private void _core_OnStart(EngineCoreBase sender)
        {
            sender.AddNewMap<MapHome>();

            if (string.IsNullOrWhiteSpace(_mapPathPassedToGame) == false)
            {
                MapPersistence.Load(_core, _mapPathPassedToGame);

                _core.Player = _core.Actors.OfType<ActorPlayer>().FirstOrDefault();

                if (_core.Player == null)
                {
                    MessageBox.Show("This map has no player.");
                }
            }
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

        private void drawingsurface_KeyUp(object sender, KeyEventArgs e)
        {
            /*
            if (e.KeyCode == Keys.ShiftKey) _core.Input.KeyStateChanged(PlayerKey.SpeedBoost, KeyPressState.Up);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(PlayerKey.Forward, KeyPressState.Up);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(PlayerKey.RotateCounterClockwise, KeyPressState.Up);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(PlayerKey.Reverse, KeyPressState.Up);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(PlayerKey.RotateClockwise, KeyPressState.Up);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(PlayerKey.PrimaryFire, KeyPressState.Up);
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(PlayerKey.SecondaryFire, KeyPressState.Up);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(PlayerKey.Escape, KeyPressState.Up);
            if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(PlayerKey.Left, KeyPressState.Up);
            if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(PlayerKey.Right, KeyPressState.Up);
            if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(PlayerKey.Up, KeyPressState.Up);
            if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(PlayerKey.Down, KeyPressState.Up);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(PlayerKey.Enter, KeyPressState.Up);
            */
        }

        private void drawingsurface_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (e.KeyCode == Keys.ShiftKey) _core.Input.KeyStateChanged(PlayerKey.SpeedBoost, KeyPressState.Down);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(PlayerKey.Forward, KeyPressState.Down);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(PlayerKey.RotateCounterClockwise, KeyPressState.Down);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(PlayerKey.Reverse, KeyPressState.Down);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(PlayerKey.RotateClockwise, KeyPressState.Down);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(PlayerKey.PrimaryFire, KeyPressState.Down);
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(PlayerKey.SecondaryFire, KeyPressState.Down);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(PlayerKey.Escape, KeyPressState.Down);
            if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(PlayerKey.Left, KeyPressState.Down);
            if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(PlayerKey.Right, KeyPressState.Down);
            if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(PlayerKey.Up, KeyPressState.Down);
            if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(PlayerKey.Down, KeyPressState.Down);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(PlayerKey.Enter, KeyPressState.Down);
            */

            _core.Input.HandleSingleKeyPress(e.KeyCode);
        }

        private void drawingsurface_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_core.Render(), 0, 0);
        }

        private void drawingsurface_Click(object sender, EventArgs e)
        {
            drawingsurface.Focus();
        }
    }
}
