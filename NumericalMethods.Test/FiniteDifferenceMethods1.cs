using LinearAlgebra.Comparers;
using LinearAlgebra.Structures;
using System;
using System.Linq;

namespace NumericalMethods.Test;

[TestFixture]
public class FiniteDifferenceMethods1
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
        Domain domain = new Domain(0, 1, 5);
        double force(double x) => -6 * x;

        // u'' => Stencil becomes 1/h^2 [-1, 2, -1] 
        Matrix<double> A = Matrix<double>.Tridiagonal(domain.NodeCount, -1, 2, -1) * (1f / (domain.DeltaX * domain.DeltaX));
        ColumnVector<double> F = new ColumnVector<double>(domain.Nodes.Select(force).ToArray());

        // Apply Dirichlet condition at u(0) = 5
        domain.ApplyDirichletBoundaryConditionLeft(5, A, F);
        // Apply Dirichlet condition at u(1) = 3
        domain.ApplyDirichletBoundaryConditionRight(3, A, F);

        ColumnVector<double> U = A.Solve(F);

        ColumnVector<double> expectedU = new(domain.Nodes.Select(x => x * x * x - 3 * x + 5).ToArray());
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
        Domain domain = new Domain(0, MathF.PI / 2, 201);
        // u'' + u => Stencil becomes 1/h^2[1, -2, 1] + [0, 1, 0] = [1/h^2, 1-2/h^2, 1/h^2]
        double one_h2 = 1d / domain.DeltaX / domain.DeltaX;
        Matrix<double> A = one_h2 * Matrix<double>.Tridiagonal(domain.NodeCount, 1, -2, 1) + Matrix<double>.Identity(domain.NodeCount);
        ColumnVector<double> F = ColumnVector<double>.Zero(domain.NodeCount);

        // Dirichlet boundary condition at u(0)    = 5
        // Dirichlet boundary condition at u(pi/2) = 0
        domain.ApplyDirichletBoundaryConditionLeft(5, A, F);
        domain.ApplyDirichletBoundaryConditionRight(0, A, F);

        ColumnVector<double> U = A.Solve(F);
        ColumnVector<double> expectedU = new(domain.Nodes.Select(x => 5 * Math.Cos(x)).ToArray());

        Assert.That(U, Is.EqualTo(expectedU).Using<ColumnVector<double>>((a,b) => a.ApproxEquals(b, 1e-5)));
    }
}