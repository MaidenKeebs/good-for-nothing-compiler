namespace GfnCompiler
{
    public sealed class Parser
    {
        private Statement m_result;
        private readonly System.Collections.Generic.IList<TokenData> m_tokens;
        private int m_index;

        public Parser(System.Collections.Generic.IList<TokenData> tokens)
        {
            m_tokens = tokens;
            m_index = 0;
        }

        public Statement GetResult()
        {
            return m_result;
        }

        private bool EndOfTokens()
        {
            return m_index >= m_tokens.Count;
        }

        private void NextToken()
        {
            ++m_index;
        }

        private TokenData CurrentToken()
        {
            return m_tokens[m_index];
        }

        public void Parse()
        {
            m_result = ParseStatement();

            if (!EndOfTokens())
            {
                throw new System.Exception("Expected end-of-file, but didn't quite get that.");
            }

            // Just a shizzling debug.
            System.Console.WriteLine("Parser Result: {0}", m_result.ToString());
        }

        private Statement ParseStatement()
        {
            Statement result;

            if (EndOfTokens())
            {
                throw new System.Exception("Expected a statement, but reached end-of-file.");
            }

            if (CurrentToken().data is string && Language.dataTypes.Contains(CurrentToken().data.ToString()))
            {
                result = ParseVariableCreation();
            }
            else
            {
                throw new System.Exception("Parse error.");
            }

            NextToken();

            // Just enforcing a semi-colon at the end of each statement.
            if (!EndOfTokens() && CurrentToken().data.Equals(Language.SpecialCharacter.SemiColon))
            {
                NextToken();

                if (!EndOfTokens())
                {
                    result = new StatementSequence(result, ParseStatement());
                }
            }
            else
            {
                throw new System.Exception("Expected SemiColon!!!");
            }

            // Nothing left to parse.
            if (EndOfTokens())
            {
                return result;
            }

            return result;
        }

        private Statement ParseVariableCreation()
        {
            ////////////////////////////////////////////
            // <data_type> <identifier> = <expression> ;
            string dataType = CurrentToken().data.ToString();

            NextToken();

            if (EndOfTokens() || !(CurrentToken().data is string))
            {
                throw new System.Exception("Expected identifier after data type.");
            }

            string identifier = CurrentToken().data.ToString();

            NextToken();

            if (EndOfTokens() || !(CurrentToken().data.Equals(Language.SpecialCharacter.AssignEquals)))
            {
                throw new System.Exception("Expected '=' after <data_type> <identifier>");
            }

            NextToken();

            IntegerLiteral expression = new IntegerLiteral(System.Int32.Parse(CurrentToken().data.ToString()));

            return new VariableCreation(identifier, expression);
        }
    }
}