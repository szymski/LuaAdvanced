using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class PreparedInstruction : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public PreparedInstruction(string prepared)
        {
            Prepared = prepared;
        }
    }
}
