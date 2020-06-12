using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 EntitySoundImpacts Logic
        /// </summary>
        private class EntitySoundImpacts : IAssetPool
        {
            #region Tables
            /// <summary>
            /// EntitySoundImpacts Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] EntitySoundImpactsOffsets =
            {
                new Tuple<string, int, int>("fleshBody",                                                     0x20, 0x18),
                new Tuple<string, int, int>("fleshBodyFatal",                                                0x24, 0x18),
                new Tuple<string, int, int>("fleshHead",                                                     0x28, 0x18),
                new Tuple<string, int, int>("fleshHeadFatal",                                                0x2c, 0x18),
                new Tuple<string, int, int>("fleshBody",                                                     0x30, 0x18),
                new Tuple<string, int, int>("fleshBodyFatal",                                                0x34, 0x18),
                new Tuple<string, int, int>("zombieBody",                                                    0x38, 0x18),
                new Tuple<string, int, int>("zombieBodyFatal",                                               0x3c, 0x18),
                new Tuple<string, int, int>("zombieHead",                                                    0x40, 0x18),
                new Tuple<string, int, int>("zombieHeadFatal",                                               0x44, 0x18),
                new Tuple<string, int, int>("zombieBody",                                                    0x48, 0x18),
                new Tuple<string, int, int>("zombieBodyFatal",                                               0x4c, 0x18),
                new Tuple<string, int, int>("armorLightBody",                                                0x50, 0x18),
                new Tuple<string, int, int>("armorLightBodyFatal",                                           0x54, 0x18),
                new Tuple<string, int, int>("armorLightHead",                                                0x58, 0x18),
                new Tuple<string, int, int>("armorLightHeadFatal",                                           0x5c, 0x18),
                new Tuple<string, int, int>("armorLightBody",                                                0x60, 0x18),
                new Tuple<string, int, int>("armorLightBodyFatal",                                           0x64, 0x18),
                new Tuple<string, int, int>("armorHeavyBody",                                                0x68, 0x18),
                new Tuple<string, int, int>("armorHeavyBodyFatal",                                           0x6c, 0x18),
                new Tuple<string, int, int>("armorHeavyHead",                                                0x70, 0x18),
                new Tuple<string, int, int>("armorHeavyHeadFatal",                                           0x74, 0x18),
                new Tuple<string, int, int>("armorHeavyBody",                                                0x78, 0x18),
                new Tuple<string, int, int>("armorHeavyBodyFatal",                                           0x7c, 0x18),
                new Tuple<string, int, int>("robotLightBody",                                                0x80, 0x18),
                new Tuple<string, int, int>("robotLightBodyFatal",                                           0x84, 0x18),
                new Tuple<string, int, int>("robotLightHead",                                                0x88, 0x18),
                new Tuple<string, int, int>("robotLightHeadFatal",                                           0x8c, 0x18),
                new Tuple<string, int, int>("robotLightBody",                                                0x90, 0x18),
                new Tuple<string, int, int>("robotLightBodyFatal",                                           0x94, 0x18),
                new Tuple<string, int, int>("robotHeavyBody",                                                0x98, 0x18),
                new Tuple<string, int, int>("robotHeavyBodyFatal",                                           0x9c, 0x18),
                new Tuple<string, int, int>("robotHeavyHead",                                                0xa0, 0x18),
                new Tuple<string, int, int>("robotHeavyHeadFatal",                                           0xa4, 0x18),
                new Tuple<string, int, int>("robotHeavyBody",                                                0xa8, 0x18),
                new Tuple<string, int, int>("robotHeavyBodyFatal",                                           0xac, 0x18),
                new Tuple<string, int, int>("robotBossBody",                                                 0xb0, 0x18),
                new Tuple<string, int, int>("robotBossBodyFatal",                                            0xb4, 0x18),
                new Tuple<string, int, int>("robotBossHead",                                                 0xb8, 0x18),
                new Tuple<string, int, int>("robotBossHeadFatal",                                            0xbc, 0x18),
                new Tuple<string, int, int>("robotBossBody",                                                 0xc0, 0x18),
                new Tuple<string, int, int>("robotBossBodyFatal",                                            0xc4, 0x18),
                new Tuple<string, int, int>("powerArmorBody",                                                0xc8, 0x18),
                new Tuple<string, int, int>("powerArmorBodyFatal",                                           0xcc, 0x18),
                new Tuple<string, int, int>("powerArmorHead",                                                0xd0, 0x18),
                new Tuple<string, int, int>("powerArmorHeadFatal",                                           0xd4, 0x18),
                new Tuple<string, int, int>("powerArmorBody",                                                0xd8, 0x18),
                new Tuple<string, int, int>("powerArmorBodyFatal",                                           0xdc, 0x18),
                new Tuple<string, int, int>("skeletonBody",                                                  0xe0, 0x18),
                new Tuple<string, int, int>("skeletonBodyFatal",                                             0xe4, 0x18),
                new Tuple<string, int, int>("skeletonHead",                                                  0xe8, 0x18),
                new Tuple<string, int, int>("skeletonHeadFatal",                                             0xec, 0x18),
                new Tuple<string, int, int>("skeletonBody",                                                  0xf0, 0x18),
                new Tuple<string, int, int>("skeletonBodyFatal",                                             0xf4, 0x18),
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

                var result = ConvertAssetBufferToGDTAsset(buffer, EntitySoundImpactsOffsets, instance);

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
