using System.Runtime.CompilerServices;
using NumericalMath.Geometry.Structures;

namespace NumericalMath.Tests.Geometry.Structures;

[TestFixture]
public class VertexTests
{
    [Test]
    public void Vertex2Constructor_WhenUsingDefaultArguments_ShouldHaveOriginVertex2()
    {
        var vertex2 = new Vertex2();
        Assert.That(vertex2.X, Is.EqualTo(0).Within(1e-8f));
        Assert.That(vertex2.Y, Is.EqualTo(0).Within(1e-8f));
    }

    [Test]
    public void Vertex2Constructor_WhenUsingValuedArguments_ShouldHaveValuedProperties()
    {
        var vertex2 = new Vertex2(1, 2);
        Assert.That(vertex2.X, Is.EqualTo(1).Within(1e-8f));
        Assert.That(vertex2.Y, Is.EqualTo(2).Within(1e-8f));
    }

    [Test]
    public void Vertex2_WhenCreated_ShouldHaveAlignedByteSize()
    {
        Assert.That(Unsafe.SizeOf<Vertex2>(), Is.EqualTo(8));
    }

    [Test]
    public void Vertex2ToString_WhenCalled_ShouldHaveTheCorrectOrderIndices()
    {
        var vertex2 = new Vertex2(1, 2);
        Assert.That(vertex2.ToString(), Does.Contain($"{1f}, {2f}"));
    }

    [Test]
    public void Vertex3Constructor_WhenUsingDefaultArguments_ShouldHaveOriginVertex2()
    {
        var vertex3 = new Vertex3();
        Assert.That(vertex3.X, Is.EqualTo(0));
        Assert.That(vertex3.Y, Is.EqualTo(0));
        Assert.That(vertex3.Z, Is.EqualTo(0));
    }

    [Test]
    public void Vertex3Constructor_WhenUsingValuedArguments_ShouldHaveValuedProperties()
    {
        var vertex3 = new Vertex3(1, 2, 3);
        Assert.That(vertex3.X, Is.EqualTo(1));
        Assert.That(vertex3.Y, Is.EqualTo(2));
        Assert.That(vertex3.Z, Is.EqualTo(3));
    }

    [Test]
    public void Vertex3_WhenCreated_ShouldHaveAlignedByteSize()
    {
        Assert.That(Unsafe.SizeOf<Vertex3>(), Is.EqualTo(12));
    }
    
    [Test]
    public void Vertex3ToString_WhenCalled_ShouldHaveTheCorrectOrderIndices()
    {
        var vertex3 = new Vertex3(1, 2, 3);
        Assert.That(vertex3.ToString(), Does.Contain($"{1f}, {2f}, {3f}"));
    }
}