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
		public bool IsProgram { get; private init; }

		public Block(List<IASTNode> lines, bool program = false) {
			Lines = lines;
			IsProgram = program;
		}

		public static Block Empty() => new Block(new List<IASTNode>());

		public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

		public override string ToString() {
			string s = "{\n";
			foreach (IASTNode d in Lines) s += d.ToString()+";\n";
			return s+"}";
		}
	}
}
