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
        Instruction Sequence(params string[] endToken)
        {
            List<Instruction> instructions = new List<Instruction>();

            bool broke = false;
            while (tokenIndex + 1 < tokens.Length)
            {
                if ((AcceptSymbol(endToken) || AcceptKeyword(endToken)))
                {
                    broke = true;
                    break;
                }

                var statement = Statement();

                instructions.AddRange(statementPreparation);
                instructions.Add(statement);
                instructions.AddRange(statementAfterPreparation);

                statementPreparation.Clear();
                statementAfterPreparation.Clear();
            }

            if (!broke)
                ThrowException("Unexpected end of file.");

            var seq = new Sequence(instructions);



            return seq;
        }

        Instruction Sequence(string endSymbol = null)
        {
            blockDepth++;
            List<Instruction> instructions = new List<Instruction>();

            bool broke = false;
            while (tokenIndex + 1 < tokens.Length)
            {
                if (endSymbol != null && AcceptSymbol(endSymbol))
                {
                    broke = true;
                    break;
                }

                var statement = Statement();

                instructions.AddRange(statementPreparation);
                instructions.Add(statement);
                instructions.AddRange(statementAfterPreparation);

                statementPreparation.Clear();
                statementAfterPreparation.Clear();
            }

            if (endSymbol != null && !broke)
                ThrowException("Unexpected end of file.");

            var seq = new Sequence(instructions);

            blockDepth--;

            return seq;
        }

        Instruction Block(bool singleStatement = true)
        {
            if (AcceptSymbol("{"))
            {
                var seq = Sequence("}");
                return seq;
            }

            if (singleStatement) // TODO: Preparation
                return new PreparedInstruction("\t" + Statement().Prepared);

            ThrowException("Block of code expected.");
            return null;
        }

        public string comment = "";

        string Parse()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(comment);
            builder.AppendLine(Sequence().Prepared);
            return builder.ToString();
        }
    }
}
