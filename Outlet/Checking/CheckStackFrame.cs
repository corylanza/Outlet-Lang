using Outlet.Checking;
using Outlet.Operands;
using Outlet.Types;
using System;
using System.Linq;
using System.Collections.Generic;
using Type = Outlet.Types.Type;
using SymbolTable = System.Collections.Generic.Dictionary<string, (Outlet.Types.Type Type, uint Id)>;

namespace Outlet.Checking
{
    public class CheckStackFrame : IStackFrame<Type>
    {
        public static CheckStackFrame Global(Func<string, Error> errorHandler) => new CheckStackFrame(errorHandler);

        private readonly CheckStackFrame? Parent;
        private readonly Stack<SymbolTable> Scopes = new Stack<SymbolTable>();
        public uint Count { get; private set; }
        private readonly Func<string, Error> CheckingError;

        public void EnterScope() => Scopes.Push(new Dictionary<string, (Type type, uint id)>());
        public void ExitScope() => Scopes.Pop();

        private CheckStackFrame(Func<string, Error> errorHandler) : this(null, errorHandler)
        {
            foreach (string s in NativeOutletTypes.NativeTypes.Keys)
            {
                Type t = NativeOutletTypes.NativeTypes[s];
                Scopes.Peek()[s] = (new MetaType(t), Count++);
            }
        }

        public CheckStackFrame(CheckStackFrame? parent, Func<string, Error> errorHandler)
        {
            Count = 0;
            Parent = parent;
            CheckingError = errorHandler;
            EnterScope();
        }

        public void Assign(IBindable decl, Type type)
        {
            if (Scopes.Peek().ContainsKey(decl.Identifier))
            {
                (Type existing, uint existingId) = Scopes.Peek()[decl.Identifier];
                uint newId = Count++;
                if (existing is FunctionType existingFunc && type is FunctionType newFunc)
                {
                    Scopes.Peek()[decl.Identifier] = (new MethodGroupType((existingFunc, existingId), (newFunc, newId)), existingId);
                    decl.Bind(newId, 0);
                }
                else if (existing is MethodGroupType existingMethodGroup && type is FunctionType added)
                {
                    existingMethodGroup.AddMethod(added, newId);
                    decl.Bind(newId, 0);
                }
                else CheckingError("variable " + decl.Identifier + " already defined in this scope");
            }
            else
            {
                decl.Bind(Count++, 0);
                if(decl.LocalId.HasValue)
                {
                    Scopes.Peek()[decl.Identifier] = (type, decl.LocalId.Value);
                }
            }
        }

        public bool Resolve(IBindable variable, out Type type, out uint resolveLevel, out uint localId)
        {
            // Local variables
            foreach (var scope in Scopes)
            {
                if (scope.ContainsKey(variable.Identifier))
                {
                    (type, localId) = scope[variable.Identifier];
                    resolveLevel = 0;
                    return true;
                }
            }
            // Global variables, closures and static or instance members
            if (Parent != null)
            {
                // pass along results from parent scope, add 1 level if found
                bool found = Parent.Resolve(variable, out type, out resolveLevel, out localId);
                if (found) resolveLevel += 1;
                return found;
            }
            // Not found
            type = CheckingError($"variable {variable.Identifier} could not be resolved");
            (resolveLevel, localId) = (0, 0);
            return false;
        }

        public IEnumerable<(string Id, Type Value)> List() => Scopes.Peek().Select(x => (x.Key, x.Value.Type));
        
        public bool Has(string s) => Scopes.Peek().ContainsKey(s);

        public Type Get(IBindable variable) => Get(variable, level: 0);

        public Type Get(IBindable variable, uint level = 0)
        {
            string id = variable.Identifier;
            if (variable.ResolveLevel < 0) return CheckingError($"variable {id} has not been resolved");
            if(level == variable.ResolveLevel)
            {
                foreach(var scope in Scopes)
                {
                    if (scope.ContainsKey(id)) return scope[id].Type;
                }
                return CheckingError($"could not get type of {id}");
            }
            if (level < variable.ResolveLevel && Parent != null) return Parent.Get(variable, level + 1);
            return CheckingError($"variable {id} is defined at a stack frame that could not be found");
        }
    }
}