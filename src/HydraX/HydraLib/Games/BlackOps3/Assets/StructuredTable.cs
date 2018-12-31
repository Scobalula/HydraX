using Newtonsoft.Json;
using PhilLibX;
using System;
using System.Collections.Generic;
using System.IO;
using HydraLib.Assets;

namespace HydraLib.Games
{
    public partial class BlackOps3
    {
        public class StructuredTable
        {
            /// <summary>
            /// Structured Table Data Types
            /// </summary>
            public enum StructuredTableDataType : int
            {
                /// <summary>
                /// Null Property
                /// </summary>
                Null = 0,
                
                /// <summary>
                /// String Property
                /// </summary>
                String = 1,

                /// <summary>
                /// 32Bit Int Property
                /// </summary>
                Int32 = 2,
            }

            /// <summary>
            /// Bo3 StructuredTable Header
            /// </summary>
            public struct StructuredTableHeader
            {
                /// <summary>
                /// Pointer to the name of this raw file
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Number of Properties of all Entries
                /// </summary>
                public int DataCount { get; set; }

                /// <summary>
                /// Number of Properties/Data within each Entry
                /// </summary>
                public int PropertyCount { get; set; }

                /// <summary>
                /// Number of Entries
                /// </summary>
                public int EntryCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                public int Padding { get; set; }

                /// <summary>
                /// Pointer to the data
                /// </summary>
                public long DataPointer { get; set; }

                /// <summary>
                /// Points to Ints that corrolate with the data count
                /// </summary>
                public long UnknownPointer { get; set; }

                /// <summary>
                /// Pointer to the data
                /// </summary>
                public long PropertiesPointer { get; set; }

                /// <summary>
                /// Points to Ints that corrolate with the property count
                /// </summary>
                public long UnknownPointer2 { get; set; }
            }

            /// <summary>
            /// Bo3 String Table Cell
            /// </summary>
            public struct StructuredTableProperty
            {
                /// <summary>
                /// Pointer to the string of this property
                /// </summary>
                public long StringPointer { get; set; }

                /// <summary>
                /// DJB Hash of the string of this property
                /// </summary>
                public int DJBHash { get; set; }

                /// <summary>
                /// Index of this property
                /// </summary>
                public int Index { get; set; }
            }

            /// <summary>
            /// Bo3 String Table Cell
            /// </summary>
            public struct StructuredTableEntryProperty
            {
                /// <summary>
                /// Data Type of this Property (0 = null, 1 = String, 2 = Int)
                /// </summary>
                public StructuredTableDataType DataType { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }

                /// <summary>
                /// If DataType is string, this is a pointer to the string
                /// </summary>
                public long StringPointer { get; set; }

                /// <summary>
                /// If DataType is Int, this is the value 
                /// </summary>
                public int IntegerValue { get; set; }

                /// <summary>
                /// If DataType is Int, this should match the above, otherwise, it's a DJB Hash of the string
                /// </summary>
                public int Checksum { get; set; }
            }

            public static void Load(AssetPool assetPool)
            {
                for (int i = 0; i < assetPool.Size; i++)
                {
                    // Read Rawfile
                    var stringTable = Hydra.ActiveGameReader.ReadStruct<StructuredTableHeader>(assetPool.FirstEntry + (i * assetPool.HeaderSize));
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
                        Information = String.Format("Entries: {0} - Properties: {1}", stringTable.EntryCount, stringTable.PropertyCount)
                    };
                    // Add Asset
                    Hydra.LoadedAssets.Add(asset);
                }
            }

            public static bool Export(Asset asset)
            {
                // Read Header
                var structuredTable = Hydra.ActiveGameReader.ReadStruct<StructuredTableHeader>(asset.HeaderAddress);
                // Check name pointer, if it's changed, our asset has changed
                if (asset.Name != Hydra.ActiveGameReader.ReadNullTerminatedString(structuredTable.NamePointer))
                    return false;
                // Path Result
                string path = Path.Combine("exported_files", Hydra.ActiveGameName, asset.Name);
                // Read Properties
                byte[] propertiesBuffer = Hydra.ActiveGameReader.ReadBytes(structuredTable.PropertiesPointer, 16 * structuredTable.PropertyCount);
                // Resulting Property strings
                string[] properties = new string[structuredTable.PropertyCount];
                // Parse all into our buffer
                for (int i = 0; i < structuredTable.PropertyCount; i++)
                    properties[i] = Hydra.ActiveGameReader.ReadNullTerminatedString(ByteUtil.BytesToStruct<StructuredTableProperty>(propertiesBuffer, i * 16).StringPointer);
                // Read read table (raw size will = DataCount * sizeof(StructuredTableEntryProperty) (which is 24)
                byte[] dataBuffer = Hydra.ActiveGameReader.ReadBytes(structuredTable.DataPointer, structuredTable.DataCount * 24);
                // Output result
                var result = new StructuredTableObj(structuredTable.EntryCount);
                // Loop through Entries
                for (int x = 0; x < structuredTable.EntryCount; x++)
                {
                    // Create Entry
                    result.Data[x] = new Dictionary<string, object>();
                    // Loop through Properties (each Entry will have a slot for ALL properties, even if it didn't use it, if unused, it'll be null)
                    for (int y = 0; y < structuredTable.PropertyCount; y++)
                    {
                        // Get Entry Propert
                        var entryProperty = ByteUtil.BytesToStruct<StructuredTableEntryProperty>(dataBuffer, ((x * structuredTable.PropertyCount) + y) * 24);
                        // Switch by type, add if it's valid
                        switch(entryProperty.DataType)
                        {
                            // Strings
                            case StructuredTableDataType.String:
                                result.Data[x][properties[y]] = Hydra.ActiveGameReader.ReadNullTerminatedString(entryProperty.StringPointer);
                                break;
                            // Integer
                            case StructuredTableDataType.Int32:
                                result.Data[x][properties[y]] = entryProperty.IntegerValue;
                                break;
                            // Null
                            default:
                                break;
                        }
                    }
                }
                // Save
                result.Save(path);
                // Done
                return true;
            }
        }
    }
}
