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
                DisplayDefaultCompilerText();

                return;
            }

            CompileSourcesFromArguments(arguments);

            DisplayPostCompilationText();
        }

        private static void CompileSourcesFromArguments(string[] arguments)
        {
            foreach (string argument in arguments)
            {
                if (File.Exists(argument))
                {
                    CompileByFile(argument);
                }
            }
        }

        private static void CompileByFile(string path)
        {
            var moduleName = System.IO.Path.GetFileNameWithoutExtension(path) + ".exe";

            Scanner scanner;

            using (TextReader inputSourceFile = System.IO.File.OpenText(path))
            {
                scanner = new Scanner(inputSourceFile);
            }

            Parser parser = new Parser(scanner.GetTokens());
            parser.Parse();

            CodeGenerator codeGenerator = new CodeGenerator(parser.GetResult(), moduleName);
            codeGenerator.Compile();
        }

        private static void DisplayDefaultCompilerText()
        {
            Console.WriteLine("Gfn Command Line Usage: gfn.exe <file_name.gfn>");
        }

        private static void DisplayPostCompilationText()
        {
            Console.WriteLine("Compilation Complete!");
        }
    }
}