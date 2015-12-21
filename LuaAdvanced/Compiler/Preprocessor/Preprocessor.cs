using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Preprocessor
{
    partial class Preprocessor
    {
        public class PreprocessorData
        {
            public class Replacement
            {
                public int line;
                public string identifier;
                public string replacement;
            }

            public List<Replacement> replacements = new List<Replacement>();
        }

        string ifDef = null;

        void ParseDirective(string directive, string[] paramList)
        {
            if (directive == "define")
            {
                if (paramList.Length == 0)
                    ThrowException("Identifier expected.");

                if (paramList.Length == 1)
                    data.replacements.Add(new PreprocessorData.Replacement()
                    {
                        line = line,
                        identifier = paramList[0],
                        replacement = "true"
                    });
                else
                    data.replacements.Add(new PreprocessorData.Replacement()
                    {
                        line = line,
                        identifier = paramList[0],
                        replacement = paramList.Skip(1).Aggregate((i, j) => i + " " + j)
                    });

                // TODO: Macros
            }

            else if (directive == "undef" || directive == "undefine")
            {
                if (paramList.Length == 0)
                    ThrowException("Identifier expected.");

                if (data.replacements.Any(r => r.identifier == paramList[0]))
                    data.replacements.RemoveAll(r => r.identifier == paramList[0]);
                else
                    ThrowException($"Undefined identifier '{paramList[0]}'.");
            }

            else if (directive == "if" || directive == "ifdef")
            {
                if (paramList.Length == 0)
                    ThrowException("Identifier expected.");

                if (!string.IsNullOrEmpty(ifDef))
                    ThrowException("If directive is alerady open.");

                ifDef = paramList[0];

                if (data.replacements.All(r => r.identifier != ifDef))
                {
                    
                }
            }

            else if (directive == "endif")
            {
                if (string.IsNullOrEmpty(ifDef))
                    ThrowException("Unexpected endif.");

                ifDef = null;
            }

            else
                ThrowException($"Unknown preprocessor directive '{directive}'.");
        }

        void Pass()
        {
            while (NextChar())
            {
                // TODO: Line removing while in if

                // Single-line comments
                if (AcceptPattern("//"))
                {
                    inputLines[line] = currentLine.Substring(0, matchPosition);
                    currentLine = inputLines[line];
                }

                // Multi-line comments
                else if (AcceptPattern(@"/\*"))
                {
                    Match match;
                    if ((match = new Regex(@"\*/").Match(currentLine)).Success)
                    {
                        inputLines[line] = currentLine.Substring(0, position - 1);
                        inputLines[line] += currentLine.Substring(match.Index + 2);
                        currentLine = inputLines[line];
                        position = 0;
                        continue;
                    }

                    inputLines[line] = currentLine.Substring(0, matchPosition);
                    int curLine = line;
                    while (NextChar())
                    {
                        if (line != curLine)
                            inputLines[line - 1] = "";
                        if (AcceptPattern(@"\*/"))
                        {
                            break;
                        }
                        curLine = line;
                    }
                    inputLines[line] = currentLine.Substring(position + 1); // BUG: Causes crash at the end of file, when the comment isn't closed
                    currentLine = inputLines[line];
                }

                // Directives
                else if (AcceptPattern("#"))
                {
                    Match m = new Regex(@"\s*#").Match(currentLine);

                    if (m.Success && m.Index == 0)
                    {
                        string[] paramList = new Regex(@"\s+").Split(currentLine.Substring(position + 1));

                        if (paramList.Length == 0)
                            ThrowException("Preprocessor directive name expected.");

                        ParseDirective(paramList[0], paramList.Skip(1).ToArray());

                        inputLines[line] = "";
                        currentLine = inputLines[line];
                    }
                }

                // Strings
                else if (AcceptPattern("\""))
                {
                    while (NextChar())
                    {
                        if (AcceptPattern("\\\\"))
                            NextChar();
                        else if (AcceptPattern("\""))
                            break;
                    }
                }
            }
        }
    }
}
