using Outlet.Operands;
using System.Collections.Generic;

namespace Outlet.Interpreting
{
    public class StackFrame
    {
        public Dictionary<int, Operand> LocalVariables = new Dictionary<int, Operand>();
        private readonly StackFrame Parent;
        public string Call { get; private set; }

        public static readonly StackFrame Global = new StackFrame();

        private StackFrame()
        {
            Call = "Global scope";
            int index = 0;
            foreach(var type in ForeignFunctions.NativeTypes.Values)
            {
                LocalVariables.Add(index++, new TypeObject(type));
            }
        }

        public StackFrame(StackFrame parent, string call)
        {
            Parent = parent;
            Call = call;
        }

        public Operand Get(IVariable variable, int level = 0)
        {
            if (variable is Outlet.AST.Variable v && v.resolveLevel > level) return Parent.Get(variable, level + 1);
            return LocalVariables[variable.LocalId];
        }

        public void Initialize(IDeclarable declaration, Operand value)
        {
            LocalVariables[declaration.LocalId] = value;
        }

        public void Assign(IVariable variable, Operand value)
        {
            LocalVariables[variable.LocalId] = value;
        }
    }
}
