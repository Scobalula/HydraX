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
        private Dictionary<string, GameDataTable> GDTs = new Dictionary<string, GameDataTable>();

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
        public List<Asset> Assets { get; set; }

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
            if (Settings["CheckGDTDB", "Yes"] == "Yes")
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
        }

        /// <summary>
        /// Checks if the image exists in the GDT DB
        /// </summary>
        /// <param name="name">name of the image</param>
        public bool ExistsInGDTDB(string assetType, string name)
        {
            if (Settings["CheckGDTDB", "No"] == "No")
                return false;

            return GDTDatabase.Contains(name);
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

        public void AddGDTAsset(GameDataTable.Asset asset, string type, string name)
        {
            lock(GDTs)
            {
                if (ExistsInGDTDB(type, name))
                    return;

                if (!GDTs.TryGetValue(asset.Type, out var gdt))
                {
                    gdt = new GameDataTable();
                    GDTs[asset.Type] = gdt;
                }

                gdt[type, name] = asset;
            }
        }

        public void LoadGDTs()
        {
            GDTs.Clear();

            try
            {
                if (Game != null)
                {
                    if (Settings["OverwriteGDT", "Yes"] == "No")
                    {
                        string outputFolder = Path.Combine("exported_files", Game.Name, "source_data", "hydrax_gdts");

                        foreach (var gdt in Directory.GetFiles(outputFolder, "*.gdt"))
                        {
                            try
                            {
                                GDTs[Path.GetFileNameWithoutExtension(gdt).Replace("_assets", "")] = new GameDataTable(gdt);
                            }
                            catch
                            {

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

        public void LoadGame()
        {
            Assets = new List<Asset>();

            foreach (var gdt in GDTs)
                gdt.Value.Assets.Clear();

            Process[] processes = Process.GetProcesses();

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

                            if (Game.Initialize(this))
                            {

                                Game.AssetPools = GetAssetPools(Game);
#if DEBUG
                                // For printing a new support table for the README.md
                                Console.WriteLine("| {0} |\n|----------------------------------|", "Asset Type".PadRight(32));
#endif
                                foreach (var assetPool in Game.AssetPools)
                                {
#if DEBUG
                                    Console.WriteLine("| {0} |", assetPool.Name.PadRight(32));
#endif
                                    Assets.AddRange(assetPool.Load(this));
                                }

                                return;
                            }
                            else
                            {
                                Clear();
                                throw new GameNotSupportedException(game.Name);
                            }
                        }
                    }
                }
            }

            throw new GameNotFoundException();
        }
    }
}