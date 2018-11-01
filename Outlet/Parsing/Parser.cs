using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.Parsing {
	public static partial class Parser {

		private static bool IsBinary(IToken last) => 
			last is Operand || last == Delimeter.RightParen || last == Delimeter.RightBrace;
		private static bool IsPreUnary(IToken last) => 
			last is null || last is Operator || last == Delimeter.LeftParen || last == Delimeter.LeftBrace || last == Delimeter.Comma;
		private static bool IsPostUnary(IToken next) =>
			next is null || next is Operator || next == Delimeter.RightParen ||
			next == Delimeter.Comma || next == Delimeter.RightBrace || next == Delimeter.SemiC;

		public static Declaration Parse(LinkedList<IToken> tokens) {
			List<Declaration> lines = new List<Declaration>();
			while(tokens.Count > 0) {
				lines.Add(NextDeclaration(tokens));
			}
			if (lines.Count == 1) return lines[0];
			return new Block(lines);
		}

		public static T Dequeue<T>(this LinkedList<T> ll) {
			T temp = ll.First();
			ll.RemoveFirst();
			return temp;
		}

		public static T Head<T>(this LinkedList<T> ll) => ll.Count == 0 ? default(T) : ll.First();

	}
}
