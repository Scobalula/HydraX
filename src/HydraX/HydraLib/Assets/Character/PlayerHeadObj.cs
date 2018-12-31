using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Assets
{
    /// <summary>
    /// Generic Player Head Object
    /// </summary>
    class PlayerHeadObj
    {
        /// <summary>
        /// In-Game Display Name
        /// </summary>
        [GameDataTable.Property("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Icon Image Asset Name
        /// </summary>
        [GameDataTable.Property("iconImage")]
        public string Icon { get; set; }

        /// <summary>
        /// Model Asset Name
        /// </summary>
        [GameDataTable.Property("model")]
        public string Model { get; set; }

        /// <summary>
        /// Gender (0 = Male, 1 = Female)
        /// </summary>
        [GameDataTable.Property("gender")]
        public int Gender { get; set; }
    }
}
