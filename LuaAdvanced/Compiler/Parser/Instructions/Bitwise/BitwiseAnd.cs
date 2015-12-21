using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class BitwiseAnd : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public BitwiseAnd(Instruction left, Instruction right)
        {
            Inline = $"bit.band({left.Inline}, {right.Inline})";
        }

    }
}
