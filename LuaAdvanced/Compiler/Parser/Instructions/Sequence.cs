using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class Sequence : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        static string AddTabs(string str, int count)
        {
            var lines = str.Split('\n');

            for (int j = 0; j < count-1; j++)
                for (int i = 0; i < lines.Length; i++)
                    lines[i] = "\t" + lines[i];

            return lines.Aggregate((i, j) => i + "\n" + j);
        }

        public Sequence(List<Instruction> instructions)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var instruction in instructions)
            {
                builder.AppendLine(AddTabs(instruction.Prepared, Parser.Instance.blockDepth));
            }

            Prepared = builder.ToString();
        }
    }
}
