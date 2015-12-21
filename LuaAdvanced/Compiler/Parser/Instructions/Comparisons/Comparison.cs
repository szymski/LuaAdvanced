using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class Comparison : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public Comparison(Instruction left, Instruction right, string compOperator)
        {
            Inline = $"{left.Inline} {compOperator} {right.Inline}";
        }
    }
}
