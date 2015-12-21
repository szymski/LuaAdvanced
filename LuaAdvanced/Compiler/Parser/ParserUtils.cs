using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaAdvanced.Compiler.Lexer;

namespace LuaAdvanced.Compiler.Parser
{
    partial class Parser
    {
        public string Output { get; }

        public Parser(Token[] tokens, string comment)
        {
            Instance = this;
            this.tokens = tokens;
            this.comment = comment;
            Output = Parse();
        }

        enum ExceptionPosition
        {
            TokenBeginning,
            TokenEnd
        }

        void ThrowException(string message, int line, int position)
        {
            throw new ParserException(message, line + 1, position + 1);
        }

        void ThrowException(string message, ExceptionPosition position)
        {
            if (position == ExceptionPosition.TokenBeginning)
                throw new ParserException(message, currentToken?.Line + 1 ?? 0, currentToken?.Position + 1 ?? 0);
            else
                throw new ParserException(message, currentToken?.Line + 1 + (currentToken?.Value.Length ?? 0) ?? 0, currentToken?.Position + 1 ?? 0);
        }

        void ThrowException(string message)
        {
            throw new ParserException(message, currentToken?.Line + 1 ?? 0, currentToken?.Position + 1 ?? 0);
        }

        public static Parser Instance { get; private set; }
    }
}
