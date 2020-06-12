using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace HydraX.Library
{
    partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Localize Logic
        /// </summary>
        private class Localize : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// Localize Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct LocalizeAsset
            {
                #region LocalizeProperties
                public long LocalizePointer;
                public long ReferencePointer;
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
            public string Name => "localize";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "RawFile";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.localize;

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

                results.Add(new Asset()
                {
                    Name        = "localizedstrings.str",
                    Type        = Name,
                    Status      = "Loaded",
                    Data        = poolInfo.PoolPointer,
                    Zone        = "none",
                    LoadMethod  = ExportAsset,
                    Information = "N/A"
                });

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                string path = Path.Combine(instance.ExportFolder, asset.Name);
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                using (var writer = new StreamWriter(path))
                {
                    writer.WriteLine("VERSION				\"1\"");
                    writer.WriteLine("CONFIG				\"C:\\projects\\cod\\t7\\bin\\StringEd.cfg\"");
                    writer.WriteLine("FILENOTES		    \"Dumped via HydraX by Scobalula\"");
                    writer.WriteLine();

                    var localizedStrings = instance.Reader.ReadArray<LocalizeAsset>(StartAddress, AssetCount);

                    for(int i = 0; i < localizedStrings.Length; i++)
                    {
                        if (IsNullAsset(localizedStrings[i].LocalizePointer))
                            continue;

                        writer.WriteLine("REFERENCE            {0}", instance.Reader.ReadNullTerminatedString(localizedStrings[i].ReferencePointer));
                        writer.WriteLine("LANG_ENGLISH         \"{0}\"", instance.Reader.ReadNullTerminatedString(localizedStrings[i].LocalizePointer));
                        writer.WriteLine();
                    }
                }

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
