using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.Kalimba {
#if LiveSplit
	public class KalimbaComponent : IComponent {
		private TimerModel Model { get; set; }
		public string ComponentName { get { return "Kalimba Autosplitter"; } }
		private float lastYP2;
		private MenuScreen mainMenu = MenuScreen.MainMenu;
		private int lastLevelComplete = 0;
		private double startGameTime, splitGameTime;
		private bool lastDisabled = false;
		private RaceWatcher raceWatcher = new RaceWatcher();
		private ILSplitInfo ilSplitInfo;
#else
	public class KalimbaComponent {
#endif
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		private static string LOGFILE = "_Kalimba.txt";
		private MemoryManager mem;
		private int currentSplit = 0, state = 0, lastLogCheck = 0;
		private bool hasLog = false;
		private MenuScreen lastMenu = MenuScreen.MainMenu;
		private Dictionary<LogObject, string> currentValues = new Dictionary<LogObject, string>();
		public KalimbaManager Manager { get; set; }
		private Thread updateLoop;
		private DateTime lastSplit = DateTime.MinValue;
		private double[] levelTimes = new double[100];

#if LiveSplit
		public KalimbaComponent(LiveSplitState state, bool shown = false) {
#else
		public KalimbaComponent(object state, bool shown = false) {
#endif
			mem = new MemoryManager();
			foreach (LogObject key in Enum.GetValues(typeof(LogObject))) {
				currentValues[key] = "";
			}
#if LiveSplit
			if (state != null) {
				Model = new TimerModel() { CurrentState = state };
				Model.InitializeGameTime();
				Model.CurrentState.IsGameTimePaused = true;
				state.OnReset += OnReset;
				state.OnPause += OnPause;
				state.OnResume += OnResume;
				state.OnStart += OnStart;
				state.OnSplit += OnSplit;
				state.OnUndoSplit += OnUndoSplit;
				state.OnSkipSplit += OnSkipSplit;

				updateLoop = new Thread(UpdateLoop);
				updateLoop.IsBackground = true;
				updateLoop.Start();
			}
#endif
			Manager = new KalimbaManager(shown);
			Manager.Memory = mem;
			Manager.Component = this;
			Manager.Show();
			Manager.Visible = shown;
		}
		private void UpdateLoop() {
			while (updateLoop != null) {
				try {
					GetValues();
				} catch (Exception ex) {
					WriteLog(ex.ToString());
				}
				Thread.Sleep(8);
			}
		}
		public void GetValues() {
			if (!mem.HookProcess()) { return; }

			MenuScreen screen = mem.GetCurrentMenu();
#if LiveSplit
			PlatformLevelId levelID = mem.GetPlatformLevelId();
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

				if (Model.CurrentState.IsGameTimePaused) {
					if (screen == MenuScreen.InGame && !mem.GetFrozen()) {
						Model.CurrentState.IsGameTimePaused = false;
					}
				} else if (screen != MenuScreen.InGame) {
					Model.CurrentState.IsGameTimePaused = true;
				} else if (currentSplit > 0) {
					PersistentLevelStats level = mem.GetLevelStats(levelID);
					if (level != null && level.minMillisecondsForMaxScore != int.MaxValue) {
						Model.CurrentState.IsGameTimePaused = true;
					}
				}

				if (currentSplit == Model.CurrentState.Run.Count + 1 && Model.CurrentState.Run.Count >= 10) {
					HandleGameTimes();
				}
			}

			raceWatcher.UpdateRace(screen == MenuScreen.InGame || screen == MenuScreen.InGameMenu, levelID, mem.GetCurrentCheckpoint(), mem.LevelComplete(screen));
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
					startGameTime = mem.GameTime();
				}
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				if (currentSplit < Model.CurrentState.Run.Count) {
					if (ilSplitInfo == null) {
						ilSplitInfo = new ILSplitInfo(Model.CurrentState.CurrentSplit.Name);
					}

					if (screen == MenuScreen.InGame) {
						if (ilSplitInfo.IsPosX || ilSplitInfo.IsPosY) {
							float pos = ilSplitInfo.Value;
							Vector2 p1 = mem.GetLastP1();
							Vector2 p2 = mem.GetLastP2();
							Vector2 p3 = mem.GetLastP3();
							Vector2 p4 = mem.GetLastP4();
							float p1Val = ilSplitInfo.IsPosX ? p1.X : p1.Y;
							float p2Val = ilSplitInfo.IsPosX ? p2.X : p2.Y;
							float p3Val = ilSplitInfo.IsPosX ? p3.X : p3.Y;
							float p4Val = ilSplitInfo.IsPosX ? p4.X : p4.Y;
							bool isCoop = Math.Abs(p3.X) >= 0.01f;
							shouldSplit = p1Val > pos || p2Val > pos || (isCoop && (p3Val > pos || p4Val > pos));
						} else if (ilSplitInfo.IsCP) {
							shouldSplit = (int)ilSplitInfo.Value > 0 && mem.GetCurrentCheckpoint() + 1 == (int)ilSplitInfo.Value;
						} else if (ilSplitInfo.IsPickup) {
							shouldSplit = (int)ilSplitInfo.Value > 0 && mem.GetCurrentScore() == (int)ilSplitInfo.Value;
						} else if (ilSplitInfo.IsBoss) {
							shouldSplit = mem.GetBossState().IndexOf(ilSplitInfo.BossText, StringComparison.OrdinalIgnoreCase) >= 0;
						} else if (ilSplitInfo.IsDis) {
							bool disabled = mem.GetIsDisabled();
							shouldSplit = disabled && !lastDisabled;
							lastDisabled = disabled;
						}
					}
				} else if (currentSplit == Model.CurrentState.Run.Count) {
					PersistentLevelStats level = mem.GetLevelStats(mem.GetPlatformLevelId());
					shouldSplit = level != null && level.minMillisecondsForMaxScore != int.MaxValue;
				}
			}

			HandleSplit(shouldSplit, screen == MenuScreen.SinglePlayerMap || screen == MenuScreen.SinglePlayerDLCMap || screen == MenuScreen.CoopMap || screen == MenuScreen.CoopDLCMap);
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
					Vector2 p2 = mem.GetLastP2();
					if (state < 6) {
						if ((state & 1) == 0 ? disabled : !disabled) {
							state++;
						}
					} else {
						shouldSplit = p2.Y < -211 && lastYP2 < p2.Y;
					}
					lastYP2 = p2.Y;
				}
			}

			HandleSplit(shouldSplit, false);
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
					shouldSplit = mem.LevelComplete(screen);
				}
			}

			HandleSplit(shouldSplit, false);
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
					shouldSplit = mem.LevelComplete(screen);
				}
			}

			HandleSplit(shouldSplit, false);
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
					shouldSplit = mem.LevelComplete(screen);
				}
			}

			HandleSplit(shouldSplit, false);
		}
		private void HandleSplit(bool shouldSplit, bool shouldReset = false) {
			if (currentSplit > 0 && shouldReset) {
				currentSplit = 0;
				Model.Reset();
			} else if (shouldSplit && DateTime.Now > lastSplit.AddSeconds(1)) {
				lastSplit = DateTime.Now;
				if (currentSplit == 0) {
					Model.Start();
				} else {
					Model.Split();
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
				string prev = string.Empty, curr = string.Empty;
				foreach (LogObject key in Enum.GetValues(typeof(LogObject))) {
					prev = currentValues[key];

					switch (key) {
						case LogObject.World: curr = mem.GetWorld().ToString(); break;
						case LogObject.Campaign: curr = mem.GetCampaign().ToString(); break;
						case LogObject.CurrentMenu: curr = mem.GetCurrentMenu().ToString(); break;
						case LogObject.PreviousMenu: curr = mem.GetPreviousMenu().ToString(); break;
						case LogObject.Cinematic: curr = mem.GetPlayingCinematic().ToString(); break;
						case LogObject.LoadingLevel: curr = mem.GetIsLoadingLevel().ToString(); break;
						case LogObject.Disabled: curr = mem.GetIsDisabled().ToString(); break;
						case LogObject.Checkpoint: curr = mem.GetCurrentCheckpoint().ToString(); break;
						case LogObject.Deaths: curr = mem.GetCurrentDeaths().ToString(); break;
						case LogObject.CurrentSplit: curr = currentSplit.ToString(); break;
						case LogObject.State: curr = state.ToString(); break;
						case LogObject.EndLevel: curr = mem.LevelComplete(mem.GetCurrentMenu()).ToString(); break;
						case LogObject.PlatformLevel: curr = mem.GetPlatformLevelId().ToString(); break;
						case LogObject.BossState: curr = mem.GetBossState(); break;
						case LogObject.Stats:
							if (screen == MenuScreen.SinglePlayerEndLevelFeedBack) {
								PersistentLevelStats level = mem.GetLevelStats(mem.GetPlatformLevelId());
								curr = level == null ? "" : level.maxScore + " - " + level.minMillisecondsForMaxScore;
							} else {
								curr = "";
							}
							break;
						default: curr = ""; break;
					}

					if (string.IsNullOrEmpty(prev)) { prev = string.Empty; }
					if (string.IsNullOrEmpty(curr)) { curr = string.Empty; }
					if (!prev.Equals(curr)) {
#if LiveSplit
						WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + key.ToString() + ": ".PadRight(16 - key.ToString().Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);
#else
						WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + ": " + key.ToString() + ": ".PadRight(16 - key.ToString().Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);
#endif

						currentValues[key] = curr;
					}
				}
			}
		}
		private void WriteLog(string data) {
			lock (LOGFILE) {
				if (hasLog || !Console.IsOutputRedirected) {
					if (Console.IsOutputRedirected) {
						using (StreamWriter wr = new StreamWriter(LOGFILE, true)) {
							wr.WriteLine(data);
						}
					} else {
						Console.WriteLine(data);
					}
				}
			}
		}
#if LiveSplit
		public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
		}
		public void OnReset(object sender, TimerPhase e) {
			currentSplit = 0;
			lastLevelComplete = 0;
			state = 0;
			for (int i = 0; i < levelTimes.Length; i++) {
				levelTimes[i] = 0;
			}
			ilSplitInfo = null;
			lastYP2 = 0;
			lastSplit = DateTime.MinValue;
			startGameTime = 0;
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
			ilSplitInfo = null;
			state = 0;
			currentSplit++;
			Model.CurrentState.SetGameTime(TimeSpan.Zero);
			Model.CurrentState.IsGameTimePaused = true;
			WriteLog("---------New Game " + Assembly.GetExecutingAssembly().GetName().Version.ToString(3) + "-------------------------");
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			ilSplitInfo = null;
			currentSplit--;
			lastLevelComplete--;
			state = 0;
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			ilSplitInfo = null;
			currentSplit++;
			lastLevelComplete++;
			state = 0;
		}
		public void OnSplit(object sender, EventArgs e) {
			ilSplitInfo = null;
			state = 0;
			currentSplit++;

			HandleGameTimes();
		}
		private void HandleGameTimes() {
			if (startGameTime > 0) {
				splitGameTime = mem.GameTime();

				if (currentSplit > 1 && currentSplit - 1 <= Model.CurrentState.Run.Count) {
					Time currentTime = Model.CurrentState.Run[currentSplit - 2].SplitTime;
					try {
						TimeSpan total;
						if (currentSplit - 1 == Model.CurrentState.Run.Count) {
							PlatformLevelId levelID = mem.GetPlatformLevelId();
							PersistentLevelStats level = mem.GetLevelStats(levelID);
							total = TimeSpan.FromMilliseconds(level.minMillisecondsForMaxScore);
						} else {
							total = TimeSpan.FromSeconds(splitGameTime - startGameTime);
						}

						TimeSpan lastLevel = TimeSpan.FromSeconds(0);
						if (currentSplit > 2) {
							lastLevel = Model.CurrentState.Run[currentSplit - 3].SplitTime.RealTime.Value;
						}
						if ((total - lastLevel).TotalSeconds > 1) {
							Model.CurrentState.Run[currentSplit - 2].SplitTime = new Time(total, total);
							WriteLog(total.TotalSeconds.ToString());
						}
					} catch {
						Model.CurrentState.Run[currentSplit - 2].SplitTime = currentTime;
					}
				}
			} else if (currentSplit > 0 && Model != null && Model.CurrentState != null && Model.CurrentState.Run != null) {
				PlatformLevelId levelID = mem.GetPlatformLevelId();
				PersistentLevelStats level = mem.GetLevelStats(levelID);
				if (level != null && level.minMillisecondsForMaxScore != int.MaxValue && currentSplit == lastLevelComplete + 2) {
					double levelTime = (double)level.minMillisecondsForMaxScore / (double)1000;
					levelTimes[lastLevelComplete] = levelTime;
					double totalLevelTime = 0;
					for (int i = 0; i <= lastLevelComplete; i++) {
						totalLevelTime += levelTimes[i];
					}

					Model.CurrentState.IsGameTimePaused = true;
					Time t = Model.CurrentState.Run[lastLevelComplete].SplitTime;
					Model.CurrentState.Run[lastLevelComplete].SplitTime = new Time(t.RealTime, TimeSpan.FromSeconds(totalLevelTime));

					lastLevelComplete++;
					WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": Set game time " + levelTime + " " + totalLevelTime);
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
			if (updateLoop != null) {
				updateLoop = null;
			}
#if LiveSplit
			if (Model != null) {
				Model.CurrentState.OnReset -= OnReset;
				Model.CurrentState.OnPause -= OnPause;
				Model.CurrentState.OnResume -= OnResume;
				Model.CurrentState.OnStart -= OnStart;
				Model.CurrentState.OnSplit -= OnSplit;
				Model.CurrentState.OnUndoSplit -= OnUndoSplit;
				Model.CurrentState.OnSkipSplit -= OnSkipSplit;
				Model = null;
			}
#endif
		}
	}
	public class ILSplitInfo {
		public string[] NameSplit;
		public float Value = -1;
		public bool IsPosX = false, IsPosY = false, IsCP = false, IsDis = false, IsBoss = false, IsPickup = false;
		public string BossText = null;
		public ILSplitInfo(string splitName) {
			NameSplit = splitName.Split(' ');
			IsBoss = splitName.StartsWith("b ", StringComparison.OrdinalIgnoreCase);

			if (!IsBoss) {
				for (int i = 0; i < NameSplit.Length; i++) {
					string sp = NameSplit[i];
					IsPosX = sp.EndsWith("x", StringComparison.OrdinalIgnoreCase);
					IsPosY = sp.EndsWith("y", StringComparison.OrdinalIgnoreCase);
					IsCP = sp.EndsWith("c", StringComparison.OrdinalIgnoreCase);
					IsDis = sp.Equals("d", StringComparison.OrdinalIgnoreCase);
					if (IsDis || float.TryParse(IsCP || IsPosX || IsPosY ? sp.Substring(0, sp.Length - 1) : sp, out Value)) {
						break;
					}
				}
			} else {
				BossText = splitName.Substring(2);
			}
			IsPickup = !IsPosX && !IsPosY && !IsCP && !IsDis && !IsBoss;
		}
	}
}