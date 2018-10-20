using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST{
	public class Function : Operand, ICallable {

        private string Name;
        private List<Identifier> ArgNames;
        private Statement Body;
		private Scope Closure;

        public Function() { }

		public Function(Scope closure, string id, List<Identifier> argnames, Statement body) {
            Name = id;
            ArgNames = argnames;
            Body = body;
			Closure = closure;
        }

		//TODO fix scopes, may need scope passed in
		public virtual Operand Call(params Operand[] args) {
			Scope exec = new Scope(Closure);
			for (int i = 0; i < args.Length; i++) {
				exec.AddVariable(ArgNames[i].Name, args[i]);
			} try {
				if (Body is Expression e) return e.Eval(exec);
				Body.Execute(exec);
			} catch (Return r) {
				return r.Value;
			}
            return null;
		}

		public override bool Equals(Operand b) => ReferenceEquals(this, b);

		public override Operand Eval(Scope block) {
			return this;
		}

        public override string ToString() {
			return "function: "+Name.ToString();
		}
	}

    public class Native : Function {
        private Func<Operand[], Operand> F;
        public Native(Func<Operand[], Operand> func)  {
            F = func;
        }
        public override Operand Call(params Operand[] args) => F(args);
    }
}
