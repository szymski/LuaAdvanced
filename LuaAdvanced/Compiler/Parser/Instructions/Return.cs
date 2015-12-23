using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class Return : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public Return(List<Instruction> valueList = null)
        {
            Prepared = valueList == null ? "return" : $"return {valueList.Select(i => i.Inline).Aggregate((i, j) => i + ", " + j)}";
        }
    }
}
