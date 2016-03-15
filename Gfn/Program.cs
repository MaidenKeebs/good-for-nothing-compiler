// The original code was originally Copyright © Microsoft Corporation.  All rights reserved.
// The original code was published with an article at https://msdn.microsoft.com/en-us/magazine/cc136756.aspx by Joel Pobar.
// The original terms were specified at http://www.microsoft.com/info/cpyright.htm but that page is long dead :)
using System;
using System.Collections.Generic;
using System.IO;

namespace GfnCompiler
{
    internal static class Program
    {
        public const bool GFN_DEBUG = true;

        private static void Main(string[] commandLineArguments)
        {
            IList<string> filesToCompile = new List<string>();
            IList<string> unknownArguments = new List<string>();

            // Separate arguments in the lists above.
            foreach (string argument in commandLineArguments)
            {
                if (File.Exists(argument) && argument.EndsWith(".gfn"))
                {
                    filesToCompile.Add(argument);
                }
                else
                {
                    unknownArguments.Add(argument);
                }
            }

            if (GFN_DEBUG)
            {
                DisplayArguments("Files:", filesToCompile);
                DisplayArguments("Unknown:", unknownArguments);
            }

            CompileFiles(filesToCompile);
        }

        private static void CompileFiles(IList<string> files)
        {
            foreach (string file in files)
            {
                Scanner scanner = null;
                using (TextReader sourceFile = File.OpenText(file))
                {
                    scanner = new Scanner(sourceFile);
                }
            }
        }

        private static void DisplayArguments(string headerText, IList<string> arguments)
        {
            if (arguments.Count == 0)
            {
                return;
            }

            Console.WriteLine(headerText);

            foreach (string argument in arguments)
            {
                Console.WriteLine(argument);
            }
        }
    }
}