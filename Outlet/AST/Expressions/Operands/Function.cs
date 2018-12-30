using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Function : Operand {

		private readonly string Name;
		public readonly (Type Type, string ID)[] ArgNames;
		public readonly Statement Body;
		public readonly Scope Closure;
		public readonly Type ReturnType;

		protected Function() { }

		public Function(Scope closure, string id, FunctionType type, Statement body) {
			Name = id;
			ReturnType = type;
			ArgNames = type.Args;
			ReturnType = type.ReturnType;
			Type = type;
			Body = body;
			Closure = closure;
		}
		/*
		public virtual Operand Call(params Operand[] args) {
			Scope exec = new Scope(Closure);
			Operand returnval = null;
			for(int i = 0; i < args.Length; i++) {
				args[i].Cast(ArgNames[i].Type);
				exec.Add(ArgNames[i].ID, ArgNames[i].Type, args[i]);
			}
			try {
				//if(Body is Expression e) returnval = e.Eval(exec);
				//else Body.Execute(exec);
			} catch(Return r) {
				returnval = r.Value;
			}
			if(ReferenceEquals(ReturnType, Primitive.Void)) return null;
			returnval.Cast(ReturnType);
			return returnval;
		}*/

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override string ToString() {
			return "function: " + Name?.ToString();
		}

	}

	public class Native : Function, ICallable {
		private readonly Func<Operand[], Operand> Underlying;
		public Native(FunctionType type, Func<Operand[], Operand> func) {
			Underlying = func;
			Type = type;
		}
		public Operand Call(params Operand[] args) => Underlying(args);
	}
}
