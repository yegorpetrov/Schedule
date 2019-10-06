using System;
using System.Linq;

namespace Schedule.Expressions
{
    public partial class ScheduleExpression
    {
        [Obsolete("Please use an external parser")]
        public ScheduleExpression(string expression)
        {
            var datePartSplit = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            (_yyyyExp, _mmExp, _ddExp) = ParseDatePattern(datePartSplit.FirstOrDefault(p => p.Contains('.')));
            _dowExp = ParseDayOfWeekPattern(datePartSplit.FirstOrDefault(p => !p.Contains('.') && !p.Contains(':')));
            (_hhExp, _minExp, _ssExp, _fffExp) = ParseTimePattern(datePartSplit.FirstOrDefault(p => p.Contains(':')));
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

        //const string L = @"([*0-9,\/-]+)";

        //static readonly Regex _regex = new Regex(
        //    $@"({L}\.{L}\.{L})? {L}? ({L}\:{L}\:{L}(\.{L})?)");
    }
}
