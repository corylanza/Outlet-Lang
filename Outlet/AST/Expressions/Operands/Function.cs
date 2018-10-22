using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST{
	public class Function : Operand, ICallable {

        private readonly string Name;
        private readonly List<Identifier> ArgNames;
        private readonly Statement Body;
		private readonly Scope Closure;

        public Function() { }

		public Function(Scope closure, string id, List<Identifier> argnames, Statement body) {
            Name = id;
            ArgNames = argnames;
            Body = body;
			Closure = closure;
        }

		//TODO fix scopes, may need scope passed in
		public virtual Operand Call(Scope block, params Operand[] args) {
			Scope exec = new Scope(Closure);
			for (int i = 0; i < args.Length; i++) {
				exec.Add(ArgNames[i].Name, args[i]);
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

		public override void Resolve(Scope block) {
			Console.WriteLine("resolved");
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
        public override Operand Call(Scope block, params Operand[] args) => F(args);
    }
}
