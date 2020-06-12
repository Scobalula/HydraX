// ------------------------------------------------------------------------
// PhilLibX - My Utility Library
// Copyright(c) 2018 Philip/Scobalula
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ------------------------------------------------------------------------
// File: Mathematics/Quaternions/cs
// Author: Philip/Scobalula
// Description: A class to hold a Quaternion
using System;

namespace PhilLibX.Mathematics
{
    /// <summary>
    /// A class to hold and manipulate a Quaternion
    /// </summary>
    public struct Quaternion
    {
        /// <summary>
        /// X Value
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y Value
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Z Value
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// W Value
        /// </summary>
        public float W { get; set; }

        /// <summary>
        /// Initializes a 4-Dimensional Quaternion with the given values
        /// </summary>
        /// <param name="x">X Value</param>
        /// <param name="y">Y Value</param>
        /// <param name="z">Z Value</param>
        /// <param name="w">W Value</param>
        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Subtracts two given Quaternions
        /// </summary>
        public static Quaternion operator -(Quaternion a, Quaternion b)
        {
            return new Quaternion(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        /// <summary>
        /// Subtracts the given value from the Quaternion
        /// </summary>
        public static Quaternion operator -(Quaternion a, float value)
        {
            return new Quaternion(a.X - value, a.Y - value, a.Z - value, a.W - value);
        }

        /// <summary>
        /// Adds two given Quaternions
        /// </summary>
        public static Quaternion operator +(Quaternion a, Quaternion b)
        {
            return new Quaternion(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        /// <summary>
        /// Adds the given value to the Quaternion
        /// </summary>
        public static Quaternion operator +(Quaternion a, float value)
        {
            return new Quaternion(a.X + value, a.Y + value, a.Z + value, a.W + value);
        }

        /// <summary>
        /// Multiplies two given Quaternions
        /// </summary>
        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return new Quaternion(
                a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
                a.W * b.Y + a.Y * b.W + a.Z * b.X - a.X * b.Z,
                a.W * b.Z + a.Z * b.W + a.X * b.Y - a.Y * b.X,
                a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z);
        }

        /// <summary>
        /// Multiplies the Quaternion by the given value
        /// </summary>
        public static Quaternion operator *(Quaternion a, float value)
        {
            return new Quaternion(a.X * value, a.Y * value, a.Z * value, a.W * value);
        }

        /// <summary>
        /// Divides two given Quaternions
        /// </summary>
        public static Quaternion operator /(Quaternion a, Quaternion b)
        {
            return new Quaternion(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        }

        /// <summary>
        /// Divides the Quaternion by the given value
        /// </summary>
        public static Quaternion operator /(Quaternion a, float value)
        {
            return new Quaternion(a.X / value, a.Y / value, a.Z / value, a.W / value);
        }

        /// <summary>
        /// Returns an inverse of the Quaternion
        /// </summary>
        public Quaternion Inverse()
        {
            return new Quaternion(-X, -Y, -Z, W);
        }

        /// <summary>
        /// Returns the linear interpolation of Quat with coefficient
        /// </summary>
        /// <param name="max">Max/End Value</param>
        /// <param name="coefficient">Coefficient</param>
        /// <returns>Resulting Quat</returns>
        public Quaternion SLerp(Quaternion max, float coefficient)
        {
            // Calculate angle between them.
            float cos_half_theta = X * max.X + Y * max.Y + Z * max.Z + W * max.W;

            // If _a=_b or _a=-_b then theta = 0 and we can return _a.
            if (Math.Abs(cos_half_theta) >= .999f)
                return this;

            // Calculate temporary values.
            float half_theta = (float)Math.Acos(cos_half_theta);
            float sin_half_theta = (float)Math.Sqrt(1.0f - cos_half_theta * cos_half_theta);

            if (sin_half_theta < .001f)
            {
                return new Quaternion(
                    (X + max.X) * .5f, (Y + max.Y) * .5f,
                    (Z + max.Z) * .5f, (W + max.W) * .5f);
            }

            float ratio_a = (float)Math.Sin((1.0f - coefficient) * half_theta) / sin_half_theta;
            float ratio_b = (float)Math.Sin(coefficient * half_theta) / sin_half_theta;

            // Calculate Quaternion.
            return new Quaternion(
                ratio_a * X + ratio_b * max.X, ratio_a * Y + ratio_b * max.Y,
                ratio_a * Z + ratio_b * max.Z, ratio_a * W + ratio_b * max.W);
        }

        /// <summary>
        /// Returns the linear interpolation of Quat with coefficient
        /// </summary>
        /// <param name="max">Max/End Value</param>
        /// <param name="coefficient">Coefficient</param>
        /// <returns>Resulting Quat</returns>
        public Quaternion Lerp(Quaternion max, float coefficient)
        {
            return new Quaternion(
                ((max.X - X) * coefficient) + X,
                ((max.Y - Y) * coefficient) + Y,
                ((max.Z - Z) * coefficient) + Z,
                ((max.W - W) * coefficient) + W);
        }

        /// <summary>
        /// Converts the Quaternion to a 3-Dimensional Matrix
        /// </summary>
        /// <returns>3-D Matrix</returns>
        public Matrix ToMatrix()
        {
            // https://en.wikipedia.org/wiki/Quaternionsand_spatial_rotation#Quaternion-derived_rotation_matrix
            var result = new Matrix(3);

            var xx = X * X;
            var yy = Y * Y;
            var zz = Z * Z;
            var xy = X * Y;
            var xz = X * Z;
            var xw = X * W;
            var yz = Y * Z;
            var yw = Y * W;
            var zw = Z * W;


            result.Values[0, 0] = 1 - 2 * (yy + zz);
            result.Values[0, 1] = 2 * (xy - zw);
            result.Values[0, 2] = 2 * (xz + yw);

            result.Values[1, 0] = 2 * (xy + zw);
            result.Values[1, 1] = 1 - 2 * (xx + zz);
            result.Values[1, 2] = 2 * (yz - xw);

            result.Values[2, 0] = 2 * (xz - yw);
            result.Values[2, 1] = 2 * (yz + xw);
            result.Values[2, 2] = 1 - 2 * (xx + yy);

            return result;
        }

        /// <summary>
        /// Converts the Quaternion to an Euler vector
        /// </summary>
        /// <returns>Euler vector</returns>
        public Vector3 ToEuler()
        {

            Vector3 result = new Vector3();

            double t0 = 2.0 * (W * X + Y * Z);
            double t1 = 1.0 - 2.0 * (X * X + Y * Y);

            result.X = (float)Math.Atan2(t0, t1);


            double t2 = 2.0 * (W * Y - Z * X);

            t2 = t2 > 1.0 ? 1.0 : t2;
            t2 = t2 < -1.0 ? -1.0 : t2;
            result.Y = (float)Math.Asin(t2);


            double t3 = +2.0 * (W * Z + X * Y);
            double t4 = +1.0 - 2.0 * (Y * Y + Z * Z);

            result.Z = (float)Math.Atan2(t3, t4);

            return result;
        }

        /// <summary>
        /// Gets a string representation of the Quaternion
        /// 
        /// </summary>
        /// <returns>String representation of the Quaternion</returns>
        public override string ToString()
        {
            return string.Format(" {0}, {1}, {2}, {3}", X, Y, Z, W);
        }
    }
}
