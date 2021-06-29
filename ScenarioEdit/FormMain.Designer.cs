
namespace ScenarioEdit
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.splitContainerBody = new System.Windows.Forms.SplitContainer();
            this.splitContainerTools1 = new System.Windows.Forms.SplitContainer();
            this.treeViewTiles = new System.Windows.Forms.TreeView();
            this.groupBoxProperties = new System.Windows.Forms.GroupBox();
            this.listViewProperties = new System.Windows.Forms.ListView();
            this.columnHeaderPropertiesName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderPropertiesValue = new System.Windows.Forms.ColumnHeader();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelMouseXY = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelHoverObject = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelDebug = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.levelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemChangeLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDeleteLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSetDefaultLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemTools = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemResetAllTileMeta = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemViewWorldItems = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelMode = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButtonInsertMode = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSelectMode = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShapeMode = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonMoveTileUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMoveTileDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonPlayMap = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBody)).BeginInit();
            this.splitContainerBody.Panel2.SuspendLayout();
            this.splitContainerBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTools1)).BeginInit();
            this.splitContainerTools1.Panel1.SuspendLayout();
            this.splitContainerTools1.Panel2.SuspendLayout();
            this.splitContainerTools1.SuspendLayout();
            this.groupBoxProperties.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerBody
            // 
            this.splitContainerBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBody.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerBody.IsSplitterFixed = true;
            this.splitContainerBody.Location = new System.Drawing.Point(0, 65);
            this.splitContainerBody.Name = "splitContainerBody";
            // 
            // splitContainerBody.Panel2
            // 
            this.splitContainerBody.Panel2.Controls.Add(this.splitContainerTools1);
            this.splitContainerBody.Size = new System.Drawing.Size(784, 474);
            this.splitContainerBody.SplitterDistance = 500;
            this.splitContainerBody.TabIndex = 0;
            // 
            // splitContainerTools1
            // 
            this.splitContainerTools1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerTools1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerTools1.Name = "splitContainerTools1";
            this.splitContainerTools1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerTools1.Panel1
            // 
            this.splitContainerTools1.Panel1.Controls.Add(this.treeViewTiles);
            // 
            // splitContainerTools1.Panel2
            // 
            this.splitContainerTools1.Panel2.Controls.Add(this.groupBoxProperties);
            this.splitContainerTools1.Size = new System.Drawing.Size(280, 474);
            this.splitContainerTools1.SplitterDistance = 236;
            this.splitContainerTools1.TabIndex = 1;
            // 
            // treeViewTiles
            // 
            this.treeViewTiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewTiles.Location = new System.Drawing.Point(0, 0);
            this.treeViewTiles.Name = "treeViewTiles";
            this.treeViewTiles.Size = new System.Drawing.Size(280, 236);
            this.treeViewTiles.TabIndex = 0;
            // 
            // groupBoxProperties
            // 
            this.groupBoxProperties.Controls.Add(this.listViewProperties);
            this.groupBoxProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxProperties.Location = new System.Drawing.Point(0, 0);
            this.groupBoxProperties.Name = "groupBoxProperties";
            this.groupBoxProperties.Size = new System.Drawing.Size(280, 234);
            this.groupBoxProperties.TabIndex = 1;
            this.groupBoxProperties.TabStop = false;
            this.groupBoxProperties.Text = "Properties";
            // 
            // listViewProperties
            // 
            this.listViewProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderPropertiesName,
            this.columnHeaderPropertiesValue});
            this.listViewProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewProperties.FullRowSelect = true;
            this.listViewProperties.GridLines = true;
            this.listViewProperties.HideSelection = false;
            this.listViewProperties.LabelEdit = true;
            this.listViewProperties.Location = new System.Drawing.Point(3, 19);
            this.listViewProperties.MultiSelect = false;
            this.listViewProperties.Name = "listViewProperties";
            this.listViewProperties.Size = new System.Drawing.Size(274, 212);
            this.listViewProperties.TabIndex = 0;
            this.listViewProperties.UseCompatibleStateImageBehavior = false;
            this.listViewProperties.View = System.Windows.Forms.View.Details;
            this.listViewProperties.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewProperties_MouseDoubleClick);
            // 
            // columnHeaderPropertiesName
            // 
            this.columnHeaderPropertiesName.Text = "Name";
            this.columnHeaderPropertiesName.Width = 100;
            // 
            // columnHeaderPropertiesValue
            // 
            this.columnHeaderPropertiesValue.Text = "Value";
            this.columnHeaderPropertiesValue.Width = 250;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelMouseXY,
            this.toolStripStatusLabelHoverObject,
            this.toolStripStatusLabelDebug});
            this.statusStrip.Location = new System.Drawing.Point(0, 539);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(784, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            // 
            // toolStripStatusLabelMouseXY
            // 
            this.toolStripStatusLabelMouseXY.Name = "toolStripStatusLabelMouseXY";
            this.toolStripStatusLabelMouseXY.Size = new System.Drawing.Size(34, 17);
            this.toolStripStatusLabelMouseXY.Text = "0x,0y";
            // 
            // toolStripStatusLabelHoverObject
            // 
            this.toolStripStatusLabelHoverObject.Name = "toolStripStatusLabelHoverObject";
            this.toolStripStatusLabelHoverObject.Size = new System.Drawing.Size(50, 17);
            this.toolStripStatusLabelHoverObject.Text = "<none>";
            // 
            // toolStripStatusLabelDebug
            // 
            this.toolStripStatusLabelDebug.Name = "toolStripStatusLabelDebug";
            this.toolStripStatusLabelDebug.Size = new System.Drawing.Size(57, 17);
            this.toolStripStatusLabelDebug.Text = "<debug>";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.levelToolStripMenuItem,
            this.toolStripMenuItemTools,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(784, 24);
            this.menuStrip.TabIndex = 2;
            this.menuStrip.Text = "menuStrip";
            this.menuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
            this.newToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.newToolStripMenuItem.Text = "&New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "&Open";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(143, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator3,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator4,
            this.selectAllToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.redoToolStripMenuItem.Text = "&Redo";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(141, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
            this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
            this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(141, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            // 
            // levelToolStripMenuItem
            // 
            this.levelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddLevel,
            this.toolStripMenuItemChangeLevel,
            this.toolStripMenuItemDeleteLevel,
            this.toolStripMenuItemSetDefaultLevel});
            this.levelToolStripMenuItem.Name = "levelToolStripMenuItem";
            this.levelToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.levelToolStripMenuItem.Text = "Level";
            // 
            // toolStripMenuItemAddLevel
            // 
            this.toolStripMenuItemAddLevel.Name = "toolStripMenuItemAddLevel";
            this.toolStripMenuItemAddLevel.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItemAddLevel.Text = "Add Level";
            // 
            // toolStripMenuItemChangeLevel
            // 
            this.toolStripMenuItemChangeLevel.Name = "toolStripMenuItemChangeLevel";
            this.toolStripMenuItemChangeLevel.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItemChangeLevel.Text = "Select Level";
            // 
            // toolStripMenuItemDeleteLevel
            // 
            this.toolStripMenuItemDeleteLevel.Name = "toolStripMenuItemDeleteLevel";
            this.toolStripMenuItemDeleteLevel.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItemDeleteLevel.Text = "Delete Level";
            // 
            // toolStripMenuItemSetDefaultLevel
            // 
            this.toolStripMenuItemSetDefaultLevel.Name = "toolStripMenuItemSetDefaultLevel";
            this.toolStripMenuItemSetDefaultLevel.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItemSetDefaultLevel.Text = "Set Default Level";
            // 
            // toolStripMenuItemTools
            // 
            this.toolStripMenuItemTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemResetAllTileMeta,
            this.toolStripMenuItemViewWorldItems});
            this.toolStripMenuItemTools.Name = "toolStripMenuItemTools";
            this.toolStripMenuItemTools.Size = new System.Drawing.Size(46, 20);
            this.toolStripMenuItemTools.Text = "Tools";
            // 
            // toolStripMenuItemResetAllTileMeta
            // 
            this.toolStripMenuItemResetAllTileMeta.Name = "toolStripMenuItemResetAllTileMeta";
            this.toolStripMenuItemResetAllTileMeta.Size = new System.Drawing.Size(175, 22);
            this.toolStripMenuItemResetAllTileMeta.Text = "Reset All Tile Metas";
            // 
            // toolStripMenuItemViewWorldItems
            // 
            this.toolStripMenuItemViewWorldItems.Name = "toolStripMenuItemViewWorldItems";
            this.toolStripMenuItemViewWorldItems.Size = new System.Drawing.Size(175, 22);
            this.toolStripMenuItemViewWorldItems.Text = "View World Items";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNew,
            this.toolStripButtonSave,
            this.toolStripButtonOpen,
            this.toolStripButtonClose,
            this.toolStripSeparator2,
            this.toolStripLabelMode,
            this.toolStripButtonInsertMode,
            this.toolStripButtonSelectMode,
            this.toolStripButtonShapeMode,
            this.toolStripSeparator5,
            this.toolStripButtonMoveTileUp,
            this.toolStripButtonMoveTileDown,
            this.toolStripSeparator6,
            this.toolStripButtonPlayMap});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(784, 41);
            this.toolStrip.TabIndex = 3;
            // 
            // toolStripButtonNew
            // 
            this.toolStripButtonNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNew.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonNew.Image")));
            this.toolStripButtonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNew.Name = "toolStripButtonNew";
            this.toolStripButtonNew.Size = new System.Drawing.Size(23, 38);
            this.toolStripButtonNew.Text = "New";
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSave.Image")));
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(23, 38);
            this.toolStripButtonSave.Text = "Save";
            // 
            // toolStripButtonOpen
            // 
            this.toolStripButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpen.Image")));
            this.toolStripButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpen.Name = "toolStripButtonOpen";
            this.toolStripButtonOpen.Size = new System.Drawing.Size(23, 38);
            this.toolStripButtonOpen.Text = "Open";
            // 
            // toolStripButtonClose
            // 
            this.toolStripButtonClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonClose.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClose.Image")));
            this.toolStripButtonClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClose.Name = "toolStripButtonClose";
            this.toolStripButtonClose.Size = new System.Drawing.Size(23, 38);
            this.toolStripButtonClose.Text = "Close";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 41);
            // 
            // toolStripLabelMode
            // 
            this.toolStripLabelMode.Name = "toolStripLabelMode";
            this.toolStripLabelMode.Size = new System.Drawing.Size(15, 38);
            this.toolStripLabelMode.Text = "Mode";
            this.toolStripLabelMode.TextDirection = System.Windows.Forms.ToolStripTextDirection.Vertical270;
            // 
            // toolStripButtonInsertMode
            // 
            this.toolStripButtonInsertMode.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonInsertMode.Image")));
            this.toolStripButtonInsertMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonInsertMode.Name = "toolStripButtonInsertMode";
            this.toolStripButtonInsertMode.Size = new System.Drawing.Size(71, 38);
            this.toolStripButtonInsertMode.Text = "Add/Delete";
            this.toolStripButtonInsertMode.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButtonSelectMode
            // 
            this.toolStripButtonSelectMode.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSelectMode.Image")));
            this.toolStripButtonSelectMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSelectMode.Name = "toolStripButtonSelectMode";
            this.toolStripButtonSelectMode.Size = new System.Drawing.Size(42, 38);
            this.toolStripButtonSelectMode.Text = "Select";
            this.toolStripButtonSelectMode.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButtonShapeMode
            // 
            this.toolStripButtonShapeMode.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonShapeMode.Image")));
            this.toolStripButtonShapeMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShapeMode.Name = "toolStripButtonShapeMode";
            this.toolStripButtonShapeMode.Size = new System.Drawing.Size(43, 38);
            this.toolStripButtonShapeMode.Text = "Shape";
            this.toolStripButtonShapeMode.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonShapeMode.ToolTipText = "Shape";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 41);
            // 
            // toolStripButtonMoveTileUp
            // 
            this.toolStripButtonMoveTileUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMoveTileUp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMoveTileUp.Image")));
            this.toolStripButtonMoveTileUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMoveTileUp.Name = "toolStripButtonMoveTileUp";
            this.toolStripButtonMoveTileUp.Size = new System.Drawing.Size(23, 38);
            this.toolStripButtonMoveTileUp.Text = "Move Tile Up";
            // 
            // toolStripButtonMoveTileDown
            // 
            this.toolStripButtonMoveTileDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMoveTileDown.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMoveTileDown.Image")));
            this.toolStripButtonMoveTileDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMoveTileDown.Name = "toolStripButtonMoveTileDown";
            this.toolStripButtonMoveTileDown.Size = new System.Drawing.Size(23, 38);
            this.toolStripButtonMoveTileDown.Text = "Move Tile Down";
            this.toolStripButtonMoveTileDown.ToolTipText = "Move Tile Down";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 41);
            // 
            // toolStripButtonPlayMap
            // 
            this.toolStripButtonPlayMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPlayMap.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPlayMap.Image")));
            this.toolStripButtonPlayMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPlayMap.Name = "toolStripButtonPlayMap";
            this.toolStripButtonPlayMap.Size = new System.Drawing.Size(23, 38);
            this.toolStripButtonPlayMap.Text = "Play Map";
            this.toolStripButtonPlayMap.ToolTipText = "Play Map";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.splitContainerBody);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RougueQuest : ScenarioEdit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.SizeChanged += new System.EventHandler(this.FormMain_SizeChanged);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormMain_KeyPress);
            this.splitContainerBody.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBody)).EndInit();
            this.splitContainerBody.ResumeLayout(false);
            this.splitContainerTools1.Panel1.ResumeLayout(false);
            this.splitContainerTools1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTools1)).EndInit();
            this.splitContainerTools1.ResumeLayout(false);
            this.groupBoxProperties.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerBody;
        private System.Windows.Forms.SplitContainer splitContainerTools1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMouseXY;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelHoverObject;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonInsertMode;
        private System.Windows.Forms.ToolStripButton toolStripButtonSelectMode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDebug;
        private System.Windows.Forms.TreeView treeViewTiles;
        private System.Windows.Forms.GroupBox groupBoxProperties;
        private System.Windows.Forms.ListView listViewProperties;
        private System.Windows.Forms.ToolStripLabel toolStripLabelMode;
        private System.Windows.Forms.ColumnHeader columnHeaderPropertiesName;
        private System.Windows.Forms.ColumnHeader columnHeaderPropertiesValue;
        private System.Windows.Forms.ToolStripButton toolStripButtonNew;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpen;
        private System.Windows.Forms.ToolStripButton toolStripButtonClose;
        private System.Windows.Forms.ToolStripButton toolStripButtonMoveTileUp;
        private System.Windows.Forms.ToolStripButton toolStripButtonMoveTileDown;
        private System.Windows.Forms.ToolStripButton toolStripButtonShapeMode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton toolStripButtonPlayMap;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTools;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemResetAllTileMeta;
        private System.Windows.Forms.ToolStripMenuItem levelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddLevel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemChangeLevel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDeleteLevel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetDefaultLevel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemViewWorldItems;
    }
}

