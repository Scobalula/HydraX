using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using PhilLibX;
using HydraX.Library.AssetContainers;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Sound Bank (Aliases, etc.) Logic
        /// </summary>
        private class Sound : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// Sound Asset Header
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct SoundAsset
            {
                #region SoundAssetProperties
                public long NamePointer;
                public long ZoneNamePointer;
                public long LanguageIDPointer;
                public long LanguagePointer;
                public int AliasCount;
                public long AliasesPointer;
                public long AliasIndicesPointer;
                public int ReverbCount;
                public long ReverbsPointer;
                public int DuckCount;
                public long DucksPointer;
                public int UnkCount;
                public long UnkPointer;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x22038)]
                public byte[] UnknownData;
                public int MusicSetCount;
                public long MusicSetsPointer;
                #endregion
            }

            /// <summary>
            /// Music State Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct MusicStateAsset
            {
                #region MusicStateAssetProperties
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
                public char[] Name;
                public uint Hash;
                public bool Looping;
                public bool CompleteLoop;
                public bool RemoveAfterPlay;
                public bool PlayAsFirstRandom;
                public bool StartSync;
                public bool StopSync;
                public bool CompleteOnStop;
                public int LoopStartOffset;
                public int BPM;
                public int AssetType;
                public int LoopNumber;
                public int Order;
                public int StartDelayBeats;
                public int StartFadeBeats;
                public int StopDelayBeats;
                public int StopFadeBeats;
                public int Padding;
                public int Meter;
                #endregion
            }

            /// <summary>
            /// Music State Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct MusicState
            {
                #region MusicStateProperties
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
                public char[] Name;
                public uint Hash;
                public MusicStateAsset IntroAsset;
                public MusicStateAsset ExitAsset;
                public int LoopAssetCount;
                public long LoopAssetsPointer;
                public int Order;
                public bool IsRandom;
                public bool IsSequential;
                public bool SkipPreviousExit;
                public long Padding;
                #endregion
            }

            /// <summary>
            /// Music Set Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct MusicSet
            {
                #region MusicSetProperties
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
                public char[] Name;
                public uint Hash;
                public int StateCount;
                public long StatesPointer;
                #endregion
            }

            /// <summary>
            /// Reverb Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 160)]
            private struct Reverb
            {
                #region ReverbProperties
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
                public char[] Name;
                public uint Hash;
                public float MasterReturn;
                public float EarlyPreDelayMs;
                public float EarlyInputLpf;
                public float EarlyFeedback;
                public float EarlySmear;
                public float EarlyBaseDelayMs;
                public float EarlyReturn;
                public float NearInputLpf;
                public float NearFeedback;
                public float NearReturn;
                public float NearLowDamp;
                public float NearHighDamp;
                public float NearDecayTime;
                public float NearSmear;
                public float NearPreDelayMs;
                public float FarInputLpf;
                public float FarFeedback;
                public float FarReturn;
                public float FarLowDamp;
                public float FarHighDamp;
                public float FarDecayTime;
                public float FarSmear;
                public float FarPreDelayMs;
                #endregion
            }

            /// <summary>
            /// Sound Alias Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 40)]
            private struct SoundAlias
            {
                #region SoundAliasProperties
                public long NamePointer;
                public uint Hash;
                public long EntiresPointer;
                public int EntryCount;
                public float MaxDistance;
                public int PanType;
                #endregion
            }

            /// <summary>
            /// Sound Alias Name Structure
            /// </summary>
            [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 16)]
            private struct SoundAliasName
            {
                #region SoundAliasNameProperties
                [FieldOffset(0)]
                public long StringPointer;
                [FieldOffset(8)]
                public uint Hash;
                #endregion
            }

            /// <summary>
            /// Sound Alias File Spec Structure
            /// </summary>
            [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 16)]
            private struct SoundAliasFileSpec
            {
                #region SoundAliasFileSpecProperties
                [FieldOffset(0)]
                public uint Hash;
                [FieldOffset(8)]
                public long FileNamePointer;
                #endregion
            }

            /// <summary>
            /// Alias Entry Context Structure
            /// </summary>
            [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 8)]
            private struct SoundAliasContext
            {
                #region SoundAliasContentProperties
                [FieldOffset(0)]
                public uint Type;
                [FieldOffset(4)]
                public uint Value;
                #endregion
            }

            /// <summary>
            /// Sound Alias Entry Structure
            /// </summary>
            [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 216)]
            private struct SoundAliasEntry
            {
                #region SoundAliasEntryProperties
                [FieldOffset(0)]
                public SoundAliasName NamePointer;
                [FieldOffset(16)]
                public long SubtitlePointer;
                [FieldOffset(24)]
                public SoundAliasName SecondaryPointer;
                [FieldOffset(40)]
                public SoundAliasName StopAlias;
                [FieldOffset(56)]
                public SoundAliasFileSpec FileSpec;
                [FieldOffset(72)]
                public SoundAliasFileSpec FileSpecSustain;
                [FieldOffset(88)]
                public SoundAliasFileSpec FileSpecRelease;
                [FieldOffset(104)]
                public int Flags0;
                [FieldOffset(108)]
                public int Flags1;
                [FieldOffset(112)]
                public uint Duck;
                [FieldOffset(116)]
                public SoundAliasContext Context0;
                [FieldOffset(124)]
                public SoundAliasContext Context1;
                [FieldOffset(132)]
                public SoundAliasContext Context2;
                [FieldOffset(140)]
                public SoundAliasContext Context3;
                [FieldOffset(148)]
                public uint StopOnPlay;
                [FieldOffset(152)]
                public uint FutzPatch;
                [FieldOffset(156)]
                public float ReverbSend;
                [FieldOffset(160)]
                public float ReverbCenter;
                [FieldOffset(164)]
                public float VolMin;
                [FieldOffset(168)]
                public float VolMax;
                [FieldOffset(172)]
                public float EnvelopPercent;
                [FieldOffset(176)]
                public short FluxTime;
                [FieldOffset(178)]
                public short StartDelay;
                [FieldOffset(180)]
                public ushort PitchMin;
                [FieldOffset(182)]
                public ushort PitchMax;
                [FieldOffset(184)]
                public short DistMin;
                [FieldOffset(186)]
                public short DistMaxDry;
                [FieldOffset(188)]
                public short DistMaxWet;
                [FieldOffset(190)]
                public short EnvelopMin;
                [FieldOffset(192)]
                public short EnvelopMax;
                [FieldOffset(194)]
                public short DistanceLpfMin;
                [FieldOffset(196)]
                public short DistanceLpfMax;
                [FieldOffset(198)]
                public short FadeIn;
                [FieldOffset(200)]
                public short FadeOut;
                [FieldOffset(202)]
                public short DopplerScale;
                [FieldOffset(204)]
                public byte PriorityThresholdMin;
                [FieldOffset(205)]
                public byte PriorityThresholdMax;
                [FieldOffset(206)]
                public byte Probability;
                [FieldOffset(207)]
                public byte OcclusionLevel;
                [FieldOffset(208)]
                public byte PriorityMin;
                [FieldOffset(209)]
                public byte PriorityMax;
                [FieldOffset(210)]
                public byte Pan;
                [FieldOffset(211)]
                public byte LimitCount;
                [FieldOffset(212)]
                public byte EntityLimitCount;
                [FieldOffset(213)]
                public byte DuckGroup;
                [FieldOffset(214)]
                public byte Bus;
                [FieldOffset(215)]
                public byte VolumeGroup;
                #endregion
            }

            /// <summary>
            /// Sound Alias Entry Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 388)]
            private struct AmbientEntry
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
                public char[] Name;
                public uint NameHash;
                public bool DefaultRoom;
                public uint ReverbHash;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
                public char[] Reverb;
                public uint NearVerbHash;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
                public char[] NearVerb;
                public uint FarVerbHash;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
                public char[] FarVerb;
                public float ReverbDryLevel;
                public float ReverbWetLevel;
                public uint Loop;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
                public char[] Duck;
                public uint DuckHash;
                public uint EntityContextType0;
                public uint EntityContextValue0;
                public uint EntityContextType1;
                public uint EntityContextValue1;
                public uint EntityContextType2;
                public uint EntityContextValue2;
                public uint GlobalContextType;
                public uint GlobalContextValue;
            }
            #endregion

            #region Tables
            /// <summary>
            /// Volumne Groups
            /// </summary>
            private static readonly string[] VolumeGroups =
            {
                "grp_air",
                "grp_alerts",
                "grp_ambience",
                "grp_announcer",
                "grp_bink",
                "grp_destructible",
                "grp_explosion",
                "grp_foley",
                "grp_hdrfx",
                "grp_igc",
                "grp_impacts",
                "grp_lfe",
                "grp_master",
                "grp_menu",
                "grp_mp_game",
                "grp_music",
                "grp_physics",
                "grp_player_foley",
                "grp_player_impacts",
                "grp_reference",
                "grp_scripted_moment",
                "grp_set_piece",
                "grp_vehicle",
                "grp_voice",
                "grp_weapon",
                "grp_weapon_3p",
                "grp_weapon_decay_3p",
                "grp_whizby",
                "grp_wpn_lfe",
            };

            /// <summary>
            /// Pans 
            /// </summary>
            private static readonly string[] Pans =
            {
                "default",
                "music",
                "wpn_all",
                "wpn_fnt",
                "wpn_rear",
                "wpn_left",
                "wpn_right",
                "music_all",
                "fly_foot_all",
                "front",
                "back",
                "front_mostly",
                "back_mostly",
                "all",
                "center",
                "center_mostly",
                "front_and_center",
                "lfe",
                "quad",
                "front_mostly_some_center",
                "front_halfback",
                "halffront_back",
                "test",
                "brass_right",
                "brass_left",
                "veh_back",
                "tst_left",
                "tst_center",
                "tst_right",
                "tst_surround_left",
                "tst_surround_right",
                "tst_lfe",
                "tst_back_left",
                "tst_back_right",
                "pip",
                "movie_vo",
            };

            /// <summary>
            /// Flux Types
            /// </summary>
            private static readonly string[] FluxTypes =
            {
                "",
                "left_player",
                "center_player",
                "right_player",
                "random_player",
                "left_shot",
                "center_shot",
                "right_shot",
                "random_direction",
            };

            /// <summary>
            /// Valid Curves
            /// </summary>
            private static readonly string[] Curves =
            {
                "default",
                "defaultmin",
                "allon",
                "alloff",
                "rcurve0",
                "rcurve1",
                "rcurve2",
                "rcurve3",
                "rcurve4",
                "rcurve5",
                "steep",
                "sindelay",
                "cosdelay",
                "sin",
                "cos",
                "rev60",
                "rev65",
            };

            /// <summary>
            /// Duck Groups
            /// </summary>
            private static readonly string[] DuckGroups =
            {
                "snp_alerts_gameplay",
                "snp_ambience",
                "snp_claw",
                "snp_destructible",
                "snp_dying",
                "snp_dying_ice",
                "snp_evt_2d",
                "snp_explosion",
                "snp_foley",
                "snp_grenade",
                "snp_hdrfx",
                "snp_igc",
                "snp_impacts",
                "snp_menu",
                "snp_movie",
                "snp_music",
                "snp_never_duck",
                "snp_player_dead",
                "snp_player_impacts",
                "snp_scripted_moment",
                "snp_set_piece",
                "snp_special",
                "snp_vehicle",
                "snp_vehicle_interior",
                "snp_voice",
                "snp_voice_announcer",
                "snp_weapon_decay_1p",
                "snp_whizby",
                "snp_wpn_1p",
                "snp_wpn_1p_act",
                "snp_wpn_1p_ads",
                "snp_wpn_1p_shot",
                "snp_wpn_3p",
                "snp_wpn_3p_decay",
                "snp_wpn_turret",
                "snp_x2",
                "snp_x3",
                "snp_distant_explosions",
            };

            /// <summary>
            /// Randomization Types
            /// </summary>
            private static readonly string[] RandomizeTypes =
            {
                "variant",
                "volume",
                "pitch",
            };

            /// <summary>
            /// Entity/Standard Limit Types
            /// </summary>
            private static readonly string[] LimitTypes =
            {
                "none",
                "oldest",
                "reject",
                "priority"
            };

            /// <summary>
            /// Storage Types (Primed = Partial Load, Loaded = In-Memory (SABL), Streamed = Loaded on Demand (SABS)
            /// </summary>
            private static readonly string[] StorageTypes =
            {
               "unknown",
               "loaded",
               "streamed",
               "primed",
            };

            /// <summary>
            /// Hashed Strings (Duck and Context Values reference this dictionary)
            /// </summary>
            private static Dictionary<uint, string> HashedStrings = new Dictionary<uint, string>()
            {
                { 0xFDBFA7F2         , "aquifer_cockpit" },
                { 0xA8E1E3AB         , "active" },
                { 0x3C962EFD         , "battle" },
                { 0x59542E3A         , "silent" },
                { 0x6DFCA373         , "slomo" },
                { 0xBE1CADD7         , "boot" },
                { 0x32C00D01         , "wet" },
                { 0x850E1232         , "foley" },
                { 0x9D0CDF4C         , "normal" },
                { 0x2BDD3460         , "igc" },
                { 0xC81277D0         , "healthstate" },
                { 0x2489C328         , "human" },
                { 0x85A08194         , "cyber" },
                { 0xFE6241D9         , "infected" },
                { 0x5A2AFE44         , "on" },
                { 0x770DEBDB         , "laststand" },
                { 0x31026DAD         , "mature" },
                { 0x97C90F59         , "explicit" },
                { 0xCE22AF32         , "safe" },
                { 0x8D844C74         , "movement" },
                { 0x8F66D6B7         , "loud" },
                { 0x85E4DD2F         , "quiet" },
                { 0xA1060CD7         , "perspective" },
                { 0xEC996326         , "player" },
                { 0x2E5C841C         , "npc" },
                { 0x9147790          , "plr_body" },
                { 0x390D0114         , "reaper" },
                { 0x378CB4EF         , "prophet" },
                { 0xA4EB18AA         , "nomad" },
                { 0x64AF02D1         , "outrider" },
                { 0x5ED5591E         , "seraph" },
                { 0xA978154D         , "ruin" },
                { 0xD2DCE0A          , "assassin" },
                { 0x4E6BB40B         , "pyro" },
                { 0x453270DA         , "grenadier" },
                { 0x2E25DAEF         , "plr_gender" },
                { 0xB6FFCC32         , "male" },
                { 0x55EE5431         , "female" },
                { 0xD57307F4         , "plr_impact" },
                { 0x3241FD74         , "veh" },
                { 0x168AC66          , "pwr_armor" },
                { 0xDB25FBBE         , "plr_stance" },
                { 0xDB4F5091         , "stand" },
                { 0xED47D8DF         , "crouch" },
                { 0xB9616B1F         , "prone" },
                { 0x12D993FE         , "plr_vehicle" },
                { 0xECCB65AD         , "driver" },
                { 0x80773E11         , "ringoff_plr" },
                { 0x5FED5218         , "indoor" },
                { 0xCCAF6B37         , "outdoor" },
                { 0xCAA43AE4         , "underwater" },
                { 0xC2290CE3         , "train" },
                { 0x554BCA71         , "country" },
                { 0xE9B422D0         , "city" },
                { 0x82E4D            , "tunnel" },
                { 0xF8B7C1A7         , "vehicle" },
                { 0xE9552675         , "interior" },
                { 0xBABBB083         , "exterior" },
                { 0x805172D2         , "water" },
                { 0x4D705673         , "under" },
                { 0x1E5DB199         , "over" },
                { 0x57A27B3F         , "amb_battle_ducker" },
                { 0x62F3D53          , "amb_battle_ducker_half" },
                { 0xB2CA4596         , "amb_lightning_indoor" },
                { 0x346DB54F         , "amb_lightning_sewer" },
                { 0xE7B4A72E         , "cmn_3p_weapon_occ" },
                { 0x720F1959         , "cmn_aar_screen" },
                { 0x2A05C2D4         , "cmn_bleedout" },
                { 0x3473D6FF         , "cmn_bo3_load" },
                { 0x370FAEF0         , "cmn_cc_securitybreach" },
                { 0x90B85A39         , "cmn_cc_securitybreach_trans" },
                { 0x7A6D9156         , "cmn_cc_tacmenu" },
                { 0xE710269          , "cmn_dead_turret_exp" },
                { 0xD749D398         , "cmn_death_coop" },
                { 0x63AAABAB         , "cmn_death_plr" },
                { 0xBFB3DECA         , "cmn_death_solo" },
                { 0xEB5226C3         , "cmn_dni_interrupt" },
                { 0x734D115B         , "cmn_duck_all" },
                { 0x1F700AAE         , "cmn_duck_all_but_movie" },
                { 0xFE51655F         , "cmn_duck_music" },
                { 0x994D3606         , "cmn_duck_music_dist" },
                { 0xE7179C80         , "cmn_duck_underscore" },
                { 0xADC513C7         , "cmn_duck_underscore_and_round" },
                { 0x1E4A10FD         , "cmn_health_laststand" },
                { 0x9D1BDD11         , "cmn_health_low" },
                { 0x144AE81E         , "cmn_health_resurrect" },
                { 0x96BC2038         , "cmn_igc_amb_silent" },
                { 0xD3041C21         , "cmn_igc_bg_lower" },
                { 0xAA82869F         , "cmn_igc_foley_lower" },
                { 0x64A9F439         , "cmn_igc_skip" },
                { 0xA33BDD93         , "cmn_jump_land_plr" },
                { 0x8E301056         , "cmn_kill_foley" },
                { 0xFCE9FB57         , "cmn_level_fadeout" },
                { 0xDCA66709         , "cmn_level_fade_immediate" },
                { 0x3FCD9007         , "cmn_level_start" },
                { 0x9BFDECE1         , "cmn_melee_pain" },
                { 0xF1DB377A         , "cmn_music_quiet" },
                { 0x724BE457         , "cmn_no_vo" },
                { 0xF0FE02EC         , "cmn_override_duck" },
                { 0xB4F9114D         , "cmn_pain_plr" },
                { 0x6D5C916F         , "cmn_qtank_railgun_shot" },
                { 0xD263BA2E         , "cmn_raps_spawn" },
                { 0xDE71D8AF         , "cmn_robot_behind" },
                { 0x8389F925         , "cmn_shock_tinitus" },
                { 0x934DBB8C         , "cmn_sniper_fire_3rd" },
                { 0x49E144FD         , "cmn_swimming" },
                { 0x8D3D88E9         , "cmn_time_freeze" },
                { 0xCECA2457         , "cmn_ui_tac_mode" },
                { 0x62323141         , "cmn_wallrun" },
                { 0xF4A1624          , "cod_ads" },
                { 0xB9DDDA1A         , "cod_alloff" },
                { 0x628C7954         , "cod_allon" },
                { 0x892D7D94         , "cod_doa_fps" },
                { 0xC4738B0D         , "cod_fadein" },
                { 0x9ED46559         , "cod_hipfire" },
                { 0xFCF06494         , "cod_hold_breath" },
                { 0x741C7F4D         , "cod_matureduck" },
                { 0x82A4A8B          , "cod_menu" },
                { 0xCC0918EC         , "cod_mpl_combat_awareness" },
                { 0xD9F53E8F         , "cod_test_alias" },
                { 0xE996F34C         , "cod_test_env" },
                { 0x6EC2083          , "cod_xcam" },
                { 0x5AACC08E         , "cp_aquifer_breach" },
                { 0xCA84F094         , "cp_aquifer_breach_slowmo" },
                { 0xB7F92C1F         , "cp_aquifer_drown_evt" },
                { 0xE5A3B5B2         , "cp_aquifer_int" },
                { 0xA53C2BF9         , "cp_aquifer_int_deep" },
                { 0x9A6C086E         , "cp_aquifer_outro" },
                { 0xF98CB254         , "cp_aquifer_pip_HeroLocation" },
                { 0x4C7C1142         , "cp_aquifer_script_jet" },
                { 0xD7B9695C         , "cp_aquifer_underwater" },
                { 0x984604F7         , "cp_aquifer_veh_dogfight" },
                { 0x85BF974E         , "cp_aquifer_veh_exp_hit" },
                { 0x1DDCEF4C         , "cp_aquifer_veh_int" },
                { 0x2134DD69         , "cp_barge_slowtime" },
                { 0x21A4FB2F         , "cp_biodome2_slide_prj" },
                { 0x4432AB71         , "cp_biodomes_2_end_vehicle" },
                { 0x4BD5458          , "cp_biodomes_dive_duckambience" },
                { 0xFC4AFD7D         , "cp_biodomes_supertree_collapse" },
                { 0x19E697FB         , "cp_biodomes_vtol_esc" },
                { 0x5DCF51D1         , "cp_biodome_acc_drive_igc" },
                { 0xB08090E4         , "cp_biodome_acc_drive_igc_2" },
                { 0x2768D6E2         , "cp_biodome_supertree_vtol" },
                { 0x176AC7F5         , "cp_blackstation_boatride" },
                { 0x11611B63         , "cp_blackstation_boatride_getoff" },
                { 0xD23755CB         , "cp_blackstation_boatride_geton" },
                { 0x124DB396         , "cp_blackstation_data_recorder" },
                { 0xCF2A4F8          , "cp_blackstation_debris" },
                { 0x39915100         , "cp_blackstation_debris_small" },
                { 0xD28979A9         , "cp_blackstation_intro_veh" },
                { 0x6F44CBC          , "cp_blackstation_intro_veh_2" },
                { 0x3A75224E         , "cp_blackstation_outro" },
                { 0xF363D940         , "cp_blackstation_scripted_wind" },
                { 0xC7CE1E67         , "cp_blackstation_thunder" },
                { 0xF153BCE6         , "cp_blackstation_warlord_igc" },
                { 0xEF1E52E7         , "cp_cybercore_activate" },
                { 0xFAEA02EF         , "cp_cybercore_ready" },
                { 0x9B874EDB         , "cp_cybercore_unstoppable" },
                { 0x653D6895         , "cp_dialog" },
                { 0xD66AA808         , "cp_infection_flyaway" },
                { 0x8937E10A         , "cp_infection_hideout_amb" },
                { 0x72687B60         , "cp_infection_interface" },
                { 0xDE8FDCD3         , "cp_infection_intro" },
                { 0x44A7BAE6         , "cp_infection_intro_2" },
                { 0x852F17E0         , "cp_infection_intro_imp" },
                { 0x9321F700         , "cp_infection_labdeath" },
                { 0x9FC68C1C         , "cp_infection_qt_birth" },
                { 0x67AC9732         , "cp_infection_testlab_transition" },
                { 0xA6691368         , "cp_life_vinetrans" },
                { 0x17FE024D         , "cp_lotus_delusion_overlay" },
                { 0x589D6984         , "cp_lotus_hospital_fade" },
                { 0x37D86E70         , "cp_lotus_vtol_bridge" },
                { 0x9AE9EF           , "cp_lotus_vtol_hallway" },
                { 0xBB56E530         , "cp_mi_sing_vengeance_slowmo" },
                { 0x9349D3E5         , "cp_newworld_pipes" },
                { 0x1FB644FD         , "cp_prologue_apc_door_close" },
                { 0x48F17350         , "cp_prologue_apc_duck_explo" },
                { 0x1CC2DA89         , "cp_prologue_duck_apc_lps" },
                { 0x82BDB1FE         , "cp_prologue_exit_apc" },
                { 0x2044053F         , "cp_prologue_outro_rippedapart" },
                { 0x6F4F0B07         , "cp_prologue_outro_runup" },
                { 0x8A694EA4         , "cp_prologue_vtolflyby" },
                { 0x52520787         , "cp_ramses_celing" },
                { 0xD5C1EE27         , "cp_ramses_demostreet_1" },
                { 0xD5C1EE28         , "cp_ramses_demostreet_2" },
                { 0xD5C1EE29         , "cp_ramses_demostreet_3" },
                { 0x6EED2F63         , "cp_ramses_intro_igc" },
                { 0x906D66C3         , "cp_ramses_int_dead" },
                { 0x685528FA         , "cp_ramses_int_veh" },
                { 0x1B75B840         , "cp_ramses_jeep_drive" },
                { 0xC9C944D5         , "cp_ramses_nasser_igc" },
                { 0x40952F1C         , "cp_ramses_outro" },
                { 0x41FC1B5A         , "cp_ramses_plaza_battle" },
                { 0x704E778          , "cp_ramses_preplaza" },
                { 0x5EE6E3C6         , "cp_ramses_pre_vtol" },
                { 0x397DBE08         , "cp_ramses_qt_vtol" },
                { 0x87443C50         , "cp_ramses_qt_wallcrash" },
                { 0x83EA541C         , "cp_ramses_quad_music" },
                { 0x7CF1828E         , "cp_ramses_raps_intro" },
                { 0x512C6C1F         , "cp_ramses_streetcollapse" },
                { 0x13EB7E43         , "cp_ramses_theater_explo" },
                { 0xFF3F7059         , "cp_ramses_trans" },
                { 0x48C35A50         , "cp_ramses_vtol_fall" },
                { 0xB6C87B5B         , "cp_ramses_vtol_impact" },
                { 0x5FB736FE         , "cp_ramses_vtol_walk" },
                { 0x867431A          , "cp_ramses_wall_3p_gunfire" },
                { 0xC9BEC057         , "cp_safehouse_exit" },
                { 0xA759B0C7         , "cp_sgen_base_explo" },
                { 0xA4CEBE99         , "cp_sgen_flooding" },
                { 0x9B96172          , "cp_sgen_flyover" },
                { 0x5809DF20         , "cp_sgen_ghost_igc" },
                { 0xD42A525D         , "cp_sgen_steam_duck" },
                { 0xD9AE7373         , "cp_sgen_twinrevenge_igc" },
                { 0x22684065         , "cp_sgen_uw_boulder" },
                { 0x690C6B6E         , "cp_sgen_wave" },
                { 0xA4C3238          , "cp_sh_cairo_foley_low" },
                { 0xA972591D         , "cp_vengeance_cafe" },
                { 0x87D5598F         , "cp_vengeance_int" },
                { 0x86863C5C         , "cp_vengeance_int_deep" },
                { 0xC5D09945         , "cp_warlord_melee" },
                { 0xE176A162         , "cp_zmb_box_3d" },
                { 0x42AEC0C4         , "cp_zmb_ending" },
                { 0x700FE2C5         , "cp_zmb_thelooper" },
                { 0x8B9B50E1         , "cp_zmb_timefreeze" },
                { 0xDBF3C435         , "cp_zmb_voice" },
                { 0x19E2FEAE         , "cp_zurich_duckrabbit" },
                { 0x8B5CB328         , "cp_zurich_end_duckexplo" },
                { 0xCD080995         , "cp_zurich_movie" },
                { 0xE5F706           , "cp_zurich_movie_long" },
                { 0x29BBCF4D         , "cp_zurich_train" },
                { 0xF680CFBC         , "default" },
                { 0x14E44A23         , "dummy" },
                { 0x70EDC08D         , "exp_barrel" },
                { 0x3BC2AC7          , "exp_grenade" },
                { 0xE4C9783C         , "exp_medium" },
                { 0x1CBB3E5C         , "exp_mortar" },
                { 0x2B7FE0A5         , "exp_quad_rocket" },
                { 0xB2722054         , "exp_rocket_close" },
                { 0x89BE6CB          , "exp_rocket_quad" },
                { 0xBE1A36A0         , "exp_small" },
                { 0x54265E65         , "exp_vehicle" },
                { 0x548630DE         , "exp_vehicle_close" },
                { 0x953869EE         , "mpl_announcer" },
                { 0x76A6C999         , "mpl_death" },
                { 0x11AAB05E         , "mpl_duck_announcer" },
                { 0x120D7241         , "mpl_duck_holoscreen" },
                { 0x93C7C225         , "mpl_endmatch" },
                { 0xCADE9E4D         , "mpl_final_killcam" },
                { 0x9121FD95         , "mpl_final_killcam_slowdown" },
                { 0x45B83F31         , "mpl_hellstorm" },
                { 0xC0A241CA         , "mpl_postmatch" },
                { 0x36B7DBE1         , "mpl_post_match" },
                { 0x462809D          , "mpl_prematch" },
                { 0xA0808543         , "mpl_speedboost_run" },
                { 0xAF7FEE02         , "prj_impact" },
                { 0x94450659         , "prj_impact_plr" },
                { 0x58EE3295         , "prj_whizby" },
                { 0xA19754E6         , "wpn_cmn_burst_3p" },
                { 0xFCD577F8         , "wpn_cmn_shot_3p" },
                { 0xCE86373B         , "wpn_cmn_shot_plr" },
                { 0x36B86475         , "wpn_cmn_suppressed_plr" },
                { 0x945C16D6         , "wpn_hunter_missile" },
                { 0x4927A700         , "wpn_jet_rocket_plr" },
                { 0x60191BA2         , "wpn_melee_knife_plr" },
                { 0xE66DFA51         , "wpn_rpg_plr" },
                { 0x2E895CD1         , "wpn_shotgun_sci" },
                { 0xF41A026C         , "wpn_shoulder_shot_npc" },
                { 0xD5B16432         , "wpn_sniper_shot_plr" },
                { 0x330A3541         , "wpn_turret_npc" },
                { 0x34025356         , "wpn_turret_plr" },
                { 0x17229E88         , "zmb_apothifight_beam" },
                { 0x351E8486         , "zmb_beastmode_enter" },
                { 0xF88C2309         , "zmb_bgb_fearinheadlights" },
                { 0x5DE9E91A         , "zmb_bgb_killingtime" },
                { 0xC15BEB77         , "zmb_castle_bossbattle" },
                { 0xAAC84C32         , "zmb_castle_bossbattle_event" },
                { 0x83974EC1         , "zmb_castle_duck_evt_3d" },
                { 0x72F33E79         , "zmb_castle_outro" },
                { 0x5CDCAEF9         , "zmb_castle_timetravel" },
                { 0x8A78E805         , "zmb_cmn_mk3_orb" },
                { 0xAA18BC01         , "zmb_cosmo_intro" },
                { 0xA475DE02         , "zmb_cp_song_suppress" },
                { 0x2DAB7127         , "zmb_derriese_start" },
                { 0x31D6507D         , "zmb_dialog" },
                { 0x16B36AD4         , "zmb_dialog_2d" },
                { 0x9784980F         , "zmb_dialog_monty" },
                { 0x8B0A78DF         , "zmb_duck_close_vehicles" },
                { 0x866CADBC         , "zmb_duck_music_3d" },
                { 0x5DB9A18F         , "zmb_easter_egg_song" },
                { 0x2736A34C         , "zmb_game_over" },
                { 0xFA69CCAA         , "zmb_game_start" },
                { 0xFF17DE32         , "zmb_game_start_nofade" },
                { 0x2E0A7E39         , "zmb_hd_game_start_nofade" },
                { 0x495426C6         , "zmb_health_low" },
                { 0x8966D755         , "zmb_island_dopple_scream" },
                { 0xB9B1301C         , "zmb_island_duck_bg_music" },
                { 0xB098EEF1         , "zmb_island_hallucinate" },
                { 0x3B5DF753         , "zmb_island_inside_thrasher" },
                { 0x1C860C72         , "zmb_island_swimming" },
                { 0x90A12A93         , "zmb_island_takeo" },
                { 0xA694E741         , "zmb_island_trial" },
                { 0xC51DDF6B         , "zmb_laststand" },
                { 0xC7994A85         , "zmb_margwa_smash" },
                { 0xFD56227C         , "zmb_moon_gasmask" },
                { 0xC1563D7D         , "zmb_moon_space" },
                { 0x6BDC6DF2         , "zmb_radio_duck" },
                { 0xCDE238E1         , "zmb_sophia" },
                { 0x9261F1F9         , "zmb_stalingrad_pa_destruct" },
                { 0x7C27C9FC         , "zmb_stal_boss_fight" },
                { 0x28F00F42         , "zmb_stal_dragon_fight" },
                { 0x53ECAD8D         , "zmb_stal_outro" },
                { 0x297961D7         , "zmb_stal_tbc" },
                { 0xB3584229         , "zmb_temple_meteor" },
                { 0x990E817E         , "zmb_temple_radio" },
                { 0x37C6BFD3         , "zmb_umonkey" },
                { 0xC2F80042         , "zmb_zhd_laststand" },
                { 0x9F3BD446         , "zmb_zod_apothibattle_duck" },
                { 0x59FDD9A2         , "zmb_zod_apothigod" },
                { 0x4BE4B87D         , "zmb_zod_beastmode" },
                { 0x7AEE79F7         , "zmb_zod_cursed" },
                { 0xC1501851         , "zmb_zod_duck_octobomb_lp" },
                { 0x3B869E6F         , "zmb_zod_endigc" },
                { 0x102D05D4         , "zmb_zod_scream" },
                { 0x94489423         , "zmb_zod_shadbattle_duck" },
                { 0x6FD35478         , "zmb_zod_sword" },
                { 0x6A7C6699         , "zmb_zod_sword_powerup" },
                { 0x2E1119D0         , "zmb_zod_teleport" },
                { 0x61205857         , "zmb_zod_totem_charge" },
            };

            /// <summary>
            /// Buses (Match Audio Sliders in Sound Menu)
            /// </summary>
            private static readonly string[] Buses =
            {
                "BUS_FX",
                "BUS_VOICE",
                "BUS_PFUTZ",
                "BUS_HDRFX",
                "BUS_UI",
                "BUS_MUSIC",
                "BUS_MOVIE",
                "BUS_REFERENCE",
            };
            #endregion

            #region HelperMethods
            /// <summary>
            /// Returns a string that matches hash from string hashed string table.
            /// </summary>
            /// <param name="hash">Hash Value</param>
            /// <returns>String if in table, otherwise empty string</returns>
            public static string GetHashedString(uint hash)
            {
                return HashedStrings.ContainsKey(hash) ? HashedStrings[hash] : "";
            }

            /// <summary>
            /// Returns Pitch Value from Short
            /// </summary>
            /// <param name="input">Pitch Value Between 0 and 65534</param>
            /// <returns>CSV Pitch Value</returns>
            public static int CalculatePitch(ushort input)
            {
                return (int)Math.Ceiling((Math.Log(input / 32768.0, 2) * 1200));
            }

            /// <summary>
            /// Calculates alias values that are between 0-100 (Vol, Reverb etc)
            /// </summary>
            /// <param name="input">Input Value</param>
            /// <returns>Value between 0-100</returns>
            public static double Calculate100Value(float input)
            {
                return input > 0 ? Math.Round((Math.Log(input, 10.0) / 0.05) + 100.0, 2) : 0;
            }
            #endregion

            /// <summary>
            /// Size of each asset
            /// </summary>
            public int AssetSize { get; set; }

            /// <summary>
            /// Gets or Sets the number of Assets 
            /// </summary>
            public int AssetCount { get; set; }

            /// <summary>
            /// Gets or Sets the Start Address
            /// </summary>
            public long StartAddress { get; set; }

            /// <summary>
            /// Gets or Sets the End Address
            /// </summary>
            public long EndAddress { get { return StartAddress + (AssetCount * AssetSize); } set => throw new NotImplementedException(); }

            /// <summary>
            /// Gets the Name of this Pool
            /// </summary>
            public string Name => "sound";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Sound";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.sound;

            /// <summary>
            /// Loads Assets from this Asset Pool
            /// </summary>
            public List<Asset> Load(HydraInstance instance)
            {
                var results = new List<Asset>();

                var poolInfo = instance.Reader.ReadStruct<AssetPoolInfo>(instance.Game.AssetPoolsAddress + (Index * 0x20));

                StartAddress = poolInfo.PoolPointer;
                AssetSize = poolInfo.AssetSize;
                AssetCount = poolInfo.PoolSize;
                AliasHashes.Clear();
                // For dumping Hashes for SAB
                // using (var writer = new StreamWriter("Aliases.csv"))
                {
                    for (int i = 0; i < AssetCount; i++)
                    {
                        var header = instance.Reader.ReadStruct<SoundAsset>(StartAddress + (i * AssetSize));

                        if (IsNullAsset(header.NamePointer))
                            continue;

                        var aliases = instance.Reader.ReadArrayUnsafe<SoundAlias>(header.AliasesPointer, header.AliasCount);

                        // For dumping Hashes for SAB
                        foreach (var alias in aliases)
                        {
                            AliasHashes[alias.Hash] = instance.Reader.ReadNullTerminatedString(alias.NamePointer);

                            //var entries = instance.Reader.ReadArray<SoundAliasEntry>(alias.EntiresPointer, alias.EntryCount);

                            //foreach (var entry in entries)
                            //{
                            //    if (entry.FileSpec.Hash != 0)
                            //        writer.WriteLine("{0:x},{1}", entry.FileSpec.Hash, instance.Reader.ReadNullTerminatedString(entry.FileSpec.FileNamePointer));
                            //    if (entry.FileSpecRelease.Hash != 0)
                            //        writer.WriteLine("{0:x},{1}", entry.FileSpecRelease.Hash, instance.Reader.ReadNullTerminatedString(entry.FileSpecRelease.FileNamePointer));
                            //    if (entry.FileSpecSustain.Hash != 0)
                            //        writer.WriteLine("{0:x},{1}", entry.FileSpecSustain.Hash, instance.Reader.ReadNullTerminatedString(entry.FileSpecSustain.FileNamePointer));
                            //}
                        }

                        var address = StartAddress + (i * AssetSize);

                        results.Add(new Asset()
                        {
                            Name = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                            Type        = Name,
                            Status      = "Loaded",
                            Data        = address,
                            LoadMethod  = ExportAsset,
                            Zone = ((BlackOps3)instance.Game).ZoneNames[address],
                            Information = string.Format("Aliases: {0}", header.AliasCount)
                        });
                    }
                }
                return results;
            }

            private static MusicSetObj[] ConvertMusicSetsToMusicSetObj(long musicSetsPointer, int musicSetCount, HydraInstance instance)
            {
                var results = new MusicSetObj[musicSetCount];

                var musicSets = instance.Reader.ReadArray<MusicSet>(musicSetsPointer, musicSetCount);

                for(int i = 0; i < musicSetCount; i++)
                {
                    results[i] = new MusicSetObj()
                    {
                        Name = new string(musicSets[i].Name).TrimEnd('\0'),
                        StateNames = new string[musicSets[i].StateCount],
                        LoopIndex = musicSets[i].StateCount,
                        StateArray = new MusicSetObj.StateObj[musicSets[i].StateCount]
                    };

                    var musicStates = instance.Reader.ReadArray<MusicState>(musicSets[i].StatesPointer, musicSets[i].StateCount);

                    for(int j = 0; j < musicSets[i].StateCount; j++)
                    {
                        string introAssetName = new string(musicStates[j].IntroAsset.Name).TrimEnd('\0');
                        string exitAssetName = new string(musicStates[j].ExitAsset.Name).TrimEnd('\0');

                        results[i].StateArray[j] = new MusicSetObj.StateObj()
                        {
                            Name = new string(musicStates[j].Name).TrimEnd('\0'),
                            IntroAsset = string.IsNullOrWhiteSpace(introAssetName) ? null : new MusicSetObj.StateObj.AssetObj()
                            {
                                AliasName         = introAssetName,
                                Looping           = musicStates[j].IntroAsset.Looping,
                                CompleteLoop      = musicStates[j].IntroAsset.CompleteLoop,
                                RemoveAfterPlay   = musicStates[j].IntroAsset.RemoveAfterPlay,
                                PlayAsFirstRandom = musicStates[j].IntroAsset.PlayAsFirstRandom,
                                StartSync         = musicStates[j].IntroAsset.StartSync,
                                StopSync          = musicStates[j].IntroAsset.StopSync,
                                CompleteOnStop    = musicStates[j].IntroAsset.CompleteOnStop,
                                LoopStartOffset   = musicStates[j].IntroAsset.LoopStartOffset,
                                BPM               = musicStates[j].IntroAsset.BPM,
                                AssetType         = musicStates[j].IntroAsset.AssetType,
                                LoopNumber        = musicStates[j].IntroAsset.LoopNumber,
                                Order             = musicStates[j].IntroAsset.Order,
                                StartDelayBeats   = musicStates[j].IntroAsset.StopDelayBeats,
                                StartFadeBeats    = musicStates[j].IntroAsset.StartFadeBeats,
                                StopDelayBeats    = musicStates[j].IntroAsset.StopDelayBeats,
                                StopFadeBeats     = musicStates[j].IntroAsset.StopFadeBeats,
                                Meter             = musicStates[j].IntroAsset.Meter
                            },
                            ExitAsset = string.IsNullOrWhiteSpace(exitAssetName) ? null : new MusicSetObj.StateObj.AssetObj()
                            {
                                AliasName         = exitAssetName,
                                Looping           = musicStates[j].ExitAsset.Looping,
                                CompleteLoop      = musicStates[j].ExitAsset.CompleteLoop,
                                RemoveAfterPlay   = musicStates[j].ExitAsset.RemoveAfterPlay,
                                PlayAsFirstRandom = musicStates[j].ExitAsset.PlayAsFirstRandom,
                                StartSync         = musicStates[j].ExitAsset.StartSync,
                                StopSync          = musicStates[j].ExitAsset.StopSync,
                                CompleteOnStop    = musicStates[j].ExitAsset.CompleteOnStop,
                                LoopStartOffset   = musicStates[j].ExitAsset.LoopStartOffset,
                                BPM               = musicStates[j].ExitAsset.BPM,
                                AssetType         = musicStates[j].ExitAsset.AssetType,
                                LoopNumber        = musicStates[j].ExitAsset.LoopNumber,
                                Order             = musicStates[j].ExitAsset.Order,
                                StartDelayBeats   = musicStates[j].ExitAsset.StopDelayBeats,
                                StartFadeBeats    = musicStates[j].ExitAsset.StartFadeBeats,
                                StopDelayBeats    = musicStates[j].ExitAsset.StopDelayBeats,
                                StopFadeBeats     = musicStates[j].ExitAsset.StopFadeBeats,
                                Meter             = musicStates[j].ExitAsset.Meter
                            },
                            Order            = musicStates[j].Order,
                            IsRandom         = musicStates[j].IsRandom,
                            IsSequential     = musicStates[j].IsSequential,
                            SkipPreviousExit = musicStates[j].SkipPreviousExit,
                            LoopAssets       = new MusicSetObj.StateObj.AssetObj[musicStates[j].LoopAssetCount]
                        };
                        results[i].StateNames[j] = results[i].StateArray[j].Name;


                        if (musicStates[j].LoopAssetCount > 0 && musicStates[j].LoopAssetsPointer > 0)
                        {
                            var loopAssets = instance.Reader.ReadArray<MusicStateAsset>(musicStates[j].LoopAssetsPointer, (int)musicStates[j].LoopAssetCount);

                            for (int x = 0; x < musicStates[j].LoopAssetCount; x++)
                            {
                                results[i].StateArray[j].LoopAssets[x] = new MusicSetObj.StateObj.AssetObj()
                                {
                                    AliasName         = new string(loopAssets[x].Name).TrimEnd('\0'),
                                    Looping           = loopAssets[x].Looping,
                                    CompleteLoop      = loopAssets[x].CompleteLoop,
                                    RemoveAfterPlay   = loopAssets[x].RemoveAfterPlay,
                                    PlayAsFirstRandom = loopAssets[x].PlayAsFirstRandom,
                                    StartSync         = loopAssets[x].StartSync,
                                    StopSync          = loopAssets[x].StopSync,
                                    CompleteOnStop    = loopAssets[x].CompleteOnStop,
                                    LoopStartOffset   = loopAssets[x].LoopStartOffset,
                                    BPM               = loopAssets[x].BPM,
                                    AssetType         = loopAssets[x].AssetType,
                                    LoopNumber        = loopAssets[x].LoopNumber,
                                    Order             = loopAssets[x].Order,
                                    StartDelayBeats   = loopAssets[x].StopDelayBeats,
                                    StartFadeBeats    = loopAssets[x].StartFadeBeats,
                                    StopDelayBeats    = loopAssets[x].StopDelayBeats,
                                    StopFadeBeats     = loopAssets[x].StopFadeBeats,
                                    Meter             = loopAssets[x].Meter
                                };
                            }
                        }
                    }
                }

                return results;
            }

            /// <summary>
            /// Exports Reverbs
            /// </summary>
            private static string ConvertReverbsToCSVString(Reverb[] reverbs)
            {
                var result = new StringBuilder();
                var properties = typeof(Reverb).GetFields();

                foreach (var property in properties)
                    if(property.Name != "Hash")
                        result.Append(property.Name + ",");
                result.AppendLine();

                foreach (var reverb in reverbs)
                {
                    // Add name to hash table
                    HashedStrings[reverb.Hash] = reverb.Name.ToString().TrimEnd('\0');

                    foreach (var property in properties)
                    {
                        switch (property.Name)
                        {
                            case "Name":
                                result.Append(new string((char[])property.GetValue(reverb)).TrimEnd('\0') + ",");
                                break;
                            case "Hash":
                                break;
                            default:
                                result.Append(property.GetValue(reverb).ToString() + ",");
                                break;
                        }
                    }
                    result.AppendLine();
                }

                return result.ToString();
            }

            /// <summary>
            /// Converts the loaded aliases to a sound source obj
            /// </summary>
            private static SoundSourceObj ConvertAliasesToSoundSourceObj(SoundAlias[] aliases, HydraInstance instance)
            {
                var sourceFile = new SoundSourceObj()
                {
                    Aliases = new SoundSourceObj.Alias[aliases.Length]
                };

                for (int i = 0; i < aliases.Length; i++)
                {
                    var aliasName = instance.Reader.ReadNullTerminatedString(aliases[i].NamePointer);
                    var aliasEntries = instance.Reader.ReadArray<SoundAliasEntry>(aliases[i].EntiresPointer, aliases[i].EntryCount);

                    sourceFile.Aliases[i] = new SoundSourceObj.Alias()
                    {
                        Entries = new SoundSourceObj.Alias.Entry[aliases[i].EntryCount]
                    };

                    for (int j = 0; j < aliases[i].EntryCount; j++)
                    {
                        sourceFile.Aliases[i].Entries[j] = new SoundSourceObj.Alias.Entry();

                        var entry = aliasEntries[j];

                        sourceFile.Aliases[i].Entries[j].Name = aliasName;
                        sourceFile.Aliases[i].Entries[j].Subtitle = instance.Reader.ReadNullTerminatedString(entry.SubtitlePointer);
                        sourceFile.Aliases[i].Entries[j].Secondary = instance.Reader.ReadNullTerminatedString(entry.SecondaryPointer.StringPointer);
                        sourceFile.Aliases[i].Entries[j].StopAlias = instance.Reader.ReadNullTerminatedString(entry.StopAlias.StringPointer);

                        sourceFile.Aliases[i].Entries[j].FileSpec = instance.Game.CleanAssetName(HydraAssetType.Sound, instance.Reader.ReadNullTerminatedString(entry.FileSpec.FileNamePointer));
                        sourceFile.Aliases[i].Entries[j].FileSpecSustain = instance.Game.CleanAssetName(HydraAssetType.Sound, instance.Reader.ReadNullTerminatedString(entry.FileSpecSustain.FileNamePointer));
                        sourceFile.Aliases[i].Entries[j].FileSpecRelease = instance.Game.CleanAssetName(HydraAssetType.Sound, instance.Reader.ReadNullTerminatedString(entry.FileSpecRelease.FileNamePointer));

                        sourceFile.Aliases[i].Entries[j].Looping         = Bytes.GetBit(entry.Flags0, 0) != 0 ? "looping" : "nonlooping";
                        sourceFile.Aliases[i].Entries[j].PanType         = Bytes.GetBit(entry.Flags0, 1) != 0 ? "3d" : "2d";
                        sourceFile.Aliases[i].Entries[j].SilentInCPZ     = Bytes.GetBit(entry.Flags0, 2) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].ContextFailsafe = Bytes.GetBit(entry.Flags0, 3) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].DistanceLpf     = Bytes.GetBit(entry.Flags0, 4) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].Doppler         = Bytes.GetBit(entry.Flags0, 5) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].Pauseable       = Bytes.GetBit(entry.Flags0, 6) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].IsMusic         = Bytes.GetBit(entry.Flags0, 7) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].StopOnEntDeath  = Bytes.GetBit(entry.Flags0, 8) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].Timescale       = Bytes.GetBit(entry.Flags0, 9) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].VoiceLimit      = Bytes.GetBit(entry.Flags0, 10) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].IgnoreMaxDist   = Bytes.GetBit(entry.Flags0, 11) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].GPAD            = Bytes.GetBit(entry.Flags0, 25) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].GPADOnly        = Bytes.GetBit(entry.Flags0, 26) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].MuteVoice       = Bytes.GetBit(entry.Flags0, 27) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].MuteMusic       = Bytes.GetBit(entry.Flags0, 28) != 0 ? "yes" : "no";

                        sourceFile.Aliases[i].Entries[j].NeverPlayTwice      = Bytes.GetBit(entry.Flags1, 0) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].IsCinematic         = Bytes.GetBit(entry.Flags1, 27) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].IsBig               = Bytes.GetBit(entry.Flags1, 28) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].AmplitudePriority   = Bytes.GetBit(entry.Flags1, 29) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].ContinuousPan       = Bytes.GetBit(entry.Flags1, 30) != 0 ? "yes" : "no";
                        sourceFile.Aliases[i].Entries[j].RestartContextLoops = Bytes.GetBit(entry.Flags1, 31) != 0 ? "yes" : "no";

                        sourceFile.Aliases[i].Entries[j].Storage = StorageTypes[(entry.Flags0 >> 12) & 3];

                        sourceFile.Aliases[i].Entries[j].LimitType        = LimitTypes[(entry.Flags0 >> 0x12) & 3];
                        sourceFile.Aliases[i].Entries[j].EntityLimitType  = LimitTypes[(entry.Flags0 >> 0x14) & 3];
                        sourceFile.Aliases[i].Entries[j].LimitCount       = entry.LimitCount;
                        sourceFile.Aliases[i].Entries[j].EntityLimitCount = entry.EntityLimitCount;

                        sourceFile.Aliases[i].Entries[j].DryMinCurve = Curves[(entry.Flags1 >> 0xE)     & 0x3F];
                        sourceFile.Aliases[i].Entries[j].DryMaxCurve = Curves[(entry.Flags1 >> 0x2)     & 0x3F];
                        sourceFile.Aliases[i].Entries[j].WetMinCurve = Curves[(entry.Flags1 >> 0x14)    & 0x3F];
                        sourceFile.Aliases[i].Entries[j].WetMaxCurve = Curves[(entry.Flags1 >> 0x8)     & 0x3F];

                        sourceFile.Aliases[i].Entries[j].Duck          = GetHashedString(entry.Duck);
                        sourceFile.Aliases[i].Entries[j].ContextType   = GetHashedString(entry.Context0.Type);
                        sourceFile.Aliases[i].Entries[j].ContextValue  = GetHashedString(entry.Context0.Value);
                        sourceFile.Aliases[i].Entries[j].ContextType1  = GetHashedString(entry.Context1.Type);
                        sourceFile.Aliases[i].Entries[j].ContextValue1 = GetHashedString(entry.Context1.Value);
                        sourceFile.Aliases[i].Entries[j].ContextType2  = GetHashedString(entry.Context2.Type);
                        sourceFile.Aliases[i].Entries[j].ContextValue2 = GetHashedString(entry.Context2.Value);
                        sourceFile.Aliases[i].Entries[j].ContextType3  = GetHashedString(entry.Context3.Type);
                        sourceFile.Aliases[i].Entries[j].ContextValue3 = GetHashedString(entry.Context3.Value);

                        sourceFile.Aliases[i].Entries[j].ReverbSend     = Calculate100Value(entry.ReverbSend);
                        sourceFile.Aliases[i].Entries[j].CenterSend     = Calculate100Value(entry.ReverbCenter);
                        sourceFile.Aliases[i].Entries[j].VolMin         = Calculate100Value(entry.VolMin);
                        sourceFile.Aliases[i].Entries[j].VolMax         = Calculate100Value(entry.VolMax);
                        sourceFile.Aliases[i].Entries[j].EnvelopPercent = Calculate100Value(entry.EnvelopPercent);

                        sourceFile.Aliases[i].Entries[j].PitchMin = CalculatePitch(entry.PitchMin);
                        sourceFile.Aliases[i].Entries[j].PitchMax = CalculatePitch(entry.PitchMax);

                        sourceFile.Aliases[i].Entries[j].DistMin        = (entry.DistMin * 2);
                        sourceFile.Aliases[i].Entries[j].DistMaxDry     = (entry.DistMaxDry * 2);
                        sourceFile.Aliases[i].Entries[j].DistMaxWet     = (entry.DistMaxWet * 2);
                        sourceFile.Aliases[i].Entries[j].EnvelopMin     = (entry.EnvelopMin * 2);
                        sourceFile.Aliases[i].Entries[j].EnvelopMax     = (entry.EnvelopMax * 2);
                        sourceFile.Aliases[i].Entries[j].DistanceLpfMin = entry.DistanceLpfMin;
                        sourceFile.Aliases[i].Entries[j].DistanceLpfMax = entry.DistanceLpfMax;

                        sourceFile.Aliases[i].Entries[j].StartDelay = entry.StartDelay;
                        sourceFile.Aliases[i].Entries[j].FadeIn = entry.FadeIn;
                        sourceFile.Aliases[i].Entries[j].FadeOut = entry.FadeOut;

                        sourceFile.Aliases[i].Entries[j].StopOnPlay = GetHashedString(entry.StopOnPlay);
                        sourceFile.Aliases[i].Entries[j].FutzPatch  = GetHashedString(entry.FutzPatch);


                        sourceFile.Aliases[i].Entries[j].PriorityThresholdMin = Math.Round(entry.PriorityThresholdMin / 255.0, 2);
                        sourceFile.Aliases[i].Entries[j].PriorityThresholdMax = Math.Round(entry.PriorityThresholdMax / 255.0, 2);
                        sourceFile.Aliases[i].Entries[j].PriorityMin          = entry.PriorityMin;
                        sourceFile.Aliases[i].Entries[j].PriorityMax          = entry.PriorityMax;


                        sourceFile.Aliases[i].Entries[j].Probability    = Math.Round(entry.Probability / 255.0, 2);
                        sourceFile.Aliases[i].Entries[j].OcclusionLevel = Math.Round(entry.OcclusionLevel / 255.0, 2);


                        sourceFile.Aliases[i].Entries[j].Pan         = Pans[entry.Pan];
                        sourceFile.Aliases[i].Entries[j].DuckGroup   = DuckGroups[entry.DuckGroup];
                        sourceFile.Aliases[i].Entries[j].Bus         = Buses[entry.Bus];
                        sourceFile.Aliases[i].Entries[j].VolumeGroup = VolumeGroups[entry.VolumeGroup];

                        sourceFile.Aliases[i].Entries[j].FluxTime = entry.FluxTime;
                        sourceFile.Aliases[i].Entries[j].FluxType = FluxTypes[(entry.Flags0 >> 0xE) & 0xF];
                    }
                }

                return sourceFile;
            }



            private static string ConvertAmbientsToCSVString(AmbientEntry[] ambients)
            {
                var result = new StringBuilder();

                var properties = typeof(AmbientEntry).GetFields();

                foreach (var property in properties)
                    if (!property.Name.Contains("Hash"))
                        result.Append(property.Name + ",");
                result.AppendLine();

                foreach (var ambient in ambients)
                {
                    foreach (var property in properties)
                    {
                        if (!property.Name.Contains("Hash"))
                        {
                            if (property.FieldType.Name == "Char[]")
                            {
                                result.Append(new string((char[])property.GetValue(ambient)).ToString().TrimEnd('\0') + ",");
                            }
                            else if (property.FieldType.Name == "UInt32")
                            {
                                result.Append(GetHashedString((uint)property.GetValue(ambient)) + ",");
                            }
                            else if (property.FieldType.Name == "Boolean")
                            {
                                result.Append(((bool)property.GetValue(ambient) == true ? "yes" : "") + ",");
                            }
                            else
                            {
                                result.Append(property.GetValue(ambient).ToString() + ",");
                            }
                        }
                    }
                    result.AppendLine();
                }

                return result.ToString();
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<SoundAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer).Split(':')[0])
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                Directory.CreateDirectory(instance.SoundZoneFolder);
                Directory.CreateDirectory(instance.SoundMusicFolder);

                var musicSetNames = new string[header.MusicSetCount];
                var musicSets = ConvertMusicSetsToMusicSetObj(header.MusicSetsPointer, header.MusicSetCount, instance);

                for(int i = 0; i < header.MusicSetCount; i++)
                {
                    musicSets[i].Save(Path.Combine(instance.SoundMusicFolder, musicSets[i].Name + ".mus"));
                    musicSetNames[i] = musicSets[i].Name;
                }

                File.WriteAllText(Path.Combine(instance.SoundZoneFolder, asset.Name + ".reverb.csv"), ConvertReverbsToCSVString(instance.Reader.ReadArray<Reverb>(header.ReverbsPointer, header.ReverbCount)));
                File.WriteAllText(Path.Combine(instance.SoundZoneFolder, asset.Name + ".ambient.csv"), ConvertAmbientsToCSVString(instance.Reader.ReadArray<AmbientEntry>(header.UnkPointer, header.UnkCount)));

                var sourceObj = ConvertAliasesToSoundSourceObj(instance.Reader.ReadArray<SoundAlias>(header.AliasesPointer, header.AliasCount), instance);

                sourceObj.Save(Path.Combine(instance.SoundZoneFolder, asset.Name + ".alias.csv"));

                using (var output = new StreamWriter(Path.Combine(instance.SoundZoneFolder, asset.Name + ".musiclist.csv")))
                {
                    output.WriteLine("Name");
                    foreach (var musicSetName in musicSetNames)
                        output.WriteLine(musicSetName);
                }

                return;
            }

            /// <summary>
            /// Checks if the given asset is a null slot
            /// </summary>
            public bool IsNullAsset(Asset asset)
            {
                return IsNullAsset((long)asset.Data);
            }

            /// <summary>
            /// Checks if the given asset is a null slot
            /// </summary>
            public bool IsNullAsset(long nameAddress)
            {
                return nameAddress >= StartAddress && nameAddress <= EndAddress || nameAddress == 0;
            }
        }
    }
}
