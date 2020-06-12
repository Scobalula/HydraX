using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Flametable Logic
        /// </summary>
        private class Flametable : IAssetPool
        {
            #region Tables
            /// <summary>
            /// AIType Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] FlametableOffsets =
            {
                new Tuple<string, int, int>("flameVar_streamChunkGravityStart",                              0x0, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkGravityEnd",                                0x4, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkMaxSize",                                   0x8, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkStartSize",                                 0xc, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkEndSize",                                   0x10, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkStartSizeRand",                             0x14, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkEndSizeRand",                               0x18, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkDistScalar",                                0x1c, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkDistSwayScale",                             0x20, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkDistSwayVelMax",                            0x24, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkSpeed",                                     0x28, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkDecel",                                     0x2c, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkVelocityAddScale",                          0x30, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkDuration",                                  0x34, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkDurationScaleMaxVel",                       0x38, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkDurationVelScalar",                         0x3c, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkSizeSpeedScale",                            0x40, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkSizeAgeScale",                              0x44, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkSpawnFireIntervalStart",                    0x48, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkSpawnFireIntervalEnd",                      0x4c, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkSpawnFireMinLifeFrac",                      0x50, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkSpawnFireMaxLifeFrac",                      0x54, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkFireMinLifeFrac",                           0x58, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkFireMinLifeFracStart",                      0x5c, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkFireMinLifeFracEnd",                        0x60, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkDripsMinLifeFrac",                          0x64, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkDripsMinLifeFracStart",                     0x68, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkDripsMinLifeFracEnd",                       0x6c, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkRotationRange",                             0x70, 0x8),
                new Tuple<string, int, int>("flameVar_streamSizeRandSinWave",                                0x74, 0x8),
                new Tuple<string, int, int>("flameVar_streamSizeRandCosWave",                                0x78, 0x8),
                new Tuple<string, int, int>("flameVar_streamDripsChunkInterval",                             0x7c, 0x8),
                new Tuple<string, int, int>("flameVar_streamDripsChunkMinFrac",                              0x80, 0x8),
                new Tuple<string, int, int>("flameVar_streamDripsChunkRandFrac",                             0x84, 0x8),
                new Tuple<string, int, int>("flameVar_streamSmokeChunkInterval",                             0x88, 0x8),
                new Tuple<string, int, int>("flameVar_streamSmokeChunkMinFrac",                              0x8c, 0x8),
                new Tuple<string, int, int>("flameVar_streamSmokeChunkRandFrac",                             0x90, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkCullDistSizeFrac",                          0x94, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkCullMinLife",                               0x98, 0x8),
                new Tuple<string, int, int>("flameVar_streamChunkCullMaxLife",                               0x9c, 0x8),
                new Tuple<string, int, int>("flameVar_streamFuelSizeStart",                                  0xa0, 0x8),
                new Tuple<string, int, int>("flameVar_streamFuelSizeEnd",                                    0xa4, 0x8),
                new Tuple<string, int, int>("flameVar_streamFuelLength",                                     0xa8, 0x8),
                new Tuple<string, int, int>("flameVar_streamFuelNumSegments",                                0xac, 0x8),
                new Tuple<string, int, int>("flameVar_streamFuelAnimLoopTime",                               0xb0, 0x8),
                new Tuple<string, int, int>("flameVar_streamFlameSizeStart",                                 0xb4, 0x8),
                new Tuple<string, int, int>("flameVar_streamFlameSizeEnd",                                   0xb8, 0x8),
                new Tuple<string, int, int>("flameVar_streamFlameLength",                                    0xbc, 0x8),
                new Tuple<string, int, int>("flameVar_streamFlameNumSegments",                               0xc0, 0x8),
                new Tuple<string, int, int>("flameVar_streamFlameAnimLoopTime",                              0xc4, 0x8),
                new Tuple<string, int, int>("flameVar_streamPrimaryLightRadius",                             0xc8, 0x8),
                new Tuple<string, int, int>("flameVar_streamPrimaryLightRadiusFlutter",                      0xcc, 0x8),
                new Tuple<string, int, int>("flameVar_streamPrimaryLightR",                                  0xd0, 0x8),
                new Tuple<string, int, int>("flameVar_streamPrimaryLightG",                                  0xd4, 0x8),
                new Tuple<string, int, int>("flameVar_streamPrimaryLightB",                                  0xd8, 0x8),
                new Tuple<string, int, int>("flameVar_streamPrimaryLightFlutterR",                           0xdc, 0x8),
                new Tuple<string, int, int>("flameVar_streamPrimaryLightFlutterG",                           0xe0, 0x8),
                new Tuple<string, int, int>("flameVar_streamPrimaryLightFlutterB",                           0xe4, 0x8),
                new Tuple<string, int, int>("flameVar_fireLife",                                             0xe8, 0x8),
                new Tuple<string, int, int>("flameVar_fireLifeRand",                                         0xec, 0x8),
                new Tuple<string, int, int>("flameVar_fireSpeedScale",                                       0xf0, 0x8),
                new Tuple<string, int, int>("flameVar_fireSpeedScaleRand",                                   0xf4, 0x8),
                new Tuple<string, int, int>("flameVar_fireVelocityAddZ",                                     0xf8, 0x8),
                new Tuple<string, int, int>("flameVar_fireVelocityAddZRand",                                 0xfc, 0x8),
                new Tuple<string, int, int>("flameVar_fireVelocityAddSideways",                              0x100, 0x8),
                new Tuple<string, int, int>("flameVar_fireGravity",                                          0x104, 0x8),
                new Tuple<string, int, int>("flameVar_fireGravityEnd",                                       0x108, 0x8),
                new Tuple<string, int, int>("flameVar_fireMaxRotVel",                                        0x10c, 0x8),
                new Tuple<string, int, int>("flameVar_fireFriction",                                         0x110, 0x8),
                new Tuple<string, int, int>("flameVar_fireEndSizeAdd",                                       0x114, 0x8),
                new Tuple<string, int, int>("flameVar_fireStartSizeScale",                                   0x118, 0x8),
                new Tuple<string, int, int>("flameVar_fireEndSizeScale",                                     0x11c, 0x8),
                new Tuple<string, int, int>("flameVar_fireBrightness",                                       0x120, 0x8),
                new Tuple<string, int, int>("flameVar_dripsLife",                                            0x124, 0x8),
                new Tuple<string, int, int>("flameVar_dripsLifeRand",                                        0x128, 0x8),
                new Tuple<string, int, int>("flameVar_dripsSpeedScale",                                      0x12c, 0x8),
                new Tuple<string, int, int>("flameVar_dripsSpeedScaleRand",                                  0x130, 0x8),
                new Tuple<string, int, int>("flameVar_dripsVelocityAddZ",                                    0x134, 0x8),
                new Tuple<string, int, int>("flameVar_dripsVelocityAddZRand",                                0x138, 0x8),
                new Tuple<string, int, int>("flameVar_dripsVelocityAddSideways",                             0x13c, 0x8),
                new Tuple<string, int, int>("flameVar_dripsGravity",                                         0x140, 0x8),
                new Tuple<string, int, int>("flameVar_dripsGravityEnd",                                      0x144, 0x8),
                new Tuple<string, int, int>("flameVar_dripsMaxRotVel",                                       0x148, 0x8),
                new Tuple<string, int, int>("flameVar_dripsFriction",                                        0x14c, 0x8),
                new Tuple<string, int, int>("flameVar_dripsEndSizeAdd",                                      0x150, 0x8),
                new Tuple<string, int, int>("flameVar_dripsStartSizeScale",                                  0x154, 0x8),
                new Tuple<string, int, int>("flameVar_dripsEndSizeScale",                                    0x158, 0x8),
                new Tuple<string, int, int>("flameVar_dripsBrightness",                                      0x15c, 0x8),
                new Tuple<string, int, int>("flameVar_smokeLife",                                            0x160, 0x8),
                new Tuple<string, int, int>("flameVar_smokeLifeRand",                                        0x164, 0x8),
                new Tuple<string, int, int>("flameVar_smokeSpeedScale",                                      0x168, 0x8),
                new Tuple<string, int, int>("flameVar_smokeVelocityAddZ",                                    0x16c, 0x8),
                new Tuple<string, int, int>("flameVar_smokeGravity",                                         0x170, 0x8),
                new Tuple<string, int, int>("flameVar_smokeGravityEnd",                                      0x174, 0x8),
                new Tuple<string, int, int>("flameVar_smokeMaxRotation",                                     0x178, 0x8),
                new Tuple<string, int, int>("flameVar_smokeMaxRotVel",                                       0x17c, 0x8),
                new Tuple<string, int, int>("flameVar_smokeFriction",                                        0x180, 0x8),
                new Tuple<string, int, int>("flameVar_smokeEndSizeAdd",                                      0x184, 0x8),
                new Tuple<string, int, int>("flameVar_smokeStartSizeAdd",                                    0x188, 0x8),
                new Tuple<string, int, int>("flameVar_smokeOriginSizeOfsZScale",                             0x18c, 0x8),
                new Tuple<string, int, int>("flameVar_smokeOriginOfsZ",                                      0x190, 0x8),
                new Tuple<string, int, int>("flameVar_smokeFadein",                                          0x194, 0x8),
                new Tuple<string, int, int>("flameVar_smokeFadeout",                                         0x198, 0x8),
                new Tuple<string, int, int>("flameVar_smokeMaxAlpha",                                        0x19c, 0x8),
                new Tuple<string, int, int>("flameVar_smokeBrightness",                                      0x1a0, 0x8),
                new Tuple<string, int, int>("flameVar_smokeOriginOffset",                                    0x1a4, 0x8),
                new Tuple<string, int, int>("flameVar_collisionSpeedScale",                                  0x1a8, 0x8),
                new Tuple<string, int, int>("flameVar_collisionVolumeScale",                                 0x1ac, 0x8),
                new Tuple<string, int, int>("name",                                                          0x1b0, 0x0),
                new Tuple<string, int, int>("fire",                                                          0x1b8, 0x33),
                new Tuple<string, int, int>("smoke",                                                         0x1c0, 0x33),
                new Tuple<string, int, int>("heat",                                                          0x1c8, 0x33),
                new Tuple<string, int, int>("drips",                                                         0x1d0, 0x33),
                new Tuple<string, int, int>("streamFuel",                                                    0x1d8, 0x33),
                new Tuple<string, int, int>("streamFuel2",                                                   0x1e0, 0x33),
                new Tuple<string, int, int>("streamFlame",                                                   0x1e8, 0x33),
                new Tuple<string, int, int>("streamFlame2",                                                  0x1f0, 0x33),
                new Tuple<string, int, int>("flameOffLoopSound",                                             0x1f8, 0x0),
                new Tuple<string, int, int>("flameIgniteSound",                                              0x200, 0x0),
                new Tuple<string, int, int>("flameOnLoopSound",                                              0x208, 0x0),
                new Tuple<string, int, int>("flameCooldownSound",                                            0x210, 0x0),
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
            public string Name => "flametable";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Weapon";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.flametable;

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
                    var namePointer = instance.Reader.ReadInt64(address + 0x1B0);

                    if (IsNullAsset(instance.Reader.ReadInt64(address)))
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

                if (asset.Name != instance.Reader.ReadNullTerminatedString(BitConverter.ToInt64(buffer, 0x1B0)))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                var result = ConvertAssetBufferToGDTAsset(buffer, FlametableOffsets, instance, HandleFlametableSettings);

                result.Type = "flametable";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);

                return;
            }

            /// <summary>
            /// Handles Flametable Specific Settings
            /// </summary>
            private static object HandleFlametableSettings(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch (type)
                {
                    case 0x33:
                        return instance.Game.GetAssetName(BitConverter.ToInt64(assetBuffer, offset), instance);
                    default:
                        return null;
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
