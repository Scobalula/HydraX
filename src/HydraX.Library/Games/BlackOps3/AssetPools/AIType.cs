using System;
using System.Collections.Generic;
using System.IO;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 AIType Logic
        /// </summary>
        private class AIType : IAssetPool
        {
            #region Tables
            /// <summary>
            /// AIType Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] AITypeOffsets =
            {
                new Tuple<string, int, int>("animTree",                                                      0x8, 0x0),
                new Tuple<string, int, int>("csvInclude",                                                    0x10, 0x0),
                new Tuple<string, int, int>("accuracy",                                                      0x18, 0x8),
                new Tuple<string, int, int>("health",                                                        0x1c, 0x4),
                new Tuple<string, int, int>("primaryweapon1",                                                0x20, 0x15),
                new Tuple<string, int, int>("primaryweapon2",                                                0x24, 0x15),
                new Tuple<string, int, int>("primaryweapon3",                                                0x28, 0x15),
                new Tuple<string, int, int>("primaryweapon4",                                                0x2c, 0x15),
                new Tuple<string, int, int>("primaryWeaponType",                                             0x34, 0x15),
                new Tuple<string, int, int>("primaryAttach1",                                                0x38, 0x8),
                new Tuple<string, int, int>("primaryAttach2",                                                0x3c, 0x8),
                new Tuple<string, int, int>("primaryAttach3",                                                0x40, 0x8),
                new Tuple<string, int, int>("primaryAttach4",                                                0x44, 0x8),
                new Tuple<string, int, int>("primaryAttach5",                                                0x48, 0x8),
                new Tuple<string, int, int>("primaryAttach6",                                                0x4c, 0x8),
                new Tuple<string, int, int>("primaryBannedAttachments",                                      0x58, 0x38),
                new Tuple<string, int, int>("primaryAllowedCamos",                                           0x158, 0x39),
                new Tuple<string, int, int>("secondaryweapon",                                               0x2e8, 0x15),
                new Tuple<string, int, int>("sidearm",                                                       0x2ec, 0x15),
                new Tuple<string, int, int>("meleeweapon",                                                   0x2f0, 0x15),
                new Tuple<string, int, int>("grenadeWeapon",                                                 0x2f4, 0x15),
                new Tuple<string, int, int>("grenadeAmmo",                                                   0x2f8, 0x4),
                new Tuple<string, int, int>("ammoPouch",                                                     0x2fc, 0x15),
                new Tuple<string, int, int>("fireMode",                                                      0x300, 0x37),
                new Tuple<string, int, int>("burstCountMin",                                                 0x304, 0x4),
                new Tuple<string, int, int>("burstCountMax",                                                 0x308, 0x4),
                new Tuple<string, int, int>("fireIntervalmin",                                               0x30c, 0x9),
                new Tuple<string, int, int>("fireIntervalmax",                                               0x310, 0x9),
                new Tuple<string, int, int>("team",                                                          0x314, 0x33),
                new Tuple<string, int, int>("type",                                                          0x318, 0x35),
                new Tuple<string, int, int>("archetype",                                                     0x31c, 0x15),
                new Tuple<string, int, int>("scoretype",                                                     0x320, 0x15),
                new Tuple<string, int, int>("category",                                                      0x324, 0x4),
                new Tuple<string, int, int>("shootAtTag",                                                    0x328, 0x15),
                new Tuple<string, int, int>("aimAtTag",                                                      0x32c, 0x15),
                new Tuple<string, int, int>("physRadius",                                                    0x330, 0x8),
                new Tuple<string, int, int>("physHeight",                                                    0x334, 0x8),
                new Tuple<string, int, int>("fixedNode",                                                     0x338, 0x6),
                new Tuple<string, int, int>("pacifist",                                                      0x339, 0x6),
                new Tuple<string, int, int>("pathEnemyFightDist",                                            0x33c, 0x8),
                new Tuple<string, int, int>("runAndGunDist",                                                 0x340, 0x8),
                new Tuple<string, int, int>("faceenemyfightdist",                                            0x344, 0x8),
                new Tuple<string, int, int>("coversearchinterval",                                           0x348, 0x4),
                new Tuple<string, int, int>("newthreatdelay",                                                0x34c, 0x4),
                new Tuple<string, int, int>("newpaindelay",                                                  0x350, 0x4),
                new Tuple<string, int, int>("defaultGoalRadius",                                             0x354, 0x8),
                new Tuple<string, int, int>("sightDistance",                                                 0x358, 0x8),
                new Tuple<string, int, int>("painTolerance",                                                 0x35c, 0x4),
                new Tuple<string, int, int>("footstepTable",                                                 0x360, 0x1c),
                new Tuple<string, int, int>("footstepScriptCallback",                                        0x368, 0x7),
                new Tuple<string, int, int>("surfaceFXTable",                                                0x370, 0x1d),
                new Tuple<string, int, int>("engageMinDist",                                                 0x378, 0x8),
                new Tuple<string, int, int>("engageMinFalloffDist",                                          0x37c, 0x8),
                new Tuple<string, int, int>("engageMaxDist",                                                 0x380, 0x8),
                new Tuple<string, int, int>("engageMaxFalloffDist",                                          0x384, 0x8),
                new Tuple<string, int, int>("frontQuadrantDegrees",                                          0x388, 0x8),
                new Tuple<string, int, int>("backQuadrantDegrees",                                           0x38c, 0x8),
                new Tuple<string, int, int>("leftQuadrantDegrees",                                           0x390, 0x8),
                new Tuple<string, int, int>("rightQuadrantDegrees",                                          0x394, 0x8),
                new Tuple<string, int, int>("movementtype",                                                  0x3c0, 0x15),
                new Tuple<string, int, int>("favorTraversalFactor",                                          0x3c4, 0x8),
                new Tuple<string, int, int>("favorSpecialTraversalFactor",                                   0x3c8, 0x8),
                new Tuple<string, int, int>("ignoresDamageHitLocations",                                     0x3cc, 0x6),
                new Tuple<string, int, int>("ignoreVortex",                                                  0x3cd, 0x6),
                new Tuple<string, int, int>("usesCovernodes",                                                0x3ce, 0x6),
                new Tuple<string, int, int>("accurateFire",                                                  0x3cf, 0x6),
                new Tuple<string, int, int>("aggressiveMode",                                                0x3d0, 0x6),
                new Tuple<string, int, int>("hero",                                                          0x3d1, 0x6),
                new Tuple<string, int, int>("matureGib",                                                     0x3d2, 0x6),
                new Tuple<string, int, int>("restrictedGib",                                                 0x3d3, 0x6),
                new Tuple<string, int, int>("allowFriendlyFire",                                             0x3d4, 0x6),
                new Tuple<string, int, int>("proceduralTraversals",                                          0x3d5, 0x6),
                new Tuple<string, int, int>("shootanglethreshold",                                           0x3d8, 0x8),
                new Tuple<string, int, int>("closeshootanglethreshold",                                      0x3dc, 0x8),
                new Tuple<string, int, int>("closeshootangledistance",                                       0x3e0, 0x8),
                new Tuple<string, int, int>("coverCrouchPosOffsetX",                                         0x3e4, 0x4),
                new Tuple<string, int, int>("coverCrouchPosOffsetY",                                         0x3e8, 0x4),
                new Tuple<string, int, int>("coverCrouchRotOffset",                                          0x3ec, 0x8),
                new Tuple<string, int, int>("coverExposedPosOffsetX",                                        0x3f0, 0x4),
                new Tuple<string, int, int>("coverExposedPosOffsetY",                                        0x3f4, 0x4),
                new Tuple<string, int, int>("coverExposedRotOffset",                                         0x3f8, 0x8),
                new Tuple<string, int, int>("coverLeftPosOffsetX",                                           0x3fc, 0x4),
                new Tuple<string, int, int>("coverLeftPosOffsetY",                                           0x400, 0x4),
                new Tuple<string, int, int>("coverLeftRotOffset",                                            0x404, 0x8),
                new Tuple<string, int, int>("coverPillarPosOffsetX",                                         0x408, 0x4),
                new Tuple<string, int, int>("coverPillarPosOffsetY",                                         0x40c, 0x4),
                new Tuple<string, int, int>("coverPillarRotOffset",                                          0x410, 0x8),
                new Tuple<string, int, int>("coverRightPosOffsetX",                                          0x414, 0x4),
                new Tuple<string, int, int>("coverRightPosOffsetY",                                          0x418, 0x4),
                new Tuple<string, int, int>("coverRightRotOffset",                                           0x41c, 0x8),
                new Tuple<string, int, int>("coverStandPosOffsetX",                                          0x420, 0x4),
                new Tuple<string, int, int>("coverStandPosOffsetY",                                          0x424, 0x4),
                new Tuple<string, int, int>("coverStandRotOffset",                                           0x428, 0x8),
                new Tuple<string, int, int>("steeringNormalTurnRate",                                        0x42c, 0x8),
                new Tuple<string, int, int>("steeringNormalMaxTurnRateAngle",                                0x430, 0x8),
                new Tuple<string, int, int>("steeringNormalSensorSize",                                      0x434, 0x8),
                new Tuple<string, int, int>("steeringNormalCollisionPenalty",                                0x438, 0x8),
                new Tuple<string, int, int>("steeringNormalDodgingPenalty",                                  0x43c, 0x8),
                new Tuple<string, int, int>("steeringNormalPenetrationPenalty",                              0x440, 0x8),
                new Tuple<string, int, int>("steeringNormalSidednessChangingPenalty",                        0x444, 0x8),
                new Tuple<string, int, int>("steeringNormalVelocityHysteresis",                              0x448, 0x8),
                new Tuple<string, int, int>("steeringSlowTurnRate",                                          0x44c, 0x8),
                new Tuple<string, int, int>("steeringSlowMaxTurnRateAngle",                                  0x450, 0x8),
                new Tuple<string, int, int>("steeringSlowSensorSize",                                        0x454, 0x8),
                new Tuple<string, int, int>("steeringSlowCollisionPenalty",                                  0x458, 0x8),
                new Tuple<string, int, int>("steeringSlowDodgingPenalty",                                    0x45c, 0x8),
                new Tuple<string, int, int>("steeringSlowPenetrationPenalty",                                0x460, 0x8),
                new Tuple<string, int, int>("steeringSlowSidednessChangingPenalty",                          0x464, 0x8),
                new Tuple<string, int, int>("steeringSlowVelocityHysteresis",                                0x468, 0x8),
                new Tuple<string, int, int>("steeringVignetteTurnRate",                                      0x46c, 0x8),
                new Tuple<string, int, int>("steeringVignetteMaxTurnRateAngle",                              0x470, 0x8),
                new Tuple<string, int, int>("steeringVignetteSensorSize",                                    0x474, 0x8),
                new Tuple<string, int, int>("steeringVignetteCollisionPenalty",                              0x478, 0x8),
                new Tuple<string, int, int>("steeringVignetteDodgingPenalty",                                0x47c, 0x8),
                new Tuple<string, int, int>("steeringVignettePenetrationPenalty",                            0x480, 0x8),
                new Tuple<string, int, int>("steeringVignetteSidednessChangingPenalty",                      0x484, 0x8),
                new Tuple<string, int, int>("steeringVignetteVelocityHysteresis",                            0x488, 0x8),
                new Tuple<string, int, int>("canFlank",                                                      0x48c, 0x6),
                new Tuple<string, int, int>("flankingScoreScale",                                            0x490, 0x4),
                new Tuple<string, int, int>("flankingAngle",                                                 0x494, 0x8),
                new Tuple<string, int, int>("demoLockOnHighlightDistance",                                   0x498, 0x8),
                new Tuple<string, int, int>("demoLockOnViewHeightOffset1",                                   0x49c, 0x4),
                new Tuple<string, int, int>("demoLockOnViewPitchMin1",                                       0x4a0, 0x8),
                new Tuple<string, int, int>("demoLockOnViewPitchMax1",                                       0x4a4, 0x8),
                new Tuple<string, int, int>("demoLockOnViewHeightOffset2",                                   0x4a8, 0x4),
                new Tuple<string, int, int>("demoLockOnViewPitchMin2",                                       0x4ac, 0x8),
                new Tuple<string, int, int>("demoLockOnViewPitchMax2",                                       0x4b0, 0x8),
                new Tuple<string, int, int>("character1",                                                    0x4b8, 0xf),
                new Tuple<string, int, int>("character2",                                                    0x4c0, 0xf),
                new Tuple<string, int, int>("character3",                                                    0x4c8, 0xf),
                new Tuple<string, int, int>("character4",                                                    0x4d0, 0xf),
                new Tuple<string, int, int>("character5",                                                    0x4d8, 0xf),
                new Tuple<string, int, int>("character6",                                                    0x4e0, 0xf),
                new Tuple<string, int, int>("character7",                                                    0x4e8, 0xf),
                new Tuple<string, int, int>("character8",                                                    0x4f0, 0xf),
                new Tuple<string, int, int>("behaviorTreeName",                                              0x500, 0x36),
                new Tuple<string, int, int>("animStateMachineName",                                          0x508, 0x26),
                new Tuple<string, int, int>("animSelectorTableName",                                         0x510, 0x27),
                new Tuple<string, int, int>("animMappingTableDefault",                                       0x518, 0x34),
                new Tuple<string, int, int>("animMappingTableName",                                          0x520, 0x28),
                new Tuple<string, int, int>("animMappingTableName2",                                         0x528, 0x28),
                new Tuple<string, int, int>("animMappingTableName3",                                         0x530, 0x28),
                new Tuple<string, int, int>("aimTable",                                                      0x538, 0x29),
                new Tuple<string, int, int>("assassinationBundle",                                           0x540, 0x1b),
                new Tuple<string, int, int>("aivsaiMeleeBundle0",                                            0x548, 0x1b),
                new Tuple<string, int, int>("aivsaiMeleeBundle1",                                            0x550, 0x1b),
                new Tuple<string, int, int>("aivsaiMeleeBundle2",                                            0x558, 0x1b),
                new Tuple<string, int, int>("aivsaiMeleeBundle3",                                            0x560, 0x1b),
                new Tuple<string, int, int>("aivsaiMeleeBundle4",                                            0x568, 0x1b),
                new Tuple<string, int, int>("aiFxBundle",                                                    0x570, 0x1b),
                new Tuple<string, int, int>("femaleVersion",                                                 0x578, 0x15),
                new Tuple<string, int, int>("properName",                                                    0x57c, 0x3b),
                new Tuple<string, int, int>("randomName",                                                    0x580, 0x1b),
                new Tuple<string, int, int>("voicePrefix",                                                   0x810, 0x15),
                new Tuple<string, int, int>("compassIcon",                                                   0x818, 0x0),
                new Tuple<string, int, int>("tacticalModeIcon",                                              0x820, 0x10),
                new Tuple<string, int, int>("validationRequiredBones",                                       0x828, 0x3a),
                new Tuple<string, int, int>("validationNoCosmeticBones",                                     0x928, 0x6),
            };

            /// <summary>
            /// AI Species Types
            /// </summary>
            private static readonly string[] AISpecies =
            {
               "human",
               "dog",
               "zombie",
               "zombie_dog",
               "robot",
            };

            /// <summary>
            /// AI Fire Modes
            /// </summary>
            private static readonly string[] AIFireModes =
            {
               "fullauto",
               "burst",
               "singleshot",
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
            public string Name => "aitype";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "AI";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.aitype;

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

                for(int i = 0; i < AssetCount; i++)
                {
                    var address = StartAddress + (i * AssetSize);
                    var namePointer = instance.Reader.ReadInt64(address);

                    if (IsNullAsset(namePointer))
                        continue;

                    results.Add(new Asset()
                    {
                        Name        = instance.Reader.ReadNullTerminatedString(namePointer),
                        Type        = Name,
                        Zone        = ((BlackOps3)instance.Game).ZoneNames[address],
                        Information = "N/A",
                        Status      = "Loaded",
                        Data        = address,
                        LoadMethod  = ExportAsset,
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var individualGdt = instance.Settings["IndividualGDT", "Yes"] == "Yes";
                var overwrite = instance.Settings["OverwriteExisting", "Yes"] == "Yes";
                var result = ConvertAssetBufferToGDTAsset(instance.Reader.ReadBytes((long)asset.Data, AssetSize), AITypeOffsets, instance, HandleAITypeSettings);

                result.Type = "aitype";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);
            }

            /// <summary>
            /// Handles AIType Specific Settings
            /// </summary>
            private static object HandleAITypeSettings(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch (type)
                {
                    case 0x33:
                        {
                            var value = BitConverter.ToInt32(assetBuffer, offset);

                            switch (value)
                            {
                                case 19:
                                    return "neutral";
                                case 3:
                                    return "team3";
                                case 2:
                                    return "axis";
                                case 1:
                                    return "allies";
                                default:
                                    return "";
                            }
                        }
                    case 0x34:
                        return "table" + (BitConverter.ToInt32(assetBuffer, offset) + 1).ToString();
                    case 0x35:
                        return AISpecies[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x36:
                        return instance.Game.GetAssetName(BitConverter.ToInt64(assetBuffer, offset), instance);
                    case 0x37:
                        return AIFireModes[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x38:
                        {
                            {
                                string result = "";
                                for (int i = 0; i < 0x40; i++)
                                    result += BitConverter.ToInt32(assetBuffer, 0x58 + (i * 4)).ToString("X") + "\\r\\n";
                                return result;
                            }
                        }
                    case 0x39:
                        {
                            string result = "";
                            for (int i = 0; i < 0x64; i++)
                                result += instance.Game.GetString(BitConverter.ToInt32(assetBuffer, 0x158 + (i * 4)), instance) + "\\r\\n";
                            return result;
                        }
                    case 0x3A:
                        {
                            string result = "";
                            for (int i = 0; i < 0x40; i++)
                                result += instance.Game.GetString(BitConverter.ToInt32(assetBuffer, 0x828 + (i * 4)), instance) + "\\r\\n";
                            return result;
                        }
                    case 0x3B:
                        return instance.Game.GetString(BitConverter.ToInt32(assetBuffer, offset), instance);
                    default:
                        {
                            return null;
                        }
                }
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
