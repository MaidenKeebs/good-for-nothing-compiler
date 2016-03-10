using System;
using System.Collections.Generic;

namespace GfnCompiler
{
    internal sealed class Parser
    {
        private Statement m_result;
        private readonly IList<TokenData> m_tokens;
        private int m_index;

        public Statement Result
        {
            get
            {
                return m_result;
            }
        }

        public Parser(IList<TokenData> tokens)
        {
            m_tokens = tokens;
            m_index = 0;

            Parse();
        }

        private void Parse()
        {
            m_result = ParseStatement();

            if (!EndOfTokens())
            {
                throw new Exception("PARSER::ERROR -> Expected end-of-file, but haven't actually reached it.");
            }
        }

        private Statement ParseStatement()
        {
            Statement result;

            if (EndOfTokens())
            {
                throw new Exception("Expected a statement, but got an end-of-file.");
            }

            if (CurrentTokenInfo().data is string && Language.dataTypes.ContainsKey(CurrentTokenInfo().data.ToString()))
            {
                result = ParseVariableInstantiation();
            }
            else
            {
                throw new Exception("PARSER::ERROR -> Came across an unknown token.");
            }

            NextToken();

            result = ParseStatementTerminator(result);
            
            return result;
        }

        private Statement ParseVariableInstantiation()
        {
            Type dataType = null;
            Language.dataTypes.TryGetValue(CurrentTokenInfo().data.ToString(), out dataType);

            if (EndOfTokens() || !(CurrentTokenInfo().data is string))
            {
                throw new Exception("PARSER::ERROR -> Expected identifier after data type, but got end-of-file or unexpected token.");
            }

            NextToken();

            string identifier = CurrentTokenInfo().data.ToString();

            NextToken();

            if (EndOfTokens() || !(CurrentTokenInfo().data.Equals(Language.SpecialCharacter.Equals)))
            {
                throw new Exception(String.Format("PARSER::ERROR ->Expected '=' after identifier, but got '{0}'", CurrentTokenInfo().data.ToString()));
            }

            NextToken();

            if (dataType == typeof(int))
            {
                IntegerLiteral integerLiteral = new IntegerLiteral((int)CurrentTokenInfo().data);
                return new VariableInstantiation(dataType, identifier, integerLiteral);
            }
            else
            {
                throw new Exception("PARSER::ERROR -> Unkown expression in variable instantiation.");
            }
        }

        private Statement ParseStatementTerminator(Statement result)
        {
            if (!EndOfTokens() && CurrentTokenInfo().data.Equals(Language.SpecialCharacter.SemiColon))
            {
                NextToken();

                if (!EndOfTokens())
                {
                    return new StatementSequence(result, ParseStatement());
                }
                else
                {
                    return result;
                }
            }
            else
            {
                throw new Exception("PARSER::ERROR -> Expected semi colon at end of statement.");
            }
        }

        #region Helper Methods
        private bool EndOfTokens()
        {
            return m_index >= m_tokens.Count;
        }

        private void NextToken()
        {
            ++m_index;
        }

        private TokenData CurrentTokenInfo()
        {
            return m_tokens[m_index];
        }
        #endregion // Helper Methods
    }
}