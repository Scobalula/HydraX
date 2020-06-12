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
// File: Mathematics/Matrixs/Matrix.cs
// Author: Philip/Scobalula
// Description: A class to hold a Matrix
using System;

namespace PhilLibX.Mathematics
{
    /// <summary>
    /// A class to hold and manipulate a Matrix
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// Gets or Sets the Values
        /// </summary>
        public float[,] Values { get; set; }

        /// <summary>
        /// Initializes a Matrix of the given dimension
        /// </summary>
        public Matrix(int dimension)
        {
            Values = new float[dimension, dimension];
        }

        /// <summary>
        /// Subtracts two given Matrixs
        /// </summary>
        public static Matrix operator -(Matrix a, Matrix b)
        {
            var result = new Matrix(a.Values.GetLength(0));

            for(int x = 0; x < a.Values.GetLength(0); x++)
            {
                for(int y = 0; y < a.Values.GetLength(1); y++)
                {
                    result.Values[x, y] = a.Values[x, y] - b.Values[x, y];
                }
            }

            return result;
        }

        /// <summary>
        /// Subtracts the given value from the Matrix
        /// </summary>
        public static Matrix operator -(Matrix a, float value)
        {
            var result = new Matrix(a.Values.GetLength(0));

            for (int x = 0; x < a.Values.GetLength(0); x++)
            {
                for (int y = 0; y < a.Values.GetLength(1); y++)
                {
                    result.Values[x, y] = a.Values[x, y] - value;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds two given Matrixs
        /// </summary>
        public static Matrix operator +(Matrix a, Matrix b)
        {
            var result = new Matrix(a.Values.GetLength(0));

            for (int x = 0; x < a.Values.GetLength(0); x++)
            {
                for (int y = 0; y < a.Values.GetLength(1); y++)
                {
                    result.Values[x, y] = a.Values[x, y] + b.Values[x, y];
                }
            }

            return result;
        }

        /// <summary>
        /// Adds the given value to the Matrix
        /// </summary>
        public static Matrix operator +(Matrix a, float value)
        {
            var result = new Matrix(a.Values.GetLength(0));

            for (int x = 0; x < a.Values.GetLength(0); x++)
            {
                for (int y = 0; y < a.Values.GetLength(1); y++)
                {
                    result.Values[x, y] = a.Values[x, y] + value;
                }
            }

            return result;
        }

        /// <summary>
        /// Multiplies two given Matrixs
        /// </summary>
        public static Matrix operator *(Matrix a, Matrix b)
        {
            var result = new Matrix(a.Values.GetLength(0));

            for (int x = 0; x < a.Values.GetLength(0); x++)
            {
                for (int y = 0; y < a.Values.GetLength(1); y++)
                {
                    result.Values[x, y] = a.Values[x, y] * b.Values[x, y];
                }
            }

            return result;
        }

        /// <summary>
        /// Multiplies the Matrix by the given value
        /// </summary>
        public static Matrix operator *(Matrix a, float value)
        {
            var result = new Matrix(a.Values.GetLength(0));

            for (int x = 0; x < a.Values.GetLength(0); x++)
            {
                for (int y = 0; y < a.Values.GetLength(1); y++)
                {
                    result.Values[x, y] = a.Values[x, y] * value;
                }
            }

            return result;
        }

        /// <summary>
        /// Divides two given Matrixs
        /// </summary>
        public static Matrix operator /(Matrix a, Matrix b)
        {
            var result = new Matrix(a.Values.GetLength(0));

            for (int x = 0; x < a.Values.GetLength(0); x++)
            {
                for (int y = 0; y < a.Values.GetLength(1); y++)
                {
                    result.Values[x, y] = a.Values[x, y] / b.Values[x, y];
                }
            }

            return result;
        }

        /// <summary>
        /// Divides the Matrix by the given value
        /// </summary>
        public static Matrix operator /(Matrix a, float value)
        {
            var result = new Matrix(a.Values.GetLength(0));

            for (int x = 0; x < a.Values.GetLength(0); x++)
            {
                for (int y = 0; y < a.Values.GetLength(1); y++)
                {
                    result.Values[x, y] = a.Values[x, y] / value;
                }
            }

            return result;
        }

        /// <summary>
        /// Transforms the Vector by the Matrix
        /// </summary>
        /// <param name="vector">Vector To Transform</param>
        /// <returns>Resulting Vector</returns>
        public Vector3 TransformVector(Vector3 vector)
        {
            return new Vector3(
                vector.DotProduct(new Vector3(Values[0, 0], Values[0, 1], Values[0, 2])),
                vector.DotProduct(new Vector3(Values[1, 0], Values[1, 1], Values[1, 2])),
                vector.DotProduct(new Vector3(Values[2, 0], Values[2, 1], Values[2, 2]))
                );
        }

        /// <summary>
        /// Gets a string representation of the Matrix
        /// </summary>
        /// <returns>String representation of the Matrix</returns>
        public override string ToString()
        {
            var result = "";

            for (int x = 0; x < Values.GetLength(0); x++)
            {
                for (int y = 0; y < Values.GetLength(1); y++)
                {
                    result += Values[x, y].ToString() + " ";
                }

                result += "\n";
            }

            return result;
        }
    }
}
