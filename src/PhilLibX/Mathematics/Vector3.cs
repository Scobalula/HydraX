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
// File: Mathematics/Vectors/Vector3.cs
// Author: Philip/Scobalula
// Description: A class to hold a 3-Dimensional Vector
using System;

namespace PhilLibX.Mathematics
{
    /// <summary>
    /// A class to hold a 3-Dimensional Vector
    /// </summary>
    public struct Vector3
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
        /// Initializes a 3-Dimensional Vector with the given values
        /// </summary>
        /// <param name="x">X Value</param>
        /// <param name="y">Y Value</param>
        /// <param name="z">Z Value</param>
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Returns the dot product of the 2 vectors
        /// </summary>
        /// <param name="input">Input vector</param>
        /// <returns>Single result</returns>
        public float DotProduct(Vector3 input)
        {
            return (X * input.X) + (Y * input.Y) + (Z * input.Z);
        }

        /// <summary>
        /// Returns the cross product of the 2 vectors
        /// </summary>
        /// <param name="input">Input vector</param>
        /// <returns>Single result</returns>
        public Vector3 CrossProduct(Vector3 input)
        {
            float a = Y * input.Z - Z * input.Y;
            float b = Z * input.X - X * input.Z;
            float c = X * input.Y - Y * input.X;

            return new Vector3(a, b, c);
        }

        /// <summary>
        /// Subtracts two given vectors
        /// </summary>
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Subtracts the given value from the vector
        /// </summary>
        public static Vector3 operator -(Vector3 a, float value)
        {
            return new Vector3(a.X - value, a.Y - value, a.Z - value);
        }

        /// <summary>
        /// Adds two given vectors
        /// </summary>
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// Adds the given value to the vector
        /// </summary>
        public static Vector3 operator +(Vector3 a, float value)
        {
            return new Vector3(a.X + value, a.Y + value, a.Z + value);
        }

        /// <summary>
        /// Multiplies two given vectors
        /// </summary>
        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        /// <summary>
        /// Multiplies the vector by the given value
        /// </summary>
        public static Vector3 operator *(Vector3 a, float value)
        {
            return new Vector3(a.X * value, a.Y * value, a.Z * value);
        }

        /// <summary>
        /// Divides two given vectors
        /// </summary>
        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        /// <summary>
        /// Divides the vector by the given value
        /// </summary>
        public static Vector3 operator /(Vector3 a, float value)
        {
            return new Vector3(a.X / value, a.Y / value, a.Z / value);
        }

        /// <summary>
        /// Normalizes the Vector
        /// </summary>
        /// <returns>Normalized Vector</returns>
        public Vector3 Normalize()
        {
            float length = (float)Math.Sqrt(DotProduct(this));
            return new Vector3(X / length, Y / length, Z / length);
        }

        /// <summary>
        /// Returns the linear interpolation of Vector with coefficient
        /// </summary>
        /// <param name="max">Max/End Value</param>
        /// <param name="coefficient">Coefficient</param>
        /// <returns>Resulting Vector</returns>
        public Vector3 Lerp(Vector3 max, float coefficient)
        {
            return new Vector3(
                (max.X - X) * coefficient + X, 
                (max.Y - Y) * coefficient + Y,
                (max.Z - Z) * coefficient + Z);
        }

        /// <summary>
        /// Gets a string representation of the vector
        /// </summary>
        /// <returns>String representation of the vector</returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", X, Y, Z);
        }
    }
}
