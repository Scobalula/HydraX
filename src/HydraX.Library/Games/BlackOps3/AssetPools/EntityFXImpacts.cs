using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 EntityFXImpacts Logic
        /// </summary>
        private class EntityFXImpacts : IAssetPool
        {
            #region Tables
            /// <summary>
            /// EntityFXImpacts Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] EntityFXImpactsOffsets =
            {
                new Tuple<string, int, int>("fleshBody",                                                     0x38, 0xa),
                new Tuple<string, int, int>("fleshBodyFatal",                                                0x40, 0xa),
                new Tuple<string, int, int>("fleshHead",                                                     0x48, 0xa),
                new Tuple<string, int, int>("fleshHeadFatal",                                                0x50, 0xa),
                new Tuple<string, int, int>("fleshLimb",                                                     0x58, 0xa),
                new Tuple<string, int, int>("fleshLimbFatal",                                                0x60, 0xa),
                new Tuple<string, int, int>("zombieBody",                                                    0x68, 0xa),
                new Tuple<string, int, int>("zombieBodyFatal",                                               0x70, 0xa),
                new Tuple<string, int, int>("zombieHead",                                                    0x78, 0xa),
                new Tuple<string, int, int>("zombieHeadFatal",                                               0x80, 0xa),
                new Tuple<string, int, int>("zombieLimb",                                                    0x88, 0xa),
                new Tuple<string, int, int>("zombieLimbFatal",                                               0x90, 0xa),
                new Tuple<string, int, int>("armorLightBody",                                                0x98, 0xa),
                new Tuple<string, int, int>("armorLightBodyFatal",                                           0xa0, 0xa),
                new Tuple<string, int, int>("armorLightHead",                                                0xa8, 0xa),
                new Tuple<string, int, int>("armorLightHeadFatal",                                           0xb0, 0xa),
                new Tuple<string, int, int>("armorLightLimb",                                                0xb8, 0xa),
                new Tuple<string, int, int>("armorLightLimbFatal",                                           0xc0, 0xa),
                new Tuple<string, int, int>("armorHeavyBody",                                                0xc8, 0xa),
                new Tuple<string, int, int>("armorHeavyBodyFatal",                                           0xd0, 0xa),
                new Tuple<string, int, int>("armorHeavyHead",                                                0xd8, 0xa),
                new Tuple<string, int, int>("armorHeavyHeadFatal",                                           0xe0, 0xa),
                new Tuple<string, int, int>("armorHeavyLimb",                                                0xe8, 0xa),
                new Tuple<string, int, int>("armorHeavyLimbFatal",                                           0xf0, 0xa),
                new Tuple<string, int, int>("robotLightBody",                                                0xf8, 0xa),
                new Tuple<string, int, int>("robotLightBodyFatal",                                           0x100, 0xa),
                new Tuple<string, int, int>("robotLightHead",                                                0x108, 0xa),
                new Tuple<string, int, int>("robotLightHeadFatal",                                           0x110, 0xa),
                new Tuple<string, int, int>("robotLightLimb",                                                0x118, 0xa),
                new Tuple<string, int, int>("robotLightLimbFatal",                                           0x120, 0xa),
                new Tuple<string, int, int>("robotHeavyBody",                                                0x128, 0xa),
                new Tuple<string, int, int>("robotHeavyBodyFatal",                                           0x130, 0xa),
                new Tuple<string, int, int>("robotHeavyHead",                                                0x138, 0xa),
                new Tuple<string, int, int>("robotHeavyHeadFatal",                                           0x140, 0xa),
                new Tuple<string, int, int>("robotHeavyLimb",                                                0x148, 0xa),
                new Tuple<string, int, int>("robotHeavyLimbFatal",                                           0x150, 0xa),
                new Tuple<string, int, int>("robotBossBody",                                                 0x158, 0xa),
                new Tuple<string, int, int>("robotBossBodyFatal",                                            0x160, 0xa),
                new Tuple<string, int, int>("robotBossHead",                                                 0x168, 0xa),
                new Tuple<string, int, int>("robotBossHeadFatal",                                            0x170, 0xa),
                new Tuple<string, int, int>("robotBossLimb",                                                 0x178, 0xa),
                new Tuple<string, int, int>("robotBossLimbFatal",                                            0x180, 0xa),
                new Tuple<string, int, int>("powerArmorBody",                                                0x188, 0xa),
                new Tuple<string, int, int>("powerArmorBodyFatal",                                           0x190, 0xa),
                new Tuple<string, int, int>("powerArmorHead",                                                0x198, 0xa),
                new Tuple<string, int, int>("powerArmorHeadFatal",                                           0x1a0, 0xa),
                new Tuple<string, int, int>("powerArmorLimb",                                                0x1a8, 0xa),
                new Tuple<string, int, int>("powerArmorLimbFatal",                                           0x1b0, 0xa),
                new Tuple<string, int, int>("skeletonBody",                                                  0x1b8, 0xa),
                new Tuple<string, int, int>("skeletonBodyFatal",                                             0x1c0, 0xa),
                new Tuple<string, int, int>("skeletonHead",                                                  0x1c8, 0xa),
                new Tuple<string, int, int>("skeletonHeadFatal",                                             0x1d0, 0xa),
                new Tuple<string, int, int>("skeletonLimb",                                                  0x1d8, 0xa),
                new Tuple<string, int, int>("skeletonLimbFatal",                                             0x1e0, 0xa),
                new Tuple<string, int, int>("fleshCorpseBody",                                               0x1e8, 0xa),
                new Tuple<string, int, int>("fleshCorpseBodyFatal",                                          0x1f0, 0xa),
                new Tuple<string, int, int>("fleshCorpseHead",                                               0x1f8, 0xa),
                new Tuple<string, int, int>("fleshCorpseHeadFatal",                                          0x200, 0xa),
                new Tuple<string, int, int>("fleshCorpseLimb",                                               0x208, 0xa),
                new Tuple<string, int, int>("fleshCorpseLimbFatal",                                          0x210, 0xa),
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
            public string Name => "entityfximpacts";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.entityfximpacts;

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

                var result = ConvertAssetBufferToGDTAsset(buffer, EntityFXImpactsOffsets, instance);

                result.Type = "entityfximpacts";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);

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
