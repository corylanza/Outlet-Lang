using Outlet.Checking;
using Outlet.Operands;
using Outlet.Types;
using System.Collections.Generic;

namespace Outlet.Checking
{
    public class SymbolTable
    {
        public static SymbolTable Global = new SymbolTable();

        private readonly Dictionary<string, (ITyped type, int id)> Symbols = new Dictionary<string, (ITyped, int)>();
        public readonly SymbolTable Parent;

        private SymbolTable()
        {
            Parent = null;
            foreach (string s in ForeignFunctions.NativeTypes.Keys)
            {
                Type t = ForeignFunctions.NativeTypes[s];
                Define(new TypeObject(t), s);
            }
        }

        public SymbolTable(SymbolTable parent)
        {
            Parent = parent;
        }

        public void Define(ITyped t, string s)
        {
            if (Symbols.ContainsKey(s))
            {
                if (Symbols[s].type is FunctionType existingFunc && t is FunctionType newFunc)
                {
                    Symbols[s] = (new MethodGroupType(existingFunc, newFunc), Symbols[s].id);
                }
                else if (Symbols[s].type is MethodGroupType existing && t is FunctionType added)
                {
                    existing.Methods.Add(added);
                }
                else Checker.Error("variable " + s + " already defined in this scope");
            }
            else Symbols[s] = (t, Symbols.Count);
        }

        public (ITyped, int, int) Bind(string s)
        {
            if (Symbols.ContainsKey(s)) return (Symbols[s].type, 0, Symbols[s].id);
            if (Parent != null)
            {
                (ITyped type, int level, int id) = Parent.Bind(s);
                // if not found in parent scope pass along not found (-1), otherwise add 1 level
                return (type, level == -1 ? -1 : level + 1, id);
            }
            return (null, -1, -1);
        }

        public IEnumerable<(string ID, ITyped Type)> List()
        {
            foreach (string id in Symbols.Keys)
            {
                var type = Symbols[id].type;
                yield return (id, type);
            }
        }

        public ITyped GetType(int level, string s)
        {
            if (level == 0 && !Symbols.ContainsKey(s)) throw new CheckerException("failed to get type");
            if (level == 0) return Symbols[s].type;
            else return Parent.GetType(level - 1, s);
        }

        public ITyped this[string id] {
            get {
                return Symbols[id].type;
            }
            set {
                Symbols[id] = (value, Symbols.Count);
            }
        }

    }
}