using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaAdvanced.Compiler.Parser.Instructions
{
    class Class : Instruction
    {
        public override string Prepared { get; }
        public override string Inline { get; }

        string Fields(Dictionary<string, Instruction> fields)
        {
            string preparation = "";
            foreach (var field in fields)
                preparation += $"\tself.{field.Key} = {(field.Value?.Inline ?? "nil")}\n";

            return preparation;
        }

        string Metamethods(List<Parser.ClassMethod> methods, string codeClassName)
        {
            string metamethods = "";

            foreach (var method in methods)
            {
                metamethods += $"function {codeClassName}:{method.name}({(method.paramList.Count > 0 ? method.paramList.Aggregate((i, j) => i + ", " + j) : "")})\n";
                metamethods += method.sequence.Prepared;
                metamethods += "end\n";
            }

            return metamethods;
        }

        public Class(string name, Dictionary<string, Instruction> fields, List<Parser.ClassMethod> methods, string baseClass, bool local)
        {
            string codeClassName = $"C{name}";
            string metatable = $@"-- {name} class metatable
{(local ? "local " : "")}{codeClassName} = {{ }}
{codeClassName}.__index = {codeClassName}
{codeClassName}.__type = ""{(name == "LUAA_Object" ? "Object" : name)}""
{codeClassName}.__baseclasses = {{ }}
";

            if (name != "LUAA_Object")
                metatable += $"luaa.Inherit({codeClassName}, C{(string.IsNullOrEmpty(baseClass) ? "LUAA_Object" : baseClass)})";

            string internalConstructor = $@"function {codeClassName}:__new(...)
    {Fields(fields)}
    {(methods.Any(m => m.name == name) ? $"self:{name}(...)" : "")}
end";

            string metamethods = Metamethods(methods, codeClassName);

            string constructor = $@"function {name}(...)
    local tbl = {{ }}
    setmetatable(tbl, {codeClassName})
    tbl:__new(...);
    return tbl
end";

            Prepared = $"{metatable}\n{metamethods}\n{internalConstructor}\n{constructor}";
        }

    }
}
