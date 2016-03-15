using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GfnCompiler
{
    internal struct TokenData
    {
        public readonly int lineNumber;
        public readonly int charPosition;
        public readonly object data;

        public TokenData(int lineNumber, int charPosition, object data)
        {
            this.lineNumber = lineNumber;
            this.charPosition = charPosition;
            this.data = data;
        }
    }

    internal sealed class Scanner
    {
        private TextReader m_sourceFile;
        private readonly IList<TokenData> m_tokens;
        private char m_currentChar;
        private int m_lineNumber;
        private int m_charPosition;

        public Scanner(TextReader sourceFile)
        {
            m_sourceFile = sourceFile;
            m_tokens = new List<TokenData>();
            m_currentChar = '\0';
            m_lineNumber = 1;
            m_charPosition = 1;

            Scan();

            if (GfnCompiler.Program.GFN_DEBUG)
            {
                foreach (TokenData token in m_tokens)
                {
                    // TODO: MaidenKeebs
                    // Add decent formatting so all text is aligned for easier readability.
                    Console.WriteLine("Token: {0}\tLine: {1}\tPosition: {2}", token.data, token.lineNumber, token.charPosition);
                }
            }
        }

        private void Scan()
        {
            while (!EndOfFile())
            {
                PeekNextChar();

                if (System.Char.IsWhiteSpace(CurrentChar()))
                {
                    ScanWhiteSpace();
                }
                else if (System.Char.IsLetter(CurrentChar()))
                {
                    ScanKeywordOrIdentifier();
                }
                else if (System.Char.IsDigit(CurrentChar()))
                {
                    ScanNumericLiteral();
                }
                else
                {
                    ScanSpecialCharacter();
                }
            }
        }

        private void ScanWhiteSpace()
        {
            if (CurrentChar().Equals('\n'))
            {
                ReadToNextLine();
            }
            else
            {
                ReadNextChar();
            }
        }

        private void ScanKeywordOrIdentifier()
        {
            StringBuilder token = new StringBuilder();

            int lineNumber = m_lineNumber;
            int charPosition = m_charPosition;

            while (System.Char.IsLetter(CurrentChar()) || CurrentChar().Equals('_'))
            {
                token.Append(CurrentChar());
                ReadNextChar();

                if (EndOfFile())
                {
                    break;
                }

                PeekNextChar();
            }

            m_tokens.Add(new TokenData(lineNumber, charPosition, token.ToString()));
        }

        private void ScanNumericLiteral()
        {
            StringBuilder token = new StringBuilder();

            int lineNumber = m_lineNumber;
            int charPosition = m_charPosition;

            while (System.Char.IsDigit(CurrentChar()))
            {
                token.Append(CurrentChar());
                ReadNextChar();

                if (EndOfFile())
                {
                    break;
                }

                PeekNextChar();
            }

            m_tokens.Add(new TokenData(lineNumber, charPosition, Int32.Parse(token.ToString())));
        }

        private void ScanSpecialCharacter()
        {
            foreach (KeyValuePair<char, Language.SpecialCharacter> pair in Language.specialCharacters)
            {
                if (CurrentChar().Equals(pair.Key))
                {
                    m_tokens.Add(new TokenData(m_lineNumber, m_charPosition, pair.Value));

                    ReadNextChar();

                    return;
                } 
            }

            throw new Exception(String.Format("SCANNER_ERROR: Encountered unrecognisable token: {0}", CurrentChar()));
        }

        private bool EndOfFile()
        {
            return m_sourceFile.Peek() == -1;
        }

        private char CurrentChar()
        {
            return m_currentChar;
        }

        private void PeekNextChar()
        {
            m_currentChar = (char)m_sourceFile.Peek();
        }

        private void ReadNextChar()
        {
            m_sourceFile.Read();

            m_charPosition += 1;
        }

        private void ReadToNextLine()
        {
            m_sourceFile.ReadLine();

            m_lineNumber += 1;
            m_charPosition = 1;
        }
    }
}