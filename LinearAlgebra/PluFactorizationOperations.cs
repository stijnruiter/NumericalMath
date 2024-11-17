using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using LinearAlgebra.Exceptions;
using LinearAlgebra.Structures;
using LinearAlgebra.Structures.MatrixStorage;

namespace LinearAlgebra;

public static class PluFactorizationOperations
{
    // Extract lower triangle, assuming the diagonal should be 1
    public static Matrix<T> ExtractLower<T>(Matrix<T> lu) where T : struct, INumber<T>
    {
        Matrix<T> result = new Matrix<T>(lu.RowCount, lu.ColumnCount);
        for (var j = 0; j < lu.RowCount; j++)
        {
            result[j, j] = T.One;
            for (var i = j + 1; i < lu.ColumnCount; i++)
            {
                result[i, j] = lu[i, j];
            }
        }
        return result;
    }

    // Extract upper triangle
    public static Matrix<T> ExtractUpper<T>(Matrix<T> lu) where T : struct, INumber<T>
    {
        Matrix<T> result = new Matrix<T>(lu.RowCount, lu.ColumnCount);
        for (var j = 0; j < lu.RowCount; j++)
        {
            for (var i = 0; i <= j; i++)
            {
                result[i, j] = lu[i, j];
            }
        }
        return result;
    }

    private static void SwapRows<T>(this T[] rows, int first, int second)
    {
        (rows[second], rows[first]) = (rows[first], rows[second]);
    }

    /// <summary>
    /// PLU Factorization using partial pivoting. Currently, no check are performed to check if A is factorizable.
    /// 
    /// LU matrix where LU = L + U - I. Since the diagonal of the lower triangle matrix L only contains ones, it is not necessary to store it. We can just store both triangular matrices in 1 matrix.
    /// Pivots contains the vector with the pivot row for each column, A[i, P[i]]
    /// Permutations is the number of row swaps,  used for computing the determinant (det(A) = det(P)det(L)det(U) = (-1)^{Permutations} * prod(diag(U))</returns
    /// </summary>
    /// <typeparam name="T">Data type. For most cases, either float or double. This factorization involves divisions, so integers should not be used.</typeparam>
    /// <param name="A">The original matrix for the LU factorization.</param>
    /// <param name="tolerance">The minimum tolerance for a pivot element. Anything lower than the tolerance, and the matrix A is assumed to be degenerate.</param>
    /// <returns>The tuple containing the LU matrix, the Pivot elements and the number of row swaps</returns>
    /// <exception cref="DegenerateMatrixException">In case an empty pivot column is found during the computations.</exception>
    public static (Matrix<T> LU, int[] Pivots, int Permutations) PluFactorization<T>(Matrix<T> A, T tolerance) where T : struct, INumber<T>
    {
        Matrix<T> LU = A.Copy();
        int permutations = 0;
        int[] pivots = Enumerable.Range(0, LU.ColumnCount).ToArray();

        for (int i = 0; i < LU.ColumnCount; i++)
        {
            (T maxValue, int maxIndex) = LU.ColumnSlice(i, i).AbsMax();

            if (maxValue <= tolerance) // No pivot column found in column i
                throw new DegenerateMatrixException();

            if (maxIndex != 0)
            {
                pivots.SwapRows(i, maxIndex + i);
                if (LU.Storage is RowMajorMatrixStorage<T> rm)
                {
                    rm.SwapRows(i, maxIndex + i);
                }
                permutations++;
            }

            ReadOnlySpan<T> pivotRow = LU.Row(i).Span.Slice(i + 1);

            //Parallel.For(i + 1, LU.RowCount, j =>
            for (int j = i + 1; j < LU.RowCount; j++)
            {
                // Gaussian elimination step using pivot i
                LU[j, i] /= LU[i, i];

                //for (int k = i + 1; k < LU.ColumnCount; k++)
                //    LU[j, k] -= LU[j, i] * LU[i, k];

                Span<T> elemRow = LU.Row(j).Span.Slice(i + 1);
                ScalarProductSubtract(elemRow, LU[j, i], pivotRow);
            };
        }

        return (LU, pivots, permutations);
    }

    private static void ScalarProductSubtract<T>(Span<T> destination, T scalar, ReadOnlySpan<T> source) where T : struct, INumberBase<T>
    {
        ReadOnlySpan<Vector<T>> sourceVec = MemoryMarshal.Cast<T, Vector<T>>(source);
        Span<Vector<T>> destinationVec = MemoryMarshal.Cast<T, Vector<T>>(destination);
        Vector<T> scalarVec = new Vector<T>(scalar);
        for (int j = 0; j < sourceVec.Length; j++)
        {
            destinationVec[j] -= scalarVec * sourceVec[j];
        }

        for (int j = sourceVec.Length * Vector<T>.Count; j <  destination.Length; j++)
        {
            destination[j] -= scalar * source[j];
        }
    }

    /// <summary>
    /// Doolittle's LU Decomposition. It has no permutations matrix.
    /// LU = L + U - I, since 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="A"></param>
    /// <returns></returns>
    public static Matrix<T> LuDecompositionDoolittle<T>(Matrix<T> A) where T : struct, INumber<T>
    {
        Matrix<T> lu = new Matrix<T>(A.RowCount, A.ColumnCount);
        T sum;

        for (int i = 0; i < A.RowCount; i++)
        {
            // Upper tiangle
            for (int j = i; j < A.ColumnCount; j++)
            {
                sum = T.AdditiveIdentity;
                for (int k = 0; k < i; k++)
                {
                    sum += lu[i, k] * lu[k, j];
                }
                lu[i, j] = A[i, j] - sum;
            }

            // Lower triangle
            for (int j = i + 1; j < A.ColumnCount; j++)
            {
                sum = T.AdditiveIdentity;
                for (int k = 0; k < i; k++)
                {
                    sum += lu[j, k] * lu[k, i];
                }
                lu[j, i] = (A[j, i] - sum) / lu[i, i];
            }
        }

        return lu;
    }

    /// <summary>
    /// Compute the determinant of matrix A using the PLU decomposition.
    /// </summary>
    /// <param name="A">A</param>
    /// <param name="tolerance">Floating point tolerance for pivot values</param>
    /// <returns>The determinant of A</returns>
    public static T PluDeterminant<T>(Matrix<T> A, T tolerance) where T : struct, INumber<T>
    {
        // P * A = L * U
        // det(A) = det(P^{-1})det(L)det(U)
        // P is the permutation matrix, thus P^{-1}=P^T

        // Both L and U are triangular matrices, thus det(L) = prod(diag(L))
        // det(L) = prod(diag(L)) = prod([1,1,1...]) = 1
        // det(U) = prod(diag(U)) = u[0,0] * u[1,1] * u[2,2]*...
        // det(P) = (-1)^(number of permutations) = (-1)^(permutations % 2)
        (Matrix<T> lu, int[] pivots, int permutations) = PluFactorization(A, tolerance);

        T determinant = permutations % 2 == 0 ? T.MultiplicativeIdentity : -T.MultiplicativeIdentity;
        determinant *= lu.DiagonalProduct();
        return determinant;
    }

    /// <summary>
    /// Forward substitution for solving the system Ly=b, where L is a lower triangular matrix where the diagonal is 1. y and b are column vectors.
    /// </summary>
    /// <param name="L">Lower triangular matrix with diagonal 1.</param>
    /// <param name="b">The right hand side column matrix</param>
    /// <returns>Solution for y</returns>
    public static ColumnVector<T> ForwardSubstitution<T>(Matrix<T> L, ColumnVector<T> b) where T : struct, INumber<T>
    {
        ColumnVector<T> y = b.Copy();// new ColumnVector<T>(b.Length);
        ForwardSubstitutionInPlace(L, y.Span);
        return y;
    }

    internal static void ForwardSubstitutionInPlace<T>(Matrix<T> L, Span<T> b) where T : struct, INumber<T>
    {
        for (int i = 0; i < b.Length; i++)
        {
            //for (int k = 0; k < i; k++)
            //    sum += L[i, k] * y[k];
            b[i] -= VectorizationOps.DotProduct<T>(L.Row(i).Span.Slice(0, i), b.Slice(0, i));
        }
    }

    /// <summary>
    /// Forward substitution for solving the system Ly=b, where L is a lower triangular matrix where the diagonal is 1. y and b are column vectors.
    /// </summary>
    /// <param name="L">Lower triangular matrix with diagonal 1.</param>
    /// <param name="B">The right hand side column matrix</param>
    /// <returns>Solution for y</returns>
    public static Matrix<T> ForwardSubstitution<T>(Matrix<T> L, Matrix<T> B) where T : struct, INumber<T>
    {
        Matrix<T> Y = B.Copy();// new Matrix<T>(B.RowCount, B.ColumnCount);
        ForwardSubstitutionInPlace(L, Y);
        return Y;
    }
    
    private static void ForwardSubstitutionInPlace<T>(Matrix<T> L, Matrix<T> B) where T : struct, INumber<T>
    {
        for (int i = 0; i < B.RowCount; i++)
        {
            ReadOnlySpan<T> partialRow = L.Row(i).Span.Slice(0, i);
            for (int j = 0; j < B.ColumnCount; j++)
            {
                //for (int k = 0; k < i; k++)
                //    Y[i, j] -= L[i, k] * Y[k, j];
                //Y[i, j] = B[i, j] - sum;

                // TODO: Might be faster if Y was column-major, then no copies need to be made
                ReadOnlySpan<T> partialColumn = B.ColumnSlice(j, 0, i).Span;
                B[i,j] -= ElementwiseOps.DotProduct(partialRow, 1, partialColumn, B.ColumnCount);

            }
        }
    }

    /// <summary>
    /// Backwards substitution for solving Ux=y, where U is an upper triangular matrix. x and y are column matrices.
    /// </summary>
    /// <param name="U">The upper matrix</param>
    /// <param name="y">The right hand side column matrix</param>
    /// <returns>Solution for x</returns>
    public static ColumnVector<T> BackwardSubstitution<T>(Matrix<T> U, ColumnVector<T> y) where T : struct, INumber<T>
    {
        ColumnVector<T> x = y.Copy();
        BackwardSubstitutionInPlace(U, x.Span);
        return x;
    }

    public static void BackwardSubstitutionInPlace<T>(Matrix<T> U, Span<T> y) where T : struct, INumber<T>
    {
        for (int i = y.Length - 1; i >= 0; i--)
        {
            //for (int k = i + 1; k < y.Length; k++)
            //    sum += U[i, k] * x[k];
            y[i] = (y[i] - VectorizationOps.DotProduct<T>(U.Row(i).Span.Slice(i + 1), y.Slice(i + 1))) / U[i, i];
        }
    }

    /// <summary>
    /// Backwards substitution for solving Ux=y, where U is an upper triangular matrix. x and y are column matrices.
    /// </summary>
    /// <param name="U">The upper matrix</param>
    /// <param name="y">The right hand side column matrix</param>
    /// <returns>Solution for x</returns>
    public static Matrix<T> BackwardSubstitution<T>(Matrix<T> U, Matrix<T> Y) where T : struct, INumber<T>
    {
        Matrix<T> X = Y.Copy();
        BackwardSubstitutionInPlace(U, X);
        return X;
    }

    private static void BackwardSubstitutionInPlace<T>(Matrix<T> U, Matrix<T> Y) where T : struct, INumber<T>
    {
        for (int i = Y.RowCount - 1; i >= 0; i--)
        {
            ReadOnlySpan<T> partialRow = U.Row(i).Span.Slice(i + 1);
            for (int j = 0; j < Y.ColumnCount; j++)
            {
                //for (int k = i + 1; k < Y.RowCount; k++)
                //    sum += U[i, k] * X[k, j];
                ReadOnlySpan<T> partialColumn = Y.ColumnSlice(j, i + 1).Span;
                Y[i, j] = (Y[i, j] - ElementwiseOps.DotProduct(partialRow, 1, partialColumn, Y.ColumnCount)) / U[i, i];
            }
        }
    }

    /// <summary>
    /// Solve the system Ax=b
    /// </summary>
    /// <param name="A">Lefthand side matrix</param>
    /// <param name="b">Righthand side column vector</param>
    /// <returns>The solution for x</returns>
    public static ColumnVector<T> SolveUsingDoolittleLU<T>(Matrix<T> A, ColumnVector<T> b) where T : struct, INumber<T>
    {
        // LU decomposition: A = L U
        Matrix<T> lu = LuDecompositionDoolittle(A); // L + U - I

        if (lu.DiagonalProduct() == T.Zero)
            throw new NotInvertibleException(NonInvertibleReason.Singular);

        // Solve Ly=b
        ColumnVector<T> y = ForwardSubstitution(lu, b);

        // Solve Ux=y
        BackwardSubstitutionInPlace(lu, y.Span);

        return y;
    }

    /// <summary>
    /// Solve the system AX=B
    /// </summary>
    /// <param name="A">Lefthand side matrix</param>
    /// <param name="b">Righthand side matrix</param>
    /// <returns>The solution for x</returns>
    public static Matrix<T> SolveUsingDoolittleLU<T>(Matrix<T> A, Matrix<T> B) where T : struct, INumber<T>
    {
        // LU decomposition: A = L U
        Matrix<T> lu = LuDecompositionDoolittle(A); // L + U - I

        if (lu.DiagonalProduct() == T.Zero)
            throw new NotInvertibleException(NonInvertibleReason.Singular);

        // Solve Ly=b
        Matrix<T> y = ForwardSubstitution(lu, B);

        // Solve Ux=y
        BackwardSubstitutionInPlace(lu, y);

        return y;
    }


    public static ColumnVector<T> SolveUsingPLU<T>(Matrix<T> A, ColumnVector<T> b, T tolerance) where T : struct, INumber<T>
    {
        // LU decomposition: P A = L U
        (Matrix<T> lu, int[] pivots, int perm) = PluFactorization(A, tolerance); // L + U - I

        if (T.Abs(lu.DiagonalProduct()) <= tolerance)
            throw new NotInvertibleException(NonInvertibleReason.Singular);

        // Ax = b <=> PAx = Pb = LUx
        ColumnVector<T> Pb = new ColumnVector<T>(pivots.Length);
        for (int i = 0; i < pivots.Length; i++)
        {
            Pb[i] = b[pivots[i]];
        }

        // Solve Ly=b
        ForwardSubstitutionInPlace(lu, Pb.Span);

        // Solve Ux=y
        BackwardSubstitutionInPlace(lu, Pb.Span);

        return Pb;
    }

    public static Matrix<T> SolveUsingPLU<T>(Matrix<T> A, Matrix<T> B, T tolerance) where T : struct, INumber<T>
    {
        // LU decomposition: P A = L U
        (Matrix<T> lu, int[] pivots, int perm) = PluFactorization(A, tolerance); // L + U - I
        
        if (T.Abs(lu.DiagonalProduct()) <= tolerance)
            throw new NotInvertibleException(NonInvertibleReason.Singular);

        // Ax = b <=> PAx = Pb = LUx
        Matrix<T> Pb = new Matrix<T>(B.RowCount, B.ColumnCount);
        for (int i = 0; i < Pb.RowCount; i++)
        {
            B.Row(pivots[i]).Span.CopyTo(Pb.Row(i).Span);
        }

        // Solve LY=B
        ForwardSubstitutionInPlace(lu, Pb);

        // Solve UX=Y
        BackwardSubstitutionInPlace(lu, Pb);

        return Pb;
    }
}