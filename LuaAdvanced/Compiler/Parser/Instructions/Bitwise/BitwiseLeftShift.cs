using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class BitwiseLeftShift : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public BitwiseLeftShift(Instruction exp, Instruction shiftCount)
        {
            Inline = $"bit.lshift({exp.Inline}, {shiftCount.Inline})";
        }

    }
}
