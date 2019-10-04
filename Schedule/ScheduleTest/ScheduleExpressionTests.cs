using Schedule;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xunit;

namespace ScheduleTest
{
    public class ScheduleExpressionTests
    {
        public DateTime ScheduleNext(string expression, DateTime baseTime)
        {
            new ScheduleExpression(expression).Find(baseTime, ref baseTime, true);
            return baseTime;
        }

        public DateTime SchedulePrev(string expression, DateTime baseTime)
        {
            new ScheduleExpression(expression).Find(baseTime, ref baseTime, false);
            return baseTime;
        }

        [Fact]
        public void NextSeconds()
        {
            var a = new DateTime(2019, 10, 2, 10, 02, 45);
            var b = new DateTime(2019, 10, 2, 10, 02, 46);

            Assert.Equal(b, ScheduleNext("*.*.* * *:*:46.000", a));
        }

        [Fact]
        public void NextMinute()
        {
            var a = new DateTime(2019, 10, 2, 10, 02, 45);
            var b = new DateTime(2019, 10, 2, 10, 03, 38);

            Assert.Equal(b, ScheduleNext("*.*.* * *:*:38.000", a));
        }

        [Fact]
        public void NextHour()
        {
            var a = new DateTime(2019, 10, 2, 10, 02, 45);
            var b = new DateTime(2019, 10, 2, 11, 00, 00);

            Assert.Equal(b, ScheduleNext("*.*.* * *:00:00.000", a));
        }

        [Fact]
        public void NextDay()
        {
            var a = new DateTime(2019, 10, 2, 10, 02, 45);
            var b = new DateTime(2019, 10, 3, 00, 00, 00);

            Assert.Equal(b, ScheduleNext("*.*.* * 00:00:00.000", a));
        }

        [Fact]
        public void NextMonth()
        {
            var a = new DateTime(2019, 10, 2, 10, 02, 45);
            var b = new DateTime(2019, 11, 1, 00, 00, 00);

            Assert.Equal(b, ScheduleNext("*.*.1 * 00:00:00.000", a));
        }

        [Fact]
        public void NextYear()
        {
            var a = new DateTime(2019, 10, 2, 10, 02, 45);
            var b = new DateTime(2020, 1, 1, 00, 00, 00);

            Assert.Equal(b, ScheduleNext("*.1.1 * 00:00:00.000", a));
        }

        [Fact]
        public void In7Years()
        {
            var a = new DateTime(2019, 10, 2, 10, 02, 45);
            var b = new DateTime(2026, 1, 1, 00, 00, 00);

            Assert.Equal(b, ScheduleNext("2026.1.1 * 00:00:00.000", a));
        }

        [Fact]
        public void OctoberAndDecember()
        {
            var a = new DateTime(2019, 10, 2, 10, 02, 45);
            var b = new DateTime(2019, 10, 3, 00, 00, 00);

            Assert.Equal(b, ScheduleNext("*.10,12.* * 00:00:00.000", a));

            a = new DateTime(2019, 12, 2, 10, 02, 45);
            b = new DateTime(2019, 12, 3, 00, 00, 00);

            Assert.Equal(b, ScheduleNext("*.10,12.* * 00:00:00.000", a));

            a = new DateTime(2019, 11, 2, 10, 02, 38);
            b = new DateTime(2019, 12, 1, 00, 00, 45);

            Assert.Equal(b, ScheduleNext("*.10,12.* * 00:00:45-49.000", a));

            a = new DateTime(2019, 11, 2, 10, 02, 38);
            b = new DateTime(2019, 10, 31, 00, 00, 49);

            Assert.Equal(b, SchedulePrev("*.10,12.* * 00:00:45-49.000", a));
        }

        [Fact]
        public void SameMoment()
        {
            var a = new DateTime(2019, 10, 2, 10, 02, 45);

            Assert.Equal(a, ScheduleNext("2019.10.2 * 10:02:45.000", a));
        }

        [Fact]
        public void NextWednesday()
        {
            var a = new DateTime(2019, 10, 3, 10, 02, 45);
            var b = new DateTime(2019, 10, 9, 0, 0, 0);

            Assert.Equal(b, ScheduleNext("*.*.* 3 *:*:*.*", a));
        }

        [Fact]
        public void February31st()
        {
            DateTime impossible = default;
            if (new ScheduleExpression("*.2.31 * *:*:*.*").Find(new DateTime(2019, 1, 1), ref impossible, true))
            {
                throw new Exception("February 31st?");
            }
        }

        [Fact]
        public void FindNextLeapYear()
        {
            // 2020 is a leap year
            Assert.Equal(2020, ScheduleNext("*.2.29 * *:*:*.*", new DateTime(2019, 1, 1)).Year);
        }

        [Fact]
        public void FindTheEndOfNextMonthUsingThe32ndNotation()
        {
            var a = new DateTime(2020, 1, 1, 0, 0, 0);
            var b = new DateTime(2020, 2, 29, 0, 0, 0);

            Assert.Equal(b, ScheduleNext("*.2.32 * *:*:*.*", a));
        }

        [Fact]
        public void MonthEndsUsingThe32ndNotation()
        {
            Assert.Equal(31, ScheduleNext("*.*.32 * *:*:*.*", new DateTime(2020, 1, 31)).Day);
            Assert.Equal(29, ScheduleNext("*.*.32 * *:*:*.*", new DateTime(2020, 2, 29)).Day);
            Assert.Equal(31, ScheduleNext("*.*.32 * *:*:*.*", new DateTime(2020, 3, 31)).Day);
            Assert.Equal(30, ScheduleNext("*.*.32 * *:*:*.*", new DateTime(2020, 4, 30)).Day);
            Assert.Equal(31, ScheduleNext("*.*.32 * *:*:*.*", new DateTime(2020, 5, 31)).Day);
            Assert.Equal(30, ScheduleNext("*.*.32 * *:*:*.*", new DateTime(2020, 6, 30)).Day);
            Assert.Equal(31, ScheduleNext("*.*.32 * *:*:*.*", new DateTime(2020, 7, 31)).Day);
            Assert.Equal(31, ScheduleNext("*.*.32 * *:*:*.*", new DateTime(2020, 8, 31)).Day);
            Assert.Equal(30, ScheduleNext("*.*.32 * *:*:*.*", new DateTime(2020, 9, 30)).Day);
            Assert.Equal(31, ScheduleNext("*.*.32 * *:*:*.*", new DateTime(2020, 10, 31)).Day);
            Assert.Equal(30, ScheduleNext("*.*.32 * *:*:*.*", new DateTime(2020, 11, 30)).Day);
            Assert.Equal(31, ScheduleNext("*.*.32 * *:*:*.*", new DateTime(2020, 12, 31)).Day);
        }

        [Fact]
        public void FindPreviousLeapYear()
        {
            // 2020 is a leap year
            Assert.Equal(2020, SchedulePrev("*.2.29 * *:*:*.*", new DateTime(2021, 1, 1)).Year);
        }

        [Fact]
        public void PreviousSeconds()
        {
            var a = new DateTime(2019, 10, 2, 10, 02, 45);
            var b = new DateTime(2019, 10, 2, 10, 02, 46);

            Assert.Equal(a, SchedulePrev("*.*.* * *:*:45.000", b));
        }
    }
}
