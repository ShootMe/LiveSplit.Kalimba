using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
namespace LiveSplit.Kalimba {
    public partial class MemoryManager {
        private static ProgramPointer globalGameManager = new ProgramPointer(new FindPointerSignature(PointerVersion.Steam, AutoDeref.Double, "558BEC5783EC34C745E4000000008B4508C74034000000008B05????????83EC086A0050E8????????83C41085C0743A8B05", 50));
        private static ProgramPointer menuManager = new ProgramPointer(new FindPointerSignature(PointerVersion.Steam, AutoDeref.Double, "558BEC53575683EC0C8B05????????83EC086A0050E8????????83C41085C074338B05", 35));
        private static ProgramPointer totemPole = new ProgramPointer(new FindPointerSignature(PointerVersion.Steam, AutoDeref.Double, "D95810D94510D958148B4D1489480CC9C3000000558BEC83EC08B8????????8B4D088908C9C3000000000000558BEC5683EC0483EC0C", 27));
        private static ProgramPointer platformManager = new ProgramPointer(new FindPointerSignature(PointerVersion.Steam, AutoDeref.Double, "83EC0C50E8????????83C41085C0740B8B05????????E9????????0FB605", 18));
        private static ProgramPointer ghostManager = new ProgramPointer(new FindPointerSignature(PointerVersion.Steam, AutoDeref.Double, "EC5783EC148B7D088B05????????83EC0C503900E8????????83C41083EC086A01503900E8", 10));
        private static ProgramPointer levelComplete = new ProgramPointer(new FindPointerSignature(PointerVersion.Steam, AutoDeref.Double, "558BEC5783EC648B7D0883EC0C57E8????????83C410B8????????C60000D9EED99F", 23));
        private static ProgramPointer musicMachine = new ProgramPointer(new FindPointerSignature(PointerVersion.Steam, AutoDeref.Double, "558BEC575683EC108B75088B7D0C83FF060F85????????8B05????????83EC0C503900E8", 58));
        private static ProgramPointer tas = new ProgramPointer(new FindPointerSignature(PointerVersion.Steam, AutoDeref.Single, "C745F88B2F7DE1C745FC933CAF568D45F883EC0868????????50E8????????83C4108BC8B8", 37));
        public static PointerVersion Version { get; set; } = PointerVersion.Steam;
        public Process Program { get; set; }
        public bool IsHooked { get; set; } = false;
        public DateTime LastHooked { get; set; }
        private IntPtr BaseAddress;

        public MemoryManager() {
            LastHooked = DateTime.MinValue;
        }

        public void SetMusicVolume(float volume) {
            musicMachine.Write<float>(Program, volume, 0x1c, 0x24);
            musicMachine.Write<float>(Program, volume, 0x20, 0x24);
        }
        public bool LevelComplete(MenuScreen currentMenu) {
            return levelComplete.Read<bool>(Program) && currentMenu == MenuScreen.InGame;
        }
        public string ReadTASOutput() {
            return tas.Read(Program);
        }
        public bool TASUI() {
            return tas.Read<bool>(Program, 4);
        }
        public void SetTASUI(bool val) {
            tas.Write<bool>(Program, val, 4);
        }
        public void SetMaxSpeed(float runSpeed, float slideSpeed, float jumpHeight) {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0]
            IntPtr p1 = globalGameManager.Read<IntPtr>(Program, 0x14, 0xc, 0x18, 0x28, 0x10, 0x8);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1]
            IntPtr p2 = globalGameManager.Read<IntPtr>(Program, 0x14, 0xc, 0x18, 0x28, 0x14, 0x8);
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
            globalGameManager.Write<float>(Program, zoom, 0x14, 0xc, 0x1c, 0x4c, 0x1c, 0x24);
        }
        public float Zoom() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.cameraController._currentZone.cameraSettings.size
            return globalGameManager.Read<float>(Program, 0x14, 0xc, 0x1c, 0x4c, 0x1c, 0x24);
        }
        public int CameraZone() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.cameraController._currentZone.cameraSettings
            return globalGameManager.Read<int>(Program, 0x14, 0xc, 0x1c, 0x4c, 0x1c);
        }
        public float CameraCenterX() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.cameraController.oldCenter.x
            return globalGameManager.Read<float>(Program, 0x14, 0xc, 0x1c, 0x2c, 0x58);
        }
        public float CameraCenterY() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.cameraController.oldCenter.y
            return globalGameManager.Read<float>(Program, 0x14, 0xc, 0x1c, 0x2c, 0x5c);
        }
        public void ResetCamera() {
            globalGameManager.Write<int>(Program, 0, 0x14, 0xc, 0x1c, 0x4c, 0x1c, 0x40);
            globalGameManager.Write<int>(Program, 0, 0x14, 0xc, 0x1c, 0x68);
            globalGameManager.Write<long>(Program, 0L, 0x14, 0xc, 0x1c, 0x4c, 0x1c, 0x1c);
            globalGameManager.Write<int>(Program, 0, 0x14, 0xc, 0x1c, 0x2c);
        }
        public void SetCameraOffset(float x, float y) {
            globalGameManager.Write<int>(Program, 2, 0x14, 0xc, 0x1c, 0x4c, 0x1c, 0x40);
            globalGameManager.Write<int>(Program, 2, 0x14, 0xc, 0x1c, 0x68);
            globalGameManager.Write<float>(Program, x, 0x14, 0xc, 0x1c, 0x4c, 0x1c, 0x1c);
            globalGameManager.Write<float>(Program, y, 0x14, 0xc, 0x1c, 0x4c, 0x1c, 0x20);
            globalGameManager.Write<bool>(Program, false, 0x14, 0xc, 0x1c, 0x4c, 0x1c, 0x28);

            if (globalGameManager.Read<int>(Program, 0x14, 0xc, 0x1c, 0x2c) == 0) {
                int checkPoints = GetCheckpointCount();
                int currentCheckpoint = GetCurrentCheckpoint();
                for (int i = 0; i < checkPoints; i++) {
                    if (globalGameManager.Read<int>(Program, 0x14, 0xc, 0x14, 0x18, 0x10 + (i * 4), 0x50) == currentCheckpoint) {
                        globalGameManager.Write<int>(Program, globalGameManager.Read<int>(Program, 0x14, 0xc, 0x14, 0x18, 0x10 + (i * 4)), 0x0, 0x14, 0xc, 0x1c, 0x2c);
                        break;
                    }
                }
            }
        }
        public int FrameCount() {
            return Program.Read<int>(BaseAddress, 0xa1e97c);
        }
        public double GameTime() {
            return Program.Read<double>(BaseAddress, 0xa1cdfc, 0x58);
        }
        public PlatformLevelId SelectedLevel() {
            //TotemWorldMap.instance.levelInfo.sceneFile.platformLevelId
            return totemPole.Read<PlatformLevelId>(Program, 0x28, 0x8c, 0x80);
        }
        public int SinglePlayerIndex() {
            //TotemWorldMap.instance.singleplayerTotemPole.menu.selectedIndex
            return totemPole.Read<int>(Program, 0x18, 0x40, 0x44);
        }
        public int CoopIndex() {
            //TotemWorldMap.instance.multiplayerTotemPole.menu.selectedIndex
            return totemPole.Read<int>(Program, 0x1c, 0x40, 0x44);
        }
        public int SinglePlayerDVIndex() {
            //TotemWorldMap.instance.singleplayerDLCTotemPole.menu.selectedIndex
            return totemPole.Read<int>(Program, 0x20, 0x40, 0x44);
        }
        public int CoopDVIndex() {
            //TotemWorldMap.instance.multiplayerDLCTotemPole.menu.selectedIndex
            return totemPole.Read<int>(Program, 0x24, 0x40, 0x44);
        }
        public void FixSpeedrun() {
            ghostManager.Write<bool>(Program, true, 0x24);
        }
        public bool SpeedrunLoaded() {
            return ghostManager.Read<bool>(Program, 0x24);
        }
        public void PassthroughPickups(bool passthrough) {
            MemorySearcher searcher = new MemorySearcher();
            searcher.MemoryFilter = delegate (MemInfo info) {
                return (info.State & 0x1000) != 0 && (info.Protect & 0x100) == 0 && (info.Protect & 0x40) != 0;
            };
            List<IntPtr> pickups = searcher.FindSignatures(Program, "000000000000000000????000000A040????????????????00000000000000000000003f");
            for (int i = 0; i < pickups.Count; i++) {
                IntPtr pickup = pickups[i] - 0x28;
                Program.Write<bool>(pickup, passthrough, 0x31);
            }
        }
        public int GetCheckpointCount() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.checkpointManager.checkPoints.Length
            return globalGameManager.Read<int>(Program, 0x14, 0xc, 0x14, 0x18, 0xc);
        }
        public void SetCheckpoint(int num, bool killTotems = true) {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.checkpointManager.checkPoints.Length
            int cpCount = GetCheckpointCount();
            if (num >= cpCount) { num = 0; }
            if (num < 0) { num = cpCount - 1; }
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].reachedCheckpoint
            globalGameManager.Write<int>(Program, num, 0x14, 0xc, 0x18, 0x28, 0x10, 0x18);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].currentCheckpoint
            globalGameManager.Write<int>(Program, num, 0x14, 0xc, 0x18, 0x28, 0x10, 0x1c);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].reachedCheckpoint
            globalGameManager.Write<int>(Program, num, 0x14, 0xc, 0x18, 0x28, 0x14, 0x18);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].currentCheckpoint
            globalGameManager.Write<int>(Program, num, 0x14, 0xc, 0x18, 0x28, 0x14, 0x1c);

            if (killTotems) {
                KillTotems();
            }
        }
        public void KillTotems() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[0]._objectDetector.updateable
            globalGameManager.Write<int>(Program, 0, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x10, 0xdc, 0x50);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[0]._objectDetector.phase
            globalGameManager.Write<int>(Program, 48, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x10, 0xdc, 0x48);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[0]._objectDetector.burntime
            globalGameManager.Write<float>(Program, 2f, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x10, 0xdc, 0x54);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[1]._objectDetector.updateable
            globalGameManager.Write<int>(Program, 0, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x14, 0xdc, 0x50);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[1]._objectDetector.phase
            globalGameManager.Write<int>(Program, 48, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x14, 0xdc, 0x48);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[1]._objectDetector.burntime
            globalGameManager.Write<float>(Program, 2f, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x14, 0xdc, 0x54);

            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[0]._objectDetector.updateable
            globalGameManager.Write<int>(Program, 0, 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x10, 0xdc, 0x50);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[0]._objectDetector.phase
            globalGameManager.Write<int>(Program, 48, 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x10, 0xdc, 0x48);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[0]._objectDetector.burntime
            globalGameManager.Write<float>(Program, 2f, 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x10, 0xdc, 0x54);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[1]._objectDetector.updateable
            globalGameManager.Write<int>(Program, 0, 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x14, 0xdc, 0x50);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[1]._objectDetector.phase
            globalGameManager.Write<int>(Program, 48, 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x14, 0xdc, 0x48);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[1]._objectDetector.burntime
            globalGameManager.Write<float>(Program, 2f, 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x14, 0xdc, 0x54);
        }
        public void SetInvincible(bool enabled) {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[0]._objectDetector.burntime
            globalGameManager.Write<float>(Program, (float)(enabled ? int.MinValue : 0), 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x10, 0xdc, 0x54);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[0]._objectDetector.updateable
            globalGameManager.Write<int>(Program, enabled ? int.MinValue : 0, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x10, 0xdc, 0x50);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[1]._objectDetector.burntime
            globalGameManager.Write<float>(Program, (float)(enabled ? int.MinValue : 0), 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x14, 0xdc, 0x54);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[1]._objectDetector.updateable
            globalGameManager.Write<int>(Program, enabled ? int.MinValue : 0, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x14, 0xdc, 0x50);

            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[0]._objectDetector.burntime
            globalGameManager.Write<float>(Program, (float)(enabled ? int.MinValue : 0), 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x10, 0xdc, 0x54);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[0]._objectDetector.updateable
            globalGameManager.Write<int>(Program, enabled ? int.MinValue : 0, 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x10, 0xdc, 0x50);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[1]._objectDetector.burntime
            globalGameManager.Write<float>(Program, (float)(enabled ? int.MinValue : 0), 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x14, 0xdc, 0x54);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[1]._objectDetector.updateable
            globalGameManager.Write<int>(Program, enabled ? int.MinValue : 0, 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x14, 0xdc, 0x50);
        }
        public bool IsInvincible() {
            return globalGameManager.Read<int>(Program, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x10, 0xdc, 0x50) < -10;
        }
        public int GetCurrentCheckpoint() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].currentCheckpoint
            int rCp = globalGameManager.Read<int>(Program, 0x14, 0xc, 0x18, 0x28, 0x10, 0x1c);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].currentCheckpoint
            int cCp = globalGameManager.Read<int>(Program, 0x14, 0xc, 0x18, 0x28, 0x14, 0x1c);
            return rCp > cCp ? rCp : cCp;
        }
        public bool IsDying() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[0].isDying
            bool dying = globalGameManager.Read<bool>(Program, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x10, 0x146);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[0].controlledPlayers[1].isDying
            dying |= globalGameManager.Read<bool>(Program, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x14, 0x146);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[0].isDying
            dying |= globalGameManager.Read<bool>(Program, 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x10, 0x146);
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controller[1].controlledPlayers[1].isDying
            dying |= globalGameManager.Read<bool>(Program, 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x14, 0x146);
            return dying;
        }
        public PlatformLevelId GetPlatformLevelId() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.sceneFile.platformLevelId
            return globalGameManager.Read<PlatformLevelId>(Program, 0x14, 0xc, 0x10, 0x80);
        }
        public World GetWorld() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.sceneFile.world
            return (World)globalGameManager.Read<int>(Program, 0x14, 0xc, 0x10, 0x5c);
        }
        public Campaign GetCampaign() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.sceneFile.campaign
            return (Campaign)globalGameManager.Read<int>(Program, 0x14, 0xc, 0x10, 0x60);
        }
        public string GetBossState() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.camerController.bossControllers
            IntPtr bossControllers = globalGameManager.Read<IntPtr>(Program, 0x14, 0xc, 0x1c, 0x5c);
            int size = Program.Read<int>(bossControllers, 0xc);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++) {
                sb.Append(Program.ReadString(Program.Read<IntPtr>(bossControllers, 0x10 + (i * 4), 0x2c, 0x10))).Append('.');
            }
            return sb.ToString();
        }
        public MenuScreen GetCurrentMenu() {
            //MenuManager.instance._currentMenu
            return menuManager.Read<MenuScreen>(Program, 0x34);
        }
        public MenuScreen GetPreviousMenu() {
            //MenuManager.instance._previousMenu
            return menuManager.Read<MenuScreen>(Program, 0x38);
        }
        public bool GetPlayingCinematic() {
            //GlobalGameManager.instance.isPlayingCinematic
            return globalGameManager.Read<bool>(Program, 0x46);
        }
        public bool GetIsLoadingLevel() {
            //GlobalGameManager.instance.levelIsLoading
            return globalGameManager.Read<bool>(Program, 0x4c);
        }
        public int GetCurrentScore() {
            //GlobalGameManager.instance.currentSession.currentScore
            return globalGameManager.Read<int>(Program, 0x14, 0x10);
        }
        public void SetCurrentScore(int score) {
            //GlobalGameManager.instance.currentSession.currentScore
            globalGameManager.Write<int>(Program, score, 0x14, 0x10);
        }
        public int GetCurrentDeaths() {
            //GlobalGameManager.instance.currentSession.currentDeaths
            return globalGameManager.Read<int>(Program, 0x14, 0x14);
        }
        public void SetCurrentDeaths(int deaths) {
            //GlobalGameManager.instance.currentSession.currentDeaths
            globalGameManager.Write<int>(Program, deaths, 0x14, 0x14);
        }
        public float GetLevelTime() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.levelMetric.completionTime
            return globalGameManager.Read<float>(Program, 0x14, 0xc, 0x18, 0x30, 0x20);
        }
        public string GetLevelName() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.levelMetric.levelName
            return globalGameManager.Read(Program, 0x14, 0xc, 0x18, 0x30, 0x14);
        }
        public bool GetIsDisabled() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.(noJump | noSwap | noMove)
            return globalGameManager.Read<int>(Program, 0x14, 0xc, 0x18, 0x64) == 65793;
        }
        public Vector2 GetLastP1() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[0].animationHandler.lastPos.X
            return globalGameManager.Read<Vector2>(Program, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x10, 0xd4, 0xd0);
        }
        public Vector2 GetLastP2() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[1].animationHandler.lastPos.X
            return globalGameManager.Read<Vector2>(Program, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x14, 0xd4, 0xd0);
        }
        public Vector2 GetLastP3() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[1].controlledPlayers[0].animationHandler.lastPos.X
            return globalGameManager.Read<Vector2>(Program, 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x10, 0xd4, 0xd0);
        }
        public Vector2 GetLastP4() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[1].controlledPlayers[1].animationHandler.lastPos.X
            return globalGameManager.Read<Vector2>(Program, 0x14, 0xc, 0x18, 0x28, 0x14, 0x08, 0x14, 0xd4, 0xd0);
        }
        public bool GetFrozen() {
            //GlobalGameManager.instance.currentSession.activeSessionHolder.gameManager.controllers[0].controlledPlayers[0].frozen
            return globalGameManager.Read<bool>(Program, 0x14, 0xc, 0x18, 0x28, 0x10, 0x08, 0x10, 0x3c);
        }
        public PersistentLevelStats GetLevelStats(PlatformLevelId id) {
            //PlatformManager.instance.imp.players[0].gameSinglePlayerStats._levels
            IntPtr levels = platformManager.Read<IntPtr>(Program, 0x10, 0x48, 0x10, 0x24, 0xc);
            PersistentLevelStats level = (id >= PlatformLevelId.Coop_Jump && id <= PlatformLevelId.Coop_EpicBossFight) || (id >= PlatformLevelId.DLC_Coop_Andrew && id <= PlatformLevelId.DLC_Coop_Jack) ? null : GetLevelStats(levels, id);
            if (level == null) {
                //PlatformManager.instance.imp.players[0].platformStats._coop["guest"]._levels
                levels = platformManager.Read<IntPtr>(Program, 0x10, 0x48, 0x10, 0x34, 0x1c, 0x14, 0x10, 0xc);
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
                PlatformLevelId levelID = Program.Read<PlatformLevelId>(keys, 0x10 + (i * 4));

                if (levelID == id) {
                    PersistentLevelStats level = new PersistentLevelStats();
                    level.id = levelID;
                    level.awardedGoldenTotemPiece = Program.Read<bool>(itemHead, 0x20);
                    level.maxScore = Program.Read<int>(itemHead, 0xc);
                    level.minDeaths = Program.Read<int>(itemHead, 0x18);
                    level.minKills = Program.Read<int>(itemHead, 0x1c);
                    level.minMillisecondsForMaxScore = Program.Read<int>(itemHead, 0x10);
                    level.minPickups = Program.Read<int>(itemHead, 0x14);
                    level.state = Program.Read<PersistentLevelStats.State>(itemHead, 0x08);
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
            IntPtr levels = platformManager.Read<IntPtr>(Program, 0x10, 0x48, 0x10, 0x24, 0xc);
            SetScore(levels, id, score, erase);
        }
        public void SetCoopLevelScore(PlatformLevelId id, int score, bool erase = false) {
            //PlatformManager.instance.imp.players[0].platformStats._coop["guest"]._levels
            IntPtr levels = platformManager.Read<IntPtr>(Program, 0x10, 0x48, 0x10, 0x34, 0x1c, 0x14, 0x10, 0xc);
            SetScore(levels, id, score, erase);
        }
        private void SetScore(IntPtr levels, PlatformLevelId id, int score, bool erase = false) {
            int listSize = Program.Read<int>(levels, 0x20);
            IntPtr keys = Program.Read<IntPtr>(levels, 0x10);
            levels = Program.Read<IntPtr>(levels, 0x14);

            for (int i = 0; i < listSize; i++) {
                IntPtr itemHead = Program.Read<IntPtr>(levels, 0x10 + (i * 4));
                PlatformLevelId levelID = Program.Read<PlatformLevelId>(keys, 0x10 + (i * 4));

                if (levelID == id || id == PlatformLevelId.None) {
                    Program.Write(itemHead, score, 0xc);
                    Program.Write(itemHead, score == 70, 0x20);
                    Program.Write(itemHead, int.MaxValue, 0x10);
                    Program.Write(itemHead, erase ? (int)PersistentLevelStats.State.Unseen : (int)PersistentLevelStats.State.Completed, 0x08);
                    Program.Write(itemHead, erase ? (int)PersistentLevelStats.State.Unseen : (int)PersistentLevelStats.State.Completed, 0x24);
                }
            }
        }
        public void EraseData() {
            //PlatformManager.instance.imp.players[0].gameSinglePlayerStats._rememberedMoments.Count
            platformManager.Write<int>(Program, 0, 0x10, 0x48, 0x10, 0x24, 0x08, 0xc);
            //PlatformManager.instance.imp.players[0].gameSinglePlayerStats._levels
            ClearStats(platformManager.Read<IntPtr>(Program, 0x10, 0x48, 0x10, 0x24, 0xc));
            //PlatformManager.instance.imp.players[0].platformStats._coop["guest"]._levels
            IntPtr coopDic = platformManager.Read<IntPtr>(Program, 0x10, 0x48, 0x10, 0x34, 0x1c, 0x14, 0x10);
            //PlatformManager.instance.imp.players[0].platformStats._coop["guest"]._rememberedMoments.Count
            Program.Write<int>(coopDic, (int)0, 0x8, 0xc);
            ClearStats(Program.Read<IntPtr>(coopDic, 0xc));
        }
        private void ClearStats(IntPtr levels) {
            int listSize = Program.Read<int>(levels, 0x20);
            levels = Program.Read<IntPtr>(levels, 0x14);

            for (int i = 0; i < listSize; i++) {
                IntPtr itemHead = Program.Read<IntPtr>(levels, 0x10 + (i * 4));

                Program.Write(itemHead, 0L, 0x8);
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
            IsHooked = Program != null && !Program.HasExited;
            if (!IsHooked && DateTime.Now > LastHooked.AddSeconds(1)) {
                LastHooked = DateTime.Now;

                Process[] processes = Process.GetProcessesByName("Kalimba");
                Program = processes.Length == 0 ? null : processes[0];

                if (Program != null) {
                    MemoryReader.Update64Bit(Program);
                    MemoryManager.Version = PointerVersion.Steam;
                    BaseAddress = Program.MainModule.BaseAddress;
                    IsHooked = true;
                }
            }

            return IsHooked;
        }
        public void Dispose() {
            if (Program != null) {
                Program.Dispose();
            }
        }
    }
}