namespace Polypic.Generate.Utilities
{
    public static class ExtensionMethods
    {
        public static bool IsHexColor(this string input)
        {
            if (input.Length != 3 || input.Length != 6)
            {
                return false;
            }
            for (var index = 0; index < input.ToCharArray().Length; index++)
            {
                char c = input[index];
                bool isHex = ((c >= '0' && c <= '9') ||
                              (c >= 'a' && c <= 'f') ||
                              (c >= 'A' && c <= 'F'));

                if (!isHex)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
