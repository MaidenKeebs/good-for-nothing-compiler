using System;
using System.Collections.Generic;

namespace GfnCompiler
{
    internal sealed class Language
    {
        public enum SpecialCharacter
        {
            Equals,
            SemiColon
        }

        public static Dictionary<char, SpecialCharacter> SpecialCharacters = new Dictionary<char, SpecialCharacter>()
        {
            { '=', SpecialCharacter.Equals },
            { ';', SpecialCharacter.SemiColon }
        };

        public static Dictionary<string, Type> dataTypes = new Dictionary<string, Type>()
        {
            { "integer", typeof(int) }
        };
    }
}