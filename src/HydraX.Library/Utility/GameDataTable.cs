/*
*  HydraX - A Call of Duty Asset Extractor
*  
*  This file is subject to the license terms set out in the
*  "LICENSE" file. 
* 
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace HydraX.Library
{
    /// <summary>
    /// Class to hold Game Data Table Assets and Read/Write Logic
    /// </summary>
    public class GameDataTable
    {
        /// <summary>
        /// XModel LOD GDT Keys by Index
        /// </summary>
        public static Tuple<string, string, string>[] LODGDTKeys =
        {
            new Tuple<string, string, string>("filename",       "highLodDist",          "autogenHighLod"),
            new Tuple<string, string, string>("mediumLod",      "mediumLodDist",        "autogenMediumLod"),
            new Tuple<string, string, string>("lowLod",         "lowLodDist",           "autogenLowLod"),
            new Tuple<string, string, string>("lowestLod",      "lowestLodDist",        "autogenLowestLod"),
            new Tuple<string, string, string>("lod4File",       "lod4Dist",             "autogenLod4"),
            new Tuple<string, string, string>("lod5File",       "lod5Dist",             "autogenLod5"),
            new Tuple<string, string, string>("lod6File",       "lod6Dist",             "autogenLod6"),
            new Tuple<string, string, string>("lod7File",       "lod7Dist",             "autogenLod7"),
        };

        /// <summary>
        /// Class to hold GDT Asset Information
        /// </summary>
        public class Asset
        {
            /// <summary>
            /// Name of Asset
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Type of Asset (GDF)
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// If this asset is derived or not
            /// </summary>
            public bool Derivative { get; set; }

            /// <summary>
            /// Asset Properties/Settings
            /// </summary>
            public Dictionary<object, object> Properties = new Dictionary<object, object>();

            /// <summary>
            /// Initializes a GDT Asset
            /// </summary>
            public Asset() { }

            /// <summary>
            /// Initializes a GDT Asset with Name and Type
            /// </summary>
            /// <param name="name">Name of the Asset</param>
            /// <param name="type">Type of Asset (GDF)</param>
            public Asset(string name, string type)
            {
                Name = name;
                Type = type;
            }

            /// <summary>
            /// Gets or Sets the Property at the given key
            /// </summary>
            public object this[string key]
            {
                get
                {
                    return Properties.TryGetValue(key, out var val) ? val : null;
                }
                set
                {
                    Properties[key] = value;
                }
            }
        }

        /// <summary>
        /// Assets within this Game Data Table
        /// </summary>
        public Dictionary<string, Asset> Assets { get; set; }

        /// <summary>
        /// Initializes a Game Data Table with an Asset List
        /// </summary>
        public GameDataTable()
        {
            Assets = new Dictionary<string, Asset>();
        }

        /// <summary>
        /// Gets or Sets the Asset with the given name
        /// </summary>
        public Asset this[string key]
        {
            get
            {
                lock(this)
                {
                    return Assets.TryGetValue(key, out var val) ? val : null;
                }
            }
            set
            {
                lock (this)
                {
                    Assets[key] = value;
                }
            }
        }

        /// <summary>
        /// Initializes a Game Data Table with a GDT file
        /// </summary>
        /// <param name="fileName">GDT File to Load</param>
        public GameDataTable(string fileName)
        {
            Load(fileName);
        }

        /// <summary>
        /// Saves Game Data Table to a File
        /// </summary>
        /// <param name="fileName">File Path to save to</param>
        public void Save(string fileName)
        {
            // Create output stream
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                // Write initial bracket
                writer.WriteLine("{");

                // Write each asset
                foreach (var asset in Assets)
                {
                    // Get whether this asset is derived or not
                    bool isDerived = asset.Value.Derivative;

                    // Write name and type
                    writer.WriteLine("	\"{0}\" {2} \"{1}\" {3}",
                        asset.Key,
                        asset.Value.Type + (isDerived ? "" : ".gdf"),
                        isDerived ? "[" : "(",
                        isDerived ? "]" : ")");

                    // Write initial setting bracket
                    writer.WriteLine("	{");

                    // Write Settings by KVP
                    foreach (var setting in asset.Value.Properties)
                        writer.WriteLine("		\"{0}\" \"{1}\"", setting.Key, setting.Value);

                    // Write end bracket
                    writer.WriteLine("	}");
                }

                // Write EOF bracket
                writer.WriteLine("}");

            }
        }

        /// <summary>
        /// Loads a Game Data Table from a File
        /// </summary>
        /// <param name="fileName">File Path of GDT</param>
        public void Load(string fileName)
        {
            // Create new asset list
            Assets = new Dictionary<string, Asset>();

            // Read GDT lines
            string[] lines = File.ReadAllLines(fileName);

            // Current State
            int state = -1;

            // Active Asset
            Asset activeAsset = null;

            // Regex
            MatchCollection matches;

            // Settings
            string key;
            string value;

            // Loop all lines
            for (int i = 0; i < lines.Length; i++)
            {
                // Trim line
                string lineTrim = lines[i].Trim();

                // Check for empty line
                if (string.IsNullOrWhiteSpace(lineTrim))
                    continue;

                // Check for initial bracket
                if (lineTrim == "{" && state == -1)
                {
                    // Set to 0, we're parsing Asset Name/Type
                    state = 0;
                }
                // Check for asset bracket
                else if (lineTrim == "{" && state == 2)
                {
                    // Set to 3, we're parsing settings
                    state = 3;
                }
                // Check for asset end bracket
                else if (lineTrim == "}" && state == 3)
                {
                    // Check for duplicate
                    if (Assets.ContainsKey(activeAsset.Name))
                        throw new ArgumentException(string.Format("Error - Duplicate Asset: {0}.", activeAsset.Name));

                    // Add asset
                    Assets.Add(activeAsset.Name, activeAsset);

                    // Reset state
                    state = 0;
                }
                // Check for EOF Bracket
                else if (lineTrim == "}" && state == 0)
                {
                    // Done
                    state = 4;
                    break;
                }
                // Parse Asset Name + Type
                else if (state == 0)
                {
                    // Check for normal parentheses
                    matches = Regex.Matches(lineTrim, "\\(([^\\)]*)\\)");

                    // Create new asset
                    activeAsset = new Asset() { Derivative = matches.Count == 0 };

                    // No matches, check for derived parentheses
                    if (matches.Count == 0)
                        matches = Regex.Matches(lineTrim, "\\[([^\\)]*)\\]");

                    // Last check
                    if (matches.Count == 0)
                        throw new ArgumentException(string.Format("Parse error on line: {0}. Expecting Parentheses for GDF or Derived Asset.", i));

                    // Get Quotes
                    matches = Regex.Matches(lineTrim, "\"([^\"]*)\"");

                    // Check matches (we should have 2 (Asset Name and GDF/Parent))
                    if (matches.Count < 2)
                        throw new ArgumentException(string.Format("Parse error on line: {0}. Expecting Asset Name and GDF/Parent.", i));

                    // Set Name/Type
                    activeAsset.Name = matches[0].Value.Replace("\"", "");
                    activeAsset.Type = matches[1].Value.Replace("\"", "").Replace(".gdf", "");

                    // Set State
                    state = 2;
                }
                // Check for Asset Settings
                else if (state == 3)
                {
                    // Get Quotes
                    matches = Regex.Matches(lineTrim, "\"([^\"]*)\"");

                    // Check matches (we should have 2 (Asset Name and GDF/Parent))
                    if (matches.Count < 2)
                        throw new ArgumentException(string.Format("Parse error on line: {0}. Expecting Setting Group.", i));

                    // Set Values
                    key = matches[0].Value.Replace("\"", "");
                    value = matches[1].Value.Replace("\"", "");

                    // Check for duplicate setting
                    if (activeAsset.Properties.ContainsKey(key))
                        throw new ArgumentException(string.Format("Error - Duplicate Setting: {0}. Asset: {1}.", key, activeAsset.Name));

                    // Add setting
                    activeAsset.Properties.Add(key, value);
                }
                // Unknown Line
                else
                {
                    // Unexpected token/Syntax issue
                    throw new ArgumentException(string.Format("Parse error on line: {0}. Unexpected line {1}.", i, lineTrim));
                }
            }

            // Check did we finish up on end bracket
            if (state != 4)
            {
                // Unexpected token/Syntax issue
                throw new ArgumentException(string.Format("Expecting EOF Bracket."));
            }
        }

        public static Asset ConvertStructToGDTAsset(byte[] assetBuffer, Tuple<string, int, int>[] properties, HydraInstance instance, Func<byte[], int, int, HydraInstance, object> extendedDataHandler = null)
        {
            // Create new asset
            var asset = new GameDataTable.Asset();

            // Loop through potential properties
            foreach (var property in properties)
            {
                // Switch type
                switch (property.Item3)
                {
                    // Strings (Enum that points to a string)
                    case 0:
                        {
                            asset[property.Item1] = instance.Reader.ReadNullTerminatedString(BitConverter.ToInt64(assetBuffer, property.Item2));
                            break;
                        }
                    // Inline Character Array?
                    case 1:
                        {
                            asset[property.Item1] = Encoding.ASCII.GetString(assetBuffer, property.Item2, 1024).TrimEnd('\0');
                            break;
                        }
                    // Inline Character Array?
                    case 2:
                        {
                            asset[property.Item1] = Encoding.ASCII.GetString(assetBuffer, property.Item2, 64).TrimEnd('\0');
                            break;
                        }
                    // Inline Character Array?
                    case 3:
                        {
                            asset[property.Item1] = Encoding.ASCII.GetString(assetBuffer, property.Item2, 256).TrimEnd('\0');
                            break;
                        }
                    // 32Bit Ints
                    case 4:
                        {
                            asset[property.Item1] = BitConverter.ToInt32(assetBuffer, property.Item2);
                            break;
                        }
                    /// Unknown
                    case 5:
                        {
                            asset[property.Item1] = BitConverter.ToInt32(assetBuffer, property.Item2);
                            break;
                        }
                    // Bools (8Bit)
                    case 6:
                        {
                            asset[property.Item1] = assetBuffer[property.Item2];
                            break;
                        }
                    // Bools (32bit)
                    case 7:
                        {
                            asset[property.Item1] = BitConverter.ToInt32(assetBuffer, property.Item2);
                            break;
                        }
                    // Floats
                    case 8:
                        {
                            asset[property.Item1] = BitConverter.ToSingle(assetBuffer, property.Item2);
                            break;
                        }
                    // Floats that get multiplied by 1000
                    case 9:
                        {
                            asset[property.Item1] = BitConverter.ToInt32(assetBuffer, property.Item2) / 1000.0;
                            break;
                        }
                    // Script Strings
                    case 0x15:
                        {
                            asset[property.Item1] = instance.Game.GetString(BitConverter.ToInt32(assetBuffer, property.Item2), instance);
                            break;
                        }
                    // Asset References
                    case 0x28:
                        {
                            // Pull from the weapon name rather than the asset name
                            asset[property.Item1] = instance.Game.GetAssetName(BitConverter.ToInt64(assetBuffer, property.Item2), instance, 8);
                            break;
                        }
                    case 0xA:
                    case 0xB:
                    case 0xD:
                    case 0x10:
                    case 0x1F:
                    case 0x1B:
                    case 0x1D:
                    case 0x19:
                    case 0x1A:
                    case 0x26:
                    case 0x27:
                    case 0x2A:
                    case 0x2C:
                    case 0x2D:
                    case 0x11:
                    case 0x16:
                    case 0x17:
                    case 0x1E:
                    case 0x2F:
                    case 0x22:
                    case 0x23:
                    case 0x2B:
                    case 0x2E:
                    case 0x20:
                        {
                            var assetName = instance.Game.GetAssetName(BitConverter.ToInt64(assetBuffer, property.Item2), instance, property.Item3 == 0x10 ? 0xF8 : 0);
                            if (property.Item3 == 0xA)
                                assetName = instance.Game.CleanAssetName(HydraAssetType.FX, assetName);
                            asset[property.Item1] = assetName;
                            break;
                        }
                    case 0x18:
                        {
                            asset[property.Item1] = BlackOps3.GetAliasByHash(BitConverter.ToUInt32(assetBuffer, property.Item2));
                            break;
                        }
                    default:
                        {
                            // Attempt to use the extended data handler, otherwise null
                            var result = extendedDataHandler?.Invoke(assetBuffer, property.Item2, property.Item3, instance);

                            if (result != null)
                            {
                                asset[property.Item1] = result;
                            }
                            else
                            {
                                asset[property.Item1] = "";
#if DEBUG
                                Console.WriteLine("Unknown Value: {0} - {1} - {2}", property.Item3, property.Item2, property.Item1);
#endif
                            }
                            // Done
                            break;
                        }
                }
            }

            // Done
            return asset;
        }
    }
}
