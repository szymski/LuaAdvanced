using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser
{
    class ParserException : CompilerException
    {
        public ParserException(string message, int line, int position) : base(message, line, position)
        {
        }
    }
}
