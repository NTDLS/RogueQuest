using Game.Actors;
using Game.Engine;
using Game.Extensions;
using Game.Maps;
using Library.Engine;
using Library.Types;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public partial class FormMain : Form
    {
        private EngineCore _core;
        private bool _fullScreen = false;
        private ToolTip _interrogationTip = new ToolTip();

        private bool _hasBeenModified = false;
        private string _currentMapFilename = string.Empty;
        private int _newFilenameIncrement = 1;

        /// <summary>
        /// Really just for debugging.
        /// </summary>
        public string _mapPathPassedToGame { get; set; }
        public string _gamePathPassedToGame { get; set; }

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

        public FormMain(string mapPath = null, string gamePath = null)
        {
            InitializeComponent();
            _mapPathPassedToGame = mapPath;
            _gamePathPassedToGame = gamePath;
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

            newToolStripMenuItem.Click += NewToolStripMenuItem_Click;
            saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;

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
            drawingsurface.MouseDown += Drawingsurface_MouseDown;
            drawingsurface.MouseUp += Drawingsurface_MouseUp;
            drawingsurface.MouseMove += Drawingsurface_MouseMove;

            drawingsurface.Select();
            drawingsurface.Focus();

            _core = new EngineCore(this.drawingsurface, new Size(this.drawingsurface.Width, this.drawingsurface.Height));
            _core.OnStop += _core_OnStop;
            _core.OnStart += _core_OnStart;
            _core.AfterTick += _core_AfterTick;
            _core.OnLog += _core_OnLog;

            toolStripButtonGet.Click += ToolStripButtonGet_Click;
            toolStripButtonRest.Click += ToolStripButtonRest_Click;
            toolStripButtonSave.Click += ToolStripButtonSave_Click;
        }

        private void Drawingsurface_MouseMove(object sender, MouseEventArgs e)
        {
            double x = e.X + _core.Display.BackgroundOffset.X;
            double y = e.Y + _core.Display.BackgroundOffset.Y;

            var hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1)).OrderBy(o => o.DrawOrder).LastOrDefault();

            string tipText = "";

            if (hoverTile != null)
            {
                if (hoverTile.Meta.BasicType == Library.Engine.Types.BasicTileType.ActorHostileBeing)
                {
                    var hostile = (hoverTile as ActorHostileBeing);
                    tipText = $"{hostile?.Meta.Name} ({hostile.DamageText})";
                }
            }

            toolStripStatusLabelDebug.Text = tipText;
        }

        private void Drawingsurface_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _interrogationTip.Hide(drawingsurface);
            }
        }

        private void Drawingsurface_MouseDown(object sender, MouseEventArgs e)
        {
            double x = e.X + _core.Display.BackgroundOffset.X;
            double y = e.Y + _core.Display.BackgroundOffset.Y;

            var hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1)).OrderBy(o => o.DrawOrder).LastOrDefault();

            if (e.Button == MouseButtons.Right)
            {
                if (hoverTile != null)
                {
                    if (hoverTile.Meta.BasicType == Library.Engine.Types.BasicTileType.ActorHostileBeing)
                    {
                        var hostile = (hoverTile as ActorHostileBeing);
                        var location = new Point((int)hostile.ScreenX, (int)hostile.ScreenY - hostile.Size.Height);
                        string text = $"{hostile?.Meta.Name} ({hostile.DamageText})";
                        _interrogationTip.Show(text, drawingsurface, location, 5000);
                    }
                }
            }
        }


        #region Menu Clicks.

        private void ToolStripButtonSave_Click(object sender, EventArgs e)
        {
            //If we already have an open file, then just save it.
            if (string.IsNullOrWhiteSpace(_currentMapFilename) == false)
            {
                _core.SaveGame(_currentMapFilename);
                _hasBeenModified = false;
            }
            else //If we do not have a current open file, then we need to "Save As".
            {
                TrySave();
            }
        }

        private void ToolStripButtonRest_Click(object sender, EventArgs e)
        {
            _core.Rest();
        }

        private void ToolStripButtonGet_Click(object sender, EventArgs e)
        {
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckForNeededSave() == false)
            {
                return;
            }

            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "RogueQuest Games (*.rqg)|*.rqg|All files (*.*)|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _currentMapFilename = dialog.FileName;
                    _core.LoadGame(_currentMapFilename);
                    UpdatePlayerStatLabels(_core);
                    _hasBeenModified = false;
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //If we already have an open file, then just save it.
            if (string.IsNullOrWhiteSpace(_currentMapFilename) == false)
            {
                _core.SaveGame(_currentMapFilename);
                _hasBeenModified = false;
            }
            else //If we do not have a current open file, then we need to "Save As".
            {
                TrySave();
            }
        }

        /// <summary>
        /// Do not continue if this returns false.
        /// </summary>
        /// <returns></returns>
        bool CheckForNeededSave()
        {
            if (_hasBeenModified)
            {
                var result = MessageBox.Show("The game has been played since it was last saved. Save it now?",
                    "Save before continuing?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    return TrySave();
                }
                else if (result == DialogResult.Cancel)
                {
                    return false;
                }
            }

            return true;
        }

        bool TrySave()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.FileName = $"Game {_newFilenameIncrement++}";
                dialog.Filter = "RogueQuest Games (*.rqg)|*.rqg|All files (*.*)|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _currentMapFilename = dialog.FileName;

                    _core.SaveGame(_currentMapFilename);
                    _hasBeenModified = false;
                    return true;
                }
            }
            return false;
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new FormNewCharacter())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _core.NewGame(
                        form.CharacterName,
                        form.Dexterity,
                        form.Constitution,
                        form.Intelligence,
                        form.Strength);

                    UpdatePlayerStatLabels(_core);
                }
            }
        }

        #endregion

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
            UpdatePlayerStatLabels(core);
        }

        private void UpdatePlayerStatLabels(EngineCore core)
        {
            var time = TimeSpan.FromMinutes(core.Tick.TimePassed);
            labelPlayer.Text = $"{core.State.Character.Name}, Level {core.State.Character.Level:N0}";
            labelXP.Text = $"{core.State.Character.Experience.ToString():N0}/{core.State.Character.NextLevelExperience.ToString():N0}";
            labelHP.Text = $"{core.State.Character.AvailableHitpoints:N0}";
            labelMana.Text = $"{core.State.Character.AvailableMana:N0}";
            labelTime.Text = time.ToString(@"dd\:hh\:mm\:ss");
        }

        private void _core_OnStart(EngineCoreBase sender)
        {
            sender.AddNewMap<MapHome>();

            if (string.IsNullOrWhiteSpace(_gamePathPassedToGame) == false)
            {
                _core.LoadGame(_gamePathPassedToGame);
                UpdatePlayerStatLabels(_core);

                if (_core.Player == null)
                {
                    MessageBox.Show("This map has no player.");
                }
            }

            if (string.IsNullOrWhiteSpace(_mapPathPassedToGame) == false)
            {
                MapPersistence.Load(_core, _mapPathPassedToGame);

                _core.Player = _core.Actors.OfType<ActorPlayer>().FirstOrDefault();

                _core.State.Character = new PlayerState()
                {
                    UID = Guid.NewGuid(),
                    Experience = 0,
                    Name = "Player",
                    Level = 1,
                    StartingDexterity = 10,
                    StartingConstitution = 10,
                    StartingIntelligence = 10,
                    StartingStrength = 10
                };

                _core.State.Character.InitializeState();

                if (_core.Player == null)
                {
                    MessageBox.Show("This map has no player.");
                }
            }
        }

        private void _core_OnStop(EngineCoreBase sender)
        {
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CheckForNeededSave() == false)
            {
                e.Cancel = true;
                return;
            }

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
            _hasBeenModified = true;
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
