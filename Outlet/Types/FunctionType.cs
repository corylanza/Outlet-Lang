using System;
using System.Linq;

namespace Outlet.Types {
	public class FunctionType : Type {

		public readonly (Type type, string id)[] Parameters;
		public readonly Type ReturnType;

		public FunctionType((Type, string)[] args, Type returntype) {
			Parameters = args;
			ReturnType = returntype;
		}

		public override bool Is(Type t, out uint level) {
            level = 0;
            if (t is FunctionType ft && Parameters.Length == ft.Parameters.Length)
            {
                foreach (var (arg, argType) in Parameters.Zip(ft.Parameters))
                {
                    if (arg.type.Is(argType.type, out uint elementLevel))
                    {
                        level += elementLevel;
                    }
                    else return false;
                }
                return true;
            }
            if (t == Primitive.Object)
            {
                level = int.MaxValue;
                return true;
            }
            level = 0;
            return false;
        }

        public bool Valid(out uint level, params Type[] args)
        {
            level = 0;
            uint distance = 0;
            if (args.Length != Parameters.Length) return false;
            for(int i = 0; i < args.Length; i++)
            {
                if (!args[i].Is(Parameters[i].type, out uint levels)) return false;
                else
                {
                    distance += levels;
                }
            }
            level = distance;
            return true;
        }

		public override string ToString() => $"({string.Join(", ", Parameters.Select(arg => arg.type.ToString()))}) => {ReturnType}";

	}
}
