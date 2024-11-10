using LinearAlgebra.Structures;
using System.Linq;
using System.Numerics;

namespace NumericalMethods;

public struct Domain
{
    public readonly double Left;
    public readonly double Right;
    public readonly double DeltaX;
    public readonly double[] Nodes;

    public int NodeCount => Nodes.Length;

    public Domain(double left, double right, int nodeCount)
    {
        Left = left;
        Right = right;
        double dx = (right - left) / (nodeCount - 1);
        DeltaX = dx;
        Nodes = Enumerable.Range(0, nodeCount).Select(i => i * dx).ToArray();
    }

    public void ApplyDirichletBoundaryConditionLeft<T>(T value, Matrix<T> A, ColumnVector<T> F) where T : struct, INumber<T>
    {
        for (int i = 1; i < NodeCount; i++)
        {
            A[0, i] = T.Zero;
        }
        A[0, 0] = T.One;
        F[0] = value;
    }

    public void ApplyDirichletBoundaryConditionRight<T>(T value, Matrix<T> A, ColumnVector<T> F) where T : struct, INumber<T>
    {
        for (int i = 0; i < NodeCount - 1; i++)
        {
            A[NodeCount - 1, i] = T.Zero;
        }
        A[NodeCount - 1, NodeCount - 1] = T.One;
        F[NodeCount - 1] = value;
    }
}
