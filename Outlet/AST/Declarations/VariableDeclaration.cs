using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.AST {
	public class VariableDeclaration : Declaration {
		
		private readonly string ID;
		private readonly Expression Type;
		private readonly Expression Initializer;

		public VariableDeclaration(Expression type, Identifier id, Expression initializer) {
			ID = id.Name;
			Type = type;
			Initializer = initializer;
		}

		public override void Resolve(Scope scope) {
            scope.Declare(ID);
			Type.Resolve(scope);
            Initializer?.Resolve(scope);
            scope.Define(ID);
		}

		public override void Execute(Scope scope) {
			Operand t = Type.Eval(scope);
			Operand initial = Initializer?.Eval(scope);
			if (t is Type type) {
				Type initType = initial?.Type;
				if (initial is null || initType.Is(type)) scope.Add(ID, type, initial);
				else throw new OutletException("cannot convert type " + initType.ToString() + " to type " + type.ToString());
			} 
			else throw new OutletException(Type.ToString()+" is not a valid type");
		}

		public override string ToString() {
			string s = "var " + ID.ToString();
			if (Initializer is null) return s;
			else return s + " = " + Initializer.ToString();
		}
	}
}
