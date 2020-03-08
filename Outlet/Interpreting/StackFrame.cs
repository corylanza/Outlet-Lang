using Outlet.Operands;
using Outlet.Types;
using System.Collections.Generic;

namespace Outlet.Interpreting
{
    public class StackFrame : IStackFrame<Operand>
    {
        public (string Id, Operand Value)[] LocalVariables { get; private set; }
        private readonly StackFrame? Parent;
        public string Call { get; private set; }

        public static readonly StackFrame Global = new StackFrame();

        private StackFrame()
        {
            Call = "Global scope";
            int index = 0;
            LocalVariables = new (string, Operand)[ForeignFunctions.NativeTypes.Count];
            foreach(string type in ForeignFunctions.NativeTypes.Keys)
            {
                LocalVariables[index++] = (type, new TypeObject(ForeignFunctions.NativeTypes[type]));
            }
        }

        public StackFrame(StackFrame parent, int localCount, string call)
        {
            Parent = parent;
            LocalVariables = new (string, Operand)[localCount];
            Call = call;
        }

        public Operand Get(IBindable variable) => Get(variable, 0);

        private Operand Get(IBindable variable, int level)
        {
            if (variable.ResolveLevel > level)
            {
                if (Parent is null) throw new System.Exception("Parent was null");
                else return Parent.Get(variable, level + 1);
            }
            return LocalVariables[variable.LocalId].Value;
        }

        public void Assign(IBindable variable, Operand value) => Assign(variable, value, 0);

        private void Assign(IBindable variable, Operand value, int level)
        {
            if (this == Global) 
            {
                (string, Operand)[] newGlobals = new (string, Operand)[LocalVariables.Length + 1];
                System.Array.Copy(LocalVariables, newGlobals, LocalVariables.Length);
                LocalVariables = newGlobals;
            }
            if (variable.ResolveLevel > level)
            {
                if(Parent is null) throw new System.Exception("Parent was null");
                else Parent.Assign(variable, value, level + 1);
            }
            else LocalVariables[variable.LocalId] = (variable.Identifier, value);
        }

        public IEnumerable<(string Id, Operand Value)> List() => LocalVariables;
    }
}
