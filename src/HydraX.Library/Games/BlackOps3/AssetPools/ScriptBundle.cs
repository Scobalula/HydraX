using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PhilLibX;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Script Bundle Logic
        /// </summary>
        private class ScriptBundle : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// Script Parse Tree Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct ScriptBundleAsset
            {
                #region ScriptBundleAssetProperties
                public long NamePointer { get; set; }
                public int TypeStringIndex { get; set; }
                public int TypeIndex { get; set; }
                public int PropertyCount { get; set; }
                private readonly int Padding;
                public long PropertiesPointer { get; set; }
                public int ObjectCount { get; set; }
                private readonly int Padding1;
                public long ObjectsPointer { get; set; }
                #endregion
            }
            #endregion

            #region Tables
            /// <summary>
            /// Script Bundle GDT Properties
            /// </summary>
            private static Dictionary<string, string[]> Properties = new Dictionary<string, string[]>()
            {
                { "scriptbundle", new string[]
                {
                    "AlignTarget",
                    "AlignTargetTag",
                    "AllowMultiple",
                    "DevState",
                    "DisableSceneSkipping",
                    "DontSync",
                    "DontThrottle",
                    "FemaleBundle",
                    "LinkXCamToOnePlayer",
                    "Looping",
                    "NextSceneBundle",
                    "NextSceneMode",
                    "SpectateOnJoin",
                    "cameraSwitcher",
                    "cameraSwitcherGraphicContents",
                    "configstringFileType",
                    "extraCamSwitcher1",
                    "extraCamSwitcher2",
                    "extraCamSwitcher3",
                    "extraCamSwitcher4",
                    "holdCameraLastFrame",
                    "AllowDeath",
                    "CameraTween",
                    "CameraTweenOut",
                    "CombatAlertAnim",
                    "CombatAlertAnimDeath",
                    "CombatAlertAnimDeathLoop",
                    "CombatAlertShot",
                    "DelayMovementAtEnd",
                    "DeleteWhenFinished",
                    "DieWhenFinished",
                    "DisableArrivalInReach",
                    "DisablePrimaryWeaponSwitch",
                    "DisableTransitionIn",
                    "DisableTransitionOut",
                    "Disabled",
                    "DoReach",
                    "DontClamp",
                    "DontReloadAmmo",
                    "DriveSceneLength",
                    "DynamicPaths",
                    "EndAnim",
                    "EndAnimDeath",
                    "EndAnimDeathLoop",
                    "EndAnimLoop",
                    "EndAnimLoopDeath",
                    "EndAnimLoopDeathLoop",
                    "EndBlend",
                    "EndShot",
                    "EndShotLoop",
                    "EntityLerpTime",
                    "FirstFrame",
                    "HasDeathAnims",
                    "Hide",
                    "HighAlertAnim",
                    "HighAlertAnimDeath",
                    "HighAlertAnimDeathLoop",
                    "HighAlertShot",
                    "IgnoreAliveCheck",
                    "InitAnim",
                    "InitAnimDeath",
                    "InitAnimDeathLoop",
                    "InitAnimLoop",
                    "InitAnimLoopDeath",
                    "InitAnimLoopDeathLoop",
                    "InitDelayMax",
                    "InitDelayMin",
                    "InitShot",
                    "InitShotLoop",
                    "IsSiege",
                    "KeepWhileSkipping",
                    "LerpTime",
                    "LockView",
                    "LookAtPlayer",
                    "LowAlertAnim",
                    "LowAlertAnimDeath",
                    "LowAlertAnimDeathLoop",
                    "LowAlertShot",
                    "MainAnim",
                    "MainAnimDeath",
                    "MainAnimDeathLoop",
                    "MainBlend",
                    "MainDelayMax",
                    "MainDelayMin",
                    "MainShot",
                    "Model",
                    "Name",
                    "NoSpawn",
                    "OverrideAICharacter",
                    "Player",
                    "PlayerStance",
                    "RemoveWeapon",
                    "RunSceneOnDmg0",
                    "RunSceneOnDmg1",
                    "RunSceneOnDmg2",
                    "RunSceneOnDmg3",
                    "RunSceneOnDmg4",
                    "RunSceneOnDmg5",
                    "SharedIGC",
                    "ShowHUD",
                    "ShowWeaponInFirstPerson",
                    "SpawnOnInit",
                    "SpawnOnce",
                    "TakeDamage",
                    "Type",
                    "viewClampBottom",
                    "viewClampLeft",
                    "viewClampRight",
                    "viewClampTop",
                    "vmType",
                }
                },
                { "postfxbundle", new string[]
                {
                    "configstringFileType",
                    "enterStage",
                    "exitStage",
                    "finishLoopOnExit",
                    "firstpersononly",
                    "looping",
                    "num_stages",
                    "screenCapture",
                    "type",
                    "vmType",
                }
                },
                { "gibcharacterdef", new string[]
                {
                    "type",
                    "vmType",
                }
                },
                { "gamedifficulty", new string[]
                {
                    "autoAimRegionHeight",
                    "autoAimRegionWidth",
                    "base_enemy_accuracy",
                    "behind_player_accuracyScalar",
                    "behind_player_angle",
                    "configstringFileType",
                    "difficulty_xp_multiplier",
                    "dog_health",
                    "dog_hits_before_kill",
                    "dog_presstime",
                    "enemyPainChance",
                    "four_player_coopEnemyAccuracyScalar",
                    "four_player_coopFriendlyAccuracyScalar",
                    "four_player_coopFriendlyThreatBiasScalar",
                    "four_player_coopMissTimeResetDelay",
                    "four_player_coopPlayerDifficultyHealth",
                    "four_player_deathInvulnerableTimeModifier",
                    "four_player_enemy_pain_chance_modifier",
                    "four_player_hit_invulnerability_modifier",
                    "healthOverlayCutoff",
                    "longRegenTime",
                    "missTimeConstant",
                    "missTimeDistanceFactor",
                    "missTimeResetDelay",
                    "one_player_coopEnemyAccuracyScalar",
                    "one_player_coopFriendlyAccuracyScalar",
                    "one_player_coopFriendlyThreatBiasScalar",
                    "one_player_coopMissTimeResetDelay",
                    "one_player_coopPlayerDifficultyHealth",
                    "one_player_deathInvulnerableTimeModifier",
                    "one_player_enemy_pain_chance_modifier",
                    "one_player_hit_invulnerability_modifier",
                    "playerDifficultyHealth",
                    "playerHealth_RegularRegenDelay",
                    "playerHitInvulnTime",
                    "playerReviveChances",
                    "playerReviveHealthPool",
                    "player_deathInvulnerableTime",
                    "threatbias",
                    "three_player_coopEnemyAccuracyScalar",
                    "three_player_coopFriendlyAccuracyScalar",
                    "three_player_coopFriendlyThreatBiasScalar",
                    "three_player_coopMissTimeResetDelay",
                    "three_player_coopPlayerDifficultyHealth",
                    "three_player_deathInvulnerableTimeModifier",
                    "three_player_enemy_pain_chance_modifier",
                    "three_player_hit_invulnerability_modifier",
                    "two_player_coopEnemyAccuracyScalar",
                    "two_player_coopFriendlyAccuracyScalar",
                    "two_player_coopFriendlyThreatBiasScalar",
                    "two_player_coopMissTimeResetDelay",
                    "two_player_coopPlayerDifficultyHealth",
                    "two_player_deathInvulnerableTimeModifier",
                    "two_player_enemy_pain_chance_modifier",
                    "two_player_hit_invulnerability_modifier",
                    "type",
                    "vmType",
                    "worthyDamageRatio",
                }
                },
                { "collectible", new string[]
                {
                    "configstringFileType",
                    "enum_slotSize",
                    "image_uiMaterial",
                    "image_uiMaterialLarge",
                    "type",
                    "vmType",
                    "xmodel_uiModel",
                    "xmodel_uiModelWithStand",
                    "xstring_audiolog_sound",
                    "xstring_codexurl",
                    "xstring_codexurldesc",
                    "xstring_description",
                    "xstring_displayNameLong",
                    "xstring_displayNameShort",
                }
                },
                { "botsettings", new string[]
                {
                    "aimDelay",
                    "aimErrorMaxPitch",
                    "aimErrorMaxYaw",
                    "aimErrorMinPitch",
                    "aimErrorMinYaw",
                    "aimTime",
                    "allowGrenades",
                    "allowHeroGadgets",
                    "allowKillstreaks",
                    "allowMelee",
                    "changeClassWeight",
                    "chaseThreatTime",
                    "chaseWanderFwdDot",
                    "chaseWanderMax",
                    "chaseWanderMin",
                    "chaseWanderSpacing",
                    "configstringFileType",
                    "damageWanderFwdDot",
                    "damageWanderMax",
                    "damageWanderMin",
                    "damageWanderSpacing",
                    "defaultAds",
                    "defaultFire",
                    "defaultRange",
                    "defaultRangeClose",
                    "fov",
                    "fovAds",
                    "headshotWeight",
                    "lethalDistanceMax",
                    "lethalDistanceMin",
                    "lethalWeight",
                    "meleeDot",
                    "meleeRangeMultiplier",
                    "mgAds",
                    "mgFire",
                    "mgRange",
                    "mgRangeClose",
                    "pistolAds",
                    "pistolFire",
                    "pistolRange",
                    "pistolRangeClose",
                    "pitchAccelerationTime",
                    "pitchDecelerationThreshold",
                    "pitchSensitivity",
                    "pitchSpeed",
                    "pitchSpeedAds",
                    "rifleAds",
                    "rifleFire",
                    "rifleRange",
                    "rifleRangeClose",
                    "rocketLauncherAds",
                    "rocketLauncherFire",
                    "rocketLauncherRange",
                    "rocketLauncherRangeClose",
                    "smgAds",
                    "smgFire",
                    "smgRange",
                    "smgRangeClose",
                    "sniperAds",
                    "sniperFire",
                    "sniperRange",
                    "sniperRangeClose",
                    "spreadAds",
                    "spreadFire",
                    "spreadRange",
                    "spreadRangeClose",
                    "strafeMax",
                    "strafeMin",
                    "strafeSideDotMax",
                    "strafeSideDotMin",
                    "strafeSpacing",
                    "tacticalDistanceMax",
                    "tacticalDistanceMin",
                    "tacticalWeight",
                    "thinkInterval",
                    "threatRadiusMax",
                    "threatRadiusMin",
                    "type",
                    "vmType",
                    "wanderFwdDot",
                    "wanderMax",
                    "wanderMin",
                    "wanderSpacing",
                    "yawAccelerationTime",
                    "yawDecelerationThreshold",
                    "yawSensitivity",
                    "yawSpeed",
                    "yawSpeedAds",
                }
                },
                { "accolade", new string[]
                {
                    "image_rewardImage",
                    "vmType",
                    "xstring_centerText",
                    "xstring_challengeDetails",
                    "xstring_challengeName",
                    "xstring_challengeReference",
                    "xstring_challengeWidget",
                    "xstring_rewardName",
                }
                }
            };
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
            public string Name => "scriptbundle";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Script Bundle";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.scriptbundle;

            /// <summary>
            /// Loads Assets from this Asset Pool
            /// </summary>
            public List<GameAsset> Load(HydraInstance instance)
            {
                var results = new List<GameAsset>();

                var poolInfo = instance.Reader.ReadStruct<AssetPoolInfo>(instance.Game.BaseAddress + instance.Game.AssetPoolsAddresses[instance.Game.ProcessIndex] + (Index * 0x20));

                StartAddress = poolInfo.PoolPointer;
                AssetSize = poolInfo.AssetSize;
                AssetCount = poolInfo.PoolSize;

                for(int i = 0; i < AssetCount; i++)
                {
                    var header = instance.Reader.ReadStruct<ScriptBundleAsset>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    results.Add(new GameAsset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                        HeaderAddress = StartAddress + (i * AssetSize),
                        AssetPool = this,
                        Type = Name,
                        Information = "N/A"
                    });
                }

                return results;
            }

            /// <summary>
            /// Cleans the KVP (fucked up naming conventions and 2 tools that are case and not case sensitive)
            /// </summary>
            private Tuple<string, string> CleanKVP(string key, string value, string type)
            {
                var resultKey = key;
                var resultVal = value.Replace("scriptvector", "scriptVector");

                if(Properties.TryGetValue(type, out var props))
                {
                    foreach (var propertyName in props)
                    {
                        var a = propertyName.ToLower();
                        var b = resultKey.ToLower();

                        if (a == b)
                        {
                            resultKey = propertyName;
                            break;
                        }
                        else if(a.Replace("xstring_", "") == b)
                        {
                            resultKey = propertyName;
                            break;
                        }
                    }
                }

                switch(resultKey.ToLower())
                {
                    case "runsceneondmg0":
                    case "runsceneondmg1":
                    case "runsceneondmg2":
                    case "runsceneondmg3":
                    case "runsceneondmg4":
                    case "runsceneondmg5":
                    case "devstate":
                    case "vmtype":
                    case "playerstance":
                    case "type":
                        resultVal = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(resultVal);
                        break;
                    default:
                        break;
                }

                return new Tuple<string, string>(resultKey, resultVal);
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public HydraStatus Export(GameAsset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<ScriptBundleAsset>(asset.HeaderAddress);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    return HydraStatus.MemoryChanged;

                var gdtAsset = new GameDataTable.Asset(asset.Name, instance.Game.GetString(header.TypeStringIndex, instance));

                if (string.IsNullOrWhiteSpace(gdtAsset.Type))
                    gdtAsset.Type = "accolade";
                else if (gdtAsset.Type == "scene")
                    gdtAsset.Type = "scriptbundle";
 
                for (int i = 0; i < header.PropertyCount; i++)
                {
                    var property = instance.Reader.ReadStruct<KVPBlock>(header.PropertiesPointer + (i * 24));

                    var key = instance.Game.GetString(property.DataNameStringIndex, instance);
                    var val = instance.Game.GetString(property.DataStringIndex, instance);

                    var result = CleanKVP(key, val, gdtAsset.Type);

                    gdtAsset[result.Item1] = result.Item2;
                }

                for (int i = 0; i < header.ObjectCount; i++)
                {
                    var propertyCount = instance.Reader.ReadInt64((header.ObjectsPointer + (i * 16)));
                    var propertiesPointer = instance.Reader.ReadInt64((header.ObjectsPointer + (i * 16)) + 8);

                    for (int x = 0; x < propertyCount; x++)
                    {
                        var property = instance.Reader.ReadStruct<KVPBlock>(propertiesPointer + (x * 24));

                        var key = instance.Game.GetString(property.DataNameStringIndex, instance);
                        var val = instance.Game.GetString(property.DataStringIndex, instance);

                        var result = CleanKVP(key, val, gdtAsset.Type);

                        gdtAsset[string.Format("object{0}_{1}", i + 1, result.Item1)] = result.Item2;
                    }
                }

                gdtAsset.Name = asset.Name;

                instance.GDTs["Misc"][asset.Name] = gdtAsset;

                return HydraStatus.Success;
            }

            /// <summary>
            /// Checks if the given asset is a null slot
            /// </summary>
            public bool IsNullAsset(GameAsset asset)
            {
                return IsNullAsset(asset.NameLocation);
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
