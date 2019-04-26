﻿using System;
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
        /// Black Ops 3 String Table Logic
        /// </summary>
        private class StringTable : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// String Table Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct StringTableAsset
            {
                /// <summary>
                /// String Table Cell Structure
                /// </summary>
                [StructLayout(LayoutKind.Sequential, Pack=1)]
                public struct Cell
                {
                    #region StringTableCellProperties
                    public long StringPointer;
                    public int Hash;
                    private readonly int Padding;
                    #endregion
                }

                #region StringTableAssetProperties
                public long NamePointer;
                public int ColumnCount;
                public int RowCount;
                public long CellsPointer;
                public long IndicesPointer;
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
            public string Name => "stringtable";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.stringtable;

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
                    var header = instance.Reader.ReadStruct<StringTableAsset>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    results.Add(new GameAsset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                        HeaderAddress = StartAddress + (i * AssetSize),
                        AssetPool = this,
                        Type = Name,
                        Information = string.Format("Rows: {0} - Columns: {1}", header.RowCount, header.ColumnCount)
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public HydraStatus Export(GameAsset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<StringTableAsset>(asset.HeaderAddress);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    return HydraStatus.MemoryChanged;

                string path = Path.Combine("exported_files", instance.Game.Name, asset.Name);

                // Create path
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                // Read read table (raw size will = (RowCount * ColumnCount) * sizeof(StringTableCell) (which is 16)
                byte[] buffer = instance.Reader.ReadBytes(header.CellsPointer, (header.RowCount * header.ColumnCount) * 16);
                // Output result
                var result = new StringBuilder();
                // Loop through rows
                for (int x = 0; x < header.RowCount; x++)
                {
                    // Loop through columns for this row
                    for (int y = 0; y < header.ColumnCount; y++)
                        // Add cell
                        result.Append(instance.Reader.ReadNullTerminatedString(Bytes.BytesToStruct<StringTableAsset.Cell>(buffer, ((x * header.ColumnCount) + y) * 16).StringPointer) + ",");
                    // Create new line
                    result.AppendLine();
                }
                // Write result
                File.WriteAllText(path, result.ToString());

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
