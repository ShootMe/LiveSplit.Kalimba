﻿namespace LiveSplit.Kalimba {
    public enum LogObject {
        CurrentSplit,
        World,
        Campaign,
        CurrentMenu,
        PreviousMenu,
        Cinematic,
        LoadingLevel,
        Disabled,
        Checkpoint,
        Deaths,
        State,
        EndLevel,
        PlatformLevel,
        Stats,
        BossState
    }
    public enum MenuScreen : int {
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
    public enum Campaign : int {
        Singleplayer,
        Cooperative
    }
    public enum World : int {
        None,
        Underground,
        Earth,
        Sky,
        Space,
        Challenge
    }
    public enum PlatformLevelId : int {
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
    public enum LevelID : int {
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
        public enum State : int {
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