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
        private Scope Parent;

        public Function() { }

		public Function(Scope parent, Identifier id, List<Identifier> argnames, Statement body) {
            Parent = parent;
            Name = id;
            ArgNames = argnames;
            Body = body;
        }

		public virtual Operand Call(params Operand[] args) {
            Scope t = new Scope(Parent) { Lines = new List<Declaration>() { Body } };
            for(int i = 0; i < args.Length; i++) {
                t.AddVariable(ArgNames[i], args[i]);
            }
            try {
                t.Execute();
            } catch(Return r) {
                return r.Value;
            }
            return null;
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
