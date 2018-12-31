using Newtonsoft.Json;
using PhilLibX;
using System;
using System.Collections.Generic;
using System.IO;
using HydraLib.Assets;
using System.Linq;
using System.Runtime.InteropServices;

namespace HydraLib.Games
{
    public partial class BlackOps3
    {
        public class Sound
        {
            /// <summary>
            /// Volumne Groups
            /// </summary>
            public static string[] VolumeGroups =
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
            public static string[] Pans =
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
            public static string[] FluxTypes =
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
            public static string[] Curves =
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
            public static string[] DuckGroups =
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
            public static string[] RandomizeTypes =
            {
                "variant",
                "volume",
                "pitch",
            };

            /// <summary>
            /// Entity/Standard Limit Types
            /// </summary>
            public static string[] LimitTypes =
            {
                "none",
                "oldest",
                "reject",
                "priority"
            };

            /// <summary>
            /// Hashed Strings (Duck and Context Values reference this dictionary)
            /// </summary>
            public static Dictionary<uint, string> HashedStrings = new Dictionary<uint, string>()
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
            public static string[] Buses =
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
            public static int Calculate100Value(int input)
            {
                return input > 0 ? (int)((1 - ((0x3F800000 - input) / 140608402.0)) * 100.0) : 0;
            }

            /// <summary>
            /// Alias Hashes and Names
            /// </summary>
            public static Dictionary<uint, string> AliasHashes = new Dictionary<uint, string>();

            /// <summary>
            /// Sound Asset Header
            /// </summary>
            public struct SoundHeader
            {
                /// <summary>
                /// Pointer to the name of the Sound Asset Bank
                /// </summary>
                public long SABNamePointer { get; set; }

                /// <summary>
                /// Number of the name of the Zone
                /// </summary>
                public long ZoneNamePointer { get; set; }

                /// <summary>
                /// Number of the language ID
                /// </summary>
                public long LanguageIDPointer { get; set; }

                /// <summary>
                /// Number of the language
                /// </summary>
                public long LanguagePointer { get; set; }

                /// <summary>
                /// Number of sound aliases
                /// </summary>
                public int AliasCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                public int Padding { get; set; }

                /// <summary>
                /// Pointer to the sound aliases
                /// </summary>
                public long AliasesPointer { get; set; }

                /// <summary>
                /// Unknown Pointer relating to sound aliases, possibly audio
                /// </summary>
                public long UnknownPointer { get; set; }
            }

            public struct SoundAliasHeader
            {
                /// <summary>
                /// Pointer to the name of the Sound Alias
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// SDBM Hash of the Alias Name (Hash with an initial value of 0x1505)
                /// </summary>
                public uint Hash { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }

                /// <summary>
                /// Pointer to the Sound Alias Entires/Variations
                /// </summary>
                public long EntiresPointer { get; set; }

                /// <summary>
                /// Number of Sound Alias Entries
                /// </summary>
                public int EntryCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding1 { get; set; }

                /// <summary>
                /// Max Distance this alias can be heard at
                /// </summary>
                public float MaxDistanceWet { get; set; }

                /// <summary>
                /// Unknown Int (Constant 1?)
                /// </summary>
                public int UnknownInt { get; set; }
            }

            /// <summary>
            /// Sound Alias Entry
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct SoundAliasEntry
            {
                public struct SoundAliasName
                {
                    /// <summary>
                    /// Pointer to the string
                    /// </summary>
                    public long StringPointer { get; set; }

                    /// <summary>
                    /// SDBM Hash of the String (Hash with an initial value of 0x1505)
                    /// </summary>
                    public uint Hash { get; set; }

                    /// <summary>
                    /// Null Padding
                    /// </summary>
                    private int Padding { get; set; }
                }

                public struct SoundAliasFileSpec
                {
                    /// <summary>
                    /// SDBM Hash of the String (Hash with an initial value of 0x1505)
                    /// </summary>
                    public uint Hash { get; set; }

                    /// <summary>
                    /// Null Padding
                    /// </summary>
                    private int Padding { get; set; }

                    /// <summary>
                    /// Pointer to the string
                    /// </summary>
                    public long FileNamePointer { get; set; }
                }

                /// <summary>
                /// Alias Entry Context
                /// </summary>
                public struct Context
                {
                    /// <summary>
                    /// Content Type Hash
                    /// </summary>
                    public uint Type { get; set; }

                    /// <summary>
                    /// Context Value Hash
                    /// </summary>
                    public uint Value { get; set; }
                }

                /// <summary>
                /// Pointer to the name of the Sound Alias
                /// </summary>
                public SoundAliasName NamePointer { get; set; }

                /// <summary>
                /// Pointer to the subtitle of the Sound Alias
                /// </summary>
                public long SubtitlePointer { get; set; }

                /// <summary>
                /// Pointer to the name of the Secondary Sound Alias
                /// </summary>
                public SoundAliasName SecondaryPointer { get; set; }

                /// <summary>
                /// Pointer to the name of the Stop Alias 
                /// </summary>
                public SoundAliasName StopAlias { get; set; }

                /// <summary>
                /// Sound Alias File Spec
                /// </summary>
                public SoundAliasFileSpec FileSpec { get; set; }

                /// <summary>
                /// Sound Alias File Spec Sustain
                /// </summary>
                public SoundAliasFileSpec FileSpecSustain { get; set; }

                /// <summary>
                /// Sound Alias File Spec Release
                /// </summary>
                public SoundAliasFileSpec FileSpecRelease { get; set; }

                /// <summary>
                /// Alias Flags and Indexed Properties
                /// </summary>
                public long Settings { get; set; }

                /// <summary>
                /// Duck Property Hash
                /// </summary>
                public uint Duck { get; set; }

                /// <summary>
                /// Contexts
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
                public Context[] Contexts;

                /// <summary>
                /// Unknown (Always null so possibly unimplemented values)
                /// </summary>
                public int UnknownNull { get; set; }

                /// <summary>
                /// Futz Patch Property Hash
                /// </summary>
                public uint FutzPatch { get; set; }

                /// <summary>
                /// Reverb Send Value
                /// </summary>
                public int ReverbSend { get; set; }

                /// <summary>
                /// Reverb Center Value
                /// </summary>
                public int ReverbCenter { get; set; }

                /// <summary>
                /// Maximum Volume Value
                /// </summary>
                public int VolMin { get; set; }

                /// <summary>
                /// Minimum Volume Value
                /// </summary>
                public int VolMax { get; set; }

                /// <summary>
                /// Envelope Percent Value
                /// </summary>
                public int EnvelopPercent { get; set; }

                /// <summary>
                /// Flux Time
                /// </summary>
                public short FluxTime { get; set; }

                /// <summary>
                /// Start Delay
                /// </summary>
                public short StartDelay { get; set; }

                /// <summary>
                /// Minimum Pitch
                /// </summary>
                public ushort PitchMin { get; set; }

                /// <summary>
                /// Maximum Pitch
                /// </summary>
                public ushort PitchMax { get; set; }

                /// <summary>
                /// Maximum Dry Distance
                /// </summary>
                public short DistMin { get; set; }

                /// <summary>
                /// Maximum Dry Distance
                /// </summary>
                public short DistMaxDry { get; set; }

                /// <summary>
                /// Maximum Wet Distance
                /// </summary>
                public short DistMaxWet { get; set; }

                /// <summary>
                /// Minimum Envelop
                /// </summary>
                public short EnvelopMin { get; set; }

                /// <summary>
                /// Maximum Envelop
                /// </summary>
                public short EnvelopMax { get; set; }

                /// <summary>
                /// Minimum Low Pass Filtering Distance
                /// </summary>
                public short DistanceLpfMin { get; set; }

                /// <summary>
                /// Maximum Low Pass Filtering Distance
                /// </summary>
                public short DistanceLpfMax { get; set; }

                /// <summary>
                /// Fade In Time
                /// </summary>
                public short FadeIn { get; set; }

                /// <summary>
                /// Fade Out Time
                /// </summary>
                public short FadeOut { get; set; }

                /// <summary>
                /// Seems to always be 1
                /// </summary>
                public short Unk2 { get; set; }

                /// <summary>
                /// Min Threshold
                /// </summary>
                public byte PriorityThresholdMin { get; set; }

                /// <summary>
                /// Max Threshold
                /// </summary>
                public byte PriorityThresholdMax { get; set; }

                /// <summary>
                /// Alias Probability
                /// </summary>
                public byte Probability { get; set; }

                /// <summary>
                /// Occlusion Level
                /// </summary>
                public byte OcclusionLevel { get; set; }

                /// <summary>
                /// Minimum Priority
                /// </summary>
                public byte PriorityMin { get; set; }

                /// <summary>
                /// Maximum Priority
                /// </summary>
                public byte PriorityMax { get; set; }

                /// <summary>
                /// Pan Index
                /// </summary>
                public byte Pan { get; set; }

                /// <summary>
                /// Limit Count
                /// </summary>
                public byte LimitCount { get; set; }

                /// <summary>
                /// Entity Limit Count
                /// </summary>
                public byte EntityLimitCount { get; set; }

                /// <summary>
                /// Duck Group Index
                /// </summary>
                public byte DuckGroup { get; set; }

                /// <summary>
                /// Bus Index
                /// </summary>
                public byte Bus { get; set; }

                /// <summary>
                /// Volume Group Index
                /// </summary>
                public byte VolumeGroup { get; set; }
            }

            /// <summary>
            /// Gets an Alias Name by Hash, if not found, returns the hash as a hex string
            /// </summary>
            /// <param name="hash"></param>
            /// <returns></returns>
            public static string GetAliasByHash(uint hash)
            {
                return AliasHashes.TryGetValue(hash, out var alias) ? alias : hash.ToString("x");
            }

            public static void Load(AssetPool assetPool)
            {
                for (int i = 0; i < assetPool.Size; i++)
                {
                    // Read Rawfile
                    var soundHeader = Hydra.ActiveGameReader.ReadStruct<SoundHeader>(assetPool.FirstEntry + (i * assetPool.HeaderSize));
                    // Check is it a null/empty slot
                    if (assetPool.IsNullAsset(soundHeader.SABNamePointer))
                        continue;
                    // Load the aliases (we require them for the ZBarriers and other potential assets)
                    var aliasBuffer = Hydra.ActiveGameReader.ReadBytes(soundHeader.AliasesPointer, soundHeader.AliasCount * 40);
                    // Loop through them
                    for(int j = 0; j < soundHeader.AliasCount; j++)
                    {
                        // Load Alias
                        var alias = ByteUtil.BytesToStruct<SoundAliasHeader>(aliasBuffer, j * 40);
                        // Add to dictionary
                        AliasHashes[alias.Hash] = Hydra.ActiveGameReader.ReadNullTerminatedString(alias.NamePointer);
                    }
                    // Create new asset
                    Hydra.LoadedAssets.Add(new Asset()
                    {
                        Name           = Hydra.ActiveGameReader.ReadNullTerminatedString(soundHeader.SABNamePointer) + ".csv",
                        HeaderAddress  = assetPool.FirstEntry + (i * assetPool.HeaderSize),
                        ExportFunction = Export,
                        Type           = assetPool.Name,
                        Information    = String.Format("Aliases: {0}", soundHeader.AliasCount)
                    });
                }
            }

            public static bool Export(Asset asset)
            {
                // Read Header
                var soundHeader = Hydra.ActiveGameReader.ReadStruct<SoundHeader>(asset.HeaderAddress);
                // Check name pointer, if it's changed, our asset has changed
                if (asset.Name != Hydra.ActiveGameReader.ReadNullTerminatedString(soundHeader.SABNamePointer) + ".csv")
                    return false;
                // Path Result
                string path = Path.Combine("exported_files", Hydra.ActiveGameName, "sound\\aliases\\", asset.Name);
                // New Source File
                var sourceFile = new SoundSourceObj()
                {
                    Aliases = new SoundSourceObj.Alias[soundHeader.AliasCount]
                };
                // Load aliases buffer
                var aliasBuffer = Hydra.ActiveGameReader.ReadBytes(soundHeader.AliasesPointer, soundHeader.AliasCount * 40);
                // Loop through them
                for (int j = 0; j < soundHeader.AliasCount; j++)
                {
                    // Load Alias
                    var alias = ByteUtil.BytesToStruct<SoundAliasHeader>(aliasBuffer, j * 40);
                    // Load Name for entries
                    var aliasName = Hydra.ActiveGameReader.ReadNullTerminatedString(alias.NamePointer);
                    // Load entries buffer
                    var aliasEntriesBuffer = Hydra.ActiveGameReader.ReadBytes(alias.EntiresPointer, alias.EntryCount * 0xD8);
                    // New Alias
                    sourceFile.Aliases[j] = new SoundSourceObj.Alias()
                    {
                        Entries = new SoundSourceObj.Alias.Entry[alias.EntryCount]
                    };
                    // Loop through entries
                    for (int x = 0; x < alias.EntryCount; x++)
                    {
                        // Load Entry
                        var entry = Hydra.ActiveGameReader.ReadStruct<SoundAliasEntry>(alias.EntiresPointer + x * 0xD8);
                        // Grab Alias Names
                        sourceFile.Aliases[j].Entries[x] = new SoundSourceObj.Alias.Entry()
                        {
                            // Read Alias Names
                            Name = aliasName,
                            Subtitle = Hydra.ActiveGameReader.ReadNullTerminatedString(entry.SubtitlePointer),
                            Secondary = Hydra.ActiveGameReader.ReadNullTerminatedString(entry.SecondaryPointer.StringPointer),
                            StopAlias = Hydra.ActiveGameReader.ReadNullTerminatedString(entry.StopAlias.StringPointer),

                            // Read File Specs
                            FileSpec        = Hydra.ActiveGameReader.ReadNullTerminatedString(entry.FileSpec.FileNamePointer).Split('.')[0] + ".wav",
                            FileSpecSustain = Hydra.ActiveGameReader.ReadNullTerminatedString(entry.FileSpecSustain.FileNamePointer).Split('.')[0] + ".wav",
                            FileSpecRelease = Hydra.ActiveGameReader.ReadNullTerminatedString(entry.FileSpecRelease.FileNamePointer).Split('.')[0] + ".wav",

                            // Grab Flags
                            RestartContextLoops = ByteUtil.GetBit(entry.Settings, 63)   != 0 ? "yes" : "no",
                            ContextFailsafe     = ByteUtil.GetBit(entry.Settings, 3)    != 0 ? "yes" : "no",
                            SilentInCPZ         = ByteUtil.GetBit(entry.Settings, 2)    != 0 ? "yes" : "no",
                            GPAD                = ByteUtil.GetBit(entry.Settings, 25)   != 0 ? "yes" : "no",
                            GPADOnly            = ByteUtil.GetBit(entry.Settings, 26)   != 0 ? "yes" : "no",
                            MuteVoice           = ByteUtil.GetBit(entry.Settings, 27)   != 0 ? "yes" : "no",
                            MuteMusic           = ByteUtil.GetBit(entry.Settings, 28)   != 0 ? "yes" : "no",
                            ContinuousPan       = ByteUtil.GetBit(entry.Settings, 62)   != 0 ? "yes" : "no",
                            NeverPlayTwice      = ByteUtil.GetBit(entry.Settings, 32)   != 0 ? "yes" : "no",
                            IgnoreMaxDist       = ByteUtil.GetBit(entry.Settings, 11)   != 0 ? "yes" : "no",
                            StopOnEntDeath      = ByteUtil.GetBit(entry.Settings, 8)    != 0 ? "yes" : "no",
                            IsCinematic         = ByteUtil.GetBit(entry.Settings, 59)   != 0 ? "yes" : "no",
                            IsMusic             = ByteUtil.GetBit(entry.Settings, 7)    != 0 ? "yes" : "no",
                            Timescale           = ByteUtil.GetBit(entry.Settings, 9)    != 0 ? "yes" : "no",
                            Doppler             = ByteUtil.GetBit(entry.Settings, 5)    != 0 ? "yes" : "no",
                            DistanceLpf         = ByteUtil.GetBit(entry.Settings, 4)    != 0 ? "yes" : "no",
                            AmplitudePriority   = ByteUtil.GetBit(entry.Settings, 61)   != 0 ? "yes" : "no",
                            Storage             = ByteUtil.GetBit(entry.Settings, 12)   != 0 ? "loaded" : "streamed",
                            Looping             = ByteUtil.GetBit(entry.Settings, 0)    != 0 ? "looping" : "nonlooping",
                            PanType             = ByteUtil.GetBit(entry.Settings, 1)    != 0 ? "3d" : "2d", // 2.5D doesn't appear to exist

                            // Grab Limit Settings
                            LimitType        = LimitTypes?.ElementAtOrDefault((int)(entry.Settings >> 0x12) & 3),
                            EntityLimitType  = LimitTypes?.ElementAtOrDefault((int)(entry.Settings >> 0x14) & 3),
                            LimitCount       = entry.LimitCount.ToString(),
                            EntityLimitCount = entry.EntityLimitCount.ToString(),

                            // Grab Curves
                            DryMinCurve = Curves?.ElementAtOrDefault((int)(entry.Settings >> 0x2E) & 0x1F),
                            DryMaxCurve = Curves?.ElementAtOrDefault((int)(entry.Settings >> 0x22) & 0x1F),
                            WetMinCurve = Curves?.ElementAtOrDefault((int)(entry.Settings >> 0x34) & 0x1F),
                            WetMaxCurve = Curves?.ElementAtOrDefault((int)(entry.Settings >> 0x28) & 0x1F),

                            // Grab Contexts and Duck
                            Duck          = GetHashedString(entry.Duck),
                            ContextType   = GetHashedString(entry.Contexts[0].Type),
                            ContextValue  = GetHashedString(entry.Contexts[0].Value),
                            ContextType1  = GetHashedString(entry.Contexts[1].Type),
                            ContextValue1 = GetHashedString(entry.Contexts[1].Value),
                            ContextType2  = GetHashedString(entry.Contexts[2].Type),
                            ContextValue2 = GetHashedString(entry.Contexts[2].Value),
                            ContextType3  = GetHashedString(entry.Contexts[3].Type),
                            ContextValue3 = GetHashedString(entry.Contexts[3].Value),

                            // Grab 0-100 Values
                            ReverbSend     = Calculate100Value(entry.ReverbSend).ToString(),
                            CenterSend     = Calculate100Value(entry.ReverbCenter).ToString(),
                            VolMin         = Calculate100Value(entry.VolMin).ToString(),
                            VolMax         = Calculate100Value(entry.VolMax).ToString(),
                            EnvelopPercent = Calculate100Value(entry.EnvelopPercent).ToString(),

                            // Grab Pitch
                            PitchMin = CalculatePitch(entry.PitchMin).ToString(),
                            PitchMax = CalculatePitch(entry.PitchMax).ToString(),

                            // Grab Distances and Envelops
                            DistMin        = (entry.DistMin * 2).ToString(),
                            DistMaxDry     = (entry.DistMaxDry * 2).ToString(),
                            DistMaxWet     = (entry.DistMaxWet * 2).ToString(),
                            EnvelopMin     = (entry.EnvelopMin * 2).ToString(),
                            EnvelopMax     = (entry.EnvelopMax * 2).ToString(),
                            DistanceLpfMin = entry.DistanceLpfMin.ToString(),
                            DistanceLpfMax = entry.DistanceLpfMax.ToString(),

                            // Grab Start Delay
                            StartDelay = entry.StartDelay.ToString(),

                            // Grab Fade In/Out
                            FadeIn  = entry.FadeIn.ToString(),
                            FadeOut = entry.FadeOut.ToString(),

                            // Grab Priority
                            PriorityThresholdMin = Math.Round(entry.PriorityThresholdMin / 255.0, 2).ToString(),
                            PriorityThresholdMax = Math.Round(entry.PriorityThresholdMax / 255.0, 2).ToString(),
                            PriorityMin          = entry.PriorityMin.ToString(),
                            PriorityMax          = entry.PriorityMax.ToString(),

                            // Grab Probability and Occlusion
                            Probability    = Math.Round(entry.Probability / 255.0, 2).ToString(),
                            OcclusionLevel = Math.Round(entry.OcclusionLevel / 255.0, 2).ToString(),

                            // Grab Indexed Properties
                            Pan         = Pans?.ElementAtOrDefault(entry.Pan),
                            DuckGroup   = DuckGroups?.ElementAtOrDefault(entry.DuckGroup),
                            Bus         = Buses?.ElementAtOrDefault(entry.Bus),
                            VolumeGroup = VolumeGroups?.ElementAtOrDefault(entry.VolumeGroup),

                            // Grab Flux Info
                            FluxTime    = entry.FluxTime.ToString(),
                            FluxType    = FluxTypes?.ElementAtOrDefault((int)(entry.Settings >> 0xE) & 0x1F)
                        };

                    }
                }
                // Save it
                sourceFile.Save(path);
                // Done
                return true;
            }
        }
    }
}
