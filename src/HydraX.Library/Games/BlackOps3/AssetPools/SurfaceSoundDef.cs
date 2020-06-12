using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 SurfaceSoundDef Logic
        /// </summary>
        private class SurfaceSoundDef : IAssetPool
        {
            #region Tables
            /// <summary>
            /// SurfaceSoundDef Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] SurfaceSoundDefOffsets =
            {
                new Tuple<string, int, int>("defaultsurface",                                                0x8, 0x18),
                new Tuple<string, int, int>("bark",                                                          0xc, 0x18),
                new Tuple<string, int, int>("brick",                                                         0x10, 0x18),
                new Tuple<string, int, int>("carpet",                                                        0x14, 0x18),
                new Tuple<string, int, int>("cloth",                                                         0x18, 0x18),
                new Tuple<string, int, int>("concrete",                                                      0x1c, 0x18),
                new Tuple<string, int, int>("dirt",                                                          0x20, 0x18),
                new Tuple<string, int, int>("flesh",                                                         0x24, 0x18),
                new Tuple<string, int, int>("foliage",                                                       0x28, 0x18),
                new Tuple<string, int, int>("glass",                                                         0x2c, 0x18),
                new Tuple<string, int, int>("grass",                                                         0x30, 0x18),
                new Tuple<string, int, int>("gravel",                                                        0x34, 0x18),
                new Tuple<string, int, int>("ice",                                                           0x38, 0x18),
                new Tuple<string, int, int>("metal",                                                         0x3c, 0x18),
                new Tuple<string, int, int>("mud",                                                           0x40, 0x18),
                new Tuple<string, int, int>("paper",                                                         0x44, 0x18),
                new Tuple<string, int, int>("plaster",                                                       0x48, 0x18),
                new Tuple<string, int, int>("rock",                                                          0x4c, 0x18),
                new Tuple<string, int, int>("sand",                                                          0x50, 0x18),
                new Tuple<string, int, int>("snow",                                                          0x54, 0x18),
                new Tuple<string, int, int>("water",                                                         0x58, 0x18),
                new Tuple<string, int, int>("wood",                                                          0x5c, 0x18),
                new Tuple<string, int, int>("asphalt",                                                       0x60, 0x18),
                new Tuple<string, int, int>("ceramic",                                                       0x64, 0x18),
                new Tuple<string, int, int>("plastic",                                                       0x68, 0x18),
                new Tuple<string, int, int>("rubber",                                                        0x6c, 0x18),
                new Tuple<string, int, int>("cushion",                                                       0x70, 0x18),
                new Tuple<string, int, int>("fruit",                                                         0x74, 0x18),
                new Tuple<string, int, int>("paintedmetal",                                                  0x78, 0x18),
                new Tuple<string, int, int>("player",                                                        0x7c, 0x18),
                new Tuple<string, int, int>("tallgrass",                                                     0x80, 0x18),
                new Tuple<string, int, int>("riotshield",                                                    0x84, 0x18),
                new Tuple<string, int, int>("metalthin",                                                     0x88, 0x18),
                new Tuple<string, int, int>("metalhollow",                                                   0x8c, 0x18),
                new Tuple<string, int, int>("metalcatwalk",                                                  0x90, 0x18),
                new Tuple<string, int, int>("metalcar",                                                      0x94, 0x18),
                new Tuple<string, int, int>("glasscar",                                                      0x98, 0x18),
                new Tuple<string, int, int>("glassbulletproof",                                              0x9c, 0x18),
                new Tuple<string, int, int>("watershallow",                                                  0xa0, 0x18),
                new Tuple<string, int, int>("bodyarmor",                                                     0xa4, 0x18),
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
            public string Name => "surfacesounddef";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.surfacesounddef;

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

                var result = ConvertAssetBufferToGDTAsset(buffer, SurfaceSoundDefOffsets, instance);

                result.Type = "surfacesounddef";
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
