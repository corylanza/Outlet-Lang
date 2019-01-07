using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Block : Statement {

		public readonly List<ClassDeclaration> Classes = new List<ClassDeclaration>();
		public readonly List<FunctionDeclaration> Functions = new List<FunctionDeclaration>();
        public readonly List<IASTNode> Lines = new List<IASTNode>();

		public Block(List<IASTNode> lines, List<FunctionDeclaration> funcs, List<ClassDeclaration> classes) {
			Lines = lines;
			Functions = funcs;
			Classes = classes;
		}

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() {
			string s = "{\n";
			foreach (IASTNode d in Lines) s += d.ToString()+";\n";
			return s+"}";
		}
	}
}
