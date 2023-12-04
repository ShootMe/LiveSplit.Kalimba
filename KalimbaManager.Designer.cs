namespace LiveSplit.Kalimba {
	partial class KalimbaManager {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KalimbaManager));
            this.lblCurrentCheckpoint = new System.Windows.Forms.Label();
            this.lblP1P2Pos = new System.Windows.Forms.Label();
            this.lblLevel = new System.Windows.Forms.Label();
            this.zoomValue = new System.Windows.Forms.TrackBar();
            this.chkLockZoom = new System.Windows.Forms.CheckBox();
            this.lblNotAvailable = new System.Windows.Forms.Label();
            this.lblTASOutput = new System.Windows.Forms.Label();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.menuWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.itemNewGame = new System.Windows.Forms.ToolStripMenuItem();
            this.itemAllTotems = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCheckpoint = new System.Windows.Forms.ToolStripMenuItem();
            this.itemCheckpointNext = new System.Windows.Forms.ToolStripMenuItem();
            this.itemCheckpointPrevious = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.itemCheckpointLock = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.itemLevelErase = new System.Windows.Forms.ToolStripMenuItem();
            this.itemLevelClear = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.itemNoPickups = new System.Windows.Forms.ToolStripMenuItem();
            this.itemInvincibleToOoze = new System.Windows.Forms.ToolStripMenuItem();
            this.itemKillTotems = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCamera = new System.Windows.Forms.ToolStripMenuItem();
            this.itemCameraLead = new System.Windows.Forms.ToolStripMenuItem();
            this.itemCameraTrail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMusic = new System.Windows.Forms.ToolStripMenuItem();
            this.itemMusicEnable = new System.Windows.Forms.ToolStripMenuItem();
            this.txtMusicVolume = new System.Windows.Forms.ToolStripTextBox();
            this.menuTAS = new System.Windows.Forms.ToolStripMenuItem();
            this.itemTASDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.lblP3P4Pos = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.zoomValue)).BeginInit();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCurrentCheckpoint
            // 
            this.lblCurrentCheckpoint.AutoSize = true;
            this.lblCurrentCheckpoint.Location = new System.Drawing.Point(2, 46);
            this.lblCurrentCheckpoint.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentCheckpoint.Name = "lblCurrentCheckpoint";
            this.lblCurrentCheckpoint.Size = new System.Drawing.Size(135, 16);
            this.lblCurrentCheckpoint.TabIndex = 7;
            this.lblCurrentCheckpoint.Text = "Checkpoint: 1/12";
            // 
            // lblP1P2Pos
            // 
            this.lblP1P2Pos.AutoSize = true;
            this.lblP1P2Pos.Location = new System.Drawing.Point(66, 62);
            this.lblP1P2Pos.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblP1P2Pos.Name = "lblP1P2Pos";
            this.lblP1P2Pos.Size = new System.Drawing.Size(367, 16);
            this.lblP1P2Pos.TabIndex = 8;
            this.lblP1P2Pos.Text = "T1: (1000.00, 1000.00) T2: (1000.00, 1000.00)";
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(42, 30);
            this.lblLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(63, 16);
            this.lblLevel.TabIndex = 6;
            this.lblLevel.Text = "Level: ";
            // 
            // zoomValue
            // 
            this.zoomValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zoomValue.AutoSize = false;
            this.zoomValue.Location = new System.Drawing.Point(100, 96);
            this.zoomValue.Margin = new System.Windows.Forms.Padding(4);
            this.zoomValue.Maximum = 150;
            this.zoomValue.Name = "zoomValue";
            this.zoomValue.Size = new System.Drawing.Size(329, 30);
            this.zoomValue.TabIndex = 19;
            this.zoomValue.TickFrequency = 2;
            this.zoomValue.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.zoomValue.Value = 50;
            // 
            // chkLockZoom
            // 
            this.chkLockZoom.AutoSize = true;
            this.chkLockZoom.Location = new System.Drawing.Point(32, 100);
            this.chkLockZoom.Margin = new System.Windows.Forms.Padding(4);
            this.chkLockZoom.Name = "chkLockZoom";
            this.chkLockZoom.Size = new System.Drawing.Size(66, 20);
            this.chkLockZoom.TabIndex = 18;
            this.chkLockZoom.Text = "Zoom:";
            this.chkLockZoom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkLockZoom.UseVisualStyleBackColor = true;
            this.chkLockZoom.CheckedChanged += new System.EventHandler(this.chkLockZoom_CheckedChanged);
            // 
            // lblNotAvailable
            // 
            this.lblNotAvailable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNotAvailable.Font = new System.Drawing.Font("Courier New", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotAvailable.Location = new System.Drawing.Point(0, 0);
            this.lblNotAvailable.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNotAvailable.Name = "lblNotAvailable";
            this.lblNotAvailable.Size = new System.Drawing.Size(437, 131);
            this.lblNotAvailable.TabIndex = 20;
            this.lblNotAvailable.Text = "Not Available";
            this.lblNotAvailable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNotAvailable.Visible = false;
            // 
            // lblTASOutput
            // 
            this.lblTASOutput.AutoSize = true;
            this.lblTASOutput.Location = new System.Drawing.Point(97, 132);
            this.lblTASOutput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTASOutput.Name = "lblTASOutput";
            this.lblTASOutput.Size = new System.Drawing.Size(295, 32);
            this.lblTASOutput.TabIndex = 22;
            this.lblTASOutput.Text = "P1-L999(999,R,J,S)(998 / 999 | 9999)\nP1-L999(999,R,J,S)(998 / 999 | 9999)";
            this.lblTASOutput.Visible = false;
            // 
            // menu
            // 
            this.menu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.menu.AutoSize = false;
            this.menu.Dock = System.Windows.Forms.DockStyle.None;
            this.menu.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuWorld,
            this.menuCheckpoint,
            this.menuLevel,
            this.menuCamera,
            this.menuMusic,
            this.menuTAS});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menu.Size = new System.Drawing.Size(437, 24);
            this.menu.TabIndex = 24;
            // 
            // menuWorld
            // 
            this.menuWorld.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemNewGame,
            this.itemAllTotems});
            this.menuWorld.Name = "menuWorld";
            this.menuWorld.Size = new System.Drawing.Size(59, 20);
            this.menuWorld.Text = "World";
            this.menuWorld.Click += new System.EventHandler(this.menuWorld_Click);
            // 
            // itemNewGame
            // 
            this.itemNewGame.Image = global::LiveSplit.Kalimba.Properties.Resources.world2New;
            this.itemNewGame.Name = "itemNewGame";
            this.itemNewGame.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.itemNewGame.Size = new System.Drawing.Size(257, 22);
            this.itemNewGame.Text = "New Game";
            this.itemNewGame.Click += new System.EventHandler(this.itemNewGame_Click);
            // 
            // itemAllTotems
            // 
            this.itemAllTotems.Image = global::LiveSplit.Kalimba.Properties.Resources.world2All;
            this.itemAllTotems.Name = "itemAllTotems";
            this.itemAllTotems.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Z)));
            this.itemAllTotems.Size = new System.Drawing.Size(257, 22);
            this.itemAllTotems.Text = "All Totems";
            this.itemAllTotems.Click += new System.EventHandler(this.itemAllTotems_Click);
            // 
            // menuCheckpoint
            // 
            this.menuCheckpoint.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemCheckpointNext,
            this.itemCheckpointPrevious,
            this.toolStripSeparator2,
            this.itemCheckpointLock});
            this.menuCheckpoint.Name = "menuCheckpoint";
            this.menuCheckpoint.Size = new System.Drawing.Size(99, 20);
            this.menuCheckpoint.Text = "Checkpoint";
            this.menuCheckpoint.Click += new System.EventHandler(this.menuCheckpoint_Click);
            // 
            // itemCheckpointNext
            // 
            this.itemCheckpointNext.Image = global::LiveSplit.Kalimba.Properties.Resources.next;
            this.itemCheckpointNext.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.itemCheckpointNext.Name = "itemCheckpointNext";
            this.itemCheckpointNext.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.itemCheckpointNext.Size = new System.Drawing.Size(193, 22);
            this.itemCheckpointNext.Text = "Next";
            this.itemCheckpointNext.Click += new System.EventHandler(this.itemCheckpointNext_Click);
            // 
            // itemCheckpointPrevious
            // 
            this.itemCheckpointPrevious.Image = global::LiveSplit.Kalimba.Properties.Resources.previous;
            this.itemCheckpointPrevious.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.itemCheckpointPrevious.Name = "itemCheckpointPrevious";
            this.itemCheckpointPrevious.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.itemCheckpointPrevious.Size = new System.Drawing.Size(193, 22);
            this.itemCheckpointPrevious.Text = "Previous";
            this.itemCheckpointPrevious.Click += new System.EventHandler(this.itemCheckpointPrevious_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(190, 6);
            // 
            // itemCheckpointLock
            // 
            this.itemCheckpointLock.CheckOnClick = true;
            this.itemCheckpointLock.Name = "itemCheckpointLock";
            this.itemCheckpointLock.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.itemCheckpointLock.Size = new System.Drawing.Size(193, 22);
            this.itemCheckpointLock.Text = "Lock";
            this.itemCheckpointLock.Click += new System.EventHandler(this.itemCheckpointLock_Click);
            // 
            // menuLevel
            // 
            this.menuLevel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemLevelErase,
            this.itemLevelClear,
            this.toolStripSeparator1,
            this.itemNoPickups,
            this.itemInvincibleToOoze,
            this.itemKillTotems});
            this.menuLevel.Name = "menuLevel";
            this.menuLevel.Size = new System.Drawing.Size(59, 20);
            this.menuLevel.Text = "Level";
            this.menuLevel.Click += new System.EventHandler(this.menuLevel_Click);
            // 
            // itemLevelErase
            // 
            this.itemLevelErase.Image = global::LiveSplit.Kalimba.Properties.Resources.erase;
            this.itemLevelErase.Name = "itemLevelErase";
            this.itemLevelErase.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.itemLevelErase.Size = new System.Drawing.Size(273, 22);
            this.itemLevelErase.Text = "Erase";
            this.itemLevelErase.Click += new System.EventHandler(this.itemLevelErase_Click);
            // 
            // itemLevelClear
            // 
            this.itemLevelClear.Image = global::LiveSplit.Kalimba.Properties.Resources.reset;
            this.itemLevelClear.Name = "itemLevelClear";
            this.itemLevelClear.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.itemLevelClear.Size = new System.Drawing.Size(273, 22);
            this.itemLevelClear.Text = "Clear";
            this.itemLevelClear.Click += new System.EventHandler(this.itemLevelClear_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(270, 6);
            // 
            // itemNoPickups
            // 
            this.itemNoPickups.CheckOnClick = true;
            this.itemNoPickups.Name = "itemNoPickups";
            this.itemNoPickups.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.P)));
            this.itemNoPickups.Size = new System.Drawing.Size(273, 22);
            this.itemNoPickups.Text = "No Pickups";
            this.itemNoPickups.Click += new System.EventHandler(this.itemNoPickups_Click);
            // 
            // itemInvincibleToOoze
            // 
            this.itemInvincibleToOoze.CheckOnClick = true;
            this.itemInvincibleToOoze.Name = "itemInvincibleToOoze";
            this.itemInvincibleToOoze.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.itemInvincibleToOoze.Size = new System.Drawing.Size(273, 22);
            this.itemInvincibleToOoze.Text = "Invincible to Ooze";
            this.itemInvincibleToOoze.Click += new System.EventHandler(this.itemInvincibleToOoze_Click);
            // 
            // itemKillTotems
            // 
            this.itemKillTotems.Image = global::LiveSplit.Kalimba.Properties.Resources.kill;
            this.itemKillTotems.Name = "itemKillTotems";
            this.itemKillTotems.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.itemKillTotems.Size = new System.Drawing.Size(273, 22);
            this.itemKillTotems.Text = "Kill Totems";
            this.itemKillTotems.Click += new System.EventHandler(this.itemKillTotems_Click);
            // 
            // menuCamera
            // 
            this.menuCamera.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemCameraLead,
            this.itemCameraTrail});
            this.menuCamera.Name = "menuCamera";
            this.menuCamera.Size = new System.Drawing.Size(67, 20);
            this.menuCamera.Text = "Camera";
            this.menuCamera.Click += new System.EventHandler(this.menuCamera_Click);
            // 
            // itemCameraLead
            // 
            this.itemCameraLead.CheckOnClick = true;
            this.itemCameraLead.Name = "itemCameraLead";
            this.itemCameraLead.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.itemCameraLead.Size = new System.Drawing.Size(169, 22);
            this.itemCameraLead.Text = "Lead";
            this.itemCameraLead.Click += new System.EventHandler(this.itemCameraLead_Click);
            // 
            // itemCameraTrail
            // 
            this.itemCameraTrail.CheckOnClick = true;
            this.itemCameraTrail.Name = "itemCameraTrail";
            this.itemCameraTrail.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.itemCameraTrail.Size = new System.Drawing.Size(169, 22);
            this.itemCameraTrail.Text = "Trail";
            this.itemCameraTrail.Click += new System.EventHandler(this.itemCameraTrail_Click);
            // 
            // menuMusic
            // 
            this.menuMusic.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemMusicEnable,
            this.txtMusicVolume});
            this.menuMusic.Name = "menuMusic";
            this.menuMusic.Size = new System.Drawing.Size(59, 20);
            this.menuMusic.Text = "Music";
            this.menuMusic.Click += new System.EventHandler(this.menuMusic_Click);
            // 
            // itemMusicEnable
            // 
            this.itemMusicEnable.CheckOnClick = true;
            this.itemMusicEnable.Name = "itemMusicEnable";
            this.itemMusicEnable.Size = new System.Drawing.Size(160, 22);
            this.itemMusicEnable.Text = "Enable";
            this.itemMusicEnable.Click += new System.EventHandler(this.menuMusic_Click);
            // 
            // txtMusicVolume
            // 
            this.txtMusicVolume.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMusicVolume.MaxLength = 3;
            this.txtMusicVolume.Name = "txtMusicVolume";
            this.txtMusicVolume.Size = new System.Drawing.Size(100, 23);
            this.txtMusicVolume.Text = "100";
            // 
            // menuTAS
            // 
            this.menuTAS.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemTASDisplay});
            this.menuTAS.Name = "menuTAS";
            this.menuTAS.Size = new System.Drawing.Size(43, 20);
            this.menuTAS.Text = "TAS";
            // 
            // itemTASDisplay
            // 
            this.itemTASDisplay.CheckOnClick = true;
            this.itemTASDisplay.Name = "itemTASDisplay";
            this.itemTASDisplay.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.itemTASDisplay.Size = new System.Drawing.Size(225, 22);
            this.itemTASDisplay.Text = "Display Info";
            this.itemTASDisplay.Click += new System.EventHandler(this.itemTASDisplay_Click);
            // 
            // lblP3P4Pos
            // 
            this.lblP3P4Pos.AutoSize = true;
            this.lblP3P4Pos.Location = new System.Drawing.Point(66, 78);
            this.lblP3P4Pos.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblP3P4Pos.Name = "lblP3P4Pos";
            this.lblP3P4Pos.Size = new System.Drawing.Size(367, 16);
            this.lblP3P4Pos.TabIndex = 25;
            this.lblP3P4Pos.Text = "T3: (1000.00, 1000.00) T4: (1000.00, 1000.00)";
            // 
            // KalimbaManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(437, 131);
            this.Controls.Add(this.lblNotAvailable);
            this.Controls.Add(this.lblP3P4Pos);
            this.Controls.Add(this.lblTASOutput);
            this.Controls.Add(this.chkLockZoom);
            this.Controls.Add(this.zoomValue);
            this.Controls.Add(this.lblLevel);
            this.Controls.Add(this.lblP1P2Pos);
            this.Controls.Add(this.lblCurrentCheckpoint);
            this.Controls.Add(this.menu);
            this.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menu;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1500, 209);
            this.MinimumSize = new System.Drawing.Size(425, 170);
            this.Name = "KalimbaManager";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Kalimba Manager";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KalimbaManager_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.zoomValue)).EndInit();
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label lblCurrentCheckpoint;
		private System.Windows.Forms.Label lblP1P2Pos;
		private System.Windows.Forms.Label lblLevel;
		private System.Windows.Forms.TrackBar zoomValue;
		private System.Windows.Forms.CheckBox chkLockZoom;
		private System.Windows.Forms.Label lblNotAvailable;
		private System.Windows.Forms.Label lblTASOutput;
		private System.Windows.Forms.MenuStrip menu;
		private System.Windows.Forms.ToolStripMenuItem menuWorld;
		private System.Windows.Forms.ToolStripMenuItem itemNewGame;
		private System.Windows.Forms.ToolStripMenuItem itemAllTotems;
		private System.Windows.Forms.ToolStripMenuItem menuCheckpoint;
		private System.Windows.Forms.ToolStripMenuItem itemCheckpointNext;
		private System.Windows.Forms.ToolStripMenuItem itemCheckpointPrevious;
		private System.Windows.Forms.ToolStripMenuItem menuLevel;
		private System.Windows.Forms.ToolStripMenuItem itemLevelErase;
		private System.Windows.Forms.ToolStripMenuItem itemLevelClear;
		private System.Windows.Forms.ToolStripMenuItem itemCheckpointLock;
		private System.Windows.Forms.ToolStripMenuItem menuCamera;
		private System.Windows.Forms.ToolStripMenuItem itemCameraLead;
		private System.Windows.Forms.ToolStripMenuItem itemCameraTrail;
		private System.Windows.Forms.ToolStripMenuItem itemNoPickups;
		private System.Windows.Forms.ToolStripMenuItem itemInvincibleToOoze;
		private System.Windows.Forms.Label lblP3P4Pos;
		private System.Windows.Forms.ToolStripMenuItem itemKillTotems;
		private System.Windows.Forms.ToolStripMenuItem menuMusic;
		private System.Windows.Forms.ToolStripTextBox txtMusicVolume;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem menuTAS;
		private System.Windows.Forms.ToolStripMenuItem itemTASDisplay;
		private System.Windows.Forms.ToolStripMenuItem itemMusicEnable;
	}
}