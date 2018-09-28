using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.Parsing {
	public static class Parser {

		private static bool IsBinary(IToken last) => 
			last is Operand || last == Delimeter.RightParen || last == Delimeter.RightBrace;
		private static bool IsUnary(IToken last) => 
			last is null || last is Operator || last == Delimeter.LeftParen || last == Delimeter.LeftBrace || last == Delimeter.Comma;

		public static Statement Parse(Queue<IToken> tokens) => Parse(new Scope(), tokens);

		public static Statement Parse(Scope block, Queue<IToken> tokens) {
			while(tokens.Count > 0) {
				block.Lines.Add(NextStatement(block, tokens));
			}
			if (block.Lines.Count == 1 && block.Lines[0] is Expression e) return e;
			return block;
		}
		public static Statement NextStatement(Scope block, Queue<IToken> tokens) {
			bool Match(IToken s) { if (tokens.Count > 0 && tokens.Peek() == s) { tokens.Dequeue(); return true; } else return false; }
			void Consume(IToken s, string error) { if (tokens.Count == 0 || tokens.Dequeue() != s) throw new OutletException("Syntax Error: "+error); }
			Statement VariableDeclaration() {
				Identifier name = tokens.Dequeue() as Identifier;
				Expression initializer = null;
				if (Match(Operator.Equal)) initializer = NextExpression(block, tokens);
				return new Assignment(block, name, initializer);
			}
			Statement Scope() {
				Scope newscope = new Scope(block);
				while (tokens.Count > 0 && tokens.Peek() != Delimeter.RightCurly) {
					newscope.Lines.Add(NextStatement(newscope, tokens));
				}
				Consume(Delimeter.RightCurly, "Expected } to close code block");
				return newscope;
			}
			Statement IfStatement() {
                Consume(Delimeter.LeftParen, "Expected ( after if");
                Expression condition = NextExpression(block, tokens);
                Consume(Delimeter.RightParen, "Expected ) after if condition");
                Statement iftrue = NextStatement(block, tokens);
                Statement ifelse = null;
				if (Match(Keyword.Else)) ifelse = NextStatement(block, tokens);
				return new IfStatement(condition, iftrue, ifelse);
			}
			Statement WhileLoop() {
                Consume(Delimeter.LeftParen, "Expected ( after while");
                Expression condition = NextExpression(block, tokens);
                Consume(Delimeter.RightParen, "Expected ) after while condition");
                Statement iftrue = NextStatement(block, tokens);
				return new WhileLoop(condition, iftrue);
			}
			Statement Return() {
				Expression e = NextExpression(block, tokens);
				return new ReturnStatement(e);
			}
			Statement Function() {
				Identifier name = tokens.Dequeue() as Identifier;
				Consume(Operator.Equal, " expected = after function name");
				return null;
			}
			if (Match(Keyword.Var)) return VariableDeclaration();
			if (Match(Delimeter.LeftCurly)) return Scope();
			if (Match(Keyword.If)) return IfStatement();
			//if (Match(Keyword.For))
			if (Match(Keyword.While)) return WhileLoop();
			if (Match(Keyword.Return)) return Return();
			if (Match(Keyword.Func)) return Function();
			//if (Match(Keyword.Class))
			return NextExpression(block, tokens);
		}

		public static Expression NextExpression(Scope s, Queue<IToken> tokens) {
			bool done = false;
			Stack<Expression> output = new Stack<Expression>();
			Stack<IToken> stack = new Stack<IToken>();
			Stack<int> arity = new Stack<int>();
			IToken cur = null, last = null;
			bool ValidToken() => 
				tokens.Count > 0 && tokens.Peek() is IToken i &&
				(i is Identifier || i is Operand || i is Operator ||
				i is Keyword k && (k == Keyword.True || k == Keyword.False || k == Keyword.Null) ||
				i is Delimeter d && (d != Delimeter.LeftCurly || d != Delimeter.RightCurly));
			bool lesserPrecedence(Operator op) => stack.Count > 0 && stack.Peek() is Operator onstack && (onstack.Precedence < op.Precedence || onstack.Precedence == op.Precedence && onstack.Asssoc == Side.Left);
			while (ValidToken() && !done) {
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
					case Keyword k:
						if (k == Keyword.True) output.Push(new Literal(true));
						else if (k == Keyword.False) output.Push(new Literal(false));
						else throw new NotImplementedException("null is not implemented");
						break;
					case Operator o:
						if (o.Name == "-" && IsUnary(last)) {
							o = Operator.Negative;
						} //else if (IsUnary(last)) throw new Exception("unary operator: " +o.Name);
						else while (lesserPrecedence(o)) {
							ReduceOperator(output, stack);
						}
						stack.Push(o);
						break;
					case Delimeter d when d == Delimeter.LeftParen || d == Delimeter.LeftBrace:
						stack.Push(d);
						arity.Push(1);
						break;
					case Delimeter comma when comma == Delimeter.Comma:
						while (stack.Peek() != Delimeter.LeftParen && stack.Peek() != Delimeter.LeftBrace) {
							ReduceOperator(output, stack);
						}
						arity.Push(arity.Pop() + 1);
						break;
					case Delimeter right when right == Delimeter.RightParen:
						while(stack.Count > 0 && stack.Peek() != Delimeter.LeftParen) {
							ReduceOperator(output, stack);
						}
                        if(stack.Count == 0) done = true; 
                        else {
                            stack.Pop();
                            int a = arity.Pop();
                            Expression temp;
                            if(a != 1) {
                                Expression[] tuple = new Expression[a];
                                for(int i = 0; i < a; i++) {
                                    tuple[a - 1 - i] = output.Pop();
                                }
                                temp = new OTuple(tuple);
                            } else temp = output.Pop();
                            if(output.Count > 0 && output.Peek() is Identifier funcid) {
                                output.Pop();
                                output.Push(new FunctionCall(funcid, temp)); //TODO
                            } else output.Push(temp);
                    }
						break;
					case Delimeter rightb when rightb == Delimeter.RightBrace:
						while (stack.Peek() != Delimeter.LeftBrace) {
							ReduceOperator(output, stack);
						}
						stack.Pop();
						int addnum = arity.Pop();
						Expression[] list = new Expression[addnum];
						for (int i = 0; i < addnum; i++) {
							list[addnum - 1 - i] = output.Pop();
						}
						output.Push(new OList(list));
						break;
					case Delimeter semicolon when semicolon == Delimeter.SemiC:
                    tokens.Dequeue();
						done = true;
						break;
					default:
						done = true;
						break;
				}
                if(!done) tokens.Dequeue();
			}
			while (stack.Count > 0) ReduceOperator(output, stack);
			return output.Pop();
		}

		public static void ReduceOperator(Stack<Expression> output, Stack<IToken> stack) {
            if(stack.Count > 0 && stack.Peek() is Operator op) {
                stack.Pop();
                if(op.Arity == Arity.Binary) {
                    if(output.Count < 2) throw new OutletException("Syntax Error: cannot evalute expression due to imbalanced operators/operands");
                    Expression temp = output.Pop();
                    output.Push(new Binary(output.Pop(), op, temp));
                } else {
                    if(output.Count < 1) throw new OutletException("Syntax Error: cannot evalute expression due to imbalanced operators/operands");
                    output.Push(new Unary(output.Pop(), op));
                }
            } else throw new OutletException("Syntax Error: cannot evalute expression due to imbalanced operators/operands");
			
		}
	}
}
