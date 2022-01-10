using System;
using System.Collections.Generic;

namespace Ampere.AmpMath
{
    /// <summary>
    /// Represents the minimum requirements to create a Matrix.
    /// </summary>
    public interface IMatrixer : IEnumerable<double>
    {
        /// <summary>
        /// Property representing the values of the IMatrixer as a generic 2D array.
        /// </summary>
        public double[,] Values { get; }

        /// <summary>
        /// Property for the number of Rows in and IMatrixer.
        /// </summary>
        public int Rows { get; }

        /// <summary>
        /// Property for the number of columns in an IMatrixer.
        /// </summary>
        public int Cols { get; }

        /// <summary>
        /// Transposes the contents of the Matrix and returns a new Matrix.
        /// </summary>
        /// <returns>A new Matrix containing the transposed version of the original</returns>
        public Matrix Transpose();

        /// <summary>
        /// Returns whether this instance is the same dimension as another IMatrixer instance.
        /// Same dimension means that the rows and the columns of both instances are the same.
        /// </summary>
        /// <returns>A new Matrix containing the transposed version of the original</returns>
        public bool EqualDimension(IMatrixer otherMatrix);

        /// <summary>
        /// Returns whether two <see cref="Matrix"/> instances are of the same dimension.
        /// Same dimension means that the rows and the columns of both instances are the same.
        /// </summary>
        /// <param name="one">The first IMatrixer instance</param>
        /// <param name="two">The second IMatrixer instance</param>
        /// <returns></returns>
        public static bool EqualDimension(IMatrixer one, IMatrixer two)
        {
            one = one ?? throw new ArgumentNullException(nameof(one));
            two = two ?? throw new ArgumentNullException(nameof(two));
            return one.Rows == two.Rows && one.Cols == two.Cols;
        }

        /// <summary>
        /// Returns whether the number of Rows equals 1
        /// </summary>
        /// <returns>A bool specifying whether the number of Rows equals 1</returns>
        public bool IsRowVector();

        /// <summary>
        /// Returns whether the number of Columns equals 1.
        /// </summary>
        /// <returns>A bool specifying whether the number of Columns equals 1</returns>
        public bool IsColumnVector();

        /// <summary>
        /// Returns whether the number of Rows equals the number of Columns.
        /// </summary>
        /// <returns>A bool specifying whether the number of Rows equals the number of columns</returns>
        public bool IsSquareVector();

        /// <summary>
        /// Returns a string representation of an IMatrixer.
        /// </summary>
        /// <returns>A string representation of the IMatrixer</returns>
        public string ToString();
    }
}
