using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class Table : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public Table(List<Tuple<Instruction, Instruction>> pairs)
        {
            Inline = "{ ";

            foreach (var pair in pairs)
                if (pair.Item1 != null)
                    Inline += $"[{pair.Item1.Inline}] = {pair.Item2.Inline}, ";
                else
                    Inline += $"{pair.Item2.Inline}, ";

            Inline += "}";
        }

    }
}
