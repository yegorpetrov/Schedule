﻿using System;
using System.Linq;

namespace Schedule.Expressions
{
    public class RangeExpression : IExpression<int>
    {
        private readonly int _min, _max, _step, _lastIndex;

        public RangeExpression(int min, int max, int step)
        {
            _min = min;
            _max = max;
            _step = step;
            _lastIndex = (_max - _min) / _step;
        }

        [Obsolete]
        public RangeExpression(string expression)
        {
            var split0 = expression.Split('/');
            var split1 = split0[0].Split('-');
            _min = int.Parse(split1[0]);
            _max = int.Parse(split1.ElementAtOrDefault(1) ?? _min.ToString());
            _step = int.Parse(split0.ElementAtOrDefault(1) ?? "1");
            _lastIndex = (_max - _min) / _step;
        }

        public bool Find(int @base, ref int @out, bool forward)
        {
            if (forward)
            {
                return CapValue(Math.Ceiling, @base, ref @out);
            }
            else
            {
                return CapValue(Math.Floor, @base, ref @out);
            }
        }

        private bool CapValue(Func<double, double> stepper, int @base, ref int @out)
        {
            double halfstep = 0.5; // Used to detect direction

            var stepIndex = (int)stepper((@base - _min) / (double)_step);

            if (stepIndex >= _lastIndex) // Cap from the top
            {
                @out = _lastIndex * _step + _min;
                // Going back <<<, we're expected to be ahead
                return stepper(halfstep) < halfstep || @out == @base;
            }
            else if (stepIndex < 0) // Cap from the bottom
            {
                @out = _min;
                // Going forward >>>, we're expected to be behind
                return stepper(halfstep) > halfstep || @out == @base;
            }
            else
            {
                // The value belongs to this range
                @out = stepIndex % _lastIndex * _step + _min;
                return true;
            }
        }
        public override string ToString()
        {
            return $"{_min}-{_max}/{_step}";
        }
    }
}
