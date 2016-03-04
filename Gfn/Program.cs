// The original code was originally Copyright © Microsoft Corporation.  All rights reserved.
// The original code was published with an article at https://msdn.microsoft.com/en-us/magazine/cc136756.aspx by Joel Pobar.
// The original terms were specified at http://www.microsoft.com/info/cpyright.htm but that page is long dead :)

namespace GfnCompiler
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.WriteLine("Command Line Usage: gfn.exe <file_name.gfn | directory_name>");
                return;
            }

            // For each argument, check if it's a file or folder,
            // and handle accordingly.
            foreach (string arg in args)
            {
                System.IO.FileAttributes fileAttr = System.IO.File.GetAttributes(arg);

                if ((fileAttr & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
                {
                    CompileByDirectory(arg);
                }
                else
                {
                    // Not necessarily a file.
                    CompileByFile(arg);
                }
            }

            // End-of-compilation output.
            System.Console.WriteLine("Press ANY key to continue.");
            System.Console.ReadLine();
        }

        // Standard compilation of a single source file.
        private static void CompileByFile(string path)
        {
            // Store the soon-to-be .exe file name.
            var moduleName = System.IO.Path.GetFileNameWithoutExtension(path) + ".exe";

            Scanner scanner;

            using (System.IO.TextReader inputSourceFile = System.IO.File.OpenText(path))
            {
                scanner = new Scanner(inputSourceFile);
            }

            Parser parser = new Parser(scanner.GetTokens());
            parser.Parse();

            CodeGenerator codeGenerator = new CodeGenerator(parser.GetResult(), moduleName);
            codeGenerator.Compile();

            // Compilation done. Let 'em know.
            System.Console.WriteLine("Compiled source to: " + moduleName);
        }

        // A new compilation method, all project sources packaged into
        // a single folder, then bulk-compiled.
        private static void CompileByDirectory(string path)
        {
            // 1) Locate & Read "Build.txt" file.
            // 3) Compile each source file one by one starting with "Main.gfn"
        }
    }
}