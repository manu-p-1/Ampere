using Ampere.Str.Cron;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmpereMSTest.Str.Cron
{
    [TestClass]
    public class CronParserTest
    {
        public TestContext TestContext { get; set; }

        //  Basic parsing – 5-field

        [TestMethod]
        public void Parse_EveryMinute()
        {
            var expr = CronParser.Parse("* * * * *");
            Assert.AreEqual(CronFormat.Standard, expr.Format);
            Assert.IsNull(expr.Seconds);
            Assert.IsNull(expr.Year);
            Assert.IsTrue(expr.Minutes.IsWildcard);
        }

        [TestMethod]
        public void Parse_SpecificTime()
        {
            var expr = CronParser.Parse("30 9 * * *");
            Assert.AreEqual(30, ((CronValueNode)expr.Minutes.Nodes[0]).Value);
            Assert.AreEqual(9, ((CronValueNode)expr.Hours.Nodes[0]).Value);
        }

        [TestMethod]
        public void Parse_StepMinutes()
        {
            var expr = CronParser.Parse("*/15 * * * *");
            Assert.IsInstanceOfType(expr.Minutes.Nodes[0], typeof(CronStepNode));
            var step = (CronStepNode)expr.Minutes.Nodes[0];
            Assert.AreEqual(15, step.Step);
            Assert.IsInstanceOfType(step.Base, typeof(CronWildcardNode));
        }

        [TestMethod]
        public void Parse_Range()
        {
            var expr = CronParser.Parse("0 9-17 * * *");
            var range = (CronRangeNode)expr.Hours.Nodes[0];
            Assert.AreEqual(9, range.Start);
            Assert.AreEqual(17, range.End);
        }

        [TestMethod]
        public void Parse_List()
        {
            var expr = CronParser.Parse("0 9,12,17 * * *");
            Assert.AreEqual(3, expr.Hours.Nodes.Count);
            Assert.AreEqual(9, ((CronValueNode)expr.Hours.Nodes[0]).Value);
            Assert.AreEqual(12, ((CronValueNode)expr.Hours.Nodes[1]).Value);
            Assert.AreEqual(17, ((CronValueNode)expr.Hours.Nodes[2]).Value);
        }

        [TestMethod]
        public void Parse_RangeWithStep()
        {
            var expr = CronParser.Parse("0 9-17/2 * * *");
            var step = (CronStepNode)expr.Hours.Nodes[0];
            Assert.AreEqual(2, step.Step);
            var range = (CronRangeNode)step.Base;
            Assert.AreEqual(9, range.Start);
            Assert.AreEqual(17, range.End);
        }

        [TestMethod]
        public void Parse_DayOfWeek_Range()
        {
            var expr = CronParser.Parse("0 9 * * 1-5");
            var range = (CronRangeNode)expr.DayOfWeek.Nodes[0];
            Assert.AreEqual(1, range.Start);
            Assert.AreEqual(5, range.End);
        }

        [TestMethod]
        public void Parse_NamedMonths()
        {
            var expr = CronParser.Parse("0 0 1 JAN,JUN *");
            Assert.AreEqual(2, expr.Month.Nodes.Count);
            Assert.AreEqual(1, ((CronValueNode)expr.Month.Nodes[0]).Value);
            Assert.AreEqual(6, ((CronValueNode)expr.Month.Nodes[1]).Value);
        }

        [TestMethod]
        public void Parse_NamedDays()
        {
            var expr = CronParser.Parse("0 9 * * MON-FRI");
            var range = (CronRangeNode)expr.DayOfWeek.Nodes[0];
            Assert.AreEqual(1, range.Start);
            Assert.AreEqual(5, range.End);
        }

        [TestMethod]
        public void Parse_Sunday7NormalisedTo0()
        {
            var expr = CronParser.Parse("0 0 * * 7");
            Assert.AreEqual(0, ((CronValueNode)expr.DayOfWeek.Nodes[0]).Value);
        }

        //  6-field (with seconds) and 7-field (with year)

        [TestMethod]
        public void Parse_6Field_WithSeconds()
        {
            var expr = CronParser.Parse("30 */5 * * * *");
            Assert.AreEqual(CronFormat.WithSeconds, expr.Format);
            Assert.IsNotNull(expr.Seconds);
            Assert.AreEqual(30, ((CronValueNode)expr.Seconds!.Nodes[0]).Value);
        }

        [TestMethod]
        public void Parse_7Field_WithYear()
        {
            var expr = CronParser.Parse("0 0 12 1 1 ? 2026");
            Assert.AreEqual(CronFormat.WithSecondsAndYear, expr.Format);
            Assert.IsNotNull(expr.Year);
            Assert.AreEqual(2026, ((CronValueNode)expr.Year!.Nodes[0]).Value);
        }

        //  Quartz extensions: L, W, #, ?

        [TestMethod]
        public void Parse_LastDayOfMonth()
        {
            var expr = CronParser.Parse("0 0 L * *");
            Assert.IsInstanceOfType(expr.DayOfMonth.Nodes[0], typeof(CronLastDayNode));
            Assert.AreEqual(0, ((CronLastDayNode)expr.DayOfMonth.Nodes[0]).Offset);
        }

        [TestMethod]
        public void Parse_LastDayWithOffset()
        {
            var expr = CronParser.Parse("0 0 L-3 * *");
            var last = (CronLastDayNode)expr.DayOfMonth.Nodes[0];
            Assert.AreEqual(3, last.Offset);
        }

        [TestMethod]
        public void Parse_NearestWeekday()
        {
            var expr = CronParser.Parse("0 0 15W * *");
            Assert.IsInstanceOfType(expr.DayOfMonth.Nodes[0], typeof(CronNearestWeekdayNode));
            Assert.AreEqual(15, ((CronNearestWeekdayNode)expr.DayOfMonth.Nodes[0]).Day);
        }

        [TestMethod]
        public void Parse_LastWeekday()
        {
            var expr = CronParser.Parse("0 0 LW * *");
            Assert.IsInstanceOfType(expr.DayOfMonth.Nodes[0], typeof(CronLastWeekdayNode));
        }

        [TestMethod]
        public void Parse_Hash_NthWeekday()
        {
            var expr = CronParser.Parse("0 0 * * 5#3");
            var hash = (CronHashNode)expr.DayOfWeek.Nodes[0];
            Assert.AreEqual(5, hash.DayOfWeek);
            Assert.AreEqual(3, hash.Nth);
        }

        [TestMethod]
        public void Parse_LastFridayOfMonth()
        {
            var expr = CronParser.Parse("0 0 * * 5L");
            Assert.IsInstanceOfType(expr.DayOfWeek.Nodes[0], typeof(CronLastWeekdayOfMonthNode));
            Assert.AreEqual(5, ((CronLastWeekdayOfMonthNode)expr.DayOfWeek.Nodes[0]).DayOfWeek);
        }

        [TestMethod]
        public void Parse_QuestionMark_DayOfMonth()
        {
            var expr = CronParser.Parse("0 0 ? * 1");
            Assert.IsTrue(expr.DayOfMonth.IsQuestionMark);
        }

        [TestMethod]
        public void Parse_QuestionMark_DayOfWeek()
        {
            var expr = CronParser.Parse("0 0 1 * ?");
            Assert.IsTrue(expr.DayOfWeek.IsQuestionMark);
        }

        //  Macros

        [TestMethod]
        [DataRow("@yearly",   "0 0 1 1 *")]
        [DataRow("@annually", "0 0 1 1 *")]
        [DataRow("@monthly",  "0 0 1 * *")]
        [DataRow("@weekly",   "0 0 * * 0")]
        [DataRow("@daily",    "0 0 * * *")]
        [DataRow("@midnight", "0 0 * * *")]
        [DataRow("@hourly",   "0 * * * *")]
        public void Parse_Macros(string macro, string expectedCron)
        {
            var expr = CronParser.Parse(macro);
            Assert.AreEqual(expectedCron, expr.ToCronString());
        }

        //  Error handling

        [TestMethod]
        [ExpectedException(typeof(CronParseException))]
        public void Parse_Empty_Throws()
        {
            CronParser.Parse("");
        }

        [TestMethod]
        [ExpectedException(typeof(CronParseException))]
        public void Parse_TooFewFields_Throws()
        {
            CronParser.Parse("* * *");
        }

        [TestMethod]
        [ExpectedException(typeof(CronParseException))]
        public void Parse_OutOfRange_Throws()
        {
            CronParser.Parse("60 * * * *");
        }

        [TestMethod]
        [ExpectedException(typeof(CronParseException))]
        public void Parse_InvalidRange_Throws()
        {
            CronParser.Parse("5-2 * * * *");
        }

        [TestMethod]
        [ExpectedException(typeof(CronParseException))]
        public void Parse_QuestionMarkInMinute_Throws()
        {
            CronParser.Parse("? * * * *");
        }

        [TestMethod]
        [ExpectedException(typeof(CronParseException))]
        public void Parse_UnknownMacro_Throws()
        {
            CronParser.Parse("@bogus");
        }

        [TestMethod]
        public void TryParse_Invalid_ReturnsFalse()
        {
            Assert.IsFalse(CronParser.TryParse("not a cron", out _));
        }

        [TestMethod]
        public void TryParse_Valid_ReturnsTrue()
        {
            Assert.IsTrue(CronParser.TryParse("*/5 * * * *", out var expr));
            Assert.IsNotNull(expr);
        }

        //  Extension methods

        [TestMethod]
        public void ParseCron_Extension()
        {
            var expr = "0 9 * * 1-5".ParseCron();
            Assert.AreEqual(CronFormat.Standard, expr.Format);
        }

        [TestMethod]
        public void TryParseCron_Extension()
        {
            Assert.IsTrue("*/10 * * * *".TryParseCron(out _));
            Assert.IsFalse("bad".TryParseCron(out _));
        }

        //  ToCronString round-trip

        [TestMethod]
        [DataRow("* * * * *")]
        [DataRow("0 9 * * 1-5")]
        [DataRow("*/15 9-17 * * *")]
        [DataRow("0 0 1,15 * *")]
        [DataRow("0 0 L * *")]
        [DataRow("0 0 LW * *")]
        [DataRow("0 0 15W * *")]
        [DataRow("0 0 * * 5#3")]
        [DataRow("0 0 * * 5L")]
        public void RoundTrip_CronString(string cron)
        {
            var expr = CronParser.Parse(cron);
            Assert.AreEqual(cron, expr.ToCronString());
        }

        //  IsMatch

        [TestMethod]
        public void IsMatch_EveryMinute_AlwaysTrue()
        {
            var expr = CronParser.Parse("* * * * *");
            Assert.IsTrue(expr.IsMatch(new DateTime(2026, 3, 15, 14, 30, 0)));
        }

        [TestMethod]
        public void IsMatch_SpecificTime()
        {
            var expr = CronParser.Parse("30 9 * * *");
            Assert.IsTrue(expr.IsMatch(new DateTime(2026, 6, 1, 9, 30, 0)));
            Assert.IsFalse(expr.IsMatch(new DateTime(2026, 6, 1, 9, 31, 0)));
            Assert.IsFalse(expr.IsMatch(new DateTime(2026, 6, 1, 10, 30, 0)));
        }

        [TestMethod]
        public void IsMatch_Weekdays()
        {
            var expr = CronParser.Parse("0 9 * * 1-5");
            // March 24, 2026 is Tuesday
            Assert.IsTrue(expr.IsMatch(new DateTime(2026, 3, 24, 9, 0, 0)));
            // March 22, 2026 is Sunday
            Assert.IsFalse(expr.IsMatch(new DateTime(2026, 3, 22, 9, 0, 0)));
        }

        [TestMethod]
        public void IsMatch_LastDayOfMonth()
        {
            var expr = CronParser.Parse("0 0 L * *");
            Assert.IsTrue(expr.IsMatch(new DateTime(2026, 3, 31, 0, 0, 0)));
            Assert.IsFalse(expr.IsMatch(new DateTime(2026, 3, 30, 0, 0, 0)));
            // February 28 (non-leap 2026)
            Assert.IsTrue(expr.IsMatch(new DateTime(2026, 2, 28, 0, 0, 0)));
        }

        [TestMethod]
        public void IsMatch_Hash_ThirdFriday()
        {
            var expr = CronParser.Parse("0 0 * * 5#3");
            // 3rd Friday of March 2026 is March 20
            Assert.IsTrue(expr.IsMatch(new DateTime(2026, 3, 20, 0, 0, 0)));
            Assert.IsFalse(expr.IsMatch(new DateTime(2026, 3, 13, 0, 0, 0)));
        }

        [TestMethod]
        public void IsMatch_NearestWeekday_Saturday()
        {
            var expr = CronParser.Parse("0 0 21W * *");
            // March 21, 2026 is Saturday → nearest weekday is Friday March 20
            Assert.IsTrue(expr.IsMatch(new DateTime(2026, 3, 20, 0, 0, 0)));
            Assert.IsFalse(expr.IsMatch(new DateTime(2026, 3, 21, 0, 0, 0)));
        }

        [TestMethod]
        public void IsMatch_NearestWeekday_Sunday()
        {
            var expr = CronParser.Parse("0 0 22W * *");
            // March 22, 2026 is Sunday → nearest weekday is Monday March 23
            Assert.IsTrue(expr.IsMatch(new DateTime(2026, 3, 23, 0, 0, 0)));
            Assert.IsFalse(expr.IsMatch(new DateTime(2026, 3, 22, 0, 0, 0)));
        }

        [TestMethod]
        public void IsMatch_WithSeconds()
        {
            var expr = CronParser.Parse("30 */5 * * * *");
            Assert.IsTrue(expr.IsMatch(new DateTime(2026, 1, 1, 0, 0, 30)));
            Assert.IsTrue(expr.IsMatch(new DateTime(2026, 1, 1, 0, 5, 30)));
            Assert.IsFalse(expr.IsMatch(new DateTime(2026, 1, 1, 0, 0, 31)));
        }

        //  GetNextOccurrence

        [TestMethod]
        public void GetNextOccurrence_EveryMinute()
        {
            var expr = CronParser.Parse("* * * * *");
            var from = new DateTime(2026, 1, 1, 0, 0, 0);
            var next = expr.GetNextOccurrence(from);
            Assert.IsNotNull(next);
            Assert.AreEqual(new DateTime(2026, 1, 1, 0, 1, 0), next.Value);
        }

        [TestMethod]
        public void GetNextOccurrence_SpecificTime_NextDay()
        {
            var expr = CronParser.Parse("0 9 * * *");
            var from = new DateTime(2026, 3, 15, 10, 0, 0);
            var next = expr.GetNextOccurrence(from);
            Assert.IsNotNull(next);
            Assert.AreEqual(new DateTime(2026, 3, 16, 9, 0, 0), next.Value);
        }

        [TestMethod]
        public void GetNextOccurrence_Weekdays_SkipsWeekend()
        {
            var expr = CronParser.Parse("0 9 * * 1-5");
            // Friday March 27, 2026, 10:00 → next is Monday March 30 at 9:00
            var from = new DateTime(2026, 3, 27, 10, 0, 0);
            var next = expr.GetNextOccurrence(from);
            Assert.IsNotNull(next);
            Assert.AreEqual(new DateTime(2026, 3, 30, 9, 0, 0), next.Value);
        }

        [TestMethod]
        public void GetNextOccurrence_Monthly()
        {
            var expr = CronParser.Parse("0 0 1 * *");
            var from = new DateTime(2026, 3, 15, 0, 0, 0);
            var next = expr.GetNextOccurrence(from);
            Assert.IsNotNull(next);
            Assert.AreEqual(new DateTime(2026, 4, 1, 0, 0, 0), next.Value);
        }

        [TestMethod]
        public void GetNextOccurrence_Inclusive()
        {
            var expr = CronParser.Parse("0 9 * * *");
            var from = new DateTime(2026, 3, 15, 9, 0, 0);
            var next = expr.GetNextOccurrence(from, inclusive: true);
            Assert.IsNotNull(next);
            Assert.AreEqual(from, next.Value);
        }

        [TestMethod]
        public void GetNextOccurrences_ReturnsMultiple()
        {
            var expr = CronParser.Parse("0 0 * * *");
            var from = new DateTime(2026, 1, 1, 0, 0, 0);
            var list = expr.GetNextOccurrences(from, 3);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(new DateTime(2026, 1, 2, 0, 0, 0), list[0]);
            Assert.AreEqual(new DateTime(2026, 1, 3, 0, 0, 0), list[1]);
            Assert.AreEqual(new DateTime(2026, 1, 4, 0, 0, 0), list[2]);
        }

        [TestMethod]
        public void GetOccurrences_BetweenDates()
        {
            var expr = CronParser.Parse("0 12 * * *");
            var from = new DateTime(2026, 3, 1, 0, 0, 0);
            var to = new DateTime(2026, 3, 3, 23, 59, 0);
            var results = expr.GetOccurrences(from, to).ToList();
            Assert.AreEqual(3, results.Count);
        }

        //  GetPreviousOccurrence

        [TestMethod]
        public void GetPreviousOccurrence_SpecificTime()
        {
            var expr = CronParser.Parse("0 9 * * *");
            var from = new DateTime(2026, 3, 15, 8, 0, 0);
            var prev = expr.GetPreviousOccurrence(from);
            Assert.IsNotNull(prev);
            Assert.AreEqual(new DateTime(2026, 3, 14, 9, 0, 0), prev.Value);
        }

        //  Describe / ToString

        [TestMethod]
        public void Describe_EveryMinute()
        {
            var expr = CronParser.Parse("* * * * *");
            Assert.AreEqual("Every minute", expr.Describe());
        }

        [TestMethod]
        public void Describe_EveryNMinutes()
        {
            var expr = CronParser.Parse("*/5 * * * *");
            TestContext.WriteLine(expr.Describe());
            Assert.IsTrue(expr.Describe().Contains("5 minutes"));
        }

        [TestMethod]
        public void Describe_SpecificTime()
        {
            var expr = CronParser.Parse("30 9 * * *");
            TestContext.WriteLine(expr.Describe());
            Assert.IsTrue(expr.Describe().Contains("9:30 AM"));
        }

        [TestMethod]
        public void Describe_Weekdays()
        {
            var expr = CronParser.Parse("0 9 * * 1-5");
            TestContext.WriteLine(expr.Describe());
            var desc = expr.Describe();
            Assert.IsTrue(desc.Contains("9:00 AM"));
            Assert.IsTrue(desc.Contains("Monday") && desc.Contains("Friday"));
        }

        [TestMethod]
        public void Describe_LastDay()
        {
            var expr = CronParser.Parse("0 0 L * *");
            TestContext.WriteLine(expr.Describe());
            Assert.IsTrue(expr.Describe().Contains("last day"));
        }

        [TestMethod]
        public void Describe_ThirdFriday()
        {
            var expr = CronParser.Parse("0 0 * * 5#3");
            TestContext.WriteLine(expr.Describe());
            Assert.IsTrue(expr.Describe().Contains("3rd") && expr.Describe().Contains("Friday"));
        }

        [TestMethod]
        public void ToString_ReturnsDescription()
        {
            var expr = CronParser.Parse("0 9 * * 1-5");
            Assert.AreEqual(expr.Describe(), expr.ToString());
        }

        //  Equality

        [TestMethod]
        public void Equality_SameExpression()
        {
            var a = CronParser.Parse("*/5 * * * *");
            var b = CronParser.Parse("*/5 * * * *");
            Assert.AreEqual(a, b);
            Assert.IsTrue(a == b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void Equality_DifferentExpression()
        {
            var a = CronParser.Parse("*/5 * * * *");
            var b = CronParser.Parse("*/10 * * * *");
            Assert.AreNotEqual(a, b);
        }

        //  CronField matching values

        [TestMethod]
        public void CronField_GetMatchingValues_Step()
        {
            var expr = CronParser.Parse("*/15 * * * *");
            var values = expr.Minutes.GetMatchingValues();
            CollectionAssert.AreEquivalent(new[] { 0, 15, 30, 45 }, values.ToList());
        }

        [TestMethod]
        public void CronField_GetMatchingValues_Range()
        {
            var expr = CronParser.Parse("* 9-12 * * *");
            var values = expr.Hours.GetMatchingValues();
            CollectionAssert.AreEquivalent(new[] { 9, 10, 11, 12 }, values.ToList());
        }

        [TestMethod]
        public void CronField_GetMatchingValues_LastDay_Context()
        {
            var expr = CronParser.Parse("0 0 L * *");
            var marchValues = expr.DayOfMonth.GetMatchingValues(2026, 3);
            CollectionAssert.AreEquivalent(new[] { 31 }, marchValues.ToList());
            var febValues = expr.DayOfMonth.GetMatchingValues(2026, 2);
            CollectionAssert.AreEquivalent(new[] { 28 }, febValues.ToList());
        }

        [TestMethod]
        public void CronField_GetMatchingValues_LeapYear()
        {
            var expr = CronParser.Parse("0 0 L * *");
            var febValues = expr.DayOfMonth.GetMatchingValues(2028, 2); // 2028 is a leap year
            CollectionAssert.AreEquivalent(new[] { 29 }, febValues.ToList());
        }
    }
}
