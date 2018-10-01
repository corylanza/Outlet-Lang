using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet {
	public class Function {

        private Identifier Name;
        private List<Identifier> ArgNames;
        private Statement Body;

        public Function() { }

		public Function(Identifier id, List<Identifier> argnames, Statement body) {
            Name = id;
            ArgNames = argnames;
            Body = body;
        }

		public virtual Operand Call(Scope block, params Operand[] args) {
            if(Body is Scope t) {
				for (int i = 0; i < args.Length; i++) {
					t.AddVariable(ArgNames[i], args[i]);
				}
				try {
					t.Execute();
				} catch (Return r) {
					t.Variables.Clear();
					return r.Value;
				}
				t.Variables.Clear();
			} else if(Body is Expression e) {
				Scope s = new Scope(block);
				for (int i = 0; i < args.Length; i++) {
					s.AddVariable(ArgNames[i], args[i]);
				}
				return e.Eval(s);
			}
            
           
            return null;
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
