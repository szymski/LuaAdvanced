using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Lexer
{
    class LexerException : CompilerException
    {
        public LexerException(string message, int line, int position) : base(message, line, position)
        {       
        }
    }
}
