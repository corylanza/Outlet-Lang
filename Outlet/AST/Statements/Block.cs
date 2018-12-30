using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Block : Statement {

		public readonly List<ClassDeclaration> Classes = new List<ClassDeclaration>();
		public readonly List<FunctionDeclaration> Functions = new List<FunctionDeclaration>();
        public readonly List<Declaration> Lines = new List<Declaration>();

		public Block(List<Declaration> lines, List<FunctionDeclaration> funcs, List<ClassDeclaration> classes) {
			Lines = lines;
			Functions = funcs;
			Classes = classes;
		}
		/*
		public override void Resolve(Scope scope) {
			Scope exec = new Scope(scope);
			foreach (Declaration d in Lines) d.Resolve(exec);
		}
		
        public override void Execute(Scope scope) {
			Scope exec = new Scope(scope);
			foreach (Declaration d in Lines) d.Execute(exec);
		}
		*/
		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() {
			string s = "{\n";
			foreach (Declaration d in Lines) s += d.ToString()+";\n";
			return s+"}";
		}
	}
}
