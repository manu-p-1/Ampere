using Ampere.Str.Cron;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AmpereMSTest.Str.Cron
{
    [TestClass]
    public class CronBuilderTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Build_Default_EveryMinute()
        {
            var expr = new CronBuilder().Build();
            Assert.AreEqual("* * * * *", expr.ToCronString());
        }

        [TestMethod]
        public void Build_AtTime()
        {
            var expr = new CronBuilder()
                .AtTime(9, 30)
                .Build();
            Assert.AreEqual("30 9 * * *", expr.ToCronString());
        }

        [TestMethod]
        public void Build_EveryNMinutes()
        {
            var expr = new CronBuilder()
                .EveryNMinutes(15)
                .Build();
            Assert.AreEqual("*/15 * * * *", expr.ToCronString());
        }

        [TestMethod]
        public void Build_EveryNHours()
        {
            var expr = new CronBuilder()
                .EveryNHours(6)
                .Build();
            Assert.AreEqual("0 */6 * * *", expr.ToCronString());
        }

        [TestMethod]
        public void Build_Weekdays()
        {
            var expr = new CronBuilder()
                .AtTime(9, 0)
                .Weekdays()
                .Build();
            Assert.AreEqual("0 9 * * 1-5", expr.ToCronString());
        }

        [TestMethod]
        public void Build_Weekends()
        {
            var expr = new CronBuilder()
                .AtTime(10, 0)
                .Weekends()
                .Build();
            Assert.AreEqual("0 10 * * 0,6", expr.ToCronString());
        }

        [TestMethod]
        public void Build_SpecificDaysOfWeek()
        {
            var expr = new CronBuilder()
                .AtTime(9, 0)
                .OnDaysOfWeek(DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday)
                .Build();
            Assert.AreEqual("0 9 * * 1,3,5", expr.ToCronString());
        }

        [TestMethod]
        public void Build_DaysOfMonth()
        {
            var expr = new CronBuilder()
                .AtTime(0, 0)
                .OnDaysOfMonth(1, 15)
                .Build();
            Assert.AreEqual("0 0 1,15 * *", expr.ToCronString());
        }

        [TestMethod]
        public void Build_LastDayOfMonth()
        {
            var expr = new CronBuilder()
                .AtTime(0, 0)
                .OnLastDayOfMonth()
                .Build();
            Assert.AreEqual("0 0 L * *", expr.ToCronString());
        }

        [TestMethod]
        public void Build_LastWeekdayOfMonth()
        {
            var expr = new CronBuilder()
                .AtTime(17, 0)
                .OnLastWeekdayOfMonth()
                .Build();
            Assert.AreEqual("0 17 LW * *", expr.ToCronString());
        }

        [TestMethod]
        public void Build_NearestWeekday()
        {
            var expr = new CronBuilder()
                .AtTime(0, 0)
                .OnNearestWeekday(15)
                .Build();
            Assert.AreEqual("0 0 15W * *", expr.ToCronString());
        }

        [TestMethod]
        public void Build_NthDayOfWeek()
        {
            var expr = new CronBuilder()
                .AtTime(0, 0)
                .OnNthDayOfWeek(DayOfWeek.Friday, 3)
                .Build();
            Assert.AreEqual("0 0 * * 5#3", expr.ToCronString());
        }

        [TestMethod]
        public void Build_LastFriday()
        {
            var expr = new CronBuilder()
                .AtTime(0, 0)
                .OnLastDayOfWeek(DayOfWeek.Friday)
                .Build();
            Assert.AreEqual("0 0 * * 5L", expr.ToCronString());
        }

        [TestMethod]
        public void Build_InMonths()
        {
            var expr = new CronBuilder()
                .AtTime(0, 0)
                .OnDaysOfMonth(1)
                .InMonths(1, 6)
                .Build();
            Assert.AreEqual("0 0 1 1,6 *", expr.ToCronString());
        }

        [TestMethod]
        public void Build_WithSeconds()
        {
            var expr = new CronBuilder()
                .WithSeconds("30")
                .WithMinutes("*/5")
                .Build();
            Assert.AreEqual(CronFormat.WithSeconds, expr.Format);
            Assert.AreEqual("30 */5 * * * *", expr.ToCronString());
        }

        [TestMethod]
        public void Build_EveryNSeconds()
        {
            var expr = new CronBuilder()
                .EveryNSeconds(10)
                .Build();
            Assert.AreEqual(CronFormat.WithSeconds, expr.Format);
            Assert.IsTrue(expr.ToCronString().StartsWith("*/10"));
        }

        [TestMethod]
        public void Build_WithYear()
        {
            var expr = new CronBuilder()
                .AtTime(12, 0)
                .OnDaysOfMonth(1)
                .InMonths(1)
                .InYears(2026, 2027)
                .Build();
            Assert.AreEqual(CronFormat.WithSecondsAndYear, expr.Format);
            Assert.IsNotNull(expr.Year);
        }

        [TestMethod]
        public void Build_DuringHours()
        {
            var expr = new CronBuilder()
                .EveryNMinutes(15)
                .DuringHours(9, 17)
                .Build();
            Assert.AreEqual("*/15 9-17 * * *", expr.ToCronString());
        }

        [TestMethod]
        public void Build_Daily()
        {
            var expr = new CronBuilder().Daily().Build();
            Assert.AreEqual("0 0 * * *", expr.ToCronString());
        }

        [TestMethod]
        public void Build_Weekly()
        {
            var expr = new CronBuilder().Weekly().Build();
            Assert.AreEqual("0 0 * * 0", expr.ToCronString());
        }

        [TestMethod]
        public void Build_Monthly()
        {
            var expr = new CronBuilder().Monthly().Build();
            Assert.AreEqual("0 0 1 * *", expr.ToCronString());
        }

        [TestMethod]
        public void Build_Yearly()
        {
            var expr = new CronBuilder().Yearly().Build();
            Assert.AreEqual("0 0 1 1 *", expr.ToCronString());
        }

        [TestMethod]
        public void TryBuild_Valid()
        {
            var builder = new CronBuilder().AtTime(9, 0).Weekdays();
            Assert.IsTrue(builder.TryBuild(out var expr, out var error));
            Assert.IsNotNull(expr);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TryBuild_Invalid()
        {
            var builder = new CronBuilder().WithMinutes("99");
            Assert.IsFalse(builder.TryBuild(out var expr, out var error));
            Assert.IsNull(expr);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Build_EveryNMinutes_OutOfRange_Throws()
        {
            new CronBuilder().EveryNMinutes(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Build_DaysOfMonth_OutOfRange_Throws()
        {
            new CronBuilder().OnDaysOfMonth(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Build_InMonths_OutOfRange_Throws()
        {
            new CronBuilder().InMonths(13);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Build_NthDayOfWeek_OutOfRange_Throws()
        {
            new CronBuilder().OnNthDayOfWeek(DayOfWeek.Monday, 6);
        }

        [TestMethod]
        public void Builder_ToString_ReturnsDescription()
        {
            var builder = new CronBuilder().AtTime(9, 0).Weekdays();
            var desc = builder.ToString();
            TestContext.WriteLine(desc);
            Assert.IsTrue(desc.Contains("9:00 AM"));
        }

        [TestMethod]
        public void Builder_ToCronString()
        {
            var builder = new CronBuilder().AtTime(9, 0).Weekdays();
            Assert.AreEqual("0 9 * * 1-5", builder.ToCronString());
        }

        [TestMethod]
        public void Builder_RoundTrip_WithParser()
        {
            var built = new CronBuilder()
                .EveryNMinutes(15)
                .DuringHours(9, 17)
                .Weekdays()
                .Build();

            var parsed = CronParser.Parse(built.ToCronString());
            Assert.AreEqual(built, parsed);
        }

        [TestMethod]
        public void Builder_Complex_BusinessHours()
        {
            var expr = new CronBuilder()
                .EveryNMinutes(30)
                .DuringHours(8, 18)
                .Weekdays()
                .InMonths(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12)
                .Build();

            TestContext.WriteLine($"Cron: {expr.ToCronString()}");
            TestContext.WriteLine($"Description: {expr.Describe()}");

            Assert.AreEqual("*/30 8-18 * 1,2,3,4,5,6,7,8,9,10,11,12 1-5", expr.ToCronString());
        }
    }
}
