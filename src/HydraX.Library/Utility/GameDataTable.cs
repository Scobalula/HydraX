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
// TODO: Speed up Read

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
            /// Gets or Sets the Name of Asset
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or Sets the Type of Asset (GDF)
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// Gets or Sets the Parent Asset if Derived
            /// </summary>
            public string Parent { get; set; }

            /// <summary>
            /// Gets or Sets the Asset Properties/Settings
            /// </summary>
            public Dictionary<string, object> Properties = new Dictionary<string, object>();

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
        /// Assets within this Game Data Table, in groups of type
        /// </summary>
        public Dictionary<string, Dictionary<string, Asset>> Assets { get; set; }

        /// <summary>
        /// Initializes a Game Data Table with an Asset List
        /// </summary>
        public GameDataTable()
        {
            Assets = new Dictionary<string, Dictionary<string, Asset>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Asset this[string type, string key]
        {
            get
            {
                return Assets.TryGetValue(type, out var assets) ? (assets.TryGetValue(key, out var val) ? val : null) : null;
            }
            set
            {
                if (!Assets.ContainsKey(type))
                    Assets[type] = new Dictionary<string, Asset>();

                Assets[type][key] = value;
            }
        }

        public Dictionary<string, Asset> this[string type]
        {
            get
            {
                return Assets.TryGetValue(type, out var assets) ? assets: null;
            }
            set
            {
                Assets[type] = value;
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
        /// Attempts to locate the asset in the lists
        /// </summary>
        /// <param name="name">Name of the asset</param>
        /// <returns>Asset</returns>
        public Asset FindAsset(string name)
        {
            foreach (var list in Assets)
                if (list.Value.TryGetValue(name, out var value))
                    return value;

            return null;
        }

        /// <summary>
        /// Attempts to locate the asset in the lists
        /// </summary>
        /// <param name="name">Name of the asset</param>
        /// <returns>Asset</returns>
        public bool ContainsAsset(string type, string name)
        {
            if(Assets.TryGetValue(type, out var list))
                if (list.ContainsKey(name))
                    return true;

            return false;
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
                foreach (var list in Assets)
                {
                    foreach (var asset in list.Value)
                    {
                        // Get whether this asset is derived or not
                        bool isDerived = !string.IsNullOrWhiteSpace(asset.Value.Parent);

                        // Write name and type
                        writer.WriteLine("\t\"{0}\" {2} \"{1}\" {3}",
                            asset.Key,
                            isDerived ? asset.Value.Parent : asset.Value.Type + ".gdf",
                            isDerived ? "[" : "(",
                            isDerived ? "]" : ")");

                        // Write initial setting bracket
                        writer.WriteLine("\t{");

                        // Write Settings by KVP
                        foreach (var setting in asset.Value.Properties)
                            writer.WriteLine("\t\t\"{0}\" \"{1}\"", setting.Key, setting.Value);

                        // Write end bracket
                        writer.WriteLine("\t}");
                    }
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
            Assets = new Dictionary<string, Dictionary<string, Asset>>();

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
                    Assets[activeAsset.Type][activeAsset.Name] = activeAsset;

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
                    // Get whether this asset is derived or not
                    bool isDerived = matches.Count == 0;

                    // Create new asset
                    activeAsset = new Asset();

                    // No matches, check for derived parentheses
                    if (isDerived)
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
                    activeAsset.Parent = matches[1].Value.Replace("\"", "").Replace(".gdf", "");

                    if (isDerived)
                        activeAsset.Type = FindAsset(activeAsset.Parent)?.Type ?? activeAsset.Parent;

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
    }
}
