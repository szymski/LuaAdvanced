using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class Ternary : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public Ternary(Instruction condition, Instruction trueExp, Instruction falseExp)
        {
            Inline = $"({condition.Inline} and {trueExp.Inline} or {falseExp.Inline})";
        }

    }
}
