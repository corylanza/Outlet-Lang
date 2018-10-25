﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.Parsing {
	public static partial class Parser {
		public static Expression NextExpression(LinkedList<IToken> tokens) {
			bool done = false;
			Stack<Expression> output = new Stack<Expression>();
			Stack<IToken> stack = new Stack<IToken>();
			Stack<int> arity = new Stack<int>();
			IToken cur = null, last = null;
			bool ValidToken() =>
				tokens.Count > 0 && tokens.First() is IToken i &&
				(i is Identifier || i is Operand || i is Operator ||
				i is Keyword k && (k == Keyword.True || k == Keyword.False || k == Keyword.Null) ||
				i is Delimeter d && (d != Delimeter.LeftCurly || d != Delimeter.RightCurly));
			bool lesserPrecedence(Operator op) => stack.Count > 0 && stack.Peek() is Operator onstack && (onstack.Precedence < op.Precedence || onstack.Precedence == op.Precedence && onstack.Asssoc == Side.Left);
			while (ValidToken() && !done) {
				last = cur;
				cur = tokens.Dequeue();
				switch (cur) {
					case Identifier id:
						output.Push(id);
						break;
					case Literal l:
						output.Push(l);
						break;
					case AST.Type type:
						output.Push(type);
						if(tokens.First() is Identifier tti) Console.WriteLine("yes");
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
					case Delimeter d when d == Delimeter.LeftParen:
						while (lesserPrecedence(Operator.Dot)) {
							ReduceOperator(output, stack);
						}
						if (!(last is null || last is Operator || last is Delimeter dl && dl.Name != ")")) {
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
						arity.Push(arity.Pop() + 1);
						break;
					case Delimeter right when right == Delimeter.RightParen:
						while (stack.Count > 0 && !(stack.Peek() is Delimeter d && d.Name == "(")) {
							ReduceOperator(output, stack);
						}
						if (stack.Count == 0) {
							tokens.AddFirst(cur);
							done = true;
						} else {
							int a = arity.Pop();
							Expression[] tuple = new Expression[a];
							for (int i = 0; i < a; i++) tuple[a - 1 - i] = output.Pop();
							if (stack.Pop() == Delimeter.FuncParen) output.Push(new FunctionCall(output.Pop(), tuple));
							else output.Push(new TupleLiteral(tuple));
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
			return output.Pop();
		}

		public static void ReduceOperator(Stack<Expression> output, Stack<IToken> stack) {
			if (stack.Count > 0 && stack.Peek() is Operator op) {
				stack.Pop();
				if (op == Operator.Dot) {
					if (output.Count < 2) throw new OutletException("Syntax Error: cannot evalute expression due to imbalanced operators/operands");
					Expression temp = output.Pop();
					output.Push(new Deref(output.Pop(), temp));
				} else if (op == Operator.Equal) {
					if (output.Count < 2) throw new OutletException("Syntax Error: cannot evalute expression due to imbalanced operators/operands");
					Expression temp = output.Pop();
					output.Push(new Assign(output.Pop(), temp));
				} else if (op == Operator.LogicalAnd || op == Operator.LogicalOr) {
					if (output.Count < 2) throw new OutletException("Syntax Error: cannot evalute expression due to imbalanced operators/operands");
					Expression temp = output.Pop();
					output.Push(new ShortCircuit(output.Pop(), op, temp));
				} else if (op.Arity == Arity.Binary) {
					if (output.Count < 2) throw new OutletException("Syntax Error: cannot evalute expression due to imbalanced operators/operands");
					Expression temp = output.Pop();
					output.Push(new Binary(output.Pop(), op, temp));
				} else {
					if (output.Count < 1) throw new OutletException("Syntax Error: cannot evalute expression due to imbalanced operators/operands");
					output.Push(new Unary(output.Pop(), op));
				}
			} else throw new OutletException("Syntax Error: cannot evalute expression due to imbalanced operators/operands");

		}
	}
}
