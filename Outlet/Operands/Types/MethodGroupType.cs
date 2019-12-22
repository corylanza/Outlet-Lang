using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Outlet.Operands
{
    public class MethodGroupType : Type
    {
        public readonly List<FunctionType> Methods;

        public MethodGroupType(params FunctionType[] functions)
        {
            Methods = functions.ToList();
        }

        public FunctionType FindBestMatch(params Type[] inputs)
        {
            (FunctionType best, int bestLevel) = (default, -1);
            foreach (FunctionType overload in Methods)
            {
                bool valid = overload.Valid(out int level, inputs);
                if (!valid) continue;
                if (bestLevel == -1 || level < bestLevel)
                {
                    (best, bestLevel) = (overload, level);
                }
            }

            return best;
        }

        public override bool Equals(Operand b) => ReferenceEquals(this, b);

        public override bool Is(Type t, out int level)
        {
            level = -1;
            return false;
        }

        public override string ToString()
        {
            return "MethodGroup";
        }
    }
}
