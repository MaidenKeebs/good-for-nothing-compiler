// TODO-MaidenKeebs:
//     Re-implement the char position code because it isn't
//     always accurate, sorry.

namespace GfnCompiler
{
    // Basic container for storing read-in tokens.
    public struct TokenData
    {
        public int lineNumber;
        public int charPosition;
        public object data;

        public TokenData(int lineNumber, int charPosition, object data)
        {
            this.lineNumber = lineNumber;
            this.charPosition = charPosition;
            this.data = data;
        }
    }

    public sealed class Scanner
    {
        private readonly System.Collections.Generic.IList<TokenData> m_tokens;
        private System.IO.TextReader m_inputSourceFile;
        private char m_currentFileChar;
        private int m_currentFileLineNumber;
        private int m_currentFileCharPosition;

        public Scanner(System.IO.TextReader inputSourceFile)
        {
            m_tokens = new System.Collections.Generic.List<TokenData>();
            m_inputSourceFile = inputSourceFile;
            m_currentFileLineNumber = 1;
            m_currentFileCharPosition = 0;

            Scan();

            foreach (TokenData token in m_tokens)
            {
                GfnDebug.Print(GfnDebug.Origin.GFN_SCANNER.ToString(), "Token: {0}", token.data.ToString());
            }
        }

        public System.Collections.Generic.IList<TokenData> GetTokens()
        {
            return m_tokens;
        }

        private int PeekNextToken()
        {
            m_currentFileChar = (char)m_inputSourceFile.Peek();
            return (int)m_currentFileChar;
        }

        private void ReadNextToken()
        {
            m_inputSourceFile.Read();
            m_currentFileCharPosition += 1;
        }

        private void ReadToNextLine()
        {
            m_inputSourceFile.ReadLine();
            m_currentFileLineNumber += 1;
            m_currentFileCharPosition = 0;
        }

        private char CurrentToken()
        {
            return m_currentFileChar;
        }

        private bool EndOfFile()
        {
            return m_inputSourceFile.Peek() == -1;
        }

        private void Scan()
        {
            while (m_inputSourceFile.Peek() != -1)
            {
                PeekNextToken();

                if (System.Char.IsWhiteSpace(CurrentToken()))
                {
                    ScanWhiteSpace();
                }
                else if (System.Char.IsLetter(CurrentToken()))
                {
                    ScanKeywordOrIdentifier();
                }
                else if (System.Char.IsDigit(CurrentToken()))
                {
                    ScanNumber();
                }
                else if (CurrentToken().Equals('"'))
                {
                    ScanStringLiteral();
                }
                else if (CurrentToken().Equals(Language.SINGLE_LINE_COMMENT))
                {
                    ReadToNextLine();
                }
                else
                {
                    ScanSpecialCharacter();
                }
            }
        }

        private void ScanWhiteSpace()
        {
            if (CurrentToken().Equals('\n'))
            {
                m_currentFileLineNumber += 1;
                m_currentFileCharPosition = 0;
            }

            m_inputSourceFile.Read();
        }

        private void ScanKeywordOrIdentifier()
        {
            System.Text.StringBuilder token = new System.Text.StringBuilder();

            int lineNumber = m_currentFileLineNumber;
            int charPosition = m_currentFileCharPosition;

            while (System.Char.IsLetter(CurrentToken()) || System.Char.IsDigit(CurrentToken()) || CurrentToken().Equals('_'))
            {
                token.Append(CurrentToken());
                ReadNextToken();

                if (EndOfFile())
                {
                    break;
                }

                PeekNextToken();
            }

            m_tokens.Add(new TokenData(lineNumber, charPosition, token.ToString()));
        }

        private void ScanNumber()
        {
            System.Text.StringBuilder token = new System.Text.StringBuilder();

            int lineNumber = m_currentFileLineNumber;
            int charPosition = m_currentFileCharPosition;

            while (System.Char.IsDigit(CurrentToken()))
            {
                token.Append(CurrentToken());
                ReadNextToken();

                if (EndOfFile())
                {
                    break;
                }

                PeekNextToken();
            }

            m_tokens.Add(new TokenData(lineNumber, charPosition, System.Int32.Parse(token.ToString())));
        }

        private void ScanStringLiteral()
        {
            System.Text.StringBuilder token = new System.Text.StringBuilder();

            int lineNumber = m_currentFileLineNumber;
            int charPosition = m_currentFileCharPosition;

            // Skip the initial '"'.
            ReadNextToken();

            if (EndOfFile())
            {
                throw new System.Exception("Unterminated string literal due to end-of-file. (A)");
            }

            while (PeekNextToken() != '"')
            {
                token.Append(CurrentToken());
                ReadNextToken();

                if (EndOfFile())
                {
                    throw new System.Exception("Unterminated string literal due to end-of-file.(B)");
                }
            }

            // Skip the second '"'.
            ReadNextToken();

            m_tokens.Add(new TokenData(lineNumber, charPosition, token));
        }

        private void ScanSpecialCharacter()
        {
            bool foundMatch = false;

            foreach (System.Collections.Generic.KeyValuePair<char, Language.SpecialCharacter> pair in Language.specialCharacters)
            {
                if (CurrentToken().Equals(pair.Key))
                {
                    foundMatch = true;

                    m_tokens.Add(new TokenData(m_currentFileLineNumber, m_currentFileCharPosition, pair.Value));
                    ReadNextToken();
                }
            }

            if (!foundMatch)
            {
                GfnDebug.ThrowException(GfnDebug.Origin.GFN_SCANNER.ToString(),
                    "Encountered an unknown token on line '{0}' at position '{1}'.", m_currentFileLineNumber, m_currentFileCharPosition);
            }
        }
    }
}