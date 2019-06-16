namespace PinvokeGen
{
    internal static class CharacterExtensions
    {
        public static bool IsOctalDigit(this char c)
        {
            return c >= '0' && c <= '7';
        }

        public static bool IsDecimalDigit(this char c)
        {
            return c >= '0' && c <= '9';
        }

        public static bool IsHexDigit(this char c)
        {
            return c >= '0' && c <= '9' ||
                   c >= 'a' && c <= 'f' ||
                   c >= 'A' && c <= 'F';
        }
    }
}
