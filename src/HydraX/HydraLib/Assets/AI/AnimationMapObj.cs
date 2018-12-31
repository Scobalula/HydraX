using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Assets
{
    /// <summary>
    /// Animation Map Object
    /// </summary>
    public class AnimationMapObj
    {
        /// <summary>
        /// Name of Map
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Animations within this Map
        /// </summary>
        public string[] Animations { get; set; }

        /// <summary>
        /// Creates a new Animation Map
        /// </summary>
        /// <param name="name">Map Name</param>
        /// <param name="animCount">Animation Count</param>
        public AnimationMapObj(string name, int animCount)
        {
            Name = name;
            Animations = new string[animCount];
        }
    }
}
