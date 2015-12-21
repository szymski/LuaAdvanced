using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaAdvanced.Compiler.Lexer;

namespace LuaAdvanced.Compiler.Parser
{
    partial class Parser
    {
        Token[] tokens;

        Token currentToken = null;
        int tokenIndex = -1;

        bool NextToken()
        {
            tokenIndex++;
            if (tokenIndex >= tokens.Length)
                return false;
            currentToken = tokens[tokenIndex];
            Debug.WriteLine($"Current token: {currentToken.Value}");
            return true;
        }

        void PrevToken()
        {
            tokenIndex--;
            if(tokenIndex<0)
                return;
            Debug.WriteLine($"Current token: {currentToken.Value}");
            currentToken = tokens[tokenIndex];
        }

        bool AcceptToken(TokenType type, string value)
        {
            if (tokenIndex + 1 >= tokens.Length || tokens[tokenIndex + 1].Type != type || tokens[tokenIndex + 1].Value != value)
                return false;
            currentToken = tokens[++tokenIndex];
            return true;
        }

        bool AcceptToken(TokenType type)
        {
            if (tokenIndex + 1 >= tokens.Length || tokens[tokenIndex + 1].Type != type)
                return false;
            currentToken = tokens[++tokenIndex];
            return true;
        }

        bool AcceptKeyword(params string[] values)
        {
            foreach (var value in values)
                if(AcceptKeyword(value)) return true;
            return false;
        }

        bool AcceptSymbol(params string[] values)
        {
            foreach (var value in values)
                if (AcceptSymbol(value)) return true;
            return false;
        }

        bool AcceptSymbolExcept(params string[] except)
        {
            if (AcceptSymbol())
            {
                if (except.Contains(currentToken.Value))
                {
                    PrevToken();
                    return false;
                }
                return true;
            }
            return false;
        }

        bool AcceptKeyword(string value) => AcceptToken(TokenType.Keyword, value);
        bool AcceptSymbol(string value) => AcceptToken(TokenType.Symbol, value);

        bool AcceptIdentifier() => AcceptToken(TokenType.Identifier);
        bool AcceptString() => AcceptToken(TokenType.String);
        bool AcceptNumber() => AcceptToken(TokenType.Number);

        Token RequireSymbol(string value, string message)
        {
            if (!AcceptSymbol(value))
                ThrowException(message, currentToken.Line, currentToken.Position + currentToken.Value.Length);
            return currentToken;
        }

        Token RequireSymbol(string value)
        {
            if (!AcceptSymbol(value))
                ThrowException($"({value}) expected.", currentToken.Line, currentToken.Position+currentToken.Value.Length);
            return currentToken;
        }

        Token RequireKeyword(string value, string message)
        {
            if (!AcceptKeyword(value))
                ThrowException(message, currentToken.Line, currentToken.Position + currentToken.Value.Length);
            return currentToken;
        }

        Token RequireKeyword(string value)
        {
            if (!AcceptKeyword(value))
                ThrowException($"({value}) expected.", currentToken.Line, currentToken.Position + currentToken.Value.Length);
            return currentToken;
        }

        Token RequireIdentifier(string message)
        {
            if (!AcceptIdentifier())
                ThrowException(message, currentToken.Line, currentToken.Position + currentToken.Value.Length);
            return currentToken;
        }

        Token RequireIdentifier()
        {
            if (!AcceptIdentifier())
                ThrowException($"Identifier expected.", currentToken.Line, currentToken.Position + currentToken.Value.Length);
            return currentToken;
        }
    }
}
