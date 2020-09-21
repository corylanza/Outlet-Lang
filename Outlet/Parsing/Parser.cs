using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Outlet.Tokens;

namespace Outlet.Parsing {
	public partial class Parser {

		private LinkedList<Token> Tokens { get; set; }

		public Parser(LinkedList<Token> tokens)
        {
			Tokens = tokens;
        }

		private bool Match(Token s)
		{
			if (Tokens.Count > 0 && s.Equals(Tokens.First()))
			{
				Tokens.RemoveFirst(); return true;
			}
			else return false;
		}
		private void Consume(Token s, string error)
		{
			if (Tokens.Count == 0 || Tokens.Dequeue() != s) throw new OutletException("Syntax Error: " + error);
		}
		private T ConsumeType<T>(string error) where T : Token
		{
			if (Tokens.Count > 0 && Tokens.Dequeue() is T t) return t;
			else throw new OutletException("Syntax Error: " + error);
		}

		private static bool IsBinary(Token last) => 
			last is TokenLiteral || last is Identifier || last == DelimeterToken.RightParen || last == DelimeterToken.RightBrace;
		private static bool IsPreUnary(Token? last) => 
			last is null || last is OperatorToken || last == DelimeterToken.LeftParen || last == DelimeterToken.LeftBrace || last == DelimeterToken.Comma;
		private static bool IsPostUnary(Token next) =>
			next is null || next is OperatorToken || next == DelimeterToken.RightParen ||
			next == DelimeterToken.Comma || next == DelimeterToken.RightBrace || next == DelimeterToken.SemiC;

		public IASTNode Parse() {
			var block = ParseBlock();
			if (block.Lines.Count == 1) return block.Lines.First();
			return block;
		}

        private Block ParseBlock()
        {
            List<IASTNode> lines = new List<IASTNode>();
            List<FunctionDeclaration> funcs = new List<FunctionDeclaration>();
            List<ClassDeclaration> classes = new List<ClassDeclaration>();
            List<OperatorOverloadDeclaration> overloads = new List<OperatorOverloadDeclaration>();
            while (Tokens.Count > 0 && Tokens.First() != DelimeterToken.RightCurly)
            {
                var nextdecl = NextDeclaration();
                if (nextdecl is FunctionDeclaration fd) funcs.Add(fd);
                if (nextdecl is ClassDeclaration cd)
                {
                    if (cd.SuperClass == null) classes.Insert(0, cd);
                    else classes.Add(cd);
                }
                if (nextdecl is OperatorOverloadDeclaration o) overloads.Add(o);
                lines.Add(nextdecl);
            }
            return new Block(lines, funcs, classes, overloads);
        }


	}
}
