using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.Parsing {
	public static class Parser {

		private static bool IsBinary(IToken last) => last is Operand || last == Delimeter.RightParen;
		private static bool IsUnary(IToken last) => last is null || last is Operator || last == Delimeter.LeftParen || last == Delimeter.Comma;

		public static Statement Parse(Queue<IToken> tokens) => Parse(new Scope(), tokens);

		public static Statement Parse(Scope block, Queue<IToken> tokens) {
			bool Match(IToken s) { if (tokens.Count > 0 && tokens.Peek() == s) { tokens.Dequeue(); return true; } else return false; }
			void Consume(IToken s, string error) { if (tokens.Dequeue() != s) throw new Exception(error); }
			Statement VariableDeclaration() {
				Identifier name = tokens.Dequeue() as Identifier;
				Expression initializer = null;
				if (Match(Operator.Equal)) initializer = NextExpression(block, tokens);
				return new Assignment(block, name, initializer);
			}
			Statement Scope() {
				Scope newscope = new Scope(block);
				Statement outscope = Parse(newscope, tokens);
				//Consume(Delimeter.RightCurly, "Expected } to close code block");
				return outscope;
			}
			Statement IfStatement() {
				Expression condition = NextExpression(block, tokens);
				Statement iftrue = Parse(block, tokens);
				return new Conditional(condition, iftrue);
			}
			if (tokens.Count == 0 && block.Lines.Count == 1) return block.Lines[0];
			if (tokens.Count == 0) return block;
			if (Match(Keyword.Var)) {
				block.Lines.Add(VariableDeclaration());
				return Parse(block, tokens);
			}
			if (Match(Delimeter.LeftCurly)) {
				block.Lines.Add(Scope());
				return Parse(block, tokens);
			};
			if (Match(Keyword.If)) {
				block.Lines.Add(IfStatement());
				return Parse(block, tokens);
			}
			if (Match(Delimeter.RightCurly)) return block;
			block.Lines.Add(NextExpression(block, tokens));
			return Parse(block, tokens);
		}

		public static Expression NextExpression(Scope s, Queue<IToken> tokens) {
			bool done = false;
			Stack<Expression> output = new Stack<Expression>();
			Stack<IToken> stack = new Stack<IToken>();
			Stack<int> arity = new Stack<int>();
			IToken cur = null, last = null;
			bool lesserPrecedence(Operator op) => stack.Count > 0 && stack.Peek() is Operator o && (o.Precedence <= op.Precedence || o.Asssoc == Side.Right);
			while (tokens.Count > 0 && !done) {
				last = cur;
				cur = tokens.Peek();
				switch (cur) {
					case Identifier id:
						output.Push(id);
						id.SetScope(s);
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
							output.Push(new FunctionCall(funcid, temp)); //TODO
						} else output.Push(temp);
						break;
					case Delimeter semicolon when semicolon == Delimeter.SemiC:
						done = true;
						tokens.Dequeue();
						break;
					default:
						done = true;
						break;
				}
				if (!done) tokens.Dequeue();
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
