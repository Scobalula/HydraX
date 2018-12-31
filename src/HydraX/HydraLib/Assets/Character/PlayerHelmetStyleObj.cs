using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Assets
{
    class PlayerHelmetStyleObj
    {
        /// <summary>
        /// Accent Color Count (Skins)
        /// </summary>
        [GameDataTable.Property("accentColor1OptionsCount")]
        public int AccentColor1Count { get; set; }

        /// <summary>
        /// Accent Color Count (Skins)
        /// </summary>
        [GameDataTable.Property("accentColorCount")]
        public int AccentColorCount { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("hideTags")]
        public string HideTags { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("iconImage")]
        public string Icon { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("impactType")]
        public string ImpactType { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("impactTypeCorpse")]
        public string ImpactTypeCorpse { get; set; }

        /// <summary>
        /// Model Asset Name
        /// </summary>
        [GameDataTable.Property("model")]
        public string Model { get; set; }
    }
}
