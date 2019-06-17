namespace PinvokeGen.Native.Syntax
{
    public class Diagnostic
    {
        public Diagnostic(int position, string message, string text = null)
        {
            Position = position;
            Message = message;
            Text = text;
        }

        public int Position { get; }
        public string Message { get; }
        public string Text { get; }

        public override string ToString()
        {
            return Text is null
                ? $"[Error] {Position}: {Message}"
                : $"[Error] {Position}: {Message} \"{Text}\"";
        }
    }
}
