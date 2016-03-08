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

        private TokenData PeekNextToken()
        {
            return m_tokens[m_index + 1];
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

            if (CurrentToken().data is string && Language.dataTypes.ContainsValue(CurrentToken().data.ToString()))
            {
                ///////////////////////////////////////////
                // <data_type> <identifier> = <expression>;

                result = ParseVariableCreation();
            }
            else if (CurrentToken().data.Equals(Language.SpecialCharacter.FunctionPrefix))
            {
                //////////////////////////////////////////////////////////
                // @<module>:<function_name>(<parameter>[, <parameter>]*);

                // Skip over the @ character.
                NextToken();

                string module = System.String.Empty;
                string identifier = System.String.Empty;
                string parameter = System.String.Empty;
                System.Collections.Generic.List<string> parameters = new System.Collections.Generic.List<string>();

                if (PeekNextToken().data.Equals(Language.SpecialCharacter.Colon))
                {
                    // We're using a function from the GfnStdLib.
                    module = CurrentToken().data.ToString();

                    // On to the colon.
                    NextToken();
                    // Now skip it.
                    NextToken();

                    identifier = CurrentToken().data.ToString();

                    NextToken();

                    if (!CurrentToken().data.Equals(Language.SpecialCharacter.LeftParenthesis))
                    {
                        throw new System.Exception(System.String.Format("Expected ) on function call. <A> but got {0}, {1}, {2}",
                            CurrentToken().data.ToString(), CurrentToken().lineNumber, CurrentToken().charPosition));
                    }

                    // Over the (
                    NextToken();

                    if (!CurrentToken().data.Equals(Language.SpecialCharacter.RightParenthesis))
                    {
                        // Add the first parameter.
                        parameters.Add(CurrentToken().data.ToString());
                        NextToken();

                        System.Console.WriteLine("PRE-PRICK");
                        // Check for any more parameters.
                        while (CurrentToken().data.Equals(Language.SpecialCharacter.Comma))
                        {
                            // Over comma.
                            NextToken();
                            // While the next token's a comma, keep parsing parameters.
                            parameters.Add(CurrentToken().data.ToString());

                            if (PeekNextToken().Equals(Language.SpecialCharacter.RightParenthesis))
                            {
                                break;
                            }

                            NextToken();
                        }
                    }

                    if (!CurrentToken().data.Equals(Language.SpecialCharacter.RightParenthesis))
                    {
                        throw new System.Exception(System.String.Format("Expected ) on function call. <B> but got {0}, {1}, {2}",
                            CurrentToken().data.ToString(), CurrentToken().lineNumber, CurrentToken().charPosition));
                    }
                }
                else
                {
                    identifier = CurrentToken().data.ToString();

                    NextToken();

                    if (!CurrentToken().data.Equals(Language.SpecialCharacter.LeftParenthesis))
                    {
                        throw new System.Exception("Expected ( on function call. <B>");
                    }

                    // Over the (
                    NextToken();

                    if (!CurrentToken().data.Equals(Language.SpecialCharacter.RightParenthesis))
                    {
                        // Get parameter.
                        parameters.Add(CurrentToken().data.ToString());

                        while (PeekNextToken().Equals(Language.SpecialCharacter.Comma))
                        {
                            // Skip the comma.
                            NextToken();

                            // Get the param.
                            parameters.Add(CurrentToken().data.ToString());
                        }

                        NextToken();
                    }

                    if (!CurrentToken().data.Equals(Language.SpecialCharacter.RightParenthesis))
                    {
                        throw new System.Exception("Expected ) on function call.");
                    }
                }

                result = new FunctionCall(module, identifier, parameter, parameters);
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
                throw new System.Exception(System.String.Format("Expected SemiColon: line {0}, position {1}",
                    CurrentToken().lineNumber.ToString(), CurrentToken().charPosition.ToString()));
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

            // Rewrite this to be more flexible and less hard-coded - MaidenKeebs
            switch (dataType)
            {
                case "integer":
                    IntegerLiteral integerLiteral = new IntegerLiteral(System.Int32.Parse(CurrentToken().data.ToString()));
                    return new VariableCreation(identifier, integerLiteral);

                case "string":
                    StringLiteral stringLiteral = new StringLiteral(CurrentToken().data.ToString());
                    return new VariableCreation(identifier, stringLiteral);

                case "boolean":
                    BooleanLiteral booleanLiteral = new BooleanLiteral(System.Boolean.Parse(CurrentToken().data.ToString()));
                    return new VariableCreation(identifier, booleanLiteral);

                default:
                    throw new System.Exception("Unknown thingy... <best error handling ever, not>");
            }
        }
    }
}