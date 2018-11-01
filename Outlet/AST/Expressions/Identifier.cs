using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Identifier : Expression, IToken {

		public string Name;
        public int resolveLevel = -1;
		public Identifier(string name) { Name = name; }

        public override Operand Eval(Scope scope) {
			if (resolveLevel == -1) {
				if (ForeignFunctions.NativeTypes.ContainsKey(Name)) return ForeignFunctions.NativeTypes[Name];
				if (ForeignFunctions.NativeFunctions.ContainsKey(Name)) return ForeignFunctions.NativeFunctions[Name];
				else throw new OutletException("Variable "+Name+" could not be resolved, possibly global variable(unimplemented)");
			} else return scope.Get(resolveLevel, Name);
        }

        public override void Resolve(Scope scope) {
			// eventually Find should return (int, Type) tuple for type check
            resolveLevel = scope.Find(Name);
        } 

        public override string ToString() => Name;
	}
}
