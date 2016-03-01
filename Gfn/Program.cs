// The original code was originally Copyright © Microsoft Corporation.  All rights reserved.
// The original code was published with an article at https://msdn.microsoft.com/en-us/magazine/cc136756.aspx by Joel Pobar.
// The original terms were specified at http://www.microsoft.com/info/cpyright.htm but that page is long dead :)

using System;
using System.IO;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: gfn.exe program.gfn");
            return;
        }

        // For each argument, check if it's a file or folder,
        // and handle accordingly.
        foreach (string arg in args)
        {
            FileAttributes fileAttr = File.GetAttributes(arg);

            if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                CompileByDirectory(arg);
            }
            else
            {
                CompileByFile(arg);
            }
        }
    }

    // Standard compilation of a single source file.
    private static void CompileByFile(string path)
    {
        var moduleName = Path.GetFileNameWithoutExtension(path) + ".exe";

        Scanner scanner;
        using (TextReader input = File.OpenText(path))
        {
            scanner = new Scanner(input);
        }
        var parser = new Parser(scanner.Tokens);
        parser.Parse();

        var codeGen = new CodeGen(parser.Result, moduleName);
        codeGen.Compile();
        Console.WriteLine("Successfully compiled to " + moduleName);
    }

    // A new compilation method, all project sources packaged into
    // a single folder, then bulk-compiled.
    private static void CompileByDirectory(string path)
    {
    }
}