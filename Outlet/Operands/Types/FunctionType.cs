using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class FunctionType : Type {

		public readonly (Type type, string id)[] Args;
		public readonly Type ReturnType;

		public FunctionType((Type, string)[] args, Type returntype) {
			Args = args;
			ReturnType = returntype;
		}

		public override bool Is(Type t, out int level) {
            level = 0;
            if (t is FunctionType ft && Args.Length == ft.Args.Length)
            {
                foreach (var (arg, argType) in Args.Zip(ft.Args))
                {
                    if (arg.type.Is(argType.type, out int elementLevel))
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
            level = -1;
            return false;
        }

        public bool Valid(out int level, params Type[] args)
        {
            level = -1;
            int distance = 0;
            if (args.Length != Args.Length) return false;
            for(int i = 0; i < args.Length; i++)
            {
                if (!args[i].Is(Args[i].type, out int levels)) return false;
                else
                {
                    distance += levels;
                }
            }
            level = distance;
            return true;
        }

		public override bool Equals(Operand b) {
			throw new NotImplementedException();
		}

		public override string ToString() => "("+Args.Select(arg => arg.type).ToList().ToListString()+")" + " => " + ReturnType.ToString();

	}
}
