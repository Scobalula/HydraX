using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 PlayerSoundsTable Logic
        /// </summary>
        private class PlayerSoundsTable : IAssetPool
        {
            #region Tables
            /// <summary>
            /// PlayerSoundsTable Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] PlayerSoundsTableOffsets =
            {
                new Tuple<string, int, int>("fallImpact",                                                    0x8, 0x1f),
                new Tuple<string, int, int>("fallImpactSlow",                                                0x10, 0x1f),
                new Tuple<string, int, int>("fallImpactMed",                                                 0x18, 0x1f),
                new Tuple<string, int, int>("fallImpactFast",                                                0x20, 0x1f),
                new Tuple<string, int, int>("fallImpactRapid",                                               0x28, 0x1f),
                new Tuple<string, int, int>("traverseImpact",                                                0x30, 0x18),
                new Tuple<string, int, int>("traverseImpactSlow",                                            0x34, 0x18),
                new Tuple<string, int, int>("traverseImpactMed",                                             0x38, 0x18),
                new Tuple<string, int, int>("traverseImpactFast",                                            0x3c, 0x18),
                new Tuple<string, int, int>("traverseImpactRapid",                                           0x40, 0x18),
                new Tuple<string, int, int>("wallrunImpact",                                                 0x48, 0x1f),
                new Tuple<string, int, int>("wallrunImpactSlow",                                             0x50, 0x1f),
                new Tuple<string, int, int>("wallrunImpactMed",                                              0x58, 0x1f),
                new Tuple<string, int, int>("wallrunImpactFast",                                             0x60, 0x1f),
                new Tuple<string, int, int>("wallrunImpactRapid",                                            0x68, 0x1f),
                new Tuple<string, int, int>("waterImpact",                                                   0x70, 0x18),
                new Tuple<string, int, int>("waterImpactSlow",                                               0x74, 0x18),
                new Tuple<string, int, int>("waterImpactMed",                                                0x78, 0x18),
                new Tuple<string, int, int>("waterImpactFast",                                               0x7c, 0x18),
                new Tuple<string, int, int>("waterImpactRapid",                                              0x80, 0x18),
                new Tuple<string, int, int>("traverseDefault",                                               0x84, 0x18),
                new Tuple<string, int, int>("traverseStepForward",                                           0x88, 0x18),
                new Tuple<string, int, int>("traverseStepBack",                                              0x8c, 0x18),
                new Tuple<string, int, int>("traverseStepLeft",                                              0x90, 0x18),
                new Tuple<string, int, int>("traverseStepRight",                                             0x94, 0x18),
                new Tuple<string, int, int>("traverseOnLow",                                                 0x98, 0x18),
                new Tuple<string, int, int>("traverseOnLowBack",                                             0x9c, 0x18),
                new Tuple<string, int, int>("traverseOnLowLeft",                                             0xa0, 0x18),
                new Tuple<string, int, int>("traverseOnLowRight",                                            0xa4, 0x18),
                new Tuple<string, int, int>("traverseOnMed",                                                 0xa8, 0x18),
                new Tuple<string, int, int>("traverseOnMedLeft",                                             0xac, 0x18),
                new Tuple<string, int, int>("traverseOnMedRight",                                            0xb0, 0x18),
                new Tuple<string, int, int>("traverseOverLowVault",                                          0xb4, 0x18),
                new Tuple<string, int, int>("traverseOverLow",                                               0xb8, 0x18),
                new Tuple<string, int, int>("traverseOverLowBack",                                           0xbc, 0x18),
                new Tuple<string, int, int>("traverseOverLowLeft",                                           0xc0, 0x18),
                new Tuple<string, int, int>("traverseOverLowRight",                                          0xc4, 0x18),
                new Tuple<string, int, int>("traverseOverHigh",                                              0xc8, 0x18),
                new Tuple<string, int, int>("traverseOverHigher",                                            0xcc, 0x18),
                new Tuple<string, int, int>("traverseOverHighest",                                           0xd0, 0x18),
                new Tuple<string, int, int>("traverseOnHigher",                                              0xd4, 0x18),
                new Tuple<string, int, int>("traverseOnHighest",                                             0xd8, 0x18),
                new Tuple<string, int, int>("jetpackStart",                                                  0xdc, 0x18),
                new Tuple<string, int, int>("jetpackStartFull",                                              0xe0, 0x18),
                new Tuple<string, int, int>("jetpackLoop",                                                   0xe4, 0x18),
                new Tuple<string, int, int>("jetpackEnd",                                                    0xe8, 0x18),
                new Tuple<string, int, int>("jetpackExhausted",                                              0xec, 0x18),
                new Tuple<string, int, int>("jetpackStartEmp",                                               0xf0, 0x18),
                new Tuple<string, int, int>("jetpackStartFullEmp",                                           0xf4, 0x18),
                new Tuple<string, int, int>("jetpackLoopEmp",                                                0xf8, 0x18),
                new Tuple<string, int, int>("jetpackEndEmp",                                                 0xfc, 0x18),
                new Tuple<string, int, int>("jetpackExhaustedEmp",                                           0x100, 0x18),
                new Tuple<string, int, int>("jetpackEnergyEmpty",                                            0x104, 0x18),
                new Tuple<string, int, int>("jetpackDisabled",                                               0x108, 0x18),
                new Tuple<string, int, int>("jetpackQuietStart",                                             0x10c, 0x18),
                new Tuple<string, int, int>("jetpackQuietStartFull",                                         0x110, 0x18),
                new Tuple<string, int, int>("jetpackQuietLoop",                                              0x114, 0x18),
                new Tuple<string, int, int>("jetpackQuietEnd",                                               0x118, 0x18),
                new Tuple<string, int, int>("jetpackQuietExhausted",                                         0x11c, 0x18),
                new Tuple<string, int, int>("jetpackQuietStartEmp",                                          0x120, 0x18),
                new Tuple<string, int, int>("jetpackQuietStartFullEmp",                                      0x124, 0x18),
                new Tuple<string, int, int>("jetpackQuietLoopEmp",                                           0x128, 0x18),
                new Tuple<string, int, int>("jetpackQuietEndEmp",                                            0x12c, 0x18),
                new Tuple<string, int, int>("jetpackQuietExhaustedEmp",                                      0x130, 0x18),
                new Tuple<string, int, int>("jetpackQuietEnergyEmpty",                                       0x134, 0x18),
                new Tuple<string, int, int>("jetpackQuietDisabled",                                          0x138, 0x18),
                new Tuple<string, int, int>("fallWind",                                                      0x13c, 0x18),
                new Tuple<string, int, int>("fallDamage",                                                    0x140, 0x18),
                new Tuple<string, int, int>("jukeStart",                                                     0x144, 0x18),
                new Tuple<string, int, int>("jukeLoop",                                                      0x148, 0x18),
                new Tuple<string, int, int>("jukeEnd",                                                       0x14c, 0x18),
                new Tuple<string, int, int>("sprintEnergyStart",                                             0x150, 0x18),
                new Tuple<string, int, int>("sprintEnergyLoop",                                              0x154, 0x18),
                new Tuple<string, int, int>("sprintEnergyEnd",                                               0x158, 0x18),
                new Tuple<string, int, int>("wallrunEnergyStart",                                            0x15c, 0x18),
                new Tuple<string, int, int>("wallrunEnergyEnd",                                              0x160, 0x18),
                new Tuple<string, int, int>("wallrunEnergyLoop",                                             0x164, 0x18),
                new Tuple<string, int, int>("wallrunQuietEnergyStart",                                       0x168, 0x18),
                new Tuple<string, int, int>("wallrunQuietEnergyEnd",                                         0x16c, 0x18),
                new Tuple<string, int, int>("wallrunQuietEnergyLoop",                                        0x170, 0x18),
                new Tuple<string, int, int>("slideEnergyStart",                                              0x174, 0x18),
                new Tuple<string, int, int>("slideEnergyStartQuiet",                                         0x178, 0x18),
                new Tuple<string, int, int>("slideEnergyEmpStartThirdPerson",                                0x17c, 0x18),
                new Tuple<string, int, int>("slideEnergyEmpStartThirdPersonQuiet",                           0x180, 0x18),
                new Tuple<string, int, int>("swimStart",                                                     0x184, 0x18),
                new Tuple<string, int, int>("swimEnd",                                                       0x188, 0x18),
                new Tuple<string, int, int>("underWaterStart",                                               0x18c, 0x18),
                new Tuple<string, int, int>("underWaterEnd",                                                 0x190, 0x18),
                new Tuple<string, int, int>("treadLoop",                                                     0x194, 0x18),
                new Tuple<string, int, int>("underWaterLoop",                                                0x198, 0x18),
                new Tuple<string, int, int>("walkStart",                                                     0x19c, 0x18),
                new Tuple<string, int, int>("walkEnd",                                                       0x1a0, 0x18),
                new Tuple<string, int, int>("runStart",                                                      0x1a4, 0x18),
                new Tuple<string, int, int>("runEnd",                                                        0x1a8, 0x18),
                new Tuple<string, int, int>("sprintStart",                                                   0x1ac, 0x18),
                new Tuple<string, int, int>("sprintEnd",                                                     0x1b0, 0x18),
                new Tuple<string, int, int>("rotateLoop",                                                    0x1b4, 0x18),
                new Tuple<string, int, int>("rotateStep",                                                    0x1b8, 0x18),
                new Tuple<string, int, int>("toStand",                                                       0x1bc, 0x18),
                new Tuple<string, int, int>("standToCrouch",                                                 0x1c0, 0x18),
                new Tuple<string, int, int>("assassinationStart",                                            0x1c4, 0x18),
                new Tuple<string, int, int>("meleeTinnitus",                                                 0x1c8, 0x18),
                new Tuple<string, int, int>("jumpTakeoff",                                                   0x1d0, 0x1f),
                new Tuple<string, int, int>("leapTakeoff",                                                   0x1d8, 0x1f),
                new Tuple<string, int, int>("slideStart",                                                    0x1e0, 0x1f),
                new Tuple<string, int, int>("slideLoop",                                                     0x1e8, 0x1f),
                new Tuple<string, int, int>("slideEnd",                                                      0x1f0, 0x1f),
                new Tuple<string, int, int>("wallrunStart",                                                  0x1f8, 0x1f),
                new Tuple<string, int, int>("wallrunLoop",                                                   0x200, 0x1f),
                new Tuple<string, int, int>("wallrunEnd",                                                    0x208, 0x1f),
                new Tuple<string, int, int>("wallrunFootstep",                                               0x210, 0x1f),
                new Tuple<string, int, int>("wallrunFallStart",                                              0x218, 0x1f),
                new Tuple<string, int, int>("wallrunFallStep",                                               0x220, 0x1f),
                new Tuple<string, int, int>("wallrunFallEnd",                                                0x228, 0x1f),
                new Tuple<string, int, int>("toProne",                                                       0x230, 0x1f),
                new Tuple<string, int, int>("proneToCrouch",                                                 0x238, 0x1f),
                new Tuple<string, int, int>("fallImpactThirdPerson",                                         0x240, 0x1f),
                new Tuple<string, int, int>("fallImpactSlowThirdPerson",                                     0x248, 0x1f),
                new Tuple<string, int, int>("fallImpactMedThirdPerson",                                      0x250, 0x1f),
                new Tuple<string, int, int>("fallImpactFastThirdPerson",                                     0x258, 0x1f),
                new Tuple<string, int, int>("fallImpactRapidThirdPerson",                                    0x260, 0x1f),
                new Tuple<string, int, int>("traverseImpactThirdPerson",                                     0x268, 0x18),
                new Tuple<string, int, int>("wallrunImpactThirdPerson",                                      0x270, 0x1f),
                new Tuple<string, int, int>("waterImpactSlowThirdPerson",                                    0x278, 0x18),
                new Tuple<string, int, int>("waterImpactSlowThirdPerson",                                    0x27c, 0x18),
                new Tuple<string, int, int>("waterImpactMedThirdPerson",                                     0x280, 0x18),
                new Tuple<string, int, int>("waterImpactFastThirdPerson",                                    0x284, 0x18),
                new Tuple<string, int, int>("waterImpactRapidThirdPerson",                                   0x288, 0x18),
                new Tuple<string, int, int>("traverseDefaultThirdPerson",                                    0x28c, 0x18),
                new Tuple<string, int, int>("traverseStepForwardThirdPerson",                                0x290, 0x18),
                new Tuple<string, int, int>("traverseStepBackThirdPerson",                                   0x294, 0x18),
                new Tuple<string, int, int>("traverseStepLeftThirdPerson",                                   0x298, 0x18),
                new Tuple<string, int, int>("traverseStepRightThirdPerson",                                  0x29c, 0x18),
                new Tuple<string, int, int>("traverseOnLowThirdPerson",                                      0x2a0, 0x18),
                new Tuple<string, int, int>("traverseOnLowBackThirdPerson",                                  0x2a4, 0x18),
                new Tuple<string, int, int>("traverseOnLowLeftThirdPerson",                                  0x2a8, 0x18),
                new Tuple<string, int, int>("traverseOnLowRightThirdPerson",                                 0x2ac, 0x18),
                new Tuple<string, int, int>("traverseOnMedThirdPerson",                                      0x2b0, 0x18),
                new Tuple<string, int, int>("traverseOnMedLeftThirdPerson",                                  0x2b4, 0x18),
                new Tuple<string, int, int>("traverseOnMedRightThirdPerson",                                 0x2b8, 0x18),
                new Tuple<string, int, int>("traverseOverLowVaultThirdPerson",                               0x2bc, 0x18),
                new Tuple<string, int, int>("traverseOverLowThirdPerson",                                    0x2c0, 0x18),
                new Tuple<string, int, int>("traverseOverLowBackThirdPerson",                                0x2c4, 0x18),
                new Tuple<string, int, int>("traverseOverLowLeftThirdPerson",                                0x2c8, 0x18),
                new Tuple<string, int, int>("traverseOverLowRightThirdPerson",                               0x2cc, 0x18),
                new Tuple<string, int, int>("traverseOverHighThirdPerson",                                   0x2d0, 0x18),
                new Tuple<string, int, int>("traverseOverHigherThirdPerson",                                 0x2d4, 0x18),
                new Tuple<string, int, int>("traverseOverHighestThirdPerson",                                0x2d8, 0x18),
                new Tuple<string, int, int>("traverseOnHigherThirdPerson",                                   0x2dc, 0x18),
                new Tuple<string, int, int>("traverseOnHighestThirdPerson",                                  0x2e0, 0x18),
                new Tuple<string, int, int>("jetpackStartThirdPerson",                                       0x2e4, 0x18),
                new Tuple<string, int, int>("jetpackStartFullThirdPerson",                                   0x2e8, 0x18),
                new Tuple<string, int, int>("jetpackLoopThirdPerson",                                        0x2ec, 0x18),
                new Tuple<string, int, int>("jetpackEndThirdPerson",                                         0x2f0, 0x18),
                new Tuple<string, int, int>("jetpackExhaustedThirdPerson",                                   0x2f4, 0x18),
                new Tuple<string, int, int>("jetpackStartThirdPersonEmp",                                    0x2f8, 0x18),
                new Tuple<string, int, int>("jetpackStartFullThirdPersonEmp",                                0x2fc, 0x18),
                new Tuple<string, int, int>("jetpackLoopThirdPersonEmp",                                     0x300, 0x18),
                new Tuple<string, int, int>("jetpackEndThirdPersonEmp",                                      0x304, 0x18),
                new Tuple<string, int, int>("jetpackExhaustedThirdPersonEmp",                                0x308, 0x18),
                new Tuple<string, int, int>("jetpackEnergyEmptyThirdPerson",                                 0x30c, 0x18),
                new Tuple<string, int, int>("jetpackDisabledThirdPerson",                                    0x310, 0x18),
                new Tuple<string, int, int>("jetpackQuietStartThirdPerson",                                  0x314, 0x18),
                new Tuple<string, int, int>("jetpackQuietStartFullThirdPerson",                              0x318, 0x18),
                new Tuple<string, int, int>("jetpackQuietLoopThirdPerson",                                   0x31c, 0x18),
                new Tuple<string, int, int>("jetpackQuietEndThirdPerson",                                    0x320, 0x18),
                new Tuple<string, int, int>("jetpackQuietExhaustedThirdPerson",                              0x324, 0x18),
                new Tuple<string, int, int>("jetpackQuietStartThirdPersonEmp",                               0x328, 0x18),
                new Tuple<string, int, int>("jetpackQuietStartFullThirdPersonEmp",                           0x32c, 0x18),
                new Tuple<string, int, int>("jetpackQuietLoopThirdPersonEmp",                                0x330, 0x18),
                new Tuple<string, int, int>("jetpackQuietEndThirdPersonEmp",                                 0x334, 0x18),
                new Tuple<string, int, int>("jetpackQuietExhaustedThirdPersonEmp",                           0x338, 0x18),
                new Tuple<string, int, int>("jetpackQuietEnergyEmptyThirdPerson",                            0x33c, 0x18),
                new Tuple<string, int, int>("jetpackQuietDisabledThirdPerson",                               0x340, 0x18),
                new Tuple<string, int, int>("fallWindThirdPerson",                                           0x344, 0x18),
                new Tuple<string, int, int>("fallDamageThirdPerson",                                         0x348, 0x18),
                new Tuple<string, int, int>("jukeStartThirdPerson",                                          0x34c, 0x18),
                new Tuple<string, int, int>("jukeLoopThirdPerson",                                           0x350, 0x18),
                new Tuple<string, int, int>("jukeEndThirdPerson",                                            0x354, 0x18),
                new Tuple<string, int, int>("sprintEnergyStartThirdPerson",                                  0x358, 0x18),
                new Tuple<string, int, int>("jetpackExhaustedThirdPerson",                                   0x35c, 0x18),
                new Tuple<string, int, int>("sprintEnergyLoopThirdPerson",                                   0x360, 0x18),
                new Tuple<string, int, int>("wallrunEnergyStartThirdPerson",                                 0x364, 0x18),
                new Tuple<string, int, int>("wallrunEnergyEndThirdPerson",                                   0x368, 0x18),
                new Tuple<string, int, int>("wallrunEnergyLoopThirdPerson",                                  0x36c, 0x18),
                new Tuple<string, int, int>("wallrunQuietEnergyStartThirdPerson",                            0x370, 0x18),
                new Tuple<string, int, int>("wallrunQuietEnergyEndThirdPerson",                              0x374, 0x18),
                new Tuple<string, int, int>("wallrunQuietEnergyLoopThirdPerson",                             0x378, 0x18),
                new Tuple<string, int, int>("slideEnergyStartThirdPerson",                                   0x37c, 0x18),
                new Tuple<string, int, int>("slideEnergyStartThirdPersonQuiet",                              0x380, 0x18),
                new Tuple<string, int, int>("swimStartThirdPerson",                                          0x38c, 0x18),
                new Tuple<string, int, int>("swimEndThirdPerson",                                            0x390, 0x18),
                new Tuple<string, int, int>("underWaterStartThirdPerson",                                    0x394, 0x18),
                new Tuple<string, int, int>("underWaterEndThirdPerson",                                      0x398, 0x18),
                new Tuple<string, int, int>("treadLoopThirdPerson",                                          0x39c, 0x18),
                new Tuple<string, int, int>("underWaterLoopThirdPerson",                                     0x3a0, 0x18),
                new Tuple<string, int, int>("walkStartThirdPerson",                                          0x3a4, 0x18),
                new Tuple<string, int, int>("walkEndThirdPerson",                                            0x3a8, 0x18),
                new Tuple<string, int, int>("runStartThirdPerson",                                           0x3ac, 0x18),
                new Tuple<string, int, int>("runEndThirdPerson",                                             0x3b0, 0x18),
                new Tuple<string, int, int>("sprintStartThirdPerson",                                        0x3b4, 0x18),
                new Tuple<string, int, int>("sprintEndThirdPerson",                                          0x3b8, 0x18),
                new Tuple<string, int, int>("toStandThirdPerson",                                            0x3c4, 0x18),
                new Tuple<string, int, int>("standToCrouchThirdPerson",                                      0x3c8, 0x18),
                new Tuple<string, int, int>("jumpTakeoffThirdPerson",                                        0x3d8, 0x1f),
                new Tuple<string, int, int>("leapTakeoffThirdPerson",                                        0x3e0, 0x1f),
                new Tuple<string, int, int>("slideStartThirdPerson",                                         0x3e8, 0x1f),
                new Tuple<string, int, int>("slideLoopThirdPerson",                                          0x3f0, 0x1f),
                new Tuple<string, int, int>("slideEndThirdPerson",                                           0x3f8, 0x1f),
                new Tuple<string, int, int>("wallrunStartThirdPerson",                                       0x400, 0x1f),
                new Tuple<string, int, int>("wallrunLoopThirdPerson",                                        0x408, 0x1f),
                new Tuple<string, int, int>("wallrunEndThirdPerson",                                         0x410, 0x1f),
                new Tuple<string, int, int>("wallrunFootstepThirdPerson",                                    0x418, 0x1f),
                new Tuple<string, int, int>("wallrunFallStartThirdPerson",                                   0x420, 0x1f),
                new Tuple<string, int, int>("wallrunFallStepThirdPerson",                                    0x428, 0x1f),
                new Tuple<string, int, int>("wallrunFallEndThirdPerson",                                     0x430, 0x1f),
                new Tuple<string, int, int>("toProneThirdPerson",                                            0x438, 0x1f),
                new Tuple<string, int, int>("proneToCrouchThirdPerson",                                      0x440, 0x1f),
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
            public string Name => "playersoundstable";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.playersoundstable;

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
                var buffer = instance.Reader.ReadBytes((long)asset.Data, AssetSize);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(BitConverter.ToInt64(buffer, 0)))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                var result = ConvertAssetBufferToGDTAsset(buffer, PlayerSoundsTableOffsets, instance);

                result.Type = "playersoundstable";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);
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
