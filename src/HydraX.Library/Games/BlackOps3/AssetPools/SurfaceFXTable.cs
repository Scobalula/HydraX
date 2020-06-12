using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 SurfaceFXTable Logic
        /// </summary>
        private class SurfaceFXTable : IAssetPool
        {
            #region Tables
            /// <summary>
            /// SurfaceFXTable Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] SurfaceFXTableOffsets =
            {
                new Tuple<string, int, int>("Surface0",                                                      0x8, 0xa),
                new Tuple<string, int, int>("Surface1",                                                      0x10, 0xa),
                new Tuple<string, int, int>("Surface2",                                                      0x18, 0xa),
                new Tuple<string, int, int>("Surface3",                                                      0x20, 0xa),
                new Tuple<string, int, int>("Surface4",                                                      0x28, 0xa),
                new Tuple<string, int, int>("Surface5",                                                      0x30, 0xa),
                new Tuple<string, int, int>("Surface6",                                                      0x38, 0xa),
                new Tuple<string, int, int>("Surface7",                                                      0x40, 0xa),
                new Tuple<string, int, int>("Surface8",                                                      0x48, 0xa),
                new Tuple<string, int, int>("Surface9",                                                      0x50, 0xa),
                new Tuple<string, int, int>("Surface10",                                                     0x58, 0xa),
                new Tuple<string, int, int>("Surface11",                                                     0x60, 0xa),
                new Tuple<string, int, int>("Surface12",                                                     0x68, 0xa),
                new Tuple<string, int, int>("Surface13",                                                     0x70, 0xa),
                new Tuple<string, int, int>("Surface14",                                                     0x78, 0xa),
                new Tuple<string, int, int>("Surface15",                                                     0x80, 0xa),
                new Tuple<string, int, int>("Surface16",                                                     0x88, 0xa),
                new Tuple<string, int, int>("Surface17",                                                     0x90, 0xa),
                new Tuple<string, int, int>("Surface18",                                                     0x98, 0xa),
                new Tuple<string, int, int>("Surface19",                                                     0xa0, 0xa),
                new Tuple<string, int, int>("Surface20",                                                     0xa8, 0xa),
                new Tuple<string, int, int>("Surface21",                                                     0xb0, 0xa),
                new Tuple<string, int, int>("Surface22",                                                     0xb8, 0xa),
                new Tuple<string, int, int>("Surface23",                                                     0xc0, 0xa),
                new Tuple<string, int, int>("Surface24",                                                     0xc8, 0xa),
                new Tuple<string, int, int>("Surface25",                                                     0xd0, 0xa),
                new Tuple<string, int, int>("Surface26",                                                     0xd8, 0xa),
                new Tuple<string, int, int>("Surface27",                                                     0xe0, 0xa),
                new Tuple<string, int, int>("Surface28",                                                     0xe8, 0xa),
                new Tuple<string, int, int>("Surface29",                                                     0xf0, 0xa),
                new Tuple<string, int, int>("Surface30",                                                     0xf8, 0xa),
                new Tuple<string, int, int>("Surface31",                                                     0x100, 0xa),
                new Tuple<string, int, int>("Surface32",                                                     0x108, 0xa),
                new Tuple<string, int, int>("Surface33",                                                     0x110, 0xa),
                new Tuple<string, int, int>("Surface34",                                                     0x118, 0xa),
                new Tuple<string, int, int>("Surface35",                                                     0x120, 0xa),
                new Tuple<string, int, int>("Surface36",                                                     0x128, 0xa),
                new Tuple<string, int, int>("Surface37",                                                     0x130, 0xa),
                new Tuple<string, int, int>("Surface38",                                                     0x138, 0xa),
                new Tuple<string, int, int>("Surface39",                                                     0x140, 0xa),
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
            public string Name => "surfacefxtable";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.surfacefxtable;

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

                var result = ConvertAssetBufferToGDTAsset(buffer, SurfaceFXTableOffsets, instance);

                result.Type = "surfacefxtable";
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
