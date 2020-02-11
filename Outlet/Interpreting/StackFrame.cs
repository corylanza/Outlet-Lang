using Outlet.Operands;
using Outlet.Types;
using System.Collections.Generic;

namespace Outlet.Interpreting
{
    public class StackFrame
    {
        public Operand[] LocalVariables { get; private set; }
        private readonly StackFrame Parent;
        public string Call { get; private set; }

        public static readonly StackFrame Global = new StackFrame();

        private StackFrame()
        {
            Call = "Global scope";
            int index = 0;
            LocalVariables = new Operand[ForeignFunctions.NativeTypes.Count];
            foreach(var type in ForeignFunctions.NativeTypes.Values)
            {
                LocalVariables[index++] = new TypeObject(type);
            }
        }

        public StackFrame(StackFrame parent, int localCount, string call)
        {
            Parent = parent;
            LocalVariables = new Operand[localCount];
            Call = call;
        }

        public Operand Get(IBindable variable, int level = 0)
        {
            if (variable.ResolveLevel > level) return Parent.Get(variable, level + 1);
            return LocalVariables[variable.LocalId];
        }

        public void Assign(IBindable variable, Operand value, int level = 0)
        {
            if (this == Global) 
            {
                Operand[] newGlobals = new Operand[LocalVariables.Length + 1];
                System.Array.Copy(LocalVariables, newGlobals, LocalVariables.Length);
                LocalVariables = newGlobals;
            }
            if (variable.ResolveLevel > level) Parent.Assign(variable, value, level + 1);
            else LocalVariables[variable.LocalId] = value;
        }

        public IEnumerable<(Operand, Type)> ListVariables()
        {
            foreach (var op in LocalVariables) yield return (op, op.GetOutletType());
        }
    }
}
