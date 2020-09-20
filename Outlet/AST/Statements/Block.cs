using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Block : Statement {

		public readonly List<ClassDeclaration> Classes = new List<ClassDeclaration>();
		public readonly List<FunctionDeclaration> Functions = new List<FunctionDeclaration>();
		public readonly List<OperatorOverloadDeclaration> OverloadedOperators = new List<OperatorOverloadDeclaration>();
        public readonly List<IASTNode> Lines = new List<IASTNode>();

		public Block(List<IASTNode> lines, List<FunctionDeclaration> funcs, List<ClassDeclaration> classes, List<OperatorOverloadDeclaration> opOverloads) {
			Lines = lines;
			Functions = funcs;
			Classes = classes;
			OverloadedOperators = opOverloads;
		}

		public static Block Empty() => new Block(new List<IASTNode>(), new List<FunctionDeclaration>(), new List<ClassDeclaration>(), new List<OperatorOverloadDeclaration>());

		public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() {
			string s = "{\n";
			foreach (IASTNode d in Lines) s += d.ToString()+";\n";
			return s+"}";
		}
	}
}
