using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Identifier : Expression, IToken {

		public string Name;
        private int resolveLevel;
		public Identifier(string name) { Name = name; }

        public override Operand Eval(Scope block) {
			if (resolveLevel == -1) {
				if (ForeignFunctions.NativeFunctions.ContainsKey(Name)) return ForeignFunctions.NativeFunctions[Name];
				else throw new OutletException("Variable "+Name+" could not be resolved, possibly global variable(unimplemented)");
			} else return block.Get(resolveLevel, Name);
        }

        public override void Resolve(Scope block) {
            resolveLevel = block.Find(Name);
        } 

        public override string ToString() => Name;
	}
}
