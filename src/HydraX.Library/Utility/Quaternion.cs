using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraX.Library.Utility
{
    /// <summary>
    /// Holds Quaternion Rotation Data
    /// </summary>
    public class Quaternion
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
        /// W Component
        /// </summary>
        public double W { get; set; }

        /// <summary>
        /// Converts a Quaternion to a 3x3 Matrix
        /// </summary>
        /// <returns>Resulting matrix</returns>
        public Matrix ToMatrix()
        {
            // http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToMatrix/index.htm
            Matrix matrix = new Matrix();

            double tempVar1;
            double tempVar2;

            double xSquared = X * X;
            double ySquared = Y * Y;
            double zSquared = Z * Z;
            double wSquared = W * W;

            double inverse = 1 / (xSquared + ySquared + zSquared + wSquared);

            matrix.X[0] = Math.Round((xSquared - ySquared - zSquared + wSquared) * inverse, 4);
            matrix.Y[1] = Math.Round((-xSquared + ySquared - zSquared + wSquared) * inverse, 4);
            matrix.Z[2] = Math.Round((-xSquared - ySquared + zSquared + wSquared) * inverse, 4);

            tempVar1 = (X * Y);
            tempVar2 = (Z * W);

            matrix.Y[0] = Math.Round(2.0 * (tempVar1 + tempVar2) * inverse, 4);
            matrix.X[1] = Math.Round(2.0 * (tempVar1 - tempVar2) * inverse, 4);

            tempVar1 = (X * Z);
            tempVar2 = (Y * W);

            matrix.Z[0] = Math.Round(2.0 * (tempVar1 - tempVar2) * inverse, 4);
            matrix.X[2] = Math.Round(2.0 * (tempVar1 + tempVar2) * inverse, 4);

            tempVar1 = (Y * Z);
            tempVar2 = (X * W);

            matrix.Z[1] = Math.Round(2.0 * (tempVar1 + tempVar2) * inverse, 4);
            matrix.Y[2] = Math.Round(2.0 * (tempVar1 - tempVar2) * inverse, 4);

            return matrix;
        }
    }
}
