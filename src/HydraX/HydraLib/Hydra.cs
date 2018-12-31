using HydraLib.Games;
using PhilLibX.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

// TODO: Code Clean Up
// TODO: Missing Documentation on some stuff and copy and pasted entries
// TODO: More Asset Types and do more tests
// TODO: Set up decent tests within the mod tools

namespace HydraLib
{
    public enum HydraStatus
    {
        Success,
        UnsupportedBinary,
        FailedToFindGame,
        Exception,
        MemoryChanged,
        GameClosed,
    }

    public class Hydra
    {
        /// <summary>
        /// Hydra GDTs
        /// </summary>
        public static Dictionary<string, GameDataTable> GDTs = new Dictionary<string, GameDataTable>()
        {
            { "Character",          new GameDataTable() },
            { "Misc",               new GameDataTable() },
            { "XCams",              new GameDataTable() },
            { "Table",              new GameDataTable() },
            { "Physic",             new GameDataTable() },
            { "Camo",               new GameDataTable() },
        };

        /// <summary>
        /// Active Game Process Reader
        /// </summary>
        public static ProcessReader ActiveGameReader { get; set; }

        /// <summary>
        /// Active Game Database Info
        /// </summary>
        public static GameDBInfo ActiveDBInfo { get; set; }

        /// <summary>
        /// Active Game Name
        /// </summary>
        public static string ActiveGameName { get; set; }

        /// <summary>
        /// Active Game Name
        /// </summary>
        public static string ActiveGameProcessName { get; set; }

        /// <summary>
        /// Loaded Assets
        /// </summary>
        public static List<Asset> LoadedAssets { get; set; }

        /// <summary>
        /// Supported Games
        /// </summary>
        public static Game[] Games =
        {
            new Game("BlackOps3",       "Black Ops III",             BlackOps3.LoadAssets,  "Core", BlackOps3.GetAddresses),
        };

        /// <summary>
        /// Checks to ensure game is running and game hasn't changed
        /// </summary>
        public static HydraStatus ValidateGame()
        {
            // Check for it
            if(ActiveGameProcessName != null)
            {
                // Get matches
                Process[] processes = Process.GetProcessesByName(ActiveGameProcessName);
                // Check results
                if(processes.Length == 0)
                    return HydraStatus.GameClosed;
                // Check IDs
                if (processes[0].Id != ActiveGameReader.ActiveProcess.Id)
                        return HydraStatus.MemoryChanged;
            }
            // Done
            return HydraStatus.Success;
        }

        /// <summary>
        /// Clears assets from Hydra
        /// </summary>
        public static void Clear()
        {
            // Clear it
            ActiveGameReader = null;
            ActiveDBInfo = null;
            ActiveGameName = null;
            ActiveGameProcessName = null;
            LoadedAssets = null;
        }

        public static void FlushGDTs()
        {
            // Create Output Directory
            string outputFolder = Path.Combine("exported_files", ActiveGameName, "source_data", "hydrax_gdts");
            Directory.CreateDirectory(outputFolder);
            // Dump GDTs
            foreach(var gdt in GDTs)
            {
                gdt.Value.Save(Path.Combine(outputFolder, gdt.Key.ToLower() + "_assets.gdt"));
                gdt.Value.Assets.Clear();
            }
        }

        /// <summary>
        /// Cleans an FX Asset Name/Path for use in a GDT
        /// </summary>
        /// <param name="fxName">FX Path</param>
        /// <returns></returns>
        public static string CleanFXName(string fxName)
        {
            return String.IsNullOrWhiteSpace(fxName) ? "" : Path.ChangeExtension(fxName.Replace(@"/", @"\").Replace(@"\", @"\\"), ".efx");
        }

        /// <summary>
        /// Loads Assets from any Call of Duty Process.
        /// </summary>
        public static HydraStatus LoadAssets()
        {
            /*
            var x = new GameDataTable("test.gdt");

            using (var writer = new StreamWriter("output.txt"))
            {
                // foreach (var asset in x.Assets)
                {
                    // foreach (var property in asset.Value.Properties)
                    {
                        /*
                        writer.WriteLine("");
                        writer.WriteLine("/// <summary>");
                        writer.WriteLine("///");
                        writer.WriteLine("/// </summary>");
                        writer.WriteLine("[GameDataTable.Property(\"{0}\")]", property.Key);
                        writer.WriteLine("public string {0} {{ get; set; }}", property.Key.First().ToString().ToUpper() + property.Key.Substring(1));
                        
                        writer.WriteLine("\"{0}\",", property.Key);
                        
            var ok = new ProcessReader(Process.GetProcessesByName("linker_modtools")[0]);

                        SortedList<int, string> Values = new SortedList<int, string>();

                        for (int i = 0; i < 60; i++)
                        {
                            writer.WriteLine(ok.ReadInt32((i * 16 + 140701912718032) + 8));
                            Values[ok.ReadInt32((i * 16 + 140701912718032) + 8)] = ok.ReadNullTerminatedString(ok.ReadInt64(i * 16 + 140701912718032));
                        }

                        foreach(var val in Values)
                        {
                            writer.WriteLine("\"{0}\",", val.Value);
                        }
                    }
                }
            }
            */
            // Clear GDTs
            foreach (var gdt in GDTs)
                gdt.Value.Assets.Clear();
            // Status
            var status = HydraStatus.FailedToFindGame;
            // Loop through games
            foreach (Game game in Games)
            {
                // Look for processes
                Process[] processes = Process.GetProcessesByName(game.ProcessName);
                // Check for a result
                if (processes.Length > 0)
                {
                    // Set it
                    ActiveGameReader = new ProcessReader(processes[0]);
                    ActiveGameName = game.GameName;
                    ActiveGameProcessName = game.ProcessName;
                    // Get Addresses
                    if(game.GetAddresses(game.AddressesKey))
                    {
                        Console.WriteLine(ActiveGameReader.GetBaseAddress() + ActiveDBInfo.StringDBAddress);
                        // Create asset list
                        LoadedAssets = new List<Asset>();
                        // Pass to loader
                        game.Load();
                        // Set status
                        status = HydraStatus.Success;
                    }
                    else
                    {
                        // Set status
                        status = HydraStatus.UnsupportedBinary;
                        // Clear it
                        ActiveGameReader = null;
                        ActiveDBInfo = null;
                        ActiveGameName = null;
                        ActiveGameProcessName = null;
                        LoadedAssets = null;
                    }
                    // Done
                    break;
                }
            }
            // Done
            return status;
        }
    }
}
