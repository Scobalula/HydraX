using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace HydraX.Library
{
    partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Raw File Logic
        /// </summary>
        private class RawFile : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// Raw File Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct RawFileAsset
            {
                #region RawFileAssetProperties
                public long NamePointer { get; set; }
                public long Size { get; set; }
                public long DataPointer { get; set; }
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
            public string Name => "rawfile";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "RawFile";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.rawfile;

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
                    var header = instance.Reader.ReadStruct<RawFileAsset>(StartAddress + (i * AssetSize));

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
                        Information = string.Format("Size: 0x{0:X}", header.Size)
                    });
                }

                return results;
            }

            /// <summary>
            /// Decompresses a deflate compressed animation tree
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            private static byte[] DecompressAnimTree(byte[] input)
            {
                using (var compressedStream = new MemoryStream(input))
                {
                    using (var decompressedStream = new MemoryStream())
                    {
                        using (var deflater = new System.IO.Compression.DeflateStream(compressedStream, System.IO.Compression.CompressionMode.Decompress))
                            deflater.CopyTo(decompressedStream);
                        return decompressedStream.ToArray();
                    }
                }
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<RawFileAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                string path = Path.Combine("exported_files", instance.Game.Name, asset.Name.Replace(".lua", ".luac"));
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                byte[] buffer;
                // Check for animation trees, as they are compressed using Deflate
                if (Path.GetExtension(path) == ".atr")
                    buffer = DecompressAnimTree(instance.Reader.ReadBytes(header.DataPointer + 4, (int)header.Size - 4));
                else
                    buffer = instance.Reader.ReadBytes(header.DataPointer, (int)header.Size);

                File.WriteAllBytes(path, buffer);

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
