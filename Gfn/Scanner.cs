using System;
using System.Collections.Generic;
using System.IO;

namespace GfnCompiler
{
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
        private readonly IList<TokenData> m_tokens;
        private TextReader m_inputSourceFile;
        private char m_currentFileChar;
        private int m_currentFileLineNumber;
        private int m_currentFileCharPosition;

        public Scanner(TextReader inputSourceFile)
        {
            m_tokens = new List<TokenData>();
            m_inputSourceFile = inputSourceFile;
            m_currentFileLineNumber = 1;
            m_currentFileCharPosition = 1;

            Scan();

            GfnDebug.PrintScannerResult(m_tokens);
        }

        #region Interface
        public IList<TokenData> GetTokens()
        {
            return m_tokens;
        }
        #endregion // Interface

        #region Helper Methods
        private int PeekNextToken()
        {
            m_currentFileChar = (char)m_inputSourceFile.Peek();
            return (int)m_currentFileChar;
        }

        private void ReadNextChar()
        {
            m_inputSourceFile.Read();
            m_currentFileCharPosition += 1;
        }

        private void ReadToNextLine()
        {
            m_inputSourceFile.ReadLine();
            m_currentFileLineNumber += 1;
            m_currentFileCharPosition = 1;
        }

        private char CurrentToken()
        {
            return m_currentFileChar;
        }

        private bool EndOfFile()
        {
            return m_inputSourceFile.Peek() == -1;
        }
        #endregion // Helper Methods

        #region Core Methods
        private void Scan()
        {
            while (m_inputSourceFile.Peek() != -1)
            {
                PeekNextToken();

                if (char.IsWhiteSpace(CurrentToken()))
                {
                    ScanWhiteSpace();
                }
                else if (char.IsLetter(CurrentToken()))
                {
                    ScanKeywordOrIdentifier();
                }
                else if (char.IsDigit(CurrentToken()))
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
                ReadToNextLine();
                return;
            }

            ReadNextChar();
        }

        private void ScanKeywordOrIdentifier()
        {
            System.Text.StringBuilder token = new System.Text.StringBuilder();

            int lineNumber = m_currentFileLineNumber;
            int charPosition = m_currentFileCharPosition;

            while (char.IsLetter(CurrentToken()) || char.IsDigit(CurrentToken()) || CurrentToken().Equals('_'))
            {
                token.Append(CurrentToken());
                ReadNextChar();

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

            while (char.IsDigit(CurrentToken()))
            {
                token.Append(CurrentToken());
                ReadNextChar();

                if (EndOfFile())
                {
                    break;
                }

                PeekNextToken();
            }

            m_tokens.Add(new TokenData(lineNumber, charPosition, int.Parse(token.ToString())));
        }

        private void ScanStringLiteral()
        {
            System.Text.StringBuilder token = new System.Text.StringBuilder();

            int lineNumber = m_currentFileLineNumber;
            int charPosition = m_currentFileCharPosition;

            // Skip the initial '"'.
            ReadNextChar();

            if (EndOfFile())
            {
                throw new Exception("Unterminated string literal due to end-of-file. (A)");
            }

            while (PeekNextToken() != '"')
            {
                token.Append(CurrentToken());
                ReadNextChar();

                if (EndOfFile())
                {
                    throw new Exception("Unterminated string literal due to end-of-file.(B)");
                }
            }

            // Skip the second '"'.
            ReadNextChar();

            m_tokens.Add(new TokenData(lineNumber, charPosition, token));
        }

        private void ScanSpecialCharacter()
        {
            bool foundMatch = false;

            foreach (KeyValuePair<char, Language.SpecialCharacter> pair in Language.specialCharacters)
            {
                if (CurrentToken().Equals(pair.Key))
                {
                    foundMatch = true;

                    m_tokens.Add(new TokenData(m_currentFileLineNumber, m_currentFileCharPosition, pair.Value));
                    ReadNextChar();
                }
            }

            if (!foundMatch)
            {
                throw new Exception(String.Format("Encountered an unknown token on line '{0}' at position '{1}'.", m_currentFileLineNumber, m_currentFileCharPosition));
            }
        }
        #endregion // Core Methods
    }
}