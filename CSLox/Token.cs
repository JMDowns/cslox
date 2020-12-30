using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    class Token
    {
        readonly TokenType Type;
        readonly string Lexeme;
        readonly object Literal;
        readonly int Line;

        public Token(TokenType type, string lexeme, object literal, int line)
        {
            this.Type = type;
            this.Lexeme = lexeme;
            this.Literal = literal;
            this.Line = line;
        }

        public override string ToString()
        {
            return Type + " " + Lexeme + " " + Literal;
        }
    }
}
