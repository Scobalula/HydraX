using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Assets
{
    public class PhysicsPresetObj
    {
        /// <summary>
        /// If this entity can float
        /// </summary>
        [GameDataTable.Property("canFloat")]
        public int CanFloat { get; set; }

        /// <summary>
        /// Object Mass in Pounds
        /// </summary>
        [GameDataTable.Property("mass")]
        public double Mass { get; set; }

        /// <summary>
        /// How much this object will bounce
        /// </summary>
        [GameDataTable.Property("bounce")]
        public double Bounce { get; set; }

        /// <summary>
        /// Object Friction
        /// </summary>
        [GameDataTable.Property("friction")]
        public double Friction { get; set; }

        /// <summary>
        /// Linear Damping Scale
        /// </summary>
        [GameDataTable.Property("damping_linear")]
        public double LinearDamping { get; set; }

        /// <summary>
        /// Angular Damping Scale
        /// </summary>
        [GameDataTable.Property("damping_angular")]
        public double AngularDamping { get; set; }

        /// <summary>
        /// Bullet Force Scale
        /// </summary>
        [GameDataTable.Property("bulletForceScale")]
        public double BulletForceScale { get; set; }

        /// <summary>
        /// Explisve Force Scale
        /// </summary>
        [GameDataTable.Property("explosiveForceScale")]
        public double ExplosiveForceScale { get; set; }

        /// <summary>
        /// Graity Scale
        /// </summary>
        [GameDataTable.Property("gravityScale")]
        public double GravityScale { get; set; }

        /// <summary>
        /// Object Impact Effects
        /// </summary>
        [GameDataTable.Property("impactsFxTable")]
        public string ImpactFXTable { get; set; }

        /// <summary>
        /// Object Impact Sounds
        /// </summary>
        [GameDataTable.Property("impactsSoundsTable")]
        public string ImpactSoundTable { get; set; }

        /// <summary>
        /// Trail FX Played on Object
        /// </summary>
        [GameDataTable.Property("trailFX")]
        public string TrailFX { get; set; }

        /// <summary>
        /// Mass Offset
        /// </summary>
        [GameDataTable.Property("massOffsetX")]
        public float MassOffsetX { get; set; }

        /// <summary>
        /// Mass Offset
        /// </summary>
        [GameDataTable.Property("massOffsetY")]
        public float MassOffsetY { get; set; }

        /// <summary>
        /// Mass Offset
        /// </summary>
        [GameDataTable.Property("massOffsetZ")]
        public float MassOffsetZ { get; set; }

        /// <summary>
        /// Buoyancy Force Min
        /// </summary>
        [GameDataTable.Property("buoyancyMinX")]
        public float BuoyancyMinX { get; set; }

        /// <summary>
        /// Buoyancy Force Min
        /// </summary>
        [GameDataTable.Property("buoyancyMinY")]
        public float BuoyancyMinY { get; set; }

        /// <summary>
        /// Buoyancy Force Min
        /// </summary>
        [GameDataTable.Property("buoyancyMinZ")]
        public float BuoyancyMinZ { get; set; }

        /// <summary>
        /// Buoyancy Force Max
        /// </summary>
        [GameDataTable.Property("buoyancyMaxX")]
        public float BuoyancyMaxX { get; set; }

        /// <summary>
        /// Buoyancy Force Max
        /// </summary>
        [GameDataTable.Property("buoyancyMaxY")]
        public float BuoyancyMaxY { get; set; }

        /// <summary>
        /// Buoyancy Force Max
        /// </summary>
        [GameDataTable.Property("buoyancyMaxZ")]
        public float BuoyancyMaxZ { get; set; }
    }
}
