namespace PinvokeGen.Native.Syntax
{
    public class SyntaxToken
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public SyntaxKind Kind { get; }

        public int Position { get; }

        public string Text { get; }

        public object Value { get; }

        public override string ToString()
        {
            return Value is null
                ? $"{Position}: {Kind} \"{Text}\""
                : $"{Position}: {Kind} \"{Text}\" ({Value})";
        }
    }
}
