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
					{"PlatformManager",   "558BEC535683EC108B05????????83EC0C50E8????????83C41085C0740B8B05"}
			}},
		};

		private Dictionary<string, string> versionedFuncPatterns = new Dictionary<string, string>();
		private IntPtr globalGameManager = IntPtr.Zero;
		private IntPtr menuManager = IntPtr.Zero;
		private IntPtr platformManager = IntPtr.Zero;
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
		public PersistentLevelStats GetLevelStats(PlatformLevelId id) {
			//PlatformManager.instance.imp.players[0].gameSinglePlayerStats._levels
			IntPtr levels = MemoryReader.Read<IntPtr>(proc, platformManager, 0x10, 0x48, 0x10, 0x24, 0x0c);
			int listSize = MemoryReader.Read<int>(proc, levels, 0x20);
			IntPtr keys = MemoryReader.Read<IntPtr>(proc, levels, 0x10);
			levels = MemoryReader.Read<IntPtr>(proc, levels, 0x14);

			for (int i = 0; i < listSize; i++) {
				IntPtr itemHead = MemoryReader.Read<IntPtr>(proc, levels, 0x10 + (i * 4));
				PlatformLevelId levelID = (PlatformLevelId)MemoryReader.Read<int>(proc, keys, 0x10 + (i * 4));

				if (levelID == id) {
					PersistentLevelStats level = new PersistentLevelStats();
					level.id = levelID;
					level.awardedGoldenTotemPiece = MemoryReader.Read<bool>(proc, itemHead, 0x20);
					level.maxScore = MemoryReader.Read<int>(proc, itemHead, 0x0c);
					level.minDeaths = MemoryReader.Read<int>(proc, itemHead, 0x18);
					level.minKills = MemoryReader.Read<int>(proc, itemHead, 0x1c);
					level.minMillisecondsForMaxScore = MemoryReader.Read<int>(proc, itemHead, 0x10);
					level.minPickups = MemoryReader.Read<int>(proc, itemHead, 0x14);
					level.state = (PersistentLevelStats.State)MemoryReader.Read<int>(proc, itemHead, 0x08);
					return level;
				}
			}
			return null;
		}
		public void EraseData() {
			//PlatformManager.instance.imp.players[0].gameSinglePlayerStats._rememberedMoments
			MemoryReader.Write<int>(proc, platformManager, 0, 0x10, 0x48, 0x10, 0x24, 0x08, 0x0c);

			//PlatformManager.instance.imp.players[0].gameSinglePlayerStats._levels
			IntPtr levels = MemoryReader.Read<IntPtr>(proc, platformManager, 0x10, 0x48, 0x10, 0x24, 0x0c);
			int listSize = MemoryReader.Read<int>(proc, levels, 0x20);
			IntPtr keys = MemoryReader.Read<IntPtr>(proc, levels, 0x10);
			levels = MemoryReader.Read<IntPtr>(proc, levels, 0x14);

			for (int i = 0; i < listSize; i++) {
				IntPtr itemHead = MemoryReader.Read<IntPtr>(proc, levels, 0x10 + (i * 4));

				MemoryReader.Write<long>(proc, itemHead, 0L, 0x08);
				MemoryReader.Write<int>(proc, itemHead, int.MaxValue, 0x10);
				MemoryReader.Write<long>(proc, itemHead, 0L, 0x14);
				MemoryReader.Write<long>(proc, itemHead, 0L, 0x1c);
				MemoryReader.Write<long>(proc, itemHead, 0L, 0x24);
				MemoryReader.Write<int>(proc, itemHead, int.MaxValue, 0x2c);
				MemoryReader.Write<long>(proc, itemHead, 0L, 0x30);
				MemoryReader.Write<short>(proc, itemHead, 0, 0x38);
			}
		}
		public bool GetEndLevel() {
			bool frozen = GetFrozen();
			bool isDisabled = GetIsDisabled();
			bool isDying = GetIsDying();
			TotemState state1 = GetCurrentStateP1();
			TotemState state2 = GetCurrentStateP2();
			MenuScreen currentMenu = GetCurrentMenu();

			return frozen && !isDying && (state1 == TotemState.WALKING || state1 == TotemState.JUMP_UP || state2 == TotemState.WALKING || state2 == TotemState.JUMP_UP) && !isDisabled && currentMenu == MenuScreen.InGame;
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

			if (platformManager == IntPtr.Zero) {
				platformManager = GetVersionedFunctionPointer("PlatformManager");
				if (platformManager != IntPtr.Zero) {
					platformManager = MemoryReader.Read<IntPtr>(proc, platformManager, 0, 0);
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
	public enum PlatformLevelId {
		None,
		Underground_Jump,
		Underground_Swap,
		Underground_TotemUp,
		Underground_JumpJump,
		Underground_IceIceTotem,
		Underground_Gravity,
		Underground_UpsideDown,
		Underground_BigSnake,
		Earth_Seekers,
		Earth_Cannons,
		Earth_SizeUp,
		Earth_SeekerAndSize,
		Earth_SizeUp_Gravity,
		Earth_2xAbility,
		Earth_Run,
		Earth_BossTwo,
		Sky_Wings,
		Sky_AirFloat,
		Sky_ColoredEnemies,
		Sky_MovingPlatforms,
		Sky_ColoredSeekers,
		Sky_WingsAndSize,
		Sky_Launch,
		Sky_BigBird,
		Coop_Jump = 41,
		Coop_Buttons,
		Coop_TotemUp,
		Coop_TwinSpirits,
		Coop_Hoppeborgen,
		Coop_Seekers,
		Coop_Sloping,
		Coop_ActionJackson,
		Coop_Traps,
		Coop_TheCrumblingBrickRoad,
		Coop_BossWarmUp,
		Coop_EpicBossFight,
		Space_BackTrack = 100,
		Space_Rails,
		Space_ReversedAndMiniBoss,
		Space_ReversedFlying,
		Space_ReversedFlyingOnIce,
		Space_TheWeirdRoom,
		Space_Ice10,
		Space_IceAndCannons,
		Space_Portals01,
		Space_Portals02,
		Space_Portals03,
		Space_ReversedTrampolineFlying,
		Space_Reversex2,
		Space_TrampolineSwaping,
		DLC_Coop_Andrew = 120,
		DLC_Coop_Bob,
		DLC_Coop_Carl,
		DLC_Coop_Dave,
		DLC_Coop_Errol,
		DLC_Coop_Frank,
		DLC_Coop_George,
		DLC_Coop_Harry,
		DLC_Coop_Ian,
		DLC_Coop_Jack,
		DLC_SP_Alice = 150,
		DLC_SP_Bella,
		DLC_SP_Carol,
		DLC_SP_Diana,
		DLC_SP_Eve,
		DLC_SP_Fiona,
		DLC_SP_Gretchen,
		DLC_SP_Hillary,
		DLC_SP_Ilene,
		DLC_SP_Jocelyn,
		Test_Scene_1 = 200
	}
	public class PersistentLevelStats {
		public enum State {
			Unseen,
			Seen,
			Completed
		}
		public PlatformLevelId id;
		public PersistentLevelStats.State state;
		public int maxScore;
		public int minMillisecondsForMaxScore = 2147483647;
		public int minPickups = 2147483647;
		public int minDeaths = 2147483647;
		public int minKills = 2147483647;
		public bool awardedGoldenTotemPiece;
	}
}