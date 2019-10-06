using Schedule.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schedule.Parser
{
    public class ScheduleParser
    {
        private bool Accept(IEnumerator<Token> tokens, Token.Kind tkind)
        {
            return tokens.Current.Type == tkind && (tokens.MoveNext() || tkind == Token.Kind.EOF);
        }

        private Token Expect(IEnumerator<Token> tokens, Token.Kind tkind)
        {
            var current = tokens.Current;

            if (!Accept(tokens, tkind))
            {
                throw new ParserException(tokens.Current,
                  $"Expected a {tkind} instead of {tokens.Current}");
            }
            else return current;
        }

        public ScheduleExpression ParseSchedule(string schedule)
        {
            var tokens = Token.Tokenize(schedule).GetEnumerator();

            tokens.MoveNext();

            var year = List(tokens).ToArray();
            Expect(tokens, Token.Kind.Dot);
            var month = List(tokens).ToArray();
            Expect(tokens, Token.Kind.Dot);
            var day = List(tokens).ToArray();

            Expect(tokens, Token.Kind.Whitespace);
            
            IEnumerable<RangeNode> dayOfWeek, hours;

            dayOfWeek = hours = List(tokens).ToArray();
            
            if (Accept(tokens, Token.Kind.Colon))
            {
                dayOfWeek = new[] { new RangeNode(0, 6, 1) };
            }
            else
            {
                Expect(tokens, Token.Kind.Whitespace);
                hours = List(tokens).ToArray();
                Expect(tokens, Token.Kind.Colon);
            }

            var minutes = List(tokens).ToArray();
            Expect(tokens, Token.Kind.Colon);
            var seconds = List(tokens).ToArray();

            IEnumerable<RangeNode> fractions;

            if (Accept(tokens, Token.Kind.Dot))
            {
                fractions = List(tokens).ToArray();
            }
            else
            {
                fractions = new[] { new RangeNode(0, 0, 1) };
            }
            Expect(tokens, Token.Kind.EOF);

            ListExpression MakeListExpression(IEnumerable<RangeNode> nodes, int min, int max)
            {
                return new ListExpression(nodes.Select(r => PerformExpressionLowering(r, min, max)).ToArray());
            }

            return new ScheduleExpression(
                MakeListExpression(year, 2000, 2100),
                MakeListExpression(month, 1, 12),
                MakeListExpression(day, 1, 32),
                MakeListExpression(dayOfWeek, 0, 6),
                MakeListExpression(hours, 0, 23),
                MakeListExpression(minutes, 0, 59),
                MakeListExpression(seconds, 0, 59),
                MakeListExpression(fractions, 0, 999));
        }

        private static RangeExpression PerformExpressionLowering(RangeNode original, int low, int high)
        {
            if (original.Start == int.MaxValue && original.End == int.MaxValue)
            {
                return new RangeExpression(low, high, original.Step);
            }
            else
            {
                return new RangeExpression(original.Start, original.End, original.Step);
            }
        }

        private IEnumerable<RangeNode> List(IEnumerator<Token> tokens)
        {
            do
            {
                yield return Range(tokens);
            }
            while (Accept(tokens, Token.Kind.Comma));
        }

        private RangeNode Range(IEnumerator<Token> tokens)
        {
            int start, end, step;

            if (Accept(tokens, Token.Kind.Wildcard))
            {
                start = end = int.MaxValue;
            }
            else
            {
                start = Number(tokens);
                end = Accept(tokens, Token.Kind.Dash) ? Number(tokens) : start;
            }

            if (Accept(tokens, Token.Kind.Slash))
            {
                step = Number(tokens);
            }
            else
            {
                step = 1;
            };

            return new RangeNode(start, end, step);
        }

        private int Number(IEnumerator<Token> tokens)
        {
            return int.Parse(Expect(tokens, Token.Kind.Number).Value);
        }
    }
}
