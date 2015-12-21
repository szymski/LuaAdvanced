using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Lexer
{
    enum TokenType
    {
        Keyword,
        Symbol,
        Identifier,
        String,
        Number
    }

    class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public int Line { get; set; }
        public int Position { get; set; }
    }
}
