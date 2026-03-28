#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Ampere.Str.Cron;

/// <summary>
/// A high-performance, enterprise-grade cron expression parser supporting standard
/// 5-field (minute–DOW), 6-field (with seconds), and 7-field (with year) formats,
/// plus Quartz-style extensions (<c>L</c>, <c>W</c>, <c>LW</c>, <c>#</c>, <c>?</c>)
/// and predefined macros (<c>@yearly</c>, <c>@monthly</c>, etc.).
/// <para>
/// All public methods are thread-safe.  Parsed <see cref="CronExpression"/> instances
/// are immutable and may be cached freely.
/// </para>
/// </summary>
public static class CronParser
{
    //  Predefined macros

    private static readonly IReadOnlyDictionary<string, string> Macros =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["@yearly"]   = "0 0 1 1 *",
            ["@annually"] = "0 0 1 1 *",
            ["@monthly"]  = "0 0 1 * *",
            ["@weekly"]   = "0 0 * * 0",
            ["@daily"]    = "0 0 * * *",
            ["@midnight"] = "0 0 * * *",
            ["@hourly"]   = "0 * * * *",
        };

    //  Public API

    /// <summary>
    /// Parses a cron expression string into an immutable <see cref="CronExpression"/>.
    /// </summary>
    /// <param name="expression">A standard or extended cron string, or a macro such as <c>@daily</c>.</param>
    /// <returns>The parsed expression.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expression"/> is <c>null</c>.</exception>
    /// <exception cref="CronParseException">The expression has invalid syntax or out-of-range values.</exception>
    public static CronExpression Parse(string expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var trimmed = expression.Trim();
        if (trimmed.Length == 0)
            throw new CronParseException("Cron expression must not be empty.", expression);

        // Macro expansion
        if (trimmed.StartsWith('@'))
        {
            if (Macros.TryGetValue(trimmed, out var expanded))
                return ParseFields(expanded, expression);
            throw new CronParseException($"Unknown cron macro '{trimmed}'.", expression);
        }

        return ParseFields(trimmed, expression);
    }

    /// <summary>
    /// Attempts to parse a cron expression string.
    /// </summary>
    /// <param name="expression">The cron string to parse.</param>
    /// <param name="result">When the method returns <c>true</c>, contains the parsed expression.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    public static bool TryParse(string? expression, out CronExpression? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(expression)) return false;
        try
        {
            result = Parse(expression);
            return true;
        }
        catch (CronParseException)
        {
            return false;
        }
    }

    /// <summary>
    /// Extension method: parses this string as a cron expression.
    /// </summary>
    /// <example><code>"*/5 * * * *".ParseCron()</code></example>
    public static CronExpression ParseCron(this string expression) => Parse(expression);

    /// <summary>
    /// Extension method: attempts to parse this string as a cron expression.
    /// </summary>
    public static bool TryParseCron(this string expression, out CronExpression? result) =>
        TryParse(expression, out result);

    //  Internal parsing machinery

    private static CronExpression ParseFields(string normalized, string original)
    {
        var parts = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return parts.Length switch
        {
            5 => BuildExpression(null, parts[0], parts[1], parts[2], parts[3], parts[4], null, CronFormat.Standard, original),
            6 => BuildExpression(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5], null, CronFormat.WithSeconds, original),
            7 => BuildExpression(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5], parts[6], CronFormat.WithSecondsAndYear, original),
            _ => throw new CronParseException(
                $"Expected 5, 6, or 7 fields but found {parts.Length}.", original)
        };
    }

    private static CronExpression BuildExpression(
        string? secStr, string minStr, string hourStr,
        string domStr, string monthStr, string dowStr,
        string? yearStr, CronFormat format, string original)
    {
        var sec  = secStr  is not null ? ParseField(secStr, CronFieldType.Second, original)    : null;
        var min  = ParseField(minStr, CronFieldType.Minute, original);
        var hour = ParseField(hourStr, CronFieldType.Hour, original);
        var dom  = ParseField(domStr, CronFieldType.DayOfMonth, original);
        var mon  = ParseField(monthStr, CronFieldType.Month, original);
        var dow  = ParseField(dowStr, CronFieldType.DayOfWeek, original);
        var year = yearStr is not null ? ParseField(yearStr, CronFieldType.Year, original)     : null;

        return new CronExpression(sec, min, hour, dom, mon, dow, year, format, original);
    }

    /// <summary>
    /// Parses a single cron field (e.g. <c>1-5</c>, <c>*/15</c>, <c>MON-FRI</c>).
    /// </summary>
    internal static CronField ParseField(string fieldStr, CronFieldType type, string expression)
    {
        if (string.IsNullOrEmpty(fieldStr))
            throw new CronParseException($"{type} field must not be empty.", expression, fieldType: type);

        // Split on comma to get list elements
        var segments = fieldStr.Split(',');
        var nodes = new List<CronNode>(segments.Length);

        foreach (var segment in segments)
        {
            var trimmed = segment.Trim();
            if (trimmed.Length == 0)
                throw new CronParseException($"Empty element in {type} field '{fieldStr}'.", expression, fieldType: type);

            nodes.Add(ParseSingleElement(trimmed, type, expression));
        }

        return new CronField(type, nodes.AsReadOnly(), fieldStr);
    }

    private static CronNode ParseSingleElement(string element, CronFieldType type, string expression)
    {
        if (element == "?")
        {
            if (type is not (CronFieldType.DayOfMonth or CronFieldType.DayOfWeek))
                throw new CronParseException(
                    $"'?' is only valid in day-of-month or day-of-week fields, not {type}.", expression, fieldType: type);
            return new CronQuestionMarkNode();
        }

        if (element.Equals("LW", StringComparison.OrdinalIgnoreCase) ||
            element.Equals("WL", StringComparison.OrdinalIgnoreCase))
        {
            ValidateFieldTypeForSpecial(type, CronFieldType.DayOfMonth, "LW", expression);
            return new CronLastWeekdayNode();
        }

        if (element.Equals("L", StringComparison.OrdinalIgnoreCase))
        {
            if (type == CronFieldType.DayOfMonth) return new CronLastDayNode(0);
            if (type == CronFieldType.DayOfWeek) return new CronLastWeekdayOfMonthNode(6); // L alone in DOW = last Saturday
            throw new CronParseException($"'L' is not valid in {type} field.", expression, fieldType: type);
        }

        if (element.StartsWith("L-", StringComparison.OrdinalIgnoreCase))
        {
            ValidateFieldTypeForSpecial(type, CronFieldType.DayOfMonth, "L-offset", expression);
            var offsetStr = element[2..];
            if (!int.TryParse(offsetStr, NumberStyles.None, CultureInfo.InvariantCulture, out var offset) || offset < 0 || offset > 30)
                throw new CronParseException($"Invalid L offset '{offsetStr}' in {type} field.", expression, fieldType: type);
            return new CronLastDayNode(offset);
        }

        if (element.Length >= 2 && element.EndsWith('L'))
        {
            ValidateFieldTypeForSpecial(type, CronFieldType.DayOfWeek, "NL", expression);
            var dowStr = element[..^1];
            var dow = ResolveIntOrName(dowStr, type, expression);
            ValidateRange(dow, type, expression);
            return new CronLastWeekdayOfMonthNode(dow);
        }

        if (element.Length >= 2 && element.EndsWith('W'))
        {
            ValidateFieldTypeForSpecial(type, CronFieldType.DayOfMonth, "W", expression);
            var dayStr = element[..^1];
            if (!int.TryParse(dayStr, NumberStyles.None, CultureInfo.InvariantCulture, out var day) || day < 1 || day > 31)
                throw new CronParseException($"Invalid day '{dayStr}' in W expression.", expression, fieldType: type);
            return new CronNearestWeekdayNode(day);
        }

        var hashIndex = element.IndexOf('#');
        if (hashIndex >= 0)
        {
            ValidateFieldTypeForSpecial(type, CronFieldType.DayOfWeek, "#", expression);
            var dowPart = element[..hashIndex];
            var nthPart = element[(hashIndex + 1)..];
            var dow = ResolveIntOrName(dowPart, type, expression);
            ValidateRange(dow, type, expression);
            if (!int.TryParse(nthPart, NumberStyles.None, CultureInfo.InvariantCulture, out var nth) || nth < 1 || nth > 5)
                throw new CronParseException($"Nth value must be 1–5 in '#' expression, got '{nthPart}'.", expression, fieldType: type);
            return new CronHashNode(dow, nth);
        }

        var slashIndex = element.IndexOf('/');
        if (slashIndex >= 0)
        {
            var basePart = element[..slashIndex];
            var stepPart = element[(slashIndex + 1)..];
            if (!int.TryParse(stepPart, NumberStyles.None, CultureInfo.InvariantCulture, out var step) || step < 1)
                throw new CronParseException($"Step value must be a positive integer, got '{stepPart}'.", expression, fieldType: type);

            CronNode baseNode;
            if (basePart == "*")
            {
                baseNode = new CronWildcardNode();
            }
            else
            {
                var dashIdx = basePart.IndexOf('-');
                if (dashIdx >= 0)
                {
                    var lo = ResolveIntOrName(basePart[..dashIdx], type, expression);
                    var hi = ResolveIntOrName(basePart[(dashIdx + 1)..], type, expression);
                    ValidateRange(lo, type, expression);
                    ValidateRange(hi, type, expression);
                    if (lo > hi)
                        throw new CronParseException($"Range start {lo} is greater than end {hi}.", expression, fieldType: type);
                    baseNode = new CronRangeNode(lo, hi);
                }
                else
                {
                    var val = ResolveIntOrName(basePart, type, expression);
                    ValidateRange(val, type, expression);
                    baseNode = new CronValueNode(val);
                }
            }
            return new CronStepNode(baseNode, step);
        }

        var dashIndex = element.IndexOf('-');
        if (dashIndex >= 0)
        {
            var lo = ResolveIntOrName(element[..dashIndex], type, expression);
            var hi = ResolveIntOrName(element[(dashIndex + 1)..], type, expression);
            ValidateRange(lo, type, expression);
            ValidateRange(hi, type, expression);
            if (lo > hi)
                throw new CronParseException($"Range start {lo} is greater than end {hi}.", expression, fieldType: type);
            return new CronRangeNode(lo, hi);
        }

        if (element == "*") return new CronWildcardNode();

        var value = ResolveIntOrName(element, type, expression);
        ValidateRange(value, type, expression);
        return new CronValueNode(value);
    }

    //  Helpers

    /// <summary>
    /// Resolves an element string to an integer value, handling named months (JAN-DEC)
    /// and days of week (SUN-SAT), as well as Sunday == 7 normalisation.
    /// </summary>
    private static int ResolveIntOrName(string token, CronFieldType type, string expression)
    {
        if (int.TryParse(token, NumberStyles.None, CultureInfo.InvariantCulture, out var num))
        {
            // Normalize 7 → 0 for day-of-week (both 0 and 7 mean Sunday)
            if (type == CronFieldType.DayOfWeek && num == 7) num = 0;
            return num;
        }

        if (type == CronFieldType.Month && CronField.MonthNames.TryGetValue(token, out var monthVal))
            return monthVal;

        if (type == CronFieldType.DayOfWeek && CronField.DayOfWeekNames.TryGetValue(token, out var dowVal))
            return dowVal;

        throw new CronParseException(
            $"Cannot parse '{token}' as a value for {type} field.", expression, fieldType: type);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ValidateRange(int value, CronFieldType type, string expression)
    {
        var (min, max) = CronField.GetFieldRange(type);
        if (value < min || value > max)
            throw new CronParseException(
                $"Value {value} is out of range [{min}–{max}] for {type} field.", expression, fieldType: type);
    }

    private static void ValidateFieldTypeForSpecial(CronFieldType actual, CronFieldType expected, string feature, string expression)
    {
        if (actual != expected)
            throw new CronParseException(
                $"'{feature}' is only valid in {expected} field, not {actual}.", expression, fieldType: actual);
    }
}
