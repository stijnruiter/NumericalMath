using System;

namespace LinearAlgebra.Exceptions;

public enum NonInvertibleReason
{
    Unkown,
    Singular, 
    Degenerate,
}

public class NotInvertibleException : Exception
{
    public NotInvertibleException(NonInvertibleReason reason = NonInvertibleReason.Unkown) : base($"Reason: {reason}") { }
}
