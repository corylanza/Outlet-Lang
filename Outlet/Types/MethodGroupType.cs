using System;
using System.Collections.Generic;
using System.Linq;

namespace Outlet.Types
{
    public class MethodGroupType : Type
    {
        public readonly List<(FunctionType type, int Id)> Methods;

        public MethodGroupType(params (FunctionType type, int id)[] functions)
        {
            Methods = functions.ToList();
        }

        public (FunctionType? type, int id) FindBestMatch(params ITyped[] inputs)
        {
            (FunctionType? best, int bestLevel, int bestId) = (default, -1, -1);
            foreach ((FunctionType overload, int id) in Methods)
            {
                bool valid = overload.Valid(out int level, inputs);
                if (!valid) continue;
                // TODO this logic may allow -1 to be set again
                if (bestLevel == -1 || level < bestLevel)
                {
                    (best, bestLevel, bestId) = (overload, level, id);
                }
            }

            return (best, bestId);
        }

        //public override bool Equals(Operand b) => ReferenceEquals(this, b);

        public override bool Is(ITyped t, out int level)
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
