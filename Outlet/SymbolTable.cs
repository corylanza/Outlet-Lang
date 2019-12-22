using Outlet.Checking;
using Outlet.Operands;
using Outlet.Types;
using System.Collections.Generic;

namespace Outlet
{
    public class SymbolTable
    {
        public static SymbolTable Global = new SymbolTable();

        private readonly Dictionary<string, ITyped> Symbols = new Dictionary<string, ITyped>();
        public readonly SymbolTable Parent;

        private SymbolTable()
        {
            Parent = null;
            foreach (string s in ForeignFunctions.NativeFunctions.Keys)
            {
                Function f = ForeignFunctions.NativeFunctions[s];
                Define(f.GetOutletType(), s);
            }
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
                if (Symbols[s] is FunctionType existingFunc && t is FunctionType newFunc)
                {
                    Symbols[s] = new MethodGroupType(existingFunc, newFunc);
                }
                else if (Symbols[s] is MethodGroupType existing && t is FunctionType added)
                {
                    existing.Methods.Add(added);
                }
                else Checker.Error("variable " + s + " already defined in this scope");
            }
            else Symbols[s] = t;
        }

        public (ITyped, int) Find(string s)
        {
            if (Symbols.ContainsKey(s)) return (Symbols[s], 0);
            if (Parent != null)
            {
                (ITyped type, int level) = Parent.Find(s);
                // if not found in parent scope pass along not found (-1), otherwise add 1 level
                return (type, level == -1 ? -1 : level + 1);
            }
            return (null, -1);
        }

        public IEnumerable<(string ID, ITyped Type)> List()
        {
            foreach (string id in Symbols.Keys)
            {
                var type = Symbols[id];
                yield return (id, type);
            }
        }

        public ITyped GetType(int level, string s)
        {
            if (level == 0 && !Symbols.ContainsKey(s)) throw new CheckerException("failed to get type");
            if (level == 0) return Symbols[s];
            else return Parent.GetType(level - 1, s);
        }

        public ITyped this[string id] {
            get {
                return Symbols[id];
            }
            set {
                Symbols[id] = value;
            }
        }

    }
}

        //public void Add(string id, Type type)
        //{
        //    Action action = (Symbols.ContainsKey(id), Symbols[id].Type, type) switch
        //    {
        //        (true, FunctionType first, FunctionType second) => () => Symbols[id] = (new MethodGroupType(first, second), null),
        //        (true, MethodGroupType group, FunctionType added) => () => group.Methods.Add(added),
        //        (true, _, _) => throw new CheckerException("Already defined"),
        //        _ => () => Symbols.Add(id, (type, null))
        //    };
        //    action();
        //}
