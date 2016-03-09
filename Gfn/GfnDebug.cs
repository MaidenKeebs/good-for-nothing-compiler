namespace GfnCompiler
{
    public sealed class GfnDebug
    {
        public enum Origin
        {
            GFN_SCANNER,
            GFN_PARSER,
            GFN_CODEGENERATOR
        }

        // Just a replacement for all those System.Exception(...) calls.
        public static void ThrowException(string origin, string exceptionText, params object[] args)
        {
            throw new System.Exception(System.String.Format(origin + "::ERROR -> " + exceptionText, args));
        }

        // Just used for checking scanner, parser, code generator results.
        public static void Print(string origin, string text, params object[] args)
        {
            System.Console.WriteLine(origin + "::INFO -> " + text, args);
        }
    }
}
