/***
 * Message from MaidenKeebs:
 *
 * Hey fellow programmer! I felt like implementing a somewhat minimal
 * standard library of simple IO functions. Feel free to add or change.
 ***/
namespace GfnCompiler
{
    public sealed class NoModule
    {
        public static void NoModulePrint(string text)
        {
            System.Console.WriteLine("NMP -> {0}", text);
        }
    }

    // Namespace purely for the stdlib.
    namespace GfnStdLib
    {
        public sealed class IO
        {
            public static void Print(string text)
            {
                System.Console.WriteLine(System.String.Format("IO:Print -> {0}", text));
            }

            public static void PrintThree(string text1, string text2, string text3)
            {
                System.Console.WriteLine("{0}, {1}, {2}", text1, text2, text3);
            }

            public static void CountToTen()
            {
                for (int i = 0; i < 10; ++i)
                {
                    System.Console.WriteLine((i+1).ToString());
                }
            }
        }
    }
}
