using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class ForLoop : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public ForLoop(Instruction init, Instruction condition, Instruction after, Instruction sequence)
        {
            string loopContinue = "";

            if (Parser.Instance.loopContinues.Contains(Parser.Instance.loopDepth+1))
            {
                Parser.Instance.loopContinues.Remove(Parser.Instance.loopDepth + 1);
                loopContinue = $"::continue_{Parser.Instance.loopDepth + 1}::";
            }

            Prepared = $@"
{init.Prepared}
while {condition.Inline} do
{sequence.Prepared}
{loopContinue}
{after.Prepared}
end
";
        }
    }
}
