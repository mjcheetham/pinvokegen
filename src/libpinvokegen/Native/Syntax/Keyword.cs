using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace PinvokeGen.Native.Syntax
{
    [DebuggerDisplay("Keyword {Text}")]
    public class Keyword
    {
        public static readonly Keyword Break    = new Keyword("break");
        public static readonly Keyword Case     = new Keyword("case");
        public static readonly Keyword Char     = new Keyword("char");
        public static readonly Keyword Const    = new Keyword("const");
        public static readonly Keyword Continue = new Keyword("continue");
        public static readonly Keyword Default  = new Keyword("default");
        public static readonly Keyword Do       = new Keyword("do");
        public static readonly Keyword Double   = new Keyword("double");
        public static readonly Keyword Else     = new Keyword("else");
        public static readonly Keyword Enum     = new Keyword("enum");
        public static readonly Keyword Extern   = new Keyword("extern");
        public static readonly Keyword Float    = new Keyword("float");
        public static readonly Keyword For      = new Keyword("for");
        public static readonly Keyword Goto     = new Keyword("goto");
        public static readonly Keyword If       = new Keyword("if");
        public static readonly Keyword Inline   = new Keyword("inline");
        public static readonly Keyword Int      = new Keyword("int");
        public static readonly Keyword Long     = new Keyword("long");
        public static readonly Keyword Register = new Keyword("register");
        public static readonly Keyword Restrict = new Keyword("restrict");
        public static readonly Keyword Return   = new Keyword("return");
        public static readonly Keyword Short    = new Keyword("short");
        public static readonly Keyword Signed   = new Keyword("signed");
        public static readonly Keyword Sizeof   = new Keyword("sizeof");
        public static readonly Keyword Static   = new Keyword("static");
        public static readonly Keyword Struct   = new Keyword("struct");
        public static readonly Keyword Switch   = new Keyword("switch");
        public static readonly Keyword Typedef  = new Keyword("typedef");
        public static readonly Keyword Union    = new Keyword("union");
        public static readonly Keyword Unsigned = new Keyword("unsigned");
        public static readonly Keyword Void     = new Keyword("void");
        public static readonly Keyword Volatile = new Keyword("volatile");
        public static readonly Keyword While    = new Keyword("while");

        private static readonly Dictionary<string, Keyword> KeywordMap = BuildKeywordMap();

        private static Dictionary<string, Keyword> BuildKeywordMap()
        {
            var keywordType = typeof(Keyword);
            IEnumerable<Keyword> keywords = keywordType
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.FieldType == keywordType)
                .Select(x => x.GetValue(null))
                .Cast<Keyword>();
            return keywords.ToDictionary(x => x.Text, x => x);
        }

        private Keyword(string text)
        {
            Text = text;
        }

        public string Text { get; }

        public static bool IsKeyword(string str, out Keyword keyword) => KeywordMap.TryGetValue(str, out keyword);

        public override string ToString()
        {
            return Text;
        }
    }
}
