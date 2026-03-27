using Ampere.AmpMath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmpereMSTest;

[TestClass]
public class MatrixTest
{
    // ── Construction ──────────────────────────────────────────

    [TestMethod]
    public void Constructor_RowsCols_CreatesZeroMatrix()
    {
        var m = new Matrix(3, 4);
        Assert.AreEqual(3, m.Rows);
        Assert.AreEqual(4, m.Cols);
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 4; c++)
                Assert.AreEqual(0.0, m[r, c]);
    }

    [TestMethod]
    public void Constructor_2DArray_CopiesDimensions()
    {
        double[,] arr = { { 1, 2 }, { 3, 4 }, { 5, 6 } };
        var m = new Matrix(arr);
        Assert.AreEqual(3, m.Rows);
        Assert.AreEqual(2, m.Cols);
        Assert.AreEqual(4.0, m[1, 1]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Constructor_ZeroRows_Throws()
    {
        _ = new Matrix(0, 5);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NullArray_Throws()
    {
        _ = new Matrix(null!);
    }

    // ── Indexer ──────────────────────────────────────────────

    [TestMethod]
    public void Indexer_SetAndGet()
    {
        var m = new Matrix(2, 2);
        m[0, 1] = 42.0;
        Assert.AreEqual(42.0, m[0, 1]);
    }

    // ── Transpose ────────────────────────────────────────────

    [TestMethod]
    public void Transpose_Rectangular()
    {
        double[,] arr = { { 1, 2, 3 }, { 4, 5, 6 } };
        var m = new Matrix(arr);
        var t = m.Transpose();
        Assert.AreEqual(3, t.Rows);
        Assert.AreEqual(2, t.Cols);
        Assert.AreEqual(1.0, t[0, 0]);
        Assert.AreEqual(4.0, t[0, 1]);
        Assert.AreEqual(3.0, t[2, 0]);
        Assert.AreEqual(6.0, t[2, 1]);
    }

    // ── Scalar Operators ─────────────────────────────────────

    [TestMethod]
    public void Add_MatrixPlusScalar()
    {
        var m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        var r = m + 10;
        Assert.AreEqual(11.0, r[0, 0]);
        Assert.AreEqual(14.0, r[1, 1]);
    }

    [TestMethod]
    public void Add_ScalarPlusMatrix()
    {
        var m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        var r = 10 + m;
        Assert.AreEqual(11.0, r[0, 0]);
        Assert.AreEqual(14.0, r[1, 1]);
    }

    [TestMethod]
    public void Subtract_MatrixMinusScalar()
    {
        var m = new Matrix(new double[,] { { 10, 20 }, { 30, 40 } });
        var r = m - 5;
        Assert.AreEqual(5.0, r[0, 0]);
        Assert.AreEqual(35.0, r[1, 1]);
    }

    [TestMethod]
    public void Subtract_ScalarMinusMatrix_ProducesScalarMinusEachElement()
    {
        var m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        var r = 10 - m;
        Assert.AreEqual(9.0, r[0, 0]);  // 10 - 1
        Assert.AreEqual(8.0, r[0, 1]);  // 10 - 2
        Assert.AreEqual(7.0, r[1, 0]);  // 10 - 3
        Assert.AreEqual(6.0, r[1, 1]);  // 10 - 4
    }

    [TestMethod]
    public void Multiply_MatrixTimesScalar()
    {
        var m = new Matrix(new double[,] { { 2, 3 }, { 4, 5 } });
        var r = m * 3;
        Assert.AreEqual(6.0, r[0, 0]);
        Assert.AreEqual(15.0, r[1, 1]);
    }

    [TestMethod]
    public void Divide_MatrixDivScalar()
    {
        var m = new Matrix(new double[,] { { 10, 20 }, { 30, 40 } });
        var r = m / 10;
        Assert.AreEqual(1.0, r[0, 0]);
        Assert.AreEqual(4.0, r[1, 1]);
    }

    [TestMethod]
    public void Divide_ScalarDivMatrix()
    {
        var m = new Matrix(new double[,] { { 2, 4 }, { 5, 10 } });
        var r = 20.0 / m;
        Assert.AreEqual(10.0, r[0, 0]); // 20 / 2
        Assert.AreEqual(5.0, r[0, 1]);  // 20 / 4
        Assert.AreEqual(4.0, r[1, 0]);  // 20 / 5
        Assert.AreEqual(2.0, r[1, 1]);  // 20 / 10
    }

    [TestMethod]
    public void Mod_MatrixModScalar()
    {
        var m = new Matrix(new double[,] { { 7, 10 }, { 13, 15 } });
        var r = m % 4;
        Assert.AreEqual(3.0, r[0, 0]);
        Assert.AreEqual(2.0, r[0, 1]);
    }

    [TestMethod]
    public void Mod_ScalarModMatrix()
    {
        var m = new Matrix(new double[,] { { 3, 4 } });
        var r = 10.0 % m;
        Assert.AreEqual(1.0, r[0, 0]); // 10 % 3
        Assert.AreEqual(2.0, r[0, 1]); // 10 % 4
    }

    // ── DoScalar does not mutate original ────────────────────

    [TestMethod]
    public void ScalarOps_DoNotMutateOriginal()
    {
        var m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        _ = m + 100;
        Assert.AreEqual(1.0, m[0, 0]); // original unchanged
    }

    // ── Negate ───────────────────────────────────────────────

    [TestMethod]
    public void Negate_FlipsSign()
    {
        var m = new Matrix(new double[,] { { 1, -2 }, { 0, 4 } });
        var r = !m;
        Assert.AreEqual(-1.0, r[0, 0]);
        Assert.AreEqual(2.0, r[0, 1]);
        Assert.AreEqual(0.0, r[1, 0]);
        Assert.AreEqual(-4.0, r[1, 1]);
    }

    // ── Matrix-Matrix Operators ──────────────────────────────

    [TestMethod]
    public void Add_TwoMatrices()
    {
        var a = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        var b = new Matrix(new double[,] { { 5, 6 }, { 7, 8 } });
        var r = a + b;
        Assert.AreEqual(6.0, r[0, 0]);
        Assert.AreEqual(12.0, r[1, 1]);
    }

    [TestMethod]
    public void Subtract_TwoMatrices()
    {
        var a = new Matrix(new double[,] { { 10, 20 }, { 30, 40 } });
        var b = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        var r = a - b;
        Assert.AreEqual(9.0, r[0, 0]);
        Assert.AreEqual(36.0, r[1, 1]);
    }

    [TestMethod]
    public void DotProduct_2x2()
    {
        var a = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        var b = new Matrix(new double[,] { { 5, 6 }, { 7, 8 } });
        var r = a * b;
        Assert.AreEqual(19.0, r[0, 0]); // 1*5 + 2*7
        Assert.AreEqual(22.0, r[0, 1]); // 1*6 + 2*8
        Assert.AreEqual(43.0, r[1, 0]); // 3*5 + 4*7
        Assert.AreEqual(50.0, r[1, 1]); // 3*6 + 4*8
    }

    [TestMethod]
    [ExpectedException(typeof(MatrixPropertyException))]
    public void DotProduct_IncompatibleDimensions_Throws()
    {
        var a = new Matrix(2, 3);
        var b = new Matrix(2, 3);
        _ = a * b; // a.Cols != b.Rows
    }

    // ── Equality ─────────────────────────────────────────────

    [TestMethod]
    public void Equality_SameValues_True()
    {
        var a = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        var b = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        Assert.IsTrue(a == b);
    }

    [TestMethod]
    public void Equality_DifferentValues_False()
    {
        var a = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        var b = new Matrix(new double[,] { { 1, 2 }, { 3, 5 } });
        Assert.IsTrue(a != b);
    }

    [TestMethod]
    public void Equality_DifferentDimensions_False()
    {
        var a = new Matrix(2, 2);
        var b = new Matrix(2, 3);
        Assert.IsTrue(a != b);
    }

    // ── GetEnumerator ────────────────────────────────────────

    [TestMethod]
    public void GetEnumerator_YieldsAllValuesRowMajor()
    {
        double[,] arr = { { 1, 2, 3 }, { 4, 5, 6 } };
        var m = new Matrix(arr);
        var values = m.ToList();
        CollectionAssert.AreEqual(new double[] { 1, 2, 3, 4, 5, 6 }, values);
    }

    // ── Clone ────────────────────────────────────────────────

    [TestMethod]
    public void Clone_IsDeepCopy()
    {
        var m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        var c = m.Clone();
        Assert.IsTrue(m == c);
        c[0, 0] = 999;
        Assert.AreEqual(1.0, m[0, 0]); // original not affected
    }

    // ── Identity ─────────────────────────────────────────────

    [TestMethod]
    public void Identity_3x3()
    {
        var m = Matrix.Identity(3);
        Assert.AreEqual(1.0, m[0, 0]);
        Assert.AreEqual(1.0, m[1, 1]);
        Assert.AreEqual(1.0, m[2, 2]);
        Assert.AreEqual(0.0, m[0, 1]);
        Assert.AreEqual(0.0, m[1, 0]);
    }

    // ── Trace ────────────────────────────────────────────────

    [TestMethod]
    public void Trace_3x3()
    {
        var m = new Matrix(new double[,] { { 1, 0, 0 }, { 0, 5, 0 }, { 0, 0, 9 } });
        Assert.AreEqual(15.0, m.Trace(), 1e-10);
    }

    [TestMethod]
    [ExpectedException(typeof(MatrixPropertyException))]
    public void Trace_NonSquare_Throws()
    {
        var m = new Matrix(2, 3);
        m.Trace();
    }

    // ── Determinant ──────────────────────────────────────────

    [TestMethod]
    public void Determinant_1x1()
    {
        var m = new Matrix(new double[,] { { 7 } });
        Assert.AreEqual(7.0, m.Determinant(), 1e-10);
    }

    [TestMethod]
    public void Determinant_2x2()
    {
        var m = new Matrix(new double[,] { { 3, 8 }, { 4, 6 } });
        Assert.AreEqual(-14.0, m.Determinant(), 1e-10);
    }

    [TestMethod]
    public void Determinant_3x3()
    {
        var m = new Matrix(new double[,] { { 6, 1, 1 }, { 4, -2, 5 }, { 2, 8, 7 } });
        Assert.AreEqual(-306.0, m.Determinant(), 1e-10);
    }

    [TestMethod]
    public void Determinant_SingularMatrix_ReturnsZero()
    {
        var m = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
        Assert.AreEqual(0.0, m.Determinant(), 1e-10);
    }

    // ── FrobeniusNorm ────────────────────────────────────────

    [TestMethod]
    public void FrobeniusNorm_2x2()
    {
        var m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        Assert.AreEqual(Math.Sqrt(30), m.FrobeniusNorm(), 1e-10);
    }

    // ── IsSymmetric, IsDiagonal, IsIdentity ──────────────────

    [TestMethod]
    public void IsSymmetric_True()
    {
        var m = new Matrix(new double[,] { { 1, 2, 3 }, { 2, 5, 6 }, { 3, 6, 9 } });
        Assert.IsTrue(m.IsSymmetric());
    }

    [TestMethod]
    public void IsSymmetric_False()
    {
        var m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        Assert.IsFalse(m.IsSymmetric());
    }

    [TestMethod]
    public void IsDiagonal_True()
    {
        var m = new Matrix(new double[,] { { 5, 0 }, { 0, 3 } });
        Assert.IsTrue(m.IsDiagonal());
    }

    [TestMethod]
    public void IsDiagonal_False()
    {
        var m = new Matrix(new double[,] { { 5, 1 }, { 0, 3 } });
        Assert.IsFalse(m.IsDiagonal());
    }

    [TestMethod]
    public void IsIdentity_True()
    {
        Assert.IsTrue(Matrix.Identity(4).IsIdentity());
    }

    [TestMethod]
    public void IsIdentity_False()
    {
        var m = new Matrix(new double[,] { { 1, 0 }, { 0, 2 } });
        Assert.IsFalse(m.IsIdentity());
    }

    // ── GetRow / GetColumn ───────────────────────────────────

    [TestMethod]
    public void GetRow_ReturnsCorrectRow()
    {
        var m = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
        CollectionAssert.AreEqual(new double[] { 4, 5, 6 }, m.GetRow(1));
    }

    [TestMethod]
    public void GetColumn_ReturnsCorrectColumn()
    {
        var m = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
        CollectionAssert.AreEqual(new double[] { 2, 5 }, m.GetColumn(1));
    }

    // ── SubMatrix ────────────────────────────────────────────

    [TestMethod]
    public void SubMatrix_ExtractsCorrectRegion()
    {
        var m = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
        var sub = m.SubMatrix(0, 1, 1, 2);
        Assert.AreEqual(2, sub.Rows);
        Assert.AreEqual(2, sub.Cols);
        Assert.AreEqual(2.0, sub[0, 0]);
        Assert.AreEqual(3.0, sub[0, 1]);
        Assert.AreEqual(5.0, sub[1, 0]);
        Assert.AreEqual(6.0, sub[1, 1]);
    }

    // ── SwapRows ─────────────────────────────────────────────

    [TestMethod]
    public void SwapRows_SwapsCorrectly()
    {
        var m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } });
        m.SwapRows(0, 2);
        Assert.AreEqual(5.0, m[0, 0]);
        Assert.AreEqual(6.0, m[0, 1]);
        Assert.AreEqual(1.0, m[2, 0]);
        Assert.AreEqual(2.0, m[2, 1]);
    }

    // ── Flatten ──────────────────────────────────────────────

    [TestMethod]
    public void Flatten_ReturnsRowMajorArray()
    {
        var m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        CollectionAssert.AreEqual(new double[] { 1, 2, 3, 4 }, m.Flatten());
    }

    // ── Map ──────────────────────────────────────────────────

    [TestMethod]
    public void Map_SquaresElements()
    {
        var m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        var r = m.Map(x => x * x);
        Assert.AreEqual(1.0, r[0, 0]);
        Assert.AreEqual(4.0, r[0, 1]);
        Assert.AreEqual(9.0, r[1, 0]);
        Assert.AreEqual(16.0, r[1, 1]);
    }

    // ── Fill ─────────────────────────────────────────────────

    [TestMethod]
    public void Fill_SetsAllElements()
    {
        var m = new Matrix(2, 3);
        m.Fill(7.0);
        foreach (var v in m)
            Assert.AreEqual(7.0, v);
    }

    // ── IsRowVector / IsColumnVector / IsSquareVector ────────

    [TestMethod]
    public void IsRowVector_True() => Assert.IsTrue(new Matrix(1, 5).IsRowVector());

    [TestMethod]
    public void IsRowVector_False() => Assert.IsFalse(new Matrix(2, 5).IsRowVector());

    [TestMethod]
    public void IsColumnVector_True() => Assert.IsTrue(new Matrix(5, 1).IsColumnVector());

    [TestMethod]
    public void IsSquareVector_True() => Assert.IsTrue(new Matrix(3, 3).IsSquareVector());

    // ── ToString ─────────────────────────────────────────────

    [TestMethod]
    public void ToString_ContainsAllValues()
    {
        var m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        var s = m.ToString();
        Assert.IsTrue(s.Contains("1"));
        Assert.IsTrue(s.Contains("4"));
    }

    // ── Wrapper Methods ──────────────────────────────────────

    [TestMethod]
    public void WrapperMethods_MatchOperators()
    {
        var a = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        var b = new Matrix(new double[,] { { 5, 6 }, { 7, 8 } });

        Assert.IsTrue(Matrix.Add(a, 1.0) == a + 1.0);
        Assert.IsTrue(Matrix.Add(a, b) == a + b);
        Assert.IsTrue(Matrix.Subtract(a, 1.0) == a - 1.0);
        Assert.IsTrue(Matrix.Subtract(a, b) == a - b);
        Assert.IsTrue(Matrix.Multiply(a, 2.0) == a * 2.0);
        Assert.IsTrue(Matrix.DotProduct(a, b) == a * b);
        Assert.IsTrue(Matrix.Divide(a, 2.0) == a / 2.0);
        Assert.IsTrue(Matrix.Mod(a, 3.0) == a % 3.0);
        Assert.IsTrue(Matrix.Negate(a) == !a);
    }
}
