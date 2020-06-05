using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Tokens;
using Outlet.Util;
using Outlet.AST;

namespace Outlet.Parsing {
	public static partial class Parser {


		public static Expression NextExpression(LinkedList<Token> tokens) {
			#region preliminary definitons
			bool expectOperand = true;
			bool done = false;
			var (output, stack, arity) = (new Stack<Expression>(), new Stack<Token>(), new Stack<int>());
			Token? cur = null, last;
			bool ValidToken() =>
				tokens.Count > 0 && tokens.First() is Token i &&
				((i is Delimeter d && (d != Delimeter.LeftCurly && d != Delimeter.RightCurly && d != Delimeter.SemiC)) ||
				i is TokenLiteral || i is Operator || i is Identifier);
			bool NotExpectingOperand(Token toputback) { if(!expectOperand) { tokens.AddFirst(toputback); done = true; return true; } else return false; }
			bool NotExpectingOperator(Token toputback) { if(expectOperand) { tokens.AddFirst(toputback); done = true; return true; } else return false; }
			bool lesserPrecedence(Operator op) => stack.Count > 0 && stack.Peek() is Operator onstack && (onstack.Precedence < op.Precedence || onstack.Precedence == op.Precedence && onstack.Assoc == Side.Left);
			#endregion

			while (ValidToken() && !done) {
				last = cur;
				cur = tokens.Dequeue();
				switch (cur) {
					case Identifier id:
						if(NotExpectingOperand(id)) break;
						output.Push(new Variable(id.Name));
						expectOperand = false;
						break;
					case TokenLiteral literal:
						if(NotExpectingOperand(literal)) break;
                        output.Push(literal.ToLiteral());
						expectOperand = false;
						break;
					case Operator o:
						if(o.Name == "-" && IsPreUnary(last)) o = Operator.Negative;
						if(o.Name == "&" && IsPreUnary(last)) o = Operator.UnaryAnd;
						if(o.Name == "+" && IsPreUnary(last)) o = Operator.UnaryPlus;
						if(o.Name == "++" && IsPreUnary(last)) o = Operator.PreInc;
						if(o.Name == "--" && IsPreUnary(last)) o = Operator.PreDec;

						if(o is UnaryOperator) {
							if(IsPreUnary(last) && NotExpectingOperand(o)) break;
							if(IsPostUnary(tokens.FirstOrDefault()) && NotExpectingOperator(o)) break;
						} 
						if(o is BinaryOperator && NotExpectingOperator(o)) break;
						while (lesserPrecedence(o)) {
							ReduceOperator(output, stack);
						}
						stack.Push(o);
						expectOperand = !IsPostUnary(tokens.FirstOrDefault());
						break;
					case Delimeter colon when colon == Delimeter.Colon:
						if(NotExpectingOperator(colon)) break;
						while(stack.Count > 0 && stack.Peek() != Operator.Question) {
							ReduceOperator(output, stack);
						}
						if (stack.Count > 0) {
							stack.Pop();
							stack.Push(Operator.Ternary);
						} else throw new OutletException("expected ? before : in ternary operator");
						expectOperand = true;
						break;
					case Delimeter d when d == Delimeter.LeftParen:
						bool func = !(last is null || last is Operator || last is Delimeter dlp && !(dlp.Name == ")" || dlp.Name == "]"));
						if(func && NotExpectingOperator(d)) break;
						if(!func && NotExpectingOperand(d)) break;
						while (lesserPrecedence(Operator.Dot)) {
							ReduceOperator(output, stack);
						}
						// If this is a function call push a special function delim to stack, otherwise push (
						stack.Push(func ? Delimeter.FuncParen : d);
						if(tokens.Count > 0 && tokens.First() == Delimeter.RightParen) {
							arity.Push(0);
							expectOperand = false;
						} else {
							arity.Push(1);
							expectOperand = true;
						}
						break;
					case Delimeter lb when lb == Delimeter.LeftBrace:
						bool index = !(last is null || last is Operator || last is Delimeter dlb && !(dlb.Name == ")" || dlb.Name == "]"));
						if(index && NotExpectingOperator(lb)) break;
						if(!index && NotExpectingOperand(lb)) break;
						// If this is an index, push a special array index delim to stack, otherwise push [
						stack.Push(index ? Delimeter.IndexBrace : lb);
						if(tokens.Count > 0 && tokens.First() == Delimeter.RightBrace) {
							arity.Push(0);
							expectOperand = false;
						} else {
							arity.Push(1);
							expectOperand = true;
						}
						break;
					case Delimeter comma when comma == Delimeter.Comma:
						if(NotExpectingOperator(comma)) break;
						while (stack.Count > 0 && !(stack.Peek() is Delimeter d && (d.Name == "(" || d.Name == "["))) {
							ReduceOperator(output, stack);
						}
						if (stack.Count > 0) arity.Push(arity.Pop() + 1);
						else throw new OutletException("Cannot have a comma without being in a grouping or list expression");
						expectOperand = true;
						break;
					case Delimeter right when right == Delimeter.RightParen:
						if(NotExpectingOperator(right)) break;
						while (stack.Count > 0 && !(stack.Peek() is Delimeter d && d.Name == "(")) {
							ReduceOperator(output, stack);
						}
						if (stack.Count == 0) {
							tokens.AddFirst(cur);
							if (output.Count == 1) return output.Pop();
							else throw new OutletException("invalid expression before )");
						} else {
							int tuplen = arity.Pop();
							Expression[] tuple = new Expression[tuplen];
							for (int i = 0; i < tuplen; i++) tuple[tuplen - 1 - i] = output.Pop();
							// If there is a func paren then this is a function call, otherwise it is a tuple literal
							output.Push(stack.Pop() == Delimeter.FuncParen ? new Call(output.Pop(), tuple) as Expression : new TupleLiteral(tuple));
						}
						expectOperand = false;
						break;
					case Delimeter rightb when rightb == Delimeter.RightBrace:
						if(NotExpectingOperator(rightb)) break;
						while (stack.Count > 0 && !(stack.Peek() is Delimeter d && d.Name == "[")) {
							ReduceOperator(output, stack);
						}
						int idxlen = arity.Pop();
						Expression[] list = new Expression[idxlen];
						for (int i = 0; i < idxlen; i++) {
							list[idxlen - 1 - i] = output.Pop();
						}
						// If there is an index brace this is a access expr, otherwise it is a list literal
						output.Push(stack.Pop() == Delimeter.IndexBrace ? new Access(output.Pop(), list) as Expression : new ListLiteral(list));
						expectOperand = false;
						break;
					default:
						throw new OutletException("unexpected token in expression: " + cur.ToString());
				}
				if(done) break;
			}
			if(expectOperand) throw new OutletException("expression incomplete, expects additonal operand");
			while (stack.Count > 0) ReduceOperator(output, stack);
			if(output.Count == 1) return output.Pop();
			throw new OutletException("Expression invalid, more operands than needed operators");
		}

		public static void ReduceOperator(Stack<Expression> output, Stack<Token> stack) {
			if (stack.Count > 0 && stack.Peek() is Operator op) {
				stack.Pop();
				if(op == Operator.Ternary) {
					if(output.Count < 3) throw new OutletException("Syntax Error: ternary operator expects 3 operands");
					output.Push(new Ternary(output.Pop(), output.Pop(), output.Pop()));
				} else if(op is BinaryOperator binop) {
					if(output.Count < 2) throw new OutletException("Syntax Error: binary operator " + binop.ToString() + " expects 2 operands");
					output.Push(binop.Construct(output.Pop(), output.Pop()));
				} else if(op is UnaryOperator unop) {
					if(output.Count < 1) throw new OutletException("Syntax Error: unary operator " + unop.ToString() + " expects 1 operand");
					output.Push(new Unary(unop.Name, output.Pop(), unop.Overloads));
				} else throw new OutletException("Syntax Error: Incomplete expression, tried to reduce");
			} else throw new OutletException("Expression invalid, more operators than needed operands");
		}

        public static Literal ToLiteral(this TokenLiteral literal)
        {
            return literal switch {
                IntLiteral i => new Literal<int>(i.Value),
                FloatLiteral f => new Literal<float>(f.Value),
                BoolLiteral b => new Literal<bool>(b.Value),
				Tokens.StringLiteral s => new AST.StringLiteral(s.Value),
                NullLiteral _ => new NullExpr(),
                _ => throw new NotImplementedException()
            }; 
        }
	}
}
