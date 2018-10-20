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
            return block.Get(resolveLevel, Name);
        }

        public override void Resolve(Scope block) {
            resolveLevel = block.Find(Name);
            if(resolveLevel == -1) throw new OutletException("Variable could not be resolved, possibly global variable (unimplemented)");
        } 

        public override string ToString() => Name;
	}
}
