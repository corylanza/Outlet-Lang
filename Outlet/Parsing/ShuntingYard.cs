using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Tokens;
using Outlet.AST;

namespace Outlet.Parsing {
	public static partial class Parser {
		public static Expression NextExpression(LinkedList<Token> tokens) {
			#region preliminary definitons
			bool done = false;
			Stack<Expression> output = new Stack<Expression>();
			Stack<Token> stack = new Stack<Token>();
			Stack<int> arity = new Stack<int>();
			Token cur = null, last = null;
			bool ValidToken() =>
				tokens.Count > 0 && tokens.First() is Token i && (!(i is Keyword) ||
				 (i is Keyword k && (k == Keyword.True || k == Keyword.False || k == Keyword.Null) ||
				!(i is Delimeter) || i is Delimeter d && (d != Delimeter.LeftCurly || d != Delimeter.RightCurly)));
			bool lesserPrecedence(Operator op) => stack.Count > 0 && stack.Peek() is Operator onstack && (onstack.Precedence < op.Precedence || onstack.Precedence == op.Precedence && onstack.Asssoc == Side.Left);
			#endregion

			while (ValidToken() && !done) {
				last = cur;
				cur = tokens.Dequeue();
				switch (cur) {
					case Identifier id:
						if (output.Count == 1 && stack.Count == 0) {
							return new Declarator(output.Pop(), id.Name);
						} else output.Push(new Variable(id.Name));
						break;
					case Literal l:
						output.Push(new Constant(l.Value));
						break;
					case Keyword k:
						if (k == Keyword.True) output.Push(new Constant(true));
						else if (k == Keyword.False) output.Push(new Constant(false));
						else output.Push(new Constant());//throw new NotImplementedException("null is not implemented");
						break;
					case Operator o:
						if (o.Name == "-" && IsPreUnary(last)) {
							o = Operator.Negative;
						}
						if (o.Name == "++" && IsPreUnary(last)) {
							o = Operator.PreInc;
						}
						if (o.Name == "--" && IsPreUnary(last)) {
							o = Operator.PreDec;
						}
						/*bool b = IsBinary(last);
						bool pre = IsPreUnary(last);
						bool post = IsPostUnary(tokens.Head());
						if (pre && post) throw new Exception("BOTH");
						if (pre) Console.WriteLine("preunary" + o.ToString());
						else if (post) Console.WriteLine("postunary" + o.ToString());
						else Console.WriteLine("binary" + o.ToString());
						if (IsPreUnary(last)) throw new Exception("preunary operator: " +o.Name);
						if (IsPostUnary(tokens.First())) throw new Exception("postunary operator: " + o.Name);
						else*/
						while (lesserPrecedence(o)) {
							ReduceOperator(output, stack);
						}
						stack.Push(o);
						break;
					case Delimeter colon when colon == Delimeter.Colon:
						while(stack.Count > 0 && stack.Peek() != Operator.Question) {
							ReduceOperator(output, stack);
						}
						if (stack.Count > 0) {
							stack.Pop();
							stack.Push(Operator.Ternary);
						} else throw new OutletException("expected ? before : in ternary operator");
						break;
					case Delimeter d when d == Delimeter.LeftParen:
						while (lesserPrecedence(Operator.Dot)) {
							ReduceOperator(output, stack);
						}
						if (!(last is null || last is Operator || last is Delimeter dl && !(dl.Name == ")" || dl.Name == "]"))) {
							stack.Push(Delimeter.FuncParen);
						} else stack.Push(d);
						if (tokens.Count > 0 && tokens.First() == Delimeter.RightParen) arity.Push(0);
						else arity.Push(1);
						break;
					case Delimeter lb when lb == Delimeter.LeftBrace:
						stack.Push(lb);
						if (tokens.Count > 0 && tokens.First() == Delimeter.RightBrace) arity.Push(0);
						else arity.Push(1);
						break;
					case Delimeter comma when comma == Delimeter.Comma:
						while (stack.Count > 0 && !(stack.Peek() is Delimeter d && (d.Name == "(" || d.Name == "["))) {
							ReduceOperator(output, stack);
						}
						if (stack.Count > 0) arity.Push(arity.Pop() + 1);
						else throw new OutletException("Cannot have a comma without being in a grouping or list expression");
						break;
					case Delimeter right when right == Delimeter.RightParen:
						while (stack.Count > 0 && !(stack.Peek() is Delimeter d && d.Name == "(")) {
							ReduceOperator(output, stack);
						}
						if (stack.Count == 0) {
							tokens.AddFirst(cur);
							if (output.Count == 1) return output.Pop();
							else throw new OutletException("invalid expression before )");
						} else {
							int a = arity.Pop();
							Expression[] tuple = new Expression[a];
							for (int i = 0; i < a; i++) tuple[a - 1 - i] = output.Pop();
							if (stack.Pop() == Delimeter.FuncParen) output.Push(new Call(output.Pop(), tuple));
							else output.Push(new TupleLiteral(tuple));
						}
						break;
					case Delimeter rightb when rightb == Delimeter.RightBrace:
						while (stack.Count > 0 && stack.Peek() != Delimeter.LeftBrace) {
							ReduceOperator(output, stack);
						}
						if (stack.Count > 0) stack.Pop();
						else throw new OutletException("expected [ before ] in list literal");
						int addnum = arity.Pop();
						Expression[] list = new Expression[addnum];
						for (int i = 0; i < addnum; i++) {
							list[addnum - 1 - i] = output.Pop();
						}
						output.Push(new ListLiteral(list));
						break;
					case Delimeter semicolon when semicolon == Delimeter.SemiC:
						done = true;
						break;
					default:
						done = true;
						break;
				}
			}
			while (stack.Count > 0) ReduceOperator(output, stack);
			if(output.Count == 1) return output.Pop();
			throw new OutletException("Expression invalid, more operands than needed operators");
		}

		public static void ReduceOperator(Stack<Expression> output, Stack<Token> stack) {
			if (stack.Count > 0 && stack.Peek() is Operator op) {
				stack.Pop();
				if(op == Operator.Ternary) {
					if (output.Count < 3) throw new OutletException("Syntax Error: ternary operator expects 3 operands");
					output.Push(new Ternary(output.Pop(), output.Pop(), output.Pop()));
				} else if(op is BinaryOperator binop) {
					if (output.Count < 2) throw new OutletException("Syntax Error: binary operator "+binop.ToString()+" expects 2 operands");
					output.Push(binop.Construct(output.Pop(), output.Pop()));
				}else if(op is UnaryOperator unop){
					if (output.Count < 1) throw new OutletException("Syntax Error: unary operator "+unop.ToString()+" expects 1 operand");
					output.Push(new Unary(output.Pop(), unop));
				}
			} else throw new OutletException("Expression invalid, more operators than needed operands");
		}
	}
}
