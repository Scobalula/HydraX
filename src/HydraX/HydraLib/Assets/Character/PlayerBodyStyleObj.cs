using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Assets
{
    class PlayerBodyStyleObj
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
        [GameDataTable.Property("characterMovementFxOverride")]
        public string CharacterMovementFXOverride { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("firstPersonCinematicModel")]
        public string FirstPersonCinematicModel { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("firstPersonLegsModel")]
        public string FirstPersonLegsModel { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("gibDef")]
        public string GibDef { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("gibLegsBoth")]
        public string GibLegsBoth { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("gibLegsClean")]
        public string GibLegsClean { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("gibLegsLeft")]
        public string GibLegsLeft { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("gibLegsRight")]
        public string GibLegsRight { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("gibTorsoClean")]
        public string GibTorsoClean { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("gibTorsoLeft")]
        public string GibTorsoLeft { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("gibTorsoRight")]
        public string GibTorsoRight { get; set; }

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
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("viewArmsModel")]
        public string ViewArmsModel { get; set; }

        /// <summary>
        /// Movement FX Override
        /// </summary>
        [GameDataTable.Property("xmodel")]
        public string ThirdPersonModel { get; set; }
    }
}
