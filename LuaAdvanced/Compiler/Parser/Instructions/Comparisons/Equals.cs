using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class Equals : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public Equals(Instruction left, Instruction right)
        {
            Inline = $"{left.Inline} == {right.Inline}";
        }
    }
}
