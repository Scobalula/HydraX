using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Utility
{
    /// <summary>
    /// Holds a 3 Dimensional Vector
    /// </summary>
    public class Vector3
    {
        /// <summary>
        /// X Component
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y Component
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Z Component
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Creates a new 3 Dimensional Vector
        /// </summary>
        public Vector3() { }

        /// <summary>
        /// Creates a new 3 Dimensional Vector with XYZ Components
        /// </summary>
        /// <param name="x">X Component</param>
        /// <param name="y">Y Component</param>
        /// <param name="z">Z Component</param>
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double[] ToArray()
        {
            return new double[] { X, Y, Z, };
        }
    }
}
