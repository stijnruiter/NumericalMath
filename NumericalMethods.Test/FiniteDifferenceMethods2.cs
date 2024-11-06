using LinearAlgebra;
using LinearAlgebra.Structures;
using System;
using System.Linq;

namespace NumericalMethods.Test;

[TestFixture]
public class FiniteDifferenceMethods2
{
    [Test, Ignore("2D-problem takes too long in debug builds")]
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

        double L = 1;
        double H = 3;

        double lambda1 = 2;
        double lambda2 = 3;

        double k2 = lambda1 * lambda1 + lambda2 * lambda2;


        double hx = L / (nNodesX - 1);
        double hy = H / (nNodesY - 1);

        double[] NodesX = Enumerable.Range(0, nNodesX).Select(i => i * hx).ToArray();
        double[] NodesY = Enumerable.Range(0, nNodesY).Select(i => i * hy).ToArray();

        Matrix<double> Ax = Matrix<double>.Tridiagonal(nNodesX, 1d / hx / hx, -2d / hx / hx, 1d / hx / hx);
        Matrix<double> Ay = Matrix<double>.Tridiagonal(nNodesY, 1d / hy / hy, -2d / hy / hy, 1d / hy / hy);

        Matrix<double> Ix = Matrix<double>.Identity(nNodesX);
        Matrix<double> Iy = Matrix<double>.Identity(nNodesY);

        Matrix<double> A = Matrix<double>.Zero(nNodesX * nNodesY); 
        Matrix<double> B = (Matrix<double>)Arithmetics.TensorProduct(Ax, Iy) + (Matrix<double>)Arithmetics.TensorProduct(Ix, Ay);
        ColumnVector<double> F = ColumnVector<double>.Zero(nNodesX * nNodesY);
        ColumnVector<double> Uexact = ColumnVector<double>.Zero(nNodesX * nNodesY);
        double UexactFunc(double x, double y) => 2 * Math.Sin(lambda1 * x) * Math.Sin(lambda2 * y);

        for (int i = 0; i < nNodesX; i++)
        {
            for (int j = 0; j < nNodesY; j++)
            {
                int index = i * nNodesY + j;
                Uexact[index] = UexactFunc(NodesX[i], NodesY[j]);

                if (i == 0 || i == (nNodesX - 1))
                {
                    A[index, index] = 1d;
                    F[index] = Uexact[index];
                    continue;
                }
                if (j == 0 || j == (nNodesY - 1))
                {
                    A[index, index] = 1d;
                    F[index] = Uexact[index];
                    continue;
                }

                A[index, index] = k2 - 2d / hx / hx - 2d / hy / hy; // (i, j)

                if (j > 0)
                {
                    A[index, index - 1] = 1d / hy / hy; // (i, j - 1)
                }
                if (j < nNodesY - 1)
                {
                    A[index, index + 1] = 1d / hy / hy; // (i, j + 1)
                }

                if (i > 0)
                {
                    A[index, index - nNodesY] = 1d / hx / hx; // (i - 1, j)
                }
                if (i < nNodesX - 1)
                {
                    A[index, index + nNodesY] = 1d / hx / hx; // (i + 1, j)
                }
            }
        }

        ColumnVector<double> U = A.Solve(F);

        Assert.That(Arithmetics.Norm2(U - Uexact), Is.EqualTo(0.22188).Within(1e-5));
        Assert.That(Arithmetics.NormInf(U - Uexact), Is.EqualTo(0.012059).Within(1e-5));
    }
}
