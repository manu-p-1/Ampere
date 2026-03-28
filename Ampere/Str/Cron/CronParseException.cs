#nullable enable
using System;

namespace Ampere.Str.Cron;

/// <summary>
/// Exception thrown when a cron expression cannot be parsed or validated.
/// Provides detailed information about the parse failure including position and field context.
/// </summary>
public sealed class CronParseException : FormatException
{
    /// <summary>
    /// Gets the raw cron expression that failed to parse.
    /// </summary>
    public string Expression { get; }

    /// <summary>
    /// Gets the zero-based position in the expression where the error was detected, or -1 if not applicable.
    /// </summary>
    public int Position { get; }

    /// <summary>
    /// Gets the field that was being parsed when the error occurred, if applicable.
    /// </summary>
    public CronFieldType? FieldType { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="CronParseException"/>.
    /// </summary>
    /// <param name="message">A description of the parse error.</param>
    /// <param name="expression">The raw cron expression.</param>
    /// <param name="position">The zero-based character position of the error, or -1.</param>
    /// <param name="fieldType">The field being parsed when the error occurred.</param>
    public CronParseException(string message, string expression, int position = -1, CronFieldType? fieldType = null)
        : base(message)
    {
        Expression = expression;
        Position = position;
        FieldType = fieldType;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CronParseException"/> with an inner exception.
    /// </summary>
    public CronParseException(string message, string expression, Exception innerException,
        int position = -1, CronFieldType? fieldType = null)
        : base(message, innerException)
    {
        Expression = expression;
        Position = position;
        FieldType = fieldType;
    }
}
