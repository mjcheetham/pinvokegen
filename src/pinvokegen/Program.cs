using System;
using System.IO;
using PinvokeGen.Native.Syntax;

namespace PinvokeGen.Cli
{
    public static class Program
    {
        private static bool _ignoreWhitespace = false;

        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage(Console.Error);
                Environment.Exit(-1);
            }

            string source = null;

            if (IsHelp(args[0]))
            {
                PrintUsage(Console.Out);
                Environment.Exit(0);
            }
            else if (args[0] == "-")
            {
                source = Console.In.ReadToEnd();
            }
            else if (File.Exists(args[0]))
            {
                using (var fileReader = new StreamReader(args[0]))
                {
                    source = fileReader.ReadToEnd();
                }
            }
            else
            {
                Console.Error.WriteLine($"error: file '{args[0]}' does not exist");
                PrintUsage(Console.Error);
                Environment.Exit(-1);
            }

            if (args.Length > 1 && StringComparer.OrdinalIgnoreCase.Equals(args[1], "--no-whitespace"))
            {
                _ignoreWhitespace = true;
            }

            var lexer = new Lexer(source);

            SyntaxToken token;
            do
            {
                token = lexer.Lex();

                PrintToken(Console.Out, token);

            } while (token.Kind != SyntaxKind.EndOfFile);

            Environment.Exit(0);
        }

        private static bool IsHelp(string str)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(str, "--help") ||
                   StringComparer.OrdinalIgnoreCase.Equals(str, "-?") ||
                   StringComparer.OrdinalIgnoreCase.Equals(str, "/?") ||
                   StringComparer.OrdinalIgnoreCase.Equals(str, "-h") ||
                   StringComparer.OrdinalIgnoreCase.Equals(str, "/h");
        }

        private static void PrintUsage(TextWriter writer)
        {
            const string appName = "pinvokegen";

            writer.WriteLine("usage: {0} <file> [--no-whitespace]", appName);
            writer.WriteLine();
            writer.WriteLine("  file\t\t\tPath to C header file (.h) or \"-\" to read from standard input.");
            writer.WriteLine("  --no-whitespace\tDo not print whitespace tokens.");
            writer.WriteLine();
        }

        private static void PrintToken(TextWriter writer, SyntaxToken token)
        {
            if (_ignoreWhitespace && token.Kind == SyntaxKind.WhitespaceToken)
            {
                return;
            }

            if (token.Value is null)
            {
                writer.WriteLine($"{token.Position}: {token.Kind} \"{token.Text}\"");
            }
            else
            {
                writer.WriteLine($"{token.Position}: {token.Kind} \"{token.Text}\" ({token.Value})");
            }
        }
    }
}
