using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Schedule
{
    public class ScheduleExpression : IExpression<DateTime>
    {
        static readonly Calendar _calendar = CultureInfo.InvariantCulture.Calendar;

        //const string L = @"([*0-9,\/-]+)";

        //static readonly Regex _regex = new Regex(
        //    $@"({L}\.{L}\.{L})? {L}? ({L}\:{L}\:{L}(\.{L})?)");

        private readonly IExpression<int> yyyyExp, mmExp, ddExp, dowExp, hhExp, minExp, ssExp, fffExp;

        public ScheduleExpression(string expression)
        {
            var datePartSplit = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            (yyyyExp, mmExp, ddExp) = ParseDatePattern(datePartSplit.FirstOrDefault(p => p.Contains('.')));
            dowExp = ParseDayOfWeekPattern(datePartSplit.FirstOrDefault(p => !p.Contains('.') && !p.Contains(':')));
            (hhExp, minExp, ssExp, fffExp) = ParseTimePattern(datePartSplit.FirstOrDefault(p => p.Contains(':')));
        }

        private static (
            ListExpression year,
            ListExpression month,
            ListExpression day)
            ParseDatePattern(string datePattern)
        {
            var split = (datePattern ?? "*.*.*").Split('.');

            return (
                Unwild(split[0], 2000, 2100),
                Unwild(split[1], 1, 12),
                Unwild(split[2], 1, 31 + 1));
        }

        private static ListExpression ParseDayOfWeekPattern(string dayOfWeekPattern)
        {
            return Unwild(dayOfWeekPattern ?? "*", 0, 6);
        }

        private static (
            ListExpression hour,
            ListExpression minute,
            ListExpression second,
            ListExpression fractions)
            ParseTimePattern(string timePattern)
        {

            var split = (timePattern ?? "*:*:*.0").Split(':', '.');

            return (
                Unwild(split[0], 0, 23),
                Unwild(split[1], 0, 59),
                Unwild(split[2], 0, 59),
                Unwild(split[3], 0, 999));
        }

        /// <summary>
        /// Wildcard lowering
        /// </summary>
        private static ListExpression Unwild(string pattern, int min, int max)
        {
            return new ListExpression(pattern.Replace("*", $"{min}-{max}"));
        }

        /// <summary>
        /// Datetime components
        /// </summary>
        [Flags]
        private enum DT
        {
            Ms =    1 << 0,
            Sec =   1 << 1,
            Min =   1 << 2,
            Hour =  1 << 3,
            Day =   1 << 4,
            Month = 1 << 5,
            Year =  1 << 6
        }

        class Stepper
        {
            private Stepper() { }

            public static Stepper Forward => new Stepper
            {
                Init = default,
                Increment = 1,
            };

            public static Stepper Backward => new Stepper
            {
                Init = int.MaxValue,
                Increment = -1,
            };

            public int Init { get; private set; }

            public int Increment { get; private set; }

            public bool Compare(int a, int b)
            {
                if (Increment > 0) return a > b;
                else return a < b;
            }
        }

        public bool Find(DateTime baseTime, ref DateTime nextOccurance, bool forward)
        {
            var stepper = forward ? Stepper.Forward : Stepper.Backward;

            int dayOfWeek = (int)baseTime.DayOfWeek,
                ms = baseTime.Millisecond,
                second = baseTime.Second,
                minute = baseTime.Minute,
                hour = baseTime.Hour,
                day = baseTime.Day,
                month = baseTime.Month,
                year = baseTime.Year;

            // Resets each datetime component to its minimum
            void Reset(DT components)
            {
                if (components.HasFlag(DT.Ms)) fffExp.Find(stepper.Init, ref ms, forward);
                if (components.HasFlag(DT.Sec)) ssExp.Find(stepper.Init, ref second, forward);
                if (components.HasFlag(DT.Min)) minExp.Find(stepper.Init, ref minute, forward);
                if (components.HasFlag(DT.Hour)) hhExp.Find(stepper.Init, ref hour, forward);
                if (components.HasFlag(DT.Day)) ddExp.Find(stepper.Init, ref day, forward);
                if (components.HasFlag(DT.Month)) mmExp.Find(stepper.Init, ref month, forward);
                if (components.HasFlag(DT.Year)) yyyyExp.Find(stepper.Init, ref year, forward);
            }

            // Milliseconds

            if (!fffExp.Find(ms, ref ms, forward))
            {
                second += stepper.Increment;
            }

            // Seconds

            if (!ssExp.Find(second, ref second, forward))
            {
                minute += stepper.Increment;
                Reset(DT.Ms);
            }
            else if (stepper.Compare(second, baseTime.Second))
            {
                Reset(DT.Ms);
            }

            // Minutes

            if (!minExp.Find(minute, ref minute, forward))
            {
                hour += stepper.Increment;
                Reset(DT.Sec | DT.Ms);
            }
            else if (stepper.Compare(minute, baseTime.Minute))
            {
                Reset(DT.Sec | DT.Ms);
            }

            // Hours

            if (!hhExp.Find(hour, ref hour, forward))
            {
                day += stepper.Increment;
                Reset(DT.Min | DT.Sec | DT.Ms);
            }
            else if (stepper.Compare(hour, baseTime.Hour))
            {
                Reset(DT.Min | DT.Sec | DT.Ms);
            }

            do
            {
                // Days

                if (!ddExp.Find(day, ref day, forward))
                {
                    month += stepper.Increment;
                    Reset(DT.Hour | DT.Min | DT.Sec | DT.Ms);
                }
                else if (stepper.Compare(day, baseTime.Day))
                {
                    Reset(DT.Hour | DT.Min | DT.Sec | DT.Ms);
                }

                // Day of week requires special handling, see below

                ;

                // Months

                if (!mmExp.Find(month, ref month, forward))
                {
                    year += stepper.Increment;
                    Reset(DT.Day | DT.Hour | DT.Min | DT.Sec | DT.Ms);
                }
                else if (stepper.Compare(month, baseTime.Month))
                {
                    Reset(DT.Day | DT.Hour | DT.Min | DT.Sec | DT.Ms);
                }

                // Years

                if (!yyyyExp.Find(year, ref year, forward))
                {
                    nextOccurance = DateTime.MaxValue;
                    return false;
                }
                else if (stepper.Compare(year, baseTime.Year))
                {
                    Reset(DT.Month | DT.Day | DT.Hour | DT.Min | DT.Sec | DT.Ms);
                }

                if (day == 32)
                {
                    day = _calendar.GetDaysInMonth(year, month);
                }

                // Try next month if the current one overflows
                if ((day != baseTime.Day || month != baseTime.Month || year != baseTime.Year)
                    && day > _calendar.GetDaysInMonth(year, month))
                {
                    day = int.MaxValue - stepper.Init;
                    month += stepper.Increment;
                }
                else break;

            } while (true);

            nextOccurance = new DateTime(year, month, day, hour, minute, second).AddMilliseconds(ms);

            dowExp.Find((int)nextOccurance.DayOfWeek, ref dayOfWeek, forward);

            if ((int)nextOccurance.DayOfWeek != dayOfWeek)
            {
                return Find(nextOccurance.Date.AddDays(stepper.Increment), ref nextOccurance, forward);
            }
            else
            {
                return true;
            }
        }
    }
}
