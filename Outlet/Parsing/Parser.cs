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
		private static bool IsUnary(IToken last) => 
			last is null || last is Operator || last == Delimeter.LeftParen || last == Delimeter.LeftBrace || last == Delimeter.Comma;

		public static Statement Parse(Queue<IToken> tokens) => Parse(new Scope(), tokens);

		public static Statement Parse(Scope block, Queue<IToken> tokens) {
			while(tokens.Count > 0) {
				block.Lines.Add(NextDeclaration(block, tokens));
			}
			if (block.Lines.Count == 1 && block.Lines[0] is Expression e) return e;
			return block;
		}

	}
}
