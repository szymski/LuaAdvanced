using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class BitwiseNot : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public BitwiseNot(Instruction left, Instruction right)
        {
            Inline = $"bit.bnot({left.Inline}, {right.Inline})";
        }

    }
}
