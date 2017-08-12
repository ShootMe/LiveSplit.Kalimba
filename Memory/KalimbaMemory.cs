using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
namespace LiveSplit.Kalimba.Memory {
	public partial class KalimbaMemory {
		private ProgramPointer globalGameManager, menuManager, totemPole, platformManager, ghostManager, levelComplete, musicMachine, tas;
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked;

		public KalimbaMemory() {
			globalGameManager = new ProgramPointer(this, MemPointer.GlobalGameManager);
			menuManager = new ProgramPointer(this, MemPointer.MenuManager);
			totemPole = new ProgramPointer(this, MemPointer.TotemPole) { AutoDeref = false };
			platformManager = new ProgramPointer(this, MemPointer.PlatformManager) { AutoDeref = false };
			ghostManager = new ProgramPointer(this, MemPointer.GhostManager);
			levelComplete = new ProgramPointer(this, MemPointer.LevelComplete) { AutoDeref = false };
			musicMachine = new ProgramPointer(this, MemPointer.MusicMachine);
			tas = new ProgramPointer(this, MemPointer.TAS) { AutoDeref = false, RetrySeconds = 10 };
			lastHooked = DateTime.MinValue;
		}

		public void SetMusicVolume(float volume) {
			musicMachine.Write(volume, 0x1c, 0x24);
			musicMachine.Write(volume, 0x20, 0x24);
		}
		public bool LevelComplete() {
			return levelComplete.Read<bool>() && (MenuScreen)menuManager.Read<int>(0x34) == MenuScreen.InGame;
		}
		public string ReadTASOutput() {
			return tas.Read();
		}
		public bool TASUI() {
			return tas.Read<bool>(4);
		}
		public void SetTASUI(bool val) {
			tas.Write(val, 4);
		}
		public void SetMaxSpeed(float runSpeed, float slideSpeed, float jumpHeight) {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0]
			IntPtr p1 = (IntPtr)globalGameManager.Read<uint>(0x14, 0xc, 0x18, 0x28, 0x10, 0x8);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1]
			IntPtr p2 = (IntPtr)globalGameManager.Read<uint>(0x14, 0xc, 0x18, 0x28, 0x14, 0x8);
			if (p1 == IntPtr.Zero) { return; }

			//PlayerMovement
			for (int i = 0xb0; i <= 0xb8; i += 4) {
				//RunSpeed
				Program.Write(p1, runSpeed, 0x10, i, 0x8);
				Program.Write(p1, runSpeed, 0x14, i, 0x8);
				Program.Write(p2, runSpeed, 0x10, i, 0x8);
				Program.Write(p2, runSpeed, 0x14, i, 0x8);
				//SlideSpeed
				Program.Write(p1, slideSpeed, 0x10, i, 0x24);
				Program.Write(p1, slideSpeed, 0x14, i, 0x24);
				Program.Write(p2, slideSpeed, 0x10, i, 0x24);
				Program.Write(p2, slideSpeed, 0x14, i, 0x24);
			}
			for (int i = 0xa4; i <= 0xac; i += 4) {
				//JumpHeight
				Program.Write(p1, jumpHeight, 0x10, i, 0x8);
				Program.Write(p1, jumpHeight, 0x14, i, 0x8);
				Program.Write(p2, jumpHeight, 0x10, i, 0x8);
				Program.Write(p2, jumpHeight, 0x14, i, 0x8);
			}
		}
		public void SetZoom(float zoom) {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.cameraController._currentZone.cameraSettings.size
			globalGameManager.Write(zoom, 0x14, 0x0c, 0x1c, 0x4c, 0x1c, 0x24);
		}
		public float Zoom() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.cameraController._currentZone.cameraSettings.size
			return globalGameManager.Read<float>(0x14, 0x0c, 0x1c, 0x4c, 0x1c, 0x24);
		}
		public int CameraZone() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.cameraController._currentZone.cameraSettings
			return globalGameManager.Read<int>(0x14, 0x0c, 0x1c, 0x4c, 0x1c);
		}
		public float CameraCenterX() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.cameraController.oldCenter.x
			return globalGameManager.Read<float>(0x14, 0x0c, 0x1c, 0x2c, 0x58);
		}
		public float CameraCenterY() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.cameraController.oldCenter.y
			return globalGameManager.Read<float>(0x14, 0x0c, 0x1c, 0x2c, 0x5c);
		}
		public void ResetCamera() {
			globalGameManager.Write(0, 0x14, 0x0c, 0x1c, 0x4c, 0x1c, 0x40);
			globalGameManager.Write(0, 0x14, 0x0c, 0x1c, 0x68);
			globalGameManager.Write(0L, 0x14, 0x0c, 0x1c, 0x4c, 0x1c, 0x1c);
			globalGameManager.Write(0, 0x14, 0x0c, 0x1c, 0x2c);
		}
		public void SetCameraOffset(float x, float y) {
			globalGameManager.Write(2, 0x14, 0x0c, 0x1c, 0x4c, 0x1c, 0x40);
			globalGameManager.Write(2, 0x14, 0x0c, 0x1c, 0x68);
			globalGameManager.Write(x, 0x14, 0x0c, 0x1c, 0x4c, 0x1c, 0x1c);
			globalGameManager.Write(y, 0x14, 0x0c, 0x1c, 0x4c, 0x1c, 0x20);
			globalGameManager.Write(false, 0x14, 0x0c, 0x1c, 0x4c, 0x1c, 0x28);

			if (globalGameManager.Read<int>(0x14, 0x0c, 0x1c, 0x2c) == 0) {
				int checkPoints = GetCheckpointCount();
				int currentCheckpoint = GetCurrentCheckpoint();
				for (int i = 0; i < checkPoints; i++) {
					if (globalGameManager.Read<int>(0x14, 0x0c, 0x14, 0x18, 0x10 + (i * 4), 0x50) == currentCheckpoint) {
						globalGameManager.Write(globalGameManager.Read<int>(0x14, 0x0c, 0x14, 0x18, 0x10 + (i * 4)), 0x14, 0x0c, 0x1c, 0x2c);
						break;
					}
				}
			}
		}
		public int FrameCount() {
			return Program.Read<int>(Program.MainModule.BaseAddress, 0xa1e97c);
		}
		public PlatformLevelId SelectedLevel() {
			//TotemWorldMap.instance.levelInfo.sceneFile.platformLevelId
			return (PlatformLevelId)totemPole.Read<int>(0x00, 0x28, 0x8c, 0x80);
		}
		public int SinglePlayerIndex() {
			//TotemWorldMap.instance.singleplayerTotemPole.menu.selectedIndex
			return totemPole.Read<int>(0x00, 0x18, 0x40, 0x44);
		}
		public int CoopIndex() {
			//TotemWorldMap.instance.multiplayerTotemPole.menu.selectedIndex
			return totemPole.Read<int>(0x00, 0x1c, 0x40, 0x44);
		}
		public int SinglePlayerDVIndex() {
			//TotemWorldMap.instance.singleplayerDLCTotemPole.menu.selectedIndex
			return totemPole.Read<int>(0x00, 0x20, 0x40, 0x44);
		}
		public int CoopDVIndex() {
			//TotemWorldMap.instance.multiplayerDLCTotemPole.menu.selectedIndex
			return totemPole.Read<int>(0x00, 0x24, 0x40, 0x44);
		}
		public void FixSpeedrun() {
			ghostManager.Write(true, 0x24);
		}
		public bool SpeedrunLoaded() {
			return ghostManager.Read<bool>(0x24);
		}
		public void PassthroughPickups(bool passthrough) {
			List<IntPtr> pickups = Program.FindAllSignatures("000000000000000000????000000A040????????????????00000000000000000000003f");
			for (int i = 0; i < pickups.Count; i++) {
				IntPtr pickup = pickups[i] - 0x4c;
				Program.Write(pickup, passthrough, 0x31);
			}
		}
		public int GetCheckpointCount() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.checkpointManager.checkPoints.Length
			return globalGameManager.Read<int>(0x14, 0x0c, 0x14, 0x18, 0x0c);
		}
		public void SetCheckpoint(int num, bool killTotems = true) {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.checkpointManager.checkPoints.Length
			int cpCount = GetCheckpointCount();
			if (num >= cpCount) { num = 0; }
			if (num < 0) { num = cpCount - 1; }
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].reachedCheckpoint
			globalGameManager.Write(num, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x18);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].currentCheckpoint
			globalGameManager.Write(num, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x1c);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].reachedCheckpoint
			globalGameManager.Write(num, 0x14, 0x0c, 0x18, 0x28, 0x14, 0x18);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].currentCheckpoint
			globalGameManager.Write(num, 0x14, 0x0c, 0x18, 0x28, 0x14, 0x1c);

			if (killTotems) {
				KillTotems();
			}
		}
		public void KillTotems() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[0]._objectDetector.updateable
			globalGameManager.Write(0, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0xdc, 0x50);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[0]._objectDetector.phase
			globalGameManager.Write(48, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0xdc, 0x48);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[0]._objectDetector.burntime
			globalGameManager.Write(2f, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0xdc, 0x54);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[1]._objectDetector.updateable
			globalGameManager.Write(0, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0xdc, 0x50);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[1]._objectDetector.phase
			globalGameManager.Write(48, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0xdc, 0x48);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[1]._objectDetector.burntime
			globalGameManager.Write(2f, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0xdc, 0x54);

			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[0]._objectDetector.updateable
			globalGameManager.Write(0, 0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x10, 0xdc, 0x50);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[0]._objectDetector.phase
			globalGameManager.Write(48, 0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x10, 0xdc, 0x48);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[0]._objectDetector.burntime
			globalGameManager.Write(2f, 0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x10, 0xdc, 0x54);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[1]._objectDetector.updateable
			globalGameManager.Write(0, 0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x14, 0xdc, 0x50);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[1]._objectDetector.phase
			globalGameManager.Write(48, 0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x14, 0xdc, 0x48);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[1]._objectDetector.burntime
			globalGameManager.Write(2f, 0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x14, 0xdc, 0x54);
		}
		public void SetInvincible(bool enabled) {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[0]._objectDetector.burntime
			globalGameManager.Write((float)(enabled ? int.MinValue : 0), 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0xdc, 0x54);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[0]._objectDetector.updateable
			globalGameManager.Write(enabled ? int.MinValue : 0, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0xdc, 0x50);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[1]._objectDetector.burntime
			globalGameManager.Write((float)(enabled ? int.MinValue : 0), 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0xdc, 0x54);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[1]._objectDetector.updateable
			globalGameManager.Write(enabled ? int.MinValue : 0, 0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0xdc, 0x50);

			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[0]._objectDetector.burntime
			globalGameManager.Write((float)(enabled ? int.MinValue : 0), 0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x10, 0xdc, 0x54);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[0]._objectDetector.updateable
			globalGameManager.Write(enabled ? int.MinValue : 0, 0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x10, 0xdc, 0x50);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[1]._objectDetector.burntime
			globalGameManager.Write((float)(enabled ? int.MinValue : 0), 0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x14, 0xdc, 0x54);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[1]._objectDetector.updateable
			globalGameManager.Write(enabled ? int.MinValue : 0, 0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x14, 0xdc, 0x50);
		}
		public bool IsInvincible() {
			return globalGameManager.Read<int>(0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0xdc, 0x50) < -10;
		}
		public int GetCurrentCheckpoint() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].currentCheckpoint
			int rCp = globalGameManager.Read<int>(0x14, 0x0c, 0x18, 0x28, 0x10, 0x1c);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].currentCheckpoint
			int cCp = globalGameManager.Read<int>(0x14, 0x0c, 0x18, 0x28, 0x14, 0x1c);
			return rCp > cCp ? rCp : cCp;
		}
		public bool IsDying() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[0].isDying
			bool dying = globalGameManager.Read<bool>(0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0x146);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[1].isDying
			dying |= globalGameManager.Read<bool>(0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x14, 0x146);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[0].isDying
			dying |= globalGameManager.Read<bool>(0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x14, 0x146);
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[1].isDying
			dying |= globalGameManager.Read<bool>(0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x14, 0x146);
			return dying;
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
		public string GetBossState() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.camerController.bossControllers
			IntPtr bossControllers = (IntPtr)globalGameManager.Read<uint>(0x14, 0x0c, 0x1c, 0x5c);
			int size = Program.Read<int>(bossControllers, 0xc);
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < size; i++) {
				sb.Append(Program.Read((IntPtr)Program.Read<uint>(bossControllers, 0x10 + (i * 4), 0x2c, 0x10))).Append('.');
			}
			return sb.ToString();
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
			globalGameManager.Write(score, 0x14, 0x10);
		}
		public int GetCurrentDeaths() {
			//GlobalGameManager.instance.currentSession.currentDeaths
			return globalGameManager.Read<int>(0x14, 0x14);
		}
		public void SetCurrentDeaths(int deaths) {
			//GlobalGameManager.instance.currentSession.currentDeaths
			globalGameManager.Write(deaths, 0x14, 0x14);
		}
		public float GetLevelTime() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.levelMetric.completionTime
			return globalGameManager.Read<float>(0x14, 0x0c, 0x18, 0x30, 0x20);
		}
		public string GetLevelName() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.levelMetric.levelName
			return globalGameManager.Read(0x14, 0x0c, 0x18, 0x30, 0x14);
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
		public float GetLastXP3() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[1].controlledPlayers[0].animationHandler.lastPos.X
			return globalGameManager.Read<float>(0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x10, 0xd4, 0xd0);
		}
		public float GetLastYP3() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[1].controlledPlayers[0].animationHandler.lastPos.Y
			return globalGameManager.Read<float>(0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x10, 0xd4, 0xd4);
		}
		public float GetLastXP4() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[1].controlledPlayers[1].animationHandler.lastPos.X
			return globalGameManager.Read<float>(0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x14, 0xd4, 0xd0);
		}
		public float GetLastYP4() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[1].controlledPlayers[1].animationHandler.lastPos.Y
			return globalGameManager.Read<float>(0x14, 0x0c, 0x18, 0x28, 0x14, 0x08, 0x14, 0xd4, 0xd4);
		}
		public bool GetFrozen() {
			//GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[0].frozen
			return globalGameManager.Read<bool>(0x14, 0x0c, 0x18, 0x28, 0x10, 0x08, 0x10, 0x3c);
		}
		public PersistentLevelStats GetLevelStats(PlatformLevelId id) {
			//PlatformManager.instance.imp.players[0].gameSinglePlayerStats._levels
			IntPtr levels = (IntPtr)platformManager.Read<uint>(0x0, 0x10, 0x48, 0x10, 0x24, 0x0c);
			PersistentLevelStats level = (id >= PlatformLevelId.Coop_Jump && id <= PlatformLevelId.Coop_EpicBossFight) || (id >= PlatformLevelId.DLC_Coop_Andrew && id <= PlatformLevelId.DLC_Coop_Jack) ? null : GetLevelStats(levels, id);
			if (level == null) {
				//PlatformManager.instance.imp.players[0].platformStats._coop["guest"]._levels
				levels = (IntPtr)platformManager.Read<uint>(0x0, 0x10, 0x48, 0x10, 0x34, 0x1c, 0x14, 0x10, 0x0c);
				return GetLevelStats(levels, id);
			}
			return level;
		}
		private PersistentLevelStats GetLevelStats(IntPtr levels, PlatformLevelId id) {
			int listSize = Program.Read<int>(levels, 0x20);
			IntPtr keys = (IntPtr)Program.Read<uint>(levels, 0x10);
			levels = (IntPtr)Program.Read<uint>(levels, 0x14);

			for (int i = 0; i < listSize; i++) {
				IntPtr itemHead = (IntPtr)Program.Read<uint>(levels, 0x10 + (i * 4));
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
		public void SetLevelScore(PlatformLevelId id, int score, bool erase = false) {
			SetSingleLevelScore(id, score, erase);
			SetCoopLevelScore(id, score, erase);
		}
		public void SetSingleLevelScore(PlatformLevelId id, int score, bool erase = false) {
			//PlatformManager.instance.imp.players[0].gameSinglePlayerStats._levels
			IntPtr levels = (IntPtr)platformManager.Read<uint>(0x0, 0x10, 0x48, 0x10, 0x24, 0x0c);
			SetScore(levels, id, score, erase);
		}
		public void SetCoopLevelScore(PlatformLevelId id, int score, bool erase = false) {
			//PlatformManager.instance.imp.players[0].platformStats._coop["guest"]._levels
			IntPtr levels = (IntPtr)platformManager.Read<uint>(0x0, 0x10, 0x48, 0x10, 0x34, 0x1c, 0x14, 0x10, 0x0c);
			SetScore(levels, id, score, erase);
		}
		private void SetScore(IntPtr levels, PlatformLevelId id, int score, bool erase = false) {
			int listSize = Program.Read<int>(levels, 0x20);
			IntPtr keys = (IntPtr)Program.Read<uint>(levels, 0x10);
			levels = (IntPtr)Program.Read<uint>(levels, 0x14);

			for (int i = 0; i < listSize; i++) {
				IntPtr itemHead = (IntPtr)Program.Read<uint>(levels, 0x10 + (i * 4));
				PlatformLevelId levelID = (PlatformLevelId)Program.Read<int>(keys, 0x10 + (i * 4));

				if (levelID == id || id == PlatformLevelId.None) {
					Program.Write(itemHead, score, 0x0c);
					Program.Write(itemHead, score == 70, 0x20);
					Program.Write(itemHead, int.MaxValue, 0x10);
					Program.Write(itemHead, erase ? (int)PersistentLevelStats.State.Unseen : (int)PersistentLevelStats.State.Completed, 0x08);
					Program.Write(itemHead, erase ? (int)PersistentLevelStats.State.Unseen : (int)PersistentLevelStats.State.Completed, 0x24);
				}
			}
		}
		public void EraseSingleData() {
			//PlatformManager.instance.imp.players[0].gameSinglePlayerStats._rememberedMoments.Count
			platformManager.Write(0, 0x0, 0x10, 0x48, 0x10, 0x24, 0x08, 0x0c);
			//PlatformManager.instance.imp.players[0].gameSinglePlayerStats._levels
			ClearStats((IntPtr)platformManager.Read<uint>(0x0, 0x10, 0x48, 0x10, 0x24, 0x0c));
		}
		public void EraseCoopData() {
			//PlatformManager.instance.imp.players[0].platformStats._coop["guest"]._levels
			IntPtr coopDic = (IntPtr)platformManager.Read<uint>(0x0, 0x10, 0x48, 0x10, 0x34, 0x1c, 0x14, 0x10);
			//PlatformManager.instance.imp.players[0].platformStats._coop["guest"]._rememberedMoments.Count
			Program.Write(coopDic, (int)0, 0x08, 0x0c);
			ClearStats((IntPtr)Program.Read<uint>(coopDic, 0x0c));
		}
		private void ClearStats(IntPtr levels) {
			int listSize = Program.Read<int>(levels, 0x20);
			levels = (IntPtr)Program.Read<uint>(levels, 0x14);

			for (int i = 0; i < listSize; i++) {
				IntPtr itemHead = (IntPtr)Program.Read<uint>(levels, 0x10 + (i * 4));

				Program.Write(itemHead, 0L, 0x08);
				Program.Write(itemHead, int.MaxValue, 0x10);
				Program.Write(itemHead, 0L, 0x14);
				Program.Write(itemHead, 0L, 0x1c);
				Program.Write(itemHead, 0L, 0x24);
				Program.Write(itemHead, int.MaxValue, 0x2c);
				Program.Write(itemHead, 0L, 0x30);
				Program.Write(itemHead, (short)0, 0x38);
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
	public enum MemVersion {
		None,
		V1
	}
	public enum MemPointer {
		GlobalGameManager,
		MenuManager,
		PlatformManager,
		TotemPole,
		GhostManager,
		LevelComplete,
		MusicMachine,
		TAS
	}
	public class ProgramPointer {
		private static Dictionary<MemVersion, Dictionary<MemPointer, string>> funcPatterns = new Dictionary<MemVersion, Dictionary<MemPointer, string>>() {
			{MemVersion.V1, new Dictionary<MemPointer, string>() {
				{MemPointer.GlobalGameManager, "558BEC5783EC34C745E4000000008B4508C74034000000008B05????????83EC086A0050E8????????83C41085C0743A8B05????????8B4D0883EC085150|-12"},
				{MemPointer.MenuManager,       "558BEC53575683EC0C8B05????????83EC086A0050E8????????83C41085C074338B05????????83EC08FF750850E8????????83C41085C0741A83EC0CFF7508E8|-30"},
				{MemPointer.PlatformManager,   "558BEC535683EC108B05????????83EC0C50E8????????83C41085C0740B8B05"},
				{MemPointer.TotemPole,         "D95810D94510D958148B4D1489480CC9C3000000558BEC83EC08B8????????8B4D088908C9C3000000000000558BEC5683EC0483EC0C|-27"},
				{MemPointer.GhostManager,      "EC5783EC148B7D088B05????????83EC0C503900E8????????83C41083EC086A01503900E8????????83C410C647240083EC0C68????????E8????????83C41083EC0C8945F450E8????????83C4108B45F489471883EC0C|-78" },
				{MemPointer.LevelComplete,     "558BEC5783EC648B7D0883EC0C57E8????????83C410B8????????C60000D9EED99F????????8B474083EC086A0050E8????????83C41085C0743083EC0C57|-40" },
				{MemPointer.MusicMachine,      "558BEC575683EC108B75088B7D0C83FF060F85????????8B05????????83EC0C503900E8????????83C4108945F48B45F43D????????74268B05" },
				{MemPointer.TAS,               "C745F88B2F7DE1C745FC933CAF568D45F883EC0868????????50E8????????83C4108BC8B8" }
			}},
		};
		private IntPtr pointer;
		public KalimbaMemory Memory { get; set; }
		public MemPointer Name { get; set; }
		public MemVersion Version { get; set; }
		public bool AutoDeref { get; set; }
		private int lastID;
		private DateTime lastTry;
		public int RetrySeconds { get; set; }
		public ProgramPointer(KalimbaMemory memory, MemPointer pointer) {
			this.Memory = memory;
			this.Name = pointer;
			this.AutoDeref = true;
			RetrySeconds = 1;
			lastID = memory.Program == null ? -1 : memory.Program.Id;
			lastTry = DateTime.MinValue;
		}

		public IntPtr Value {
			get {
				GetPointer();
				return pointer;
			}
		}
		public T Read<T>(params int[] offsets) where T : struct {
			return Memory.Program.Read<T>(Value, offsets);
		}
		public string Read(params int[] offsets) {
			if (!Memory.IsHooked) { return string.Empty; }

			bool is64bit = Memory.Program.Is64Bit();
			IntPtr p = IntPtr.Zero;
			if (is64bit) {
				p = (IntPtr)Memory.Program.Read<ulong>(Value, offsets);
			} else {
				p = (IntPtr)Memory.Program.Read<uint>(Value, offsets);
			}
			return Memory.Program.Read(p);
		}
		public void Write<T>(T value, params int[] offsets) where T : struct {
			Memory.Program.Write<T>(Value, value, offsets);
		}
		private void GetPointer() {
			if (!Memory.IsHooked) {
				pointer = IntPtr.Zero;
				Version = MemVersion.None;
				return;
			}

			if (Memory.Program.Id != lastID) {
				pointer = IntPtr.Zero;
				Version = MemVersion.None;
				lastID = Memory.Program.Id;
			}
			if (pointer == IntPtr.Zero && DateTime.Now > lastTry.AddSeconds(RetrySeconds)) {
				lastTry = DateTime.Now;
				pointer = GetVersionedFunctionPointer();
				if (pointer != IntPtr.Zero) {
					bool is64bit = Memory.Program.Is64Bit();
					pointer = (IntPtr)Memory.Program.Read<uint>(pointer);
					if (AutoDeref) {
						if (is64bit) {
							pointer = (IntPtr)Memory.Program.Read<ulong>(pointer);
						} else {
							pointer = (IntPtr)Memory.Program.Read<uint>(pointer);
						}
					}
				}
			}
		}
		private IntPtr GetVersionedFunctionPointer() {
			foreach (MemVersion version in Enum.GetValues(typeof(MemVersion))) {
				Dictionary<MemPointer, string> patterns = null;
				if (!funcPatterns.TryGetValue(version, out patterns)) { continue; }

				string pattern = null;
				if (!patterns.TryGetValue(Name, out pattern)) { continue; }

				IntPtr ptr = Memory.Program.FindSignatures(pattern)[0];
				if (ptr != IntPtr.Zero) {
					Version = version;
					return ptr;
				}
			}
			Version = MemVersion.None;
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
	public enum LevelID {
		NappyOwl = 1,
		LazyHound = 2,
		StranDog = 3,
		StaringCow = 4,
		GreenishVireo = 5,
		Buckteeth = 6,
		Reptilicus = 7,
		GrindingChief = 8,
		GrizzlyRodent = 9,
		SnakyFace = 10,
		SnaglePuff = 11,
		Skully = 14,
		GrimeyGator = 13,
		MustyCyclops = 12,
		OwlBear = 15,
		ZemiChief = 16,
		DecafDogChild = 17,
		GravGaard = 18,
		KoalaKid = 19,
		TerraCotta = 20,
		SpiritualOoze = 21,
		Raymond = 22,
		SpikeyBrow = 23,
		JurakanChief = 24,

		Demongo = 154,
		KoolDoktor = 153,
		Slim = 150,
		Sosumi = 152,
		Smokingref = 151,
		Cyclops = 156,
		JusticeBeaver = 155,
		Kuthulu = 157,
		Jamarly = 158,
		Illuminator = 159,

		SneakyRascal = 41,
		OculusMug = 43,
		RosyCheeks = 44,
		MoonlightBandit = 42,
		DopeyPeg = 45,
		SauceyBaboon = 46,
		SpunkyFangs = 47,
		MummyGreen = 51,
		Nurtle = 48,
		NarlyTwoFace = 52,

		Frogger = 123,
		Thumba = 120,
		PurpleSlander = 124,
		PierceParrot = 126,
		KaleidoFace = 122,
		Dario = 128,
		DJSteelFace = 121,
		JeffMoldblum = 125,
		CrustyMouth = 127,
		Lemmy = 129
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