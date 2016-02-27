using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Kalimba.Memory;
namespace LiveSplit.Kalimba {
	public class KalimbaComponent : IComponent {
		public string ComponentName { get { return "Kalimba Autosplitter"; } }
		public TimerModel Model { get; set; }
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		private KalimbaMemory mem;
		private int currentSplit = 0;
		private int state = 0;
		private bool hasLog = false;
		private int lastLogCheck = 0;
		private float lastYP2;
		private MenuScreen lastMenu = MenuScreen.MainMenu;
		private MenuScreen mainMenu = MenuScreen.MainMenu;
		double levelTimes;
		private int lastLevelComplete = 0;
		internal static string[] keys = { "CurrentSplit", "World", "Campaign", "CurrentMenu", "PreviousMenu", "Cinematic", "LoadingLevel", "LevelTime", "Disabled", "Score", "Deaths", "LevelName", "P1Y", "P2Y", "State", "EndLevel", "PlayerState", "Frozen", "InTransition", "PlatformLevel", "Checkpoint", "CheckpointCount", "Stats" };
		private Dictionary<string, string> currentValues = new Dictionary<string, string>();
		private KalimbaManager manager;

		public KalimbaComponent() {
			mem = new KalimbaMemory();
			foreach (string key in keys) {
				currentValues[key] = "";
			}
			manager = new KalimbaManager();
			manager.Memory = mem;
			manager.Component = this;
			manager.Show();
			manager.Visible = false;
		}

		public void GetValues() {
			if (!mem.HookProcess()) {
				if (manager.Visible) { manager.Invoke((Action)delegate () { manager.Hide(); }); }
				return;
			} else if (!manager.Visible) {
				manager.Invoke((Action)delegate () { manager.Show(); });
			}

			MenuScreen screen = mem.GetCurrentMenu();

			if (Model != null) {
				if (Model.CurrentState.CurrentPhase == TimerPhase.NotRunning) {
					mainMenu = screen;
				}
				if (Model.CurrentState.Run.Count == 1) {
					HandleIL(screen);
				} else if (Model.CurrentState.Run.Count == 10) {
					if (mainMenu == MenuScreen.SinglePlayerPathSelect) {
						HandleDarkVoid(screen);
					} else if (mainMenu == MenuScreen.CoopMap) {
						HandleJourneyCoop(screen);
					} else {
						HandleDarkVoidCoop(screen);
					}
					if (currentSplit == 1 && (screen == MenuScreen.CoopMap || screen == MenuScreen.CoopDLCMap)) {
						mainMenu = screen;
					}
				} else if (Model.CurrentState.Run.Count == 24) {
					HandleJourney(screen);
				}

				HandleGameTimes(screen);
			}

			LogValues(screen);
		}
		private void HandleIL(MenuScreen screen) {
			bool shouldSplit = false;

			if (currentSplit == 0) {
				MenuScreen prev = mem.GetPreviousMenu();
				if (state == 0 && screen == MenuScreen.Loading && (prev == MenuScreen.SinglePlayerMap || prev == MenuScreen.SinglePlayerDLCMap || prev == MenuScreen.CoopMap || prev == MenuScreen.CoopDLCMap)) {
					state++;
				} else if (state == 1 && prev == MenuScreen.Loading) {
					state++;
					mem.SetLevelScore(mem.GetPlatformLevelId(), 0);
				} else if (state >= 2 && state <= 3) {
					shouldSplit = state++ == 3;
				}
			} else if (state == 0 && mem.GetEndLevel()) {
				state++;
			} else if (state >= 1) {
				if (screen != MenuScreen.InGame) {
					state = 0;
				} else {
					shouldSplit = state++ == 2;
				}
			}

			HandleSplit(shouldSplit, screen, screen == MenuScreen.SinglePlayerMap || screen == MenuScreen.SinglePlayerDLCMap || screen == MenuScreen.CoopMap || screen == MenuScreen.CoopDLCMap);
		}
		private void HandleJourney(MenuScreen screen) {
			bool shouldSplit = false;

			if (currentSplit == 0) {
				shouldSplit = screen == MenuScreen.SinglePlayerPathSelect && mem.GetPlayingCinematic();
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				MenuScreen prev = mem.GetPreviousMenu();
				if (currentSplit < 24) {
					shouldSplit = screen == MenuScreen.Loading && prev == MenuScreen.SinglePlayerMap && lastMenu != MenuScreen.Loading;
					if (shouldSplit && currentSplit == 1 && state == 0) {
						state++;
						shouldSplit = false;
					}
				} else if (currentSplit == 24) {
					if (screen == MenuScreen.Loading && prev == MenuScreen.InGame) {
						state = 0;
					}
					bool disabled = mem.GetIsDisabled();
					float p2Y = mem.GetLastYP2();
					if (state < 6) {
						if ((state & 1) == 0 ? disabled : !disabled) {
							state++;
						}
					} else {
						shouldSplit = p2Y < -211 && lastYP2 < p2Y;
					}
					lastYP2 = p2Y;
				}
			}

			lastMenu = screen;
			HandleSplit(shouldSplit, screen);
		}
		private void HandleDarkVoid(MenuScreen screen) {
			bool shouldSplit = false;

			if (currentSplit == 0) {
				shouldSplit = screen == MenuScreen.SinglePlayerPathSelect && mem.GetPlayingCinematic();
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				MenuScreen prev = mem.GetPreviousMenu();
				if (currentSplit < 10) {
					shouldSplit = screen == MenuScreen.Loading && prev == MenuScreen.SinglePlayerDLCMap && lastMenu != MenuScreen.Loading;
					if (shouldSplit && currentSplit == 1 && state == 0) {
						state++;
						shouldSplit = false;
					}
				} else if (currentSplit == 10) {
					shouldSplit = mem.GetEndLevel();
				}
			}

			lastMenu = screen;
			HandleSplit(shouldSplit, screen);
		}
		private void HandleJourneyCoop(MenuScreen screen) {
			bool shouldSplit = false;

			if (currentSplit == 0) {
				shouldSplit = screen == MenuScreen.CoopPathSelect && mem.GetPlayingCinematic();
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				MenuScreen prev = mem.GetPreviousMenu();
				if (currentSplit < 10) {
					shouldSplit = screen == MenuScreen.Loading && prev == MenuScreen.CoopMap && lastMenu != MenuScreen.Loading;
					if (shouldSplit && currentSplit == 1 && state == 0) {
						state++;
						shouldSplit = false;
					}
				} else if (currentSplit == 10) {
					shouldSplit = mem.GetEndLevel();
				}
			}

			lastMenu = screen;
			HandleSplit(shouldSplit, screen);
		}
		private void HandleDarkVoidCoop(MenuScreen screen) {
			bool shouldSplit = false;

			if (currentSplit == 0) {
				shouldSplit = screen == MenuScreen.CoopPathSelect && mem.GetPlayingCinematic();
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				MenuScreen prev = mem.GetPreviousMenu();
				if (currentSplit < 10) {
					shouldSplit = screen == MenuScreen.Loading && prev == MenuScreen.CoopDLCMap && lastMenu != MenuScreen.Loading;
					if (shouldSplit && currentSplit == 1 && state == 0) {
						state++;
						shouldSplit = false;
					}
				} else if (currentSplit == 10) {
					shouldSplit = mem.GetEndLevel();
				}
			}

			lastMenu = screen;
			HandleSplit(shouldSplit, screen);
		}
		private void HandleSplit(bool shouldSplit, MenuScreen screen, bool shouldReset = false) {
			if (currentSplit > 0 && (screen == MenuScreen.MainMenu || shouldReset)) {
				Model.Reset();
			} else if (shouldSplit) {
				if (currentSplit == 0) {
					Model.Start();
				} else {
					Model.Split();
				}
			}
		}
		private void HandleGameTimes(MenuScreen screen) {
			if (Model.CurrentState.IsGameTimePaused && screen == MenuScreen.InGame && !mem.GetFrozen() && mem.GetIsMoving()) {
				Model.CurrentState.IsGameTimePaused = false;
			}

			if (screen == MenuScreen.SinglePlayerEndLevelFeedBack && currentSplit > 0 && currentSplit != lastLevelComplete) {
				PersistentLevelStats level = mem.GetLevelStats(mem.GetPlatformLevelId());
				if (level.minMillisecondsForMaxScore != int.MaxValue) {
					double levelTime = (double)level.minMillisecondsForMaxScore / (double)1000;
					levelTimes += levelTime;
					Model.CurrentState.IsGameTimePaused = true;
					if (currentSplit == Model.CurrentState.Run.Count + 1) {
						Time t = Model.CurrentState.Run[lastLevelComplete].SplitTime;
						Model.CurrentState.Run[lastLevelComplete].SplitTime = new Time(t.RealTime, TimeSpan.FromSeconds(levelTimes));
					} else {
						Model.CurrentState.SetGameTime(TimeSpan.FromSeconds(levelTimes));
					}
					lastLevelComplete++;
					WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": Set game time " + levelTime + " " + levelTimes);
				}
			}
		}
		private void LogValues(MenuScreen screen) {
			if (lastLogCheck == 0) {
				hasLog = File.Exists("_Kalimba.log");
				lastLogCheck = 300;
			}
			lastLogCheck--;

			if (hasLog) {
				string prev = "", curr = "";
				foreach (string key in keys) {
					prev = currentValues[key];

					switch (key) {
						case "World": curr = mem.GetWorld().ToString(); break;
						case "Campaign": curr = mem.GetCampaign().ToString(); break;
						case "CurrentMenu": curr = mem.GetCurrentMenu().ToString(); break;
						case "PreviousMenu": curr = mem.GetPreviousMenu().ToString(); break;
						case "Cinematic": curr = mem.GetPlayingCinematic().ToString(); break;
						case "LoadingLevel": curr = mem.GetIsLoadingLevel().ToString(); break;
						case "Disabled": curr = mem.GetIsDisabled().ToString(); break;
						case "LevelTime": curr = mem.GetLevelTime().ToString(); break;
						case "Score": curr = mem.GetCurrentScore().ToString(); break;
						case "Deaths": curr = mem.GetCurrentDeaths().ToString(); break;
						case "CurrentSplit": curr = currentSplit.ToString(); break;
						case "State": curr = state.ToString(); break;
						case "EndLevel": curr = mem.GetEndLevel().ToString(); break;
						case "Frozen": curr = mem.GetFrozen().ToString(); break;
						case "PlayerState": curr = mem.GetCurrentStateP1().ToString(); break;
						case "InTransition": curr = mem.GetInTransition().ToString(); break;
						case "Stats":
							if (screen == MenuScreen.SinglePlayerEndLevelFeedBack) {
								PersistentLevelStats level = mem.GetLevelStats(mem.GetPlatformLevelId());
								curr = level == null ? "" : level.maxScore + " - " + level.minMillisecondsForMaxScore;
							} else {
								curr = "";
							}
							break;
						default: curr = ""; break;
					}

					if (!prev.Equals(curr)) {
						WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + key + ": ".PadRight(16 - key.Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

						currentValues[key] = curr;
					}
				}
			}
		}

		public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
			if (Model == null) {
				Model = new TimerModel() { CurrentState = lvstate };
				Model.InitializeGameTime();
				Model.CurrentState.IsGameTimePaused = true;
				lvstate.OnReset += OnReset;
				lvstate.OnPause += OnPause;
				lvstate.OnResume += OnResume;
				lvstate.OnStart += OnStart;
				lvstate.OnSplit += OnSplit;
				lvstate.OnUndoSplit += OnUndoSplit;
				lvstate.OnSkipSplit += OnSkipSplit;
			}

			GetValues();
		}

		public void OnReset(object sender, TimerPhase e) {
			currentSplit = 0;
			lastLevelComplete = 0;
			state = 0;
			levelTimes = 0;
			lastYP2 = 0;
			lastMenu = MenuScreen.MainMenu;
			Model.CurrentState.IsGameTimePaused = true;
			WriteLog("---------Reset----------------------------------");
		}
		public void OnResume(object sender, EventArgs e) {
			WriteLog("---------Resumed--------------------------------");
		}
		public void OnPause(object sender, EventArgs e) {
			WriteLog("---------Paused---------------------------------");
		}
		public void OnStart(object sender, EventArgs e) {
			currentSplit++;
			state = 0;
			Model.CurrentState.IsGameTimePaused = true;
			WriteLog("---------New Game-------------------------------");
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			currentSplit--;
			state = 0;
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) + ": CurrentSplit: " + currentSplit.ToString().PadLeft(24, ' '));
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			currentSplit++;
			state = 0;
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) + ": CurrentSplit: " + currentSplit.ToString().PadLeft(24, ' '));
		}
		public void OnSplit(object sender, EventArgs e) {
			currentSplit++;
			state = 0;
			Model.CurrentState.IsGameTimePaused = true;
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) + ": CurrentSplit: " + currentSplit.ToString().PadLeft(24, ' '));
		}
		private void WriteLog(string data) {
			if (hasLog) {
				Console.WriteLine(data);
				using (StreamWriter wr = new StreamWriter("_Kalimba.log", true)) {
					wr.WriteLine(data);
				}
			}
		}

		public Control GetSettingsControl(LayoutMode mode) { return null; }
		public void SetSettings(XmlNode settings) { }
		public XmlNode GetSettings(XmlDocument document) { return null; }
		public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
		public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }
		public float HorizontalWidth { get { return 0; } }
		public float MinimumHeight { get { return 0; } }
		public float MinimumWidth { get { return 0; } }
		public float PaddingBottom { get { return 0; } }
		public float PaddingLeft { get { return 0; } }
		public float PaddingRight { get { return 0; } }
		public float PaddingTop { get { return 0; } }
		public float VerticalHeight { get { return 0; } }
		public void Dispose() {
			manager.Memory = null;
			manager.Close();
			manager.Dispose();
		}
	}
}