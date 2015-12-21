using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaAdvanced.Compiler.Lexer;
using LuaAdvanced.Compiler.Parser.Instructions;

namespace LuaAdvanced.Compiler.Parser
{
    partial class Parser
    {
        class Scope
        {
            // TODO: Class scopes
        }

        public int blockDepth = 0;

        public int loopDepth = 0;
        public List<int> loopContinues = new List<int>();

        List<Instruction> statementPreparation = new List<Instruction>(); 
        List<Instruction> statementAfterPreparation = new List<Instruction>();

        void CancelPreparation()
        {
            statementPreparation.Clear();
            statementAfterPreparation.Clear();
        }
    }
}
