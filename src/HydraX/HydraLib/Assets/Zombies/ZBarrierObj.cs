using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Assets
{
    public class ZBarrierObj
    {
        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("attackSpotHorzOffset")]
        public string AttackSpotHorzOffset { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("autoHideOpenPieces")]
        public string AutoHideOpenPieces { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("collisionModel")]
        public string CollisionModel { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("delayBetweenGeneralRepSounds")]
        public string DelayBetweenGeneralRepSounds { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("earthquakeMaxDuration")]
        public string EarthquakeMaxDuration { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("earthquakeMaxScale")]
        public string EarthquakeMaxScale { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("earthquakeMinDuration")]
        public string EarthquakeMinDuration { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("earthquakeMinScale")]
        public string EarthquakeMinScale { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("earthquakeOnRepair")]
        public string EarthquakeOnRepair { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("earthquakeRadius")]
        public string EarthquakeRadius { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("generalRepairSound0")]
        public string GeneralRepairSound0 { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("generalRepairSound1")]
        public string GeneralRepairSound1 { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("numAttackSlots")]
        public string NumAttackSlots { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("reachThroughAttacks")]
        public string ReachThroughAttacks { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("taunts")]
        public string Taunts { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("upgradedGeneralRepairSound0")]
        public string UpgradedGeneralRepairSound0 { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("upgradedGeneralRepairSound1")]
        public string UpgradedGeneralRepairSound1 { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("useDelayBetweenGeneralRepSounds")]
        public string UseDelayBetweenGeneralRepSounds { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("zombieReachThroughAnimState")]
        public string ZombieReachThroughAnimState { get; set; }

        /// <summary>
        ///
        /// </summary>
        [GameDataTable.Property("zombieTauntAnimState")]
        public string ZombieTauntAnimState { get; set; }
    }
}
