using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NumericalMath.Geometry.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct HalfEdge
{
    public int V1 = -1;
    public int V2 = -1;
    public int PrevEdge = -1;
    public int NextEdge = -1;
    public int TwinEdge = -1;
    public int ElementIndex = -1;

    public HalfEdge(int v1, int v2, int prevEdge, int nextEdge, int twinEdge, int elementIndex)
    {
        V1 = v1;
        V2 = v2;
        PrevEdge = prevEdge;
        NextEdge = nextEdge;
        TwinEdge = twinEdge;
        ElementIndex = elementIndex;
    }

    public override string ToString() =>
        $"Face {ElementIndex}: prev(e): {PrevEdge}, next(e): {NextEdge}, twin(e): {TwinEdge}. ({V1},{V2})";
}

public class HalfEdgeElement : IEnumerable<HalfEdge>
{
    private readonly IReadOnlyList<HalfEdge> _edges;
    private readonly int _firstFirstEdgeIndex;
    public HalfEdgeElement(IReadOnlyList<HalfEdge> halfEdges, int firstEdgeIndex)
    {
        if (firstEdgeIndex < 0 || firstEdgeIndex >= halfEdges.Count)
            throw new IndexOutOfRangeException($"Index {firstEdgeIndex} not in range, Count: {halfEdges.Count}.");
        _edges = halfEdges;
        _firstFirstEdgeIndex = firstEdgeIndex;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<HalfEdge> GetEnumerator() => new HalfEdgeEnumerator(_edges, _firstFirstEdgeIndex);
}

public class HalfEdgeEnumerator(IReadOnlyList<HalfEdge> halfEdges, int index) : IEnumerator<HalfEdge>
{
    public bool MoveNext()
    {
        if (_position == -1)
        {
            _position = _index;
            return _position >= 0 && _position < _halfEdges.Count;
        }

        _position = _halfEdges[_position].NextEdge;
        return _position != _index;
    }

    public void Reset()
    {
        _position = -1;
    }

    public HalfEdge Current => _halfEdges[_position];

    object? IEnumerator.Current => Current;
    private int _position = -1;
    private readonly IReadOnlyList<HalfEdge> _halfEdges = halfEdges;
    private readonly int _index = index;

    public void Dispose()
    {
    }
}