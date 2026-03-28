#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ampere.Str.Cron;

/// <summary>
/// Identifies which field of a cron expression a <see cref="CronField"/> represents.
/// </summary>
public enum CronFieldType
{
    /// <summary>Seconds field (0–59). Present only in extended cron formats.</summary>
    Second,
    /// <summary>Minutes field (0–59).</summary>
    Minute,
    /// <summary>Hours field (0–23).</summary>
    Hour,
    /// <summary>Day-of-month field (1–31).</summary>
    DayOfMonth,
    /// <summary>Month field (1–12).</summary>
    Month,
    /// <summary>Day-of-week field (0–6, where 0 = Sunday).</summary>
    DayOfWeek,
    /// <summary>Year field (1970–2099). Present only in extended cron formats.</summary>
    Year
}

/// <summary>
/// Specifies the number of fields in a cron expression.
/// </summary>
public enum CronFormat
{
    /// <summary>Standard 5-field format: minute hour day-of-month month day-of-week.</summary>
    Standard,
    /// <summary>6-field format with seconds: second minute hour day-of-month month day-of-week.</summary>
    WithSeconds,
    /// <summary>7-field format: second minute hour day-of-month month day-of-week year.</summary>
    WithSecondsAndYear
}

// AST node hierarchy – each record represents one syntactic
// element between commas inside a single cron field.

/// <summary>
/// Abstract base for all nodes in a cron-field AST.
/// </summary>
public abstract record CronNode
{
    /// <summary>
    /// Expands this node into the discrete set of integer values it represents
    /// within the given inclusive range [<paramref name="min"/>, <paramref name="max"/>].
    /// For context-dependent nodes (L, W, #), callers should use the overload that
    /// accepts year and month.
    /// </summary>
    internal abstract IEnumerable<int> Expand(int min, int max);

    /// <summary>
    /// Expands this node with date context for L / W / # resolution.
    /// The default implementation delegates to <see cref="Expand(int,int)"/>.
    /// </summary>
    internal virtual IEnumerable<int> Expand(int min, int max, int year, int month)
        => Expand(min, max);

    /// <summary>Returns the canonical cron-syntax representation of this node.</summary>
    public abstract string ToCronString();
}

/// <summary>Wildcard (<c>*</c>) – matches every value in the field's range.</summary>
public sealed record CronWildcardNode : CronNode
{
    internal override IEnumerable<int> Expand(int min, int max)
    {
        for (var i = min; i <= max; i++) yield return i;
    }

    public override string ToCronString() => "*";
}

/// <summary>Question mark (<c>?</c>) – "no specific value", used in day-of-month / day-of-week.</summary>
public sealed record CronQuestionMarkNode : CronNode
{
    internal override IEnumerable<int> Expand(int min, int max)
    {
        for (var i = min; i <= max; i++) yield return i;
    }

    public override string ToCronString() => "?";
}

/// <summary>A single integer value (e.g. <c>5</c>).</summary>
public sealed record CronValueNode(int Value) : CronNode
{
    internal override IEnumerable<int> Expand(int min, int max)
    {
        if (Value >= min && Value <= max) yield return Value;
    }

    public override string ToCronString() => Value.ToString(CultureInfo.InvariantCulture);
}

/// <summary>A contiguous range (e.g. <c>1-5</c>).</summary>
public sealed record CronRangeNode(int Start, int End) : CronNode
{
    internal override IEnumerable<int> Expand(int min, int max)
    {
        var lo = Math.Max(Start, min);
        var hi = Math.Min(End, max);
        for (var i = lo; i <= hi; i++) yield return i;
    }

    public override string ToCronString() =>
        $"{Start.ToString(CultureInfo.InvariantCulture)}-{End.ToString(CultureInfo.InvariantCulture)}";
}

/// <summary>
/// A step expression (e.g. <c>* /5</c>, <c>1-10/2</c>, <c>0/15</c>).
/// The <see cref="Base"/> is the range or wildcard; the <see cref="Step"/> is the increment.
/// </summary>
public sealed record CronStepNode(CronNode Base, int Step) : CronNode
{
    internal override IEnumerable<int> Expand(int min, int max)
    {
        var baseValues = Base.Expand(min, max).OrderBy(v => v).ToList();
        if (baseValues.Count == 0) yield break;

        var first = baseValues[0];
        for (var v = first; v <= max; v += Step)
        {
            if (v >= min) yield return v;
        }
    }

    internal override IEnumerable<int> Expand(int min, int max, int year, int month)
    {
        var baseValues = Base.Expand(min, max, year, month).OrderBy(v => v).ToList();
        if (baseValues.Count == 0) yield break;

        var first = baseValues[0];
        for (var v = first; v <= max; v += Step)
        {
            if (v >= min) yield return v;
        }
    }

    public override string ToCronString() => $"{Base.ToCronString()}/{Step.ToString(CultureInfo.InvariantCulture)}";
}

/// <summary>
/// Last day indicator (<c>L</c>) for day-of-month, optionally with an offset (<c>L-3</c>).
/// </summary>
public sealed record CronLastDayNode(int Offset = 0) : CronNode
{
    internal override IEnumerable<int> Expand(int min, int max) =>
        Array.Empty<int>(); // Requires date context

    internal override IEnumerable<int> Expand(int min, int max, int year, int month)
    {
        var last = DateTime.DaysInMonth(year, month) - Offset;
        if (last >= min && last <= max) yield return last;
    }

    public override string ToCronString() => Offset == 0 ? "L" : $"L-{Offset.ToString(CultureInfo.InvariantCulture)}";
}

/// <summary>
/// Last specific weekday of the month (<c>5L</c> = last Friday).
/// Used in the day-of-week field.
/// </summary>
public sealed record CronLastWeekdayOfMonthNode(int DayOfWeek) : CronNode
{
    internal override IEnumerable<int> Expand(int min, int max) =>
        Array.Empty<int>(); // Requires date context

    internal override IEnumerable<int> Expand(int min, int max, int year, int month)
    {
        var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        var target = (System.DayOfWeek)DayOfWeek;
        while (lastDay.DayOfWeek != target) lastDay = lastDay.AddDays(-1);
        yield return lastDay.Day;
    }

    public override string ToCronString() =>
        $"{DayOfWeek.ToString(CultureInfo.InvariantCulture)}L";
}

/// <summary>
/// Nearest weekday to a given day-of-month (<c>15W</c>).
/// </summary>
public sealed record CronNearestWeekdayNode(int Day) : CronNode
{
    internal override IEnumerable<int> Expand(int min, int max) =>
        Array.Empty<int>();

    internal override IEnumerable<int> Expand(int min, int max, int year, int month)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var day = Math.Min(Day, daysInMonth);
        var date = new DateTime(year, month, day);

        int resolved;
        if (date.DayOfWeek == System.DayOfWeek.Saturday)
            resolved = day == 1 ? day + 2 : day - 1;
        else if (date.DayOfWeek == System.DayOfWeek.Sunday)
            resolved = day >= daysInMonth ? day - 2 : day + 1;
        else
            resolved = day;

        resolved = Math.Clamp(resolved, 1, daysInMonth);
        if (resolved >= min && resolved <= max) yield return resolved;
    }

    public override string ToCronString() => $"{Day.ToString(CultureInfo.InvariantCulture)}W";
}

/// <summary>
/// Last weekday (<c>LW</c>) of the month – shorthand for the last business day.
/// </summary>
public sealed record CronLastWeekdayNode : CronNode
{
    internal override IEnumerable<int> Expand(int min, int max) =>
        Array.Empty<int>();

    internal override IEnumerable<int> Expand(int min, int max, int year, int month)
    {
        var last = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        while (last.DayOfWeek == System.DayOfWeek.Saturday || last.DayOfWeek == System.DayOfWeek.Sunday)
            last = last.AddDays(-1);
        if (last.Day >= min && last.Day <= max) yield return last.Day;
    }

    public override string ToCronString() => "LW";
}

/// <summary>
/// Nth day-of-week in the month (<c>5#3</c> = third Friday).
/// </summary>
public sealed record CronHashNode(int DayOfWeek, int Nth) : CronNode
{
    internal override IEnumerable<int> Expand(int min, int max) =>
        Array.Empty<int>();

    internal override IEnumerable<int> Expand(int min, int max, int year, int month)
    {
        var target = (System.DayOfWeek)DayOfWeek;
        var count = 0;
        var daysInMonth = DateTime.DaysInMonth(year, month);
        for (var d = 1; d <= daysInMonth; d++)
        {
            if (new DateTime(year, month, d).DayOfWeek != target) continue;
            count++;
            if (count != Nth) continue;
            if (d >= min && d <= max) yield return d;
            yield break;
        }
    }

    public override string ToCronString() =>
        $"{DayOfWeek.ToString(CultureInfo.InvariantCulture)}#{Nth.ToString(CultureInfo.InvariantCulture)}";
}

// CronField – a parsed, validated single field of a cron expression.

/// <summary>
/// Represents a single parsed field (e.g. minutes, hours) of a cron expression.
/// A field contains one or more <see cref="CronNode"/> elements (the comma-separated parts).
/// Instances are immutable and thread-safe.
/// </summary>
public sealed class CronField
{
    /// <summary>Gets which part of the cron expression this field represents.</summary>
    public CronFieldType FieldType { get; }

    /// <summary>Gets the AST nodes that compose this field.</summary>
    public IReadOnlyList<CronNode> Nodes { get; }

    /// <summary>Gets the raw text that was parsed to produce this field.</summary>
    public string RawValue { get; }

    /// <summary>Returns <c>true</c> if this field is a bare wildcard (<c>*</c>).</summary>
    public bool IsWildcard => Nodes.Count == 1 && Nodes[0] is CronWildcardNode;

    /// <summary>Returns <c>true</c> if this field is a question mark (<c>?</c>).</summary>
    public bool IsQuestionMark => Nodes.Count == 1 && Nodes[0] is CronQuestionMarkNode;

    /// <summary>Returns <c>true</c> when the field contains context-dependent nodes (L, W, #).</summary>
    public bool IsContextDependent => Nodes.Any(static n =>
        n is CronLastDayNode or CronLastWeekdayOfMonthNode or CronNearestWeekdayNode
            or CronLastWeekdayNode or CronHashNode);

    // Cached expanded values for context-free fields.
    private readonly Lazy<HashSet<int>> _expandedValues;

    internal CronField(CronFieldType fieldType, IReadOnlyList<CronNode> nodes, string rawValue)
    {
        FieldType = fieldType;
        Nodes = nodes;
        RawValue = rawValue;
        _expandedValues = new Lazy<HashSet<int>>(() =>
        {
            var (min, max) = GetFieldRange(fieldType);
            return new HashSet<int>(nodes.SelectMany(n => n.Expand(min, max)));
        });
    }

    /// <summary>
    /// Returns the valid inclusive [min, max] range for a given field type.
    /// </summary>
    public static (int Min, int Max) GetFieldRange(CronFieldType type) => type switch
    {
        CronFieldType.Second => (0, 59),
        CronFieldType.Minute => (0, 59),
        CronFieldType.Hour => (0, 23),
        CronFieldType.DayOfMonth => (1, 31),
        CronFieldType.Month => (1, 12),
        CronFieldType.DayOfWeek => (0, 6),
        CronFieldType.Year => (1970, 2099),
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };

    /// <summary>
    /// Determines whether the given <paramref name="value"/> is matched by this field.
    /// For context-dependent fields (L, W, #), use <see cref="Matches(int, int, int)"/>.
    /// </summary>
    public bool Matches(int value) => _expandedValues.Value.Contains(value);

    /// <summary>
    /// Determines whether the given <paramref name="value"/> is matched by this field,
    /// using <paramref name="year"/> and <paramref name="month"/> for context-dependent resolution.
    /// </summary>
    public bool Matches(int value, int year, int month)
    {
        if (!IsContextDependent) return _expandedValues.Value.Contains(value);

        var (min, max) = GetFieldRange(FieldType);
        return Nodes.Any(n => n.Expand(min, max, year, month).Contains(value));
    }

    /// <summary>
    /// Returns the set of all integer values this field can match.
    /// </summary>
    public IReadOnlySet<int> GetMatchingValues() => _expandedValues.Value;

    /// <summary>
    /// Returns the set of all integer values this field can match for a specific year/month context.
    /// </summary>
    public IReadOnlySet<int> GetMatchingValues(int year, int month)
    {
        if (!IsContextDependent) return _expandedValues.Value;
        var (min, max) = GetFieldRange(FieldType);
        return new HashSet<int>(Nodes.SelectMany(n => n.Expand(min, max, year, month)));
    }

    /// <summary>
    /// Returns the smallest value ≥ <paramref name="startFrom"/> that this field matches,
    /// or <c>null</c> if none exists within the field's valid range.
    /// </summary>
    public int? GetNextMatchingValue(int startFrom)
    {
        var (_, max) = GetFieldRange(FieldType);
        var values = _expandedValues.Value;
        for (var v = startFrom; v <= max; v++)
        {
            if (values.Contains(v)) return v;
        }
        return null;
    }

    /// <summary>
    /// Returns the smallest value ≥ <paramref name="startFrom"/> that this field matches
    /// with date context, or <c>null</c> if none exists.
    /// </summary>
    public int? GetNextMatchingValue(int startFrom, int year, int month)
    {
        if (!IsContextDependent) return GetNextMatchingValue(startFrom);

        var (_, max) = GetFieldRange(FieldType);
        var values = GetMatchingValues(year, month);
        for (var v = startFrom; v <= max; v++)
        {
            if (values.Contains(v)) return v;
        }
        return null;
    }

    /// <summary>
    /// Returns the smallest matching value within this field's valid range, or <c>null</c>.
    /// </summary>
    public int? GetFirstMatchingValue() => GetNextMatchingValue(GetFieldRange(FieldType).Min);

    /// <summary>
    /// Returns the canonical cron string for this field (e.g. <c>1-5</c>, <c>*/15</c>).
    /// </summary>
    public string ToCronString() =>
        string.Join(",", Nodes.Select(static n => n.ToCronString()));

    /// <inheritdoc />
    public override string ToString() => ToCronString();
    internal static readonly IReadOnlyDictionary<string, int> MonthNames =
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["JAN"] = 1, ["FEB"] = 2, ["MAR"] = 3, ["APR"] = 4,
            ["MAY"] = 5, ["JUN"] = 6, ["JUL"] = 7, ["AUG"] = 8,
            ["SEP"] = 9, ["OCT"] = 10, ["NOV"] = 11, ["DEC"] = 12
        };

    internal static readonly IReadOnlyDictionary<string, int> DayOfWeekNames =
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["SUN"] = 0, ["MON"] = 1, ["TUE"] = 2, ["WED"] = 3,
            ["THU"] = 4, ["FRI"] = 5, ["SAT"] = 6
        };

    internal static readonly string[] MonthFullNames =
    {
        "", "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    };

    internal static readonly string[] DayOfWeekFullNames =
    {
        "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
    };
}
