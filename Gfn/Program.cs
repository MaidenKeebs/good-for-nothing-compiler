// The original code was originally Copyright © Microsoft Corporation.  All rights reserved.
// The original code was published with an article at https://msdn.microsoft.com/en-us/magazine/cc136756.aspx by Joel Pobar.
// The original terms were specified at http://www.microsoft.com/info/cpyright.htm but that page is long dead :)
using System;
using System.IO;

namespace GfnCompiler
{
    internal static class Program
    {
        private static void Main(string[] arguments)
        {
            if (arguments.Length == 0)
            {
                DisplayDefaultCompilerOutput();
                return;
            }

            foreach (string argument in arguments)
            {
                Compile(argument);
            }

            DisplayPostCompilationOutput();
        }

        private static void DisplayDefaultCompilerOutput()
        {
            Console.WriteLine("Gfn Compiler v1.0.0.0");
            Console.WriteLine("Usage: gfn.exe filename.gfn");
        }

        private static void Compile(string inputSourceFileName)
        {
            Scanner scanner = null;

            using (TextReader inputSourceFile = File.OpenText(inputSourceFileName))
            {
                scanner = new Scanner(inputSourceFile);
            }

            Parser parser = new Parser(scanner.Tokens);
        }

        private static void DisplayPostCompilationOutput()
        {
            Console.WriteLine("Compilation Successful!");
            Console.WriteLine("Press ANY key to continue.");
            Console.ReadLine();
        }
    }
}