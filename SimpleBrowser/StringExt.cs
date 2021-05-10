namespace System
{
    public static class StringExt
    {
        public static bool IsNullOrWhiteSpace(this string text) {
            if (text == null) return true;

            if (text.IndexOf(' ') == -1)
                return string.IsNullOrEmpty(text);

            return string.IsNullOrEmpty(text.Trim());
        }
    }
}
