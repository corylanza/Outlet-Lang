using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public class FunctionDeclaration : Declaration {
		
        public readonly string ID;
		private readonly List<(Expression Type, Identifier ID)> ArgNames;
		private readonly Statement Body;
		private readonly Expression Type;
		//private readonly Function Func;

		public FunctionDeclaration(Expression type, Identifier id, List<(Expression, Identifier)> argnames, Statement body) {
            ID = id.Name;
			ArgNames = argnames;
			Body = body;
			Type = type;
		}

		public Function Construct(Scope closure) {
			Operand t = Type.Eval(closure);
			List<(Type, string)> args = new List<(Type, string)>();
			foreach(var arg in ArgNames) {
				Operand pt = arg.Type.Eval(closure);
				if (pt is Type ptype) args.Add((ptype, arg.ID.Name));
				else throw new OutletException(arg.Type.ToString() + " is not a valid type");
			}
			if(t is Type type) return new Function(closure, ID, type, args, Body);
			throw new OutletException(Type.ToString() + " is not a valid type");
		}

		public override void Resolve(Scope scope) {
			scope.Define(ID);
			Scope exec = new Scope(scope);
			foreach (var arg in ArgNames) {
				exec.Define(arg.ID.Name);
			}
			Body.Resolve(exec);
		}

		public override void Execute(Scope scope) {
            scope.Add(ID, null, Construct(scope));
        }

        public override string ToString() {
			string s = "func "+ID+"(";
			return s+")";
        }
    }
}
