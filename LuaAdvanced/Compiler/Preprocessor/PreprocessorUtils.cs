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
        string[] inputLines;

        int line = -1, position = 1;

        char currentChar = '\0';
        string currentLine = "";
        Match patternMatch = null;
        int matchPosition = 0;

        public PreprocessorData data = new PreprocessorData();

        struct PatternMatchSave
        {
            public int line, position;
            public Match patternMatch;
        }

        Stack<PatternMatchSave> patternStack = new Stack<PatternMatchSave>();

        public string output = "";

        public Preprocessor(string input)
        {
            inputLines = input.Split('\n');
            Pass();
            output = inputLines.Aggregate((i, j) => i + "\n" + j);
        }

        bool NextChar()
        {
            position++;

            if (position >= currentLine.Length) // Go to next line, if current one has ended
            {
                position = 0;
                do
                {
                    line++;
                } while (line < inputLines.Length && inputLines[line].Length == 0);
                if (line >= inputLines.Length) // Return false at the end of file
                    return false;
                currentLine = inputLines[line];
            }

            currentChar = currentLine[position];
            return true;
        }

        bool NextLine()
        {
            position = 0;
            do
            {
                line++;
            } while (line < inputLines.Length && inputLines[line].Length == 0);
            if (line >= inputLines.Length) // Return false at the end of file
                return false;
            currentLine = inputLines[line];
            currentChar = currentLine[position];
            return true;
        }

        Regex whitespaceRegex = new Regex(@"\S+");

        void SkipWhitespace()
        {
            Match match = null;
            while (!(match = whitespaceRegex.Match(currentLine.Substring(position))).Success)
                NextLine(); // Go to next line, if the current one doesn't have any characters
            position += match.Index - 1;
            NextChar();
        }

        bool AcceptPattern(string pattern)
        {
            var regex = new Regex(pattern);
            matchPosition = position;
            var match = regex.Match(currentLine.Substring(position));
            if (match.Success && match.Index == 0)
            {
                patternStack.Push(new PatternMatchSave()
                {
                    line = line,
                    position = position,
                    patternMatch = patternMatch
                });
                patternMatch = match;
                position += patternMatch.Length - 1;
                return true;
            }
            return false;
        }

        void PreviousPattern()
        {
            var save = patternStack.Pop();
            line = save.line;
            position = save.position;
            patternMatch = save.patternMatch;
        }

        void ThrowException(string message)
        {
            throw new PreprocessorException(message, line + 1, position + 1);
        }
    }
}
