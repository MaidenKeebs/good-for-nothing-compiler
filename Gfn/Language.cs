namespace GfnCompiler
{
    internal static class Language
    {
        public const char SINGLE_LINE_COMMENT = '#';

        public enum SpecialCharacter
        {
            // Assignment.
            AssignEquals,
            // Separators.
            Colon,
            SemiColon,
            Comma,
            UnderScore,
            // Function call prefix.
            FunctionPrefix,
            // Braces.
            LeftParenthesis,
            RightParenthesis,
            LeftBrace,
            RightBrace
        }

        public static System.Collections.Generic.Dictionary<string, string> dataTypes =
            new System.Collections.Generic.Dictionary<string, string>()
        {
                { "INT_32", "Integer" },
                { "STRING", "String" },
                { "BOOLEAN", "Boolean" }
        };

        public static System.Collections.Generic.Dictionary<char, Language.SpecialCharacter> specialCharacters =
            new System.Collections.Generic.Dictionary<char, SpecialCharacter>()
        {
                // Assignment.
                { '=', Language.SpecialCharacter.AssignEquals },
                // Separators.
                { ':', Language.SpecialCharacter.Colon },
                { ';', Language.SpecialCharacter.SemiColon },
                { ',', Language.SpecialCharacter.Comma },
                { '_', Language.SpecialCharacter.UnderScore },
                // Function call prefix.
                { '@', Language.SpecialCharacter.FunctionPrefix },
                // Braces.
                { '(', Language.SpecialCharacter.LeftParenthesis },
                { ')', Language.SpecialCharacter.RightParenthesis },
                { '{', Language.SpecialCharacter.LeftBrace },
                { '}', Language.SpecialCharacter.RightBrace }
        };
    }
}