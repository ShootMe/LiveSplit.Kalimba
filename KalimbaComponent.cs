#if LiveSplit
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;
#endif
using LiveSplit.Kalimba.Memory;
using System;
using System.Collections.Generic;
using System.IO;
namespace LiveSplit.Kalimba {
#if LiveSplit
	public class KalimbaComponent : IComponent {
		private TimerModel Model { get; set; }
		public string ComponentName { get { return "Kalimba Autosplitter"; } }
		private float lastYP2;
		private MenuScreen mainMenu = MenuScreen.MainMenu;
		private int lastLevelComplete = 0, startFrameCount, splitFrameCount;
#else
	public class KalimbaComponent {
#endif
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		internal static string[] keys = { "CurrentSplit", "World", "Campaign", "CurrentMenu", "PreviousMenu", "Cinematic", "LoadingLevel", "Disabled", "Score", "Deaths", "State", "EndLevel", "PlatformLevel", "Stats" };
		private KalimbaMemory mem;
		private int currentSplit = 0, state = 0, lastLogCheck = 0;
		private bool hasLog = false;
		private MenuScreen lastMenu = MenuScreen.MainMenu;
		private Dictionary<string, string> currentValues = new Dictionary<string, string>();
		public KalimbaManager Manager { get; set; }
		private DateTime lastSplit = DateTime.MinValue;
		private double[] levelTimes = new double[100];

		public KalimbaComponent(bool shown = false) {
			mem = new KalimbaMemory();
			foreach (string key in keys) {
				currentValues[key] = "";
			}
			Manager = new KalimbaManager(shown);
			Manager.Memory = mem;
			Manager.Component = this;
			Manager.Show();
			Manager.Visible = shown;
		}

		public void GetValues() {
			if (!mem.HookProcess()) {
				if (Manager.Visible && !Manager.AlwaysShown) { Manager.Invoke((Action)delegate () { Manager.Hide(); }); }
				return;
			} else if (!Manager.Visible) {
				Manager.Invoke((Action)delegate () { Manager.Show(); });
			}

			MenuScreen screen = mem.GetCurrentMenu();
#if LiveSplit
			if (Model != null) {
				if (Model.CurrentState.CurrentPhase == TimerPhase.NotRunning) {
					mainMenu = screen;
				}
				if (Model.CurrentState.Run.Count < 10) {
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
#endif

			lastMenu = screen;
			LogValues(screen);
		}
#if LiveSplit
		private void HandleIL(MenuScreen screen) {
			bool shouldSplit = false;

			if (currentSplit == 0) {
				MenuScreen prev = mem.GetPreviousMenu();
				if (state == 0 && screen == MenuScreen.Loading && (prev == MenuScreen.SinglePlayerMap || prev == MenuScreen.SinglePlayerDLCMap || prev == MenuScreen.CoopMap || prev == MenuScreen.CoopDLCMap)) {
					state++;
				} else if (state == 1 && prev == MenuScreen.Loading) {
					state++;
					mem.SetLevelScore(mem.GetPlatformLevelId(), 0);
				} else if (state == 2) {
					shouldSplit = true;
					startFrameCount = mem.FrameCount();
				}
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				if (currentSplit < Model.CurrentState.Run.Count && screen == MenuScreen.InGame) {
					string[] splits = Model.CurrentState.Run[currentSplit - 1].Name.Split(' ');
					float pickupsPos = -1;
					bool isPosX = false, isPosY = false, isLessThan = false;
					for (int i = 0; i < splits.Length; i++) {
						isPosX = splits[i].EndsWith("x", StringComparison.OrdinalIgnoreCase);
						isPosY = splits[i].EndsWith("y", StringComparison.OrdinalIgnoreCase);
						isLessThan = splits[i].StartsWith("<", StringComparison.OrdinalIgnoreCase);
						if (((isPosX || isPosY) && float.TryParse(splits[i].Substring(isLessThan ? 1 : 0, splits[i].Length - (isLessThan ? 2 : 1)), out pickupsPos)) || (!isPosX && !isPosY && float.TryParse(splits[i], out pickupsPos))) {
							break;
						}
					}

					bool isNotCoop = Math.Abs(mem.GetLastXP3()) < 0.01f;
					shouldSplit = (!isPosX && !isPosY && (int)pickupsPos > 0 && mem.GetCurrentScore() == (int)pickupsPos)
								|| (isPosX && (isLessThan ? mem.GetLastXP1() < pickupsPos || mem.GetLastXP2() < pickupsPos || (!isNotCoop && (mem.GetLastXP3() < pickupsPos || mem.GetLastXP4() < pickupsPos))
									: mem.GetLastXP1() > pickupsPos || mem.GetLastXP2() > pickupsPos || (!isNotCoop && (mem.GetLastXP3() > pickupsPos || mem.GetLastXP4() > pickupsPos))))
								|| (isPosY && (isLessThan ? mem.GetLastYP1() < pickupsPos || mem.GetLastYP2() < pickupsPos || (!isNotCoop && (mem.GetLastYP3() < pickupsPos || mem.GetLastYP4() < pickupsPos))
									: mem.GetLastYP1() > pickupsPos || mem.GetLastYP2() > pickupsPos || (!isNotCoop && (mem.GetLastYP3() > pickupsPos || mem.GetLastYP4() > pickupsPos))));
					if (shouldSplit) {
						lastLevelComplete++;
						splitFrameCount = mem.FrameCount();
					}
				} else if (currentSplit == Model.CurrentState.Run.Count) {
					PersistentLevelStats level = mem.GetLevelStats(mem.GetPlatformLevelId());
					shouldSplit = level != null && level.minMillisecondsForMaxScore != int.MaxValue;
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
					shouldSplit = screen == MenuScreen.Loading && prev == MenuScreen.SinglePlayerMap && lastMenu != MenuScreen.Loading && (23 - mem.SinglePlayerIndex()) == currentSplit;
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

			HandleSplit(shouldSplit, screen);
		}
		private void HandleDarkVoid(MenuScreen screen) {
			bool shouldSplit = false;

			if (currentSplit == 0) {
				shouldSplit = screen == MenuScreen.SinglePlayerPathSelect && mem.GetPlayingCinematic();
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				MenuScreen prev = mem.GetPreviousMenu();
				if (currentSplit < 10) {
					shouldSplit = screen == MenuScreen.Loading && prev == MenuScreen.SinglePlayerDLCMap && lastMenu != MenuScreen.Loading && (9 - mem.SinglePlayerDVIndex()) == currentSplit;
				} else if (currentSplit == 10) {
					shouldSplit = mem.LevelComplete();
				}
			}

			HandleSplit(shouldSplit, screen);
		}
		private void HandleJourneyCoop(MenuScreen screen) {
			bool shouldSplit = false;

			if (currentSplit == 0) {
				shouldSplit = screen == MenuScreen.CoopPathSelect && mem.GetPlayingCinematic();
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				MenuScreen prev = mem.GetPreviousMenu();
				if (currentSplit < 10) {
					shouldSplit = screen == MenuScreen.Loading && prev == MenuScreen.CoopMap && lastMenu != MenuScreen.Loading && (9 - mem.CoopIndex()) == currentSplit;
				} else if (currentSplit == 10) {
					shouldSplit = mem.LevelComplete();
				}
			}

			HandleSplit(shouldSplit, screen);
		}
		private void HandleDarkVoidCoop(MenuScreen screen) {
			bool shouldSplit = false;

			if (currentSplit == 0) {
				shouldSplit = screen == MenuScreen.CoopPathSelect && mem.GetPlayingCinematic();
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				MenuScreen prev = mem.GetPreviousMenu();
				if (currentSplit < 10) {
					shouldSplit = screen == MenuScreen.Loading && prev == MenuScreen.CoopDLCMap && lastMenu != MenuScreen.Loading && (9 - mem.CoopDVIndex()) == currentSplit;
				} else if (currentSplit == 10) {
					shouldSplit = mem.LevelComplete();
				}
			}

			HandleSplit(shouldSplit, screen);
		}
		private void HandleSplit(bool shouldSplit, MenuScreen screen, bool shouldReset = false) {
			if (currentSplit > 0 && (screen == MenuScreen.MainMenu || shouldReset)) {
				Model.Reset();
			} else if (shouldSplit && DateTime.Now > lastSplit.AddSeconds(1)) {
				lastSplit = DateTime.Now;
				state = 0;
				if (currentSplit == 0) {
					currentSplit++;
					Model.Start();
				} else {
					currentSplit++;
					Model.Split();
				}
			}
		}
		private void HandleGameTimes(MenuScreen screen) {
			if (Model.CurrentState.IsGameTimePaused && screen == MenuScreen.InGame && !mem.GetFrozen()) {
				Model.CurrentState.IsGameTimePaused = false;
			}

			if (screen == MenuScreen.SinglePlayerEndLevelFeedBack && currentSplit > 0 && currentSplit != lastLevelComplete && lastLevelComplete < Model.CurrentState.Run.Count) {
				PersistentLevelStats level = mem.GetLevelStats(mem.GetPlatformLevelId());
				if (level.minMillisecondsForMaxScore != int.MaxValue) {
					double levelTime = (double)level.minMillisecondsForMaxScore / (double)1000;
					levelTimes[lastLevelComplete] = levelTime;
					double totalLevelTime = 0;
					for (int i = 0; i < Model.CurrentState.Run.Count; i++) {
						totalLevelTime += levelTimes[i];
					}
					Model.CurrentState.IsGameTimePaused = true;
					if (currentSplit == Model.CurrentState.Run.Count + 1) {
						Time t = Model.CurrentState.Run[lastLevelComplete].SplitTime;
						Model.CurrentState.Run[lastLevelComplete].SplitTime = new Time(Model.CurrentState.Run.Count < 10 ? TimeSpan.FromSeconds(totalLevelTime) : t.RealTime, TimeSpan.FromSeconds(totalLevelTime));
					} else {
						Model.CurrentState.SetGameTime(TimeSpan.FromSeconds(totalLevelTime));
					}
					lastLevelComplete++;
					WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": Set game time " + levelTime + " " + totalLevelTime);
				}
			}
		}
		public bool IsNotRunning() {
			return Model == null || Model.CurrentState.CurrentPhase != TimerPhase.Running;
		}
#else
		public bool IsNotRunning() {
			return true;
		}
#endif
		private void LogValues(MenuScreen screen) {
			if (lastLogCheck == 0) {
				hasLog = File.Exists("_Kalimba.log");
				lastLogCheck = 300;
			}
			lastLogCheck--;

			if (hasLog || !Console.IsOutputRedirected) {
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
						case "Score": curr = mem.GetCurrentScore().ToString(); break;
						case "Deaths": curr = mem.GetCurrentDeaths().ToString(); break;
						case "CurrentSplit": curr = currentSplit.ToString(); break;
						case "State": curr = state.ToString(); break;
						case "EndLevel": curr = mem.LevelComplete().ToString(); break;
						case "PlatformLevel": curr = mem.GetPlatformLevelId().ToString(); break;
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
#if LiveSplit
						WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + key + ": ".PadRight(16 - key.Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);
#else
						WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + ": " + key + ": ".PadRight(16 - key.Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);
#endif

						currentValues[key] = curr;
					}
				}
			}
		}
		private void WriteLog(string data) {
			if (hasLog || !Console.IsOutputRedirected) {
				if (Console.IsOutputRedirected) {
					using (StreamWriter wr = new StreamWriter("_Kalimba.log", true)) {
						wr.WriteLine(data);
					}
				} else {
					Console.WriteLine(data);
				}
			}
		}

#if LiveSplit
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
			for (int i = 0; i < levelTimes.Length; i++) {
				levelTimes[i] = 0;
			}
			lastYP2 = 0;
			lastSplit = DateTime.MinValue;
			startFrameCount = 0;
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
			Model.CurrentState.IsGameTimePaused = true;
			WriteLog("---------New Game-------------------------------");
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			currentSplit--;
			lastLevelComplete--;
			state = 0;
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			currentSplit++;
			lastLevelComplete++;
			state = 0;
		}
		public void OnSplit(object sender, EventArgs e) {
			Model.CurrentState.IsGameTimePaused = true;

			if (lastMenu == MenuScreen.InGame && startFrameCount > 0 && currentSplit > 1 && currentSplit - 1 < Model.CurrentState.Run.Count) {
				Time currentTime = Model.CurrentState.Run[lastLevelComplete - 1].SplitTime;
				try {
					TimeSpan total = TimeSpan.FromSeconds((splitFrameCount - startFrameCount) / 60f);
					TimeSpan lastLevel = TimeSpan.FromSeconds(0);
					if (lastLevelComplete > 1) {
						lastLevel = Model.CurrentState.Run[lastLevelComplete - 2].SplitTime.RealTime.Value;
					}
					if ((total - lastLevel).TotalSeconds > 1) {
						Model.CurrentState.Run[lastLevelComplete - 1].SplitTime = new Time(total, total);
						WriteLog(total.TotalSeconds.ToString());
					}
				} catch {
					Model.CurrentState.Run[lastLevelComplete - 1].SplitTime = currentTime;
				}
			}
		}
		public Control GetSettingsControl(LayoutMode mode) { return null; }
		public void SetSettings(XmlNode settings) { }
		public XmlNode GetSettings(XmlDocument document) { return document.CreateElement("Settings"); }
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
#endif
		public void Dispose() {
			Manager.Memory = null;
			Manager.Close();
			Manager.Dispose();
		}
	}
}