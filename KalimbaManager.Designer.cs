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
			((System.ComponentModel.ISupportInitialize)(this.musicVolume)).BeginInit();
			this.SuspendLayout();
			// 
			// btnNewGame
			// 
			this.btnNewGame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnNewGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnNewGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnNewGame.ForeColor = System.Drawing.Color.Black;
			this.btnNewGame.Location = new System.Drawing.Point(33, 12);
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
			this.btnAllTotems.Location = new System.Drawing.Point(152, 12);
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
			this.btnPreviousCheckpoint.Location = new System.Drawing.Point(33, 40);
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
			this.btnNextCheckpoint.Location = new System.Drawing.Point(152, 40);
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
			this.chkPickups.Location = new System.Drawing.Point(162, 112);
			this.chkPickups.Name = "chkPickups";
			this.chkPickups.Size = new System.Drawing.Size(107, 24);
			this.chkPickups.TabIndex = 9;
			this.chkPickups.Text = "No Pickups";
			this.chkPickups.UseVisualStyleBackColor = true;
			this.chkPickups.CheckedChanged += new System.EventHandler(this.chkPickups_CheckedChanged);
			// 
			// musicVolume
			// 
			this.musicVolume.Location = new System.Drawing.Point(206, 134);
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
			this.lblMusic.Location = new System.Drawing.Point(158, 139);
			this.lblMusic.Name = "lblMusic";
			this.lblMusic.Size = new System.Drawing.Size(58, 20);
			this.lblMusic.TabIndex = 10;
			this.lblMusic.Text = "Music: ";
			// 
			// KalimbaManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(298, 171);
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
			this.Text = "Kalimba Manager 1.7.7";
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KalimbaManager_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.musicVolume)).EndInit();
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
	}
}