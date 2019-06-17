using System;
using System.Collections.Generic;
using System.Text;

namespace PinvokeGen.Native.Syntax
{
    public class Lexer
    {
        private const char NULL   = '\0';
        private const char BSLASH = '\\';
        private const char DQT    = '\"';
        private const char LF     = '\n';
        private const char CR     = '\r';
        private const char BSPACE = '\b';
        private const char HTAB   = '\t';
        private const char FEED   = '\f';
        private const char BELL   = '\a';
        private const char VTAB   = '\v';
        private const char SPACE  = ' ';

        private static readonly IReadOnlyDictionary<string, char> SimpleEscapeCharMap = new Dictionary<string, char>
        {
            [@"\0"]  = NULL,
            [@"\\"]  = BSLASH,
            [@"\"""] = DQT,
            [@"\n"]  = LF,
            [@"\r"]  = CR,
            [@"\b"]  = BSPACE,
            [@"\t"]  = HTAB,
            [@"\f"]  = FEED,
            [@"\a"]  = BELL,
            [@"\v"]  = VTAB,
        };

        private readonly string _text;

        private int _position;

        public Lexer(string text)
        {
            _text = text;
        }

        public ICollection<Diagnostic> Diagnostics { get; } = new List<Diagnostic>();

        private int CurrentPosition => _position;

        private char CurrentCharacter => Peek(0);

        private char NextCharacter => Peek(1);

        private char Consume()
        {
            return _text[_position++];
        }

        private string Consume(int count)
        {
            var ret = _text.Substring(_position, count);
            _position += count;
            return ret;
        }

        private string ConsumeWhile(Predicate<char> predicate, int maxCount = int.MaxValue)
        {
            var arr = new char[maxCount];
            int i;
            for (i = 0; i < maxCount && predicate(CurrentCharacter); i++)
            {
                arr[i] = Consume();
            }
            return new string(arr, 0, i);
        }

        private char Peek(int offset)
        {
            var index = _position + offset;

            if (index >= _text.Length)
            {
                return NULL;
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
                case NULL:
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

                case SPACE: case HTAB:
                case LF:    case CR:
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
                        char invalidChar = Consume();
                        Diagnostics.Add(new Diagnostic(CurrentPosition, "Unknown token", invalidChar.ToString()));
                    }
                    break;
            }

            string text = _text.Substring(start, CurrentPosition - start);

            return new SyntaxToken(kind, start, text, value);
        }

        private void ReadCharacterLiteral(out SyntaxKind kind, out object value)
        {
            value = null;

            if (CurrentCharacter != '\'')
            {
                throw new LexerException();
            }

            // Skip the first quote
            Consume();

            if (CurrentCharacter == '\'') // Empty character literal (invalid)
            {
                Diagnostics.Add(new Diagnostic(CurrentPosition, "Empty character literal is invalid", "\'\'"));
                kind = SyntaxKind.InvalidToken;
                return;
            }

            if (CurrentCharacter == '\\') // Escape character
            {
                Consume();

                if (SimpleEscapeCharMap.TryGetValue($@"\{CurrentCharacter}", out char escapeChar) && NextCharacter == '\'')
                {
                    Consume();
                    value = escapeChar;
                }
                else if (CurrentCharacter == 'x') // Hex sequence
                {
                    Consume();

                    // Consume up-to 8 hex digits
                    string hexChars = ConsumeWhile(CharacterExtensions.IsHexDigit, maxCount: 8);

                    if (TryCreateCharacter(hexChars, 16, out char c))
                    {
                        value = c;
                    }
                    else
                    {
                        Diagnostics.Add(new Diagnostic(CurrentPosition, "Invalid hex escape character", hexChars));
                        kind = SyntaxKind.InvalidToken;
                        return;
                    }
                }
                else if (CurrentCharacter.IsOctalDigit()) // Octal sequence
                {
                    // Consume 3 octal digits
                    string octalChars = ConsumeWhile(CharacterExtensions.IsOctalDigit, maxCount: 3);

                    if (octalChars.Length != 3)
                    {
                        Diagnostics.Add(new Diagnostic(CurrentPosition, "Octal escape character must consist of 3 digits", octalChars));
                        kind = SyntaxKind.InvalidToken;
                        return;
                    }

                    if (TryCreateCharacter(octalChars, 8, out char c))
                    {
                        value = c;
                    }
                    else
                    {
                        Diagnostics.Add(new Diagnostic(CurrentPosition, "Invalid oct escape character", octalChars));
                        kind = SyntaxKind.InvalidToken;
                        return;
                    }
                }
                else
                {
                    char c = Consume();
                    Diagnostics.Add(new Diagnostic(CurrentPosition, "Unknown escape character", $"\\{c}"));
                    kind = SyntaxKind.InvalidToken;
                    return;
                }
            }
            else // Normal character
            {
                value = Consume();
            }

            char lastChar = Consume();
            if (lastChar == '\'') // Closing quote
            {
                kind = SyntaxKind.CharacterLiteral;
            }
            else
            {
                Diagnostics.Add(new Diagnostic(CurrentPosition, "Expected closing character quote", lastChar.ToString()));
                kind = SyntaxKind.InvalidToken;
            }
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
                    case NULL:
                    case CR:
                    case LF:
                        Diagnostics.Add(new Diagnostic(CurrentPosition, "Unterminated string literal; expected \""));
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

            string octalChars = ConsumeWhile(CharacterExtensions.IsOctalDigit);

            if (TryConvertInteger(octalChars, 8, out int i))
            {
                value = i;
                kind = SyntaxKind.OctalLiteralToken;
            }
            else
            {
                Diagnostics.Add(new Diagnostic(CurrentPosition, "Invalid octal literal", octalChars));
                value = default;
                kind = SyntaxKind.InvalidToken;
            }
        }

        private void ReadIntegerLiteral(out SyntaxKind kind, out object value)
        {
            if (!CurrentCharacter.IsDecimalDigit())
            {
                throw new LexerException();
            }

            string decChars = ConsumeWhile(CharacterExtensions.IsOctalDigit);

            if (TryConvertInteger(decChars, 10, out int i))
            {
                value = i;
                kind = SyntaxKind.IntegerLiteralToken;
            }
            else
            {
                Diagnostics.Add(new Diagnostic(CurrentPosition, "Invalid integer literal", decChars));
                value = default;
                kind = SyntaxKind.InvalidToken;
            }
        }

        private void ReadHexLiteral(out SyntaxKind kind, out object value)
        {
            if (CurrentCharacter != '0' && (NextCharacter != 'x' || NextCharacter != 'X'))
            {
                throw new LexerException();
            }

            // Skip 0x prefix
            Consume(2);

            string hexChars = ConsumeWhile(CharacterExtensions.IsHexDigit);

            if (TryConvertInteger(hexChars, 8, out int i))
            {
                value = i;
                kind = SyntaxKind.HexLiteralToken;
            }
            else
            {
                Diagnostics.Add(new Diagnostic(CurrentPosition, "Invalid hex literal", hexChars));
                value = default;
                kind = SyntaxKind.InvalidToken;
            }
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

            string text = ConsumeWhile(c => char.IsLetterOrDigit(c) || c == '_');

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

        private static bool TryCreateCharacter(string str, int fromBase, out char c)
        {
            try
            {
                int value = Convert.ToInt32(str, fromBase);
                return TryCreateCharacter(value, out c);
            }
            catch
            {
                c = default;
                return false;
            }
        }

        private static bool TryCreateCharacter(int value, out char c)
        {
            try
            {
                c = char.ConvertFromUtf32(value)[0];
                return true;
            }
            catch
            {
                c = default;
                return false;
            }
        }

        private static bool TryConvertInteger(string str, int fromBase, out int value)
        {
            try
            {
                value = Convert.ToInt32(str, fromBase);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }
    }
}
