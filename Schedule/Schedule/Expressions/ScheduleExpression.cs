using System;
using System.Globalization;

namespace Schedule.Expressions
{
    public partial class ScheduleExpression : IExpression<DateTime>
    {
        static readonly Calendar _calendar = CultureInfo.InvariantCulture.Calendar;

        private readonly IExpression<int>
            _yyyyExp, _mmExp, _ddExp, _dowExp,
            _hhExp, _minExp, _ssExp, _fffExp;

        public ScheduleExpression(
            IExpression<int> yyyyExp,
            IExpression<int> mmExp,
            IExpression<int> ddExp,
            IExpression<int> dowExp,
            IExpression<int> hhExp,
            IExpression<int> minExp,
            IExpression<int> ssExp,
            IExpression<int> fffExp)
        {
            _yyyyExp = yyyyExp;
            _mmExp = mmExp;
            _ddExp = ddExp;
            _dowExp = dowExp;
            _hhExp = hhExp;
            _minExp = minExp;
            _ssExp = ssExp;
            _fffExp = fffExp;
        }

        /// <summary>
        /// Datetime components
        /// </summary>
        [Flags]
        private enum DT
        {
            Ms = 1 << 0,
            Sec = 1 << 1,
            Min = 1 << 2,
            Hour = 1 << 3,
            Day = 1 << 4,
            Month = 1 << 5,
            Year = 1 << 6
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

            DateTime GetCurrentResult()
            {
                return new DateTime(year, month, day, hour, minute, second).AddMilliseconds(ms);
            }

            // Resets each datetime component to its minimum
            void Reset(DT components)
            {
                if (components.HasFlag(DT.Ms)) _fffExp.Find(stepper.Init, ref ms, forward);
                if (components.HasFlag(DT.Sec)) _ssExp.Find(stepper.Init, ref second, forward);
                if (components.HasFlag(DT.Min)) _minExp.Find(stepper.Init, ref minute, forward);
                if (components.HasFlag(DT.Hour)) _hhExp.Find(stepper.Init, ref hour, forward);
                if (components.HasFlag(DT.Day)) _ddExp.Find(stepper.Init, ref day, forward);
                if (components.HasFlag(DT.Month)) _mmExp.Find(stepper.Init, ref month, forward);
                if (components.HasFlag(DT.Year)) _yyyyExp.Find(stepper.Init, ref year, forward);
            }

            // Milliseconds

            if (!_fffExp.Find(ms, ref ms, forward))
            {
                second += stepper.Increment;
            }

            // Seconds

            if (!_ssExp.Find(second, ref second, forward))
            {
                minute += stepper.Increment;
                Reset(DT.Ms);
            }
            else if (stepper.Compare(second, baseTime.Second))
            {
                Reset(DT.Ms);
            }

            // Minutes

            if (!_minExp.Find(minute, ref minute, forward))
            {
                hour += stepper.Increment;
                Reset(DT.Sec | DT.Ms);
            }
            else if (stepper.Compare(minute, baseTime.Minute))
            {
                Reset(DT.Sec | DT.Ms);
            }

            // Hours

            if (!_hhExp.Find(hour, ref hour, forward))
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

                if (!_ddExp.Find(day, ref day, forward))
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

                if (!_mmExp.Find(month, ref month, forward))
                {
                    year += stepper.Increment;
                    Reset(DT.Day | DT.Hour | DT.Min | DT.Sec | DT.Ms);
                }
                else if (stepper.Compare(month, baseTime.Month))
                {
                    Reset(DT.Day | DT.Hour | DT.Min | DT.Sec | DT.Ms);
                }

                // Years

                if (!_yyyyExp.Find(year, ref year, forward))
                {
                    nextOccurance = DateTime.MaxValue;
                    return false;
                }
                else if (stepper.Compare(year, baseTime.Year))
                {
                    Reset(DT.Month | DT.Day | DT.Hour | DT.Min | DT.Sec | DT.Ms);
                }

                if (day == 32) // patch to the end of month
                {
                    day = _calendar.GetDaysInMonth(year, month);

                    // but make sure we don't move in the opposite direction
                    if ((forward && GetCurrentResult() < baseTime) || (!forward && GetCurrentResult() > baseTime))
                    {
                        day = 32;
                    }
                }

                // Try next month if the current one overflows
                if ((day != baseTime.Day || month != baseTime.Month || year != baseTime.Year)
                    && day > _calendar.GetDaysInMonth(year, month))
                {
                    day = int.MaxValue - stepper.Init;
                    month += stepper.Increment;
                }
                else
                {
                    break;
                }

            } while (true);

            nextOccurance = GetCurrentResult();

            _dowExp.Find((int)nextOccurance.DayOfWeek, ref dayOfWeek, forward);

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
