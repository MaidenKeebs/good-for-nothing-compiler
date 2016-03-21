using System;

namespace GfnCompiler
{
    // This is just a placeholder for user-defined functions.
    // Just making sure the user-defined function call syntax works.
    // @<identifier>(<parameter>*[,<parameter>]*);
    public sealed class NoModule
    {
        public static void NoModulePrint(string text)
        {
            System.Console.WriteLine(System.String.Format("NMP -> {0}", text));
        }
    }

    // Namespace purely for the stdlib.
    namespace GfnStdLib
    {
        public sealed class IO
        {
            public static void PrintLine(string text)
            {
                Console.WriteLine(text);
            }

            public static void PrintLine(int text)
            {
                Console.WriteLine(text);
            }

            public static void PrintLine(bool text)
            {
                Console.WriteLine(text);
            }
        }
    }
}
