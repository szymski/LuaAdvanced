using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class NullPropagation : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public NullPropagation(Instruction first, Instruction second)
        {
            Inline = $"({first.Inline} or {second.Inline})";
        }

    }
}
