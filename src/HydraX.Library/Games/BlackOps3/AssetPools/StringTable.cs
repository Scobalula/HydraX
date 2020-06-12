using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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
            public string SettingGroup => "MetaData";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.stringtable;

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
                    var header = instance.Reader.ReadStruct<StringTableAsset>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    var address = StartAddress + (i * AssetSize);

                    results.Add(new Asset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                        Type        = Name,
                        Status      = "Loaded",
                        Data        = address,
                        LoadMethod  = ExportAsset,
                        Zone = ((BlackOps3)instance.Game).ZoneNames[address],
                        Information = string.Format("Rows: {0} - Columns: {1}", header.RowCount, header.ColumnCount)
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<StringTableAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

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
