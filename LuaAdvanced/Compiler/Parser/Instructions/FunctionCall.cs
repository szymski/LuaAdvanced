using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class FunctionCall : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public FunctionCall(string name, List<Instruction> parameters)
        {
            Inline = $"{name}({(parameters.Count > 0 ? parameters.Select(i => i.Inline).Aggregate((i, j) => i + ", " + j) : "")})";
        }

    }
}
