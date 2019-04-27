using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhilLibX.IO;

namespace HydraX.Library
{
    public partial class BlackOps3 : IGame
    {
        #region Enums
        /// <summary>
        /// Data Block Data Types
        /// </summary>
        public enum DataTypes : int
        {
            Image            = 0xF,
            Anim             = 0x3,
            PlayerAnim       = 0x4,
            XCam             = 0xD,
            XModel           = 0x6,
            XString          = 0x0,
            FX               = 0x9,
            Int              = 0x1,
            Float            = 0x2,
            Bool,
        }
        #endregion

        #region Structures
        /// <summary>
        /// Asset Pool Data
        /// </summary>
        public struct AssetPoolInfo
        {
            #region AssetPoolInfoProperties
            public long PoolPointer { get; set; }
            public int AssetSize { get; set; }
            public int PoolSize { get; set; }
            public int Padding { get; set; }
            public int AssetCount { get; set; }
            public long FreeSlot { get; set; }
            #endregion
        }

        public struct KVPBlock
        {
            public int DataNameStringIndex { get; set; }
            public int DataStringIndex { get; set; }
            public DataTypes DataType { get; set; }
            public int Data { get; set; }
            public long DataPointer { get; set; }
        }
        #endregion

        /// <summary>
        /// Gets Black Ops 3's Game Name
        /// </summary>
        public string Name => "Black Ops III";

        /// <summary>
        /// Gets Black Ops 3's Process Names
        /// </summary>
        public string[] ProcessNames => new string[]
        {
            "BlackOps3"
        };

        /// <summary>
        /// Gets Black Ops 3 Asset Pools Addresses
        /// </summary>
        public long[] AssetPoolsAddresses => new long[]
        {
            0x93FA290
        };

        /// <summary>
        /// Gets Black Ops 3 Asset Sizes Addresses (Not Implemented for Black Ops 3, stored in pool info)
        /// </summary>
        public long[] AssetSizesAddresses => throw new NotImplementedException();

        /// <summary>
        /// Gets Black Ops 3 String Table Addresses
        /// </summary>
        public long[] StringTableAddresses => new long[]
        {
            0x4D4F104
        };

        /// <summary>
        /// Gets or Sets Black Ops 3's Base Address (ASLR)
        /// </summary>
        public long BaseAddress { get; set; }

        /// <summary>
        /// Gets or Sets the current Process Index
        /// </summary>
        public int ProcessIndex { get; set; }

        /// <summary>
        /// Gets or sets the list of Asset Pools
        /// </summary>
        public List<IAssetPool> AssetPools { get; set; }

        /// <summary>
        /// Black Ops III Asset Pool Indices
        /// </summary>
        private enum AssetPool : int
        {
            physpreset,
            physconstraints,
            destructibledef,
            xanim,
            xmodel,
            xmodelmesh,
            material,
            computeshaderset,
            techset,
            image,
            sound,
            sound_patch,
            col_map,
            com_map,
            game_map,
            map_ents,
            gfx_map,
            lightdef,
            lensflaredef,
            ui_map,
            font,
            fonticon,
            localize,
            weapon,
            weapondef,
            weaponvariant,
            weaponfull,
            cgmediatable,
            playersoundstable,
            playerfxtable,
            sharedweaponsounds,
            attachment,
            attachmentunique,
            weaponcamo,
            customizationtable,
            customizationtable_feimages,
            customizationtablecolor,
            snddriverglobals,
            fx,
            tagfx,
            klf,
            impactsfxtable,
            impactsoundstable,
            player_character,
            aitype,
            character,
            xmodelalias,
            rawfile,
            stringtable,
            structuredtable,
            leaderboarddef,
            ddl,
            glasses,
            texturelist,
            scriptparsetree,
            keyvaluepairs,
            vehicle,
            addon_map_ents,
            tracer,
            slug,
            surfacefxtable,
            surfacesounddef,
            footsteptable,
            entityfximpacts,
            entitysoundimpacts,
            zbarrier,
            vehiclefxdef,
            vehiclesounddef,
            typeinfo,
            scriptbundle,
            scriptbundlelist,
            rumble,
            bulletpenetration,
            locdmgtable,
            aimtable,
            animselectortable,
            animmappingtable,
            animstatemachine,
            behaviortree,
            behaviorstatemachine,
            ttf,
            sanim,
            lightdescription,
            shellshock,
            xcam,
            bgcache,
            texturecombo,
            flametable,
            bitfield,
            attachmentcosmeticvariant,
            maptable,
            maptableloadingimages,
            medal,
            medaltable,
            objective,
            objectivelist,
            umbra_tome,
            navmesh,
            navvolume,
            binaryhtml,
            laser,
            beam,
            streamerhint,
            _string,
            assetlist,
            report,
            depend,
        }

        /// <summary>
        /// Black Ops 3 Specialty Names
        /// </summary>
        private static readonly string[] SpecialtyNames =
        {
            "specialty_additionalprimaryweapon",
            "specialty_accuracyandflatspread",
            "specialty_ammodrainsfromstockfirst",
            "specialty_anteup",
            "specialty_armorpiercing",
            "specialty_armorvest",
            "specialty_bulletaccuracy",
            "specialty_bulletdamage",
            "specialty_bulletflinch",
            "specialty_bulletpenetration",
            "specialty_combat_efficiency",
            "specialty_deadshot",
            "specialty_decoy",
            "specialty_delayexplosive",
            "specialty_detectexplosive",
            "specialty_detectnearbyenemies",
            "specialty_directionalfire",
            "specialty_disarmexplosive",
            "specialty_doubletap2",
            "specialty_earnmoremomentum",
            "specialty_electriccherry",
            "specialty_extraammo",
            "specialty_fallheight",
            "specialty_fastads",
            "specialty_fastequipmentuse",
            "specialty_fastladderclimb",
            "specialty_fastmantle",
            "specialty_fastmeleerecovery",
            "specialty_fastreload",
            "specialty_fasttoss",
            "specialty_fastweaponswitch",
            "specialty_finalstand",
            "specialty_fireproof",
            "specialty_flakjacket",
            "specialty_flashprotection",
            "specialty_gpsjammer",
            "specialty_grenadepulldeath",
            "specialty_healthregen",
            "specialty_holdbreath",
            "specialty_immunecounteruav",
            "specialty_immuneemp",
            "specialty_immunemms",
            "specialty_immunenvthermal",
            "specialty_immunerangefinder",
            "specialty_immunesmoke",
            "specialty_immunetriggerbetty",
            "specialty_immunetriggerc4",
            "specialty_immunetriggershock",
            "specialty_jetcharger",
            "specialty_jetnoradar",
            "specialty_jetpack",
            "specialty_jetquiet",
            "specialty_killstreak",
            "specialty_longersprint",
            "specialty_loudenemies",
            "specialty_lowgravity",
            "specialty_marksman",
            "specialty_microwaveprotection",
            "specialty_movefaster",
            "specialty_nokillstreakreticle",
            "specialty_nomotionsensor",
            "specialty_noname",
            "specialty_nottargetedbyairsupport",
            "specialty_nottargetedbyaitank",
            "specialty_nottargetedbyraps",
            "specialty_nottargetedbyrobot",
            "specialty_nottargetedbysentry",
            "specialty_overcharge",
            "specialty_phdflopper",
            "specialty_pin_back",
            "specialty_pistoldeath",
            "specialty_playeriszombie",
            "specialty_proximityprotection",
            "specialty_quickrevive",
            "specialty_quieter",
            "specialty_rof",
            "specialty_scavenger",
            "specialty_sengrenjammer",
            "specialty_shellshock",
            "specialty_showenemyequipment",
            "specialty_showenemyvehicles",
            "specialty_showscorestreakicons",
            "specialty_sixthsensejammer",
            "specialty_spawnpingenemies",
            "specialty_sprintequipment",
            "specialty_sprintfire",
            "specialty_sprintgrenadelethal",
            "specialty_sprintgrenadetactical",
            "specialty_sprintrecovery",
            "specialty_sprintfirerecovery",
            "specialty_stalker",
            "specialty_staminup",
            "specialty_stunprotection",
            "specialty_teflon",
            "specialty_tombstone",
            "specialty_tracer",
            "specialty_tracker",
            "specialty_trackerjammer",
            "specialty_twogrenades",
            "specialty_twoprimaries",
            "specialty_unlimitedsprint",
            "specialty_vultureaid",
            "specialty_whoswho",
            "specialty_widowswine",
            "specialty_locdamagecountsasheadshot",
        };

        /// <summary>
        /// Struct Sizes to check against
        /// </summary>
        private static readonly int[] StructSizes =
        {
            120,
            1680,
            48,
            248
        };

        public string GetString(long index, HydraInstance instance)
        {
            return instance.Reader.ReadNullTerminatedString(instance.Game.BaseAddress + instance.Game.StringTableAddresses[instance.Game.ProcessIndex] + (0x1C * index));
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public bool ValidateAddresses(HydraInstance instance)
        {
            BaseAddress = instance.Reader.GetBaseAddress();
            var pools = instance.Reader.ReadArray<AssetPoolInfo>(BaseAddress + instance.Game.AssetPoolsAddresses[instance.Game.ProcessIndex], 4);
            bool result = true;

            for (int i = 0; i < pools.Length; i++)
                if (StructSizes[i] != pools[i].AssetSize)
                    result = false;

            return result;
        }

        public string GetAssetName(long address, HydraInstance instance, long offset = 0)
        {
            return address == 0 ? "" : instance.Reader.ReadNullTerminatedString(instance.Reader.ReadInt64(address + offset));
        }

        public string CleanAssetName(HydraAssetType type, string assetName)
        {
            if(!string.IsNullOrWhiteSpace(assetName))
            {
                switch (type)
                {
                    case HydraAssetType.FX:
                        return Path.ChangeExtension(assetName.Replace(@"/", @"\").Replace(@"\", @"\\"), ".efx");
                    case HydraAssetType.Sound:
                        return Path.ChangeExtension(assetName.Split('.')[0], ".wav");
                    default:
                        return assetName;
                }
            }

            return "";
        }
    }
}
