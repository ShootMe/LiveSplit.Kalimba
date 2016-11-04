using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
namespace LiveSplit.Kalimba.Memory {
	public partial class KalimbaMemory {
		private ProgramPointer globalGameManager, menuManager, totemPole, platformManager, ghostManager, levelComplete, musicMachine;
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked;

		public KalimbaMemory() {
			globalGameManager = new ProgramPointer(this, "GlobalGameManager");
			menuManager = new ProgramPointer(this, "MenuManager");
			totemPole = new ProgramPointer(this, "TotemPole");
			platformManager = new ProgramPointer(this, "PlatformManager");
			ghostManager = new ProgramPointer(this, "GhostManager");
			levelComplete = new ProgramPointer(this, "LevelComplete") { IsStatic = false };
			musicMachine = new ProgramPointer(this, "MusicMachine");
			lastHooked = DateTime.MinValue;
		}

		public void SetMusicVolume(float volume) {
			musicMachine.Write<float>(volume, 0x1c, 0x24);
			musicMachine.Write<float>(volume, 0x20, 0x24);
		}
		public bool LevelComplete() {
			return levelComplete.Read<bool>() && (MenuScreen)menuManager.Read<int>(0x34) == MenuScreen.InGame;
		}
		public void ZoomOut() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.cameraController._currentZone.cameraSettings.size
			globalGameManager.Write<float>(130, 0x14, 0x0c, 0x1c, 0x4c, 0x1c, 0x24);
		}
		public float Zoom() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.cameraController._currentZone.cameraSettings.size
			return globalGameManager.Read<float>(0x14, 0x0c, 0x1c, 0x4c, 0x1c, 0x24);
		}
		public int FrameCount() {
			return Program.Read<int>(Program.MainModule.BaseAddress, 0xa1e97c);
		}
		public PlatformLevelId SelectedLevel() {
			//TotemWorldMap.instance.levelInfo.sceneFile.platformLevelId
			return (PlatformLevelId)totemPole.Read<int>(0x28, 0x8c, 0x80);
		}
		public int SinglePlayerIndex() {
			//TotemWorldMap.instance.singleplayerTotemPole.menu.selectedIndex
			return totemPole.Read<int>(0x18, 0x40, 0x44);
		}
		public int CoopIndex() {
			//TotemWorldMap.instance.multiplayerTotemPole.menu.selectedIndex
			return totemPole.Read<int>(0x1c, 0x40, 0x44);
		}
		public int SinglePlayerDVIndex() {
			//TotemWorldMap.instance.singleplayerDLCTotemPole.menu.selectedIndex
			return totemPole.Read<int>(0x20, 0x40, 0x44);
		}
		public int CoopDVIndex() {
			//TotemWorldMap.instance.multiplayerDLCTotemPole.menu.selectedIndex
			return totemPole.Read<int>(0x24, 0x40, 0x44);
		}
		public void FixSpeedrun() {
			ghostManager.Write<bool>(true, 0x24);
		}
		public bool SpeedrunLoaded() {
			return ghostManager.Read<bool>(0x24);
		}
		public void PassthroughPickups(bool passthrough) {
			List<IntPtr> pickups = Program.FindAllSignatures("000000000000000000????000000A040????????????????00000000000000000000003f");
			for (int i = 0; i < pickups.Count; i++) {
				IntPtr pickup = pickups[i] - 0x4c;
				Program.Write<bool>(pickup, passthrough, 0x31);
			}
		}
		public int GetCheckpointCount() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.checkpointManager.checkPoints.Length
			return globalGameManager.Read<int>(0x14, 0x0c, 0x14, 0x18, 0x0c);
		}
		public void SetCheckpoint(int num) {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.checkpointManager.checkPoints.Length
			int cpCount = GetCheckpointCount();
			if (num >= cpCount) { num = cpCount - 1; }
			if (num < 0) { num = 0; }
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].reachedCheckpoint
			globalGameManager.Write<int>(num, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x18);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].currentCheckpoint
			globalGameManager.Write<int>(num, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x1c);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].reachedCheckpoint
			globalGameManager.Write<int>(num, 0x14, 0x0c, 0x18, 0x28, 0x14, 0x18);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].currentCheckpoint
			globalGameManager.Write<int>(num, 0x14, 0x0c, 0x18, 0x28, 0x14, 0x1c);
		}
		public int GetCurrentCheckpoint() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].currentCheckpoint
			int rCp = globalGameManager.Read<int>(0x14, 0x0c, 0x18, 0x28, 0x10, 0x1c);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].currentCheckpoint
			int cCp = globalGameManager.Read<int>(0x14, 0x0c, 0x18, 0x28, 0x14, 0x1c);
			return rCp > cCp ? rCp : cCp;
		}
		public PlatformLevelId GetPlatformLevelId() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.sceneFile.platformLevelId
			return (PlatformLevelId)globalGameManager.Read<int>(0x14, 0x0c, 0x10, 0x80);
		}
		public World GetWorld() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.sceneFile.world
			return (World)globalGameManager.Read<int>(0x14, 0x0c, 0x10, 0x5c);
		}
		public Campaign GetCampaign() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.sceneFile.campaign
			return (Campaign)globalGameManager.Read<int>(0x14, 0x0c, 0x10, 0x60);
		}
		public MenuScreen GetCurrentMenu() {
			//MenuManager.instance._currentMenu
			return (MenuScreen)menuManager.Read<int>(0x34);
		}
		public MenuScreen GetPreviousMenu() {
			//MenuManager.instance._previousMenu
			return (MenuScreen)menuManager.Read<int>(0x38);
		}
		public bool GetPlayingCinematic() {
			//GlobalGameManager.instance.isPlayingCinematic
			return globalGameManager.Read<bool>(0x46);
		}
		public bool GetIsLoadingLevel() {
			//GlobalGameManager.instance.levelIsLoading
			return globalGameManager.Read<bool>(0x4c);
		}
		public int GetCurrentScore() {
			//GlobalGameManager.instance.currentSession.currentScore
			return globalGameManager.Read<int>(0x14, 0x10);
		}
		public void SetCurrentScore(int score) {
			//GlobalGameManager.instance.currentSession.currentScore
			globalGameManager.Write<int>(score, 0x14, 0x10);
		}
		public int GetCurrentDeaths() {
			//GlobalGameManager.instance.currentSession.currentDeaths
			return globalGameManager.Read<int>(0x14, 0x14);
		}
		public void SetCurrentDeaths(int deaths) {
			//GlobalGameManager.instance.currentSession.currentDeaths
			globalGameManager.Write<int>(deaths, 0x14, 0x14);
		}
		public float GetLevelTime() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.levelMetric.completionTime
			return globalGameManager.Read<float>(0x14, 0x0c, 0x18, 0x30, 0x20);
		}
		public string GetLevelName() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.levelMetric.levelName
			return globalGameManager.ReadString(0x14, 0x0c, 0x18, 0x30, 0x14);
		}
		public bool GetIsDisabled() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.(noJump | noSwap | noMove)
			return globalGameManager.Read<int>(0x14, 0x0c, 0x18, 0x64) == 65793;
		}
		public float GetLastXP1() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[0].animationHandler.lastPos.X
			return globalGameManager.Read<float>(0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0xd4, 0xd0);
		}
		public float GetLastYP1() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[0].animationHandler.lastPos.Y
			return globalGameManager.Read<float>(0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0xd4, 0xd4);
		}
		public float GetLastXP2() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[1].animationHandler.lastPos.X
			return globalGameManager.Read<float>(0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0xd4, 0xd0);
		}
		public float GetLastYP2() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[1].animationHandler.lastPos.Y
			return globalGameManager.Read<float>(0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0xd4, 0xd4);
		}
		public bool GetFrozen() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[0].frozen
			return globalGameManager.Read<bool>(0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0x3c);
		}
		public PersistentLevelStats GetLevelStats(PlatformLevelId id) {
			//PlatformManager.instance.imp.players[0].gameSinglePlayerStats._levels
			IntPtr levels = platformManager.Read<IntPtr>(0x10, 0x48, 0x10, 0x24, 0x0c);
			PersistentLevelStats level = GetLevelStats(levels, id);
			if (level == null) {
				//PlatformManager.instance.imp.players[0].platformStats._coop["guest"]._levels
				levels = platformManager.Read<IntPtr>(0x10, 0x48, 0x10, 0x34, 0x1c, 0x14, 0x10, 0x0c);
				return GetLevelStats(levels, id);
			}
			return level;
		}
		private PersistentLevelStats GetLevelStats(IntPtr levels, PlatformLevelId id) {
			int listSize = Program.Read<int>(levels, 0x20);
			IntPtr keys = Program.Read<IntPtr>(levels, 0x10);
			levels = Program.Read<IntPtr>(levels, 0x14);

			for (int i = 0; i < listSize; i++) {
				IntPtr itemHead = Program.Read<IntPtr>(levels, 0x10 + (i * 4));
				PlatformLevelId levelID = (PlatformLevelId)Program.Read<int>(keys, 0x10 + (i * 4));

				if (levelID == id) {
					PersistentLevelStats level = new PersistentLevelStats();
					level.id = levelID;
					level.awardedGoldenTotemPiece = Program.Read<bool>(itemHead, 0x20);
					level.maxScore = Program.Read<int>(itemHead, 0x0c);
					level.minDeaths = Program.Read<int>(itemHead, 0x18);
					level.minKills = Program.Read<int>(itemHead, 0x1c);
					level.minMillisecondsForMaxScore = Program.Read<int>(itemHead, 0x10);
					level.minPickups = Program.Read<int>(itemHead, 0x14);
					level.state = (PersistentLevelStats.State)Program.Read<int>(itemHead, 0x08);
					return level;
				}
			}
			return null;
		}
		public void SetLevelScore(PlatformLevelId id, int score) {
			//PlatformManager.instance.imp.players[0].gameSinglePlayerStats._levels
			IntPtr levels = platformManager.Read<IntPtr>(0x10, 0x48, 0x10, 0x24, 0x0c);
			SetScore(levels, id, score);
			//PlatformManager.instance.imp.players[0].platformStats._coop["guest"]._levels
			levels = platformManager.Read<IntPtr>(0x10, 0x48, 0x10, 0x34, 0x1c, 0x14, 0x10, 0x0c);
			SetScore(levels, id, score);
		}
		private void SetScore(IntPtr levels, PlatformLevelId id, int score) {
			int listSize = Program.Read<int>(levels, 0x20);
			IntPtr keys = Program.Read<IntPtr>(levels, 0x10);
			levels = Program.Read<IntPtr>(levels, 0x14);

			for (int i = 0; i < listSize; i++) {
				IntPtr itemHead = Program.Read<IntPtr>(levels, 0x10 + (i * 4));
				PlatformLevelId levelID = (PlatformLevelId)Program.Read<int>(keys, 0x10 + (i * 4));

				if (levelID == id || id == PlatformLevelId.None) {
					Program.Write<int>(itemHead, score, 0x0c);
					Program.Write<int>(itemHead, int.MaxValue, 0x10);
					Program.Write<int>(itemHead, (int)PersistentLevelStats.State.Completed, 0x08);
				}
			}
		}
		public void EraseData() {
			//PlatformManager.instance.imp.players[0].gameSinglePlayerStats._rememberedMoments.Count
			platformManager.Write<int>(0, 0x10, 0x48, 0x10, 0x24, 0x08, 0x0c);
			//PlatformManager.instance.imp.players[0].gameSinglePlayerStats._levels
			ClearStats(platformManager.Read<IntPtr>(0x10, 0x48, 0x10, 0x24, 0x0c));
			//PlatformManager.instance.imp.players[0].platformStats._coop["guest"]._levels
			IntPtr coopDic = platformManager.Read<IntPtr>(0x10, 0x48, 0x10, 0x34, 0x1c, 0x14, 0x10);
			//PlatformManager.instance.imp.players[0].platformStats._coop["guest"]._rememberedMoments.Count
			Program.Write<int>(coopDic, 0, 0x08, 0x0c);
			ClearStats(Program.Read<IntPtr>(coopDic, 0x0c));
		}
		private void ClearStats(IntPtr levels) {
			int listSize = Program.Read<int>(levels, 0x20);
			levels = Program.Read<IntPtr>(levels, 0x14);

			for (int i = 0; i < listSize; i++) {
				IntPtr itemHead = Program.Read<IntPtr>(levels, 0x10 + (i * 4));

				Program.Write<long>(itemHead, 0L, 0x08);
				Program.Write<int>(itemHead, int.MaxValue, 0x10);
				Program.Write<long>(itemHead, 0L, 0x14);
				Program.Write<long>(itemHead, 0L, 0x1c);
				Program.Write<long>(itemHead, 0L, 0x24);
				Program.Write<int>(itemHead, int.MaxValue, 0x2c);
				Program.Write<long>(itemHead, 0L, 0x30);
				Program.Write<short>(itemHead, 0, 0x38);
			}
		}
		public bool HookProcess() {
			if ((Program == null || Program.HasExited) && DateTime.Now > lastHooked.AddSeconds(1)) {
				lastHooked = DateTime.Now;
				Process[] processes = Process.GetProcessesByName("Kalimba");
				Program = processes.Length == 0 ? null : processes[0];
				IsHooked = true;
			}

			if (Program == null || Program.HasExited) {
				IsHooked = false;
			}

			return IsHooked;
		}
		public void Dispose() {
			if (Program != null) {
				Program.Dispose();
			}
		}
	}
	public class ProgramPointer {
		private static string[] versions = new string[1] { "v1.0" };
		private static Dictionary<string, Dictionary<string, string>> funcPatterns = new Dictionary<string, Dictionary<string, string>>() {
			{"v1.0", new Dictionary<string, string>() {
					{"GlobalGameManager", "558BEC5783EC34C745E4000000008B4508C74034000000008B05????????83EC086A0050E8????????83C41085C0743A8B05????????8B4D0883EC085150|-12"},
					{"MenuManager",       "558BEC53575683EC0C8B05????????83EC086A0050E8????????83C41085C074338B05????????83EC08FF750850E8????????83C41085C0741A83EC0CFF7508E8|-30"},
					{"PlatformManager",   "558BEC535683EC108B05????????83EC0C50E8????????83C41085C0740B8B05"},
					{"TotemPole",         "D95810D94510D958148B4D1489480CC9C3000000558BEC83EC08B8????????8B4D088908C9C3000000000000558BEC5683EC0483EC0C|-27"},
					{"GhostManager",      "EC5783EC148B7D088B05????????83EC0C503900E8????????83C41083EC086A01503900E8????????83C410C647240083EC0C68????????E8????????83C41083EC0C8945F450E8????????83C4108B45F489471883EC0C|-78" },
					{"LevelComplete",     "558BEC5783EC648B7D0883EC0C57E8????????83C410B8????????C60000D9EED99F????????8B474083EC086A0050E8????????83C41085C0743083EC0C57|-40" },
					{"MusicMachine",      "558BEC575683EC108B75088B7D0C83FF060F85????????8B05????????83EC0C503900E8????????83C4108945F48B45F43D????????74268B05" }
			}},
		};
		private IntPtr pointer;
		public KalimbaMemory Memory { get; set; }
		public string Name { get; set; }
		public bool IsStatic { get; set; }
		private int lastID;
		private DateTime lastTry;
		public ProgramPointer(KalimbaMemory memory, string name) {
			this.Memory = memory;
			this.Name = name;
			this.IsStatic = true;
			lastID = memory.Program == null ? -1 : memory.Program.Id;
			lastTry = DateTime.MinValue;
		}

		public IntPtr Value {
			get {
				if (!Memory.IsHooked) {
					pointer = IntPtr.Zero;
				} else {
					GetPointer(ref pointer, Name);
				}
				return pointer;
			}
		}
		public T Read<T>(params int[] offsets) {
			if (!Memory.IsHooked) { return default(T); }
			return Memory.Program.Read<T>(Value, offsets);
		}
		public string ReadString(params int[] offsets) {
			if (!Memory.IsHooked) { return string.Empty; }
			IntPtr p = Memory.Program.Read<IntPtr>(Value, offsets);
			return Memory.Program.GetString(p);
		}
		public void Write<T>(T value, params int[] offsets) {
			if (!Memory.IsHooked) { return; }
			Memory.Program.Write<T>(Value, value, offsets);
		}
		private void GetPointer(ref IntPtr ptr, string name) {
			if (Memory.IsHooked) {
				if (Memory.Program.Id != lastID) {
					ptr = IntPtr.Zero;
					lastID = Memory.Program.Id;
				}
				if (ptr == IntPtr.Zero && DateTime.Now > lastTry.AddSeconds(1)) {
					lastTry = DateTime.Now;
					ptr = GetVersionedFunctionPointer(name);
					if (ptr != IntPtr.Zero) {
						if (IsStatic) {
							ptr = Memory.Program.Read<IntPtr>(ptr, 0, 0);
						} else {
							ptr = Memory.Program.Read<IntPtr>(ptr, 0);
						}
					}
				}
			}
		}
		public IntPtr GetVersionedFunctionPointer(string name) {
			foreach (string version in versions) {
				if (funcPatterns[version].ContainsKey(name)) {
					return Memory.Program.FindSignatures(funcPatterns[version][name])[0];
				}
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