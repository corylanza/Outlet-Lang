using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Function : Operand, ICallable {

        private readonly string Name;
        private readonly List<(Type Type, string ID)> ArgNames;
        private readonly Statement Body;
		private readonly Scope Closure;
		private readonly Type ReturnType;

		protected Function() { }

		public Function(Scope closure, string id, Type type, List<(Type, string)> argnames, Statement body) {
            Name = id;
			ReturnType = type;
            ArgNames = argnames;
            Body = body;
			Closure = closure;
        }
		
		public virtual Operand Call(params Operand[] args) {
			Scope exec = new Scope(Closure);
			Operand returnval = null;
 			for (int i = 0; i < args.Length; i++) {
				Operand param = args[i];
				Type paramType = ArgNames[i].Type;
				if(param.Type.Is(paramType)) exec.Add(ArgNames[i].ID, ArgNames[i].Type, args[i]);
				else throw new OutletException("cannot convert type " + param.Type.ToString() + " to type " + paramType.ToString());
			} try {
				if (Body is Expression e) returnval = e.Eval(exec);
				else Body.Execute(exec);
			} catch (Return r) {
				returnval = r.Value;
			}
			if (returnval.Type.Is(ReturnType)) return returnval;
			else throw new OutletException("cannot convert type " + returnval.Type.ToString() + " to type " + ReturnType.ToString());	
		}

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override string ToString() {
			return "function: "+Name?.ToString();
		}
	}

    public class Native : Function, ICallable {
        private readonly Func<Operand[], Operand> Underlying;
        public Native(Func<Operand[], Operand> func)  { Underlying = func; }
        public override Operand Call(params Operand[] args) => Underlying(args);
    }
}
