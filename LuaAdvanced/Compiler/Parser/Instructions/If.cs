using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class If : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public If(List<Tuple<Instruction, Instruction>> ifs)
        {
            var builder = new StringBuilder();

            bool first = true;

            int count = 0;
            foreach (var obj in ifs)
            {
                if (obj.Item1 != null)
                {
                    builder.AppendLine($"{(first ? "if" : "elseif")}({obj.Item1.Inline}) then");
                    builder.AppendLine(obj.Item2.Prepared);
                    if (ifs.Count - 1 == count)
                        builder.AppendLine("end");
                }
                else
                {
                    builder.AppendLine($"else");
                    builder.AppendLine(obj.Item2.Prepared);
                    builder.AppendLine("end");
                }

                first = false;
                count++;
            }

            Prepared = builder.ToString();
        }
    }
}
