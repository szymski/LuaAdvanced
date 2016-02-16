using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaAdvanced.Compiler;

namespace Test
{
    class Program
    {
        static void WriteLogo(string text)
        {
            var defaultColor = Console.ForegroundColor;
            var defaultBgColor = Console.BackgroundColor;

            foreach (var c in text.Split('\r').SelectMany(l => l))
            {
                if (c == '$')
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.Write("█");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.BackgroundColor = defaultBgColor;
                    Console.Write(c);
                }
            }

            Console.WriteLine();

            Console.ForegroundColor = defaultColor;
            Console.BackgroundColor = defaultBgColor;
        }

        static void Main(string[] args)
        {
            string welcome = @"
 /$$                            /$$$$$$        /$$                                                          /$$
| $$                           /$$__  $$      | $$                                                         | $$
| $$       /$$   /$$  /$$$$$$ | $$  \ $$  /$$$$$$$ /$$    /$$  /$$$$$$  /$$$$$$$   /$$$$$$$  /$$$$$$   /$$$$$$$
| $$      | $$  | $$ |____  $$| $$$$$$$$ /$$__  $$|  $$  /$$/ |____  $$| $$__  $$ /$$_____/ /$$__  $$ /$$__  $$
| $$      | $$  | $$  /$$$$$$$| $$__  $$| $$  | $$ \  $$/$$/   /$$$$$$$| $$  \ $$| $$      | $$$$$$$$| $$  | $$
| $$      | $$  | $$ /$$__  $$| $$  | $$| $$  | $$  \  $$$/   /$$__  $$| $$  | $$| $$      | $$_____/| $$  | $$
| $$$$$$$$|  $$$$$$/|  $$$$$$$| $$  | $$|  $$$$$$$   \  $/   |  $$$$$$$| $$  | $$|  $$$$$$$|  $$$$$$$|  $$$$$$$
|________/ \______/  \_______/|__/  |__/ \_______/    \_/     \_______/|__/  |__/ \_______/ \_______/ \_______/
                                                                                                                                                                                                                                                                                                                                           
";

            WriteLogo(welcome);

            new Compiler().Compile(@"

partial class Dupsko {
    
    var gay = 123;

    function Gay() {
        print(""I am gay func "" .. this.gay);    
    }
}

partial class Dupsko {
    var derp = 666;

    function Derp() {
        print(""I am derp func "" .. this.derp);  
    }
}

var obj = Dupsko();
obj:Gay();
obj:Derp();

".Replace("\r", ""));
            Console.ReadKey();
        }
    }
}
