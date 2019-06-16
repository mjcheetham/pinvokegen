using System;
using System.Linq;
using System.Text;

namespace PinvokeGen.Native.Syntax
{
    public class Lexer
    {
        private const char NUL = '\0';
        private const char CR  = '\r';
        private const char LF  = '\n';
        private const char TAB = '\t';
        private const char SP  = ' ';

        private readonly string _text;

        private int _position;

        public Lexer(string text)
        {
            _text = text;
        }

        private int CurrentPosition => _position;

        private char CurrentCharacter => Peek(0);

        private char NextCharacter => Peek(1);

        private char Consume()
        {
            return _text[_position++];
        }

        private char[] Consume(int count)
        {
            var ret = _text.Substring(_position, count).ToCharArray();
            _position += count;
            return ret;
        }

        private char Peek(int offset)
        {
            var index = _position + offset;

            if (index >= _text.Length)
            {
                return NUL;
            }

            return _text[index];
        }

        public SyntaxToken Lex()
        {
            int start = CurrentPosition;
            object value = null;
            SyntaxKind kind = SyntaxKind.InvalidToken;

            switch (CurrentCharacter)
            {
                case NUL:
                    kind = SyntaxKind.EndOfFile;
                    break;

                case '+' when NextCharacter == '+':
                    kind = SyntaxKind.PlusPlusToken;
                    Consume(2);
                    break;
                case '+' when NextCharacter == '=':
                    kind = SyntaxKind.PlusEqualToken;
                    Consume(2);
                    break;
                case '+':
                    kind = SyntaxKind.PlusToken;
                    Consume();
                    break;

                case '-' when NextCharacter == '>':
                    kind = SyntaxKind.ArrowToken;
                    Consume();
                    break;
                case '-' when NextCharacter == '-':
                    kind = SyntaxKind.MinusMinusToken;
                    Consume(2);
                    break;
                case '-' when NextCharacter == '=':
                    kind = SyntaxKind.MinusEqualToken;
                    Consume(2);
                    break;
                case '-':
                    kind = SyntaxKind.MinusToken;
                    Consume();
                    break;


                case '*' when NextCharacter == '=':
                    kind = SyntaxKind.AsteriskEqualToken;
                    Consume(2);
                    break;
                case '*':
                    kind = SyntaxKind.AsteriskToken;
                    Consume();
                    break;

                case '=' when NextCharacter == '=':
                    kind = SyntaxKind.EqualEqualToken;
                    Consume(2);
                    break;
                case '=':
                    kind = SyntaxKind.EqualToken;
                    Consume();
                    break;

                case '<' when NextCharacter == '<':
                    kind = SyntaxKind.LeftShiftToken;
                    Consume(2);
                    break;
                case '<' when NextCharacter == '=':
                    kind = SyntaxKind.LessThanEqualToken;
                    Consume(2);
                    break;
                case '<':
                    kind = SyntaxKind.LessThanToken;
                    Consume();
                    break;

                case '>' when NextCharacter == '>':
                    kind = SyntaxKind.RightShiftToken;
                    Consume(2);
                    break;
                case '>' when NextCharacter == '=':
                    kind = SyntaxKind.GreaterThanEqualToken;
                    Consume(2);
                    break;
                case '>':
                    kind = SyntaxKind.GreaterThanToken;
                    Consume();
                    break;

                case '(':
                    kind = SyntaxKind.OpenParenToken;
                    Consume();
                    break;
                case ')':
                    kind = SyntaxKind.CloseParenToken;
                    Consume();
                    break;

                case '{':
                    kind = SyntaxKind.OpenBraceToken;
                    Consume();
                    break;
                case '}':
                    kind = SyntaxKind.CloseBraceToken;
                    Consume();
                    break;

                case ':':
                    kind = SyntaxKind.ColonToken;
                    Consume();
                    break;

                case ';':
                    kind = SyntaxKind.SemiColonToken;
                    Consume();
                    break;

                case ',':
                    kind = SyntaxKind.CommaToken;
                    Consume();

                    break;
                case '&' when NextCharacter == '=':
                    kind = SyntaxKind.AmpersandEqualToken;
                    Consume(2);
                    break;
                case '&' when NextCharacter == '&':
                    kind = SyntaxKind.LogicalAndToken;
                    Consume(2);
                    break;
                case '&':
                    kind = SyntaxKind.AmpersandToken;
                    Consume();
                    break;

                case '|' when NextCharacter == '=':
                    kind = SyntaxKind.PipeEqualToken;
                    Consume(2);
                    break;
                case '|' when NextCharacter == '|':
                    kind = SyntaxKind.LogicalOrToken;
                    Consume(2);
                    break;
                case '|':
                    kind = SyntaxKind.PipeToken;
                    Consume();
                    break;

                case '!' when NextCharacter == '=':
                    kind = SyntaxKind.NotEqualToken;
                    Consume(2);
                    break;
                case '!':
                    kind = SyntaxKind.ExclamationToken;
                    Consume();
                    break;

                case '?':
                    kind = SyntaxKind.QuestionToken;
                    Consume();
                    break;

                case '.':
                    kind = SyntaxKind.DotToken;
                    Consume();
                    break;

                case '~' when NextCharacter == '=':
                    kind = SyntaxKind.TildeEqualToken;
                    Consume(2);
                    break;
                case '~':
                    kind = SyntaxKind.TildeToken;
                    Consume();
                    break;

                case '^' when NextCharacter == '=':
                    kind = SyntaxKind.CaretEqualToken;
                    Consume(2);
                    break;
                case '^':
                    kind = SyntaxKind.CaretToken;
                    Consume();
                    break;

                case '[':
                    kind = SyntaxKind.OpenBracketToken;
                    Consume();
                    break;
                case ']':
                    kind = SyntaxKind.CloseBracketToken;
                    Consume();
                    break;

                case '\'':
                    ReadCharacterLiteral(out kind, out value);
                    break;

                case '"':
                    ReadString(out kind, out value);
                    break;

                case '0' when NextCharacter.IsOctalDigit():
                    ReadOctalLiteral(out kind, out value);
                    break;

                case '0' when NextCharacter == 'x' || NextCharacter == 'X':
                    ReadHexLiteral(out kind, out value);
                    break;

                case SP: case TAB:
                case LF: case CR:
                    ReadWhiteSpace(out kind);
                    break;

                default:
                    if (char.IsDigit(CurrentCharacter))
                    {
                        ReadIntegerLiteral(out kind, out value);
                    }
                    else if (char.IsLetter(CurrentCharacter) || CurrentCharacter == '_')
                    {
                        ReadIdentifierOrKeyword(out kind, out value);
                    }
                    else if (char.IsWhiteSpace(CurrentCharacter))
                    {
                        ReadWhiteSpace(out kind);
                    }
                    else
                    {
                        // TODO: report bad token error
                        Consume();
                    }
                    break;
            }

            string text = _text.Substring(start, CurrentPosition - start);

            return new SyntaxToken(kind, start, text, value);
        }

        private void ReadCharacterLiteral(out SyntaxKind kind, out object value)
        {
            char[] simpleEscapeCharacters = {'0', '\\', '"', '\'', 'n', 'r', 'b', 't', 'f', 'a', 'v', '?'};
            var charLiteralValue = new StringBuilder();

            if (CurrentCharacter != '\'')
            {
                throw new LexerException();
            }

            // Skip the first quote
            Consume();

            if (CurrentCharacter == '\'') // Empty character literal (invalid)
            {
                // TODO: error empty character literal is invalid
            }
            else if (CurrentCharacter == '\\') // Escape character
            {
                charLiteralValue.Append(CurrentCharacter);
                Consume();

                if (CurrentCharacter == 'x') // Hex sequence
                {
                    Consume();

                    // Consume up-to 8 hex digits
                    for (int i = 0; i < 8 && CurrentCharacter.IsHexDigit(); i++)
                    {
                        charLiteralValue.Append(Consume());
                    }
                }
                else if (CurrentCharacter.IsOctalDigit()) // Octal sequence
                {
                    // Consume exactly 3 octal digits
                    for (int i = 0; i < 3; i++)
                    {
                        if (!CurrentCharacter.IsOctalDigit())
                        {
                            // TODO: invalid octal sequence
                            break;
                        }
                        charLiteralValue.Append(Consume());
                    }
                }
                else if (simpleEscapeCharacters.Contains(CurrentCharacter))
                {
                    charLiteralValue.Append(Consume());
                }
            }
            else // Normal character
            {
                charLiteralValue.Append(Consume());
            }

            if (CurrentCharacter == '\'') // Closing quote
            {
                Consume();
            }
            else
            {
                // TODO: error unterminated character literal
            }

            kind = SyntaxKind.CharacterLiteral;
            value = char.Parse(charLiteralValue.ToString());
        }

        private void ReadString(out SyntaxKind kind, out object value)
        {
            if (CurrentCharacter != '"')
            {
                throw new LexerException();
            }

            // Skip the first quote
            Consume();

            var sb = new StringBuilder();
            var done = false;

            while (!done)
            {
                switch (CurrentCharacter)
                {
                    case NUL:
                    case CR:
                    case LF:
                        // TODO: report unterminated string
                        done = true;
                        break;
                    case '"':
                        // TODO: support escaping " with \"
                        Consume();
                        done = true;
                        break;
                    default:
                        sb.Append(Consume());
                        break;
                }
            }

            kind = SyntaxKind.StringToken;
            value = sb.ToString();
        }

        private void ReadOctalLiteral(out SyntaxKind kind, out object value)
        {
            if (!CurrentCharacter.IsOctalDigit())
            {
                throw new LexerException();
            }

            int start = CurrentPosition;

            while (CurrentCharacter.IsOctalDigit())
            {
                Consume();
            }

            string text = _text.Substring(start, CurrentPosition - start);
            int octValue = Convert.ToInt32(text, 8);

            value = octValue;
            kind = SyntaxKind.OctalLiteralToken;
        }

        private void ReadIntegerLiteral(out SyntaxKind kind, out object value)
        {
            if (!CurrentCharacter.IsDecimalDigit())
            {
                throw new LexerException();
            }

            int start = CurrentPosition;

            while (CurrentCharacter.IsDecimalDigit())
            {
                Consume();
            }

            string text = _text.Substring(start, CurrentPosition - start);
            int decValue = Convert.ToInt32(text, 10);

            value = decValue;
            kind = SyntaxKind.IntegerLiteralToken;
        }

        private void ReadHexLiteral(out SyntaxKind kind, out object value)
        {
            if (CurrentCharacter != '0' && (NextCharacter != 'x' || NextCharacter != 'X'))
            {
                throw new LexerException();
            }

            int start = CurrentPosition;

            // Skip 0x prefix
            Consume(2);

            while (CurrentCharacter.IsHexDigit())
            {
                Consume();
            }

            string text = _text.Substring(start, CurrentPosition - start);
            int hexValue = Convert.ToInt32(text, 16);

            value = hexValue;
            kind = SyntaxKind.HexLiteralToken;
        }

        private void ReadWhiteSpace(out SyntaxKind kind)
        {
            if (!char.IsWhiteSpace(CurrentCharacter))
            {
                throw new LexerException();
            }

            while (char.IsWhiteSpace(CurrentCharacter))
            {
                Consume();
            }

            kind = SyntaxKind.WhitespaceToken;
        }

        private void ReadIdentifierOrKeyword(out SyntaxKind kind, out object value)
        {
            if (!char.IsLetter(CurrentCharacter) && CurrentCharacter != '_')
            {
                throw new LexerException();
            }

            int start = CurrentPosition;

            while (char.IsLetterOrDigit(CurrentCharacter) || CurrentCharacter == '_')
            {
                Consume();
            }

            string text = _text.Substring(start, CurrentPosition - start);

            if (Keyword.IsKeyword(text, out Keyword keyword))
            {
                value = keyword;
                kind = SyntaxKind.KeywordToken;
            }
            else
            {
                value = text;
                kind = SyntaxKind.IdentifierToken;
            }
        }
    }
}
