using NumericalMath.Geometry.Structures;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NumericalMath.Tests.Geometry;

[TestFixture]
public class SimplexElementsTests
{
    [Test]
    public void LineElement()
    {
        var element = new LineElement();
        Assert.That(element.I, Is.EqualTo(0));
        Assert.That(element.J, Is.EqualTo(0));

        element = new LineElement(5, 6);
        Assert.That(element.I, Is.EqualTo(5));
        Assert.That(element.J, Is.EqualTo(6));

        Assert.That(Unsafe.SizeOf<LineElement>(), Is.EqualTo(8));
    }

    [Test]
    public void TriangleElement()
    {
        var element = new TriangleElement();
        Assert.That(element.I, Is.EqualTo(0));
        Assert.That(element.J, Is.EqualTo(0));
        Assert.That(element.K, Is.EqualTo(0));

        element = new TriangleElement(5, 6, 7);
        Assert.That(element.I, Is.EqualTo(5));
        Assert.That(element.J, Is.EqualTo(6));
        Assert.That(element.K, Is.EqualTo(7));

        Assert.That(Unsafe.SizeOf<TriangleElement>(), Is.EqualTo(12));
    }

    [Test]
    public void TetrahedronElement()
    {
        var element = new TetrahedronElement();
        Assert.That(element.I, Is.EqualTo(0));
        Assert.That(element.J, Is.EqualTo(0));
        Assert.That(element.K, Is.EqualTo(0));
        Assert.That(element.L, Is.EqualTo(0));

        element = new TetrahedronElement(5, 6, 7, 8);
        Assert.That(element.I, Is.EqualTo(5));
        Assert.That(element.J, Is.EqualTo(6));
        Assert.That(element.K, Is.EqualTo(7));
        Assert.That(element.L, Is.EqualTo(8));

        Assert.That(Unsafe.SizeOf<TetrahedronElement>(), Is.EqualTo(16));
    }

    [Test]
    public void Vertex2()
    {
        var vertex2 = new Vertex2();
        Assert.That(vertex2.X, Is.EqualTo(0));
        Assert.That(vertex2.Y, Is.EqualTo(0));

        vertex2 = new Vertex2(1, 2);
        Assert.That(vertex2.X, Is.EqualTo(1));
        Assert.That(vertex2.Y, Is.EqualTo(2));

        Assert.That(Unsafe.SizeOf<Vertex2>(), Is.EqualTo(8));
    }

    [Test]
    public void Vertex3()
    {
        var vertex3 = new Vertex3();
        Assert.That(vertex3.X, Is.EqualTo(0));
        Assert.That(vertex3.Y, Is.EqualTo(0));
        Assert.That(vertex3.Z, Is.EqualTo(0));

        vertex3 = new Vertex3(1, 2, 3);
        Assert.That(vertex3.X, Is.EqualTo(1));
        Assert.That(vertex3.Y, Is.EqualTo(2));
        Assert.That(vertex3.Z, Is.EqualTo(3));

        Assert.That(Unsafe.SizeOf<Vertex3>(), Is.EqualTo(12));
    }

    [Test]
    public void RectElement()
    {
        var rect = new Rectangle();
        Assert.Multiple(() =>
        {
            Assert.That(rect.Left, Is.EqualTo(0f));
            Assert.That(rect.Right, Is.EqualTo(0f));
            Assert.That(rect.Bottom, Is.EqualTo(0f));
            Assert.That(rect.Top, Is.EqualTo(0f));
            Assert.That(rect.Width, Is.EqualTo(0f));
            Assert.That(rect.Height, Is.EqualTo(0f));
        });
    }

    [TestCase(5, 10, -10, 5, 5, 15)]
    [TestCase(5, -10, 5, -5, -15, -10)]
    public void RectElement(float left, float right, float bottom, float top, float width, float height)
    {
        var rect = new Rectangle(left, right, bottom, top);
        Assert.Multiple(() =>
        {
            Assert.That(rect.Left, Is.EqualTo(left));
            Assert.That(rect.Right, Is.EqualTo(right));
            Assert.That(rect.Bottom, Is.EqualTo(bottom));
            Assert.That(rect.Top, Is.EqualTo(top));
            Assert.That(rect.Width, Is.EqualTo(width));
            Assert.That(rect.Height, Is.EqualTo(height));
        });
    }

    [Test]
    public void RectCorners()
    {
        var rect = new Rectangle(0, 1, 2, 3);
        var vertices = rect.ToVertices().ToArray();
        Assert.That(vertices, Has.Length.EqualTo(4));
        Assert.That(vertices[0], Is.EqualTo(new Vertex2(0, 2)));
        Assert.That(vertices[1], Is.EqualTo(new Vertex2(1, 2)));
        Assert.That(vertices[2], Is.EqualTo(new Vertex2(1, 3)));
        Assert.That(vertices[3], Is.EqualTo(new Vertex2(0, 3)));
    }

    [TestCase(5, 0f)]
    [TestCase(25, 0f)]
    [TestCase(5, 5f)]
    [TestCase(25, 5f)]
    public void RectBoundingBox(int nPoints, float dilate)
    {
        var x = GetFloats(nPoints).ToArray();
        var y = GetFloats(nPoints).ToArray();
        var expectedRectangle = new Rectangle(x.Min() - dilate, x.Max() + dilate, y.Min() - dilate, y.Max() + dilate);

        var vertices = x.Zip(y).Select(pair => new Vertex2(pair.First, pair.Second)).ToArray();
        var boundingBox = Rectangle.BoundingBox(vertices, dilate);
        Assert.That(boundingBox.Left, Is.EqualTo(expectedRectangle.Left).Within(1e-5));
        Assert.That(boundingBox.Right, Is.EqualTo(expectedRectangle.Right).Within(1e-5));
        Assert.That(boundingBox.Bottom, Is.EqualTo(expectedRectangle.Bottom).Within(1e-5));
        Assert.That(boundingBox.Top, Is.EqualTo(expectedRectangle.Top).Within(1e-5));

    }

    private IEnumerable<float> GetFloats(int nPoints)
    {
        for (int i = 0; i < nPoints; i++)
        {
            yield return TestContext.CurrentContext.Random.NextFloat(-5f, 5f);
        }
    }
}
