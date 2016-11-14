﻿using LiveSplit.Kalimba.Memory;
using System;
using System.Threading;
using System.Windows.Forms;
namespace LiveSplit.Kalimba {
	public partial class KalimbaManager : Form {
		public KalimbaMemory Memory { get; set; }
		public KalimbaComponent Component { get; set; }
		private int lockedCheckpoint = -1;
		private DateTime lastCheckLoading = DateTime.MinValue;
		public KalimbaManager() {
			InitializeComponent();
			Visible = false;
			Thread t = new Thread(UpdateLoop);
			t.IsBackground = true;
			t.Start();
		}

		private void KalimbaManager_FormClosing(object sender, FormClosingEventArgs e) {
			e.Cancel = Memory != null;
		}

		private void btnNewGame_Click(object sender, System.EventArgs e) {
			try {
				if (Memory.GetCurrentMenu() != MenuScreen.MainMenu) {
					MessageBox.Show("Please go to the Main Menu before setting Kalimba back to a New Game.", "Kalimba");
				} else {
					Memory.EraseData();
				}
			} catch { }
		}
		private void btnAllTotems_Click(object sender, System.EventArgs e) {
			try {
				if (Memory.GetCurrentMenu() != MenuScreen.MainMenu) {
					MessageBox.Show("Please go to the Main Menu before resetting all Totems.", "Kalimba");
				} else {
					Memory.SetLevelScore(PlatformLevelId.None, 40);
				}
			} catch { }
		}
		private void btnPreviousCheckpoint_Click(object sender, System.EventArgs e) {
			try {
				int cp = Memory.GetCurrentCheckpoint();
				Memory.SetCheckpoint(cp - 1);
				if (lockedCheckpoint >= 0) {
					lockedCheckpoint = Memory.GetCurrentCheckpoint();
				}
			} catch { }
		}
		private void btnNextCheckpoint_Click(object sender, System.EventArgs e) {
			try {
				int cp = Memory.GetCurrentCheckpoint();
				Memory.SetCheckpoint(cp + 1);
				if (lockedCheckpoint >= 0) {
					lockedCheckpoint = Memory.GetCurrentCheckpoint();
				}
			} catch { }
		}

		private void UpdateLoop() {
			try {
				while (true) {
					UpdateValues();
					Thread.Sleep(15);
				}
			} catch { }
		}
		public void UpdateValues() {
			if (this.InvokeRequired) {
				this.Invoke((Action)UpdateValues);
			} else if (this.Visible && Memory != null && Memory.IsHooked) {
				MenuScreen menu = Memory.GetCurrentMenu();
				MenuScreen prevMenu = Memory.GetPreviousMenu();
				bool inGameNotRunning = menu == MenuScreen.InGame && (Component == null || Component.Model == null || Component.Model.CurrentState.CurrentPhase != Model.TimerPhase.Running);
				btnNextCheckpoint.Enabled = inGameNotRunning;
				btnPreviousCheckpoint.Enabled = inGameNotRunning;
				lblLevel.Text = "Level: " + Memory.SelectedLevel().ToString();
				int currentCheckpoint = Memory.GetCurrentCheckpoint();
				if (chkLockCheckpoint.Checked && currentCheckpoint != lockedCheckpoint) {
					Memory.SetCheckpoint(lockedCheckpoint);
					currentCheckpoint = lockedCheckpoint;
				}

				chkLockZoom.Enabled = inGameNotRunning;
				chkCameraLead.Enabled = inGameNotRunning;
				chkCameraTrail.Enabled = inGameNotRunning;
				float zoom = Memory.Zoom();
				if (chkLockZoom.Checked) {
					Memory.SetZoom(zoomValue.Value);
					zoomValue.Enabled = true;
				} else {
					zoomValue.Enabled = false;
					zoomValue.Value = Math.Min(Math.Max(0, (int)Memory.Zoom()), 150);
				}

				if (chkCameraLead.Checked || chkCameraTrail.Checked) {
					float currentZoneCenter = Memory.CameraCenterX();
					float p1x = Memory.GetLastXP1();
					float p2x = Memory.GetLastXP2();
					if ((int)currentZoneCenter == 0) {
						currentZoneCenter = p1x;
					}
					float min = Math.Min(p1x, p2x);
					float max = Math.Max(p1x, p2x);
					Memory.SetCameraOffset((chkCameraLead.Checked ? max : min) - currentZoneCenter);
				}

				lblCurrentCheckpoint.Text = "Checkpoint: " + (currentCheckpoint + 1) + " / " + Memory.GetCheckpointCount();
				lblP1Pos.Text = "T1: (" + Memory.GetLastXP1().ToString("0.00") + ", " + Memory.GetLastYP1().ToString("0.00") + ")";
				lblP2Pos.Text = "T2: (" + Memory.GetLastXP2().ToString("0.00") + ", " + Memory.GetLastYP2().ToString("0.00") + ")";
				chkPickups.Enabled = inGameNotRunning;
				chkLockCheckpoint.Enabled = inGameNotRunning;

				Memory.SetMusicVolume((float)musicVolume.Value / 20f);

				if (!inGameNotRunning) {
					chkLockCheckpoint.Checked = false;
					chkPickups.Checked = false;
					chkLockZoom.Checked = false;
					chkCameraLead.Checked = false;
					chkCameraTrail.Checked = false;
				}

				if (prevMenu == MenuScreen.SpeedRunLevelSelect && menu == MenuScreen.Loading && !Memory.SpeedrunLoaded()) {
					if (lastCheckLoading == DateTime.MinValue) {
						lastCheckLoading = DateTime.Now;
					} else if (lastCheckLoading.AddSeconds(5) < DateTime.Now) {
						Memory.FixSpeedrun();
						lastCheckLoading = DateTime.MinValue;
					}
				}
			} else if (Memory == null && this.Visible) {
				this.Hide();
			}
		}
		private void chkLockCheckpoint_CheckedChanged(object sender, EventArgs e) {
			if (chkLockCheckpoint.Checked) {
				lockedCheckpoint = Memory.GetCurrentCheckpoint();
			} else {
				lockedCheckpoint = -1;
			}
		}
		private void chkPickups_CheckedChanged(object sender, EventArgs e) {
			Memory.PassthroughPickups(chkPickups.Checked);
		}
		private void chkCameraLead_CheckedChanged(object sender, EventArgs e) {
			if (chkCameraLead.Checked && chkCameraTrail.Checked) {
				chkCameraTrail.Checked = false;
			} else if (!chkCameraLead.Checked && !chkCameraTrail.Checked) {
				Memory.ResetCamera();
			}
		}
		private void chkCameraTrail_CheckedChanged(object sender, EventArgs e) {
			if (chkCameraLead.Checked && chkCameraTrail.Checked) {
				chkCameraLead.Checked = false;
			} else if (!chkCameraLead.Checked && !chkCameraTrail.Checked) {
				Memory.ResetCamera();
			}
		}
	}
}