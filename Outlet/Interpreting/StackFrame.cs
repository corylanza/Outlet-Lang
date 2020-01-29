using Outlet.Operands;
using System.Collections.Generic;

namespace Outlet.Interpreting
{
    public class StackFrame
    {
        public Dictionary<int, Operand> LocalVariables = new Dictionary<int, Operand>();
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

        public StackFrame(string call)
        {
            Call = call;
        }

        public Operand Get(IVariable variable) => LocalVariables[variable.LocalId];

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
