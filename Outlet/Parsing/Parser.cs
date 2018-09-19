using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Expressions;

namespace Outlet.Parsing {
	public static class Parser {

		private static bool IsBinary(IToken last) => last is Operand || last == Delimeter.RightParen;
		private static bool IsUnary(IToken last) => last is null || last is Operator || last == Delimeter.LeftParen || last == Delimeter.Comma;

		//public static Expression Parse(Queue<IToken> t) => Parse(t);//ConstructAST(Shunt(t));

		public static Expression Parse(Queue<IToken> t) {
			Stack<Expression> output = new Stack<Expression>();
			Stack<IToken> stack = new Stack<IToken>();
			Stack<int> arity = new Stack<int>();
			IToken cur = null, last = null;
			bool lesserPrecedence(Operator op) => stack.Count > 0 && stack.Peek() is Operator o && (o.Precedence <= op.Precedence || o.Asssoc == Side.Right);
			while (t.Count > 0) {
				last = cur;
				cur = t.Dequeue();
				switch (cur) {
					case Identifier id:
						output.Push(id);
						break;
					case Literal l: // does not handle other future operand types
						output.Push(l);
						break;
					case Operator o:
						if (o.Name == "-" && IsUnary(last)) {
							o = Operator.Negative;
						} //else if (IsUnary(last)) throw new Exception("unary operator: " +o.Name);
						else while (lesserPrecedence(o)) {
							ReduceOperator(output, stack.Pop() as Operator);
						}
						stack.Push(o);
						break;
					case Delimeter d when d == Delimeter.LeftParen:
						stack.Push(d);
						arity.Push(1);
						break;
					case Delimeter comma when comma == Delimeter.Comma:
						while (stack.Peek() != Delimeter.LeftParen) {
							ReduceOperator(output, stack.Pop() as Operator);
						}
						arity.Push(arity.Pop() + 1);
						break;
					case Delimeter right when right == Delimeter.RightParen:
						while(stack.Peek() != Delimeter.LeftParen) {
							ReduceOperator(output, stack.Pop() as Operator);
						} stack.Pop();
						int a = arity.Pop();
						Expression temp;
						if (a != 1) {
							Expression[] toadd = new Expression[a];
							for (int i = 0; i < a; i++) {
								toadd[a - 1 - i] = output.Pop();
							}
							temp = new OTuple(toadd);
						} else temp = output.Pop();
						if (output.Count > 0 && output.Peek() is Identifier funcid) {
							output.Pop();
							output.Push(new FunctionCall(funcid, temp as Operand)); //TODO
						} else output.Push(temp);
						break;
					default:
						throw new Exception("unrecognized token");
				}
			}
			while (stack.Count > 0) ReduceOperator(output, stack.Pop() as Operator);

			return output.Pop();
		}

		public static void ReduceOperator(Stack<Expression> stack, Operator op) {
			if(op.Arity == Arity.Binary) {
				Expression temp = stack.Pop();
				stack.Push(new Binary(stack.Pop(), op, temp));
			} else {
				stack.Push(new Unary(stack.Pop(), op));
			}
		}
	}
}
