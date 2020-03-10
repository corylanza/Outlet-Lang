using System;
using System.Collections.Generic;
using System.Linq;

namespace Outlet.Types
{
    public class MethodGroupType : Type
    {
        public readonly List<(FunctionType type, uint Id)> Methods;

        public MethodGroupType(params (FunctionType type, uint id)[] functions)
        {
            Methods = functions.ToList();
        }

        public (FunctionType? type, uint? id) FindBestMatch(params Type[] inputs)
        {
            (FunctionType? best, uint? bestLevel, uint? bestId) = (default, null, null);
            foreach ((FunctionType overload, uint id) in Methods)
            {
                bool valid = overload.Valid(out uint level, inputs);
                if (!valid) continue;
                // TODO this logic may allow -1 to be set again
                if (bestLevel == null || level < bestLevel)
                {
                    (best, bestLevel, bestId) = (overload, level, id);
                }
            }

            return (best, bestId);
        }

        //public override bool Equals(Operand b) => ReferenceEquals(this, b);

        public override bool Is(Type t, out uint level)
        {
            level = 0;
            return false;
        }

        public override string ToString()
        {
            return "MethodGroup";
        }
    }
}
