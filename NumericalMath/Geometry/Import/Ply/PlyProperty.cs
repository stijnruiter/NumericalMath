using System;

namespace NumericalMath.Geometry.Import.Ply;

public sealed class PlyProperty
{
    public string Name { get; }
    public Type DataType { get; }
    public bool IsList { get; }
    public Type? CountType { get; }

    public PlyProperty(string name, Type dataType)
    {
        Name = name;
        DataType = dataType;
        IsList = false;
    }

    public PlyProperty(string name, Type countType, Type dataType)
    {
        Name = name;
        CountType = countType;
        DataType = dataType;
        IsList = true;
    }
    
    public static Type MapPlyType(string plyType) => plyType switch
    {
        "char" or "int8"      => typeof(sbyte),
        "uchar" or "uint8"    => typeof(byte),
        "short" or "int16"    => typeof(short),
        "ushort" or "uint16"  => typeof(ushort),
        "int" or "int32"      => typeof(int),
        "uint" or "uint32"    => typeof(uint),
        "float" or "float32"  => typeof(float),
        "double" or "float64" => typeof(double),
        _ => throw new NotSupportedException($"Unsupported PLY type: {plyType}")
    };

}
