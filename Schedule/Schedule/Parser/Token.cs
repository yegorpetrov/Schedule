using System;
using System.Collections.Generic;
using System.Text;

namespace Schedule.Parser
{
    public struct Token
    {
        private int _pos;

        public Token(Kind kind, char value, int pos) : this(kind, value.ToString(), pos)
        {
            
        }

        public Token(Kind kind, string value, int pos)
        {
            Type = kind;
            Value = value;
            _pos = pos;
        }

        public enum Kind
        {
            Unknown, EOF, Whitespace, Number, Dash, Dot, Comma, Colon, Wildcard, Slash
        }

        public Kind Type { get; }

        public string Value { get; }

        public static IEnumerable<Token> Tokenize(string input)
        {
            var buffer = new StringBuilder(4);
            int pos = 0;
            Kind prev = Kind.Unknown, current;

            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (char.IsWhiteSpace(c)) current = Kind.Whitespace;
                else if (char.IsDigit(c)) current = Kind.Number;
                else if (c == '-') current = Kind.Dash;
                else if (c == '.') current = Kind.Dot;
                else if (c == ',') current = Kind.Comma;
                else if (c == ':') current = Kind.Colon;
                else if (c == '*') current = Kind.Wildcard;
                else if (c == '/') current = Kind.Slash;
                else current = Kind.Unknown;

                if (prev != current)
                {
                    if (buffer.Length > 0)
                    {
                        yield return new Token(prev, buffer.ToString(), pos);
                        buffer.Clear();
                    }

                    prev = current;
                    pos = i;
                }

                buffer.Append(c);
            }

            if (buffer.Length > 0)
            {
                yield return new Token(prev, buffer.ToString(), pos);
                buffer.Clear();
            }

            yield return new Token(Kind.EOF, string.Empty, pos + 1);
        }

        public override string ToString()
        {
            return $"{Type} '{Value}' at {_pos}";
        }
    }
}
