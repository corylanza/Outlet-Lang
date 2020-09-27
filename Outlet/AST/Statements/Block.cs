using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Block : Statement {

		public List<ClassDeclaration> Classes => Lines.OfType<ClassDeclaration>().OrderBy(c => c.SuperClass != null).ToList();
		public List<FunctionDeclaration> Functions => Lines.OfType<FunctionDeclaration>().ToList();
		public List<OperatorOverloadDeclaration> OverloadedOperators => Lines.OfType<OperatorOverloadDeclaration>().ToList();

		public List<IASTNode> Lines { get; private set; }

		public Block(List<IASTNode> lines) {
			Lines = lines;
		}

		public static Block Empty() => new Block(new List<IASTNode>());

		public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() {
			string s = "{\n";
			foreach (IASTNode d in Lines) s += d.ToString()+";\n";
			return s+"}";
		}
	}
}
