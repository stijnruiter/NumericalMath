using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using LinearAlgebra.Exceptions;
using LinearAlgebra.Structures;

namespace LinearAlgebra;

public static class PluFactorizationOperations
{
    // Extract lower triangle, assuming the diagonal should be 1
    public static Matrix<T> ExtractLower<T>(Matrix<T> lu) where T : INumber<T>
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
    public static Matrix<T> ExtractUpper<T>(Matrix<T> lu) where T : INumber<T>
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

    /// <summary>
    /// Get largest absolute column value, e.g. max(abs(matrix[startIndex.. , columnIndex]))
    /// </summary>
    /// <param name="matrix">The searchmatrix</param>
    /// <returns>The max abs value and its index</returns>
    private static (T value, int index) GetAbsMaxElementInColumn<T>(Matrix<T> matrix, int columnIndex, int startIndex = 0) where T : INumber<T>
    {
        // Start with first element
        int maxIndex = startIndex;
        T maxValue = T.Abs(matrix[columnIndex, startIndex]);
        T nextValue;

        // Loop over the rest of the rows
        for (int i = startIndex + 1; i < matrix.RowCount; i++)
        {
            if ((nextValue = T.Abs(matrix[i, columnIndex])) > maxValue)
            {
                maxValue = nextValue;
                maxIndex = i;
            }
        }
        return (maxValue, maxIndex);
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
    public static (Matrix<T> LU, int[] Pivots, int Permutations) PluFactorization<T>(Matrix<T> A, T tolerance) where T:INumber<T>
    {
        Matrix<T> LU = A.Copy();
        int permutations = 0;
        int[] pivots = Enumerable.Range(0, LU.ColumnCount).ToArray();

        for (int i = 0; i < LU.ColumnCount; i++)
        {
            (T maxValue, int maxIndex) = GetAbsMaxElementInColumn(LU, i, i);

            if (maxValue < tolerance) // No pivot column found in column i
                throw new DegenerateMatrixException();

            if (maxIndex != i)
            {
                pivots.SwapRows(i, maxIndex);
                LU.SwapRows(i, maxIndex);
                permutations++;
            }

            Parallel.For(i + 1, LU.RowCount, j =>
            {
                // Gaussian elimination step using pivot i
                LU[j, i] /= LU[i, i];

                for (int k = i + 1; k < LU.ColumnCount; k++)
                {
                    LU[j, k] -= LU[j, i] * LU[i, k];
                }
            });
        }

        return (LU, pivots, permutations);
    }

    /// <summary>
    /// Doolittle's LU Decomposition. It has no permutations matrix.
    /// LU = L + U - I, since 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="A"></param>
    /// <returns></returns>
    public static Matrix<T> LuDecompositionDoolittle<T>(Matrix<T> A) where T : INumber<T>
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
    public static T PluDeterminant<T>(Matrix<T> A, T tolerance) where T : INumber<T>
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
        determinant *= DiagonalProduct(lu);
        return determinant;
    }

    /// <summary>
    /// Forward substitution for solving the system Ly=b, where L is a lower triangular matrix where the diagonal is 1. y and b are column vectors.
    /// </summary>
    /// <param name="L">Lower triangular matrix with diagonal 1.</param>
    /// <param name="b">The right hand side column matrix</param>
    /// <returns>Solution for y</returns>
    public static ColumnVector<T> ForwardSubstitution<T>(Matrix<T> L, ColumnVector<T> b) where T : INumber<T>
    {
        T sum;
        ColumnVector<T> y = new ColumnVector<T>(b.Length);
        for (int i = 0; i < b.Length; i++)
        {
            sum = T.AdditiveIdentity;
            for (int k = 0; k < i; k++)
            {
                sum += L[i, k] * y[k];
            }
            y[i] = b[i] - sum;
        }
        return y;
    }

    /// <summary>
    /// Forward substitution for solving the system Ly=b, where L is a lower triangular matrix where the diagonal is 1. y and b are column vectors.
    /// </summary>
    /// <param name="L">Lower triangular matrix with diagonal 1.</param>
    /// <param name="B">The right hand side column matrix</param>
    /// <returns>Solution for y</returns>
    public static Matrix<T> ForwardSubstitution<T>(Matrix<T> L, Matrix<T> B) where T : INumber<T>
    {
        T sum;
        Matrix<T> Y = new Matrix<T>(B.RowCount, B.ColumnCount);
        for (int i = 0; i < B.RowCount; i++)
        {
            for (int j = 0; j < B.ColumnCount; j++)
            {
                sum = T.AdditiveIdentity;
                for (int k = 0; k < i; k++)
                {
                    sum += L[i, k] * Y[k, j];
                }
                Y[i, j] = B[i, j] - sum;
            }
        }
        return Y;
    }

    /// <summary>
    /// Backwards substitution for solving Ux=y, where U is an upper triangular matrix. x and y are column matrices.
    /// </summary>
    /// <param name="U">The upper matrix</param>
    /// <param name="y">The right hand side column matrix</param>
    /// <returns>Solution for x</returns>
    public static ColumnVector<T> BackwardSubstitution<T>(Matrix<T> U, ColumnVector<T> y) where T : INumber<T>
    {
        // Solve 
        T sum;
        ColumnVector<T> x = new ColumnVector<T>(y.Length);
        for (int i = y.Length - 1; i >= 0; i--)
        {
            sum = T.AdditiveIdentity;
            for (int k = i + 1; k < y.Length; k++)
            {
                sum += U[i, k] * x[k];
            }

            x[i] = (y[i] - sum) / U[i, i];
        }
        return x;
    }

    /// <summary>
    /// Backwards substitution for solving Ux=y, where U is an upper triangular matrix. x and y are column matrices.
    /// </summary>
    /// <param name="U">The upper matrix</param>
    /// <param name="y">The right hand side column matrix</param>
    /// <returns>Solution for x</returns>
    public static Matrix<T> BackwardSubstitution<T>(Matrix<T> U, Matrix<T> Y) where T : INumber<T>
    {
        // Solve
        T sum;
        Matrix<T> X = new Matrix<T>(Y.RowCount, Y.ColumnCount);
        for (int i = Y.RowCount - 1; i >= 0; i--)
        {
            for (int j = 0; j < Y.ColumnCount; j++)
            {
                sum = T.AdditiveIdentity;
                for (int k = i + 1; k < Y.RowCount; k++)
                {
                    sum += U[i, k] * X[k, j];
                }

                X[i, j] = (Y[i, j] - sum) / U[i, i];
            }
        }
        return X;
    }

    /// <summary>
    /// Solve the system Ax=b
    /// </summary>
    /// <param name="A">Lefthand side matrix</param>
    /// <param name="b">Righthand side column vector</param>
    /// <returns>The solution for x</returns>
    public static ColumnVector<T> SolveUsingDoolittleLU<T>(Matrix<T> A, ColumnVector<T> b) where T : INumber<T>
    {
        // LU decomposition: A = L U
        Matrix<T> lu = LuDecompositionDoolittle(A); // L + U - I

        if (DiagonalProduct(lu) == T.Zero)
            throw new NotInvertibleException(NonInvertibleReason.Singular);

        // Solve Ly=b
        ColumnVector<T> y = ForwardSubstitution(lu, b);

        // Solve Ux=y
        ColumnVector<T> x = BackwardSubstitution(lu, y);

        return x;
    }

    /// <summary>
    /// Solve the system AX=B
    /// </summary>
    /// <param name="A">Lefthand side matrix</param>
    /// <param name="b">Righthand side matrix</param>
    /// <returns>The solution for x</returns>
    public static Matrix<T> SolveUsingDoolittleLU<T>(Matrix<T> A, Matrix<T> B) where T : INumber<T>
    {
        // LU decomposition: A = L U
        Matrix<T> lu = LuDecompositionDoolittle(A); // L + U - I

        if (DiagonalProduct(lu) == T.Zero)
            throw new NotInvertibleException(NonInvertibleReason.Singular);

        // Solve Ly=b
        Matrix<T> y = ForwardSubstitution(lu, B);

        // Solve Ux=y
        Matrix<T> x = BackwardSubstitution(lu, y);

        return x;
    }


    public static ColumnVector<T> SolveUsingPLU<T>(Matrix<T> A, ColumnVector<T> b, T tolerance) where T : INumber<T>
    {
        // LU decomposition: P A = L U
        (Matrix<T> lu, int[] pivots, int perm) = PluFactorization(A, tolerance); // L + U - I

        if (T.Abs(DiagonalProduct(lu)) <= tolerance)
            throw new NotInvertibleException(NonInvertibleReason.Singular);

        // Ax = b <=> PAx = Pb = LUx
        ColumnVector<T> Pb = new ColumnVector<T>(pivots.Length);
        for (int i = 0; i < pivots.Length; i++)
        {
            Pb[i] = b[pivots[i]];
        }

        // Solve Ly=b
        ColumnVector<T> y = ForwardSubstitution(lu, Pb);

        // Solve Ux=y
        ColumnVector<T> x = BackwardSubstitution(lu, y);

        return x;
    }


    public static Matrix<T> SolveUsingPLU<T>(Matrix<T> A, Matrix<T> B, T tolerance) where T : INumber<T>
    {
        // LU decomposition: P A = L U
        (Matrix<T> lu, int[] pivots, int perm) = PluFactorization(A, tolerance); // L + U - I
        
        if (T.Abs(DiagonalProduct(lu)) <= tolerance)
            throw new NotInvertibleException(NonInvertibleReason.Singular);

        // Ax = b <=> PAx = Pb = LUx
        Matrix<T> Pb = Matrix<T>.Zero(B.RowCount, B.ColumnCount);
        for (int i = 0; i < Pb.RowCount; i++)
        {
            for (int j = 0; j < Pb.ColumnCount; j++)
            {
                Pb[i, j] = B[pivots[i], j];
            }
        }

        // Solve LY=B
        Matrix<T> Y = ForwardSubstitution(lu, Pb);

        // Solve UX=Y
        Matrix<T> X = BackwardSubstitution(lu, Y);

        return X;
    }

    private static T DiagonalProduct<T>(Matrix<T> matrix) where T : INumber<T>
    {
        T product = T.MultiplicativeIdentity;
        for(int i = 0; i < matrix.ColumnCount; i++)
        {
            product *= matrix[i, i];
        }
        return product;
    }
}