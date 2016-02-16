using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LuaAdvanced.Compiler.Lexer;
using LuaAdvanced.Compiler.Parser.Instructions;

namespace LuaAdvanced.Compiler.Parser
{
    partial class Parser
    {
        public class ClassMethod
        {
            public string name;
            public List<string> paramList;
            public Instruction sequence;
        }

        Instruction Statement_Class(bool local, bool partial)
        {
            var name = RequireIdentifier("Class name expected.").Value;

            string baseClass = "";

            if (AcceptSymbol(":"))
                baseClass = RequireIdentifier("Base class name expected.").Value;

            RequireSymbol("{", "'{' required to open the class.");

            Dictionary<string, Instruction> fields = new Dictionary<string, Instruction>();
            List<ClassMethod> methods = new List<ClassMethod>();

            while (!AcceptSymbol("}"))
            {
                if (AcceptKeyword("var"))
                {
                    List<string> varList = new List<string>();

                    bool comma = true, any = false;
                    while (!AcceptSymbol("=", ";"))
                    {
                        any = true;
                        if (!comma)
                            ThrowException("Comma required between variables");
                        varList.Add(Expression_VariableOrTableVariable().Inline);
                        comma = AcceptSymbol(",");
                    }
                    if (comma && any)
                        ThrowException("Unexpected comma.");

                    PrevToken();

                    List<Instruction> valueList = new List<Instruction>();

                    if (AcceptSymbol("="))
                    {
                        comma = false;
                        any = false;
                        do
                        {
                            if (any && !comma)
                                ThrowException("Comma required between variables");
                            any = true;
                            valueList.Add(Expression());
                        } while ((comma = AcceptSymbol(",")));
                    }

                    RequireSymbol(";");

                    for (int i = 0; i < varList.Count; i++)
                        fields.Add(varList[i], valueList.Count > i ? valueList[i] : null);
                }

                else if (AcceptKeyword("function"))
                {
                    string methodName = RequireIdentifier("Function name expected.").Value;

                    if (methods.Any(m => m.name == methodName))
                        ThrowException($"Method '{methodName}' is already defined.");

                    RequireSymbol("(");
                    List<string> paramList = new List<string>();
                    bool comma = true, any = false;
                    while (AcceptIdentifier())
                    {
                        any = true;
                        if (!comma)
                            ThrowException("Comma required between function parameters");
                        paramList.Add(currentToken.Value);
                        comma = AcceptSymbol(",");
                    }
                    if (comma && any)
                        ThrowException("Unexpected comma.");
                    RequireSymbol(")");

                    methods.Add(new ClassMethod()
                    {
                        name = methodName,
                        paramList = paramList,
                        sequence = Block()
                    });
                }

                else if(NextToken())
                    ThrowException($"Unexpected token '{currentToken.Value}'.");

                else
                    ThrowException("Unexpected end of file.");
            }

            return new Class(name, fields, methods, baseClass, local, partial);
        }
    }
}
