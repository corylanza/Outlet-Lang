using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Variable : Expression/*, IAssignable*/ {

		public readonly string Name;
        public int resolveLevel = -1;
		public Variable(string name) { Name = name; }
		/*
		public void Assign(Scope s, Operand value) {
			s.Assign(resolveLevel, Name, value);
		}
		
		public override Operand Eval(Scope scope) {
			if (resolveLevel == -1) {
				if (ForeignFunctions.NativeTypes.ContainsKey(Name)) return ForeignFunctions.NativeTypes[Name];
				if (ForeignFunctions.NativeFunctions.ContainsKey(Name)) return ForeignFunctions.NativeFunctions[Name];
				else throw new OutletException("could not find variable, THIS SHOULD NEVER PRINT");
			} else return scope.Get(resolveLevel, Name);
        }

		public override void Resolve(Scope scope) {
			// eventually Find should return (int, Type) tuple for type check
			resolveLevel = scope.Find(Name).Item2;
			if (resolveLevel == -1) {
				if (ForeignFunctions.NativeTypes.ContainsKey(Name)) return;
				if (ForeignFunctions.NativeFunctions.ContainsKey(Name)) return;
                throw new OutletException("variable " + Name + " could not be resolved");
            } 
		}*/

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() => Name;
	}
}
