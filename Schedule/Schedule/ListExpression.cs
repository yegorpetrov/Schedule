using System;
using System.Collections.Generic;
using System.Linq;

namespace Schedule
{
    public class ListExpression : IExpression<int>
    {
        private readonly IEnumerable<IExpression<int>> _elements;

        public ListExpression(string listExpression)
        {
            _elements = listExpression
                .Split(',')
                .Select(e => new RangeExpression(e))
                .ToArray();
        }

        public bool Find(int @base, ref int @out, bool forward)
        {
            var elementsInChosenDirection = forward ? _elements : _elements.Reverse();

            foreach (var entry in elementsInChosenDirection)
            {
                if (entry.Find(@base, ref @out, forward))
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return string.Join(',', _elements.Select(r => r.ToString()));
        }
    }
}
