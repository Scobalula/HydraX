using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Animation Mapping Table Logic
        /// </summary>
        private class AnimationMappingTable : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// Animation Mapping Table Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct AnimationMappingTableAsset
            {
                public long NamePointer;
                public long MapsPointer;
                public int MapCount;
            }

            /// <summary>
            /// Animation Map Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct AnimationMap
            {
                public int NameStringIndex;
                public long AnimationStringIndicesPointer;
                public int AnimationCount;
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
            public string Name => "animmappingtable";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "AI";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.animmappingtable;

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
                    var header = instance.Reader.ReadStruct<AnimationMappingTableAsset>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    results.Add(new Asset()
                    {
                        Name        = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                        Type        = Name,
                        Status      = "Loaded",
                        Data        = address,
                        LoadMethod  = ExportAsset,
                        Zone = ((BlackOps3)instance.Game).ZoneNames[address],
                        Information = string.Format("Maps: {0}", header.MapCount)
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<AnimationMappingTableAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                string path = Path.Combine(instance.AnimationTableFolder, asset.Name);
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                var maps = instance.Reader.ReadArray<AnimationMap>(header.MapsPointer, header.MapCount);

                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine("#");

                    for (int i = 0; i < header.MapCount; i++)
                    {
                        writer.Write("{0},", instance.Game.GetString(maps[i].NameStringIndex, instance));

                        int[] indicesBuffer = instance.Reader.ReadArray<int>(maps[i].AnimationStringIndicesPointer, maps[i].AnimationCount);

                        for (int j = 0; j < maps[i].AnimationCount; j++)
                            writer.Write("{0},", instance.Game.GetString(indicesBuffer[j], instance));

                        writer.WriteLine();
                    }
                }

                // Done
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
