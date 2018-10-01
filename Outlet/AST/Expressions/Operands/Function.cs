﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST{
	public class Function : Operand {

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
			Scope exec = new Scope(block);
			for (int i = 0; i < args.Length; i++) {
				exec.AddVariable(ArgNames[i], args[i]);
			}
			try {
				if (Body is Scope s) s.Parent = exec;
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
        public override Operand Call(Scope block, params Operand[] args) => F(args);
    }
}