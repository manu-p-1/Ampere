#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Ampere.Str.Cron;

/// <summary>
/// An immutable, thread-safe representation of a fully parsed cron expression.
/// <para>
/// Supports standard 5-field (minute–DOW), 6-field with seconds, and 7-field
/// with seconds + year formats.  Also supports Quartz-style extensions: <c>L</c>,
/// <c>W</c>, <c>LW</c>, <c>#</c>, and <c>?</c>.
/// </para>
/// <para>
/// Use <see cref="CronParser.Parse"/> to obtain instances, or <see cref="CronBuilder"/>
/// to construct them fluently.
/// </para>
/// </summary>
public sealed class CronExpression : IEquatable<CronExpression>
{
    private const int MaxIterationYears = 4;

    /// <summary>Gets the seconds field, or <c>null</c> for standard 5-field expressions.</summary>
    public CronField? Seconds { get; }

    /// <summary>Gets the minutes field.</summary>
    public CronField Minutes { get; }

    /// <summary>Gets the hours field.</summary>
    public CronField Hours { get; }

    /// <summary>Gets the day-of-month field.</summary>
    public CronField DayOfMonth { get; }

    /// <summary>Gets the month field.</summary>
    public CronField Month { get; }

    /// <summary>Gets the day-of-week field.</summary>
    public CronField DayOfWeek { get; }

    /// <summary>Gets the year field, or <c>null</c> when not present.</summary>
    public CronField? Year { get; }

    /// <summary>Gets the format that was detected during parsing.</summary>
    public CronFormat Format { get; }

    /// <summary>Gets the original raw expression string.</summary>
    public string RawExpression { get; }

    internal CronExpression(
        CronField? seconds,
        CronField minutes,
        CronField hours,
        CronField dayOfMonth,
        CronField month,
        CronField dayOfWeek,
        CronField? year,
        CronFormat format,
        string rawExpression)
    {
        Seconds = seconds;
        Minutes = minutes;
        Hours = hours;
        DayOfMonth = dayOfMonth;
        Month = month;
        DayOfWeek = dayOfWeek;
        Year = year;
        Format = format;
        RawExpression = rawExpression;
    }

    //  Serialisation

    /// <summary>
    /// Returns the canonical cron-notation string (e.g. <c>0 9 * * 1-5</c>).
    /// </summary>
    public string ToCronString()
    {
        var sb = new StringBuilder();
        if (Seconds is not null) sb.Append(Seconds.ToCronString()).Append(' ');
        sb.Append(Minutes.ToCronString()).Append(' ');
        sb.Append(Hours.ToCronString()).Append(' ');
        sb.Append(DayOfMonth.ToCronString()).Append(' ');
        sb.Append(Month.ToCronString()).Append(' ');
        sb.Append(DayOfWeek.ToCronString());
        if (Year is not null) sb.Append(' ').Append(Year.ToCronString());
        return sb.ToString();
    }

    /// <summary>
    /// Returns a human-readable English description of this cron expression.
    /// </summary>
    public override string ToString() => Describe();

    //  Human-readable description

    /// <summary>
    /// Produces a human-readable English description of this cron expression.
    /// Examples: "Every minute", "At 9:00 AM, Monday through Friday",
    /// "Every 15 minutes, 9:00 AM – 5:59 PM, Monday through Friday".
    /// </summary>
    public string Describe()
    {
        var parts = new List<string>();

        var timePart = DescribeTime();
        if (!string.IsNullOrEmpty(timePart)) parts.Add(timePart);

        var domPart = DescribeDayOfMonth();
        if (!string.IsNullOrEmpty(domPart)) parts.Add(domPart);

        var monthPart = DescribeMonth();
        if (!string.IsNullOrEmpty(monthPart)) parts.Add(monthPart);

        var dowPart = DescribeDayOfWeek();
        if (!string.IsNullOrEmpty(dowPart)) parts.Add(dowPart);

        var yearPart = DescribeYear();
        if (!string.IsNullOrEmpty(yearPart)) parts.Add(yearPart);

        return parts.Count == 0 ? "Every minute" : string.Join(", ", parts);
    }

    private string DescribeTime()
    {
        var hasSeconds = Seconds is not null && !Seconds.IsWildcard;
        var minWild = Minutes.IsWildcard;
        var hourWild = Hours.IsWildcard;

        string? secPart = null;
        if (Seconds is not null && !Seconds.IsWildcard)
        {
            secPart = DescribeFieldValues(Seconds, "second");
        }

        if (minWild && hourWild)
        {
            if (Seconds is not null && Seconds.IsWildcard)
                return "Every second";
            return secPart is not null ? secPart : "Every minute";
        }

        // Step on minutes
        if (Minutes.Nodes.Count == 1 && Minutes.Nodes[0] is CronStepNode minStep)
        {
            var minPart = $"Every {minStep.Step} minutes";
            var hourRange = DescribeHourRange();
            if (hourRange is not null) minPart += $", {hourRange}";
            if (secPart is not null) minPart = $"{secPart}, {minPart}";
            return minPart;
        }

        // Specific minute & hour → "At HH:MM"
        if (Minutes.Nodes.Count >= 1 && !minWild && Hours.Nodes.Count >= 1 && !hourWild)
        {
            var times = BuildTimeStrings();
            var result = $"At {JoinEnglish(times)}";
            if (secPart is not null) result = $"{secPart}, {result}";
            return result;
        }

        // Specific minute, any hour
        if (!minWild && hourWild)
        {
            var desc = $"Every hour, at {DescribeFieldValues(Minutes, "minute")}";
            if (secPart is not null) desc = $"{secPart}, {desc}";
            return desc;
        }

        // Every minute within hour range
        if (minWild && !hourWild)
        {
            var hr = DescribeHourRange();
            var desc = hr is not null ? $"Every minute, {hr}" : "Every minute";
            if (secPart is not null) desc = $"{secPart}, {desc}";
            return desc;
        }

        var fallback = $"At {DescribeFieldValues(Minutes, "minute")}, {DescribeFieldValues(Hours, "hour")}";
        if (secPart is not null) fallback = $"{secPart}, {fallback}";
        return fallback;
    }

    private List<string> BuildTimeStrings()
    {
        var minutes = Minutes.GetMatchingValues().OrderBy(v => v).ToList();
        var hours = Hours.GetMatchingValues().OrderBy(v => v).ToList();
        var times = new List<string>();
        foreach (var h in hours)
        foreach (var m in minutes)
            times.Add(FormatTime(h, m));
        return times;
    }

    private string? DescribeHourRange()
    {
        if (Hours.IsWildcard) return null;
        if (Hours.Nodes.Count == 1 && Hours.Nodes[0] is CronRangeNode range)
            return $"{FormatTime(range.Start, 0)} – {FormatTime(range.End, 59)}";
        if (Hours.Nodes.Count == 1 && Hours.Nodes[0] is CronStepNode step)
            return $"every {step.Step} hours";
        return $"during {DescribeFieldValues(Hours, "hour")}";
    }

    private string DescribeDayOfMonth()
    {
        if (DayOfMonth.IsWildcard || DayOfMonth.IsQuestionMark) return string.Empty;
        var parts = new List<string>();
        foreach (var node in DayOfMonth.Nodes)
        {
            switch (node)
            {
                case CronLastDayNode last:
                    parts.Add(last.Offset == 0
                        ? "the last day"
                        : $"{last.Offset} day(s) before the last day");
                    break;
                case CronNearestWeekdayNode nw:
                    parts.Add($"the nearest weekday to the {Ordinal(nw.Day)}");
                    break;
                case CronLastWeekdayNode:
                    parts.Add("the last weekday");
                    break;
                case CronValueNode v:
                    parts.Add($"the {Ordinal(v.Value)}");
                    break;
                case CronRangeNode r:
                    parts.Add($"the {Ordinal(r.Start)} through the {Ordinal(r.End)}");
                    break;
                default:
                    parts.Add($"day {node.ToCronString()}");
                    break;
            }
        }
        return $"on {JoinEnglish(parts)} of the month";
    }

    private string DescribeMonth()
    {
        if (Month.IsWildcard || Month.IsQuestionMark) return string.Empty;
        var values = Month.GetMatchingValues().OrderBy(v => v)
            .Select(v => CronField.MonthFullNames[v]).ToList();
        return $"in {JoinEnglish(values)}";
    }

    private string DescribeDayOfWeek()
    {
        if (DayOfWeek.IsWildcard || DayOfWeek.IsQuestionMark) return string.Empty;
        var parts = new List<string>();
        foreach (var node in DayOfWeek.Nodes)
        {
            switch (node)
            {
                case CronHashNode hash:
                    parts.Add($"the {Ordinal(hash.Nth)} {CronField.DayOfWeekFullNames[hash.DayOfWeek]} of the month");
                    break;
                case CronLastWeekdayOfMonthNode lw:
                    parts.Add($"the last {CronField.DayOfWeekFullNames[lw.DayOfWeek]} of the month");
                    break;
                case CronRangeNode r:
                    parts.Add($"{CronField.DayOfWeekFullNames[r.Start]} through {CronField.DayOfWeekFullNames[r.End]}");
                    break;
                case CronValueNode v:
                    parts.Add(CronField.DayOfWeekFullNames[v.Value]);
                    break;
                default:
                    parts.Add(node.ToCronString());
                    break;
            }
        }
        return $"on {JoinEnglish(parts)}";
    }

    private string DescribeYear()
    {
        if (Year is null || Year.IsWildcard) return string.Empty;
        return $"in year(s) {DescribeFieldValues(Year, "year")}";
    }

    private static string DescribeFieldValues(CronField field, string unit)
    {
        var parts = new List<string>();
        foreach (var node in field.Nodes)
        {
            switch (node)
            {
                case CronStepNode step:
                    parts.Add($"every {step.Step} {unit}(s)");
                    break;
                case CronRangeNode range:
                    parts.Add($"{range.Start}–{range.End}");
                    break;
                case CronValueNode v:
                    parts.Add(v.Value.ToString(CultureInfo.InvariantCulture));
                    break;
                default:
                    parts.Add(node.ToCronString());
                    break;
            }
        }
        return JoinEnglish(parts);
    }

    //  Matching

    /// <summary>
    /// Returns <c>true</c> if <paramref name="dateTime"/> satisfies every field
    /// of this cron expression.
    /// </summary>
    public bool IsMatch(DateTime dateTime)
    {
        if (Year is not null && !Year.Matches(dateTime.Year)) return false;
        if (!Month.Matches(dateTime.Month, dateTime.Year, dateTime.Month)) return false;
        if (!MatchesDay(dateTime)) return false;
        if (!Hours.Matches(dateTime.Hour)) return false;
        if (!Minutes.Matches(dateTime.Minute)) return false;
        if (Seconds is not null && !Seconds.Matches(dateTime.Second)) return false;
        return true;
    }

    private bool MatchesDay(DateTime dt)
    {
        var domWild = DayOfMonth.IsWildcard || DayOfMonth.IsQuestionMark;
        var dowWild = DayOfWeek.IsWildcard || DayOfWeek.IsQuestionMark;

        if (domWild && dowWild) return true;

        var domMatch = domWild || DayOfMonth.Matches(dt.Day, dt.Year, dt.Month);
        var dowMatch = dowWild || MatchesDayOfWeek(dt);

        // Standard UNIX semantics: if both are specified, it's a union (OR).
        if (!domWild && !dowWild) return domMatch || dowMatch;

        return domWild ? dowMatch : domMatch;
    }

    private bool MatchesDayOfWeek(DateTime dt)
    {
        var dow = (int)dt.DayOfWeek; // Sunday = 0

        // Simple value / range / wildcard matching
        if (!DayOfWeek.IsContextDependent) return DayOfWeek.Matches(dow);

        // Context-dependent: # and L nodes resolve to day-of-month values
        foreach (var node in DayOfWeek.Nodes)
        {
            switch (node)
            {
                case CronHashNode or CronLastWeekdayOfMonthNode:
                {
                    var (min, max) = CronField.GetFieldRange(CronFieldType.DayOfMonth);
                    if (node.Expand(min, max, dt.Year, dt.Month).Contains(dt.Day))
                        return true;
                    break;
                }
                default:
                {
                    var (min, max) = CronField.GetFieldRange(CronFieldType.DayOfWeek);
                    if (node.Expand(min, max).Contains(dow))
                        return true;
                    break;
                }
            }
        }
        return false;
    }

    //  Occurrence calculation

    /// <summary>
    /// Returns the next <see cref="DateTime"/> that matches this cron expression
    /// after <paramref name="from"/>, or <c>null</c> if none exists within ~4 years.
    /// </summary>
    /// <param name="from">The reference point (exclusive by default).</param>
    /// <param name="inclusive">When <c>true</c>, <paramref name="from"/> itself may be returned.</param>
    /// <param name="timeZone">Optional time zone; when supplied, the search
    /// and result are performed in that zone and returned as local time.</param>
    public DateTime? GetNextOccurrence(DateTime from, bool inclusive = false, TimeZoneInfo? timeZone = null)
    {
        var dt = inclusive ? from : IncrementSmallestUnit(from);
        if (timeZone is not null)
            dt = TimeZoneInfo.ConvertTime(dt, timeZone);

        // Snap to the boundary of the smallest unit
        dt = Seconds is not null
            ? new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second)
            : new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);

        var maxDate = dt.AddYears(MaxIterationYears);

        while (dt < maxDate)
        {
            if (Year is not null && !Year.Matches(dt.Year))
            {
                var nextYear = Year.GetNextMatchingValue(dt.Year + 1);
                if (nextYear is null) return null;
                dt = new DateTime(nextYear.Value, 1, 1, 0, 0, 0);
                continue;
            }

            if (!Month.Matches(dt.Month, dt.Year, dt.Month))
            {
                var nextMonth = Month.GetNextMatchingValue(dt.Month + 1);
                if (nextMonth is not null)
                {
                    dt = new DateTime(dt.Year, nextMonth.Value, 1, 0, 0, 0);
                }
                else
                {
                    dt = new DateTime(dt.Year + 1, 1, 1, 0, 0, 0);
                }
                continue;
            }

            if (!MatchesDay(dt))
            {
                dt = dt.Date.AddDays(1);
                continue;
            }

            if (!Hours.Matches(dt.Hour))
            {
                var nextHour = Hours.GetNextMatchingValue(dt.Hour + 1);
                if (nextHour is not null)
                {
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, nextHour.Value, 0, 0);
                }
                else
                {
                    dt = dt.Date.AddDays(1);
                }
                continue;
            }

            if (!Minutes.Matches(dt.Minute))
            {
                var nextMin = Minutes.GetNextMatchingValue(dt.Minute + 1);
                if (nextMin is not null)
                {
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, nextMin.Value, 0);
                }
                else
                {
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0).AddHours(1);
                }
                continue;
            }

            if (Seconds is not null && !Seconds.Matches(dt.Second))
            {
                var nextSec = Seconds.GetNextMatchingValue(dt.Second + 1);
                if (nextSec is not null)
                {
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, nextSec.Value);
                }
                else
                {
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0).AddMinutes(1);
                }
                continue;
            }

            if (timeZone is not null)
                dt = TimeZoneInfo.ConvertTime(dt, timeZone, TimeZoneInfo.Local);
            return dt;
        }

        return null;
    }

    /// <summary>
    /// Returns the previous <see cref="DateTime"/> that matches this cron expression
    /// before <paramref name="from"/>, or <c>null</c> if none exists within ~4 years.
    /// </summary>
    public DateTime? GetPreviousOccurrence(DateTime from, bool inclusive = false)
    {
        var dt = inclusive ? from : DecrementSmallestUnit(from);
        dt = Seconds is not null
            ? new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second)
            : new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);

        var minDate = dt.AddYears(-MaxIterationYears);

        while (dt > minDate)
        {
            if (Year is not null && !Year.Matches(dt.Year))
            {
                var prev = GetPreviousMatchingValue(Year, dt.Year - 1);
                if (prev is null) return null;
                dt = new DateTime(prev.Value, 12, 31, 23, 59, Seconds is not null ? 59 : 0);
                continue;
            }

            if (!Month.Matches(dt.Month, dt.Year, dt.Month))
            {
                var prev = GetPreviousMatchingValue(Month, dt.Month - 1);
                if (prev is not null)
                {
                    var lastDay = DateTime.DaysInMonth(dt.Year, prev.Value);
                    dt = new DateTime(dt.Year, prev.Value, lastDay, 23, 59, Seconds is not null ? 59 : 0);
                }
                else
                {
                    dt = new DateTime(dt.Year - 1, 12, 31, 23, 59, Seconds is not null ? 59 : 0);
                }
                continue;
            }

            if (!MatchesDay(dt))
            {
                dt = dt.Date.AddDays(-1).AddHours(23).AddMinutes(59);
                if (Seconds is not null) dt = dt.AddSeconds(59);
                continue;
            }

            if (!Hours.Matches(dt.Hour))
            {
                var prev = GetPreviousMatchingValue(Hours, dt.Hour - 1);
                if (prev is not null)
                {
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, prev.Value, 59, Seconds is not null ? 59 : 0);
                }
                else
                {
                    dt = dt.Date.AddDays(-1).AddHours(23).AddMinutes(59);
                    if (Seconds is not null) dt = dt.AddSeconds(59);
                }
                continue;
            }

            if (!Minutes.Matches(dt.Minute))
            {
                var prev = GetPreviousMatchingValue(Minutes, dt.Minute - 1);
                if (prev is not null)
                {
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, prev.Value, Seconds is not null ? 59 : 0);
                }
                else
                {
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0).AddMinutes(-1);
                    if (Seconds is not null) dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 59);
                }
                continue;
            }

            if (Seconds is not null && !Seconds.Matches(dt.Second))
            {
                var prev = GetPreviousMatchingValue(Seconds, dt.Second - 1);
                if (prev is not null)
                {
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, prev.Value);
                }
                else
                {
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0).AddSeconds(-1);
                }
                continue;
            }

            return dt;
        }

        return null;
    }

    /// <summary>
    /// Enumerates all occurrences between <paramref name="from"/> (exclusive)
    /// and <paramref name="to"/> (inclusive).
    /// </summary>
    public IEnumerable<DateTime> GetOccurrences(DateTime from, DateTime to)
    {
        var current = from;
        while (true)
        {
            var next = GetNextOccurrence(current);
            if (next is null || next.Value > to) yield break;
            yield return next.Value;
            current = next.Value;
        }
    }

    /// <summary>
    /// Returns the next <paramref name="count"/> occurrences after <paramref name="from"/>.
    /// </summary>
    public IReadOnlyList<DateTime> GetNextOccurrences(DateTime from, int count)
    {
        var list = new List<DateTime>(count);
        var current = from;
        for (var i = 0; i < count; i++)
        {
            var next = GetNextOccurrence(current);
            if (next is null) break;
            list.Add(next.Value);
            current = next.Value;
        }
        return list;
    }

    private static int? GetPreviousMatchingValue(CronField field, int startFrom)
    {
        var (min, _) = CronField.GetFieldRange(field.FieldType);
        var values = field.GetMatchingValues();
        for (var v = startFrom; v >= min; v--)
        {
            if (values.Contains(v)) return v;
        }
        return null;
    }

    private DateTime IncrementSmallestUnit(DateTime dt) =>
        Seconds is not null ? dt.AddSeconds(1) : dt.AddMinutes(1);

    private DateTime DecrementSmallestUnit(DateTime dt) =>
        Seconds is not null ? dt.AddSeconds(-1) : dt.AddMinutes(-1);

    //  Equality

    /// <inheritdoc />
    public bool Equals(CronExpression? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ToCronString() == other.ToCronString();
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as CronExpression);

    /// <inheritdoc />
    public override int GetHashCode() => ToCronString().GetHashCode(StringComparison.Ordinal);

    /// <summary>Equality operator.</summary>
    public static bool operator ==(CronExpression? left, CronExpression? right) => Equals(left, right);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(CronExpression? left, CronExpression? right) => !Equals(left, right);

    //  Helpers

    private static string FormatTime(int hour, int minute)
    {
        var period = hour < 12 ? "AM" : "PM";
        var h = hour % 12;
        if (h == 0) h = 12;
        return $"{h}:{minute:D2} {period}";
    }

    private static string Ordinal(int n) => (n % 100) switch
    {
        11 or 12 or 13 => $"{n}th",
        _ when n % 10 == 1 => $"{n}st",
        _ when n % 10 == 2 => $"{n}nd",
        _ when n % 10 == 3 => $"{n}rd",
        _ => $"{n}th"
    };

    private static string JoinEnglish(IList<string> items) => items.Count switch
    {
        0 => string.Empty,
        1 => items[0],
        2 => $"{items[0]} and {items[1]}",
        _ => string.Join(", ", items.Take(items.Count - 1)) + $", and {items[^1]}"
    };
}
