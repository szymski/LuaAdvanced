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
        string statementEnd = ";";

        void RequireStatementEnd()
        {
            if (string.IsNullOrEmpty(statementEnd))
                return;
            RequireSymbol(statementEnd);
        }

        Instruction Statement(string end = ";")
        {
            var prev = statementEnd;
            statementEnd = end;
            var statment = Statement1_If();
            statementEnd = prev; // Set previous end string, to make everything stable.
            return statment;
        }

        Instruction Statement1_If()
        {
            List<Tuple<Instruction, Instruction>> ifs = new List<Tuple<Instruction, Instruction>>();

            while (AcceptKeyword("if", "else"))
            {
                if (currentToken.Value == "if")
                {
                    if (ifs.Count > 0)
                    {
                        PrevToken();
                        break;
                    }

                    RequireSymbol("(");
                    Instruction cond = Expression();
                    RequireSymbol(")");
                    ifs.Add(new Tuple<Instruction, Instruction>(cond, Block()));
                }
                else
                {
                    if (AcceptKeyword("if"))
                    {
                        RequireSymbol("(");
                        Instruction cond = Expression();
                        RequireSymbol(")");
                        ifs.Add(new Tuple<Instruction, Instruction>(cond, Block()));
                        continue;
                    }

                    ifs.Add(new Tuple<Instruction, Instruction>(null, Block()));
                    break;
                }
            }

            if (ifs.Count > 0)
                return new If(ifs);

            return Statement2_WhileLoop();
        }

        Instruction Statement2_WhileLoop()
        {
            if (AcceptKeyword("while"))
            {
                loopDepth++;
                RequireSymbol("(");
                Instruction cond = Expression();
                RequireSymbol(")");
                Instruction seq = Block();
                loopDepth--;
                return new WhileLoop(cond, seq);
            }
            return Statement3_ForLoop();
        }

        Instruction Statement3_ForLoop()
        {
            if (AcceptKeyword("for"))
            {
                loopDepth++;
                RequireSymbol("(");
                Instruction init = Statement();
                Instruction cond = Expression();
                RequireSymbol(";");
                Instruction after = Statement("");
                RequireSymbol(")");
                Instruction seq = Block();
                loopDepth--;
                return new ForLoop(init, cond, after, seq);
            }
            return Statement4_ForEachLoop();
        }

        Instruction Statement4_ForEachLoop()
        {
            if (AcceptKeyword("foreach"))
            {
                loopDepth++;
                RequireSymbol("(");
                RequireKeyword("var", "Variable expected.");
                string varName = RequireIdentifier("Variable name expected.").Value;
                if (AcceptSymbol(","))
                {
                    string keyName = varName;
                    varName = RequireIdentifier("Key variable name expected.").Value;
                    RequireKeyword("in");
                    Instruction expr2 = Expression();
                    RequireSymbol(")");
                    Instruction seq2 = Block();
                    loopDepth--;
                    return new ForEachLoop(keyName, varName, expr2, seq2);
                }
                RequireKeyword("in");
                Instruction expr = Expression();
                RequireSymbol(")");
                Instruction seq = Block();
                loopDepth--;
                return new ForEachLoop(varName, expr, seq);
            }
            return Statement5_BreakContinueReturn();
        }

        Instruction Statement5_BreakContinueReturn()
        {
            if (AcceptKeyword("break", "continue"))
            {
                if (loopDepth > 0)
                {
                    if (currentToken.Value == "break")
                    {
                        RequireSymbol(";");
                        return new PreparedInstruction("break");
                    }
                    else
                    {
                        RequireSymbol(";");
                        if (!loopContinues.Contains(loopDepth))
                            loopContinues.Add(loopDepth);
                        return new Continue(loopDepth);
                    }
                }
                else
                    ThrowException($"{currentToken.Value} must be placed inside a loop.");
                RequireSymbol(";");
            }

            if (AcceptKeyword("return"))
            {
                if (AcceptSymbol(";"))
                    return new PreparedInstruction("return");

                var exp = Expression();
                RequireSymbol(";");
                return new PreparedInstruction($"return {exp.Inline}");
            }

            return Statement6_Switch();
        }

        Instruction Statement6_Switch()
        {
            if (AcceptKeyword("switch"))
            {
                RequireSymbol("(");
                Instruction value = Expression();
                RequireSymbol(")");

                List<Tuple<Instruction, Instruction, bool>> cases = new List<Tuple<Instruction, Instruction, bool>>();

                RequireSymbol("{");
                while (!AcceptSymbol("}"))
                {
                    if (AcceptKeyword("case"))
                    {
                        var ins = Expression_RawValue();

                        RequireSymbol(":");

                        var seq = Sequence("break", "case", "default");
                        PrevToken();

                        bool brk = AcceptKeyword("break");
                        if (brk)
                            RequireSymbol(";");

                        cases.Add(new Tuple<Instruction, Instruction, bool>(ins, seq, brk));
                    }
                    else if (AcceptKeyword("default"))
                    {
                        var ins = new Expression("");

                        RequireSymbol(":");

                        var seq = Sequence("break", "case");
                        PrevToken();

                        bool brk = AcceptKeyword("break");
                        if (brk)
                            RequireSymbol(";");

                        cases.Add(new Tuple<Instruction, Instruction, bool>(ins, seq, brk));
                    }
                    else
                    {
                        ThrowException("Unexpected statement.");
                    }
                }

                return new Switch(value, cases);
            }
            return Statement7_Function();
        }

        Instruction Statement7_Function()
        {
            bool local = AcceptKeyword("local");

            if (AcceptKeyword("function"))
            {
                //string name = RequireIdentifier("Function name expected.").Value;

                //RequireSymbol("(");
                //List<string> paramList = new List<string>();
                //bool comma = true, any = false;
                //while (AcceptIdentifier())
                //{
                //    any = true;
                //    if (!comma)
                //        ThrowException("Comma required between function parameters");
                //    paramList.Add(currentToken.Value);
                //    comma = AcceptSymbol(",");
                //}
                //if (comma && any)
                //    ThrowException("Unexpected comma.");
                //RequireSymbol(")");
                //return new Function(name, paramList, Block(), local);

                // TODO: Make sure the new way works proper, and remove the old one

                string name = Expression_FunctionCall().Inline;
                return new Function(name, Block(), local);
            }
            else if (local)
                PrevToken();

            return Statement85_Class();
        }

        Instruction Statement85_Class()
        {
            if (AcceptKeyword("class"))
                return Statement_Class();

            return Statement8_NewVariableAssignment();
        }

        Instruction Statement8_NewVariableAssignment()
        {
            if (AcceptKeyword("var"))
            {
                var startTokenIndex = tokenIndex;

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
                    }
                    while ((comma = AcceptSymbol(",")));
                }

                RequireStatementEnd();

                return new NewVariable(varList, valueList);
            }

            return Statement9_ExistingVariableAssignment();
        }

        Instruction Statement9_ExistingVariableAssignment()
        {
            var startTokenIndex = tokenIndex;

            List<string> varList = new List<string>();

            bool comma = true, any = false;
            while (!AcceptSymbol("=", ";", "+=", "-=", "*=", "/=", "^=", "..=", "++", "--", "(", ".", ":"))
            {
                any = true;
                if (!comma)
                    ThrowException("Comma required between variables.");
                varList.Add(Expression_VariableOrTableVariable().Inline);

                comma = AcceptSymbol(",");
            }
            if (comma && any)
                ThrowException("Unexpected comma.");

            PrevToken();

            if (!AcceptSymbol("="))
            {
                tokenIndex = startTokenIndex;
                CancelPreparation();
                return Statement10_StatementExpressions();
            }

            List<Instruction> valueList = new List<Instruction>();

            comma = false;
            any = false;
            do
            {
                if (any && !comma)
                    ThrowException("Comma required between variables");
                any = true;
                valueList.Add(Expression());
            }
            while ((comma = AcceptSymbol(",")));

            RequireStatementEnd();

            return new VariableModify(varList, valueList);
        }

        Instruction Statement10_StatementExpressions()
        {
            var startIndex = tokenIndex;
            var exp = Expression_VariableOrTableVariable();

            if (AcceptSymbol("++"))
                exp = new SelfAssignmentOperation(exp, "+", "1");
            else if (AcceptSymbol("--"))
                exp = new SelfAssignmentOperation(exp, "-", "1");
            else if (AcceptSymbol("+="))
                exp = new SelfAssignmentOperation(exp, "+", Expression().Inline);
            else if (AcceptSymbol("-="))
                exp = new SelfAssignmentOperation(exp, "-", Expression().Inline);
            else if (AcceptSymbol("*="))
                exp = new SelfAssignmentOperation(exp, "*", Expression().Inline);
            else if (AcceptSymbol("/="))
                exp = new SelfAssignmentOperation(exp, "/", Expression().Inline);
            else if (AcceptSymbol("^="))
                exp = new SelfAssignmentOperation(exp, "^", Expression().Inline);
            else if (AcceptSymbol("..="))
                exp = new SelfAssignmentOperation(exp, "..", Expression().Inline);
            else
            {
                tokenIndex = startIndex;
                return Statement11_FunctionCalls();
            }

            RequireStatementEnd();

            return exp;
        }

        Instruction Statement11_FunctionCalls()
        {
            var exp = Expression_FunctionCall();

            RequireStatementEnd();

            return new PreparedInstruction(exp.Inline);
        }

        Instruction InvalidStatement()
        {
            ThrowException("Unknown statement.");
            return null;
        }
    }
}
