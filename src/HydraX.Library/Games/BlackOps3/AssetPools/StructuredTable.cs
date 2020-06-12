using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using HydraX.Library.AssetContainers;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Structured Table Logic
        /// </summary>
        private class StructuredTable : IAssetPool
        {
            #region Enums
            /// <summary>
            /// Data Types
            /// </summary>
            public enum EntryDataType : int
            {
                Null   = 0,
                String = 1,
                Int32  = 2,
            }
            #endregion

            #region AssetStructures
            /// <summary>
            /// Structured Table Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x30)]
            private struct StructuredTableAsset
            {
                public long NamePointer;
                public int TotalCellCount;
                public int HeaderCount;
                public int CellCount;
                public long CellsPointer;
                public long CellIndicesPointer;
                public long HeadersPointer;
                public long HeaderIndicesPointer;
            }

            /// <summary>
            /// Structured Table Property Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x10)]
            private struct StructuredTableHeader
            {
                public long StringPointer;
                public int Hash;
                public int Index;
            }

            /// <summary>
            /// Structured Table Entry Data Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x18)]
            private struct StructuredTableCell
            {
                public EntryDataType DataType;
                public long StringPointer;
                public int IntegerValue;
                public int Checksum;
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
            public string Name => "structuredtable";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "MetaData";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.structuredtable;

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
                    var header = instance.Reader.ReadStruct<StructuredTableAsset>(StartAddress + (i * AssetSize));

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
                        Information = string.Format("Entries: {0} - Properties: {1}", header.CellCount, header.HeaderCount)
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<StructuredTableAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                var data            = instance.Reader.ReadArray<StructuredTableCell>(header.CellsPointer, header.TotalCellCount);
                var properties      = instance.Reader.ReadArray<StructuredTableHeader>(header.HeadersPointer, header.HeaderCount);
                var propertyNames   = new string[header.HeaderCount];

                for (int i = 0; i < propertyNames.Length; i++)
                    propertyNames[i] = instance.Reader.ReadNullTerminatedString(properties[i].StringPointer);

                var structuredTableObj = new StructuredTableObj(header.CellCount);

                for(int i = 0; i < header.CellCount; i++)
                {
                    structuredTableObj.Data[i] = new Dictionary<string, object>();

                    for(int j = 0; j < header.HeaderCount; j++)
                    {
                        var dataIndex = (i * header.HeaderCount) + j;

                        // Switch by type, add if it's valid
                        switch (data[dataIndex].DataType)
                        {
                            // Strings
                            case EntryDataType.String:
                                structuredTableObj.Data[i][propertyNames[j]] = instance.Reader.ReadNullTerminatedString(data[dataIndex].StringPointer);
                                break;
                            // Integer
                            case EntryDataType.Int32:
                                structuredTableObj.Data[i][propertyNames[j]] = data[dataIndex].IntegerValue;
                                break;
                            // Null
                            default:
                                break;
                        }
                    }
                }

                structuredTableObj.Save(Path.Combine(instance.ExportFolder, asset.Name));
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
