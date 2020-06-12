using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 PlayerFXTable Logic
        /// </summary>
        private class PlayerFXTable : IAssetPool
        {
            #region Tables
            /// <summary>
            /// PlayerFXTable Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] PlayerFXTableOffsets =
            {
                new Tuple<string, int, int>("wallRunHand",                                                   0x8, 0xa),
                new Tuple<string, int, int>("footstep",                                                      0x10, 0x1d),
                new Tuple<string, int, int>("footstepTracker",                                               0x18, 0x1d),
                new Tuple<string, int, int>("land",                                                          0x20, 0x1d),
                new Tuple<string, int, int>("jumpStart",                                                     0x28, 0x1d),
                new Tuple<string, int, int>("leapStart",                                                     0x30, 0x1d),
                new Tuple<string, int, int>("doubleJumpStart",                                               0x38, 0x1d),
                new Tuple<string, int, int>("slideLeftKnee",                                                 0x40, 0x1d),
                new Tuple<string, int, int>("slideRightKnee",                                                0x48, 0x1d),
                new Tuple<string, int, int>("slideLeftFoot",                                                 0x50, 0x1d),
                new Tuple<string, int, int>("playerJet",                                                     0x58, 0xa),
                new Tuple<string, int, int>("playerJetWeak",                                                 0x60, 0xa),
                new Tuple<string, int, int>("playerJetEMP",                                                  0x68, 0xa),
                new Tuple<string, int, int>("underWaterBreathing",                                           0x70, 0xa),
                new Tuple<string, int, int>("splashSmall",                                                   0x78, 0xa),
                new Tuple<string, int, int>("splashMed",                                                     0x80, 0xa),
                new Tuple<string, int, int>("splashLarge",                                                   0x88, 0xa),
                new Tuple<string, int, int>("splashMassive",                                                 0x90, 0xa),
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
            public string Name => "playerfxtable";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.playerfxtable;

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

                var result = ConvertAssetBufferToGDTAsset(buffer, PlayerFXTableOffsets, instance);

                result.Type = "playerfxtable";
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
