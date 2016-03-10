using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GfnCompiler
{
    internal struct TokenData
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

    internal sealed class Scanner
    {
        private readonly IList<TokenData> m_tokens;
        private TextReader m_inputFile;
        private char m_currentChar;
        private int m_currentLineNumber;
        private int m_currentCharPosition;

        public IList<TokenData> Tokens
        {
            get
            {
                return m_tokens;
            }
        }

        public Scanner(TextReader inputFile)
        {
            m_tokens = new List<TokenData>();
            m_inputFile = inputFile;
            m_currentLineNumber = 1;
            m_currentCharPosition = 1;

            Scan();

            /*foreach (TokenData token in m_tokens)
            {
                Console.WriteLine("Token: {0}\tLine: {1}\tPosition: {2}",
                    token.data.ToString(), token.lineNumber, token.charPosition);
            }*/
        }

        private void Scan()
        {
            while (m_inputFile.Peek() != -1)
            {
                PeekNextChar();

                char currentChar = CurrentChar();

                if (Char.IsWhiteSpace(currentChar))
                {
                    ScanWhiteSpace();
                }
                else if (Char.IsLetter(currentChar))
                {
                    ScanKeywordOrIdentifier();
                }
                else if (Char.IsDigit(currentChar))
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
                return;
            }

            ReadNextChar();
        }

        private void ScanKeywordOrIdentifier()
        {
            StringBuilder token = new StringBuilder();

            int lineNumber = m_currentLineNumber;
            int charPosition = m_currentCharPosition;

            while (Char.IsLetter(CurrentChar()))
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

            int lineNumber = m_currentLineNumber;
            int charPosition = m_currentCharPosition;

            while (Char.IsDigit(CurrentChar()))
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
            bool foundMatch = false;

            foreach (KeyValuePair<char, Language.SpecialCharacter> pair in Language.SpecialCharacters)
            {
                if (CurrentChar().Equals(pair.Key))
                {
                    foundMatch = true;

                    m_tokens.Add(new TokenData(m_currentLineNumber, m_currentCharPosition, pair.Value));

                    ReadNextChar();
                }
            }

            if (!foundMatch)
            {
                throw new Exception(String.Format("SCANNER::ERROR -> Unknown token encountered '{0}'", CurrentChar()));
            }
        }

        #region Helper Methods
        private int PeekNextChar()
        {
            m_currentChar = (char)m_inputFile.Peek();
            return (int)m_currentChar;
        }

        private void ReadNextChar()
        {
            m_inputFile.Read();
            m_currentCharPosition += 1;
        }

        private void ReadToNextLine()
        {
            m_inputFile.ReadLine();
            m_currentLineNumber += 1;
            m_currentCharPosition = 1;
        }

        private char CurrentChar()
        {
            return m_currentChar;
        }

        private bool EndOfFile()
        {
            return PeekNextChar() == -1;
        }
        #endregion // Helper Methods
    }
}