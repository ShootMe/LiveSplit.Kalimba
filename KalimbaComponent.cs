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
		protected TimerModel Model { get; set; }
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		private KalimbaMemory mem;
		private int currentSplit = 0;
		private int state = 0;
		private bool hasLog = false;
		private int lastLogCheck = 0;
		private float lastYP2;
		internal static string[] keys = { "CurrentSplit", "World", "Campaign", "CurrentMenu", "PreviousMenu", "Cinematic", "LoadingLevel", "LevelTime", "Disabled", "Score", "Deaths", "LevelName", "Moving", "P1Y", "P2Y", "State", "EndLevel" };
		private Dictionary<string, string> currentValues = new Dictionary<string, string>();

		public KalimbaComponent() {
			mem = new KalimbaMemory();
			foreach (string key in keys) {
				currentValues[key] = "";
			}
		}

		public void GetValues() {
			if (!mem.HookProcess()) {
				if (currentSplit > 0) {
					if (Model != null) { Model.Reset(); }
				}
				return;
			}

			bool shouldSplit = false;
			MenuScreen screen = mem.GetCurrentMenu();

			if (currentSplit == 0) {
				shouldSplit = screen == MenuScreen.SinglePlayerPathSelect && mem.GetPlayingCinematic();
			} else if ((currentSplit < 24 && (Model == null || Model.CurrentState.Run.Count == 24)) || currentSplit < 10) {
				shouldSplit = screen == MenuScreen.Loading && mem.GetPreviousMenu() == MenuScreen.SinglePlayerMap && !currentValues["CurrentMenu"].Equals("Loading", StringComparison.OrdinalIgnoreCase);
				if (shouldSplit && currentSplit == 1 && state == 0) {
					state++;
					shouldSplit = false;
				}
			} else if (currentSplit == 10) {
				shouldSplit = mem.GetEndLevel();
			} else if (currentSplit == 24) {
				if (screen == MenuScreen.Loading && mem.GetPreviousMenu() == MenuScreen.InGame) {
					state = 0;
				}
				bool disabled = mem.GetIsDisabled();
				if (state < 3) {
					if (disabled && currentValues["Disabled"].Equals("False", StringComparison.OrdinalIgnoreCase)) {
						state++;
					}
				} else {
					float p2Y = mem.GetLastYP2();
					shouldSplit = p2Y < -211 && lastYP2 < p2Y;
				}
			}

			if (currentSplit > 0 && screen == MenuScreen.MainMenu) {
				if (Model != null) { Model.Reset(); } else { currentSplit = 0; state = 0; }
			} else if (shouldSplit) {
				if (currentSplit == 0) {
					if (Model != null) { Model.Start(); } else { currentSplit++; state = 0; }
				} else {
					if (Model != null) { Model.Split(); } else { currentSplit++; state = 0; }
				}
			}

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
						case "LevelName": curr = mem.GetLevelName(); break;
						case "Moving": curr = mem.GetIsMoving().ToString(); break;
						//case "P1Y": curr = mem.GetLastYP2().ToString("0"); break;
						case "P2Y": lastYP2 = mem.GetLastYP2(); curr = ""; break;
						case "CurrentSplit": curr = currentSplit.ToString(); break;
						case "State": curr = state.ToString(); break;
						case "EndLevel": curr = mem.GetEndLevel().ToString(); break;
						default: curr = ""; break;
					}

					if (!prev.Equals(curr)) {
						WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + key + ": ".PadRight(16 - key.Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

						currentValues[key] = curr;
					}
				}
			} else {
				currentValues["Disabled"] = mem.GetIsDisabled().ToString();
			}
		}

		public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
			if (Model == null) {
				Model = new TimerModel() { CurrentState = lvstate };
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
			state = 0;
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
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) + ": CurrentSplit: " + currentSplit.ToString().PadLeft(24, ' '));
		}
		private void WriteLog(string data) {
			//Console.WriteLine(data);
			if (hasLog) {
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
		public void Dispose() { }
	}
}