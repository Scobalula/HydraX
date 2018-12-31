using PhilLibX.Compression;
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
        public class RawFile
        {
            /// <summary>
            /// Bo3 Raw File Header
            /// </summary>
            public struct RawFileHeader
            {
                /// <summary>
                /// Pointer to the name of this raw file
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Raw Size of this Asset
                /// </summary>
                public long Size { get; set; }

                /// <summary>
                /// Pointer to the raw data
                /// </summary>
                public long DataPointer { get; set; }
            }

            public static void Load(AssetPool assetPool)
            {
                for (int i = 0; i < assetPool.Size; i++)
                {
                    // Read Rawfile
                    var rawFile = Hydra.ActiveGameReader.ReadStruct<RawFileHeader>(assetPool.FirstEntry + (i * assetPool.HeaderSize));
                    // Check is it a null/empty slot
                    if (assetPool.IsNullAsset(rawFile.NamePointer))
                        continue;
                    // Create new asset
                    Asset asset = new Asset()
                    {
                        Name = Hydra.ActiveGameReader.ReadNullTerminatedString(rawFile.NamePointer),
                        HeaderAddress = assetPool.FirstEntry + (i * assetPool.HeaderSize),
                        ExportFunction = Export,
                        Type = assetPool.Name,
                        Information = String.Format("Size: 0x{0:X}", rawFile.Size)
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
                var rawFile = Hydra.ActiveGameReader.ReadStruct<RawFileHeader>(asset.HeaderAddress);
                // Check name pointer, if it's changed, our asset has changed
                if (asset.Name != Hydra.ActiveGameReader.ReadNullTerminatedString(rawFile.NamePointer))
                    return false;
                // Create Directory
                string path = Path.Combine("exported_files", Hydra.ActiveGameName, asset.Name);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                // Resulting buffer
                byte[] buffer;
                // Check for animation trees, as they are compressed using Deflate
                if (Path.GetExtension(path) == ".atr")
                    buffer = Deflate.Decompress(Hydra.ActiveGameReader.ReadBytes(rawFile.DataPointer + 4, (int)rawFile.Size - 4));
                else
                    buffer = Hydra.ActiveGameReader.ReadBytes(rawFile.DataPointer, (int)rawFile.Size);
                // Dump it
                File.WriteAllBytes(path, buffer);
                // Done
                return true;
            }
        }
    }
}
