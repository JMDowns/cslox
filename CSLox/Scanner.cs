using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static CSLox.TokenType;

namespace CSLox
{
    class Scanner
    {
        private static readonly Dictionary<String, TokenType> Keywords;

        private readonly string Source;
        private readonly List<Token> Tokens = new List<Token>();
        private int Start = 0;
        private int Current = 0;
        private int Line = 1;

        static Scanner()
        {
            Keywords = new Dictionary<String, TokenType>();
            Keywords.Add("and", AND);
            Keywords.Add("class", CLASS);
            Keywords.Add("else", ELSE);
            Keywords.Add("false", FALSE);
            Keywords.Add("for", FOR);
            Keywords.Add("fun", FUN);
            Keywords.Add("if", IF);
            Keywords.Add("nil", NIL);
            Keywords.Add("or", OR);
            Keywords.Add("print", PRINT);
            Keywords.Add("return", RETURN);
            Keywords.Add("super", SUPER);
            Keywords.Add("this", THIS);
            Keywords.Add("true", TRUE);
            Keywords.Add("var", VAR);
            Keywords.Add("while", WHILE);
        }
        public Scanner(string Source)
        {
            this.Source = Source;
        
    }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                //We are at the beginning of the next lexeme
                Start = Current;
                ScanToken();
            }

            Tokens.Add(new Token(EOF, "", null, Line));
            return Tokens;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                //One Character Lexemes
                case '(': AddToken(LEFT_PAREN); break;
                case ')': AddToken(RIGHT_PAREN); break;
                case '{': AddToken(LEFT_BRACE); break;
                case '}': AddToken(RIGHT_BRACE); break;
                case ',': AddToken(COMMA); break;
                case '.': AddToken(DOT); break;
                case '-': AddToken(MINUS); break;
                case '+': AddToken(PLUS); break;
                case ';': AddToken(SEMICOLON); break;
                case '*': AddToken(STAR); break;

                //One to two char lexemes
                case '!':
                    AddToken(Match('=') ? BANG_EQUAL : BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? LESS_EQUAL : LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? GREATER_EQUAL : GREATER);
                    break;

                //Division or comments?
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of a line
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else
                    {
                        AddToken(SLASH);
                    }

                    break;

                //Whitespaces
                case ' ': break;
                case '\r': break;
                case '\t': break;

                case '\n': Line++; break;

                case '"': String(); break;

                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(Line, "unexpected character");
                    }
                    break;
            }
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            String text = Source.Substring(Start, Current - Start);
            TokenType type;
            if (!Keywords.ContainsKey(text))
            {
                type = IDENTIFIER;
            } else
            {
                type = Keywords[text];
            }
            AddToken(type);
        }

        private bool IsDigit(char c)
        {
            return (c >= '0' && c <= '9');
        }

        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            //Look for a fractional part
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                //Consume the '.'
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            AddToken(NUMBER, Double.Parse(Source.Substring(Start, Current - Start)));
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') Line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(Line, "Unterminated String.");
                return;
            }

            //the closing "
            Advance();

            // Trim the surrounding quotes
            string value = Source.Substring(Start + 1, (Current - 2)-Start);
            AddToken(STRING, value);
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (Source.ElementAtOrDefault(Current) != expected) return false;

            Current++;
            return true;
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return Source.ElementAtOrDefault(Current);
        }

        private char PeekNext()
        {
            if (Current + 1 >= Source.Length) return '\0';
            return Source.ElementAtOrDefault(Current + 1);
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private bool IsAtEnd()
        {
            return Current >= Source.Length;
        }

        private char Advance()
        {
            Current++;
            return Source.ElementAtOrDefault(Current - 1);
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            string text = Source.Substring(Start, Current-Start);
            Tokens.Add(new Token(type, text, literal, Line));
        }
    }
}
