#nullable enable

using System;

namespace Ampere.Statistics;

/// <summary>
/// An exception that is thrown when the data set is not large enough to compute a statistic.
/// </summary>
public class InsufficientDataSetException : Exception
{
    /// <summary>
    /// Creates a new InsufficientDataSetException.
    /// </summary>
    public InsufficientDataSetException() { }

    /// <summary>
    /// Creates a new InsufficientDataSetException containing a message.
    /// </summary>
    /// <param name="message">The message of this exception type</param>
    public InsufficientDataSetException(string message) : base(message) { }

    /// <summary>
    /// Creates a new InsufficientDataSetException containing a message and an inner Exception.
    /// </summary>
    /// <param name="message">The message of this exception type</param>
    /// <param name="inner">The inner Exception</param>
    public InsufficientDataSetException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// An exception that is thrown when no mode exists in the data set.
/// </summary>
public class NoModeException : Exception
{
    /// <summary>
    /// Creates a new NoModeException.
    /// </summary>
    public NoModeException() { }

    /// <summary>
    /// Creates a new NoModeException containing a message.
    /// </summary>
    /// <param name="message">The message of this exception type</param>
    public NoModeException(string message) : base(message) { }

    /// <summary>
    /// Creates a new NoModeException containing a message and an inner Exception.
    /// </summary>
    /// <param name="message">The message of this exception type</param>
    /// <param name="inner">The inner Exception</param>
    public NoModeException(string message, Exception inner) : base(message, inner) { }
}
