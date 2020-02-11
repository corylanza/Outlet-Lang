using Outlet.Checking;
using Outlet.Operands;
using Outlet.Types;
using System;
using System.Linq;
using System.Collections.Generic;
using Type = Outlet.Types.Type;

namespace Outlet.Checking
{
    public class SymbolTable
    {
        public static SymbolTable Global = new SymbolTable();

        private readonly Dictionary<string, (ITyped type, int id)> Symbols = new Dictionary<string, (ITyped, int)>();
        public readonly SymbolTable Parent;
        private readonly bool newStackFrame;

        private SymbolTable()
        {
            Parent = null;
            newStackFrame = true;
            int index = 0;
            foreach (string s in ForeignFunctions.NativeTypes.Keys)
            {
                Type t = ForeignFunctions.NativeTypes[s];
                Symbols[s] = (new TypeObject(t), index++);
            }
        }

        public SymbolTable(SymbolTable parent, bool enterStackFrame)
        {
            Parent = parent;
            newStackFrame = enterStackFrame;
        }

        public void Define(ITyped type, IBindable decl, Func<int> getNextId)
        {
            if (Symbols.ContainsKey(decl.Identifier))
            {
                (ITyped existing, int existingId) = Symbols[decl.Identifier];
                int newId = getNextId();
                if (existing is FunctionType existingFunc && type is FunctionType newFunc)
                {
                    Symbols[decl.Identifier] = (new MethodGroupType((existingFunc, existingId), (newFunc, newId)), existingId);
                    decl.Bind(newId, 0);
                }
                else if (existing is MethodGroupType existingMethodGroup && type is FunctionType added)
                {
                    existingMethodGroup.Methods.Add((added, newId));
                    decl.Bind(newId, 0);
                }
                else Checker.Error("variable " + decl.Identifier + " already defined in this scope");
            }
            else
            {
                decl.Bind(getNextId(), 0);
                Symbols[decl.Identifier] = (type, decl.LocalId);
            }
        }

        public (ITyped type, int level, int localId) Resolve(IBindable variable)
        {
            if (Symbols.ContainsKey(variable.Identifier))
            {
                (ITyped type, int id) = Symbols[variable.Identifier];
                return (type, 0, id);
            }
            if (Parent != null)
            {
                (ITyped type, int level, int id) = Parent.Resolve(variable);
                // if not found in parent scope pass along not found (-1), otherwise add 1 level
                return (type, level == -1 ? -1 : newStackFrame ? level + 1 : level, id);
            }
            return (null, -1, -1);
        }

        public IEnumerable<(string Id, ITyped Type)> List() => Symbols.Select(x => (x.Key, x.Value.type));
        
        public bool Has(string s) => Symbols.ContainsKey(s);

        public ITyped GetType(IBindable variable, int level = 0)
        {
            var id = variable.Identifier;
            if (variable.ResolveLevel == 0 && Symbols.ContainsKey(id)) return Symbols[id].type;
            else if (Parent != null) return Parent.GetType(variable, newStackFrame ? level + 1 : level);
            else return Checker.Error($"failed to get type of {id}");
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