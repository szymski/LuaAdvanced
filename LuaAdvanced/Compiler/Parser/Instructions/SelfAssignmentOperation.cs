using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class SelfAssignmentOperation : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public SelfAssignmentOperation(Instruction variable, string operation, string value)
        {
            Prepared = $"{variable.Inline} = {variable.Inline} {operation} {value}";
        }
    }
}
