using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PhilLibX;

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
            public List<GameAsset> Load(HydraInstance instance)
            {
                var results = new List<GameAsset>();

                var poolInfo = instance.Reader.ReadStruct<AssetPoolInfo>(instance.Game.BaseAddress + instance.Game.AssetPoolsAddresses[instance.Game.ProcessIndex] + (Index * 0x20));

                StartAddress = poolInfo.PoolPointer;
                AssetSize = poolInfo.AssetSize;
                AssetCount = poolInfo.PoolSize;

                for(int i = 0; i < AssetCount; i++)
                {
                    var address = StartAddress + (i * AssetSize);
                    var namePointer = instance.Reader.ReadInt64(address);

                    if (IsNullAsset(namePointer))
                        continue;

                    results.Add(new GameAsset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(namePointer),
                        NameLocation = namePointer,
                        HeaderAddress = StartAddress + (i * AssetSize),
                        AssetPool = this,
                        Size = AssetSize,
                        Type = Name,
                        Information = "N/A"
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public HydraStatus Export(GameAsset asset, HydraInstance instance)
            {
                var buffer = instance.Reader.ReadBytes(asset.HeaderAddress, asset.Size);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(BitConverter.ToInt64(buffer, 0)))
                    return HydraStatus.MemoryChanged;

                var result = GameDataTable.ConvertStructToGDTAsset(buffer, PlayerFXTableOffsets, instance);

                result.Type = "playerfxtable";

                instance.GDTs["Table"][asset.Name] = result;

                return HydraStatus.Success;
            }

            /// <summary>
            /// Checks if the given asset is a null slot
            /// </summary>
            public bool IsNullAsset(GameAsset asset)
            {
                return IsNullAsset(asset.NameLocation);
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
