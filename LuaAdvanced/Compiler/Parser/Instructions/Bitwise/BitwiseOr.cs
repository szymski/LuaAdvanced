using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class BitwiseOr : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public BitwiseOr(Instruction left, Instruction right)
        {
            Inline = $"bit.bor({left.Inline}, {right.Inline})";
        }

    }
}
