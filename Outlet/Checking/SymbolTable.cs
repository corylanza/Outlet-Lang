using Outlet.Checking;
using Outlet.Operands;
using Outlet.Types;
using System;
using System.Collections.Generic;
using Type = Outlet.Types.Type;

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
            int index = 0;
            foreach (string s in ForeignFunctions.NativeTypes.Keys)
            {
                Type t = ForeignFunctions.NativeTypes[s];
                Symbols[s] = (new TypeObject(t), index++);
            }
        }

        public SymbolTable(SymbolTable parent)
        {
            Parent = parent;
        }

        public int Define(ITyped t, string s)
        {
            if (Symbols.ContainsKey(s))
            {
                if (Symbols[s].type is FunctionType existingFunc && t is FunctionType newFunc)
                {
                    Symbols[s] = (new MethodGroupType(existingFunc, newFunc), Symbols[s].id);
                    return Symbols[s].id;
                }
                else if (Symbols[s].type is MethodGroupType existing && t is FunctionType added)
                {
                    existing.Methods.Add(added);
                    return Symbols[s].id;
                }
                else Checker.Error("variable " + s + " already defined in this scope");
            }
            else Symbols[s] = (t, Symbols.Count);
            return Symbols.Count;
        }

        public void Define(ITyped type, IDeclarable decl, Func<int> getNextId)
        {
            if (Symbols.ContainsKey(decl.Identifier))
            {
                int existingId = Symbols[decl.Identifier].id;
                if (Symbols[decl.Identifier].type is FunctionType existingFunc && type is FunctionType newFunc)
                {
                    Symbols[decl.Identifier] = (new MethodGroupType(existingFunc, newFunc), existingId);
                    decl.LocalId = existingId;
                }
                else if (Symbols[decl.Identifier].type is MethodGroupType existing && type is FunctionType added)
                {
                    existing.Methods.Add(added);
                    decl.LocalId = existingId;
                }
                else Checker.Error("variable " + decl.Identifier + " already defined in this scope");
            }
            else Symbols[decl.Identifier] = (type, decl.LocalId = getNextId());
        }

        public (ITyped, int, int) ResolveAndBind(IVariable variable)
        {
            if (Symbols.ContainsKey(variable.Identifier))
            {
                (ITyped type, int id) = Symbols[variable.Identifier];
                return (type, 0, id);
            }
            if (Parent != null)
            {
                (ITyped type, int level, int id) = Parent.ResolveAndBind(variable);
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