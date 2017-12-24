using LiveSplit.Kalimba.Memory;
#if LiveSplit
using IrcDotNet;
using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using LiveSplit.View;
using LiveSplit.Web.SRL;
#endif
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
namespace LiveSplit.Kalimba {
	public partial class KalimbaManager : Form {
		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll")]
		static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
		public KalimbaMemory Memory { get; set; }
		public KalimbaComponent Component { get; set; }
		private int lockedCheckpoint = -1;
		private DateTime lastCheckLoading = DateTime.MinValue;
		private Dictionary<int, float> oldZoomValues = new Dictionary<int, float>();
		private RaceWatcher raceWatcher = new RaceWatcher();
		private Thread getValuesThread = null;
		private KeyboardHook keyboard = new KeyboardHook();
		public bool AlwaysShown { get; set; }

		public KalimbaManager(bool shown) {
			InitializeComponent();
			Text = "Kalimba Manager " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
			AlwaysShown = shown;
			Visible = shown;

			keyboard.KeyPressed += Keyboard_KeyPressed;
			keyboard.RegisterHotKey(Keys.Control | Keys.Z);
			keyboard.RegisterHotKey(Keys.Control | Keys.Shift | Keys.Z);
			keyboard.RegisterHotKey(Keys.Control | Keys.N);
			keyboard.RegisterHotKey(Keys.Control | Keys.P);
			keyboard.RegisterHotKey(Keys.Control | Keys.L);
			keyboard.RegisterHotKey(Keys.Control | Keys.E);
			keyboard.RegisterHotKey(Keys.Control | Keys.C);
			keyboard.RegisterHotKey(Keys.Control | Keys.I);
			keyboard.RegisterHotKey(Keys.Control | Keys.K);
			keyboard.RegisterHotKey(Keys.Control | Keys.Shift | Keys.P);
			keyboard.RegisterHotKey(Keys.Control | Keys.F);
			keyboard.RegisterHotKey(Keys.Control | Keys.R);
			keyboard.RegisterHotKey(Keys.Control | Keys.T);

			getValuesThread = new Thread(UpdateLoop);
			getValuesThread.IsBackground = true;
			getValuesThread.Start();
		}

		private void Keyboard_KeyPressed(object sender, KeyEventArgs e) {
			if (this.InvokeRequired) {
				this.Invoke((Action<object, KeyEventArgs>)Keyboard_KeyPressed, sender, e);
				return;
			}

			if (Focused || Memory == null || !Memory.IsHooked) { return; }

			IntPtr ptr = GetForegroundWindow();
			StringBuilder sb = new StringBuilder(256);
			string title = null;
			if (GetWindowText(ptr, sb, 256) > 0) {
				title = sb.ToString();
			}
			if (!"Kalimba".Equals(title, StringComparison.OrdinalIgnoreCase)) { return; }

			if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Z) {
				if (itemNewGame.Enabled) {
					itemNewGame_Click(this, null);
				}
			} else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.Z) {
				if (itemAllTotems.Enabled) {
					itemAllTotems_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.N) {
				if (itemCheckpointNext.Enabled) {
					itemCheckpointNext_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.P) {
				if (itemCheckpointPrevious.Enabled) {
					itemCheckpointPrevious_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.L) {
				if (itemCheckpointLock.Enabled) {
					itemCheckpointLock.Checked = !itemCheckpointLock.Checked;
					itemCheckpointLock_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.E) {
				if (itemLevelErase.Enabled) {
					itemLevelErase_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C) {
				if (itemLevelClear.Enabled) {
					itemLevelClear_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.I) {
				if (itemInvincibleToOoze.Enabled) {
					itemInvincibleToOoze.Checked = !itemInvincibleToOoze.Checked;
					itemInvincibleToOoze_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.K) {
				if (itemKillTotems.Enabled) {
					itemKillTotems_Click(this, null);
				}
			} else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.P) {
				if (itemNoPickups.Enabled) {
					itemNoPickups.Checked = !itemNoPickups.Checked;
					itemNoPickups_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F) {
				if (itemCameraLead.Enabled) {
					itemCameraLead.Checked = !itemCameraLead.Checked;
					itemCameraLead_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.R) {
				if (itemCameraTrail.Enabled) {
					itemCameraTrail.Checked = !itemCameraTrail.Checked;
					itemCameraTrail_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.T) {
				if (itemTASDisplay.Enabled) {
					itemTASDisplay.Checked = !itemTASDisplay.Checked;
					itemTASDisplay_Click(this, null);
				}
			}
		}
		private void KalimbaManager_FormClosing(object sender, FormClosingEventArgs e) {
			e.Cancel = Memory != null && !AlwaysShown;
			if (!e.Cancel) {
				if (getValuesThread != null) {
					getValuesThread = null;
				}
				keyboard.KeyPressed -= Keyboard_KeyPressed;
			}
		}
		private void UpdateLoop() {
			while (getValuesThread != null) {
				try {
					if (Component != null && AlwaysShown) {
						Component.GetValues();
					}
					keyboard.Poll();
					UpdateValues();
					Thread.Sleep(15);
				} catch { }
			}
		}
		public void UpdateValues() {
			if (this.InvokeRequired) {
				this.Invoke((Action)UpdateValues);
			} else if (Memory != null && Memory.IsHooked) {
				if (!Visible) { this.Show(); }

				lblNotAvailable.Visible = false;
				MenuScreen menu = Memory.GetCurrentMenu();
				bool inGame = menu == MenuScreen.InGame || menu == MenuScreen.InGameMenu;
				bool inGameNotRunning = inGame && (Component == null || Component.IsNotRunning());
				bool notInGameNotRunning = !inGame && (Component == null || Component.IsNotRunning());
				bool canClearErase = menu == MenuScreen.SinglePlayerDLCMap || menu == MenuScreen.SinglePlayerMap || menu == MenuScreen.CoopDLCMap || menu == MenuScreen.CoopMap;

				itemLevelClear.Enabled = canClearErase;
				itemLevelErase.Enabled = canClearErase;
				itemNewGame.Enabled = notInGameNotRunning;
				itemAllTotems.Enabled = notInGameNotRunning;
				itemCheckpointNext.Enabled = inGameNotRunning;
				itemCheckpointPrevious.Enabled = inGameNotRunning;
				itemCheckpointLock.Enabled = inGameNotRunning;
				chkLockZoom.Enabled = inGameNotRunning;
				itemCameraLead.Enabled = inGameNotRunning;
				itemCameraTrail.Enabled = inGameNotRunning;
				itemInvincibleToOoze.Enabled = inGameNotRunning;
				itemNoPickups.Enabled = inGameNotRunning;
				itemKillTotems.Enabled = inGameNotRunning;

				if (!inGameNotRunning) {
					itemCheckpointLock.Checked = false;
					itemNoPickups.Checked = false;
					chkLockZoom.Checked = false;
					itemCameraLead.Checked = false;
					itemCameraTrail.Checked = false;
					itemInvincibleToOoze.Checked = false;
				} else if (itemInvincibleToOoze.Checked && !Memory.IsInvincible() && !Memory.IsDying()) {
					Memory.SetInvincible(true);
				}

				int currentCheckpoint = -1;
				float zoom = 0;
				if (inGame) {
					currentCheckpoint = Memory.GetCurrentCheckpoint();
					if (itemCheckpointLock.Checked && currentCheckpoint != lockedCheckpoint) {
						Memory.SetCheckpoint(lockedCheckpoint, false);
						currentCheckpoint = lockedCheckpoint;
					}

					zoom = Memory.Zoom();

					lblP1P2Pos.Text = "T1: (" + Memory.GetLastXP1().ToString("0.00") + ", " + Memory.GetLastYP1().ToString("0.00") + ")  T2: (" + Memory.GetLastXP2().ToString("0.00") + ", " + Memory.GetLastYP2().ToString("0.00") + ")";
					lblP3P4Pos.Text = "T3: (" + Memory.GetLastXP3().ToString("0.00") + ", " + Memory.GetLastYP3().ToString("0.00") + ")  T4: (" + Memory.GetLastXP4().ToString("0.00") + ", " + Memory.GetLastYP4().ToString("0.00") + ")";
					lblCurrentCheckpoint.Text = "Checkpoint: " + (currentCheckpoint + 1) + " / " + Memory.GetCheckpointCount();
				} else {
					lblP1P2Pos.Text = "T1: (0.00, 0.00) T2: (0.00, 0.00)";
					lblP3P4Pos.Text = "T3: (0.00, 0.00) T4: (0.00, 0.00)";
					lblCurrentCheckpoint.Text = "Checkpoint: N/A";
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

				if (itemCameraLead.Checked || itemCameraTrail.Checked) {
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

					Memory.SetCameraOffset((itemCameraLead.Checked ? maxX : minX) - currentZoneCenterX, (itemCameraLead.Checked ? maxY : minY) - currentZoneCenterY);
				}

				PlatformLevelId level = Memory.SelectedLevel();
				lblLevel.Text = "Level: " + level.ToString() + " (" + ((LevelID)level).ToString() + ")";

				if (itemTASDisplay.Checked) {
					lblTASOutput.Text = Memory.ReadTASOutput();
					bool TASUI = Memory.TASUI();
					if (TASUI == itemTASDisplay.Checked) {
						itemTASDisplay.Checked = !TASUI;
					}
				}

				float temp;
				if (float.TryParse(txtMusicVolume.Text, out temp)) {
					if (temp < 0) {
						temp = 0;
					} else if (temp > 100) {
						temp = 100;
					}
					Memory.SetMusicVolume(temp / 100f);
				}

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
			} else {
				if (!AlwaysShown) {
					this.Hide();
				} else {
					lblNotAvailable.Visible = true;
				}
			}
		}
		private void chkLockZoom_CheckedChanged(object sender, EventArgs e) {
			try {
				if (!chkLockZoom.Checked) {
					foreach (KeyValuePair<int, float> pair in oldZoomValues) {
						Memory.Program.Write((IntPtr)pair.Key, pair.Value, 0x24);
					}
					oldZoomValues.Clear();
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void itemTASDisplay_Click(object sender, EventArgs e) {
			try {
				Memory.SetTASUI(!itemTASDisplay.Checked);
				lblTASOutput.Visible = itemTASDisplay.Checked;
				if (itemTASDisplay.Checked) {
					Size = new Size(Size.Width, 209);
				} else {
					Size = new Size(Size.Width, 170);
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void itemCameraLead_Click(object sender, EventArgs e) {
			try {
				if (itemCameraTrail.Checked && itemCameraLead.Checked) {
					itemCameraTrail.Checked = false;
				} else if (!itemCameraTrail.Checked && !itemCameraLead.Checked) {
					Memory.ResetCamera();
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void itemCameraTrail_Click(object sender, EventArgs e) {
			try {
				if (itemCameraTrail.Checked && itemCameraLead.Checked) {
					itemCameraLead.Checked = false;
				} else if (!itemCameraTrail.Checked && !itemCameraLead.Checked) {
					Memory.ResetCamera();
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void itemKillTotems_Click(object sender, EventArgs e) {
			try {
				Memory.KillTotems();
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void itemInvincibleToOoze_Click(object sender, EventArgs e) {
			try {
				Memory.SetCurrentDeaths(70);
				Memory.SetInvincible(itemInvincibleToOoze.Checked);
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void itemNoPickups_Click(object sender, EventArgs e) {
			try {
				Memory.PassthroughPickups(itemNoPickups.Checked);
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void itemCheckpointLock_Click(object sender, EventArgs e) {
			try {
				if (itemCheckpointLock.Checked) {
					lockedCheckpoint = Memory.GetCurrentCheckpoint();
				} else {
					lockedCheckpoint = -1;
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void itemCheckpointNext_Click(object sender, EventArgs e) {
			try {
				int cp = Memory.GetCurrentCheckpoint();
				Memory.SetCheckpoint(cp + 1);
				if (lockedCheckpoint >= 0) {
					lockedCheckpoint = Memory.GetCurrentCheckpoint();
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void itemCheckpointPrevious_Click(object sender, EventArgs e) {
			try {
				int cp = Memory.GetCurrentCheckpoint();
				Memory.SetCheckpoint(cp - 1);
				if (lockedCheckpoint >= 0) {
					lockedCheckpoint = Memory.GetCurrentCheckpoint();
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void itemLevelErase_Click(object sender, EventArgs e) {
			try {
				MenuScreen screen = Memory.GetCurrentMenu();
				if (screen == MenuScreen.CoopDLCMap || screen == MenuScreen.CoopMap || screen == MenuScreen.SinglePlayerDLCMap || screen == MenuScreen.SinglePlayerMap) {
					Memory.SetLevelScore(Memory.SelectedLevel(), 0, true);
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void itemLevelClear_Click(object sender, EventArgs e) {
			try {
				MenuScreen screen = Memory.GetCurrentMenu();
				if (screen == MenuScreen.CoopDLCMap || screen == MenuScreen.CoopMap || screen == MenuScreen.SinglePlayerDLCMap || screen == MenuScreen.SinglePlayerMap) {
					Memory.SetLevelScore(Memory.SelectedLevel(), 0);
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void itemNewGame_Click(object sender, EventArgs e) {
			try {
				if (Memory.GetCurrentMenu() != MenuScreen.MainMenu) {
					MessageBox.Show("Please go to the Main Menu before setting Kalimba back to a New Game.", "Kalimba");
				} else {
					Memory.EraseData();
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void itemAllTotems_Click(object sender, EventArgs e) {
			try {
				if (Memory.GetCurrentMenu() != MenuScreen.MainMenu) {
					MessageBox.Show("Please go to the Main Menu before activating All Totems.", "Kalimba");
				} else {
					Memory.SetLevelScore(PlatformLevelId.None, 40);
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
	public class RaceWatcher {
#if LiveSplit
		private SpeedRunsLiveIRC raceIRC = null;
		private IrcChannel liveSplitChannel = null;
		private IrcClient raceClient = null;
		private RegularTimeFormatter timeFormatter = new RegularTimeFormatter(TimeAccuracy.Hundredths);
		private PlatformLevelId lastLevel = PlatformLevelId.None;
		private int lastCheckPoint = -1;
#endif

		public void UpdateRace(bool inGame, PlatformLevelId currentLevel, int currentCheckpoint, bool levelEnded) {
#if LiveSplit
			try {
				if (raceIRC != null && raceIRC.RaceState == RaceState.RaceEnded) {
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
#endif
		}
#if LiveSplit
		private void SendCheckpointInfo() {
			if (liveSplitChannel != null && raceIRC.RaceState == RaceState.RaceStarted) {
				if (raceIRC.Model.CurrentState.CurrentSplitIndex >= 0) {
					var split = raceIRC.Model.CurrentState.Run[raceIRC.Model.CurrentState.CurrentSplitIndex];
					Time currentTime = raceIRC.Model.CurrentState.CurrentTime;
					var timeRTA = "-";
					if (currentTime.RealTime != null)
						timeRTA = timeFormatter.Format(currentTime.RealTime);
					if (raceIRC.Model.CurrentState.CurrentPhase == TimerPhase.Running) {
						raceClient.LocalUser.SendMessage(liveSplitChannel, $"!time RealTime \"{Escape(split.Name)}|{lastCheckPoint + 1}\" {timeRTA}");
					}
				}
			}
		}
		private static string Escape(string value) {
			return value.Replace("\\", "\\\\").Replace("\"", "\\.");
		}
#endif
	}
}