using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Animation Selector Table Logic
        /// </summary>
        private class AnimationSelectorTable : IAssetPool
        {
            #region Enums
            /// <summary>
            /// Column Data Types
            /// </summary>
            private enum ColumnDataType : int
            {
                Enumerator = 0,
                Float      = 1,
                Int32      = 2,
                FloatMin   = 3,
                FloatMax   = 4,
                String     = 6,
            }
            #endregion

            #region AssetStructures
            /// <summary>
            /// Animation Selector Table Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x10)]
            private struct AnimationSelectorTableAsset
            {
                #region AnimationSelectorTableProperties
                public long NamePointer;
                public long SelectorsPointer;
                public int SelectorCount;
                #endregion
            }

            /// <summary>
            /// Animation Selector Table Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x58)]
            private struct AnimationSelector
            {
                #region AnimationSelectorProperties
                public int NameStringIndex { get; set; }
                public long ColumnsPointer { get; set; }
                public int ColumnCount { get; set; }
                public long RowsPointer { get; set; }
                public int RowCount { get; set; }
                // Other data we don't give a fuck about
                #endregion
            }

            /// <summary>
            /// Animation Selector Table Column Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct AnimationSelectorColumn
            {
                #region AnimationSelectorColumnProperties
                public int NameStringIndex { get; set; }
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x8)]
                public byte[] Flags;
                public ColumnDataType DataType;
                public long VerifyMePls;
                #endregion
            }

            /// <summary>
            /// Animation Selector Table Row Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct AnimationSelectorRow
            {
                #region AnimationSelectorRowProperties
                public long ColumnsPointer { get; set; }
                public int ColumnCount { get; set; }
                #endregion
            }

            /// <summary>
            /// Animation Selector Table Row Column Structure
            /// </summary>
            [StructLayout(LayoutKind.Explicit, Size = 8)]
            private struct AnimationSelectorRowColumn
            {
                #region AnimationSelectorRowColumnProperties
                [FieldOffset(0)]
                public int StringIndex;
                [FieldOffset(4)]
                public int IntegerValue;
                [FieldOffset(4)]
                public float FloatValue;
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
            public string Name => "animselectortable";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "AI";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.animselectortable;

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
                    var header = instance.Reader.ReadStruct<AnimationSelectorTableAsset>(StartAddress + (i * AssetSize));

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
                        Information = string.Format("Selectors: {0}", header.SelectorCount)
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<AnimationSelectorTableAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                string path = Path.Combine(instance.AnimationTableFolder, asset.Name);
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                var selectors = instance.Reader.ReadArray<AnimationSelector>(header.SelectorsPointer, header.SelectorCount);

                using (StreamWriter writer = new StreamWriter(path))
                {
                    for(int i = 0; i < selectors.Length; i++)
                    {
                        writer.WriteLine(instance.Game.GetString(selectors[i].NameStringIndex, instance));

                        var columns = instance.Reader.ReadArray<AnimationSelectorColumn>(selectors[i].ColumnsPointer, selectors[i].ColumnCount);

                        for (int j = 0; j < columns.Length; j++)
                            writer.Write("{0},", instance.Game.GetString(columns[j].NameStringIndex, instance));
                        writer.WriteLine();

                        var rows = instance.Reader.ReadArray<AnimationSelectorRow>(selectors[i].RowsPointer, selectors[i].RowCount);

                        for(int j = 0; j < rows.Length; j++)
                        {
                            var rowColumns = instance.Reader.ReadArray<AnimationSelectorRowColumn>(rows[j].ColumnsPointer, rows[j].ColumnCount);

                            for (int k = 0; k < columns.Length; k++)
                            {
                                string stringValue = instance.Game.GetString(rowColumns[k].StringIndex, instance).ToUpper();

                                switch (columns[k].DataType)
                                {
                                    case ColumnDataType.String:
                                    case ColumnDataType.Enumerator:
                                        writer.Write("{0},", string.IsNullOrEmpty(stringValue) ? "*" : stringValue);
                                        break;
                                    case ColumnDataType.Int32:
                                        writer.Write("{0},", string.IsNullOrEmpty(stringValue) ? rowColumns[k].IntegerValue.ToString() : stringValue);
                                        break;
                                    case ColumnDataType.Float:
                                    case ColumnDataType.FloatMin:
                                    case ColumnDataType.FloatMax:
                                        writer.Write("{0},", string.IsNullOrEmpty(stringValue) ? rowColumns[k].FloatValue.ToString() : stringValue);
                                        break;
                                }
                            }

                            writer.WriteLine();
                        }

                        writer.WriteLine(",");
                    }
                }

                // Done
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
