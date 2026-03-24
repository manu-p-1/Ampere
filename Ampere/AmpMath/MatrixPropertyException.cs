using System;

#nullable enable

namespace Ampere.AmpMath;

/// <summary>
/// An exception that occurs if a Matrix property is violated
/// when examining certain properties at runtime.
/// </summary>
public class MatrixPropertyException : Exception
{
    /// <summary>
    /// Creates a new MatrixPropertyException.
    /// </summary>
    public MatrixPropertyException()
    { }

    /// <summary>
    /// Creates a new overloaded MatrixPropertyException containing a message.
    /// </summary>
    /// <param name="message">The message of this exception type</param>
    public MatrixPropertyException(string message) : base(message) { }

    /// <summary>
    /// Creates a new overloaded MatrixPropertyException containing a message and an inner Exception.
    /// </summary>
    /// <param name="message">The message of this exception type</param>
    /// <param name="inner">The inner Exception</param>
    public MatrixPropertyException(string message, Exception inner) : base(message, inner) { }
}
