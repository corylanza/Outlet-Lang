using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Expressions;

namespace Outlet.Parsing {
	public static class Parser {

		private static bool IsBinary(IToken last)=> last is Operand || last == Delimeter.RightParen;
		private static bool IsUnary(IToken last) => last is null || last is Operator || last == Delimeter.LeftParen;

		public static Expression Parse(Queue<IToken> t) => ConstructAST(Shunt(t));

		public static Queue<IToken> Shunt(Queue<IToken> t) {
			Queue<IToken> output = new Queue<IToken>();
			Stack<IToken> stack = new Stack<IToken>();
			Stack<int> arity = new Stack<int>();
			IToken cur = null, last = null;
			bool lesserPrecedence(Operator op) => stack.Count > 0 && stack.Peek() is Operator o && (o.Precedence <= op.Precedence || o.Asssoc == Side.Right);
			while (t.Count > 0) {
				last = cur;
				cur = t.Dequeue();
				switch (cur) {
					case Literal l: // does not handle other future operand types
						output.Enqueue(l);
						break;
					case Operator o:
						if (o.Name == "-" && IsUnary(last)) {
							o = Operator.Negative;
						} //else if (IsUnary(last)) throw new Exception("unary operator: " +o.Name);
						else while (lesserPrecedence(o)) {
								output.Enqueue(stack.Pop());
						}
						stack.Push(o);
						break;
					case Delimeter d when d == Delimeter.LeftParen:
						stack.Push(d);
						arity.Push(1);
						break;
					case Delimeter comma when comma == Delimeter.Comma:
						while (stack.Peek() != Delimeter.LeftParen) {
							output.Enqueue(stack.Pop());
						}
						arity.Push(arity.Pop() + 1);
						break;
					case Delimeter right when right == Delimeter.RightParen:
						while(stack.Peek() != Delimeter.LeftParen) {
							output.Enqueue(stack.Pop());
						} stack.Pop();
						Console.WriteLine("arity: " + arity.Pop()); // number of args
						break;
					default:
						throw new Exception("unrecognized token");
				}
			}
			while (stack.Count > 0) output.Enqueue(stack.Pop());
			
			return output;
		}

		// Can be modified to be in the Shunt function by replacing stack of ops to expressions
		public static Expression ConstructAST(Queue<IToken> input) {
			Stack<Expression> stack = new Stack<Expression>();
			while(input.Count > 0) {
				IToken cur = input.Dequeue();
				switch (cur) {
					case Literal l:
						stack.Push(l);
						break;
					case Operator binop when binop.Arity == Arity.Binary:
						Expression temp = stack.Pop();
						stack.Push(new Binary(stack.Pop(), binop, temp));
						break;
					case Operator unop when unop.Arity == Arity.Unary:
						stack.Push(new Unary(stack.Pop(), unop));
						break;
				}
			}
			if (stack.Count != 1) throw new Exception("unbalanced equation, stack had " + stack.Count + " elements");
			return stack.Pop();
		}
	}
}
