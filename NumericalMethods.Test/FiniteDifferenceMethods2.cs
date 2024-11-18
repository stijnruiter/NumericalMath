using LinearAlgebra;
using LinearAlgebra.Structures;
using System;
using System.Linq;

namespace NumericalMethods.Test;

[TestFixture]
public class FiniteDifferenceMethods2
{
    //[Test, Ignore("2f-problem takes too long in debug builds")]
    [Test]
    public void HelmholtzEquation()
    {
        /**
         * u'' + k^2 * u = 0
         * 
         * u(x,0) = 0
         * u(x,1) = 0
         * u(0,y) = 0
         * u(1,y) = 0
         * 
         * k^2 = pi^2/L^2 + pi^2/H^2
         * 
         * Solution:
         * u(x,y) = 2 * sin (pi * x / L) * sin (pi * y / H)
         * 
         */

        int nNodesX = 50;
        int nNodesY = nNodesX;

        float L = 1;
        float H = 3;

        float lambda1 = 2;
        float lambda2 = 3;

        float k2 = lambda1 * lambda1 + lambda2 * lambda2;


        float hx = L / (nNodesX - 1);
        float hy = H / (nNodesY - 1);

        float[] NodesX = Enumerable.Range(0, nNodesX).Select(i => i * hx).ToArray();
        float[] NodesY = Enumerable.Range(0, nNodesY).Select(i => i * hy).ToArray();

        Matrix<float> Ax = Matrix<float>.Tridiagonal(nNodesX, 1f / hx / hx, -2f / hx / hx, 1f / hx / hx);
        Matrix<float> Ay = Matrix<float>.Tridiagonal(nNodesY, 1f / hy / hy, -2f / hy / hy, 1f / hy / hy);

        Matrix<float> Ix = Matrix<float>.Identity(nNodesX);
        Matrix<float> Iy = Matrix<float>.Identity(nNodesY);

        Matrix<float> A = Matrix<float>.Zero(nNodesX * nNodesY); 
        Matrix<float> B = Matrix<float>.TensorProduct(Ax, Iy) + Matrix<float>.TensorProduct(Ix, Ay);
        ColumnVector<float> F = ColumnVector<float>.Zero(nNodesX * nNodesY);
        ColumnVector<float> Uexact = ColumnVector<float>.Zero(nNodesX * nNodesY);
        float UexactFunc(float x, float y) => 2 * MathF.Sin(lambda1 * x) * MathF.Sin(lambda2 * y);

        for (int i = 0; i < nNodesX; i++)
        {
            for (int j = 0; j < nNodesY; j++)
            {
                int index = i * nNodesY + j;
                Uexact[index] = UexactFunc(NodesX[i], NodesY[j]);

                if (i == 0 || i == (nNodesX - 1))
                {
                    A[index, index] = 1f;
                    F[index] = Uexact[index];
                    continue;
                }
                if (j == 0 || j == (nNodesY - 1))
                {
                    A[index, index] = 1f;
                    F[index] = Uexact[index];
                    continue;
                }

                A[index, index] = k2 - 2f / hx / hx - 2f / hy / hy; // (i, j)

                if (j > 0)
                {
                    A[index, index - 1] = 1f / hy / hy; // (i, j - 1)
                }
                if (j < nNodesY - 1)
                {
                    A[index, index + 1] = 1f / hy / hy; // (i, j + 1)
                }

                if (i > 0)
                {
                    A[index, index - nNodesY] = 1f / hx / hx; // (i - 1, j)
                }
                if (i < nNodesX - 1)
                {
                    A[index, index + nNodesY] = 1f / hx / hx; // (i + 1, j)
                }
            }
        }

        ColumnVector<float> U = A.Solve(F);

        Assert.That((U - Uexact).Norm2(), Is.EqualTo(0.190595f).Within(1e-5f));
        Assert.That((U - Uexact).NormInf(), Is.EqualTo(0.012213f).Within(1e-5f));
    }
}
