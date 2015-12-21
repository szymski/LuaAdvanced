using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class LogicalOr : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public LogicalOr(Instruction left, Instruction right)
        {
            Inline = $"({left.Inline} or {right.Inline})";
        }

    }
}
