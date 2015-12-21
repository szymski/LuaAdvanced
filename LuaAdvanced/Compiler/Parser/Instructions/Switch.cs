using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class Switch : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        static int caseCount = 0;

        public Switch(Instruction value, List<Tuple<Instruction, Instruction, bool>> cases)
        {
            string str = "\n";

            for (int i = 0; i < cases.Count; i++)
            {
                var c = cases[i];
                var prev = i > 0 ? cases[i - 1] : null;

                string prep = "";

                // TODO: Gotos' visibility range doesn't allow that

                if (prev != null && !prev.Item3)
                    prep = $"::case_{caseCount}::";

                if (!c.Item3)
                    prep += $"\ngoto case_{++caseCount}";

                if (c.Item1.Inline != "")
                    str += $@"
{(i == 0 ? "if" : "elseif")} {value.Inline} == {c.Item1.Inline} then
{prep}
{c.Item2.Prepared}
{(i == cases.Count - 1 ? "end" : "")}
";
                else
                    str += $@"
else
{prep}
{c.Item2.Prepared}
{(i == cases.Count - 1 ? "end" : "")}
";
            }

            Prepared = str + "\n";
        }
    }
}
