using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class Is : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public Is(Instruction expression, string type)
        {
            if (type == "number" || type == "string" || type == "function" || type == "table")
                Inline = $"type({expression.Inline}) == \"{type}\"";
            else
                Inline = $"luaa.IsSubclassOf({expression.Inline}, \"{type}\")";
        }

    }
}
