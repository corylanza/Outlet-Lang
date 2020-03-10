using System;
using System.Linq;

namespace Outlet.Types {
	public class FunctionType : Type {

		public readonly (Type type, string id)[] Args;
		public readonly Type ReturnType;

		public FunctionType((Type, string)[] args, Type returntype) {
			Args = args;
			ReturnType = returntype;
		}

		public override bool Is(Type t, out uint level) {
            level = 0;
            if (t is FunctionType ft && Args.Length == ft.Args.Length)
            {
                foreach (var (arg, argType) in Args.Zip(ft.Args))
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
            if (args.Length != Args.Length) return false;
            for(int i = 0; i < args.Length; i++)
            {
                if (!args[i].Is(Args[i].type, out uint levels)) return false;
                else
                {
                    distance += levels;
                }
            }
            level = distance;
            return true;
        }

		public override string ToString() => "("+Args.Select(arg => arg.type).ToList().ToListString()+")" + " => " + ReturnType.ToString();

	}
}
