using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 DestructibleDef Logic
        /// </summary>
        private class DestructibleDef : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// DestructibleDef Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct DestructibleDefAsset
            {
                public long NamePointer;
                public long ModelPointer;
                public long PrestineModelPointer;
                public int PieceCount;
                public long PiecesPointer;
                public int ClientOnly;
                public int SyncBaseHealthWithEntity;

            }
            #endregion

            #region Tables
            /// <summary>
            /// DestructibleDef Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] DestructiblePieceOffsets =
            {
                new Tuple<string, int, int>("showBone",                                                     0x0, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone0",                                            0x4, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone1",                                            0x8, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone2",                                            0xc, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone3",                                            0x10, 0x15),
                new Tuple<string, int, int>("breakHealth",                                                  0x14, 0x8),
                new Tuple<string, int, int>("maxTime",                                                      0x18, 0x8),
                new Tuple<string, int, int>("explosiveDamageOnly",                                          0x1c, 0x33),
                new Tuple<string, int, int>("meleeDamage",                                                  0x1c, 0x3a),
                new Tuple<string, int, int>("parentDamage",                                                 0x1c, 0x41),
                new Tuple<string, int, int>("physicsParent",                                                0x1c, 0x48),
                new Tuple<string, int, int>("breakOnEntityDeath",                                           0x1c, 0x4f),
                new Tuple<string, int, int>("includeChildren",                                              0x1c, 0x56),
                new Tuple<string, int, int>("breakAnim",                                                    0x2c, 0x15),
                new Tuple<string, int, int>("breakEffect",                                                  0x30, 0xa),
                new Tuple<string, int, int>("breakEffectTag",                                               0x38, 0x15),
                new Tuple<string, int, int>("breakSound",                                                   0x40, 0x0),
                new Tuple<string, int, int>("loopSound",                                                    0x50, 0x0),
                new Tuple<string, int, int>("breakNotify",                                                  0x48, 0x0),
                new Tuple<string, int, int>("baseEffect",                                                   0x20, 0xa),
                new Tuple<string, int, int>("baseEffectTag",                                                0x28, 0x15),
                new Tuple<string, int, int>("spawnModel1",                                                  0x58, 0xd),
                new Tuple<string, int, int>("spawnModel2",                                                  0x60, 0xd),
                new Tuple<string, int, int>("physPreset",                                                   0x70, 0x14),
                new Tuple<string, int, int>("showBoneD1",                                                   0x78, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone0D1",                                          0x7c, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone1D1",                                          0x80, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone2D1",                                          0x84, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone3D1",                                          0x88, 0x15),
                new Tuple<string, int, int>("breakHealthD1",                                                0x8c, 0x8),
                new Tuple<string, int, int>("maxTimeD1",                                                    0x90, 0x8),
                new Tuple<string, int, int>("explosiveDamageOnlyD1",                                        0x94, 0x34),
                new Tuple<string, int, int>("meleeDamageD1",                                                0x94, 0x3b),
                new Tuple<string, int, int>("parentDamageD1",                                               0x94, 0x42),
                new Tuple<string, int, int>("physicsParentD1",                                              0x94, 0x49),
                new Tuple<string, int, int>("breakOnEntityDeathD1",                                         0x94, 0x50),
                new Tuple<string, int, int>("includeChildrenD1",                                            0x94, 0x57),
                new Tuple<string, int, int>("breakAnimD1",                                                  0xa4, 0x15),
                new Tuple<string, int, int>("breakEffectD1",                                                0xa8, 0xa),
                new Tuple<string, int, int>("breakEffectTagD1",                                             0xb0, 0x15),
                new Tuple<string, int, int>("breakSoundD1",                                                 0xb8, 0x0),
                new Tuple<string, int, int>("loopSoundD1",                                                  0xc8, 0x0),
                new Tuple<string, int, int>("breakNotifyD1",                                                0xc0, 0x0),
                new Tuple<string, int, int>("spawnModel1D1",                                                0xd0, 0xd),
                new Tuple<string, int, int>("spawnModel2D1",                                                0xd8, 0xd),
                new Tuple<string, int, int>("physPresetD1",                                                 0xe8, 0x14),
                new Tuple<string, int, int>("showBoneD2",                                                   0xf0, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone0D2",                                          0xf4, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone1D2",                                          0xf8, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone2D2",                                          0xfc, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone3D2",                                          0x100, 0x15),
                new Tuple<string, int, int>("breakHealthD2",                                                0x104, 0x8),
                new Tuple<string, int, int>("maxTimeD2",                                                    0x108, 0x8),
                new Tuple<string, int, int>("explosiveDamageOnlyD2",                                        0x10c, 0x35),
                new Tuple<string, int, int>("meleeDamageD2",                                                0x10c, 0x3c),
                new Tuple<string, int, int>("parentDamageD2",                                               0x10c, 0x43),
                new Tuple<string, int, int>("physicsParentD2",                                              0x10c, 0x4a),
                new Tuple<string, int, int>("breakOnEntityDeathD2",                                         0x10c, 0x51),
                new Tuple<string, int, int>("includeChildrenD2",                                            0x10c, 0x58),
                new Tuple<string, int, int>("breakAnimD2",                                                  0x11c, 0x15),
                new Tuple<string, int, int>("breakEffectD2",                                                0x120, 0xa),
                new Tuple<string, int, int>("breakEffectTagD2",                                             0x128, 0x15),
                new Tuple<string, int, int>("breakSoundD2",                                                 0x130, 0x0),
                new Tuple<string, int, int>("loopSoundD2",                                                  0x140, 0x0),
                new Tuple<string, int, int>("breakNotifyD2",                                                0x138, 0x0),
                new Tuple<string, int, int>("spawnModel1D2",                                                0x148, 0xd),
                new Tuple<string, int, int>("spawnModel2D2",                                                0x150, 0xd),
                new Tuple<string, int, int>("physPresetD2",                                                 0x160, 0x14),
                new Tuple<string, int, int>("showBoneD3",                                                   0x168, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone0D3",                                          0x16c, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone1D3",                                          0x170, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone2D3",                                          0x174, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone3D3",                                          0x178, 0x15),
                new Tuple<string, int, int>("breakHealthD3",                                                0x17c, 0x8),
                new Tuple<string, int, int>("maxTimeD3",                                                    0x180, 0x8),
                new Tuple<string, int, int>("explosiveDamageOnlyD3",                                        0x184, 0x36),
                new Tuple<string, int, int>("meleeDamageD3",                                                0x184, 0x3d),
                new Tuple<string, int, int>("parentDamageD3",                                               0x184, 0x44),
                new Tuple<string, int, int>("physicsParentD3",                                              0x184, 0x4b),
                new Tuple<string, int, int>("breakOnEntityDeathD3",                                         0x184, 0x52),
                new Tuple<string, int, int>("includeChildrenD3",                                            0x184, 0x59),
                new Tuple<string, int, int>("breakAnimD3",                                                  0x194, 0x15),
                new Tuple<string, int, int>("breakEffectD3",                                                0x198, 0xa),
                new Tuple<string, int, int>("breakEffectTagD3",                                             0x1a0, 0x15),
                new Tuple<string, int, int>("breakSoundD3",                                                 0x1a8, 0x0),
                new Tuple<string, int, int>("loopSoundD3",                                                  0x1b8, 0x0),
                new Tuple<string, int, int>("breakNotifyD3",                                                0x1b0, 0x0),
                new Tuple<string, int, int>("spawnModel1D3",                                                0x1c0, 0xd),
                new Tuple<string, int, int>("spawnModel2D3",                                                0x1c8, 0xd),
                new Tuple<string, int, int>("physPresetD3",                                                 0x1d8, 0x14),
                new Tuple<string, int, int>("showBoneD4",                                                   0x1e0, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone0D4",                                          0x1e4, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone1D4",                                          0x1e8, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone2D4",                                          0x1ec, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone3D4",                                          0x1f0, 0x15),
                new Tuple<string, int, int>("breakHealthD4",                                                0x1f4, 0x8),
                new Tuple<string, int, int>("maxTimeD4",                                                    0x1f8, 0x8),
                new Tuple<string, int, int>("explosiveDamageOnlyD4",                                        0x1fc, 0x37),
                new Tuple<string, int, int>("meleeDamageD4",                                                0x1fc, 0x3e),
                new Tuple<string, int, int>("parentDamageD4",                                               0x1fc, 0x45),
                new Tuple<string, int, int>("physicsParentD4",                                              0x1fc, 0x4c),
                new Tuple<string, int, int>("breakOnEntityDeathD4",                                         0x1fc, 0x53),
                new Tuple<string, int, int>("includeChildrenD4",                                            0x1fc, 0x5a),
                new Tuple<string, int, int>("breakAnimD4",                                                  0x20c, 0x15),
                new Tuple<string, int, int>("breakEffectD4",                                                0x210, 0xa),
                new Tuple<string, int, int>("breakEffectTagD4",                                             0x218, 0x15),
                new Tuple<string, int, int>("breakSoundD4",                                                 0x220, 0x0),
                new Tuple<string, int, int>("loopSoundD4",                                                  0x230, 0x0),
                new Tuple<string, int, int>("breakNotifyD4",                                                0x228, 0x0),
                new Tuple<string, int, int>("spawnModel1D4",                                                0x238, 0xd),
                new Tuple<string, int, int>("spawnModel2D4",                                                0x240, 0xd),
                new Tuple<string, int, int>("physPresetD4",                                                 0x250, 0x14),
                new Tuple<string, int, int>("showBoneD5",                                                   0x258, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone0D5",                                          0x25c, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone1D5",                                          0x260, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone2D5",                                          0x264, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone3D5",                                          0x268, 0x15),
                new Tuple<string, int, int>("breakHealthD5",                                                0x26c, 0x8),
                new Tuple<string, int, int>("maxTimeD5",                                                    0x270, 0x8),
                new Tuple<string, int, int>("explosiveDamageOnlyD5",                                        0x274, 0x38),
                new Tuple<string, int, int>("meleeDamageD5",                                                0x274, 0x3f),
                new Tuple<string, int, int>("parentDamageD5",                                               0x274, 0x46),
                new Tuple<string, int, int>("physicsParentD5",                                              0x274, 0x4d),
                new Tuple<string, int, int>("breakOnEntityDeathD5",                                         0x274, 0x54),
                new Tuple<string, int, int>("includeChildrenD5",                                            0x274, 0x5b),
                new Tuple<string, int, int>("breakAnimD5",                                                  0x284, 0x15),
                new Tuple<string, int, int>("breakEffectD5",                                                0x288, 0xa),
                new Tuple<string, int, int>("breakEffectTagD5",                                             0x290, 0x15),
                new Tuple<string, int, int>("breakSoundD5",                                                 0x298, 0x0),
                new Tuple<string, int, int>("loopSoundD5",                                                  0x2a8, 0x0),
                new Tuple<string, int, int>("breakNotifyD5",                                                0x2a0, 0x0),
                new Tuple<string, int, int>("spawnModel1D5",                                                0x2b0, 0xd),
                new Tuple<string, int, int>("spawnModel2D5",                                                0x2b8, 0xd),
                new Tuple<string, int, int>("physPresetD5",                                                 0x2c8, 0x14),
                new Tuple<string, int, int>("showBoneD6",                                                   0x2d0, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone0D6",                                          0x2d4, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone1D6",                                          0x2d8, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone2D6",                                          0x2dc, 0x15),
                new Tuple<string, int, int>("cosmeticShowBone3D6",                                          0x2e0, 0x15),
                new Tuple<string, int, int>("breakHealthD6",                                                0x2e4, 0x8),
                new Tuple<string, int, int>("maxTimeD6",                                                    0x2e8, 0x8),
                new Tuple<string, int, int>("explosiveDamageOnlyD6",                                        0x2ec, 0x39),
                new Tuple<string, int, int>("meleeDamageD6",                                                0x2ec, 0x40),
                new Tuple<string, int, int>("parentDamageD6",                                               0x2ec, 0x47),
                new Tuple<string, int, int>("physicsParentD6",                                              0x2ec, 0x4e),
                new Tuple<string, int, int>("breakOnEntityDeathD6",                                         0x2ec, 0x55),
                new Tuple<string, int, int>("includeChildrenD6",                                            0x2ec, 0x5c),
                new Tuple<string, int, int>("breakAnimD6",                                                  0x2fc, 0x15),
                new Tuple<string, int, int>("breakEffectD6",                                                0x300, 0xa),
                new Tuple<string, int, int>("breakEffectTagD6",                                             0x308, 0x15),
                new Tuple<string, int, int>("breakSoundD6",                                                 0x310, 0x0),
                new Tuple<string, int, int>("loopSoundD6",                                                  0x320, 0x0),
                new Tuple<string, int, int>("breakNotifyD6",                                                0x318, 0x0),
                new Tuple<string, int, int>("spawnModel1D6",                                                0x328, 0xd),
                new Tuple<string, int, int>("spawnModel2D6",                                                0x330, 0xd),
                new Tuple<string, int, int>("physPresetD6",                                                 0x340, 0x14),
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
            public string Name => "destructibledef";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.destructibledef;

            /// <summary>
            /// Loads Assets from this Asset Pool
            /// </summary>
            public List<Asset> Load(HydraInstance instance)
            {
                var results = new List<Asset>();

                // Not complete
                return results;

                var poolInfo = instance.Reader.ReadStruct<AssetPoolInfo>(instance.Game.AssetPoolsAddress + (Index * 0x20));

                StartAddress = poolInfo.PoolPointer;
                AssetSize = poolInfo.AssetSize;
                AssetCount = poolInfo.PoolSize;

                for (int i = 0; i < AssetCount; i++)
                {
                    var address = StartAddress + (i * AssetSize);
                    var namePointer = instance.Reader.ReadInt64(address);

                    if (IsNullAsset(namePointer))
                        continue;

                    results.Add(new Asset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(namePointer),
                        Type = Name,
                        Zone = ((BlackOps3)instance.Game).ZoneNames[address],
                        Information = "N/A",
                        Status = "Loaded",
                        Data = address,
                        LoadMethod = ExportAsset,
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                //var header = instance.Reader.ReadStruct<DestructibleDefAsset>((long)asset.Data);

                //if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                //    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                //var result = new GameDataTable.Asset(asset.Name, "destructibledef");

                //for(int i = 0; i < header.PieceCount; i++)
                //{
                //    var pieceBuffer = instance.Reader.ReadBytes(header.PiecesPointer + (i * 932), 932);

                //    var pieceGdtAsset = ConvertAssetBufferToGDTAsset(pieceBuffer, DestructiblePieceOffsets, instance, HandleDestructibleDefSettings);

                //    pieceGdtAsset.Name = string.Format("{0}_piece{1}", asset.Name, i);
                //    pieceGdtAsset.Type = "destructiblepiece";


                //    result[string.Format("piece{0}", i)] = pieceGdtAsset.Name;

                //    // Add to GDT
                //    instance.GDTs["Misc"][pieceGdtAsset.Name] = pieceGdtAsset;
                //}

                //// Add to GDT
                //instance.GDTs["Misc"][asset.Name] = result;

                //return;
            }

            /// <summary>
            /// Handles DestructibleDef Specific Settings
            /// </summary>
            private static object HandleDestructibleDefSettings(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch (type)
                {
                    default:
                        return null;
                }
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
