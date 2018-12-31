using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Utility
{
    /// <summary>
    /// Holds a 3x3 Matrix
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// X Components
        /// </summary>
        public Vector3 X = new Vector3();

        /// <summary>
        /// Y Components
        /// </summary>
        public Vector3 Y = new Vector3();

        /// <summary>
        /// Z Components
        /// </summary>
        public Vector3 Z = new Vector3();

        /// <summary>
        /// Returns this matrix as a formatted string.
        /// </summary>
        public override string ToString()
        {
            return String.Format
                (
                "[ {0:0.000000}, {1:0.000000}, {2:0.000000} ]\n[ {3:0.000000}, {4:0.000000}, {5:0.000000} ]\n[ {6:0.000000}, {7:0.000000}, {8:0.000000} ]",
                X.X, X.Y, X.Z, Y.X, Y.Y, Y.Z, Z.X, Z.Y, Z.Z);
        }
    }
}
