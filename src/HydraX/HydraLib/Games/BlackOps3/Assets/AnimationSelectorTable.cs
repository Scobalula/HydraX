using PhilLibX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HydraLib.Assets;

namespace HydraLib.Games
{
    public partial class BlackOps3
    {
        public class AnimationSelectorTable
        {
            /// <summary>
            /// Bo3 AnimMappingTable Header
            /// </summary>
            public struct AnimationSelectorTableHeader
            {
                /// <summary>
                /// Pointer to the name of this raw file
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Pointer to the raw data
                /// </summary>
                public long DataPointer { get; set; }

                /// <summary>
                /// Number of Entries/Selectors
                /// </summary>
                public int SelectorCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }
            }

            /// <summary>
            /// Bo3 AnimMappingTable Header
            /// </summary>
            public struct AnimationSelectorHeader
            {
                /// <summary>
                /// Index of the Name of this Map in the String Database
                /// </summary>
                public int NameStringIndex { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }

                /// <summary>
                /// Pointer to the columns
                /// </summary>
                public long ColumnsPointer { get; set; }

                /// <summary>
                /// Number of Columns/Header Entries
                /// </summary>
                public int ColumnCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding1 { get; set; }

                /// <summary>
                /// Pointer to the Rows/Entries
                /// </summary>
                public long RowsPointer { get; set; }

                /// <summary>
                /// Number of Rows/Header Entries
                /// </summary>
                public int RowCount { get; set; }
            }

            /// <summary>
            /// Bo3 Animation Selector Row 
            /// </summary>
            public struct AnimationSelectorRow
            {
                /// <summary>
                /// Pointer to the raw data
                /// </summary>
                public long DataPointer { get; set; }

                /// <summary>
                /// Number of Columns/Headers
                /// </summary>
                public int ColumnCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }
            }

            /// <summary>
            /// Bo3 Animation Selector Row Column
            /// </summary>
            [StructLayout(LayoutKind.Explicit)]
            public struct AnimationSelectorRowColumn
            {
                /// <summary>
                /// String Index
                /// </summary>
                [FieldOffset(0)]
                public int StringIndex;

                /// <summary>
                /// Integer Value
                /// </summary>
                [FieldOffset(4)]
                public int IntegerValue;

                /// <summary>
                /// Float Value
                /// </summary>
                [FieldOffset(4)]
                public float FloatValue;
            }

            /// <summary>
            /// Bo3 Animation Map Header
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct AnimationSelectorColumn
            {
                /// <summary>
                /// Index of the Name of this Map in the String Database
                /// </summary>
                public int NameStringIndex { get; set; }

                /// <summary>
                /// Unknown Ints (Constant 8 and 256?)
                /// </summary>
                private long Padding { get; set; }

                /// <summary>
                /// Data Type of this Property (0 = null, 1 = String, 2 = Int)
                /// </summary>
                public AnimationSelectorObj.Column.ColumnDataType DataType { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private long Padding1 { get; set; }
            }

            public static void Load(AssetPool assetPool)
            {
                // Loop through entire pool, and add what valid assets
                for (int i = 0; i < assetPool.Size; i++)
                {
                    // Read Rawfile
                    var animSelectorTable = Hydra.ActiveGameReader.ReadStruct<AnimationSelectorTableHeader>(assetPool.FirstEntry + (i * assetPool.HeaderSize));
                    // Check is it a null/empty slot
                    if (assetPool.IsNullAsset(animSelectorTable.NamePointer))
                        continue;
                    // Add Asset
                    Hydra.LoadedAssets.Add(new Asset()
                    {
                        Name           = Hydra.ActiveGameReader.ReadNullTerminatedString(animSelectorTable.NamePointer),
                        HeaderAddress  = assetPool.FirstEntry + (i * assetPool.HeaderSize),
                        ExportFunction = Export,
                        Type           = assetPool.Name,
                        Information    = String.Format("Selectors - {0}", animSelectorTable.SelectorCount),
                    });
                }
            }

            public static bool Export(Asset asset)
            {
                // Read Header
                var animSelectorTable = Hydra.ActiveGameReader.ReadStruct<AnimationSelectorTableHeader>(asset.HeaderAddress);
                // Check name pointer, if it's changed, our asset has changed
                if (asset.Name != Hydra.ActiveGameReader.ReadNullTerminatedString(animSelectorTable.NamePointer))
                    return false;
                // Create output path
                string path = Path.Combine("exported_files", Hydra.ActiveGameName, "animtables", asset.Name);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                // Preallocate and read buffer (buffer = MapCount * sizeof(AnimationSelectorHeader) (which is 88))
                AnimationSelectorObj[] animationSelectors = new AnimationSelectorObj[animSelectorTable.SelectorCount];
                byte[] buffer = Hydra.ActiveGameReader.ReadBytes(animSelectorTable.DataPointer, animSelectorTable.SelectorCount * 88);
                // Loop selectors
                for(int i = 0; i < animSelectorTable.SelectorCount; i++)
                {
                    // Grab
                    var animSelector = ByteUtil.BytesToStruct<AnimationSelectorHeader>(buffer, i * 88);
                    // Create the selector
                    animationSelectors[i] = new AnimationSelectorObj(GetString(animSelector.NameStringIndex), animSelector.RowCount, animSelector.ColumnCount);
                    // Read Columns
                    byte[] columnBuffer = Hydra.ActiveGameReader.ReadBytes(animSelector.ColumnsPointer, animSelector.ColumnCount * 24);
                    // Loop Columns
                    for (int j = 0; j < animSelector.ColumnCount; j++)
                    {
                        // Grab struct
                        var column = ByteUtil.BytesToStruct<AnimationSelectorColumn>(columnBuffer, j * 24);
                        // Add to selector
                        animationSelectors[i].Columns[j] = new AnimationSelectorObj.Column(GetString(column.NameStringIndex), column.DataType);
                    }
                    // Read Rows
                    byte[] rowBuffer = Hydra.ActiveGameReader.ReadBytes(animSelector.RowsPointer, animSelector.RowCount * 16);
                    // Loop headers
                    for (int j = 0; j < animSelector.RowCount; j++)
                    {
                        // Grab struct
                        var row = ByteUtil.BytesToStruct<AnimationSelectorRow>(rowBuffer, j * 16);
                        // Add to selector
                        animationSelectors[i].Rows[j] = new AnimationSelectorObj.Row(row.ColumnCount);
                        // Read Columns
                        byte[] rowColumnBuffer = Hydra.ActiveGameReader.ReadBytes(row.DataPointer, row.ColumnCount * 8);
                        // Loop Columns
                        for (int k = 0; k < row.ColumnCount; k++)
                        {
                            // Grab struct
                            var column = ByteUtil.BytesToStruct<AnimationSelectorRowColumn>(rowColumnBuffer, k * 8);
                            // Get String
                            string stringValue = GetString(column.StringIndex).ToUpper();
                            // Switch Type
                            switch(animationSelectors[i].Columns[k].DataType)
                            {
                                // Enumerators/Strings
                                case AnimationSelectorObj.Column.ColumnDataType.String:
                                case AnimationSelectorObj.Column.ColumnDataType.Enumerator:
                                    {
                                        animationSelectors[i].Rows[j].Columns[k] = String.IsNullOrEmpty(stringValue) ? "*" : stringValue;
                                        break;
                                    }
                                // Ints
                                case AnimationSelectorObj.Column.ColumnDataType.Int32:
                                    {
                                        animationSelectors[i].Rows[j].Columns[k] = String.IsNullOrEmpty(stringValue) ? column.IntegerValue.ToString() : stringValue;
                                        break;
                                    }
                                // Floats
                                case AnimationSelectorObj.Column.ColumnDataType.Float:
                                case AnimationSelectorObj.Column.ColumnDataType.FloatMin:
                                case AnimationSelectorObj.Column.ColumnDataType.FloatMax:
                                    {
                                        animationSelectors[i].Rows[j].Columns[k] = String.IsNullOrEmpty(stringValue) ? column.FloatValue.ToString() : stringValue;
                                        break;
                                    }
                            }
                        }
                    }
                }
                // Output Stream
                using (StreamWriter writer = new StreamWriter(path))
                {
                    // Loop Selectors
                    for (int i = 0; i < animationSelectors.Length; i++)
                    {
                        // Write Name
                        writer.WriteLine(animationSelectors[i].Name + ",");
                        // Write Columns
                        for (int k = 0; k < animationSelectors[i].Columns.Length; k++)
                            writer.Write(animationSelectors[i].Columns[k].Name + ",");
                        // End Line
                        writer.WriteLine();
                        // Write Rows
                        for (int k = 0; k < animationSelectors[i].Rows.Length; k++)
                        {
                            // Write Columns
                            for (int j = 0; j < animationSelectors[i].Rows[k].Columns.Length; j++)
                                writer.Write(animationSelectors[i].Rows[k].Columns[j] + ",");
                            // End Line
                            writer.WriteLine();
                        }
                        // Write comma separator
                        writer.WriteLine(",");
                    }
                }
                // Done
                return true;
            }
        }
    }
}
