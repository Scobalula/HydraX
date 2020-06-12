using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 XModelAlias Logic
        /// </summary>
        private class XModelAlias : IAssetPool
        {
            #region Tables
            /// <summary>
            /// XModelAlias Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] XModelAliasTableOffsets =
            {
                new Tuple<string, int, int>("model1",                                                        0x8, 0xd),
                new Tuple<string, int, int>("model2",                                                        0x10, 0xd),
                new Tuple<string, int, int>("model3",                                                        0x18, 0xd),
                new Tuple<string, int, int>("model4",                                                        0x20, 0xd),
                new Tuple<string, int, int>("model5",                                                        0x28, 0xd),
                new Tuple<string, int, int>("model6",                                                        0x30, 0xd),
                new Tuple<string, int, int>("model7",                                                        0x38, 0xd),
                new Tuple<string, int, int>("model8",                                                        0x40, 0xd),
                new Tuple<string, int, int>("model9",                                                        0x48, 0xd),
                new Tuple<string, int, int>("model10",                                                       0x50, 0xd),
                new Tuple<string, int, int>("model11",                                                       0x58, 0xd),
                new Tuple<string, int, int>("model12",                                                       0x60, 0xd),
                new Tuple<string, int, int>("model13",                                                       0x68, 0xd),
                new Tuple<string, int, int>("model14",                                                       0x70, 0xd),
                new Tuple<string, int, int>("model15",                                                       0x78, 0xd),
                new Tuple<string, int, int>("model16",                                                       0x80, 0xd),
                new Tuple<string, int, int>("model17",                                                       0x88, 0xd),
                new Tuple<string, int, int>("model18",                                                       0x90, 0xd),
                new Tuple<string, int, int>("model19",                                                       0x98, 0xd),
                new Tuple<string, int, int>("model20",                                                       0xa0, 0xd),
                new Tuple<string, int, int>("model21",                                                       0xa8, 0xd),
                new Tuple<string, int, int>("model22",                                                       0xb0, 0xd),
                new Tuple<string, int, int>("model23",                                                       0xb8, 0xd),
                new Tuple<string, int, int>("model24",                                                       0xc0, 0xd),
                new Tuple<string, int, int>("model25",                                                       0xc8, 0xd),
                new Tuple<string, int, int>("model26",                                                       0xd0, 0xd),
                new Tuple<string, int, int>("model27",                                                       0xd8, 0xd),
                new Tuple<string, int, int>("model28",                                                       0xe0, 0xd),
                new Tuple<string, int, int>("model29",                                                       0xe8, 0xd),
                new Tuple<string, int, int>("model30",                                                       0xf0, 0xd),
                new Tuple<string, int, int>("model31",                                                       0xf8, 0xd),
                new Tuple<string, int, int>("model32",                                                       0x100, 0xd),
                new Tuple<string, int, int>("model33",                                                       0x108, 0xd),
                new Tuple<string, int, int>("model34",                                                       0x110, 0xd),
                new Tuple<string, int, int>("model35",                                                       0x118, 0xd),
                new Tuple<string, int, int>("model36",                                                       0x120, 0xd),
                new Tuple<string, int, int>("model37",                                                       0x128, 0xd),
                new Tuple<string, int, int>("model38",                                                       0x130, 0xd),
                new Tuple<string, int, int>("model39",                                                       0x138, 0xd),
                new Tuple<string, int, int>("model40",                                                       0x140, 0xd),
                new Tuple<string, int, int>("model41",                                                       0x148, 0xd),
                new Tuple<string, int, int>("model42",                                                       0x150, 0xd),
                new Tuple<string, int, int>("model43",                                                       0x158, 0xd),
                new Tuple<string, int, int>("model44",                                                       0x160, 0xd),
                new Tuple<string, int, int>("model45",                                                       0x168, 0xd),
                new Tuple<string, int, int>("model46",                                                       0x170, 0xd),
                new Tuple<string, int, int>("model47",                                                       0x178, 0xd),
                new Tuple<string, int, int>("model48",                                                       0x180, 0xd),
                new Tuple<string, int, int>("model49",                                                       0x188, 0xd),
                new Tuple<string, int, int>("model50",                                                       0x190, 0xd),
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
            public string Name => "xmodelalias";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.xmodelalias;

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

                var result = ConvertAssetBufferToGDTAsset(buffer, XModelAliasTableOffsets, instance);

                result.Type = "xmodelalias";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);

                return;
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
