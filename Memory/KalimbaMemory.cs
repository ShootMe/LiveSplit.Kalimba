using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
namespace LiveSplit.Kalimba.Memory {
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
			//GlobalGameManager.instance.currentSession.activeSessionHolder.sceneFile.world
			return (World)MemoryReader.Read<int>(proc, globalGameManager, 0x14, 0x0c, 0x10, 0x5c);
		}
		public Campaign GetCampaign() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.sceneFile.campaign
			return (Campaign)MemoryReader.Read<int>(proc, globalGameManager, 0x14, 0x0c, 0x10, 0x60);
		}
		public MenuScreen GetCurrentMenu() {
			//MenuManager.instance._currentMenu
			return (MenuScreen)MemoryReader.Read<int>(proc, menuManager, 0x34);
		}
		public MenuScreen GetPreviousMenu() {
			//MenuManager.instance._previousMenu
			return (MenuScreen)MemoryReader.Read<int>(proc, menuManager, 0x38);
		}
		public bool GetPlayingCinematic() {
			//GlobalGameManager.instance.isPlayingCinematic
			return MemoryReader.Read<bool>(proc, globalGameManager, 0x46);
		}
		public bool GetIsLoadingLevel() {
			//GlobalGameManager.instance.levelIsLoading
			return MemoryReader.Read<bool>(proc, globalGameManager, 0x4c);
		}
		public int GetCurrentScore() {
			//GlobalGameManager.instance.currentSession.currentScore
			return MemoryReader.Read<int>(proc, globalGameManager, 0x14, 0x10);
		}
		public int GetCurrentDeaths() {
			//GlobalGameManager.instance.currentSession.currentDeaths
			return MemoryReader.Read<int>(proc, globalGameManager, 0x14, 0x14);
		}
		public float GetLevelTime() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.levelMetric.completionTime
			return MemoryReader.Read<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x30, 0x20);
		}
		public string GetLevelName() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.levelMetric.levelName
			return GetString(MemoryReader.Read<IntPtr>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x30, 0x14));
		}
		public bool GetIsDisabled() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.(noJump | noSwap | noMove)
			return MemoryReader.Read<int>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x64) == 65793;
		}
		public bool GetIsMoving() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.totemsIsMoving
			return MemoryReader.Read<bool>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x88);
		}
		public float GetXCenter() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].bounceCenter.X
			return MemoryReader.Read<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x30);
		}
		public float GetYCenter() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].bounceCenter.Y
			return MemoryReader.Read<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x34);
		}
		public float GetLastXP1() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[0].animationHandler.lastPos.X
			return MemoryReader.Read<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0xd4, 0xd0);
		}
		public float GetLastYP1() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[0].animationHandler.lastPos.Y
			return MemoryReader.Read<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0xd4, 0xd4);
		}
		public float GetLastXP2() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[1].animationHandler.lastPos.X
			return MemoryReader.Read<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0xd4, 0xd0);
		}
		public float GetLastYP2() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[1].animationHandler.lastPos.Y
			return MemoryReader.Read<float>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0xd4, 0xd4);
		}
		public bool GetFrozen() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[0].frozen
			return MemoryReader.Read<bool>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0x3c);
		}
		public bool GetIsDying() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[0].isDying
			bool p1 = MemoryReader.Read<bool>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0x146);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[1].isDying
			bool p2 = MemoryReader.Read<bool>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0x146);
			return p1 | p2;
		}
		public TotemState GetCurrentStateP1() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[0]._currentState
			return (TotemState)MemoryReader.Read<int>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0x148);
		}
		public TotemState GetCurrentStateP2() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[1]._currentState
			return (TotemState)MemoryReader.Read<int>(proc, globalGameManager, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0x148);
		}
		public bool GetEndLevel() {
			bool frozen = GetFrozen();
			bool isDisabled = GetIsDisabled();
			bool isMoving = GetIsMoving();
			bool isDying = GetIsDying();
			MenuScreen currentMenu = GetCurrentMenu();

			return frozen && !isDying && isMoving && !isDisabled && currentMenu == MenuScreen.InGame;
		}

		private string GetString(IntPtr address) {
			if (address == IntPtr.Zero) { return string.Empty; }
			int length = MemoryReader.Read<int>(proc, address, 0x8);
			return Encoding.Unicode.GetString(MemoryReader.GetBytes(proc, address + 0x0C, 2 * length));
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
					globalGameManager = MemoryReader.Read<IntPtr>(proc, globalGameManager, 0, 0);
				}
			}

			if (menuManager == IntPtr.Zero) {
				menuManager = GetVersionedFunctionPointer("MenuManager");
				if (menuManager != IntPtr.Zero) {
					menuManager = MemoryReader.Read<IntPtr>(proc, menuManager, 0, 0);
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
						IntPtr[] addrs = MemoryReader.FindSignatures(proc, funcPatterns[version][name]);
						if (addrs[0] != IntPtr.Zero) {
							versionedFuncPatterns[name] = version;
							return addrs[0];
						}
					}
				}
			} else {
				string version = versionedFuncPatterns[name];
				IntPtr[] addrs = MemoryReader.FindSignatures(proc, funcPatterns[version][name]);
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
	public enum TotemState {
		IDLE,
		WALKING,
		JUMP_UP,
		JUMP_DOWN,
		PASSIVE,
		TUMPLING,
		ON_SLOPE,
		ON_ICE,
		DEAD,
		IN_CANNON
	}
}