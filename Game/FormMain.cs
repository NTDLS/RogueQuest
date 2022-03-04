using Assets;
using Game.Actors;
using Game.Classes;
using Game.Engine;
using Game.Extensions;
using Library.Engine;
using Library.Engine.Types;
using Library.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public partial class FormMain : Form
    {
        enum MouseMode
        {
            None,
            RangedTargetSelction //Wand or ranged weapon.
        }

        private EngineCore _core;
        private bool _fullScreen = false;
        private ToolTip _interrogationTip = new ToolTip();

        private bool _hasBeenModified = false;
        private string _currentMapFilename = string.Empty;
        private int _newFilenameIncrement = 1;

        private MouseMode CurrentMouseMode = MouseMode.None;
        private QuickItemButtonInfo PendingQuickItemMouseOperation = null;

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
            this.Shown += FormMain_Shown;

            splitContainerHoriz.Cursor = Cursors.Default;
            splitContainerVert.Cursor = Cursors.Default;

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
            drawingsurface.MouseClick += Drawingsurface_MouseClick;

            drawingsurface.Select();
            drawingsurface.Focus();

            _core = new EngineCore(drawingsurface, new Size(drawingsurface.Width, drawingsurface.Height));
            _core.BlindPlay = true;
            _core.OnStop += _core_OnStop;
            _core.OnStart += _core_OnStart;
            _core.AfterTick += _core_AfterTick;
            _core.OnLog += _core_OnLog;
            _core.OnLevelUp += _core_OnLevelUp;

            toolStripButtonGet.Click += ToolStripButtonGet_Click;
            toolStripButtonRest.Click += ToolStripButtonRest_Click;
            toolStripButtonSave.Click += ToolStripButtonSave_Click;

            var timer = new Timer()
            {
                Interval = 1000
            };

            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void _core_OnLevelUp(EngineCore core)
        {
            if (this.InvokeRequired)
            {
                Action safeWrite = delegate
                {
                    _core_OnLevelUp(core);
                };
                this.Invoke(safeWrite);
            }
            else
            {
                using (var spellForm = new FormPickNewSpell(_core))
                {
                    if (spellForm.ShowDialog() == DialogResult.OK)
                    {
                        var spell = spellForm.SelectedSpell.DeriveCopy();
                        spell.Meta.IsSpell = true;
                        _core.State.Character.KnownSpells.Add(spell);
                    }
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdatePlayerStatLabels(_core);
        }

        private void Drawingsurface_MouseClick(object sender, MouseEventArgs e)
        {
            if (_core.State.ActiveThreadCount > 0)
            {
                return;
            }

            if (CurrentMouseMode == MouseMode.RangedTargetSelction)
            {
                if (e.Button == MouseButtons.Right)
                {
                    PendingQuickItemMouseOperation = null;
                    CurrentMouseMode = MouseMode.None;
                    splitContainerHoriz.Cursor = Cursors.Default;
                }
                else if (e.Button == MouseButtons.Left)
                {
                    var tag = PendingQuickItemMouseOperation;
                    var inventoryItem = _core.State.Items.Where(o => o.Tile.Meta.UID == tag.UID).First();

                    double x = e.X + _core.Display.BackgroundOffset.X;
                    double y = e.Y + _core.Display.BackgroundOffset.Y;

                    ActorBase hoverTile = null;

                    if (inventoryItem.Tile.Meta.TargetType == TargetType.HostileBeing)
                    {
                        hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1))
                            .Where(o => o.Visible == true && o.Meta.ActorClass == ActorClassName.ActorHostileBeing)
                            .OrderBy(o => o.DrawOrder).LastOrDefault();
                    }
                    else if (inventoryItem.Tile.Meta.TargetType == TargetType.Terrain)
                    {
                        hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1))
                            .Where(o => o.Visible == true && o.Meta.ActorClass == ActorClassName.ActorTerrain)
                            .OrderBy(o => o.DrawOrder).LastOrDefault();
                    }
                    if (hoverTile == null)
                    {
                        return;
                    }

                    PendingQuickItemMouseOperation = null;
                    CurrentMouseMode = MouseMode.None;
                    splitContainerHoriz.Cursor = Cursors.Default;

                    if (inventoryItem.Tile.Meta.SubType == ActorSubType.Wand || inventoryItem.Tile.Meta.SubType == ActorSubType.Scroll)
                    {
                        if (inventoryItem.Tile.Meta.IsConsumable == true)
                        {
                            UseWandOrScroll(inventoryItem.Tile, hoverTile);
                        }
                    }
                    else if (inventoryItem.Tile.Meta.SubType == ActorSubType.RangedWeapon && inventoryItem.Tile.Meta.ProjectileType != null)
                    {
                        var projectile = _core.State.Character.GetQuiverSlotOfType((ProjectileType)inventoryItem.Tile.Meta.ProjectileType);
                        if (projectile != null && projectile.Tile != null)
                        {
                            if (projectile.Tile.Meta.IsConsumable == true)
                            {
                                UseRangedWeapon(inventoryItem.Tile, hoverTile);
                            }
                        }
                        else
                        {
                            Constants.Alert("You have no projectiles equipped for this item. Add them to the quiver.");
                        }
                    }

                    UpdatePlayerStatLabels(_core);
                }
            }
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

            var hoverTile = _core.Actors.Intersections(new Point<double>(x, y), new Point<double>(1, 1)).OrderBy(o => o.DrawOrder ?? 0).LastOrDefault();

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
                        form.Strength,
                        form.StartingSpell);

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
        private void FormMain_Shown(object sender, EventArgs e)
        {
            if (_firstShown == true)
            {
                _core.Start();

                if (string.IsNullOrWhiteSpace(_levelPathPassedToGame) == false)
                {
                    _core.Load(_levelPathPassedToGame);

                    _core.State.Character = new PlayerState(_core)
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
                        Constants.Alert("This level contains no Spawn Point and cannot be played.");
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

                    _core.Display.BackgroundOffset.Y = _core.Player.Y - (_core.Display.DrawingSurface.Height / 2.0);
                    _core.Display.BackgroundOffset.X = _core.Player.X - (_core.Display.DrawingSurface.Width / 2.0);

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
            if (richTextBoxLog.InvokeRequired)
            {
                Action safeWrite = delegate
                {
                    _core_OnLog(core, text, color);
                };
                richTextBoxLog.Invoke(safeWrite);
            }
            else
            {
                richTextBoxLog.AppendText(text, color);
                richTextBoxLog.HideSelection = true;
                richTextBoxLog.SelectionStart = richTextBoxLog.Text.Length;
                richTextBoxLog.ScrollToCaret();
            }
        }

        private void _core_AfterTick(EngineCore core, Types.TickInput input, Library.Types.Point<double> offsetApplied)
        {
            UpdatePlayerStatLabels(core);
        }

        #region Utility.

        private ImageList _imageList = new ImageList();

        private string GetImageKey(string tilePath)
        {
            if (_imageList.Images.Keys.Contains(tilePath))
            {
                return tilePath;
            }

            var bitmap = SpriteCache.GetBitmapCached(Assets.Constants.GetAssetPath($"{tilePath}.png"));
            _imageList.Images.Add(tilePath, bitmap);

            return tilePath;
        }

        private Image GetImage(string tilePath)
        {
            if (_imageList.Images.Keys.Contains(tilePath))
            {
                return _imageList.Images[tilePath];
            }

            var bitmap = SpriteCache.GetBitmapCached(Assets.Constants.GetAssetPath($"{tilePath}.png"));
            _imageList.Images.Add(tilePath, bitmap);

            return bitmap;
        }

        #endregion

        private void UpdateKnownSpells()
        {
            var currentlyLoadedSpells = new List<QuickItemButtonInfo>();
            var itemsToRemove = new List<ToolStripItem>();

            foreach (var item in toolStripDropDownButtonSpells.DropDownItems)
            {
                var tag = ((ToolStripItem)item).Tag as QuickItemButtonInfo;

                if (_core.State.Character.KnownSpells.Where(o => o.Meta.UID == tag.Tile.Meta.UID).Any() == false)
                {
                    itemsToRemove.Add(item as ToolStripItem);
                }

                currentlyLoadedSpells.Add(tag);
            }

            foreach (var item in itemsToRemove)
            {
                toolStripDropDownButtonSpells.DropDownItems.Remove(item);
            }

            foreach (var spell in _core.State.Character.KnownSpells)
            {
                if (currentlyLoadedSpells.Where(o => o.UID == spell.Meta.UID).Any() == false)
                {
                    QuickItemButtonInfo info = new QuickItemButtonInfo()
                    {
                        Tile = spell,
                        UID = (Guid)spell.Meta.UID
                    };

                    var item = toolStripDropDownButtonSpells.DropDownItems.Add(spell.Meta.Name);
                    item.Tag = info;
                    item.ToolTipText = $"{spell.Meta.Name} ({spell.Meta.Mana} mana)";
                    item.Image = GetImage(spell.TilePath);
                    item.Click += KnownSpellItem_Click;

                    _core.State.Items.Add(new CustodyItem()
                    {
                        Tile = spell,
                        ContainerId = null
                    });
                }
            }
        }

        private void KnownSpellItem_Click(object sender, EventArgs e)
        {
            PendingQuickItemMouseOperation = null;
            CurrentMouseMode = MouseMode.None;
            splitContainerHoriz.Cursor = Cursors.Default;

            if (sender is ToolStripItem && ((ToolStripItem)sender).Tag is QuickItemButtonInfo)
            {
                var tag = (QuickItemButtonInfo)(((ToolStripItem)sender).Tag);
                var inventoryItem = _core.State.Items.Where(o => o.Tile.Meta.UID == tag.UID).FirstOrDefault();

                if (inventoryItem == null)
                {
                    return;
                }

                if (_core.State.Character.AvailableMana < inventoryItem.Tile.Meta.Mana)
                {
                    Constants.Alert("You do not have enough mana to activate this spell.");
                    return;
                }

                if ((inventoryItem.Tile.Meta.SubType == ActorSubType.Wand && inventoryItem.Tile.Meta.TargetType != TargetType.Self)
                    || (inventoryItem.Tile.Meta.SubType == ActorSubType.Scroll && inventoryItem.Tile.Meta.TargetType != TargetType.Self))
                {
                    splitContainerHoriz.Cursor = Cursors.Cross;
                    _core.LogLine($"Select a target for the {inventoryItem.Tile.Meta.Name}... (right-click to cancel)");
                    CurrentMouseMode = MouseMode.RangedTargetSelction;
                    PendingQuickItemMouseOperation = tag;
                }
                else if (inventoryItem.Tile.Meta.SubType == ActorSubType.Scroll && inventoryItem.Tile.Meta.TargetType == TargetType.Self)
                {
                    if (inventoryItem.Tile.Meta.IsConsumable == true)
                    {
                        if (UseSelfPotionOrScroll(inventoryItem.Tile))
                        {

                        }
                    }
                }
            }

            UpdatePlayerStatLabels(_core);
        }

        private void UpdateQuickSlots()
        {
            if (toolStripQuick.ImageList == null)
            {
                toolStripQuick.ImageList = _imageList;
            }

            var existingButtons = new List<QuickItemButtonInfo>();
            //A list of what items would have been added. We use to remove items that are no longer available.
            var availableButtons = new List<QuickItemButtonInfo>();

            foreach (var item in toolStripQuick.Items)
            {
                if (item is ToolStripButton button && button.Tag is QuickItemButtonInfo)
                {
                    existingButtons.Add((QuickItemButtonInfo)button.Tag);
                }
            }

            var freeHand = _core.State.Character.GetEquipSlot(EquipSlot.FreeHand);
            if (freeHand.Tile != null &&
                (freeHand.Tile.Meta.SubType == ActorSubType.RangedWeapon
                || freeHand.Tile.Meta.SubType == ActorSubType.Potion
                || freeHand.Tile.Meta.SubType == ActorSubType.Scroll
                || freeHand.Tile.Meta.SubType == ActorSubType.Wand))
            {
                var info = UpsertQuickSlotItem(freeHand.Tile, existingButtons);
                availableButtons.Add(info);
            }

            var weaponHand = _core.State.Character.GetEquipSlot(EquipSlot.Weapon);
            if (weaponHand.Tile != null && weaponHand.Tile.Meta.SubType == ActorSubType.RangedWeapon)
            {
                var info = UpsertQuickSlotItem(weaponHand.Tile, existingButtons);
                availableButtons.Add(info);
            }

            var belt = _core.State.Character.GetEquipSlot(EquipSlot.Belt);
            if (belt.Tile != null)
            {
                var items = _core.State.Items.Where(o => o.ContainerId == belt.Tile.Meta.UID).ToList();

                foreach (var item in items)
                {
                    var info = UpsertQuickSlotItem(item.Tile, existingButtons);
                    availableButtons.Add(info);
                }
            }

            var removedItemsUids = existingButtons.Select(o => o.UID).Except(availableButtons.Select(o => o.UID));

            foreach (var removedItemsUid in removedItemsUids)
            {
                toolStripQuick.Items.Remove(existingButtons.Where(o => o.UID == removedItemsUid).First().Button);
            }
        }

        private QuickItemButtonInfo UpsertQuickSlotItem(TileIdentifier tile, List<QuickItemButtonInfo> existingButtons)
        {
            var info = new QuickItemButtonInfo()
            {
                UID = (Guid)tile.Meta.UID,
                Tile = tile
            };

            if (existingButtons.Where(o => o.UID == (Guid)tile.Meta.UID).Any())
            {
                info.Button = existingButtons.Where(o => o.UID == (Guid)tile.Meta.UID).First().Button;

                string text = tile.Meta.Name;
                if (tile.Meta.Charges > 0)
                {
                    text += $"\r\n{tile.Meta.Charges} charges remaining.";
                }
                if (tile.Meta.Quantity > 0)
                {
                    text += $"\r\n{tile.Meta.Quantity} remaining.";
                }

                if (tile.Meta.ProjectileType != null && tile.Meta.ProjectileType != ProjectileType.Unspecified)
                {
                    var projectile = _core.State.Character.GetQuiverSlotOfType((ProjectileType)tile.Meta.ProjectileType);
                    if (projectile != null && projectile.Tile != null)
                    {
                        if (projectile.Tile.Meta.Quantity > 0)
                        {
                            text += $"\r\n{projectile.Tile.Meta.Quantity} remaining.";
                        }
                    }
                }

                info.Button.ToolTipText = text;
            }
            else
            {
                string text = tile.Meta.Name;
                if (tile.Meta.Charges > 0)
                {
                    text += $"\r\n{tile.Meta.Charges} charges remaining.";
                }
                if (tile.Meta.Quantity > 0)
                {
                    text += $"\r\n{tile.Meta.Quantity} remaining.";
                }

                if (tile.Meta.ProjectileType != null && tile.Meta.ProjectileType != ProjectileType.Unspecified)
                {
                    var projectile = _core.State.Character.GetQuiverSlotOfType((ProjectileType)tile.Meta.ProjectileType);
                    if (projectile != null && projectile.Tile != null)
                    {
                        if (projectile.Tile.Meta.Quantity > 0)
                        {
                            text += $"\r\n{projectile.Tile.Meta.Quantity} remaining.";
                        }
                    }
                }

                var button = new ToolStripButton();
                button.ImageKey = GetImageKey(tile.TilePath);
                button.Tag = info;
                button.ToolTipText = text;
                button.Click += Button_Click;

                info.Button = button;

                toolStripQuick.Items.Add(button);
            }

            return info;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            PendingQuickItemMouseOperation = null;
            CurrentMouseMode = MouseMode.None;
            splitContainerHoriz.Cursor = Cursors.Default;

            if (sender is ToolStripButton && ((ToolStripButton)sender).Tag is QuickItemButtonInfo)
            {
                var tag = (QuickItemButtonInfo)(((ToolStripButton)sender).Tag);
                var inventoryItem = _core.State.Items.Where(o => o.Tile.Meta.UID == tag.UID).FirstOrDefault();

                if (inventoryItem == null)
                {
                    return;
                }

                if (inventoryItem.Tile.Meta.SubType == ActorSubType.RangedWeapon
                    || (inventoryItem.Tile.Meta.SubType == ActorSubType.Wand && inventoryItem.Tile.Meta.TargetType != TargetType.Self)
                    || (inventoryItem.Tile.Meta.SubType == ActorSubType.Scroll && inventoryItem.Tile.Meta.TargetType != TargetType.Self))
                {
                    splitContainerHoriz.Cursor = Cursors.Cross;
                    _core.LogLine($"Select a target for the {inventoryItem.Tile.Meta.Name}... (right-click to cancel)");
                    CurrentMouseMode = MouseMode.RangedTargetSelction;
                    PendingQuickItemMouseOperation = tag;
                }
                else if (inventoryItem.Tile.Meta.SubType == ActorSubType.Potion
                    || (inventoryItem.Tile.Meta.SubType == ActorSubType.Scroll && inventoryItem.Tile.Meta.TargetType == TargetType.Self))
                {
                    if (inventoryItem.Tile.Meta.IsConsumable == true)
                    {
                        if (UseSelfPotionOrScroll(inventoryItem.Tile))
                        {
                            if ((inventoryItem.Tile.Meta.Quantity ?? 0) == 0)
                            {
                                toolStripQuick.Items.Remove(tag.Button);
                            }
                            else
                            {
                                string text = inventoryItem.Tile.Meta.Name;
                                if (inventoryItem.Tile.Meta.CanStack == true && inventoryItem.Tile.Meta.Quantity > 0)
                                {
                                    text += $" ({inventoryItem.Tile.Meta.Quantity})";
                                }
                                tag.Button.ToolTipText = text;
                            }
                        }
                    }
                }
            }

            UpdatePlayerStatLabels(_core);
        }

        private bool UseWandOrScroll(TileIdentifier wand, ActorBase target)
        {
            var inventoryItem = _core.State.Items.Where(o => o.Tile.Meta.UID == wand.Meta.UID).First();

            if (inventoryItem != null && inventoryItem.Tile.Meta.UID != null)
            {
                var result = _core.Tick.UseConsumableItem((Guid)inventoryItem.Tile.Meta.UID, target);
                _core.State.Character.PushFreshInventoryItemsToEquipSlots();
                return result;
            }

            return false;
        }

        private bool UseRangedWeapon(TileIdentifier rangedWeapon, ActorBase target)
        {
            var inventoryItem = _core.State.Items.Where(o => o.Tile.Meta.UID == rangedWeapon.Meta.UID).First();

            if (inventoryItem != null && inventoryItem.Tile.Meta.UID != null)
            {
                var result = _core.Tick.UseConsumableItem((Guid)inventoryItem.Tile.Meta.UID, target/*, projectile.Meta.UID*/);
                _core.State.Character.PushFreshInventoryItemsToEquipSlots();
                return result;
            }

            return false;
        }

        private bool UseSelfPotionOrScroll(TileIdentifier item)
        {
            string message = $"Use {item.Meta.Name}?";
            if (item.Meta.IsSpell == true)
            {
                message = $"Use {item.Meta.Name} ({item.Meta.Mana} mana)?";
            }

            if (MessageBox.Show(message, $"RougeQuest :: Use Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var inventoryItem = _core.State.Items.Where(o => o.Tile.Meta.UID == item.Meta.UID).First();

                if (inventoryItem != null && inventoryItem.Tile.Meta.UID != null)
                {
                    var result = _core.Tick.UseConsumableItem((Guid)inventoryItem.Tile.Meta.UID, null);
                    _core.State.Character.PushFreshInventoryItemsToEquipSlots();
                    return result;
                }
            }

            return false;
        }

        private void UpdatePlayerStatLabels(EngineCore core)
        {
            var time = TimeSpan.FromSeconds(core.State.TimePassed);
            labelPlayer.Text = $"{core.State.Character.Name}, Level {core.State.Character.Level:N0}";
            labelXP.Text = $"{core.State.Character.Experience.ToString():N0}/{core.State.Character.NextLevelExperience.ToString():N0}";
            labelHP.Text = $"{core.State.Character.AvailableHitpoints:N0}";
            labelMana.Text = $"{core.State.Character.AvailableMana:N0}";
            labelTime.Text = time.ToString(@"dd\:hh\:mm\:ss");
            labelGold.Text = $"{core.State.Character.Money:N0}gp";

            UpdateQuickSlots();
            UpdateKnownSpells(); 

            labelPlayer.ForeColor = Color.Black;

            string modifier = "";

            if (core.State.ActorStates.HasState(core.State.Character.UID, StateOfBeing.Poisoned))
            {
                modifier += $"POISON,";
                labelPlayer.ForeColor = Color.Red;
            }
            if (core.State.ActorStates.HasState(core.State.Character.UID, StateOfBeing.IncreaseStrength))
            {
                modifier += "STR+,";
            }
            if (core.State.ActorStates.HasState(core.State.Character.UID, StateOfBeing.IncreaseIntelligence))
            {
                modifier += "INT+,";
            }
            if (core.State.ActorStates.HasState(core.State.Character.UID, StateOfBeing.IncreaseConstitution))
            {
                modifier += "CON+,";
            }
            if (core.State.ActorStates.HasState(core.State.Character.UID, StateOfBeing.IncreaseDexterity))
            {
                modifier += "DEX+,";
            }
            if (core.State.ActorStates.HasState(core.State.Character.UID, StateOfBeing.IncreaseArmorClass))
            {
                modifier += "AC+,";
            }

            modifier = modifier.Trim(new char[] { ',' });

            if (modifier.Length > 0)
            {
                labelPlayer.Text += $" ({modifier})";
            }
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

        private void drawingsurface_KeyDown(object sender, KeyEventArgs e)
        {
            if (_core.State.ActiveThreadCount > 0)
            {
                return;
            }

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
                    UpdatePlayerStatLabels(_core);
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

        private void toolStripButtonCharacter_Click(object sender, EventArgs e)
        {
            using (var form = new FormCharacter(_core))
            {
                form.ShowDialog();
            }
        }
    }
}
