using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Outlet.Operands
{
    public class MethodGroup : Operand<MethodGroupType>
    {
        public readonly List<Function> Methods;

        public MethodGroup(params Function[] functions)
        {
            Methods = functions.ToList();
            Type = GetMethodGroupType();
        }

        public void AddMethod(Function toAdd)
        {
            Methods.Add(toAdd);
            Type = GetMethodGroupType();
        }

        public Function FindBestMatch(params Operand[] inputs)
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
            return new MethodGroupType(Methods.Select(method => method.Type).ToArray());
        }

        public override bool Equals(Operand other) => ReferenceEquals(this, other);

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
