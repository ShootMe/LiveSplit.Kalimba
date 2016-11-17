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
			this.btnNewGame = new System.Windows.Forms.Button();
			this.btnAllTotems = new System.Windows.Forms.Button();
			this.btnPreviousCheckpoint = new System.Windows.Forms.Button();
			this.btnNextCheckpoint = new System.Windows.Forms.Button();
			this.lblCurrentCheckpoint = new System.Windows.Forms.Label();
			this.lblP1Pos = new System.Windows.Forms.Label();
			this.lblLevel = new System.Windows.Forms.Label();
			this.lblP2Pos = new System.Windows.Forms.Label();
			this.chkLockCheckpoint = new System.Windows.Forms.CheckBox();
			this.chkPickups = new System.Windows.Forms.CheckBox();
			this.musicVolume = new System.Windows.Forms.TrackBar();
			this.lblMusic = new System.Windows.Forms.Label();
			this.zoomValue = new System.Windows.Forms.TrackBar();
			this.chkCameraLead = new System.Windows.Forms.CheckBox();
			this.chkCameraTrail = new System.Windows.Forms.CheckBox();
			this.chkLockZoom = new System.Windows.Forms.CheckBox();
			this.lblCamera = new System.Windows.Forms.Label();
			this.chkInvincible = new System.Windows.Forms.CheckBox();
			this.btnKill = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.musicVolume)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomValue)).BeginInit();
			this.SuspendLayout();
			// 
			// btnNewGame
			// 
			this.btnNewGame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnNewGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnNewGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnNewGame.ForeColor = System.Drawing.Color.Black;
			this.btnNewGame.Location = new System.Drawing.Point(12, 12);
			this.btnNewGame.Name = "btnNewGame";
			this.btnNewGame.Size = new System.Drawing.Size(113, 29);
			this.btnNewGame.TabIndex = 0;
			this.btnNewGame.Text = "New Game";
			this.btnNewGame.UseVisualStyleBackColor = false;
			this.btnNewGame.Click += new System.EventHandler(this.btnNewGame_Click);
			// 
			// btnAllTotems
			// 
			this.btnAllTotems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnAllTotems.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAllTotems.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnAllTotems.ForeColor = System.Drawing.Color.Black;
			this.btnAllTotems.Location = new System.Drawing.Point(131, 12);
			this.btnAllTotems.Name = "btnAllTotems";
			this.btnAllTotems.Size = new System.Drawing.Size(113, 29);
			this.btnAllTotems.TabIndex = 1;
			this.btnAllTotems.Text = "All Totems";
			this.btnAllTotems.UseVisualStyleBackColor = false;
			this.btnAllTotems.Click += new System.EventHandler(this.btnAllTotems_Click);
			// 
			// btnPreviousCheckpoint
			// 
			this.btnPreviousCheckpoint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnPreviousCheckpoint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnPreviousCheckpoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnPreviousCheckpoint.ForeColor = System.Drawing.Color.Black;
			this.btnPreviousCheckpoint.Location = new System.Drawing.Point(12, 40);
			this.btnPreviousCheckpoint.Name = "btnPreviousCheckpoint";
			this.btnPreviousCheckpoint.Size = new System.Drawing.Size(113, 29);
			this.btnPreviousCheckpoint.TabIndex = 2;
			this.btnPreviousCheckpoint.Text = "< Checkpoint";
			this.btnPreviousCheckpoint.UseVisualStyleBackColor = false;
			this.btnPreviousCheckpoint.Click += new System.EventHandler(this.btnPreviousCheckpoint_Click);
			// 
			// btnNextCheckpoint
			// 
			this.btnNextCheckpoint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnNextCheckpoint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnNextCheckpoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnNextCheckpoint.ForeColor = System.Drawing.Color.Black;
			this.btnNextCheckpoint.Location = new System.Drawing.Point(131, 40);
			this.btnNextCheckpoint.Name = "btnNextCheckpoint";
			this.btnNextCheckpoint.Size = new System.Drawing.Size(113, 29);
			this.btnNextCheckpoint.TabIndex = 3;
			this.btnNextCheckpoint.Text = "Checkpoint >";
			this.btnNextCheckpoint.UseVisualStyleBackColor = false;
			this.btnNextCheckpoint.Click += new System.EventHandler(this.btnNextCheckpoint_Click);
			// 
			// lblCurrentCheckpoint
			// 
			this.lblCurrentCheckpoint.AutoSize = true;
			this.lblCurrentCheckpoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCurrentCheckpoint.Location = new System.Drawing.Point(7, 92);
			this.lblCurrentCheckpoint.Name = "lblCurrentCheckpoint";
			this.lblCurrentCheckpoint.Size = new System.Drawing.Size(128, 20);
			this.lblCurrentCheckpoint.TabIndex = 5;
			this.lblCurrentCheckpoint.Text = "Checkpoint: 1/12";
			// 
			// lblP1Pos
			// 
			this.lblP1Pos.AutoSize = true;
			this.lblP1Pos.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblP1Pos.Location = new System.Drawing.Point(7, 112);
			this.lblP1Pos.Name = "lblP1Pos";
			this.lblP1Pos.Size = new System.Drawing.Size(151, 20);
			this.lblP1Pos.TabIndex = 7;
			this.lblP1Pos.Text = "T1: (100.00, 100.00)";
			// 
			// lblLevel
			// 
			this.lblLevel.AutoSize = true;
			this.lblLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblLevel.Location = new System.Drawing.Point(7, 72);
			this.lblLevel.Name = "lblLevel";
			this.lblLevel.Size = new System.Drawing.Size(54, 20);
			this.lblLevel.TabIndex = 4;
			this.lblLevel.Text = "Level: ";
			// 
			// lblP2Pos
			// 
			this.lblP2Pos.AutoSize = true;
			this.lblP2Pos.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblP2Pos.Location = new System.Drawing.Point(7, 132);
			this.lblP2Pos.Name = "lblP2Pos";
			this.lblP2Pos.Size = new System.Drawing.Size(151, 20);
			this.lblP2Pos.TabIndex = 8;
			this.lblP2Pos.Text = "T2: (100.00, 100.00)";
			// 
			// chkLockCheckpoint
			// 
			this.chkLockCheckpoint.AutoSize = true;
			this.chkLockCheckpoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkLockCheckpoint.Location = new System.Drawing.Point(162, 91);
			this.chkLockCheckpoint.Name = "chkLockCheckpoint";
			this.chkLockCheckpoint.Size = new System.Drawing.Size(62, 24);
			this.chkLockCheckpoint.TabIndex = 6;
			this.chkLockCheckpoint.Text = "Lock";
			this.chkLockCheckpoint.UseVisualStyleBackColor = true;
			this.chkLockCheckpoint.CheckedChanged += new System.EventHandler(this.chkLockCheckpoint_CheckedChanged);
			// 
			// chkPickups
			// 
			this.chkPickups.AutoSize = true;
			this.chkPickups.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkPickups.Location = new System.Drawing.Point(228, 92);
			this.chkPickups.Name = "chkPickups";
			this.chkPickups.Size = new System.Drawing.Size(107, 24);
			this.chkPickups.TabIndex = 9;
			this.chkPickups.Text = "No Pickups";
			this.chkPickups.UseVisualStyleBackColor = true;
			this.chkPickups.CheckedChanged += new System.EventHandler(this.chkPickups_CheckedChanged);
			// 
			// musicVolume
			// 
			this.musicVolume.Location = new System.Drawing.Point(55, 154);
			this.musicVolume.Maximum = 20;
			this.musicVolume.Name = "musicVolume";
			this.musicVolume.Size = new System.Drawing.Size(94, 45);
			this.musicVolume.TabIndex = 11;
			this.musicVolume.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.musicVolume.Value = 20;
			// 
			// lblMusic
			// 
			this.lblMusic.AutoSize = true;
			this.lblMusic.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblMusic.Location = new System.Drawing.Point(7, 159);
			this.lblMusic.Name = "lblMusic";
			this.lblMusic.Size = new System.Drawing.Size(54, 20);
			this.lblMusic.TabIndex = 10;
			this.lblMusic.Text = "Music:";
			// 
			// zoomValue
			// 
			this.zoomValue.Location = new System.Drawing.Point(226, 154);
			this.zoomValue.Maximum = 150;
			this.zoomValue.Name = "zoomValue";
			this.zoomValue.Size = new System.Drawing.Size(130, 45);
			this.zoomValue.TabIndex = 13;
			this.zoomValue.TickFrequency = 5;
			this.zoomValue.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.zoomValue.Value = 50;
			// 
			// chkCameraLead
			// 
			this.chkCameraLead.AutoSize = true;
			this.chkCameraLead.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkCameraLead.Location = new System.Drawing.Point(228, 112);
			this.chkCameraLead.Name = "chkCameraLead";
			this.chkCameraLead.Size = new System.Drawing.Size(64, 24);
			this.chkCameraLead.TabIndex = 14;
			this.chkCameraLead.Text = "Lead";
			this.chkCameraLead.UseVisualStyleBackColor = true;
			this.chkCameraLead.CheckedChanged += new System.EventHandler(this.chkCameraLead_CheckedChanged);
			// 
			// chkCameraTrail
			// 
			this.chkCameraTrail.AutoSize = true;
			this.chkCameraTrail.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkCameraTrail.Location = new System.Drawing.Point(290, 112);
			this.chkCameraTrail.Name = "chkCameraTrail";
			this.chkCameraTrail.Size = new System.Drawing.Size(57, 24);
			this.chkCameraTrail.TabIndex = 15;
			this.chkCameraTrail.Text = "Trail";
			this.chkCameraTrail.UseVisualStyleBackColor = true;
			this.chkCameraTrail.CheckedChanged += new System.EventHandler(this.chkCameraTrail_CheckedChanged);
			// 
			// chkLockZoom
			// 
			this.chkLockZoom.AutoSize = true;
			this.chkLockZoom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkLockZoom.Location = new System.Drawing.Point(162, 158);
			this.chkLockZoom.Name = "chkLockZoom";
			this.chkLockZoom.Size = new System.Drawing.Size(69, 24);
			this.chkLockZoom.TabIndex = 16;
			this.chkLockZoom.Text = "Zoom";
			this.chkLockZoom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkLockZoom.UseVisualStyleBackColor = true;
			this.chkLockZoom.CheckedChanged += new System.EventHandler(this.chkLockZoom_CheckedChanged);
			// 
			// lblCamera
			// 
			this.lblCamera.AutoSize = true;
			this.lblCamera.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCamera.Location = new System.Drawing.Point(158, 112);
			this.lblCamera.Name = "lblCamera";
			this.lblCamera.Size = new System.Drawing.Size(65, 20);
			this.lblCamera.TabIndex = 17;
			this.lblCamera.Text = "Camera";
			// 
			// chkInvincible
			// 
			this.chkInvincible.AutoSize = true;
			this.chkInvincible.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkInvincible.Location = new System.Drawing.Point(162, 132);
			this.chkInvincible.Name = "chkInvincible";
			this.chkInvincible.Size = new System.Drawing.Size(153, 24);
			this.chkInvincible.TabIndex = 18;
			this.chkInvincible.Text = "Invincible to Ooze";
			this.chkInvincible.UseVisualStyleBackColor = true;
			this.chkInvincible.CheckedChanged += new System.EventHandler(this.chkInvincible_CheckedChanged);
			// 
			// btnKill
			// 
			this.btnKill.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnKill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnKill.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnKill.ForeColor = System.Drawing.Color.Black;
			this.btnKill.Location = new System.Drawing.Point(250, 40);
			this.btnKill.Name = "btnKill";
			this.btnKill.Size = new System.Drawing.Size(103, 29);
			this.btnKill.TabIndex = 19;
			this.btnKill.Text = "Kill Totems";
			this.btnKill.UseVisualStyleBackColor = false;
			this.btnKill.Click += new System.EventHandler(this.btnKill_Click);
			// 
			// KalimbaManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(365, 190);
			this.Controls.Add(this.btnKill);
			this.Controls.Add(this.chkInvincible);
			this.Controls.Add(this.lblCamera);
			this.Controls.Add(this.chkLockZoom);
			this.Controls.Add(this.chkCameraTrail);
			this.Controls.Add(this.chkCameraLead);
			this.Controls.Add(this.zoomValue);
			this.Controls.Add(this.musicVolume);
			this.Controls.Add(this.chkPickups);
			this.Controls.Add(this.chkLockCheckpoint);
			this.Controls.Add(this.lblLevel);
			this.Controls.Add(this.lblP2Pos);
			this.Controls.Add(this.lblP1Pos);
			this.Controls.Add(this.lblCurrentCheckpoint);
			this.Controls.Add(this.btnNextCheckpoint);
			this.Controls.Add(this.btnPreviousCheckpoint);
			this.Controls.Add(this.btnAllTotems);
			this.Controls.Add(this.btnNewGame);
			this.Controls.Add(this.lblMusic);
			this.ForeColor = System.Drawing.Color.Black;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "KalimbaManager";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Kalimba Manager 1.8.4";
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KalimbaManager_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.musicVolume)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomValue)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnNewGame;
		private System.Windows.Forms.Button btnAllTotems;
		private System.Windows.Forms.Button btnPreviousCheckpoint;
		private System.Windows.Forms.Button btnNextCheckpoint;
		private System.Windows.Forms.Label lblCurrentCheckpoint;
		private System.Windows.Forms.Label lblP1Pos;
		private System.Windows.Forms.Label lblLevel;
		private System.Windows.Forms.Label lblP2Pos;
		private System.Windows.Forms.CheckBox chkLockCheckpoint;
		private System.Windows.Forms.CheckBox chkPickups;
		private System.Windows.Forms.TrackBar musicVolume;
		private System.Windows.Forms.Label lblMusic;
		private System.Windows.Forms.TrackBar zoomValue;
		private System.Windows.Forms.CheckBox chkCameraLead;
		private System.Windows.Forms.CheckBox chkCameraTrail;
		private System.Windows.Forms.CheckBox chkLockZoom;
		private System.Windows.Forms.Label lblCamera;
		private System.Windows.Forms.CheckBox chkInvincible;
		private System.Windows.Forms.Button btnKill;
	}
}