using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class Continue : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public Continue(int loopDepth)
        {
            Prepared = $"goto continue_{loopDepth}";
        }
    }
}
