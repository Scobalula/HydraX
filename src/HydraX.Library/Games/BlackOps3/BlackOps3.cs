using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;

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
        /// Alias Hashes and Names
        /// </summary>
        public static Dictionary<uint, string> AliasHashes = new Dictionary<uint, string>();

        /// <summary>
        /// Gets Black Ops 3's Game Name
        /// </summary>
        public string Name => "Black Ops III";

        /// <summary>
        /// Gets Black Ops 3's Process Names
        /// </summary>
        public string[] ProcessNames => new string[]
        {
            "blackops3"
        };

        /// <summary>
        /// Gets Black Ops 3 Asset Pools Addresses
        /// </summary>
        public long AssetPoolsAddress { get; set; }

        /// <summary>
        /// Gets Black Ops 3 Asset Sizes Addresses (Not Implemented for Black Ops 3, stored in pool info)
        /// </summary>
        public long AssetSizesAddress { get; set; }

        /// <summary>
        /// Gets Black Ops 3 String Pool
        /// </summary>
        public long StringPoolAddress { get; set; }

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
        /// Gets or Sets the Zone Names for each asset
        /// </summary>
        public Dictionary<long, string> ZoneNames { get; set; }

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
        /// Gets an Alias Name by Hash, if not found, returns the hash as a hex string
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static string GetAliasByHash(uint hash)
        {
            if (hash == 0)
                return "";

            return AliasHashes.TryGetValue(hash, out var alias) ? alias : "";
        }

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
            return instance.Reader.ReadNullTerminatedString(instance.Game.StringPoolAddress + (0x1C * index) + 4);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x20)]
        struct XAssetEntryPoolEntry
        {
            public AssetPool AssetType;
            public long HeaderPointer;
            public byte ZoneIndex;
        }

        public bool Initialize(HydraInstance instance)
        {
            var module = instance.Reader.Modules[0];
            bool isWindowsStore = instance.Reader.ReadInt16(module.BaseAddress.ToInt64() + 0x3C) == 0x1A0; // NOTE(serious): only works for a specific windows store executable (initial release -> current as of Jan 2025)
            
            long zoneEntriesAddress;
            XAssetEntryPoolEntry[] poolEntries;
            if (!isWindowsStore)
            {
                long[] pools, strPool, poolEntrys;
                pools = instance.Reader.FindBytes(
                new byte?[] { 0x63, 0xC1, 0x48, 0x8D, 0x05, null, null, null, null, 0x49, 0xC1, 0xE0, null, 0x4C, 0x03, 0xC0 },
                module.BaseAddress.ToInt64(),
                module.BaseAddress.ToInt64() + module.Size,
                true);

                strPool = instance.Reader.FindBytes(
                    new byte?[] { 0x4C, 0x03, 0xF6, 0x33, 0xDB, 0x49, null, null, 0x8B, 0xD3, 0x8D, 0x7B },
                    module.BaseAddress.ToInt64(),
                    module.BaseAddress.ToInt64() + module.Size,
                    true);

                poolEntrys = instance.Reader.FindBytes(
                    new byte?[] { 0x48, 0x8D, 0x05, null, null, null, null, 0x41, 0x8B, 0x34, 0x24, 0x85, 0xF6, 0x0F, 0x84, 0xF0, 0x00, 0x00, 0x00, 0x4C, 0x8D },
                    module.BaseAddress.ToInt64(),
                    module.BaseAddress.ToInt64() + module.Size,
                    true);

                AssetPoolsAddress = instance.Reader.ReadInt32(pools[0] + 5) + pools[0] + 9;
                StringPoolAddress = instance.Reader.ReadInt32(strPool[0] + 29) + strPool[0] + 33;

                zoneEntriesAddress = instance.Reader.ReadInt32(poolEntrys[0] + 22) + poolEntrys[0] + 26;

                // Max of 156672 as per listassetpool
                poolEntries = instance.Reader.ReadArrayUnsafe<XAssetEntryPoolEntry>(instance.Reader.ReadInt32(poolEntrys[0] + 3) + poolEntrys[0] + 7, 156672);
            }
            else
            {
                // NOTE(serious): has hardcoded addresses for specific windows store executable -- pattern scanning is still possible but many functions get inlined/messed with, and I don't expect the windows store version to be updated with anything substantial.
                //                just in case, I provided signatures to find the same code sections, but the rel offsets will be different for calculating the final addresses, so if this becomes an issue those will have to be messed with.
                
                // 48 8B CD 48 8B 6C 24 ? 48 C1 E1 05 42 80 BC 21 ? ? ? ? ? 75 1B 4A 8B 84 21 ? ? ? ? 48 89 03 42 FF 8C 21 ? ? ? ? 4A 89 9C 21 ? ? ? ?
                AssetPoolsAddress = 0xF3B0C70L + module.BaseAddress.ToInt64();

                // 33 FF 8B D7 48 8B 08 48 8D 44 24 ? 8D 5F 01 49 89 04 0E 48 8D 05 ? ? ? ? 48 89 05 ? ? ? ? 48 89 05 ? ? ? ? 48 8D 05 ? ? ? ? 48 89 05 ? ? ? ?
                StringPoolAddress = 0x3B1F780L + module.BaseAddress.ToInt64();

                // 48 8D 05 ? ? ? ? 66 66 0F 1F 84 00 ? ? ? ? 41 8B 34 24 85 F6 0F 84 ? ? ? ? 4C 8D 2D ? ? ? ? 4C 8D 25 ? ? ? ? 66 0F 1F 44 00 ?
                zoneEntriesAddress = 0xF882300L + module.BaseAddress.ToInt64();

                // Max of 156672 as per listassetpool
                poolEntries = instance.Reader.ReadArrayUnsafe<XAssetEntryPoolEntry>(0xF3BA2F0L + module.BaseAddress.ToInt64(), 156672);
            }

            // Store zone names by asset pointer
            var zones = new string[65];
            zones[0] = "default_zone";
            for (int i = 1; i < zones.Length; i++)
            {
                zones[i] = instance.Reader.ReadNullTerminatedString(zoneEntriesAddress + 96 * i);

                if (string.IsNullOrWhiteSpace(zones[i]))
                    zones[i] = "unknown";
            }

            ZoneNames = new Dictionary<long, string>();

            foreach (var poolEntry in poolEntries)
            {
                if (poolEntry.HeaderPointer != 0) // Validate if this is not an empty slot (same as pools, points to next, but we can check Header Pointer)
                    ZoneNames[poolEntry.HeaderPointer] = zones[poolEntry.ZoneIndex];
            }

            return true;
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

        /// <summary>
        /// Converts an asset buffer to GDT Asset, the buffer passed will be the exact same as the buffer Linker uses to load the GDT asset
        /// </summary>
        public static GameDataTable.Asset ConvertAssetBufferToGDTAsset(byte[] assetBuffer, Tuple<string, int, int>[] properties, HydraInstance instance, Func<GameDataTable.Asset, byte[], int, int, HydraInstance, object> extendedDataHandler = null)
        {
            var asset = new GameDataTable.Asset();

            foreach (var property in properties)
            {
                switch (property.Item3)
                {
                    // Strings (Enum that points to a string)
                    case 0:
                        {
                            asset[property.Item1] = instance.Reader.ReadNullTerminatedString(BitConverter.ToInt64(assetBuffer, property.Item2));
                            break;
                        }
                    // Inline Character Array
                    case 1:
                        {
                            asset[property.Item1] = Encoding.ASCII.GetString(assetBuffer, property.Item2, 1024).TrimEnd('\0');
                            break;
                        }
                    // Inline Character Array
                    case 2:
                        {
                            asset[property.Item1] = Encoding.ASCII.GetString(assetBuffer, property.Item2, 64).TrimEnd('\0');
                            break;
                        }
                    // Inline Character Array
                    case 3:
                        {
                            asset[property.Item1] = Encoding.ASCII.GetString(assetBuffer, property.Item2, 256).TrimEnd('\0');
                            break;
                        }
                    // 32Bit Ints
                    case 4:
                        {
                            asset[property.Item1] = BitConverter.ToInt32(assetBuffer, property.Item2);
                            break;
                        }
                    // Unsigned Ints
                    case 5:
                        {
                            asset[property.Item1] = BitConverter.ToUInt32(assetBuffer, property.Item2);
                            break;
                        }
                    // Bools
                    case 6:
                        {
                            asset[property.Item1] = assetBuffer[property.Item2];
                            break;
                        }
                    // QBools
                    case 7:
                        {
                            asset[property.Item1] = BitConverter.ToInt32(assetBuffer, property.Item2);
                            break;
                        }
                    // Floats
                    case 8:
                        {
                            asset[property.Item1] = BitConverter.ToSingle(assetBuffer, property.Item2);
                            break;
                        }
                    // Milliseconds
                    case 9:
                        {
                            asset[property.Item1] = BitConverter.ToInt32(assetBuffer, property.Item2) / 1000.0;
                            break;
                        }
                    // Script Strings
                    case 0x15:
                        {
                            asset[property.Item1] = instance.Game.GetString(BitConverter.ToInt32(assetBuffer, property.Item2), instance);
                            break;
                        }
                    case 0xA:
                        {
                            var assetName = instance.Game.GetAssetName(BitConverter.ToInt64(assetBuffer, property.Item2), instance, 0);
                            asset[property.Item1] = string.IsNullOrWhiteSpace(assetName) ? "" : @"fx\\" + Path.ChangeExtension(assetName.Replace(@"/", @"\").Replace(@"\", @"\\"), ".efx");
                            break;
                        }
                    // Images
                    case 0x10:
                    case 0x11:
                        {
                            asset[property.Item1] = instance.Game.GetAssetName(BitConverter.ToInt64(assetBuffer, property.Item2), instance, 0xF8);
                            break;
                        }
                    // Alias Hash
                    case 0x18:
                        {
                            asset[property.Item1] = GetAliasByHash(BitConverter.ToUInt32(assetBuffer, property.Item2));
                            break;
                        }
                    // Flametable Assets
                    case 0x2B:
                        {
                            asset[property.Item1] = instance.Game.GetAssetName(BitConverter.ToInt64(assetBuffer, property.Item2), instance, 0x1B0);
                            break;
                        }
                    // Standard Assets
                    case 0xB:
                    case 0xD:
                    case 0xF:
                    case 0x12:
                    case 0x13:
                    case 0x14:
                    case 0x16:
                    case 0x17:
                    case 0x19:
                    case 0x1A:
                    case 0x1B:
                    case 0x1C:
                    case 0x1D:
                    case 0x1E:
                    case 0x1F:
                    case 0x20:
                    case 0x21:
                    case 0x22:
                    case 0x23:
                    case 0x24:
                    case 0x25:
                    case 0x26:
                    case 0x27:
                    case 0x28:
                    case 0x29:
                    case 0x2A:
                    case 0x2C:
                    case 0x2D:
                    case 0x2E:
                    case 0x2F:
                    case 0x30:
                    case 0x31:
                    case 0x32:
                        {
                            var assetName = instance.Game.GetAssetName(BitConverter.ToInt64(assetBuffer, property.Item2), instance, property.Item3 == 0x10 ? 0xF8 : 0);
                            asset[property.Item1] = assetName;
                            break;
                        }
                    default:
                        {
                            // Attempt to use the extended data handler, otherwise null
                            var result = extendedDataHandler?.Invoke(asset, assetBuffer, property.Item2, property.Item3, instance);

                            if (result != null)
                            {
                                asset[property.Item1] = result;
                            }
                            else
                            {
                                asset[property.Item1] = "";
#if DEBUG
                                Console.WriteLine("Unknown Value: {0} - {1} - {2}", property.Item3, property.Item2, property.Item1);
#endif
                            }
                            // Done
                            break;
                        }
                }

                //assetBuffer[property.Item2] = 0xAF;
            }

            // Done
            return asset;
        }
    }
}
