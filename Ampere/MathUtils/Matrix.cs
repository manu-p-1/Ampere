using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ampere.Base;
using Ampere.Base.Attributes;

namespace Ampere.MathUtils
{
    /// <summary>
    /// A class representing a mathematical Matrix. Creates a rectangular
    /// array of rows and columns with numbers as elements. The Matrix
    /// class includes mathematical matrix operations to manipulate it.
    /// </summary>
    [Beta]
    public class Matrix : IMatrixer, IIndexableDouble<int, double>
    {
        private const double Tolerance = 0.000000000001;

        /// <inheritdoc cref="IMatrixer"/>
        public double[,] Values { get; }

        /// <inheritdoc cref="IMatrixer"/>
        public int Rows { get; }

        /// <inheritdoc cref="IMatrixer"/>
        public int Cols { get; }

        /// <summary>
        /// Creates an instance of a Matrix given rows and columns.
        /// </summary>
        /// <param name="rows">The number of rows in this Matrix</param>
        /// <param name="cols">The number of columns in this Matrix</param>
        public Matrix(int rows, int cols)
        {
            Values = new double[rows, cols];
            Rows = rows;
            Cols = cols;
        }

        /// <summary>
        /// Creates an instance of a Matrix given a 2D array. 
        /// </summary>
        /// <param name="matrix">A 2D array of doubles</param>
        public Matrix(double[,] matrix)
        {
            matrix = matrix ?? throw new ArgumentNullException(nameof(matrix));
            Values = matrix;
            Rows = matrix.GetLength(0);
            Cols = matrix.GetLength(1);
        }

        /// <summary>
        /// The indexer to add values for each row and column.
        /// </summary>
        /// <param name="row">The row to insert the value</param>
        /// <param name="col">The column to insert the value</param>
        /// <returns></returns>
        public double this[int row, int col]
        {
            get => Values[row, col];
            set => Values[row, col] = value;
        }

        /// <inheritdoc cref="IMatrixer"/>
        public Matrix Transpose()
        {
            var cp = new Matrix(Cols, Rows);

            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Cols; c++)
                {
                    cp[c, r] = Values[r, c];
                }
            }
            return cp;
        }

        /// <summary>
        /// Returns whether two <see cref="Matrix"/> instances are of the same dimension.
        /// Same dimension means that the rows and the columns of both instances are the same. This
        /// overload is an instance method of the Matrix class.
        /// </summary>
        /// <param name="otherMatrix"></param>
        /// <returns></returns>
        public bool EqualDimension(IMatrixer otherMatrix)
        {
            return this.Rows == otherMatrix.Rows && this.Cols == otherMatrix.Cols;
        }

        private protected static Matrix DoTwoMatrixScalar(Matrix one, Matrix two, Func<double, double, double> action)
        {
            var n = new Matrix(one.Rows, two.Cols);

            if (!IMatrixer.EqualDimension(one, two))
            {
                throw new MatrixPropertyException("Both matrices must be of equal dimensions");
            }

            for (var r = 0; r < one.Rows; r++)
            {
                for (var c = 0; c < two.Cols; c++)
                {
                    n[r, c] = action(one[r, c], two[r, c]);
                }
            }
            return n;
        }

        private protected static Matrix DoScalar(Matrix m, double? sc, Func<double, double?, double> action)
        {
            for (var r = 0; r < m.Rows; r++)
            {
                for (var c = 0; c < m.Cols; c++)
                {
                    m[r, c] = action(m[r, c], sc);
                }
            }
            return m;
        }

        /// <summary>
        /// An operator method to add a matrix instance with a scalar.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix operator +(Matrix m, double scalar) => DoScalar(m, scalar, (val, sc) => val + (double)sc);

        /// <summary>
        /// An operator method to add a matrix instance with a scalar.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix operator +(double scalar, Matrix m) => DoScalar(m, scalar, (val, sc) => val + (double)sc);

        /// <summary>
        /// An operator method to subtract a matrix instance with a scalar.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix operator -(Matrix m, double scalar) => DoScalar(m, scalar, (val, sc) => val - (double)sc);

        /// <summary>
        /// An operator method to subtract a matrix instance with a scalar.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix operator -(double scalar, Matrix m) => DoScalar(m, scalar, (val, sc) => val - (double)sc);

        /// <summary>
        /// An operator method to multiply a matrix instance with a scalar.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix operator *(Matrix m, double scalar) => DoScalar(m, scalar, (val, sc) => val * (double)sc);

        /// <summary>
        /// An operator method to multiply a matrix instance with a scalar.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix operator *(double scalar, Matrix m) => DoScalar(m, scalar, (val, sc) => val * (double)sc);

        /// <summary>
        /// An operator method to divide a matrix instance with a scalar. This may result in a Divide By Zero exception
        /// if the matrix contains a value of 0.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix operator /(Matrix m, double scalar) => DoScalar(m, scalar, (val, sc) => val / (double)sc);

        /// <summary>
        /// An operator method to divide a matrix instance with a scalar. This may result in a Divide By Zero exception
        /// if the matrix contains a value of 0.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix operator /(double scalar, Matrix m) => DoScalar(m, scalar, (sc, val) => sc / (double)val);

        /// <summary>
        /// An operator method to mod a matrix instance with a scalar.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix operator %(Matrix m, double scalar) => DoScalar(m, scalar, (val, sc) => val % (double)sc);

        /// <summary>
        /// An operator method to mod a matrix instance with a scalar.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix operator %(double scalar, Matrix m) => DoScalar(m, scalar, (sc, val) => sc % (double)val);

        /// <summary>
        /// An operator method to negate a matrix. Every value in the Matrix provided will be negated.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix operator !(Matrix m) => DoScalar(m, null, (val, sc) => val - (val * 2));

        /// <summary>
        /// An operator method to add two Matrix instances.
        /// </summary>
        /// <param name="one">The first Matrix instance</param>
        /// <param name="two">The second Matrix instance</param>
        /// <returns>A new Matrix instance containing the added values</returns>
        public static Matrix operator +(Matrix one, Matrix two)
        {
            one = one ?? throw new ArgumentNullException(nameof(one));
            two = two ?? throw new ArgumentNullException(nameof(two));

            return DoTwoMatrixScalar(one, two, (ac, ac2) => ac + ac2);
        }

        /// <summary>
        /// An operator method to subtract two Matrix instances.
        /// </summary>
        /// <param name="one">The first Matrix instance</param>
        /// <param name="two">The second Matrix instance</param>
        /// <returns>A new Matrix instance containing the added values</returns>
        public static Matrix operator -(Matrix one, Matrix two)
        {
            one = one ?? throw new ArgumentNullException(nameof(one));
            two = two ?? throw new ArgumentNullException(nameof(two));

            return DoTwoMatrixScalar(one, two, (ac, ac2) => ac - ac2);
        }

        /// <summary>
        /// An operator method to compute the dot product of two Matrix instances.
        /// </summary>
        /// <param name="one">The first Matrix instance</param>
        /// <param name="two">The second Matrix instance</param>
        /// <returns>A new Matrix instance containing dot product</returns>
        public static Matrix operator *(Matrix one, Matrix two)
        {
            one = one ?? throw new ArgumentNullException(nameof(one));
            two = two ?? throw new ArgumentNullException(nameof(two));

            var cp = new Matrix(one.Rows, two.Cols);

            if (one.Cols != two.Rows)
            {
                throw new MatrixPropertyException($"Found: Matrix one columns: {one.Cols}, Matrix Two rows: {one.Cols} "
                                                  + "but required Matrix one columns == Matrix two rows");
            }

            for (var i = 0; i < one.Rows; i++)
            {
                for (var j = 0; j < two.Cols; j++)
                {
                    for (var k = 0; k < one.Cols; k++)
                    {
                        cp[i, j] += one[i, k] * two[k, j];
                    }
                }
            }
            return cp;
        }

        /// <summary>
        /// An operator method to compare two Matrix instances for equality. Equality is defined by structure and values, meaning that
        /// both matrices must have an equal dimension and every value in both matrices must be equal.
        /// </summary>
        /// <param name="one">The first Matrix instance</param>
        /// <param name="two">The second Matrix instance</param>
        /// <returns>A bool representing whether the two Matrix instances are equal</returns>
        public static bool operator ==(Matrix one, Matrix two)
        {
            if(one is null || two is null)
            {
                return false;
            }
            if (!IMatrixer.EqualDimension(one, two))
                return false;

            for(var i = 0; i < one.Rows; i++)
            {
                for(var j = 0; j < one.Cols; j++)
                {
                    if (Math.Abs(one[i, j] - two[i, j]) > Tolerance) 
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// An operator method to compare two Matrix instances for equality. Equality is defined by structure and values, meaning that
        /// both matrices must have an equal dimension and every value in both matrices must be equal.
        /// </summary>
        /// <param name="one">The first Matrix instance</param>
        /// <param name="two">The second Matrix instance</param>
        /// <returns>A new Matrix instance containing the added values</returns>
        public static bool operator !=(Matrix one, Matrix two)
        {
            if (one is null || two is null)
            {
                return true;
            }
            if (!IMatrixer.EqualDimension(one, two))
                return true;

            for (var i = 0; i < one.Rows; i++)
            {
                for (var j = 0; j < one.Cols; j++)
                {
                    if (Math.Abs(one[i, j] - two[i, j]) > Tolerance)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// A wrapper for the operator method to add a matrix instance with a scalar.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix Add(Matrix m, double scalar)
           => m + scalar;

        /// <summary>
        /// A wrapper for the operator method to add two Matrix instances.
        /// </summary>
        /// <param name="one">The first Matrix instance</param>
        /// <param name="two">The second Matrix instance</param>
        /// <returns>A new Matrix instance containing the added values</returns>
        public static Matrix Add(Matrix one, Matrix two)
           => one + two;

        /// <summary>
        /// A wrapper for the operator method to subtract a matrix instance with a scalar.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix Subtract(Matrix m, double scalar)
            => m - scalar;

        /// <summary>
        /// A wrapper for the operator method to subtract two Matrix instances.
        /// </summary>
        /// <param name="one">The first Matrix instance</param>
        /// <param name="two">The second Matrix instance</param>
        /// <returns>A new Matrix instance containing the added values</returns>
        public static Matrix Subtract(Matrix one, Matrix two)
            => one - two;

        /// <summary>
        /// A wrapper for the operator method to multiply a matrix instance with a scalar.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix Multiply(Matrix m, double scalar)
            => m * scalar;

        /// <summary>
        /// A wrapper for the operator method to compute the dot product of two Matrix instances.
        /// </summary>
        /// <param name="one">The first Matrix instance</param>
        /// <param name="two">The second Matrix instance</param>
        /// <returns>A new Matrix instance containing dot product</returns>
        public static Matrix DotProduct(Matrix one, Matrix two)
            => one * two;

        /// <summary>
        /// A wrapper for the operator method to divide a matrix instance with a scalar. This may result in a Divide By Zero exception
        /// if the matrix contains a value of 0.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix Divide(Matrix m, double scalar)
            => m / scalar;

        /// <summary>
        /// A wrapper for the operator method to mod a matrix instance with a scalar.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <param name="scalar">The scalar value</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix Mod(Matrix m, double scalar)
            => m % scalar;

        /// <summary>
        /// A wrapper for the operator method to negate a matrix. Every value in the Matrix provided will be negated.
        /// </summary>
        /// <param name="m">The Matrix instance</param>
        /// <returns>The Matrix after the scalar has been applied</returns>
        public static Matrix Negate(Matrix m)
            => !m;

        /// <summary>
        /// Compares an object to a Matrix instance. This will check for type first then calls the
        /// <see cref="Equals(Matrix)"/> method.
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>A bool representing whether the object is equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Matrix) obj);
        }

        /// <summary>
        /// A wrapper method for operator== method.
        /// </summary>
        /// <param name="other">The Matrix instance to check for equality</param>
        /// <returns>A bool representing whether the Matrix instance is equal</returns>
        public bool Equals(Matrix other)
        {
            return this == other;
        }

        /// <inheritdoc cref="Object"/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Values, Rows, Cols);
        }

        /// <summary>
        /// Returns whether the number of Rows equals 1
        /// </summary>
        /// <returns>A bool specifying whether the number of Rows equals 1</returns>
        public bool IsRowVector() => Rows == 1;

        /// <summary>
        /// Returns whether the number of Columns equals 1.
        /// </summary>
        /// <returns>A bool specifying whether the number of Columns equals 1</returns>
        public bool IsColumnVector() => Cols == 1;

        /// <summary>
        /// Returns whether the number of Rows equals the number of Columns.
        /// </summary>
        /// <returns>A bool specifying whether the number of Rows equals the number of columns</returns>
        public bool IsSquareVector() => Rows == Cols;

        /// <summary>
        /// Returns an enumerator this Matrix containing every value. The iteration occurs through each row
        /// in the matrix.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> instance for the Matrix</returns>
        public IEnumerator<double> GetEnumerator()
        {
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Cols; c++)
                {
                    yield return Values[Rows, Cols];
                }
            }
        }

        /// <summary>
        /// Returns the non-generic Enumerator for the Matrix. This returns the generic <see cref="GetEnumerator"/> method
        /// under the hood.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> instance for the Matrix</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc cref="IMatrixer"/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Cols; j++)
                {
                    sb.Append($"{Values[i, j]}\t");
                }
                sb.Append(Environment.NewLine + Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}