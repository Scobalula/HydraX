using PhilLibX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Games
{
    public partial class BlackOps3
    {
        public class StringTable
        {
            /// <summary>
            /// Bo3 StringTable Header
            /// </summary>
            public struct StringTableHeader
            {
                /// <summary>
                /// Pointer to the name of this raw file
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Number of Columns
                /// </summary>
                public int ColumnCount { get; set; }

                /// <summary>
                /// Number of Rows
                /// </summary>
                public int RowCount { get; set; }

                /// <summary>
                /// Pointer to the data
                /// </summary>
                public long DataPointer { get; set; }

                /// <summary>
                /// Pointer to the end of this string table
                /// </summary>
                public long EndPointer { get; set; }
            }

            /// <summary>
            /// Bo3 String Table Cell
            /// </summary>
            public struct StringTableCell
            {
                /// <summary>
                /// Pointer to the string in this cell
                /// </summary>
                public long StringPointer { get; set; }

                /// <summary>
                /// DJB Hash of the string in this cell
                /// </summary>
                public long DJBHash { get; set; }
            }

            public static void Load(AssetPool assetPool)
            {
                for (int i = 0; i < assetPool.Size; i++)
                {
                    // Read Rawfile
                    var stringTable = Hydra.ActiveGameReader.ReadStruct<StringTableHeader>(assetPool.FirstEntry + (i * assetPool.HeaderSize));
                    // Check is it a null/empty slot
                    if (assetPool.IsNullAsset(stringTable.NamePointer))
                        continue;
                    // Create new asset
                    Asset asset = new Asset()
                    {
                        Name = Hydra.ActiveGameReader.ReadNullTerminatedString(stringTable.NamePointer),
                        HeaderAddress = assetPool.FirstEntry + (i * assetPool.HeaderSize),
                        ExportFunction = Export,
                        Type = assetPool.Name,
                        Information = String.Format("Rows: {0} - Columns: {1}", stringTable.RowCount, stringTable.ColumnCount)
                    };
                    // Check name
                    if (asset.Name == "*")
                        continue;
                    // Set 
                    asset.ExportFunction = Export;
                    // Add Asset
                    Hydra.LoadedAssets.Add(asset);
                }
            }

            public static bool Export(Asset asset)
            {
                // Read Header
                var stringTable = Hydra.ActiveGameReader.ReadStruct<StringTableHeader>(asset.HeaderAddress);
                // Check name pointer, if it's changed, our asset has changed
                if (asset.Name != Hydra.ActiveGameReader.ReadNullTerminatedString(stringTable.NamePointer))
                    return false;
                // Path Result
                string path = Path.Combine("exported_files", Hydra.ActiveGameName, asset.Name);
                // Create path
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                // Read read table (raw size will = (RowCount * ColumnCount) * sizeof(StringTableCell) (which is 16)
                byte[] buffer = Hydra.ActiveGameReader.ReadBytes(stringTable.DataPointer, (stringTable.RowCount * stringTable.ColumnCount) * 16);
                // Output result
                var result = new StringBuilder();
                // Loop through rows
                for(int x = 0; x < stringTable.RowCount; x++)
                {
                    // Loop through columns for this row
                    for (int y = 0; y < stringTable.ColumnCount; y++)
                        // Add cell
                        result.Append(Hydra.ActiveGameReader.ReadNullTerminatedString(ByteUtil.BytesToStruct<StringTableCell>(buffer, ((x * stringTable.ColumnCount) + y) * 16).StringPointer) + ",");
                    // Create new line
                    result.AppendLine();
                }
                // Write result
                File.WriteAllText(path, result.ToString());
                // Done
                return true;
            }
        }
    }
}
