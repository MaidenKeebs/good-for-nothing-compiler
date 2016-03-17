namespace GfnCompiler
{
    public sealed class Parser
    {
        #region Data Members
        private Statement m_result;
        private readonly System.Collections.Generic.IList<TokenData> m_tokens;
        private int m_index;
        #endregion // Data Members.

        #region Helper Methods
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
        #endregion // Helper Methods

        public Parser(System.Collections.Generic.IList<TokenData> tokens)
        {
            m_tokens = tokens;
            m_index = 0;
        }

        public void Parse()
        {
            m_result = ParseStatement();

            if (!EndOfTokens())
            {
                throw new System.Exception("Expected end-of-file, but didn't quite get that.");
            }

            // Just a shizzling debug.
            //System.Console.WriteLine("Parser Result: {0}", m_result.ToString());
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
                result = ParseVariableCreation();
            }
            // Don't have to worry about this code running, ever.
            else if (CurrentToken().data.Equals(Language.SpecialCharacter.FunctionPrefix))
            {
                result = ParseFunctionCall();
            }
            else
            {
                throw new System.Exception("Parse error.");
            }

            result = ParseStatementTerminator(result);

            if (EndOfTokens())
            {
                return result;
            }

            return result;
        }

        private Statement ParseVariableCreation()
        {
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
                case "integer32":
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

        private Statement ParseFunctionCall()
        {
            // Skip over the @ character.
            NextToken();

            string module = System.String.Empty;
            string identifier = System.String.Empty;
            System.Collections.Generic.List<Expression> parameters = new System.Collections.Generic.List<Expression>();

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
                    parameters.Add(ParseExpression());
                    NextToken();
                    
                    // Check for any more parameters.
                    while (CurrentToken().data.Equals(Language.SpecialCharacter.Comma))
                    {
                        // Over comma.
                        NextToken();
                        // While the next token's a comma, keep parsing parameters.
                        parameters.Add(ParseExpression());

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
                    parameters.Add(ParseExpression());

                    while (PeekNextToken().Equals(Language.SpecialCharacter.Comma))
                    {
                        // Skip the comma.
                        NextToken();

                        // Get the param.
                        parameters.Add(ParseExpression());
                    }

                    NextToken();
                }

                if (!CurrentToken().data.Equals(Language.SpecialCharacter.RightParenthesis))
                {
                    throw new System.Exception("Expected ) on function call.");
                }
            }

            return new FunctionCall(module, identifier, parameters);
        }

        private Statement ParseStatementTerminator(Statement result)
        {
            NextToken();
            
            // Just enforcing a semi-colon at the end of each statement.
            if (!EndOfTokens() && CurrentToken().data.Equals(Language.SpecialCharacter.SemiColon))
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
                throw new System.Exception(System.String.Format("Expected SemiColon: line {0}, position {1}",
                    CurrentToken().lineNumber.ToString(), CurrentToken().charPosition.ToString()));
            }
        }

        // TODO-MaidenKeebs:
        //     Continue working on this function.
        private Expression ParseExpression()
        {
            Expression expression;

            if (EndOfTokens())
            {
                throw new System.Exception("Reached end of tokens in ParseExpression().");
            }

            expression = ParseUnaryExpression();

            return expression;
        }

        // Example: "Hello, World!"
        private Expression ParseUnaryExpression()
        {
            if (CurrentToken().data is System.Text.StringBuilder)
            {
                string value = CurrentToken().data.ToString();
                StringLiteral stringLiteral = new StringLiteral(value);

                return stringLiteral;
            }
            else if (CurrentToken().data is int)
            {
                int value = (int)CurrentToken().data;
                IntegerLiteral integerLiteral = new IntegerLiteral(value);

                return integerLiteral;
            }
            else if (CurrentToken().data is string)
            {
                string identifier = CurrentToken().data.ToString();
                Variable variable = new Variable(identifier);

                return variable;
            }

            // Manual here.
            throw new System.Exception("Parser Error: Unknown Unary Expression has been found!");
        }

        // Example: a+b
        private Expression ParseBinaryExpression()
        {
            return null;
        }
    }
}