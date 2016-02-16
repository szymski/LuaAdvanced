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

        public Class(string name, Dictionary<string, Instruction> fields, List<Parser.ClassMethod> methods, string baseClass, bool local, bool partial)
        {
            string codeClassName = $"C{name}";

            string partialCheck = "", partialCheckEnd = "", partialConstructorCheck = "";

            if (partial)
            {
                partialCheck = $"if not {codeClassName} then";
                partialCheckEnd = "end";
                partialConstructorCheck = $"if not {name} then";
            }

            string metatable = $@"-- {name} class metatable
{(local ? "local " : "")}{codeClassName} = {{ }}
{codeClassName}.__index = {codeClassName}
{codeClassName}.__type = ""{(name == "LUAA_Object" ? "Object" : name)}""
{codeClassName}.__baseclasses = {{ }}
{codeClassName}.__initializers = {{ }}
";

            if (name != "LUAA_Object")
                metatable += $"luaa.Inherit({codeClassName}, C{(string.IsNullOrEmpty(baseClass) ? "LUAA_Object" : baseClass)})";

            string internalConstructor = $@"function {codeClassName}:__new(...)
    for k, v in pairs(self.__initializers) do
        v(self)
    end
    {(methods.Any(m => m.name == name) ? $"self:{name}(...)" : "")}
end";

            string initializer = $@"{codeClassName}.__initializers[#{codeClassName}.__initializers + 1] = function(self)
    {Fields(fields)}
end";

            string metamethods = Metamethods(methods, codeClassName);

            string constructor = $@"function {name}(...)
    local tbl = {{ }}
    setmetatable(tbl, {codeClassName})
    tbl:__new(...);
    return tbl
end";

            Prepared = $"{partialCheck}\n{metatable}\n{partialCheckEnd}\n{metamethods}\n{initializer}\n{partialConstructorCheck}\n{internalConstructor}\n{constructor}\n{partialCheckEnd}";
        }

    }
}
