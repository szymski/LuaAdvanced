using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class NewVariable : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public NewVariable(List<string> variableList, List<Instruction> valueList)
        {
            if(valueList.Count > 0)
            Prepared = $"local {variableList.Aggregate((i, j) => $"{i}, {j}")} = {valueList.Select(v => v.Inline).Aggregate((i, j) => $"{i}, {j}")}";
            else
                Prepared = $"local {variableList.Aggregate((i, j) => $"{i}, {j}")}";
        }

    }
}
