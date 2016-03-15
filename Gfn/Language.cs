using System.Collections.Generic;

namespace GfnCompiler
{
    internal sealed class Language
    {
        public enum SpecialCharacter
        {
            Equals,
            SemiColon
        };

        public static IDictionary<char, SpecialCharacter> specialCharacters = new Dictionary<char, SpecialCharacter>()
        {
            { '=', SpecialCharacter.Equals },
            { ';', SpecialCharacter.SemiColon }
        };
    }
}