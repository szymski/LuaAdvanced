using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class VariableModify : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public VariableModify(List<string> variableList, List<Instruction> valueList)
        {
            Prepared = $"{variableList.Aggregate((i,j)=>$"{i}, {j}")} = {valueList.Select(v => v.Inline).Aggregate((i, j) => $"{i}, {j}")}";
        }

    }
}
