using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.AST {
	public class VariableDeclaration : Declaration {

		public readonly Declarator Decl;
		public readonly Expression Initializer;

		public VariableDeclaration(Declarator decl, Expression initializer) {
			Decl = decl;
			Initializer = initializer;
		}
		/*
		public override void Resolve(Scope scope) {
            scope.Declare(Decl.GetType(scope), Decl.ID);
			Decl.Resolve(scope);
            Initializer?.Resolve(scope);
            scope.Define(Decl.GetType(scope),Decl.ID);
		}

		public override void Execute(Scope scope) {
			Operand initial = Initializer?.Eval(scope);
			Type type = Decl.GetType(scope);
			Type initType = initial?.Type;
			if (initial is null || initType.Is(type)) scope.Add(Decl.ID, type, initial);
			else throw new OutletException("cannot convert type "+initType.ToString() + " to type "+type.ToString());
		}
		*/
		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() {
			string s = Decl.ToString();
			if (Initializer is null) return s;
			else return s + " = " + Initializer.ToString();
		}
	}
}
