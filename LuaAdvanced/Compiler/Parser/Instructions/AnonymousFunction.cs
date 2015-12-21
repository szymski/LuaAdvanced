using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class AnonymousFunction : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public AnonymousFunction(List<string> parameters, Instruction sequence, bool predicate)
        {
            Inline = $@"function({(parameters.Count > 0 ? parameters.Aggregate((i, j) => $"{i}, {j}") : "")})
{(predicate ? "return " : "")}{sequence.Prepared}
end";
        }

    }
}
