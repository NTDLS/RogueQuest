using Game.Actors;
using Game.Engine;
using Game.Extensions;
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
        public string _levelPathPassedToGame { get; set; }
        public int _levelIndexPassedToGame { get; set; }

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

        public FormMain(string levelPath = null, int levelIndex = 0)
        {
            InitializeComponent();
            _levelPathPassedToGame = levelPath;
            _levelIndexPassedToGame = levelIndex;
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
            this.Shown += FormMain_Shown1;

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
            System.Reflection.PropertyInfo controlProperty = typeof(Control)
                    .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            controlProperty.SetValue(drawingsurface, true, null);

            splitContainerHoriz.Panel1.Controls.Add(drawingsurface);
            drawingsurface.Dock = DockStyle.Fill;

            richTextBoxLog.Click += RichTextBoxLog_Click;
            drawingsurface.KeyDown += drawingsurface_KeyDown;
            drawingsurface.Paint += drawingsurface_Paint;
            drawingsurface.SizeChanged += drawingsurface_SizeChanged;
            drawingsurface.MouseDown += Drawingsurface_MouseDown;
            drawingsurface.MouseUp += Drawingsurface_MouseUp;
            drawingsurface.MouseMove += Drawingsurface_MouseMove;

            drawingsurface.Select();
            drawingsurface.Focus();

            _core = new EngineCore(drawingsurface, new Size(drawingsurface.Width, drawingsurface.Height));
            _core.BlindPlay = true;
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

            toolStripStatusLabelDebug.Text = $"Mouse: {e.X:N0}x,{e.Y:N0}y, Screen: {x:N0}x,{y:N0}y, Offset: {_core.Display.BackgroundOffset.X:N0}x,{_core.Display.BackgroundOffset.Y:N0}y";

            /*


            var hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1))
                .Where(o => o.Visible == true).OrderBy(o => o.DrawOrder).LastOrDefault();
            if (hoverTile == null)
            {
                return;
            }

            string tipText = hoverTile.Meta?.Name;

            if (hoverTile != null)
            {
                if (hoverTile.Meta.ActorClass == Library.Engine.Types.ActorClassName.ActorHostileBeing)
                {
                    var hostile = (hoverTile as ActorHostileBeing);
                    tipText += $" ({hostile.DamageText})";
                }
            }

            toolStripStatusLabelDebug.Text = tipText;
            */
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
                    string text = hoverTile?.Meta.Name;

                    if (hoverTile.Meta.ActorClass == Library.Engine.Types.ActorClassName.ActorHostileBeing)
                    {
                        var hostile = (hoverTile as ActorHostileBeing);
                        text += $" ({hostile.DamageText})";
                    }
                    else if (hoverTile.Meta.ActorClass == Library.Engine.Types.ActorClassName.ActorItem)
                    {
                        text += "\r\n" + $"Weight: {hoverTile.Meta.Bulk:N0}";
                        text += "\r\n" + $"Bulk: {hoverTile.Meta.Weight:N0}";
                    }

                    if (string.IsNullOrWhiteSpace(text) == false)
                    {
                        var location = new Point((int)hoverTile.ScreenX, (int)hoverTile.ScreenY - hoverTile.Size.Height);
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
            _core.ActionRest();
        }

        private void ToolStripButtonGet_Click(object sender, EventArgs e)
        {
            _core.ActionGet();
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
                    richTextBoxLog.Clear();
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
            if (_hasBeenModified && string.IsNullOrWhiteSpace(_levelPathPassedToGame))
            {
                if (_core.Player?.Visible == false)
                {
                    return true; //Player is dead, probably a bad idea to save.
                }

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
                dialog.InitialDirectory = Constants.SaveFolder;

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
                        form.ScenerioFile,
                        form.CharacterName,
                        form.SelectedAvatar,
                        form.Dexterity,
                        form.Constitution,
                        form.Intelligence,
                        form.Strength);

                    UpdatePlayerStatLabels(_core);

                    _currentMapFilename = null;
                    _hasBeenModified = true;

                    richTextBoxLog.Clear();
                }
            }
        }

        #endregion

        private void RichTextBoxLog_Click(object sender, EventArgs e)
        {
            drawingsurface.Focus();
        }

        private bool _firstShown = true;
        private void FormMain_Shown1(object sender, EventArgs e)
        {
            if (_firstShown == true)
            {
                if (string.IsNullOrWhiteSpace(_levelPathPassedToGame) == false)
                {
                    _core.Load(_levelPathPassedToGame);

                    _core.State.Character = new PlayerState()
                    {
                        UID = Guid.NewGuid(),
                        Experience = 0,
                        Name = "Tester",
                        Avatar = 1,
                        Level = 1,
                        StartingDexterity = 100,
                        StartingConstitution = 100,
                        StartingIntelligence = 100,
                        StartingStrength = 100
                    };

                    _core.State.Character.InitializeState();
                    _core.State.CurrentLevel = _core.State.DefaultLevel;

                    _core.Levels.PopLevel(_levelIndexPassedToGame);

                    var spawnPoint = _core.Actors.OfType<ActorSpawnPoint>().FirstOrDefault();
                    if (spawnPoint == null)
                    {
                        MessageBox.Show("This level contains no Spawn Point and cannot be played.");
                        return;
                    }

                    _core.Actors.AddNew<ActorPlayer>(spawnPoint.X, spawnPoint.Y, @$"Tiles\Special\@Player\{_core.State.Character.Avatar}\Front 1");
                    _core.Player = _core.Actors.OfType<ActorPlayer>().FirstOrDefault();
                    _core.Player.DrawOrder = _core.Actors.Tiles.Max(o => o.DrawOrder) + 1;

                    _core.Player.Meta = new TileMetadata()
                    {
                        ActorClass = Library.Engine.Types.ActorClassName.ActorPlayer,
                        //Should we store the player stats here???
                    };

                    spawnPoint.Visible = false; //Keep the spawn point here so we can place the player over it if we ever come back to this level.

                    _core.Display.BackgroundOffset.Y = _core.Player.Y / 2;
                    _core.Display.BackgroundOffset.X = _core.Player.X / 2;

                    UpdatePlayerStatLabels(_core);

                    return;
                }

                using (var form = new FormWelcome())
                {
                    var result = form.ShowDialog();

                    if (result == DialogResult.Yes)
                    {
                        NewToolStripMenuItem_Click(null, null);
                    }
                    else if (result == DialogResult.OK)
                    {
                        _currentMapFilename = form.SelectedFileName;
                        _core.LoadGame(_currentMapFilename);
                        UpdatePlayerStatLabels(_core);
                        _hasBeenModified = false;
                    }

                }

                _firstShown = false;
            }
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

        private void drawingsurface_KeyDown(object sender, KeyEventArgs e)
        {
            if (_core.Player == null || _core.Player.Visible == false)
            {
                return; //Player is dead.
            }

            if (_core.State.IsDialogActive)
            {
                _core.ActionDialogInput();
                return;
            }

            _hasBeenModified = true;

            if (e.KeyCode == Keys.G)
            {
                _core.ActionGet();
            }
            else if (e.KeyCode == Keys.I)
            {
                using (var form = new FormInventory(_core))
                {
                    form.ShowDialog();
                }
            }
            else if (e.KeyCode == Keys.R)
            {
                _core.ActionRest();
            }
            else
            {
                _core.HandleSingleKeyPress(e.KeyCode);
            }
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
