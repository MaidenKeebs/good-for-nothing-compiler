namespace GfnCompiler
{
    public sealed class CodeGenerator
    {
        private readonly System.Reflection.Emit.AssemblyBuilder m_assemblyBuilder;
        private readonly System.Reflection.Emit.ILGenerator m_ilGenerator;
        private readonly System.Reflection.Emit.MethodBuilder m_methodBuilder;
        private readonly System.Reflection.Emit.ModuleBuilder m_moduleBuilder;
        private readonly string m_moduleName;
        private readonly Statement m_statement;
        private static System.Collections.Generic.Dictionary<string, System.Reflection.Emit.LocalBuilder> m_symbolTable;
        private readonly System.Reflection.Emit.TypeBuilder m_typeBuilder;

        public CodeGenerator(Statement statement, string moduleName)
        {
            m_statement = statement;
            m_moduleName = moduleName;

            // Just making sure moduleName is directed to current directory, that's all.
            if (System.IO.Path.GetFileName(moduleName) != moduleName)
            {
                throw new System.Exception("CodeGenerator: Can only output binary to current directory, sorry.");
            }

            // Comment this shit later. No, seriously.
            string fileName = System.IO.Path.GetFileNameWithoutExtension(moduleName);
            System.Reflection.AssemblyName assemblyName = new System.Reflection.AssemblyName(fileName);
            m_assemblyBuilder = System.AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, System.Reflection.Emit.AssemblyBuilderAccess.Save);
            m_moduleBuilder = m_assemblyBuilder.DefineDynamicModule(moduleName);
            m_typeBuilder = m_moduleBuilder.DefineType("MaidenType");
            m_methodBuilder = m_typeBuilder.DefineMethod("Main", System.Reflection.MethodAttributes.Static, typeof(void), System.Type.EmptyTypes);
            m_ilGenerator = m_methodBuilder.GetILGenerator();
            m_symbolTable = new System.Collections.Generic.Dictionary<string, System.Reflection.Emit.LocalBuilder>();
        }

        public void Compile()
        {
            GenerateStatement(m_statement);

            m_ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ret);
            m_typeBuilder.CreateType();
            m_moduleBuilder.CreateGlobalFunctions();
            m_assemblyBuilder.SetEntryPoint(m_methodBuilder);
            m_assemblyBuilder.Save(m_moduleName);
        }

        private void GenerateStatement(Statement statement)
        {
            if (statement is StatementSequence)
            {
                System.Console.WriteLine("Generating: StatementSequence");
                StatementSequence statementSequence = (StatementSequence)statement;

                GenerateStatement(statementSequence.first);
                GenerateStatement(statementSequence.second);
            }
            else if (statement is VariableCreation)
            {
                System.Console.WriteLine("Generating: VariableCreation");
                VariableCreation variableCreation = (VariableCreation)statement;

                // Declare the variable.
                m_symbolTable[variableCreation.identifier] = m_ilGenerator.DeclareLocal(variableCreation.expression.GetType());

                // Set the initial value.
                VariableAssignment variableAssignment = new VariableAssignment(variableCreation.identifier, variableCreation.expression);
                GenerateStatement(variableAssignment);
            }
            else if (statement is VariableAssignment)
            {
                System.Console.WriteLine("Generating: VariableAssignment");
                VariableAssignment variableAssignment = (VariableAssignment)statement;

                // I'm not going to pretend to know what these do. Just let it be - MaidenKeebs
                GenerateLoadToStackForExpression(variableAssignment.expression, variableAssignment.expression.GetType());
                GenerateStoreFromStack(variableAssignment.identifier, variableAssignment.expression.GetType());
            }
            else
            {
                throw new System.Exception("Unable to generator a: " + statement.GetType().Name);
            }
        }

        // This does something.
        private void GenerateLoadToStackForExpression(Expression expression, System.Type expectedType)
        {
            System.Type deliveredType = null;

            if (expression is IntegerLiteral)
            {
                IntegerLiteral integerLiteral = (IntegerLiteral)expression;

                deliveredType = typeof(int);

                m_ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, integerLiteral.value);
            }
            else
            {
                throw new System.Exception("Don't know how to generate:" + expression.GetType().Name);
            }

            // Happy times.
            if (deliveredType == expectedType)
            {
                return;
            }
        }

        // This does something aswell.
        private void GenerateStoreFromStack(string name, System.Type type)
        {
            if (!m_symbolTable.ContainsKey(name))
            {
                throw new System.Exception("Undeclared variable: " + name);
            }

            System.Reflection.Emit.LocalBuilder localName = m_symbolTable[name];
            System.Type localType = localName.LocalType;

            if (localType != type)
            {
                throw new System.Exception(System.String.Format("{0} is of type {1} but attempted to store value of type {2}",
                    name, localType == null ? "<unknown>" : localType.Name, type.Name));
            }
        }
    }
}