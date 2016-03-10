using System;

namespace GfnCompiler
{
    public abstract class Statement
    {
    }

    public class StatementSequence : Statement
    {
        public Statement first;
        public Statement second;

        public StatementSequence(Statement first, Statement second)
        {
            this.first = first;
            this.second = second;
        }
    }

    public class VariableInstantiation : Statement
    {
        public Type dataType;
        public string identifier;
        public Expression expression;

        public VariableInstantiation(Type dataType, string identifier, Expression expression)
        {
            this.dataType = dataType;
            this.identifier = identifier;
            this.expression = expression;
        }
    }

    public abstract class Expression
    {
    }

    public class IntegerLiteral : Expression
    {
        int value;

        public IntegerLiteral(int value)
        {
            this.value = value;
        }
    }
}