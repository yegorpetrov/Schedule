using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SC = Test.Schedule;

namespace ScheduleTest
{
    /// <summary>
    /// The original Schedule example tests
    /// </summary>
    /// <remarks>
    /// For extending testing please refer to the <see cref="Schedule.Expressions.ScheduleExpression"/> tests in <see cref="ScheduleExpressionTests"/>
    /// </remarks>
    public class ScheduleTests
    {
        [Fact]
        public void EmptySchedule()
        {
            var schedule = new SC();

            var a = new DateTime(2019, 1, 1);
            var b = a.AddMilliseconds(1);

            Assert.Equal(b, schedule.NextEvent(a));
            Assert.Equal(a, schedule.PrevEvent(b));
        }

        [Fact]
        public void Only10SharpOnWorkDaysInSeptemberOnOddDates()
        {
            var schedule = new SC("*.9.*/2 1-5 10:00:00.000");

            DateTime dt = new DateTime(2019, 8, 25);

            do
            {
                dt = schedule.NextEvent(dt);
                Assert.True(
                    dt.Hour == 10 && dt.Minute == 0 && dt.Second == 0 && dt.Millisecond == 0 &&
                    dt.DayOfWeek >= DayOfWeek.Monday && dt.DayOfWeek <= DayOfWeek.Friday &&
                    dt.Month == 9 && dt.Day % 2 == 1);

                var nearest = schedule.NearestEvent(dt);

                Assert.Equal(nearest, dt);
            } while (dt.Year == 2019);

            Assert.Equal(2020, dt.Year);
        }

        [Fact]
        public void Hourly()
        {
            var schedule = new SC("*:00:00");

            DateTime dt = new DateTime(2019, 8, 31, 23, 0, 0);

            do
            {
                dt = schedule.NextEvent(dt);
                Assert.True(dt.Minute == 0 && dt.Second == 0 && dt.Millisecond == 0);
            } while (dt.Month == 8);
        }

        [Fact]
        public void HalfPastOneAMEachMonthFirstDay()
        {
            var schedule = new SC("*.*.01 01:30:00");

            DateTime dt = new DateTime(2019, 1, 1, 14, 0, 0);

            do
            {
                dt = schedule.NextEvent(dt);
                Assert.True(dt.Day == 1 && dt.Hour == 1 && dt.Minute == 30 && dt.Second == 0 && dt.Millisecond == 0);
            } while (dt.Year < 2050);

            do
            {
                dt = schedule.PrevEvent(dt);
                Assert.True(dt.Day == 1 && dt.Hour == 1 && dt.Minute == 30 && dt.Second == 0 && dt.Millisecond == 0);
            } while (dt.Year > 2019);
        }
    }
}
