using PhilLibX.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            { "Character",          new GameDataTable() },
            { "Misc",               new GameDataTable() },
            { "XCams",              new GameDataTable() },
            { "Table",              new GameDataTable() },
            { "Physic",             new GameDataTable() },
            { "Weapon",             new GameDataTable() },
        };

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
        /// Initializes Supported Games
        /// </summary>
        /// <returns></returns>
        public void Initialize()
        {
            Games = new List<IGame>();
            var gameType = typeof(IGame);
            var games = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => gameType.IsAssignableFrom(p));

            foreach (var game in games)
                if(!game.IsInterface)
                    Games.Add((IGame)Activator.CreateInstance(game));
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

            foreach(var pool in pools)
                if (!pool.IsInterface)
                    if (pool.DeclaringType is Type gameType)
                        if (gameType == game.GetType())
                            results.Add((IAssetPool)Activator.CreateInstance(pool));

            return results;
        }

        public void FlushGDTs()
        {
            if(Game != null)
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
                    if (Settings["OverwriteGDT", "true"] == "false")
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
                    for(int i = 0; i < game.ProcessNames.Length; i++)
                    {
                        if(process.ProcessName == game.ProcessNames[i])
                        {
                            Game = (IGame)game.Clone();
                            Game.ProcessIndex = i;
                            Reader = new ProcessReader(process);

                            if(Game.ValidateAddresses(this))
                            {
                                Game.AssetPools = GetAssetPools(Game);

                                Assets = new List<GameAsset>();
                                Console.WriteLine("| {0} | {1} |", "Asset Type".PadRight(24), "Settings Group".PadRight(24));
                                foreach (var assetPool in Game.AssetPools)
                                {
                                    if (Settings[assetPool.SettingGroup, "true"] == "true")
                                    {
                                        Console.WriteLine("| {0} | {1} |", assetPool.Name.PadRight(24), assetPool.SettingGroup.PadRight(24));
                                        Assets.AddRange(assetPool.Load(this));
                                    }
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
