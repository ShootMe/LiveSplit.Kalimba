using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
namespace LiveSplit.Kalimba {
	public class KalimbaMemory {
		//These are checked in order, so they should be in reverse release order
		private string[] versions = new string[1] { "v1.0" };
		private Dictionary<string, Dictionary<string, string>> funcPatterns = new Dictionary<string, Dictionary<string, string>>() {
			{"v1.0", new Dictionary<string, string>() {
					{"GlobalGameManager", "558BEC5783EC34C745E4000000008B4508C74034000000008B05????????83EC086A0050E8????????83C41085C0743A8B05????????8B4D0883EC085150|-12"},
					{"MenuManager",       "558BEC53575683EC0C8B05????????83EC086A0050E8????????83C41085C074338B05????????83EC08FF750850E8????????83C41085C0741A83EC0CFF7508E8|-30"},
			}},
		};

		private Dictionary<string, string> versionedFuncPatterns = new Dictionary<string, string>();
		private IntPtr globalGameManager = IntPtr.Zero;
		private IntPtr menuManager = IntPtr.Zero;
		private Process proc;
		private bool isHooked = false;
		private DateTime hookedTime;

		public World GetWorld() {
			return (World)Memory.ReadValue<int>(proc, globalGameManager, 0x14, 0x0c, 0x10, 0x5c);
		}
		public Campaign GetCampaign() {
			return (Campaign)Memory.ReadValue<int>(proc, globalGameManager, 0x14, 0x0c, 0x10, 0x60);
		}
		public MenuScreen GetCurrentMenu() {
			return (MenuScreen)Memory.ReadValue<int>(proc, menuManager, 0x34);
		}
		public MenuScreen GetPreviousMenu() {
			return (MenuScreen)Memory.ReadValue<int>(proc, menuManager, 0x38);
		}
		public bool GetPlayingCinematic() {
			return Memory.ReadValue<bool>(proc, globalGameManager, 0x46);
		}
		public bool GetIsLoadingLevel() {
			return Memory.ReadValue<bool>(proc, globalGameManager, 0x4c);
		}
		public int GetCurrentScore() {
			return Memory.ReadValue<int>(proc, globalGameManager, 0x14, 0x10);
		}
		public int GetCurrentDeaths() {
			return Memory.ReadValue<int>(proc, globalGameManager, 0x14, 0x14);
		}
		public float GetLevelTime() {
			return Memory.ReadValue<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x30, 0x20);
		}
		public string GetLevelName() {
			return GetString(Memory.ReadValue<IntPtr>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x30, 0x14));
		}
		public bool GetIsDisabled() {
			return Memory.ReadValue<int>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x64) == 65793;
		}
		public bool GetIsMoving() {
			return Memory.ReadValue<bool>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x88);
		}
		public float GetXCenter() {
			return Memory.ReadValue<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x30);
		}
		public float GetYCenter() {
			return Memory.ReadValue<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x34);
		}
		public float GetLastXP1() {
			return Memory.ReadValue<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0xd4, 0xd0);// 0x17c);
		}
		public float GetLastYP1() {
			return Memory.ReadValue<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0xd4, 0xd4);// 0x180);
		}
		public float GetLastXP2() {
			return Memory.ReadValue<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0xd4, 0xd0);// 0x17c);
		}
		public float GetLastYP2() {
			return Memory.ReadValue<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0xd4, 0xd4);// 0x180);
		}

		private string GetString(IntPtr address) {
			if (address == IntPtr.Zero) { return string.Empty; }
			int length = Memory.ReadValue<int>(proc, address, 0x8);
			return Encoding.Unicode.GetString(Memory.GetBytes(proc, address + 0x0C, 2 * length));
		}

		public bool HookProcess() {
			if (proc == null || proc.HasExited) {
				Process[] processes = Process.GetProcessesByName("Kalimba");
				if (processes.Length == 0) {
					globalGameManager = IntPtr.Zero;
					menuManager = IntPtr.Zero;
					isHooked = false;
					return isHooked;
				}

				proc = processes[0];
				if (proc.HasExited) {
					globalGameManager = IntPtr.Zero;
					menuManager = IntPtr.Zero;
					isHooked = false;
					return isHooked;
				}

				isHooked = true;
				hookedTime = DateTime.Now;
			}

			if (globalGameManager == IntPtr.Zero) {
				globalGameManager = GetVersionedFunctionPointer("GlobalGameManager");
				if (globalGameManager != IntPtr.Zero) {
					globalGameManager = Memory.ReadValue<IntPtr>(proc, globalGameManager, 0, 0);
				}
			}

			if (menuManager == IntPtr.Zero) {
				menuManager = GetVersionedFunctionPointer("MenuManager");
				if (menuManager != IntPtr.Zero) {
					menuManager = Memory.ReadValue<IntPtr>(proc, menuManager, 0, 0);
				}
			}

			return isHooked;
		}

		public void Dispose() {
			// We want to appropriately dispose of the `Process` that is attached 
			// to the process to avoid being unable to close LiveSplit.
			if (proc != null) this.proc.Dispose();
		}

		public IntPtr GetVersionedFunctionPointer(string name) {
			// If we haven't already worked out what version is needed for this function signature, 
			// then iterate the versions checking each until we get a positive result. Store the
			// version so we don't need to search again in the future, and return the address.
			if (!versionedFuncPatterns.ContainsKey(name)) {
				foreach (string version in this.versions) {
					if (funcPatterns[version].ContainsKey(name)) {
						IntPtr[] addrs = Memory.FindMemorySignatures(proc, funcPatterns[version][name]);
						if (addrs[0] != IntPtr.Zero) {
							versionedFuncPatterns[name] = version;
							return addrs[0];
						}
					}
				}
			} else {
				string version = versionedFuncPatterns[name];
				IntPtr[] addrs = Memory.FindMemorySignatures(proc, funcPatterns[version][name]);
				return addrs[0];
			}

			return IntPtr.Zero;
		}
	}
	public enum MenuScreen {
		MainMenu,
		InGameMenu,
		SinglePlayerMap,
		CoopMap,
		SinglePlayerEndLevelFeedBack,
		CoopEndLevelFeedback,
		InGame,
		Splash,
		Loading,
		None,
		Cheat,
		Controls,
		SinglePlayerDemoSplash,
		CoopDemoSplash,
		CoopLogin,
		Options,
		Challengeroom,
		Cinematic,
		Leaderboard,
		Credits,
		InGameOptions,
		CoopLeaderboard,
		Gamma,
		BetaSplash,
		Overflow,
		OldSchoolWin,
		OldSchoolLoose,
		OldSchoolIntro,
		ShorthandedIntro,
		ShorthandedWin,
		LogoVideo,
		SinglePlayerDLCMap,
		CoopDLCMap,
		SpeedRunIntro,
		LlamaIntro,
		LlamaPlayerSelection,
		LlamaRoundEndFeedback,
		LlamaGameEndFeedback,
		SpeedRunLevelSelect,
		Graphics,
		AcceptGraphics,
		RemapControls,
		Upsell,
		SpeedRunWinScreen,
		SinglePlayerPathSelect,
		CoopPathSelect,
		CinematicsMenu
	}
	public enum Campaign {
		Singleplayer,
		Cooperative
	}
	public enum World {
		None,
		Underground,
		Earth,
		Sky,
		Space,
		Challenge
	}
	public enum PlayerStatus {
		Disabled,
		WaitingForController,
		WaitingForUser,
		WaitingForPersistentUserStats,
		WaitingForControllerMapping,
		Ready
	}
}