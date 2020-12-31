using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Outlet.Tokens;

namespace Outlet.Parsing {
	public partial class Parser {

		private LinkedList<Lexeme> Tokens { get; set; }

		public Parser(LinkedList<Lexeme> tokens)
        {
			Tokens = tokens;
        }

		private bool PeekMatch(Token s) => Tokens.Count > 0 && s == Tokens.First().InnerToken;

		private bool PeekNextTokenExistsAndIsnt(Token s) => Tokens.Count > 0 && s != Tokens.First().InnerToken;

		private bool PeekMatchType<T>([NotNullWhen(true)] out T? i) where T : Token
		{
			i = null;
			if(Tokens.Count > 0 && Tokens.First().InnerToken is T t)
            {
				i = t;
				return true;
            }
			return false;
		}

		private bool Match(Token s)
		{
			if (Tokens.Count > 0 && s == Tokens.First().InnerToken)
			{
				Tokens.RemoveFirst(); return true;
			}
			else return false;
		}

		private void Consume(Token s, string error)
		{
			Lexeme? found = Tokens.Count > 0 ? Tokens.Dequeue() : null;
			if (found is null) throw new OutletException($"Syntax Error: {error}, found: no more tokens");
			if (found.InnerToken != s) throw new OutletException($"Syntax Error at line {found.Line}: {error}, found: {found.InnerToken}");
		}

		private Lexeme ConsumeTypeGetLexeme<T>(string error) where T : Token
		{
			if (Tokens.Count > 0 && Tokens.Dequeue() is Lexeme l && l.InnerToken is T) return l;
			else throw new OutletException("Syntax Error: " + error);
		}

		private T ConsumeType<T>(string error) where T : Token 
		{
			if (Tokens.Count > 0 && Tokens.Dequeue().InnerToken is T t) return t;
			else throw new OutletException("Syntax Error: " + error);
		}

		private static bool IsBinary(Token last) => 
			last is TokenLiteral || last is Identifier || last == DelimeterToken.RightParen || last == DelimeterToken.RightBrace;
		private static bool IsPreUnary(Token? last) => 
			last is null || last is OperatorToken || last == DelimeterToken.LeftParen || last == DelimeterToken.LeftBrace || last == DelimeterToken.Comma;
		private static bool IsPostUnary(Token? next) =>
			next is null || next is OperatorToken || next == DelimeterToken.RightParen ||
			next == DelimeterToken.Comma || next == DelimeterToken.RightBrace || next == DelimeterToken.SemiC;

		public IASTNode Parse() {
			var block = ParseBlock(isProgram: true);
			if (block.Lines.Count == 1 && block.Lines.First() is Expression expr) return expr;
			return block;
		}

        private Block ParseBlock(bool isProgram = false)
        {
			List<IASTNode> lines = new List<IASTNode>();
            while (PeekNextTokenExistsAndIsnt(DelimeterToken.RightCurly))
            {
                var nextdecl = NextDeclaration();
                lines.Add(nextdecl);
            }
            return new Block(lines, isProgram);
        }
	}
}
