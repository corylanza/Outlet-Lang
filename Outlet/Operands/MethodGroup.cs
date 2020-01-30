using System.Collections.Generic;
using System.Linq;
using Outlet.Types;

namespace Outlet.Operands
{
    public class MethodGroup : Operand<MethodGroupType>, ICallable
    {
        private readonly List<Function> Methods;

        public MethodGroup(params Function[] functions)
        {
            Methods = functions.ToList();
            RuntimeType = GetMethodGroupType();
        }

        public void AddMethod(Function toAdd)
        {
            Methods.Add(toAdd);
            RuntimeType = GetMethodGroupType();
        }

        private Function FindBestMatch(params Operand[] inputs)
        {
            (Function best, int bestLevel) = (default, -1);
            foreach (Function overload in Methods)
            {
                bool valid = overload.Valid(out int level, inputs.Select(arg => arg.GetOutletType()).ToArray());
                if (!valid) continue;
                if (bestLevel == -1 || level < bestLevel)
                {
                    (best, bestLevel) = (overload, level);
                }
            }

            return best;
        }

        private MethodGroupType GetMethodGroupType()
        {
            return new MethodGroupType(Methods.Select(method => (method.RuntimeType, 0)).ToArray());
        }

        public override bool Equals(Operand other) => ReferenceEquals(this, other);

        public override string ToString()
        {
            string s = "MethodGroup {\n";
            Methods.ForEach(method => s += "\t" + method + "\n");
            s += "}";
            return s;
        }

        public Operand Call(params Operand[] args) => FindBestMatch(args).Call(args);
    }
}
