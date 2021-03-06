﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaAdvanced.Compiler.Lexer;

namespace LuaAdvanced.Compiler
{
    public class Compiler
    {
        public string Comment { get; set; } = "--[[\n\tCompiled using LuaAdvanced\n\tThis file should not be modified\n]]\n";
        public Dictionary<string, string> Directives { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Compiles input string and returns the final code.
        /// </summary>
        /// <param name="input">LuaAdvanced code</param>
        /// <returns>Compiled Lua code</returns>
        public string Compile(string input)
        {
            try
            {
                Preprocessor.Preprocessor pre = new Preprocessor.Preprocessor(input, Directives);

                var tokens = new Lexer.Lexer(pre.output, pre.data).OutputTokens;
                foreach (var token in tokens)
                {
                    Console.WriteLine($"{token.Type}\t - \t{token.Value}");
                }

                var parser = new Parser.Parser(tokens.ToArray(), Comment);
                Console.WriteLine(parser.Output);
                return parser.Output;
            }
            catch (CompilerException e)
            {
                Console.WriteLine($"{e.Line}:{e.Position}\t-\t{e.Message}");
                Console.WriteLine(e.StackTrace);
                throw e;
            }
        }
    }
}
