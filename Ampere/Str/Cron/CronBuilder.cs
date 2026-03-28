#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Ampere.Str.Cron;

/// <summary>
/// A fluent, type-safe builder for constructing <see cref="CronExpression"/> instances.
/// <para>
/// Provides both raw field methods (<see cref="WithMinutes(string)"/>) and
/// high-level convenience methods (<see cref="EveryNMinutes(int)"/>,
/// <see cref="AtTime(int,int)"/>, <see cref="OnDaysOfWeek(DayOfWeek[])"/>).
/// </para>
/// <example>
/// <code>
/// var expr = new CronBuilder()
///     .AtTime(9, 0)
///     .OnDaysOfWeek(DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday)
///     .Build();
/// // produces "0 9 * * 1,3,5"
/// </code>
/// </example>
/// </summary>
public sealed class CronBuilder
{
    private string _seconds  = string.Empty;
    private string _minutes  = "*";
    private string _hours    = "*";
    private string _dom      = "*";
    private string _month    = "*";
    private string _dow      = "*";
    private string _year     = string.Empty;

    /// <summary>Sets the seconds field (enables 6-field format). Any valid cron syntax.</summary>
    public CronBuilder WithSeconds(string expression)  { _seconds = expression; return this; }

    /// <summary>Sets the minutes field. Any valid cron syntax (e.g. <c>*/15</c>, <c>0,30</c>).</summary>
    public CronBuilder WithMinutes(string expression)  { _minutes = expression; return this; }

    /// <summary>Sets the hours field.</summary>
    public CronBuilder WithHours(string expression)    { _hours = expression; return this; }

    /// <summary>Sets the day-of-month field.</summary>
    public CronBuilder WithDayOfMonth(string expression) { _dom = expression; return this; }

    /// <summary>Sets the month field.</summary>
    public CronBuilder WithMonth(string expression)    { _month = expression; return this; }

    /// <summary>Sets the day-of-week field.</summary>
    public CronBuilder WithDayOfWeek(string expression) { _dow = expression; return this; }

    /// <summary>Sets the year field (enables 7-field format).</summary>
    public CronBuilder WithYear(string expression)     { _year = expression; return this; }

    /// <summary>Sets the expression to fire every minute (<c>* * * * *</c> baseline).</summary>
    public CronBuilder EveryMinute() { _minutes = "*"; _hours = "*"; return this; }

    /// <summary>Sets the expression to fire every <paramref name="n"/> minutes.</summary>
    public CronBuilder EveryNMinutes(int n)
    {
        if (n < 1 || n > 59) throw new ArgumentOutOfRangeException(nameof(n), "Must be 1–59.");
        _minutes = $"*/{n}";
        return this;
    }

    /// <summary>Sets the expression to fire every hour at minute 0.</summary>
    public CronBuilder EveryHour() { _minutes = "0"; _hours = "*"; return this; }

    /// <summary>Sets the expression to fire every <paramref name="n"/> hours at minute 0.</summary>
    public CronBuilder EveryNHours(int n)
    {
        if (n < 1 || n > 23) throw new ArgumentOutOfRangeException(nameof(n), "Must be 1–23.");
        _minutes = "0";
        _hours = $"*/{n}";
        return this;
    }

    /// <summary>Sets the expression to fire every <paramref name="n"/> seconds (enables seconds field).</summary>
    public CronBuilder EveryNSeconds(int n)
    {
        if (n < 1 || n > 59) throw new ArgumentOutOfRangeException(nameof(n), "Must be 1–59.");
        _seconds = $"*/{n}";
        return this;
    }

    /// <summary>Sets the time to a specific hour and minute (e.g. 9:30).</summary>
    public CronBuilder AtTime(int hour, int minute)
    {
        if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException(nameof(hour));
        if (minute < 0 || minute > 59) throw new ArgumentOutOfRangeException(nameof(minute));
        _hours = hour.ToString(CultureInfo.InvariantCulture);
        _minutes = minute.ToString(CultureInfo.InvariantCulture);
        return this;
    }

    /// <summary>Sets the time to multiple specific times during the day.</summary>
    public CronBuilder AtTimes(params (int Hour, int Minute)[] times)
    {
        if (times.Length == 0) throw new ArgumentException("At least one time required.", nameof(times));

        var hours = new SortedSet<int>();
        var minutes = new SortedSet<int>();
        foreach (var (h, m) in times)
        {
            if (h < 0 || h > 23) throw new ArgumentOutOfRangeException(nameof(times), $"Hour {h} out of range.");
            if (m < 0 || m > 59) throw new ArgumentOutOfRangeException(nameof(times), $"Minute {m} out of range.");
            hours.Add(h);
            minutes.Add(m);
        }

        // If all times share the same minute, we can express it cleanly
        if (minutes.Count == 1)
        {
            _minutes = minutes.First().ToString(CultureInfo.InvariantCulture);
            _hours = string.Join(",", hours.Select(h => h.ToString(CultureInfo.InvariantCulture)));
        }
        else if (hours.Count == 1)
        {
            _hours = hours.First().ToString(CultureInfo.InvariantCulture);
            _minutes = string.Join(",", minutes.Select(m => m.ToString(CultureInfo.InvariantCulture)));
        }
        else
        {
            // Fallback – only works correctly when all (hour, minute) combos are intended
            _hours = string.Join(",", hours.Select(h => h.ToString(CultureInfo.InvariantCulture)));
            _minutes = string.Join(",", minutes.Select(m => m.ToString(CultureInfo.InvariantCulture)));
        }

        return this;
    }

    /// <summary>Sets hours to a range (e.g. 9–17 for business hours).</summary>
    public CronBuilder DuringHours(int startHour, int endHour)
    {
        if (startHour < 0 || startHour > 23) throw new ArgumentOutOfRangeException(nameof(startHour));
        if (endHour < 0 || endHour > 23) throw new ArgumentOutOfRangeException(nameof(endHour));
        _hours = $"{startHour}-{endHour}";
        return this;
    }

    //  Convenience: day-of-week

    /// <summary>Restricts to the given days of the week.</summary>
    public CronBuilder OnDaysOfWeek(params DayOfWeek[] days)
    {
        if (days.Length == 0) throw new ArgumentException("At least one day required.", nameof(days));
        var values = days.Select(d => (int)d).OrderBy(v => v).Distinct();
        _dow = string.Join(",", values.Select(v => v.ToString(CultureInfo.InvariantCulture)));
        return this;
    }

    /// <summary>Shorthand for Monday–Friday.</summary>
    public CronBuilder Weekdays() { _dow = "1-5"; return this; }

    /// <summary>Shorthand for Saturday and Sunday.</summary>
    public CronBuilder Weekends() { _dow = "0,6"; return this; }

    /// <summary>Fires on the Nth occurrence of a weekday in the month (e.g. 2nd Tuesday).</summary>
    public CronBuilder OnNthDayOfWeek(DayOfWeek day, int nth)
    {
        if (nth < 1 || nth > 5) throw new ArgumentOutOfRangeException(nameof(nth), "Must be 1–5.");
        _dow = $"{(int)day}#{nth}";
        return this;
    }

    /// <summary>Fires on the last occurrence of a weekday in the month.</summary>
    public CronBuilder OnLastDayOfWeek(DayOfWeek day) { _dow = $"{(int)day}L"; return this; }

    /// <summary>Restricts to specific days of the month.</summary>
    public CronBuilder OnDaysOfMonth(params int[] days)
    {
        if (days.Length == 0) throw new ArgumentException("At least one day required.", nameof(days));
        foreach (var d in days)
            if (d < 1 || d > 31) throw new ArgumentOutOfRangeException(nameof(days), $"Day {d} out of range 1–31.");
        _dom = string.Join(",", days.OrderBy(d => d).Distinct().Select(d => d.ToString(CultureInfo.InvariantCulture)));
        return this;
    }

    /// <summary>Fires on the last day of every month (<c>L</c>).</summary>
    public CronBuilder OnLastDayOfMonth() { _dom = "L"; return this; }

    /// <summary>Fires on the last weekday (Mon–Fri) of every month (<c>LW</c>).</summary>
    public CronBuilder OnLastWeekdayOfMonth() { _dom = "LW"; return this; }

    /// <summary>Fires on the nearest weekday to the given day of the month.</summary>
    public CronBuilder OnNearestWeekday(int day)
    {
        if (day < 1 || day > 31) throw new ArgumentOutOfRangeException(nameof(day));
        _dom = $"{day}W";
        return this;
    }

    /// <summary>Restricts to specific months (1–12).</summary>
    public CronBuilder InMonths(params int[] months)
    {
        if (months.Length == 0) throw new ArgumentException("At least one month required.", nameof(months));
        foreach (var m in months)
            if (m < 1 || m > 12) throw new ArgumentOutOfRangeException(nameof(months), $"Month {m} out of range 1–12.");
        _month = string.Join(",", months.OrderBy(m => m).Distinct().Select(m => m.ToString(CultureInfo.InvariantCulture)));
        return this;
    }

    /// <summary>Restricts to specific years (enables 7-field format).</summary>
    public CronBuilder InYears(params int[] years)
    {
        if (years.Length == 0) throw new ArgumentException("At least one year required.", nameof(years));
        foreach (var y in years)
            if (y < 1970 || y > 2099) throw new ArgumentOutOfRangeException(nameof(years), $"Year {y} out of range 1970–2099.");
        _year = string.Join(",", years.OrderBy(y => y).Distinct().Select(y => y.ToString(CultureInfo.InvariantCulture)));
        _seconds = string.IsNullOrEmpty(_seconds) ? "0" : _seconds;
        return this;
    }

    /// <summary>Every day at midnight.</summary>
    public CronBuilder Daily() => AtTime(0, 0);

    /// <summary>Every Sunday at midnight.</summary>
    public CronBuilder Weekly() { AtTime(0, 0); _dow = "0"; return this; }

    /// <summary>First of every month at midnight.</summary>
    public CronBuilder Monthly() { AtTime(0, 0); _dom = "1"; return this; }

    /// <summary>January 1 at midnight each year.</summary>
    public CronBuilder Yearly() { AtTime(0, 0); _dom = "1"; _month = "1"; return this; }

    /// <summary>
    /// Builds and validates the <see cref="CronExpression"/>.
    /// Throws <see cref="CronParseException"/> if the assembled fields are invalid.
    /// </summary>
    public CronExpression Build()
    {
        var cronString = AssembleCronString();
        return CronParser.Parse(cronString);
    }

    /// <summary>
    /// Attempts to build the expression without throwing.
    /// </summary>
    /// <param name="result">The built expression, or <c>null</c>.</param>
    /// <param name="error">A description of the failure, or <c>null</c>.</param>
    /// <returns><c>true</c> on success.</returns>
    public bool TryBuild(out CronExpression? result, out string? error)
    {
        try
        {
            result = Build();
            error = null;
            return true;
        }
        catch (CronParseException ex)
        {
            result = null;
            error = ex.Message;
            return false;
        }
    }

    /// <summary>
    /// Returns the cron string that <see cref="Build"/> would parse,
    /// without performing validation.
    /// </summary>
    public string ToCronString() => AssembleCronString();

    /// <summary>
    /// Returns a human-readable description of what the built expression would produce.
    /// Returns the cron string if validation fails.
    /// </summary>
    public override string ToString()
    {
        if (TryBuild(out var expr, out _))
            return expr!.Describe();
        return AssembleCronString();
    }

    private string AssembleCronString()
    {
        var sb = new StringBuilder();
        var hasSeconds = !string.IsNullOrEmpty(_seconds);
        var hasYear = !string.IsNullOrEmpty(_year);

        if (hasSeconds) sb.Append(_seconds).Append(' ');
        sb.Append(_minutes).Append(' ')
          .Append(_hours).Append(' ')
          .Append(_dom).Append(' ')
          .Append(_month).Append(' ')
          .Append(_dow);
        if (hasYear) sb.Append(' ').Append(_year);

        return sb.ToString();
    }
}
