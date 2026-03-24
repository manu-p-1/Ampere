using Ampere.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace Ampere.AmpMath;

/// <summary>
/// A class representing a mathematical Matrix. Creates a rectangular
/// array of rows and columns with numbers as elements. The Matrix
/// class includes mathematical matrix operations to manipulate it.
/// </summary>
public class Matrix : IMatrixer, IIndexableDouble<int, double>
{
    private const double Tolerance = 1e-12;

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
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rows);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cols);
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
        ArgumentNullException.ThrowIfNull(matrix);
        Values = matrix;
        Rows = matrix.GetLength(0);
        Cols = matrix.GetLength(1);
    }

    /// <summary>
    /// The indexer to add values for each row and column.
    /// </summary>
    /// <param name="row">The row to insert the value</param>
    /// <param name="col">The column to insert the value</param>
    /// <returns>The value at the specified row and column</returns>
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
            for (var c = 0; c < Cols; c++)
                cp[c, r] = Values[r, c];
        return cp;
    }

    /// <summary>
    /// Returns whether two <see cref="Matrix"/> instances are of the same dimension.
    /// </summary>
    /// <param name="otherMatrix">The other matrix to compare</param>
    /// <returns>True if dimensions match</returns>
    public bool EqualDimension(IMatrixer otherMatrix)
    {
        return Rows == otherMatrix.Rows && Cols == otherMatrix.Cols;
    }

    private static Matrix DoTwoMatrixScalar(Matrix one, Matrix two, Func<double, double, double> action)
    {
        if (!IMatrixer.EqualDimension(one, two))
            throw new MatrixPropertyException("Both matrices must be of equal dimensions");

        var n = new Matrix(one.Rows, one.Cols);
        for (var r = 0; r < one.Rows; r++)
            for (var c = 0; c < one.Cols; c++)
                n[r, c] = action(one[r, c], two[r, c]);
        return n;
    }

    private static Matrix DoScalar(Matrix m, double scalar, Func<double, double, double> action)
    {
        var result = new Matrix(m.Rows, m.Cols);
        for (var r = 0; r < m.Rows; r++)
            for (var c = 0; c < m.Cols; c++)
                result[r, c] = action(m[r, c], scalar);
        return result;
    }

    private static Matrix DoUnaryOp(Matrix m, Func<double, double> action)
    {
        var result = new Matrix(m.Rows, m.Cols);
        for (var r = 0; r < m.Rows; r++)
            for (var c = 0; c < m.Cols; c++)
                result[r, c] = action(m[r, c]);
        return result;
    }

    /// <summary>
    /// An operator method to add a matrix instance with a scalar.
    /// </summary>
    public static Matrix operator +(Matrix m, double scalar) => DoScalar(m, scalar, (val, sc) => val + sc);

    /// <summary>
    /// An operator method to add a scalar with a matrix instance.
    /// </summary>
    public static Matrix operator +(double scalar, Matrix m) => DoScalar(m, scalar, (val, sc) => val + sc);

    /// <summary>
    /// An operator method to subtract a scalar from a matrix instance.
    /// </summary>
    public static Matrix operator -(Matrix m, double scalar) => DoScalar(m, scalar, (val, sc) => val - sc);

    /// <summary>
    /// An operator method to subtract a matrix from a scalar (scalar - each element).
    /// </summary>
    public static Matrix operator -(double scalar, Matrix m) => DoScalar(m, scalar, (val, sc) => sc - val);

    /// <summary>
    /// An operator method to multiply a matrix instance with a scalar.
    /// </summary>
    public static Matrix operator *(Matrix m, double scalar) => DoScalar(m, scalar, (val, sc) => val * sc);

    /// <summary>
    /// An operator method to multiply a scalar with a matrix instance.
    /// </summary>
    public static Matrix operator *(double scalar, Matrix m) => DoScalar(m, scalar, (val, sc) => val * sc);

    /// <summary>
    /// An operator method to divide a matrix instance by a scalar.
    /// </summary>
    public static Matrix operator /(Matrix m, double scalar) => DoScalar(m, scalar, (val, sc) => val / sc);

    /// <summary>
    /// An operator method to divide a scalar by each element of a matrix (scalar / each element).
    /// </summary>
    public static Matrix operator /(double scalar, Matrix m) => DoScalar(m, scalar, (val, sc) => sc / val);

    /// <summary>
    /// An operator method to mod a matrix instance with a scalar.
    /// </summary>
    public static Matrix operator %(Matrix m, double scalar) => DoScalar(m, scalar, (val, sc) => val % sc);

    /// <summary>
    /// An operator method to compute scalar mod each element of a matrix (scalar % each element).
    /// </summary>
    public static Matrix operator %(double scalar, Matrix m) => DoScalar(m, scalar, (val, sc) => sc % val);

    /// <summary>
    /// An operator method to negate a matrix. Every value in the Matrix provided will be negated.
    /// Returns a new Matrix without modifying the original.
    /// </summary>
    /// <param name="m">The Matrix instance</param>
    /// <returns>A new Matrix with negated values</returns>
    public static Matrix operator !(Matrix m) => DoUnaryOp(m, val => -val);

    /// <summary>
    /// An operator method to add two Matrix instances.
    /// </summary>
    public static Matrix operator +(Matrix one, Matrix two)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(two);
        return DoTwoMatrixScalar(one, two, (a, b) => a + b);
    }

    /// <summary>
    /// An operator method to subtract two Matrix instances.
    /// </summary>
    public static Matrix operator -(Matrix one, Matrix two)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(two);
        return DoTwoMatrixScalar(one, two, (a, b) => a - b);
    }

    /// <summary>
    /// An operator method to compute the dot product of two Matrix instances.
    /// </summary>
    public static Matrix operator *(Matrix one, Matrix two)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(two);

        if (one.Cols != two.Rows)
            throw new MatrixPropertyException(
                $"Found: Matrix one columns: {one.Cols}, Matrix two rows: {two.Rows} "
                + "but required Matrix one columns == Matrix two rows");

        var cp = new Matrix(one.Rows, two.Cols);
        for (var i = 0; i < one.Rows; i++)
            for (var j = 0; j < two.Cols; j++)
                for (var k = 0; k < one.Cols; k++)
                    cp[i, j] += one[i, k] * two[k, j];
        return cp;
    }

    /// <summary>
    /// An operator method to compare two Matrix instances for equality.
    /// Equality is defined by structure and values within tolerance.
    /// </summary>
    public static bool operator ==(Matrix? one, Matrix? two)
    {
        if (one is null || two is null) return false;
        if (ReferenceEquals(one, two)) return true;
        if (!IMatrixer.EqualDimension(one, two)) return false;

        for (var i = 0; i < one.Rows; i++)
            for (var j = 0; j < one.Cols; j++)
                if (Math.Abs(one[i, j] - two[i, j]) > Tolerance)
                    return false;
        return true;
    }

    /// <summary>
    /// An operator method to compare two Matrix instances for inequality.
    /// </summary>
    public static bool operator !=(Matrix? one, Matrix? two)
    {
        return !(one == two);
    }

    /// <summary>A wrapper for the operator method to add a matrix instance with a scalar.</summary>
    public static Matrix Add(Matrix m, double scalar) => m + scalar;

    /// <summary>A wrapper for the operator method to add two Matrix instances.</summary>
    public static Matrix Add(Matrix one, Matrix two) => one + two;

    /// <summary>A wrapper for the operator method to subtract a scalar from a matrix instance.</summary>
    public static Matrix Subtract(Matrix m, double scalar) => m - scalar;

    /// <summary>A wrapper for the operator method to subtract two Matrix instances.</summary>
    public static Matrix Subtract(Matrix one, Matrix two) => one - two;

    /// <summary>A wrapper for the operator method to multiply a matrix instance with a scalar.</summary>
    public static Matrix Multiply(Matrix m, double scalar) => m * scalar;

    /// <summary>A wrapper for the operator method to compute the dot product of two Matrix instances.</summary>
    public static Matrix DotProduct(Matrix one, Matrix two) => one * two;

    /// <summary>A wrapper for the operator method to divide a matrix instance by a scalar.</summary>
    public static Matrix Divide(Matrix m, double scalar) => m / scalar;

    /// <summary>A wrapper for the operator method to mod a matrix instance with a scalar.</summary>
    public static Matrix Mod(Matrix m, double scalar) => m % scalar;

    /// <summary>A wrapper for the operator method to negate a matrix.</summary>
    public static Matrix Negate(Matrix m) => !m;

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is Matrix other && this == other;
    }

    /// <summary>
    /// A wrapper method for operator== method.
    /// </summary>
    /// <param name="other">The Matrix instance to check for equality</param>
    /// <returns>True if equal</returns>
    public bool Equals(Matrix? other) => this == other;

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Values, Rows, Cols);

    /// <inheritdoc cref="IMatrixer"/>
    public bool IsRowVector() => Rows == 1;

    /// <inheritdoc cref="IMatrixer"/>
    public bool IsColumnVector() => Cols == 1;

    /// <inheritdoc cref="IMatrixer"/>
    public bool IsSquareVector() => Rows == Cols;

    /// <summary>
    /// Creates a deep copy of this Matrix.
    /// </summary>
    /// <returns>A new Matrix with the same values</returns>
    public Matrix Clone()
    {
        var clone = new Matrix(Rows, Cols);
        Array.Copy(Values, clone.Values, Values.Length);
        return clone;
    }

    /// <summary>
    /// Creates an identity matrix of the specified size.
    /// </summary>
    /// <param name="n">The dimension of the square identity matrix</param>
    /// <returns>An n×n identity matrix</returns>
    public static Matrix Identity(int n)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(n);
        var m = new Matrix(n, n);
        for (int i = 0; i < n; i++)
            m[i, i] = 1.0;
        return m;
    }

    /// <summary>
    /// Creates a zero matrix of the specified dimensions.
    /// </summary>
    /// <param name="rows">The number of rows</param>
    /// <param name="cols">The number of columns</param>
    /// <returns>A rows×cols zero matrix</returns>
    public static Matrix Zeros(int rows, int cols) => new(rows, cols);

    /// <summary>
    /// Fills the entire matrix with the specified value.
    /// </summary>
    /// <param name="value">The value to fill with</param>
    public void Fill(double value)
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                Values[r, c] = value;
    }

    /// <summary>
    /// Applies a function to every element and returns a new Matrix with the results.
    /// </summary>
    /// <param name="func">The transformation function</param>
    /// <returns>A new Matrix with transformed values</returns>
    public Matrix Map(Func<double, double> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        return DoUnaryOp(this, func);
    }

    /// <summary>
    /// Computes the trace of the matrix (sum of diagonal elements).
    /// The matrix must be square.
    /// </summary>
    /// <returns>The trace of the matrix</returns>
    /// <exception cref="MatrixPropertyException">Thrown when the matrix is not square</exception>
    public double Trace()
    {
        if (!IsSquareVector())
            throw new MatrixPropertyException("Trace is only defined for square matrices.");

        double sum = 0;
        for (int i = 0; i < Rows; i++)
            sum += Values[i, i];
        return sum;
    }

    /// <summary>
    /// Computes the determinant of the matrix using LU decomposition.
    /// The matrix must be square.
    /// </summary>
    /// <returns>The determinant of the matrix</returns>
    /// <exception cref="MatrixPropertyException">Thrown when the matrix is not square</exception>
    public double Determinant()
    {
        if (!IsSquareVector())
            throw new MatrixPropertyException("Determinant is only defined for square matrices.");

        int n = Rows;
        if (n == 1) return Values[0, 0];
        if (n == 2) return Values[0, 0] * Values[1, 1] - Values[0, 1] * Values[1, 0];

        // LU decomposition with partial pivoting
        var lu = new double[n, n];
        Array.Copy(Values, lu, Values.Length);
        double det = 1.0;

        for (int col = 0; col < n; col++)
        {
            // Partial pivoting
            int maxRow = col;
            double maxVal = Math.Abs(lu[col, col]);
            for (int row = col + 1; row < n; row++)
            {
                if (Math.Abs(lu[row, col]) > maxVal)
                {
                    maxVal = Math.Abs(lu[row, col]);
                    maxRow = row;
                }
            }

            if (maxRow != col)
            {
                for (int k = 0; k < n; k++)
                    (lu[col, k], lu[maxRow, k]) = (lu[maxRow, k], lu[col, k]);
                det = -det;
            }

            if (Math.Abs(lu[col, col]) < Tolerance) return 0.0;

            det *= lu[col, col];

            for (int row = col + 1; row < n; row++)
            {
                lu[row, col] /= lu[col, col];
                for (int k = col + 1; k < n; k++)
                    lu[row, k] -= lu[row, col] * lu[col, k];
            }
        }

        return det;
    }

    /// <summary>
    /// Computes the Frobenius norm of the matrix (square root of sum of squares of all elements).
    /// </summary>
    /// <returns>The Frobenius norm</returns>
    public double FrobeniusNorm()
    {
        double sum = 0;
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                sum += Values[r, c] * Values[r, c];
        return Math.Sqrt(sum);
    }

    /// <summary>
    /// Returns whether the matrix is symmetric (A == A^T).
    /// The matrix must be square.
    /// </summary>
    /// <returns>True if the matrix is symmetric</returns>
    public bool IsSymmetric()
    {
        if (!IsSquareVector()) return false;
        for (int r = 0; r < Rows; r++)
            for (int c = r + 1; c < Cols; c++)
                if (Math.Abs(Values[r, c] - Values[c, r]) > Tolerance)
                    return false;
        return true;
    }

    /// <summary>
    /// Returns whether the matrix is diagonal (all off-diagonal elements are zero).
    /// The matrix must be square.
    /// </summary>
    /// <returns>True if the matrix is diagonal</returns>
    public bool IsDiagonal()
    {
        if (!IsSquareVector()) return false;
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (r != c && Math.Abs(Values[r, c]) > Tolerance)
                    return false;
        return true;
    }

    /// <summary>
    /// Returns whether the matrix is an identity matrix.
    /// </summary>
    /// <returns>True if the matrix is an identity matrix</returns>
    public bool IsIdentity()
    {
        if (!IsSquareVector()) return false;
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
            {
                double expected = r == c ? 1.0 : 0.0;
                if (Math.Abs(Values[r, c] - expected) > Tolerance)
                    return false;
            }
        return true;
    }

    /// <summary>
    /// Extracts the specified row as a one-dimensional array.
    /// </summary>
    /// <param name="row">The zero-based row index</param>
    /// <returns>An array containing the row values</returns>
    public double[] GetRow(int row)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(row);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(row, Rows);

        var result = new double[Cols];
        for (int c = 0; c < Cols; c++)
            result[c] = Values[row, c];
        return result;
    }

    /// <summary>
    /// Extracts the specified column as a one-dimensional array.
    /// </summary>
    /// <param name="col">The zero-based column index</param>
    /// <returns>An array containing the column values</returns>
    public double[] GetColumn(int col)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(col);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(col, Cols);

        var result = new double[Rows];
        for (int r = 0; r < Rows; r++)
            result[r] = Values[r, col];
        return result;
    }

    /// <summary>
    /// Extracts a submatrix from the specified row and column ranges (inclusive).
    /// </summary>
    /// <param name="rowStart">Starting row index (inclusive)</param>
    /// <param name="rowEnd">Ending row index (inclusive)</param>
    /// <param name="colStart">Starting column index (inclusive)</param>
    /// <param name="colEnd">Ending column index (inclusive)</param>
    /// <returns>A new Matrix containing the submatrix</returns>
    public Matrix SubMatrix(int rowStart, int rowEnd, int colStart, int colEnd)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rowStart);
        ArgumentOutOfRangeException.ThrowIfNegative(colStart);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(rowEnd, Rows);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(colEnd, Cols);

        if (rowEnd < rowStart || colEnd < colStart)
            throw new ArgumentException("End indices must be >= start indices.");

        int newRows = rowEnd - rowStart + 1;
        int newCols = colEnd - colStart + 1;
        var result = new Matrix(newRows, newCols);

        for (int r = 0; r < newRows; r++)
            for (int c = 0; c < newCols; c++)
                result[r, c] = Values[rowStart + r, colStart + c];
        return result;
    }

    /// <summary>
    /// Swaps two rows in the matrix (in-place).
    /// </summary>
    /// <param name="r1">First row index</param>
    /// <param name="r2">Second row index</param>
    public void SwapRows(int r1, int r2)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(r1);
        ArgumentOutOfRangeException.ThrowIfNegative(r2);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(r1, Rows);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(r2, Rows);

        if (r1 == r2) return;
        for (int c = 0; c < Cols; c++)
            (Values[r1, c], Values[r2, c]) = (Values[r2, c], Values[r1, c]);
    }

    /// <summary>
    /// Flattens the matrix into a one-dimensional array in row-major order.
    /// </summary>
    /// <returns>A one-dimensional array of all matrix values</returns>
    public double[] Flatten()
    {
        var result = new double[Rows * Cols];
        int idx = 0;
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                result[idx++] = Values[r, c];
        return result;
    }

    /// <summary>
    /// Returns an enumerator for this Matrix yielding every value in row-major order.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/> instance for the Matrix</returns>
    public IEnumerator<double> GetEnumerator()
    {
        for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Cols; c++)
                yield return Values[r, c];
    }

    /// <summary>
    /// Returns the non-generic Enumerator for the Matrix.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc cref="IMatrixer"/>
    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Cols; j++)
                sb.Append($"{Values[i, j]}\t");
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
