using System.Collections.Generic;
using System.IO;
using System.Text;
using PinvokeGen.Native.Syntax;
using Xunit;

namespace PinvokeGen.Tests
{
    public class LexerTests
    {
        [Fact]
        public void LexFile()
        {
            string source = File.ReadAllText("/tmp/test.h");

            string s = RunLex(source);
        }

        [Fact]
        public void Lex1()
        {
            const string source = @"
int lim = 100;
int x1 = 0;
int x2 = 1;

do {
    int t = x1;
    x1 = x2;
    x2 = t + x1;
} while (x2 < lim);

printf(""max fib(n) < %i = %i\n"", lim, x2);
";

            string s = RunLex(source);
        }

        [Fact]
        public void Lex2()
        {
            const string source = @"
char c2 = 'A';
char c1 = '\0';
char c1 = '\""';
char c1 = '\a';
char c1 = '\A';
char c2 = 'A';
char c2 = '\212';
char c2 = '\12';
char c2 = '\777';
char c2 = '\790';
char c2 = '\01';
char c2 = '\xFF';
char c2 = '\xDEADBEEF';
char c2 = '\xABCDZYXW';";

            string s = RunLex(source);
        }

        private static string RunLex(string source)
        {
            var lexer = new Lexer(source);
            var tokens = new List<SyntaxToken>();

            SyntaxToken token;
            do
            {
                token = lexer.Lex();
                tokens.Add(token);
            } while (token.Kind != SyntaxKind.EndOfFile);

            return CreateTokenSummary(tokens);
        }

        private static string CreateTokenSummary(IEnumerable<SyntaxToken> tokens)
        {
            var sb = new StringBuilder();
            foreach (SyntaxToken token in tokens)
            {
                sb.AppendLine(token.Value is null
                    ? $"{token.Position}: {token.Kind} \"{token.Text}\""
                    : $"{token.Position}: {token.Kind} \"{token.Text}\" ({token.Value})");
            }
            return sb.ToString();
        }
    }
}
