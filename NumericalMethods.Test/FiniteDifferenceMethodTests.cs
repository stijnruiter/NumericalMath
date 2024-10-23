using LinearAlgebra.Comparers;
using LinearAlgebra.Structures;
using System;
using System.Linq;

namespace NumericalMethods.Test;

public class FiniteDifferenceMethodTests
{
    [Test]
    /**
     * Poisson's Equations
     * -u''(x) = f(x)
     * 
     * f(x) = -6*x
     * u(0) = 5
     * u(1) = 3
     * 
     * Solution:
     * u(x) = x^3 + (u(1) - 6) * x + u(0)
     *      = x^3 - 3 * x + 5
     */
    public void PoissonsEquation1D()
    {
        float x1 = 0f;
        float x2 = 1f;
        
        int nElements = 4;
        int nNodes = nElements + 1;

        float dx = (x2 - x1) / nElements;

        float[] nodes = Enumerable.Range(0, nNodes).Select(i => i * dx).ToArray();
        Assert.That(nodes.Length, Is.EqualTo(nNodes));

        float force(float x) => -6 * x;

        Matrix<float> A = new Matrix<float>(nNodes, nNodes);
        for (int i = 1; i < nNodes - 1; i++)
        {
            A[i, i - 1] = -1 / dx / dx;
            A[i, i    ] =  2 / dx / dx;
            A[i, i + 1] = -1 / dx / dx;
        }

        ColumnVector<float> F = new ColumnVector<float>(nodes.Select(force).ToArray());

        // Apply Dirichlet condition at u(0) = 5
        A[0, 0] = 1;
        F[0] = 5;

        // Apply Dirichlet condition at u(1) = 3
        A[nNodes - 1, nNodes - 1] = 1;
        F[nNodes - 1] = 3;

        ColumnVector<float> U = A.Solve(F);

        ColumnVector<float> expectedU = new(nodes.Select(x => x * x * x - 3 * x + 5).ToArray());
        Assert.That(U, Is.EqualTo(expectedU));
    }

    [Test]
    /**
     * Helmholtz equation
     * 
     * u''(x) = - k^2 * u(x)
     * 
     * k = 1
     * u(0) = 5
     * u(pi/2) = 0
     * 
     * Solution: 
     * u(x) = a * cos(k * x) + b * sin(k * x)
     *      = 5 * cos(x)
     */
    public void HelmholtzEquation()
    {
        double x1 = 0;
        double x2 = MathF.PI / 2;

        int nElements = 200;
        int nNodes = nElements + 1;
        double dx = (x2 - x1) / nElements;
        double[] nodes = Enumerable.Range(0, nNodes).Select(i => i * dx).ToArray();
        Assert.That(nodes.Length, Is.EqualTo(nNodes));
        
        Matrix<double> A = new Matrix<double>(nNodes, nNodes);
        for (int i = 1; i < nNodes - 1; i++)
        {
            A[i, i - 1] = 1 / dx / dx;
            A[i, i] = (1 - 2 / dx / dx);
            A[i, i + 1] = 1 / dx / dx;
        }
        ColumnVector<double> F = new ColumnVector<double>(nNodes);

        // Dirichlet boundary condition at u(0)    = 5
        // Dirichlet boundary condition at u(pi/2) = 0

        A[0, 0] = 1;
        F[0] = 5;

        A[nNodes - 1, nNodes - 1] = 1;
        F[nNodes - 1] = 0;

        ColumnVector<double> U = A.Solve(F);
        ColumnVector<double> expectedU = new(nodes.Select(x => 5 * Math.Cos(x)).ToArray());

        Assert.That(U, Is.EqualTo(expectedU).Using<ColumnVector<double>>((a,b) => a.ApproxEquals(b, 1e-5)));
    }
}