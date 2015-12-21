using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class ForEachLoop : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        public ForEachLoop(string variableName, Instruction table, Instruction sequence)
        {
            string loopContinue = "";

            if (Parser.Instance.loopContinues.Contains(Parser.Instance.loopDepth + 1))
            {
                Parser.Instance.loopContinues.Remove(Parser.Instance.loopDepth + 1);
                loopContinue = $"::continue_{Parser.Instance.loopDepth + 1}::";
            }

            Prepared = $@"
for _, {variableName} in pairs({table.Inline}) do
{sequence.Prepared}
{loopContinue}
end
";
        }

        public ForEachLoop(string keyName, string variableName, Instruction table, Instruction sequence)
        {
            string loopContinue = "";

            if (Parser.Instance.loopContinues.Contains(Parser.Instance.loopDepth + 1))
            {
                Parser.Instance.loopContinues.Remove(Parser.Instance.loopDepth + 1);
                loopContinue = $"::continue_{Parser.Instance.loopDepth + 1}::";
            }

            Prepared = $@"
for {keyName}, {variableName} in pairs({table.Inline}) do
{sequence.Prepared}
{loopContinue}
end
";
        }
    }
}
