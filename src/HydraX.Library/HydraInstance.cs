using Newtonsoft.Json;
using PhilLibX.IO;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HydraX.Library
{
    /// <summary>
    /// A Class to hold an Instance of the Hydra Library
    /// </summary>
    public class HydraInstance
    {
        /// <summary>
        /// Hydra GDTs
        /// </summary>
        public Dictionary<string, GameDataTable> GDTs = new Dictionary<string, GameDataTable>()
        {
            { "AI",                 new GameDataTable() },
            { "Character",          new GameDataTable() },
            { "Misc",               new GameDataTable() },
            { "XCams",              new GameDataTable() },
            { "Table",              new GameDataTable() },
            { "Physic",             new GameDataTable() },
            { "Weapon",             new GameDataTable() },
            { "Material",           new GameDataTable() },
            { "XModel",             new GameDataTable() },
        };

        /// <summary>
        /// Material Technique Cache
        /// </summary>
        public Dictionary<string, MaterialTechniqueSet> TechniqueSetCache = new Dictionary<string, MaterialTechniqueSet>();

        /// <summary>
        /// Gets or Sets the List of Supported Games
        /// </summary>
        public List<IGame> Games { get; set; }

        /// <summary>
        /// Gets or Sets the current Game
        /// </summary>
        public IGame Game { get; set; }

        /// <summary>
        /// Gets or Sets the current Process Reader
        /// </summary>
        public ProcessReader Reader { get; set; }

        /// <summary>
        /// Gets or Sets the current Settings
        /// </summary>
        public HydraSettings Settings = new HydraSettings();

        /// <summary>
        /// Gets or Sets the loaded Assets
        /// </summary>
        public List<GameAsset> Assets { get; set; }

        /// <summary>
        /// Gets the Export Path
        /// </summary>
        public string ExportFolder { get { return Path.Combine("exported_files", Game.Name); } }

        /// <summary>
        /// Gets the Sound Path
        /// </summary>
        public string SoundFolder { get { return Path.Combine(ExportFolder, "sound"); } }

        /// <summary>
        /// Gets the Sound Zone Path
        /// </summary>
        public string SoundZoneFolder { get { return Path.Combine(SoundFolder, "zone"); } }

        /// <summary>
        /// Gets the Sound Music Path
        /// </summary>
        public string SoundMusicFolder { get { return Path.Combine(SoundFolder, "mus"); } }

        /// <summary>
        /// Gets the Animation Table Path
        /// </summary>
        public string AnimationTableFolder { get { return Path.Combine(ExportFolder, "animtables"); } }

        /// <summary>
        /// Gets the ASM Path
        /// </summary>
        public string AnimationStateMachinesFolder { get { return Path.Combine(ExportFolder, "animstatemachines"); } }

        /// <summary>
        /// Gets the Behavior Path
        /// </summary>
        public string BehaviorFolder { get { return Path.Combine(ExportFolder, "behavior"); } }

        /// <summary>
        /// Gets or Sets all assets in the GDT Database
        /// </summary>
        public List<string> GDTDatabase { get; set; }

        /// <summary>
        /// Initializes Supported Games
        /// </summary>
        /// <returns></returns>
        public void Initialize()
        {
            GDTDatabase = new List<string>();
            Games = new List<IGame>();
            var gameType = typeof(IGame);
            var games = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => gameType.IsAssignableFrom(p));

            foreach (var game in games)
                if (!game.IsInterface)
                    Games.Add((IGame)Activator.CreateInstance(game));

            try
            {
                TechniqueSetCache = JsonConvert.DeserializeObject<Dictionary<string, MaterialTechniqueSet>>(File.ReadAllText("Data\\TechsetCache.json"));
            }
            catch { }
        }

        /// <summary>
        /// Refreshes list of assets from GDT DB
        /// </summary>
        public void RefreshGDTDB()
        {
            try
            {
                var toolsPath = Environment.GetEnvironmentVariable("TA_TOOLS_PATH");

                if (!File.Exists("icudt64r53.dll"))
                    File.Copy(Path.Combine(toolsPath, "bin\\icudt64r53.dll"), "icudt64r53.dll", true);
                if (!File.Exists("icuin64r53.dll"))
                    File.Copy(Path.Combine(toolsPath, "bin\\icuin64r53.dll"), "icuin64r53.dll", true);
                if (!File.Exists("icuuc64r53.dll"))
                    File.Copy(Path.Combine(toolsPath, "bin\\icuuc64r53.dll"), "icuuc64r53.dll", true);
                if (!File.Exists("sqlite64r.dll"))
                    File.Copy(Path.Combine(toolsPath, "bin\\sqlite64r.dll"), "sqlite64r.dll", true);

                // Create and open the SQL Connection
                var connection = new SQLiteConnection(@"data source=" + Path.Combine(toolsPath, "gdtdb\\gdt.db"));
                connection.Open();

                // Create Command
                var command = new SQLiteCommand("SELECT name FROM _entity;", connection);

                // Execute the Read
                var reader = command.ExecuteReader();

                // Loop all assets
                while (reader.Read())
                    GDTDatabase.Add(reader["name"].ToString().Split('/').Last());
            }
            catch { }
        }

        /// <summary>
        /// Checks if the image exists in the GDT DB
        /// </summary>
        /// <param name="name">name of the image</param>
        public bool ExistsInGDTDB(string name)
        {
            if (Settings["CheckGDTDB", "Yes"] == "No")
                return false;

            return GDTDatabase.Contains(name);
        }

        /// <summary>
        /// Checks to ensure game is running and hasn't changed
        /// </summary>
        public HydraStatus ValidateGame()
        {
            if (Reader != null && Game != null)
            {
                Process[] processes = Process.GetProcessesByName(Game.ProcessNames[Game.ProcessIndex]);

                if (processes.Length == 0)
                    return HydraStatus.GameClosed;
                if (processes[0].Id != Reader.ActiveProcess.Id)
                    return HydraStatus.MemoryChanged;

                return HydraStatus.Success;
            }

            return HydraStatus.GameClosed;
        }

        /// <summary>
        /// Gets Asset Pools for the given Game
        /// </summary>
        public static List<IAssetPool> GetAssetPools(IGame game)
        {
            var poolType = typeof(IAssetPool);
            var results = new List<IAssetPool>();

            var pools = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => poolType.IsAssignableFrom(p));

            foreach (var pool in pools)
                if (!pool.IsInterface)
                    if (pool.DeclaringType is Type gameType)
                        if (gameType == game.GetType())
                            results.Add((IAssetPool)Activator.CreateInstance(pool));

            return results;
        }

        public void FlushGDTs()
        {
            if (Game != null)
            {
                string outputFolder = Path.Combine("exported_files", Game.Name, "source_data", "hydrax_gdts");
                Directory.CreateDirectory(outputFolder);

                foreach (var gdt in GDTs)
                {
                    gdt.Value.Save(Path.Combine(outputFolder, gdt.Key.ToLower() + "_assets.gdt"));
                    gdt.Value.Assets.Clear();
                }
            }
        }

        public void LoadGDTs()
        {
            try
            {
                if (Game != null)
                {
                    if (Settings["OverwriteGDT", "Yes"] == "No")
                    {
                        string outputFolder = Path.Combine("exported_files", Game.Name, "source_data", "hydrax_gdts");

                        foreach (var gdt in GDTs)
                        {
                            var gdtPath = Path.Combine(outputFolder, gdt.Key.ToLower() + "_assets.gdt");

                            if (File.Exists(gdtPath))
                            {
                                gdt.Value.Assets.Clear();
                                gdt.Value.Assets = new GameDataTable(gdtPath).Assets;
                            }
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }

        public void Clear()
        {
            Game = null;
            Reader = null;
            Assets = null;

            foreach (var gdt in GDTs)
                gdt.Value.Assets.Clear();
        }

        public HydraInstance()
        {
            Initialize();
        }

        public HydraStatus LoadGame()
        {
            foreach (var gdt in GDTs)
                gdt.Value.Assets.Clear();

            Process[] processes = Process.GetProcesses();

            var status = HydraStatus.FailedToFindGame;

            foreach (var process in processes)
            {
                foreach (var game in Games)
                {
                    for (int i = 0; i < game.ProcessNames.Length; i++)
                    {
                        if (process.ProcessName.ToLower() == game.ProcessNames[i].ToLower())
                        {
                            Game = (IGame)game.Clone();
                            Game.ProcessIndex = i;
                            Reader = new ProcessReader(process);

                            if (Game.ValidateAddresses(this))
                            {
                                Game.AssetPools = GetAssetPools(Game);

                                Assets = new List<GameAsset>();
#if DEBUG
                                // For printing a new support table for the README.md
                                Console.WriteLine("| {0} | {1} |", "Asset Type".PadRight(32), "Settings Group".PadRight(32));
#endif
                                foreach (var assetPool in Game.AssetPools)
                                {
                                    if (Settings["Show" + assetPool.SettingGroup, "Yes"] == "Yes")
                                    {
#if DEBUG
                                        Console.WriteLine("| {0} | {1} |", assetPool.Name.PadRight(32), assetPool.SettingGroup.PadRight(32));
#endif
                                        Assets.AddRange(assetPool.Load(this));
                                    }
#if DEBUG
                                    else
                                    {
                                        Console.WriteLine("Ignoring Pool: {0}", assetPool.Name);
                                    }
#endif
                                }

                                status = HydraStatus.Success;
                            }
                            else
                            {
                                Clear();
                                status = HydraStatus.UnsupportedBinary;
                            }

                            break;
                        }
                    }
                }
            }

            return status;
        }
    }
}