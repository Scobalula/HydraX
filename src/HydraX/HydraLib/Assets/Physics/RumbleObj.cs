using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Assets
{
    class RumbleObj
    {
        /// <summary>
        /// High Rumble File Name
        /// </summary>
        [GameDataTable.Property("highRumbleFile")]
        public string HighRumble { get; set; }

        /// <summary>
        /// Low Rumble File Name
        /// </summary>
        [GameDataTable.Property("lowRumbleFile")]
        public string LowRumble { get; set; }

        /// <summary>
        /// Rumble Duration
        /// </summary>
        [GameDataTable.Property("duration")]
        public float Duration { get; set; }

        /// <summary>
        /// Rumble Range
        /// </summary>
        [GameDataTable.Property("range")]
        public float Range { get; set; }

        /// <summary>
        /// Rumble Camera Shake Range
        /// </summary>
        [GameDataTable.Property("camShakeRange")]
        public float CamShakeRange { get; set; }

        /// <summary>
        /// Rumble Camera Shake Scale
        /// </summary>
        [GameDataTable.Property("camShakeScale")]
        public float CamShakeScale { get; set; }

        /// <summary>
        /// Rumble Camera Shake Duration
        /// </summary>
        [GameDataTable.Property("camShakeDuration")]
        public float CamShakeDuration { get; set; }

        /// <summary>
        /// Rumble Pulse Scale
        /// </summary>
        [GameDataTable.Property("pulseScale")]
        public float PulseScale { get; set; }

        /// <summary>
        /// Rumble Outer Pulse Radius
        /// </summary>
        [GameDataTable.Property("pulseRadiusOuter")]
        public float PulseRadiusOuter { get; set; }

        /// <summary>
        /// Should this Rumble Fade With Distance
        /// </summary>
        [GameDataTable.Property("fadeWithDistance")]
        public int FadeWithDistance { get; set; }

        /// <summary>
        /// Should this Rumble Broadcast
        /// </summary>
        [GameDataTable.Property("broadcast")]
        public int Broadcast { get; set; }
    }
}
