namespace GfnCompiler
{
    internal static class Language
    {
        public enum SpecialCharacter
        {
            // Assignment.
            AssignEquals,
            // Separators.
            SemiColon,
            Comma
        }

        public static System.Collections.Generic.IList<string> dataTypes =
            new System.Collections.Generic.List<string>()
        {
                "integer", // 32-bit only.
                "float",   // No support for doubles.
                "string",  // Basic string.
                "boolean"  // Basic boolean.
        };

        public static System.Collections.Generic.Dictionary<char, Language.SpecialCharacter> specialCharacters =
            new System.Collections.Generic.Dictionary<char, SpecialCharacter>()
        {
                // Assignment.
                { '=', Language.SpecialCharacter.AssignEquals },
                // Separators.
                { ';', Language.SpecialCharacter.SemiColon },
                { ',', Language.SpecialCharacter.Comma }
        };
    }
}