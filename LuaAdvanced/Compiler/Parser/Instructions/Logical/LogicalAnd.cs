using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class LogicalAnd : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public LogicalAnd(Instruction left, Instruction right)
        {
            Inline = $"({left.Inline} and {right.Inline})";
        }

    }
}
