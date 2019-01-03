using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Outlet.Tokens;

namespace Outlet.Parsing {
	public static partial class Parser {

		private static bool IsBinary(Token last) => 
			last is TokenLiteral || last == Delimeter.RightParen || last == Delimeter.RightBrace;
		private static bool IsPreUnary(Token last) => 
			last is null || last is Operator || last == Delimeter.LeftParen || last == Delimeter.LeftBrace || last == Delimeter.Comma;
		private static bool IsPostUnary(Token next) =>
			next is null || next is Operator || next == Delimeter.RightParen ||
			next == Delimeter.Comma || next == Delimeter.RightBrace || next == Delimeter.SemiC;

		public static Declaration Parse(LinkedList<Token> tokens) {
			List<Declaration> lines = new List<Declaration>();
			List<FunctionDeclaration> funcs = new List<FunctionDeclaration>();
			List<ClassDeclaration> classes = new List<ClassDeclaration>();
			while(tokens.Count > 0) {
				var nextdecl = NextDeclaration(tokens);
				if(nextdecl is FunctionDeclaration fd) funcs.Add(fd);
				if(nextdecl is ClassDeclaration cd) classes.Add(cd);
				lines.Add(nextdecl);
			}
			if (lines.Count == 1) return lines[0];
			return new Block(lines, funcs, classes);
		}

		public static T Dequeue<T>(this LinkedList<T> ll) {
			T temp = ll.First();
			ll.RemoveFirst();
			return temp;
		}

		public static T Head<T>(this LinkedList<T> ll) => ll.Count == 0 ? default(T) : ll.First();

	}
}
