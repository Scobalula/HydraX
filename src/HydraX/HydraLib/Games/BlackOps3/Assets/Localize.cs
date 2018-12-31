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
        public class Localize
        {
            /// <summary>
            /// Bo3 Raw File Header
            /// </summary>
            public struct LocalizedStringEntry
            {
                /// <summary>
                /// Pointer to the localized string
                /// </summary>
                public long LocalizedStringPointer { get; set; }

                /// <summary>
                /// Pointer to the localized string
                /// </summary>
                public long ReferenceStringPointer { get; set; }
            }

            public static void Load(AssetPool assetPool)
            {
                // Add Asset
                Hydra.LoadedAssets.Add(new Asset()
                {
                    Name = "localizedstrings.str",
                    ExportFunction = Export,
                    Type = assetPool.Name,
                    Information = "N/A"
                });
            }

            public static bool Export(Asset asset)
            {
                // Read Header (we'll use the pool struct as Header)
                var poolInfo = Hydra.ActiveGameReader.ReadStruct<AssetPoolInfo>(Hydra.ActiveGameReader.GetBaseAddress() + Hydra.ActiveDBInfo.AssetDBAddress + 0x2C0);
                // Output Path
                string path = Path.Combine("exported_files", Hydra.ActiveGameName, asset.Name);
                // Create it
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                // End Address
                long endAddress = poolInfo.PoolPointer + (poolInfo.PoolSize * poolInfo.AssetSize);
                // Read Buffer
                byte[] buffer = Hydra.ActiveGameReader.ReadBytes(poolInfo.PoolPointer, poolInfo.PoolSize * poolInfo.AssetSize);
                // Create output 
                StringBuilder output = new StringBuilder();
                // Initial Lines
                output.AppendLine("VERSION				\"1\"");
                output.AppendLine("CONFIG				\"C:\\projects\\cod\\t7\\bin\\StringEd.cfg\"");
                output.AppendLine("FILENOTES		    \"Dumped via HydraX by Scobalula\"");
                output.AppendLine();
                // Loop through entries
                for (int i = 0; i < poolInfo.PoolSize; i++)
                {
                    // Get Entry
                    var entry = ByteUtil.BytesToStruct<LocalizedStringEntry>(buffer, i * 16);
                    // Check if null entry
                    if (entry.LocalizedStringPointer >= poolInfo.PoolPointer && entry.LocalizedStringPointer <= endAddress || entry.LocalizedStringPointer == 0)
                        continue;
                    // Add to output
                    output.AppendLine(String.Format("REFERENCE            {0}", Hydra.ActiveGameReader.ReadNullTerminatedString(entry.ReferenceStringPointer)));
                    output.AppendLine(String.Format("LANG_ENGLISH         \"{0}\"", Hydra.ActiveGameReader.ReadNullTerminatedString(entry.LocalizedStringPointer)));
                    output.AppendLine();
                }
                // Dump
                File.WriteAllText(path, output.ToString());
                // Done
                return true;
            }
        }
    }
}
