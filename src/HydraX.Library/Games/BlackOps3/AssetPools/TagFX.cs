using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HydraX.Library.CommonStructures;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 TagFX Logic
        /// </summary>
        private class TagFX : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// TagFX Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct TagFXAsset
            {
                #region TagFXProperties
                public long NamePointer;
                public int ItemCount;
                public long ItemsPointer;
                #endregion
            }

            /// <summary>
            /// TagFX Item Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct TagFXItem
            {
                #region TagFXItemProperties
                public long FXPointer;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 88)]
                public byte[] Unknown;
                public int TagNameIndex;
                public int Delay;
                public int Bolted;
                #endregion
            }
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
            public string Name => "tagfx";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.tagfx;

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
                    var header = instance.Reader.ReadStruct<TagFXAsset>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    results.Add(new GameAsset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                        HeaderAddress = StartAddress + (i * AssetSize),
                        AssetPool = this,
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
                var header = instance.Reader.ReadStruct<TagFXAsset>(asset.HeaderAddress);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    return HydraStatus.MemoryChanged;

                var tagFXAsset = new GameDataTable.Asset(asset.Name, "tagfx");

                tagFXAsset["tagFXItemCount"] = header.ItemCount;

                var items = instance.Reader.ReadArray<TagFXItem>(header.ItemsPointer, header.ItemCount);

                for(int i = 0; i < items.Length; i++)
                {
                    tagFXAsset["bolted" + (i + 1).ToString()]    = items[i].Bolted;
                    tagFXAsset["fx" + (i + 1).ToString()]        = instance.Game.CleanAssetName(HydraAssetType.FX, instance.Game.GetAssetName(items[i].FXPointer, instance));
                    tagFXAsset["timeDelay" + (i + 1).ToString()] = items[i].Delay;
                    tagFXAsset["tag" + (i + 1).ToString()]       = instance.Game.GetString(items[i].TagNameIndex, instance);
                }

                instance.GDTs["Misc"][asset.Name] = tagFXAsset;

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
