using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;

namespace Outlet.AST {
	public class FunctionDeclaration : Declaration {

		public readonly Declarator Decl;
		public readonly List<Declarator> Args;
		public readonly Statement Body;
		public FunctionType Type;	// set in the checking phase

		public FunctionDeclaration(Declarator decl, List<Declarator> argnames, Statement body) {
			Name = decl.ID;
			Decl = decl;
			Args = argnames;
			Body = body;
		}
		/*
		public Function Construct(Scope closure) {
			List<(Type, string)> args = Args.Select(x => (x.GetType(closure), x.ID)).ToList();
			return new Function(closure, Decl.ID, Decl.GetType(closure), args, Body);
		}

		public override void Resolve(Scope scope) {
			scope.Define(Decl.GetType(scope), Decl.ID);
			Decl.Resolve(scope);
			Scope exec = new Scope(scope);
			Args.ForEach(x => { x.Resolve(scope); exec.Define(x.GetType(scope), x.ID); });
			Body.Resolve(exec);
		}

		public override void Execute(Scope scope) {
			scope.Add(Decl.ID, null, Construct(scope));
		}*/

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() {
			string s = "func " + Decl.ID + "(";
			return s + ")";
		}
	}
}
