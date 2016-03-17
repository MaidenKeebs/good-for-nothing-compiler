/***
 * Message from MaidenKeebs:
 *
 * Hey fellow programmer! I felt like implementing a somewhat minimal
 * standard library of simple IO functions. Feel free to add or change.
 ***/
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
        namespace Objects
        {
            public class Player
            {
                public Player(string name)
                {
                    System.Console.WriteLine("Welcome to your new adventure, {0}!", name);
                }
            }
        }

        public sealed class IO
        {
            public static void Print(string text)
            {
                System.Console.WriteLine(System.String.Format("IO:Print -> {0}", text));
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
