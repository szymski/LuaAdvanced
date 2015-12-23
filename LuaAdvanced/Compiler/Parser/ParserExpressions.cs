using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaAdvanced.Compiler.Lexer;
using LuaAdvanced.Compiler.Parser.Instructions;

namespace LuaAdvanced.Compiler.Parser
{
    partial class Parser
    {
        /// <summary>
        /// Parses an expression.
        /// </summary>
        /// <returns>Compiled expression.</returns>
        Instruction Expression()
        {
            return Expression1_Ternary_NullPropagation();
        }

        Instruction Expression1_Ternary_NullPropagation()
        {
            var exp1 = Expression2_LogicalOperators();

            if (AcceptSymbol("?"))
            {
                var exp2 = Expression2_LogicalOperators();
                RequireSymbol(":", "Colon (:) required for ternary operator.");
                return new Ternary(exp1, exp2, Expression2_LogicalOperators());
            }

            if (AcceptSymbol("??"))
            {
                return new NullPropagation(exp1, Expression2_LogicalOperators());
            }

            return exp1;
        }

        Instruction Expression2_LogicalOperators()
        {
            var expr = Expression3_BitwiseOperators();

            if (AcceptSymbol("||"))
            {
                return new LogicalOr(expr, Expression2_LogicalOperators());
            }
            else if (AcceptSymbol("&&"))
            {
                return new LogicalAnd(expr, Expression2_LogicalOperators());
            }

            return expr;
        }

        Instruction Expression3_BitwiseOperators()
        {
            var expr = Expression4_Comparisons();

            if (AcceptSymbol("|"))
            {
                return new BitwiseOr(expr, Expression2_LogicalOperators());
            }
            else if (AcceptSymbol("^^"))
            {
                return new BitwiseXor(expr, Expression2_LogicalOperators());
            }
            else if (AcceptSymbol("&"))
            {
                return new BitwiseAnd(expr, Expression2_LogicalOperators());
            }
            else if (AcceptSymbol("~"))
            {
                return new BitwiseNot(expr, Expression2_LogicalOperators());
            }
            else if (AcceptSymbol(">>"))
            {
                return new BitwiseRightShift(expr, Expression2_LogicalOperators());
            }
            else if (AcceptSymbol("<<"))
            {
                return new BitwiseLeftShift(expr, Expression2_LogicalOperators());
            }

            return expr;
        }

        Instruction Expression4_Comparisons()
        {
            var expr = Expression5_PreOperators();

            if (AcceptSymbol("=="))
            {
                return new Equals(expr, Expression3_BitwiseOperators());
            }
            else if (AcceptSymbol("!="))
            {
                return new NotEquals(expr, Expression3_BitwiseOperators());
            }
            else if (AcceptSymbol("<", "<=", ">", ">="))
            {
                var compOperator = currentToken.Value; // Why not? I'm lazy
                return new Comparison(expr, Expression3_BitwiseOperators(), compOperator);
            }

            return expr;
        }

        Instruction Expression5_PreOperators()
        {
            int logicalNegations = 0;
            while (AcceptSymbol("!"))
                logicalNegations++;

            if (logicalNegations > 0)
                return new LogicalNegation(Expression6_AdditionAndSubtraction_StringJoin(), logicalNegations);

            if (AcceptSymbol("-"))
                return new Expression($"-{Expression6_AdditionAndSubtraction_StringJoin().Inline}");
            else if (AcceptSymbol("#"))
                return new Expression($"#{Expression6_AdditionAndSubtraction_StringJoin().Inline}");

            if (AcceptSymbol("++"))
            {
                var exp = Expression_VariableOrTableVariable();
                statementPreparation.Add(new PreparedInstruction($"{exp.Inline} = {exp.Inline} + 1"));
                return exp;
            }
            else if (AcceptSymbol("--"))
            {
                var exp = Expression_VariableOrTableVariable();
                statementPreparation.Add(new PreparedInstruction($"{exp.Inline} = {exp.Inline} - 1"));
                return exp;
            }

            return Expression6_AdditionAndSubtraction_StringJoin();
        }

        Instruction Expression6_AdditionAndSubtraction_StringJoin()
        {
            var exp = Expression7_MultiplicationDivisionModulo();

            while (AcceptSymbol("+", "-", ".."))
            {
                exp = new Expression($"{exp.Inline} {currentToken.Value} {Expression().Inline}");
            }

            return exp;
        }

        Instruction Expression7_MultiplicationDivisionModulo()
        {
            var exp = Expression8_Power();

            while (AcceptSymbol("*", "/", "%"))
            {
                exp = new Expression($"{exp.Inline} {currentToken.Value} {Expression().Inline}");
            }

            return exp;
        }

        Instruction Expression8_Power()
        {
            var exp = Expression9_AssignmentOperations();

            while (AcceptSymbol("^"))
            {
                exp = new Expression($"{exp.Inline} {currentToken.Value} {Expression().Inline}");
            }

            return exp;
        }

        Instruction Expression9_AssignmentOperations()
        {
            var exp = Expression10_Is();

            if (AcceptSymbol("++"))
                statementAfterPreparation.Add(new PreparedInstruction($"{exp.Inline} = {exp.Inline} + 1"));
            else if (AcceptSymbol("--"))
                statementAfterPreparation.Add(new PreparedInstruction($"{exp.Inline} = {exp.Inline} - 1"));
            else if (AcceptSymbol("+="))
                statementPreparation.Add(new PreparedInstruction($"{exp.Inline} = {exp.Inline} + {Expression().Inline}"));
            else if (AcceptSymbol("-="))
                statementPreparation.Add(new PreparedInstruction($"{exp.Inline} = {exp.Inline} - {Expression().Inline}"));
            else if (AcceptSymbol("*="))
                statementPreparation.Add(new PreparedInstruction($"{exp.Inline} = {exp.Inline} * {Expression().Inline}"));
            else if (AcceptSymbol("/="))
                statementPreparation.Add(new PreparedInstruction($"{exp.Inline} = {exp.Inline} / {Expression().Inline}"));
            else if (AcceptSymbol("^="))
                statementPreparation.Add(new PreparedInstruction($"{exp.Inline} = {exp.Inline} ^ {Expression().Inline}"));
            else if (AcceptSymbol("..="))
                statementPreparation.Add(new PreparedInstruction($"{exp.Inline} = {exp.Inline} .. {Expression().Inline}"));

            return exp;
        }

        Instruction Expression10_Is()
        {
            var exp = Expression11_TableIndexPeriod();

            if (AcceptKeyword("is"))
            {
                var type = RequireIdentifier("Type name expected.").Value;
                return new Is(exp, type); // TODO: LuaAdvanced library
            }

            return exp;
        }

        Instruction Expression11_TableIndexPeriod()
        {
            var exp = Expression12_TableIndexAndCalls();

            while (AcceptSymbol("."))
            {
                if (!AcceptKeyword("this"))
                    RequireIdentifier("Table index expected");
                PrevToken();
                exp = new Expression($"{exp.Inline}.{Expression12_TableIndexAndCalls().Inline}");
            }

            return exp;
        }

        Instruction Expression12_TableIndexAndCalls()
        {
            var exp = Expression13_AnonymousFunctions();

            while (AcceptSymbol("(", "[", ":"))
            {
                PrevToken();

                if (AcceptSymbol("("))
                {
                    PrevToken();
                    exp = new FunctionCall($"{exp.Inline}", Expression_CallParameters());
                }
                else if (AcceptSymbol("["))
                {
                    exp = new Expression($"{exp.Inline}[{Expression().Inline}]");
                    RequireSymbol("]");
                }
                else if (AcceptSymbol(":"))
                {
                    if (AcceptIdentifier()) // Ternary operator gets mad
                    {
                        var name = currentToken.Value;

                        if (AcceptSymbol("("))
                            PrevToken();
                        else
                        {
                            PrevToken();
                            PrevToken();
                            break;
                        }

                        exp = new FunctionCall($"{exp.Inline}:{name}", Expression_CallParameters());
                    }
                    else
                    {
                        PrevToken();
                        break;
                    }
                }
            }

            return exp;
        }

        Instruction Expression13_AnonymousFunctions()
        {
            if (AcceptIdentifier())
            {
                var paramName = currentToken.Value;

                if (AcceptSymbol("=>"))
                {
                    return new AnonymousFunction(new List<string>() { paramName }, new PreparedInstruction(Expression().Inline), true);
                }
                else
                    PrevToken();
            }

            // TODO: Anonymous function with more than one parameter

            if (AcceptKeyword("function"))
            {
                RequireSymbol("(");

                List<string> paramList = new List<string>();
                bool comma = true, any = false;
                while (AcceptIdentifier() || AcceptKeyword("this"))
                {
                    any = true;
                    if (!comma)
                        ThrowException("Comma required between function parameters");
                    if (currentToken.Value == "this")
                        currentToken.Value = "self";
                    paramList.Add(currentToken.Value);
                    comma = AcceptSymbol(",");
                }
                if (comma && any)
                    ThrowException("Unexpected comma.");

                RequireSymbol(")");

                return new AnonymousFunction(paramList, Block(false), false);
            }

            return Expression14_GroupedEquation();
        }

        Instruction Expression14_GroupedEquation()
        {
            if (AcceptSymbol("("))
            {
                var exp = Expression();
                RequireSymbol(")", "Right parenthesis ')' missing, to close grouped equation.");
                return new Expression($"({exp.Inline})");
            }

            return Expression15_Tables();
        }

        Instruction Expression15_Tables()
        {
            if (AcceptSymbol("{"))
            {
                List<Tuple<Instruction, Instruction>> pairs = new List<Tuple<Instruction, Instruction>>();
                bool comma = true;
                while (!AcceptSymbol("}"))
                {
                    if (!comma)
                        ThrowException("Comma required between table keys.");

                    if (AcceptSymbol("["))
                    {
                        var key = Expression();

                        RequireSymbol("]", "Right square bracket (]) missing, to close table key.");
                        RequireSymbol("=");

                        pairs.Add(new Tuple<Instruction, Instruction>(key, Expression()));
                    }
                    else
                    {
                        if (AcceptIdentifier())
                        {
                            var key = currentToken.Value;

                            if (AcceptSymbol("="))
                            {
                                pairs.Add(new Tuple<Instruction, Instruction>(new Expression($"\"{key}\""), Expression()));
                                comma = AcceptSymbol(",");
                                continue;
                            }
                            else
                                PrevToken();
                        }

                        pairs.Add(new Tuple<Instruction, Instruction>(null, Expression()));
                    }

                    comma = AcceptSymbol(",");
                }

                return new Table(pairs);
            }

            return Expression16_RawValuesAndVariables();
        }

        Instruction Expression16_RawValuesAndVariables()
        {
            if (AcceptKeyword("true", "false") || AcceptNumber())
                return new Expression(currentToken.Value);
            if (AcceptSymbol("..."))
                return new Expression("..."); // TODO: Move ... above, we don't want to be able to make equations with it. 
            if (AcceptString())
                return new Expression($"\"{currentToken.Value}\"");
            if (AcceptKeyword("this"))
                return new Expression("self");
            if (AcceptKeyword("null"))
                return new Expression("nil");
            if (AcceptIdentifier())
                return new Expression(currentToken.Value);

            return InvalidExpression();
        }

        Instruction InvalidExpression()
        {
            if (NextToken())
                ThrowException($"Unexpected token '{currentToken.Value}'.");
            else
                ThrowException($"Unexpected end of file.");

            return null;
        }

        /// <summary>
        /// Compiles single raw value expression.
        /// </summary>
        /// <returns>Expression</returns>
        Instruction Expression_RawValue()
        {
            if (AcceptKeyword("true", "false") || AcceptNumber() || AcceptIdentifier())
                return new Expression(currentToken.Value);
            if (AcceptString())
                return new Expression($"\"{currentToken.Value}\"");
            if (AcceptKeyword("this"))
                return new Expression("self");
            if (AcceptKeyword("null"))
                return new Expression("nil");

            ThrowException("Value expected.");
            return null;
        }

        /// <summary>
        /// Compiles variable or table reference.
        /// </summary>
        Instruction Expression_VariableOrTableVariable()
        {
            var startIndex = tokenIndex;

            var exp = Expression();

            if (currentToken.Value == "]")
                return exp;

            PrevToken();
            if (tokenIndex > startIndex && (currentToken.Value == "." || currentToken.Value == ")"))
            {
                NextToken();
                return exp;
            }

            CancelPreparation();
            tokenIndex = startIndex;

            return Expression_RawValue();
        }

        Instruction Expression_FunctionCall()
        {
            var startIndex = tokenIndex;

            var exp = Expression();

            if (currentToken.Value == ")")
            {
                //PrevToken();
                //if (currentToken.Value != ")")
                //{
                //    NextToken();
                return exp; // TODO: Check this for grouped equations
                //}
            }

            tokenIndex = startIndex;

            return new Expression(RequireIdentifier("Function call expected.").Value);
        }

        /// <summary>
        /// Compiles and returns a list of function call parameters. Parentheses are parsed automatically.
        /// </summary>
        /// <returns>List of compiled expressions</returns>
        List<Instruction> Expression_CallParameters()
        {
            RequireSymbol("(");

            List<Instruction> paramList = new List<Instruction>();
            bool comma = true, any = false;
            while (!AcceptSymbol(")"))
            {
                any = true;
                if (!comma)
                    ThrowException("Comma required between function parameters");
                paramList.Add(Expression());
                comma = AcceptSymbol(",");
            }
            if (comma && any)
                ThrowException("Unexpected comma.");

            return paramList;
        }
    }
}
