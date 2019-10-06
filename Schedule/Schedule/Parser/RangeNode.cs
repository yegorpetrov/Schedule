using System;
using System.Collections.Generic;
using System.Text;

namespace Schedule.Parser
{
    public class RangeNode
    {
        public RangeNode(int start, int end, int step)
        {
            Start = start;
            End = end;
            Step = step;
        }

        public int Start { get; }
        public int End { get; }
        public int Step { get; }
    }
}
