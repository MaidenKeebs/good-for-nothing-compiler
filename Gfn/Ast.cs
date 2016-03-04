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

    public class VariableCreation : Statement
    {
//        public System.Type dataType;
        public string identifier;
        public Expression expression;

        public VariableCreation(string identifier, Expression expression)
        {
            this.identifier = identifier;
            this.expression = expression;
        }
    }

    public class VariableAssignment : Statement
    {
        public string identifier;
        public Expression expression;

        public VariableAssignment(string identifier, Expression expression)
        {
            this.identifier = identifier;
            this.expression = expression;
        }
    }

    public abstract class Expression
    {
    }

    public class IntegerLiteral : Expression
    {
        public int value;

        public IntegerLiteral(int value)
        {
            this.value = value;
        }
    }
}