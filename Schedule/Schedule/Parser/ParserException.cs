using System;

namespace Schedule.Parser
{
    public class ParserException : Exception
    {
        public ParserException(Token pos, string message) : base(message)
        {
            Position = pos;
        }

        public Token Position { get; }
    }
}
