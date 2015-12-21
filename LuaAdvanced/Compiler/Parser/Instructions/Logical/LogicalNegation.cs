using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class LogicalNegation : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public LogicalNegation(Instruction exp, int negationCount = 1)
        {
            string output = exp.Inline;

            for (int i = 0; i < negationCount; i++)
                output = $"not ({output})";

            Inline = output;
        }

    }
}
