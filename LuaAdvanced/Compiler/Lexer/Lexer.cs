using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Lexer
{
    partial class Lexer
    {
        void Pass()
        {
            while (NextChar())
            {
                if (!SkipWhitespace())
                    break;

                // Keywords and identifiers
                if (AcceptPattern(@"[a-zA-Z_][a-zA-Z0-9_]*"))
                    if (preprocessorData.replacements.Any(r => r.line < line && r.identifier == patternMatch.Value))
                    {
                        var replacement = preprocessorData.replacements.First(r => r.line < line && r.identifier == patternMatch.Value);
                        var endPos = position;
                        PreviousPattern();
                        var newLine = currentLine.Substring(0, position) + replacement.replacement + currentLine.Substring(endPos+1);
                        inputLines[line] = newLine;
                        currentLine = newLine;
                        position--;
                    }
                    else
                        PushToken(LanguageSpecification.IsKeyword(patternMatch.Value) ? TokenType.Keyword : TokenType.Identifier, patternMatch.Value);

                // Hex number
                else if (AcceptPattern(@"0x([0-9a-fA-F]+)"))
                    PushToken(TokenType.Number, Convert.ToInt32(patternMatch.Groups[1].Value, 16).ToString());

                // Numbers
                else if (AcceptPattern(@"[0-9]+\.[0-9]+") || AcceptPattern(@"[0-9]+"))
                    PushToken(TokenType.Number, patternMatch.Value);

                // Multi-line strings
                else if (AcceptPattern("@\""))
                {
                    string str = "";
                    int startLine = line, lastLine = line;
                    while (NextChar() && currentChar != '"')
                    {
                        if (line != lastLine)
                            str += "\\n";

                        if (currentChar == '\\' && NextChar() && currentChar == '"')
                            str += "\\";

                        str += currentChar;

                        lastLine = line;
                    }
                    if (!AcceptPattern("\""))
                        ThrowException("Unexpected end of file.");

                    PushToken(TokenType.String, str);
                }

                // Single-line strings
                else if (AcceptPattern("\""))
                {
                    string str = "";
                    int startLine = line;
                    while (NextChar() && currentChar != '"')
                    {
                        if(currentChar == '\\' && NextChar() && currentChar == '"')
                            str += "\\";
                        str += currentChar;
                    }
                    if (!AcceptPattern("\""))
                        ThrowException("Unexpected end of file.");

                    if (line != startLine)
                        ThrowException("Line break in single-line string. For multi-line string use @ prefix.");

                    PushToken(TokenType.String, str);
                }

                // Triple character symbols
                else if (AcceptPattern(@"[^a-zA-Z0-9\s]{3}"))
                    if (LanguageSpecification.IsSymbol(patternMatch.Value))
                        PushToken(TokenType.Symbol, patternMatch.Value);
                    else
                    {
                        PreviousPattern();
                        if (AcceptPattern(@"[^a-zA-Z0-9\s]{2}") && LanguageSpecification.IsSymbol(patternMatch.Value))
                            PushToken(TokenType.Symbol, patternMatch.Value);
                        else
                        {
                            PreviousPattern();
                            if (AcceptPattern(@"[^a-zA-Z0-9\s]") && LanguageSpecification.IsSymbol(patternMatch.Value))
                                PushToken(TokenType.Symbol, patternMatch.Value);
                            else
                                ThrowException($"Unknown symbol ({currentChar}).");
                        }
                    }

                // Double character symbols
                else if (AcceptPattern(@"[^a-zA-Z0-9\s]{2}"))
                    if (LanguageSpecification.IsSymbol(patternMatch.Value))
                        PushToken(TokenType.Symbol, patternMatch.Value);
                    else
                    {
                        PreviousPattern();
                        if (AcceptPattern(@"[^a-zA-Z0-9\s]") && LanguageSpecification.IsSymbol(patternMatch.Value))
                            PushToken(TokenType.Symbol, patternMatch.Value);
                        else
                            ThrowException($"Unknown symbol ({currentChar}).");
                    }

                // Single character symbols
                else if (AcceptPattern(@"[^a-zA-Z0-9\s]") && LanguageSpecification.IsSymbol(patternMatch.Value))
                    PushToken(TokenType.Symbol, patternMatch.Value);

                // Unknown characters or whitespaces - the thing at the top doesn't work properly sometimes
                else if (!AcceptPattern(@"\s+"))
                    ThrowException($"Unknown character ({currentChar}).");
            }
        }
    }
}
