using IrcDotNet;
using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using LiveSplit.View;
using LiveSplit.Web.SRL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
namespace LiveSplit.Kalimba {
	public partial class KalimbaManager : Form {
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();
		public MemoryManager Memory { get; set; }
		public KalimbaComponent Component { get; set; }
		private int lockedCheckpoint = -1;
		private DateTime lastCheckLoading = DateTime.MinValue;
		private Dictionary<int, float> oldZoomValues = new Dictionary<int, float>();
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

			if (Memory.Program.MainWindowHandle != GetForegroundWindow()) {
				return;
			}

			if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Z) {
				UpdateWorldMenu();
				if (itemNewGame.Enabled) {
					itemNewGame_Click(this, null);
				}
			} else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.Z) {
				UpdateWorldMenu();
				if (itemAllTotems.Enabled) {
					itemAllTotems_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.N) {
				UpdateCheckpointMenu();
				if (itemCheckpointNext.Enabled) {
					itemCheckpointNext_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.P) {
				UpdateCheckpointMenu();
				if (itemCheckpointPrevious.Enabled) {
					itemCheckpointPrevious_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.L) {
				UpdateCheckpointMenu();
				if (itemCheckpointLock.Enabled) {
					itemCheckpointLock.Checked = !itemCheckpointLock.Checked;
					itemCheckpointLock_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.E) {
				UpdateLevelMenu();
				if (itemLevelErase.Enabled) {
					itemLevelErase_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C) {
				UpdateLevelMenu();
				if (itemLevelClear.Enabled) {
					itemLevelClear_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.I) {
				UpdateLevelMenu();
				if (itemInvincibleToOoze.Enabled) {
					itemInvincibleToOoze.Checked = !itemInvincibleToOoze.Checked;
					itemInvincibleToOoze_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.K) {
				UpdateLevelMenu();
				if (itemKillTotems.Enabled) {
					itemKillTotems_Click(this, null);
				}
			} else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.P) {
				UpdateLevelMenu();
				if (itemNoPickups.Enabled) {
					itemNoPickups.Checked = !itemNoPickups.Checked;
					itemNoPickups_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F) {
				UpdateCameraMenu();
				if (itemCameraLead.Enabled) {
					itemCameraLead.Checked = !itemCameraLead.Checked;
					itemCameraLead_Click(this, null);
				}
			} else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.R) {
				UpdateCameraMenu();
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
					keyboard.Poll();
					UpdateValues();
					Thread.Sleep(15);
				} catch { }
			}
		}
		public void UpdateValues() {
			if (this.InvokeRequired) {
				this.Invoke((Action)UpdateValues);
			} else if (Memory != null && Memory.HookProcess()) {
				if (!Visible) { this.Show(); }

				lblNotAvailable.Visible = false;
				MenuScreen menu = Memory.GetCurrentMenu();
				bool inGame = menu == MenuScreen.InGame || menu == MenuScreen.InGameMenu;
				bool inGameNotRunning = inGame && (Component == null || Component.IsNotRunning());

				if (chkLockZoom.Enabled != inGameNotRunning) {
					chkLockZoom.Enabled = inGameNotRunning;
				}

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

					lblP1P2Pos.Text = $"T1: ({Memory.GetLastP1()})  T2: ({Memory.GetLastP2()})";
					lblP3P4Pos.Text = $"T3: ({Memory.GetLastP3()})  T4: ({Memory.GetLastP4()})";
					lblCurrentCheckpoint.Text = $"Checkpoint: {(currentCheckpoint + 1)} / {Memory.GetCheckpointCount()}";
				} else {
					lblP1P2Pos.Text = "T1: (0.00, 0.00) T2: (0.00, 0.00)";
					lblP3P4Pos.Text = "T3: (0.00, 0.00) T4: (0.00, 0.00)";
					lblCurrentCheckpoint.Text = "Checkpoint: N/A";
				}

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
					Vector2 p1 = Memory.GetLastP1();
					Vector2 p2 = Memory.GetLastP2();
					float minX = p1.X;
					float maxX = p2.X;
					float minY = p1.Y;
					float maxY = p2.Y;
					if (maxX < minX) {
						minX = p2.X;
						maxX = p1.X;
						minY = p2.Y;
						maxY = p1.Y;
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

				if (itemMusicEnable.Checked) {
					float temp;
					if (float.TryParse(txtMusicVolume.Text, out temp)) {
						if (temp < 0) {
							temp = 0;
						} else if (temp > 100) {
							temp = 100;
						}
						Memory.SetMusicVolume(temp / 100f);
					}
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
		private void menuWorld_Click(object sender, EventArgs e) {
			try {
				UpdateWorldMenu();
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void UpdateWorldMenu() {
			MenuScreen menu = Memory.GetCurrentMenu();
			bool inGame = menu == MenuScreen.InGame || menu == MenuScreen.InGameMenu;
			bool notInGameNotRunning = !inGame && (Component == null || Component.IsNotRunning());
			itemNewGame.Enabled = notInGameNotRunning;
			itemAllTotems.Enabled = notInGameNotRunning;
		}
		private void menuCheckpoint_Click(object sender, EventArgs e) {
			try {
				UpdateCheckpointMenu();
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void UpdateCheckpointMenu() {
			MenuScreen menu = Memory.GetCurrentMenu();
			bool inGame = menu == MenuScreen.InGame || menu == MenuScreen.InGameMenu;
			bool inGameNotRunning = inGame && (Component == null || Component.IsNotRunning());
			itemCheckpointNext.Enabled = inGameNotRunning;
			itemCheckpointPrevious.Enabled = inGameNotRunning;
			itemCheckpointLock.Enabled = inGameNotRunning;
		}
		private void menuLevel_Click(object sender, EventArgs e) {
			try {
				UpdateLevelMenu();
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void UpdateLevelMenu() {
			MenuScreen menu = Memory.GetCurrentMenu();
			bool inGame = menu == MenuScreen.InGame || menu == MenuScreen.InGameMenu;
			bool inGameNotRunning = inGame && (Component == null || Component.IsNotRunning());
			bool canClearErase = menu == MenuScreen.SinglePlayerDLCMap || menu == MenuScreen.SinglePlayerMap || menu == MenuScreen.CoopDLCMap || menu == MenuScreen.CoopMap;
			itemLevelClear.Enabled = canClearErase;
			itemLevelErase.Enabled = canClearErase;
			itemNoPickups.Enabled = inGameNotRunning;
			itemInvincibleToOoze.Enabled = inGameNotRunning;
			itemKillTotems.Enabled = inGameNotRunning;
		}
		private void menuCamera_Click(object sender, EventArgs e) {
			try {
				UpdateCameraMenu();
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void UpdateCameraMenu() {
			MenuScreen menu = Memory.GetCurrentMenu();
			bool inGame = menu == MenuScreen.InGame || menu == MenuScreen.InGameMenu;
			bool inGameNotRunning = inGame && (Component == null || Component.IsNotRunning());
			itemCameraLead.Enabled = inGameNotRunning;
			itemCameraTrail.Enabled = inGameNotRunning;
		}
		private void menuMusic_Click(object sender, EventArgs e) {
			try {
				txtMusicVolume.Enabled = itemMusicEnable.Checked;
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