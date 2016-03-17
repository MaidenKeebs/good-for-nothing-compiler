using System.Collections.Generic;
using System.IO;

namespace GfnCompiler
{
    internal sealed class GfnDebug
    {
        internal static void PrintScannerResult(IList<TokenData> tokens)
        {
            using (StreamWriter file = new StreamWriter(@"./Output/Scanner_Result.txt"))
            {
                foreach (TokenData token in tokens)
                {
                    file.WriteLine(string.Format("Data: {0,24} | Line Number: {1,8} | Char Position: {2,8}",
                        token.data.ToString(), token.lineNumber, token.charPosition));
                }
            }
        }
    }
}
