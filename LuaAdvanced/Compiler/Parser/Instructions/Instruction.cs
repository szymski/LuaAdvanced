using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    abstract class Instruction
    {
        public abstract string Prepared { get; }
        public abstract string Inline { get; }
    }
}
