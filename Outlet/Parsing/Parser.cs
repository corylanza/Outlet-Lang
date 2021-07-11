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

		// Store this for errors where unexpected EOF
		private int TotalLines { get; init; }

		private readonly List<SyntaxException> SyntaxErrors = new List<SyntaxException>();

		public Parser(LinkedList<Lexeme> tokens)
        {
			Tokens = tokens;
			TotalLines = tokens.Last().Line;
        }

		private bool PeekMatch(Token s) => Tokens.Count > 0 && s.Equals(Tokens.First().InnerToken);

		private bool PeekNextTokenExistsAndIsnt(Token s) => Tokens.Count > 0 && !s.Equals(Tokens.First().InnerToken);

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
			if (Tokens.Count > 0 && s.Equals(Tokens.First().InnerToken))
			{
				Tokens.RemoveFirst(); return true;
			}
			else return false;
		}

		private void Consume(Token s, string expected)
		{
			Lexeme? found = Tokens.Count > 0 ? Tokens.Dequeue() : null;
			if (found is not null && s.Equals(found.InnerToken)) return;
			else throw SyntaxError(expected, found);
		}

		private (Lexeme lexeme, T token) ConsumeTypeGetLexeme<T>(string expected) where T : Token
		{
			Lexeme? found = Tokens.Count > 0 ? Tokens.Dequeue() : null;
			if (found is not null && found.InnerToken is T token) return (found, token);
			throw SyntaxError(expected, found);
		}

		private T ConsumeType<T>(string expected) where T : Token
		{
			Lexeme? found = Tokens.Count > 0 ? Tokens.Dequeue() : null;
			if (found is not null && found.InnerToken is T t) return t; 
			throw SyntaxError(expected, found);
		}

		private SyntaxException SyntaxError(string expected, Lexeme? found)
        {
			var line = found?.Line ?? TotalLines;
			if (found is null) return new SyntaxException($"Line {line}: expected {expected}, found: EndOfFile", line, new Range());
			else
			{
				var characterRange = new Range(found.Character, found.Character + (found.InnerToken.ToString()?.Length ?? 0));
				return new SyntaxException($"Line {line} chars [{characterRange.Start}:{characterRange.End}]: expected {expected}, found: {found.InnerToken}", line, characterRange);
			}
		}


		private static bool IsBinary(Token last) => 
			last is TokenLiteral || last is Identifier || last == Symbol.RightParen || last == Symbol.RightBrace;
		private static bool IsPreUnary(Token? last) => 
			last is null || last is OperatorToken || last == Symbol.LeftParen || last == Symbol.LeftBrace || last == Symbol.Comma;
		private static bool IsPostUnary(Token? next) =>
			next is null || next is OperatorToken || next == Symbol.RightParen ||
			next == Symbol.Comma || next == Symbol.RightBrace || next == Symbol.SemiC;

		public IASTNode Parse() {
			var block = ParseBlock(isProgram: true);
			if (SyntaxErrors.Count > 0) throw new ParserException(SyntaxErrors.ToArray());
			if (block.Lines.Count == 1 && block.Lines.First() is Expression expr) return expr;
			return block;
		}

		/// <summary>
		/// Parses sequences of statements after a { until the next } or out of tokens
		/// This is how all programs are represented as well including single lines
		/// It is technically allowed to have extraneous } in a program currently, TODO maybe change this
		/// </summary>
		/// <param name="isProgram">whether this is the root level block representing the program</param>
        private Block ParseBlock(bool isProgram = false)
        {
			List<IASTNode> lines = new List<IASTNode>();

            while (PeekNextTokenExistsAndIsnt(DelimeterToken.RightCurly))
            {
				try
				{
					var nextdecl = NextDeclaration();
					lines.Add(nextdecl);
				}
				catch (SyntaxException e)
				{
					SyntaxErrors.Add(e);
					Tokens = new LinkedList<Lexeme>(Tokens.SkipWhile(lexeme => lexeme.InnerToken != DelimeterToken.SemiC));
				}
			}

            return new Block(lines, isProgram);
        }
	}
}
