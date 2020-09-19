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
            LocalVariables = new (string, Operand)[NativeOutletTypes.NativeTypes.Count];
            foreach(string type in NativeOutletTypes.NativeTypes.Keys)
            {
                LocalVariables[index++] = (type, new TypeObject(NativeOutletTypes.NativeTypes[type]));
            }
        }

        public StackFrame(StackFrame parent, uint localCount, string call)
        {
            Parent = parent;
            LocalVariables = new (string, Operand)[localCount];
            Call = call;
        }

        public Operand Get(IBindable variable) => Get(variable, level: 0);

        private Operand Get(IBindable variable, uint level = 0)
        {
            if (variable.ResolveLevel > level)
            {
                if (Parent is null) throw new UnexpectedException("Parent was null");
                else return Parent.Get(variable, level + 1);
            }
            else if(variable.LocalId.HasValue) return LocalVariables[variable.LocalId.Value].Value;
            else throw new UnexpectedException($"Variable {variable.Identifier} was not resolved");
        }

        public void Assign(IBindable variable, Operand value) => Assign(variable, value, level: 0);

        private void Assign(IBindable variable, Operand value, uint level = 0)
        {
            if (this == Global) 
            {
                (string, Operand)[] newGlobals = new (string, Operand)[LocalVariables.Length + 1];
                System.Array.Copy(LocalVariables, newGlobals, LocalVariables.Length);
                LocalVariables = newGlobals;
            }
            if (variable.ResolveLevel > level)
            {
                if(Parent is null) throw new UnexpectedException("Parent was null");
                else Parent.Assign(variable, value, level + 1);
            }
            else if(variable.LocalId.HasValue) LocalVariables[variable.LocalId.Value] = (variable.Identifier, value);
            else throw new UnexpectedException($"Variable {variable.Identifier} was not resolved");
        }

        public IEnumerable<(string Id, Operand Value)> List() => LocalVariables;
    }
}
