using System;
using System.Reflection;
using System.Reflection.Emit;

namespace GfnCompiler
{
    public sealed class CodeGenerator
    {
        // Comment This - MaidenKeebs
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

            // Just making sure moduleName is directed to current directory, that's all - MaidenKeebs
            if (System.IO.Path.GetFileName(moduleName) != moduleName)
            {
                throw new System.Exception("CodeGenerator: Can only output binary to current directory, sorry.");
            }

            // Comment this shit later. No, seriously - MaidenKeebs
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
            // Comment this - MaidenKeebs
            GenerateStatement(m_statement);

            // Create custom method for testing purposes.
            MethodBuilder methodBuilder = m_typeBuilder.DefineMethod(
                "PrintGreeting",
                MethodAttributes.Public | MethodAttributes.Static,
                typeof(void),
                new Type[] { });
            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldstr, "Greetings, Adventurer.");
            ilGenerator.Emit(
                OpCodes.Call,
                typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
            ilGenerator.Emit(OpCodes.Ret);

            // Call the method here.
            m_ilGenerator.EmitCall(
                OpCodes.Call,
                methodBuilder,
                new Type[] { });

            m_ilGenerator.Emit(OpCodes.Ret);
            m_typeBuilder.CreateType();
            m_moduleBuilder.CreateGlobalFunctions();
            m_assemblyBuilder.SetEntryPoint(m_methodBuilder);
            m_assemblyBuilder.Save(m_moduleName);
        }

        private void GenerateStatement(Statement statement)
        {
            if (statement is StatementSequence)
            {
                //System.Console.WriteLine("Generating: StatementSequence");
                StatementSequence statementSequence = (StatementSequence)statement;

                GenerateStatement(statementSequence.first);
                GenerateStatement(statementSequence.second);
            }
            else if (statement is VariableCreation)
            {
                //System.Console.WriteLine("Generating: VariableCreation");
                VariableCreation variableCreation = (VariableCreation)statement;

                // Declare the variable.
                m_symbolTable[variableCreation.identifier] = m_ilGenerator.DeclareLocal(variableCreation.expression.GetType());

                // Set the initial value.
                VariableAssignment variableAssignment = new VariableAssignment(variableCreation.identifier, variableCreation.expression);
                GenerateStatement(variableAssignment);
            }
            else if (statement is VariableAssignment)
            {
                //System.Console.WriteLine("Generating: VariableAssignment");
                VariableAssignment variableAssignment = (VariableAssignment)statement;

                // I'm not going to pretend to know what these do. Just let it be - MaidenKeebs
                GenerateLoadToStackForExpression(variableAssignment.expression, variableAssignment.expression.GetType());
                GenerateStoreFromStack(variableAssignment.identifier, variableAssignment.expression.GetType());
            }
            else if (statement is FunctionCall)
            {
                FunctionCall functionCall = (FunctionCall)statement;

                if (functionCall.module != System.String.Empty)
                {
                    if (functionCall.parameters.Count > 0)
                    {
                        System.Type[] typeList = new System.Type[functionCall.parameters.Count];
                        int i = 0;
                        foreach (Expression parameter in functionCall.parameters)
                        {
                            GenerateLoadToStackForExpression(parameter, parameter.GetType());

                            typeList[i] = typeof(string);
                            ++i;
                        }

                        m_ilGenerator.Emit(System.Reflection.Emit.OpCodes.Call, typeof(GfnStdLib.IO).GetMethod(functionCall.identifier,
                        typeList));
                    }
                    else
                    {
                        // Parameterless function call IL generation.
                        m_ilGenerator.Emit(System.Reflection.Emit.OpCodes.Call, typeof(GfnStdLib.IO).GetMethod(functionCall.identifier));
                    }
                }
                else
                {
                    // Basically the same as above, but doesn't run from the GfnStdLib.
                    // This will be eventually used for user-defined functions.
                    if (functionCall.parameters.Count > 0)
                    {
                        foreach (Expression parameter in functionCall.parameters)
                        {
                            //System.Console.WriteLine("!-!-! CodeGen <A> {0}", parameter);
                            //m_ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ldstr, parameter);
                            GenerateLoadToStackForExpression(parameter, parameter.GetType());
                        }

                        // Hard-coded.
                        System.Type[] typeList = new System.Type[functionCall.parameters.Count];
                        for (int i = 0; i < typeList.Length; ++i)
                        {
                            typeList[i] = typeof(string);
                        }

                        m_ilGenerator.Emit(System.Reflection.Emit.OpCodes.Call, typeof(GfnCompiler.NoModule).GetMethod(functionCall.identifier,
                        typeList));

                    }
                    else
                    {
                        // Parameterless function call IL generation.
                        m_ilGenerator.Emit(System.Reflection.Emit.OpCodes.Call, typeof(GfnCompiler.NoModule).GetMethod(functionCall.identifier));
                    }
                }
            }
            else
            {
                throw new System.Exception("Unable to generator a: " + statement.GetType().Name);
            }
        }

        // Load variables to stack.
        private void GenerateLoadToStackForExpression(Expression expression, System.Type expectedType)
        {
            if (expression is IntegerLiteral)
            {
                IntegerLiteral integerLiteral = (IntegerLiteral)expression;

                m_ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, integerLiteral.value);
            }
            else if (expression is StringLiteral)
            {
                StringLiteral stringLiteral = (StringLiteral)expression;

                m_ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ldstr, stringLiteral.value);
            }
            else if (expression is BooleanLiteral)
            {
                BooleanLiteral booleanLiteral = (BooleanLiteral)expression;

                // Generate either 1 or 0 to represent boolean values.
                if (booleanLiteral.value == true)
                {
                    m_ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_1);
                }
                else
                {
                    m_ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
                }
            }
            else if (expression is Variable)
            {
                Variable variable = (Variable)expression;

                if (!m_symbolTable.ContainsKey(variable.identifier))
                {
                    throw new System.Exception(System.String.Format("Undeclared variable '{0}'", variable.identifier));
                }

                m_ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ldloc, m_symbolTable[variable.identifier]);
            }
            else
            {
                throw new System.Exception("Don't know how to generate:" + expression.GetType().Name);
            }
            // Some code here, I do believe.
        }

        // Store variables in memory.
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

            m_ilGenerator.Emit(System.Reflection.Emit.OpCodes.Stloc, m_symbolTable[name]);
        }
    }
}