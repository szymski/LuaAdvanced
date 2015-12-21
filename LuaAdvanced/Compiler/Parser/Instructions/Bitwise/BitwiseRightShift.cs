using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class BitwiseRightShift : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public BitwiseRightShift(Instruction exp, Instruction shiftCount)
        {
            Inline = $"bit.rshift({exp.Inline}, {shiftCount.Inline})";
        }

    }
}
