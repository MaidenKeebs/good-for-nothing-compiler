using System;
using System.Collections.Generic;

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
        public Type dataType;
        public string identifier;
        public Expression expression;

        public VariableCreation(Type dataType, string identifier, Expression expression)
        {
            this.dataType = dataType;
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

    public class FunctionCall : Statement
    {
        public string module; // Not always applicable.
        public string identifier;
        public System.Collections.Generic.IList<Expression> parameters;

        public FunctionCall(string module, string identifier, System.Collections.Generic.List<Expression> parameters)
        {
            this.module = module;
            this.identifier = identifier;
            this.parameters = parameters;
        }
    }

    public class FunctionDefinition : Statement
    {
        public string identifier;
        public string returnType;
        public Statement body;

        public FunctionDefinition(string identifier, string returnType, Statement body)
        {
            this.identifier = identifier;
            this.returnType = returnType;
            this.body = body;
        }
    }

    public abstract class Expression
    {
    }

    public class Variable : Expression
    {
        public string identifier;

        public Variable(string identifier)
        {
            this.identifier = identifier;
        }
    }

    public class IntegerLiteral : Expression
    {
        public int value;

        public IntegerLiteral(int value)
        {
            this.value = value;
        }
    }

    public class StringLiteral : Expression
    {
        public string value;

        public StringLiteral(string value)
        {
            this.value = value;
        }
    }

    public class BooleanLiteral : Expression
    {
        public bool value;

        public BooleanLiteral(bool value)
        {
            this.value = value;
        }
    }
}