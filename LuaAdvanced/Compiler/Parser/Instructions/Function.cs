using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class Function : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public Function(string name, List<string> parameters, Instruction sequence, bool local)
        {
            Prepared = $@"{(local ? "local " : "")}function {name}({(parameters.Count > 0 ? parameters.Aggregate((i, j) => $"{i}, {j}") : "")})
{sequence.Prepared}
end";
        }

        public Function(string nameWithParameters, Instruction sequence, bool local)
        {
            Prepared = $@"{(local ? "local " : "")}function {nameWithParameters}
{sequence.Prepared}
end";
        }

    }
}
