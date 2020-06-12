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
            public List<Asset> Load(HydraInstance instance)
            {
                var results = new List<Asset>();

                var poolInfo = instance.Reader.ReadStruct<AssetPoolInfo>(instance.Game.AssetPoolsAddress + (Index * 0x20));

                StartAddress = poolInfo.PoolPointer;
                AssetSize = poolInfo.AssetSize;
                AssetCount = poolInfo.PoolSize;

                for(int i = 0; i < AssetCount; i++)
                {
                    var header = instance.Reader.ReadStruct<TagFXAsset>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    var address = StartAddress + (i * AssetSize);

                    results.Add(new Asset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                        Type        = Name,
                        Status      = "Loaded",
                        Data        = address,
                        Zone        = ((BlackOps3)instance.Game).ZoneNames[address],
                        LoadMethod  = ExportAsset,
                        Information = "N/A"
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<TagFXAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

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

                instance.AddGDTAsset(tagFXAsset, tagFXAsset.Type, tagFXAsset.Name);

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
