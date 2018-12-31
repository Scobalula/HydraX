using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib
{
    /// <summary>
    /// Creates a new title for in-game exporting support
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Process Name
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// Game Name
        /// </summary>
        public string GameName { get; set; }

        /// <summary>
        /// Load Assets Function
        /// </summary>
        public Action LoadFunction { get; set; }

        /// <summary>
        /// Load Assets Function
        /// </summary>
        public Func<string, bool> GetAddresses { get; set; }

        /// <summary>
        /// Process Key (SP, etc.)
        /// </summary>
        public string AddressesKey { get; set; }

        /// <summary>
        /// Creates a new Game we support
        /// </summary>
        /// <param name="processName">Game's Process Name</param>
        /// <param name="loadFunc">Game's Load Function</param>
        public Game(string processName, string gameName, Action loadFunc, string addressesKey, Func<string, bool> getAddresses)
        {
            ProcessName = processName;
            LoadFunction = loadFunc;
            AddressesKey = addressesKey;
            GameName = gameName;
            GetAddresses = getAddresses;
        }

        /// <summary>
        /// Loads assets from the Game
        /// </summary>
        public void Load()
        {
            LoadFunction();
        }
    }
}
