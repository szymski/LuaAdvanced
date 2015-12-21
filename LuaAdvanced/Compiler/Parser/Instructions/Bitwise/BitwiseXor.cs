using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class BitwiseXor : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public BitwiseXor(Instruction left, Instruction right)
        {
            Inline = $"bit.bxor({left.Inline}, {right.Inline})";
        }

    }
}
