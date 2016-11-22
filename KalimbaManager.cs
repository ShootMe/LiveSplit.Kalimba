using IrcDotNet;
using LiveSplit.Kalimba.Memory;
using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using LiveSplit.View;
using LiveSplit.Web.SRL;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
namespace LiveSplit.Kalimba {
	public partial class KalimbaManager : Form {
		public KalimbaMemory Memory { get; set; }
		public KalimbaComponent Component { get; set; }
		private int lockedCheckpoint = -1;
		private DateTime lastCheckLoading = DateTime.MinValue;
		private Dictionary<int, float> oldZoomValues = new Dictionary<int, float>();
		private RaceWatcher raceWatcher = new RaceWatcher();

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
				bool inGame = menu == MenuScreen.InGame || menu == MenuScreen.InGameMenu;
				bool inGameNotRunning = inGame && (Component == null || Component.Model == null || Component.Model.CurrentState.CurrentPhase != Model.TimerPhase.Running);

				btnNextCheckpoint.Enabled = inGameNotRunning;
				btnPreviousCheckpoint.Enabled = inGameNotRunning;
				btnKill.Enabled = inGameNotRunning;
				chkLockZoom.Enabled = inGameNotRunning;
				chkCameraLead.Enabled = inGameNotRunning;
				chkCameraTrail.Enabled = inGameNotRunning;
				chkInvincible.Enabled = inGameNotRunning;
				chkPickups.Enabled = inGameNotRunning;
				chkLockCheckpoint.Enabled = inGameNotRunning;

				if (!inGameNotRunning) {
					chkLockCheckpoint.Checked = false;
					chkPickups.Checked = false;
					chkLockZoom.Checked = false;
					chkCameraLead.Checked = false;
					chkCameraTrail.Checked = false;
					chkInvincible.Checked = false;
				} else if (chkInvincible.Checked && !Memory.IsInvincible() && !Memory.IsDying()) {
					Memory.SetInvincible(true);
				}

				int currentCheckpoint = -1;
				float zoom = 0;
				if (inGame) {
					currentCheckpoint = Memory.GetCurrentCheckpoint();
					if (chkLockCheckpoint.Checked && currentCheckpoint != lockedCheckpoint) {
						Memory.SetCheckpoint(lockedCheckpoint, false);
						currentCheckpoint = lockedCheckpoint;
					}

					zoom = Memory.Zoom();

					lblP1Pos.Text = "T1: (" + Memory.GetLastXP1().ToString("0.00") + ", " + Memory.GetLastYP1().ToString("0.00") + ")";
					lblP2Pos.Text = "T2: (" + Memory.GetLastXP2().ToString("0.00") + ", " + Memory.GetLastYP2().ToString("0.00") + ")";
					lblCurrentCheckpoint.Text = "Checkpoint: " + (currentCheckpoint + 1) + " / " + Memory.GetCheckpointCount();
				}

				raceWatcher.UpdateRace(inGame, Memory.GetPlatformLevelId(), currentCheckpoint, Memory.LevelComplete());

				if (chkLockZoom.Checked) {
					int cameraZone = Memory.CameraZone();
					if (!oldZoomValues.ContainsKey(cameraZone)) {
						oldZoomValues.Add(cameraZone, Memory.Zoom());
					}
					Memory.SetZoom(zoomValue.Value);
					zoomValue.Enabled = true;
				} else {
					zoomValue.Enabled = false;
					zoomValue.Value = Math.Min(Math.Max(0, (int)zoom), 150);
				}

				if (chkCameraLead.Checked || chkCameraTrail.Checked) {
					float currentZoneCenterX = Memory.CameraCenterX();
					float currentZoneCenterY = Memory.CameraCenterY();
					float p1x = Memory.GetLastXP1();
					float p2x = Memory.GetLastXP2();
					float p1y = Memory.GetLastYP1();
					float p2y = Memory.GetLastYP2();
					float minX = p1x;
					float maxX = p2x;
					float minY = p1y;
					float maxY = p2y;
					if (maxX < minX) {
						minX = p2x;
						maxX = p1x;
						minY = p2y;
						maxY = p1y;
					}

					Memory.SetCameraOffset((chkCameraLead.Checked ? maxX : minX) - currentZoneCenterX, (chkCameraLead.Checked ? maxY : minY) - currentZoneCenterY);
				}

				lblLevel.Text = "Level: " + Memory.SelectedLevel().ToString();

				Memory.SetMusicVolume((float)musicVolume.Value / 20f);

				if (menu == MenuScreen.Loading) {
					MenuScreen prevMenu = Memory.GetPreviousMenu();
					if (prevMenu == MenuScreen.SpeedRunLevelSelect && !Memory.SpeedrunLoaded()) {
						if (lastCheckLoading == DateTime.MinValue) {
							lastCheckLoading = DateTime.Now;
						} else if (lastCheckLoading.AddSeconds(5) < DateTime.Now) {
							Memory.FixSpeedrun();
							lastCheckLoading = DateTime.MinValue;
						}
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
		private void chkLockZoom_CheckedChanged(object sender, EventArgs e) {
			if (!chkLockZoom.Checked) {
				foreach (KeyValuePair<int, float> pair in oldZoomValues) {
					Memory.Program.Write<float>((IntPtr)pair.Key, pair.Value, 0x24);
				}
				oldZoomValues.Clear();
			}
		}
		private void chkInvincible_CheckedChanged(object sender, EventArgs e) {
			Memory.SetCurrentDeaths(70);
			Memory.SetInvincible(chkInvincible.Checked);
		}
		private void btnKill_Click(object sender, EventArgs e) {
			Memory.KillTotems();
		}
	}
	public class RaceWatcher {
		private SpeedRunsLiveIRC raceIRC = null;
		private IrcChannel liveSplitChannel = null;
		private IrcClient raceClient = null;
		private RegularTimeFormatter timeFormatter = new RegularTimeFormatter(TimeAccuracy.Hundredths);
		private PlatformLevelId lastLevel = PlatformLevelId.None;
		private int lastCheckPoint = -1;

		public void UpdateRace(bool inGame, PlatformLevelId currentLevel, int currentCheckpoint, bool levelEnded) {
			try {
				if (raceIRC != null && !raceIRC.IsConnected) {
					raceIRC = null;
					liveSplitChannel = null;
					raceClient = null;
					lastCheckPoint = -1;
					lastLevel = PlatformLevelId.None;
				}

				if (raceIRC == null || liveSplitChannel == null || raceClient == null) {
					foreach (var form in Application.OpenForms) {
						SpeedRunsLiveForm srl = form as SpeedRunsLiveForm;
						if (srl != null) {
							PropertyInfo[] fields = typeof(SpeedRunsLiveForm).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
							PropertyInfo field = null;
							for (int i = 0; i < fields.Length; i++) {
								if (fields[i].Name.IndexOf("SRLClient", StringComparison.OrdinalIgnoreCase) >= 0 && fields[i].PropertyType == typeof(SpeedRunsLiveIRC)) {
									field = fields[i];
									break;
								}
							}

							if (field != null) {
								raceIRC = (SpeedRunsLiveIRC)field.GetValue(srl);
							}

							if (raceIRC != null) {
								fields = typeof(SpeedRunsLiveIRC).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
								field = null;
								for (int i = 0; i < fields.Length; i++) {
									if (fields[i].Name.IndexOf("LiveSplitChannel", StringComparison.OrdinalIgnoreCase) >= 0 && fields[i].PropertyType == typeof(IrcChannel)) {
										field = fields[i];
										break;
									}
								}
								if (field != null) {
									liveSplitChannel = (IrcChannel)field.GetValue(raceIRC);
								}

								field = null;
								for (int i = 0; i < fields.Length; i++) {
									if (fields[i].Name.IndexOf("Client", StringComparison.OrdinalIgnoreCase) >= 0 && fields[i].PropertyType == typeof(IrcClient)) {
										field = fields[i];
										break;
									}
								}
								if (field != null) {
									raceClient = (IrcClient)field.GetValue(raceIRC);
								}
							}
							break;
						}
					}
				}

				if (raceIRC != null && liveSplitChannel != null && raceClient != null) {
					if (inGame) {
						if (currentLevel != lastLevel) {
							lastLevel = currentLevel;
							lastCheckPoint = -1;
						}
						if (levelEnded) { currentCheckpoint++; }

						if (currentCheckpoint > lastCheckPoint) {
							lastCheckPoint = currentCheckpoint;
							SendCheckpointInfo();
						}
					}
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
			}
		}
		private void SendCheckpointInfo() {
			if (liveSplitChannel != null && raceIRC.RaceState == RaceState.RaceStarted) {
				if (raceIRC.Model.CurrentState.CurrentSplitIndex >= 0) {
					var split = raceIRC.Model.CurrentState.Run[raceIRC.Model.CurrentState.CurrentSplitIndex];
					Time currentTime = raceIRC.Model.CurrentState.CurrentTime;
					var timeRTA = "-";
					if (currentTime.RealTime != null)
						timeRTA = timeFormatter.Format(currentTime.RealTime);
					if (raceIRC.Model.CurrentState.CurrentPhase == TimerPhase.Running) {
						raceClient.LocalUser.SendMessage(liveSplitChannel, $"!time RealTime \"{Escape(split.Name)}|{lastCheckPoint}\" {timeRTA}");
					}
				}
			}
		}
		private static string Escape(string value) {
			return value.Replace("\\", "\\\\").Replace("\"", "\\.");
		}
	}
}