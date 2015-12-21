using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler
{
    public class CompilerException : Exception
    {
        public override string Message { get; }
        public int Line { get; }
        public int Position { get; }

        public CompilerException(string message, int line, int position)
        {
            Message = message;
            Line = line;
            Position = position;
        }
    }
}
