using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Tokens;
using Outlet.Util;
using Outlet.AST;
using Outlet.Operators;

namespace Outlet.Parsing {
	public partial class Parser {

		private Expression NextExpression()
        {
			if(Tokens.Count == 0) throw SyntaxError(expected: "expression", found: null);
			return NextExpression(Tokens.First());
        }

		private Expression NextExpression(Lexeme cur) {
			#region preliminary definitons
			bool expectOperand = true;
			bool done = false;
			var (output, stack, arity) = (new Stack<Expression>(), new Stack<IOperatorPrecedenceParsable>(), new Stack<int>());
			Lexeme first = cur;
			Lexeme? last = null;
			bool ValidToken() =>
                PeekMatchType(out Token? i) &&
				((i is DelimeterToken d && (d != DelimeterToken.LeftCurly && d != DelimeterToken.RightCurly && d != DelimeterToken.SemiC)) ||
				i is TokenLiteral || i is OperatorToken || i is Identifier);

			bool ExpectingOperator(Lexeme toputback) { if(!expectOperand) { Tokens.AddFirst(toputback); done = true; return true; } else return false; }
			bool ExpectingOperand(Lexeme toputback) { if(expectOperand) { Tokens.AddFirst(toputback); done = true; return true; } else return false; }
			bool lesserPrecedence(Operator op) => stack.Count > 0 && stack.Peek() is Operator onstack && (onstack.Precedence < op.Precedence || onstack.Precedence == op.Precedence && onstack.Association == Side.Left);
			SyntaxException SyntaxError(string message) => this.SyntaxError(message, first);
			
			#endregion

			while (ValidToken() && !done) {
				last = cur;
				cur = Tokens.Dequeue();
				switch (cur.InnerToken) {
					case Identifier id:
						if(!expectOperand && output.Count > 0)
                        {
							// for cases with declarators in expressions such as
							// tuple assignment: (int a, int b) = (3, 5) 
							// lambdas: (int a, int b) => a + b;
							// multiple assignment: int a = int b = 5;
							var typeExpr = output.Pop();
							output.Push(new Declarator(typeExpr, id.Name));
							expectOperand = false;
						}
						else
                        {
							if (ExpectingOperator(cur)) break;
							output.Push(new Variable(id.Name));
							expectOperand = false;
						}
						break;
					case TokenLiteral literal:
						if(ExpectingOperator(cur)) break;
                        output.Push(literal.ToLiteral());
						expectOperand = false;
						break;
					case OperatorToken o:
						var isBinary = last != null && IsBinary(last.InnerToken);
						var isPreUnary = IsPreUnary(last?.InnerToken);
						var isPostUnary = IsPostUnary(Tokens.FirstOrDefault()?.InnerToken);
						Operator op =
							isBinary && o.HasBinaryOperation(out var binop) ? binop :
							isPreUnary && o.HasPreUnaryOperation(out var unop) ? unop :
							isPostUnary && o.HasPostUnaryOperation(out var postUnOp) ? postUnOp :
							throw new UnexpectedException("An operator should always be either binary, pre unary, or post unary, something has gone wrong");

						if(op is UnaryOperator) {
							if(isPreUnary && ExpectingOperator(cur)) break;
							if(isPostUnary && ExpectingOperand(cur)) break;
						} 
						if(op is BinaryOperator && ExpectingOperand(cur)) break;

						while(lesserPrecedence(op)) {
							ReduceOperator(output, stack);
						}
						stack.Push(op);
						expectOperand = isBinary || isPreUnary;
						break;
					case DelimeterToken colon when colon == DelimeterToken.Colon:
						if(ExpectingOperand(cur)) break;
						while(stack.Count > 0 && stack.Peek() is not TernaryQuestion) {
							ReduceOperator(output, stack);
						}
						if (stack.Count > 0) {
							stack.Pop();
							stack.Push(new TernaryElse());
						} else throw new OutletException("expected ? before : in ternary operator");
						expectOperand = true;
						break;
					case DelimeterToken d when d == DelimeterToken.LeftParen:
						bool func = !(last is null || last.InnerToken is OperatorToken || last.InnerToken is DelimeterToken dlp && !(dlp.Name == ")" || dlp.Name == "]"));
						if(func && ExpectingOperand(cur)) break;
						if(!func && ExpectingOperator(cur)) break;
						while (lesserPrecedence(new DotOp())) {
							ReduceOperator(output, stack);
						}
						// If this is a function call push a special function delim to stack, otherwise push (
						stack.Push(func ? Delimeter.FuncParen : Delimeter.LeftParen);
						if(PeekMatch(DelimeterToken.RightParen)) {
							arity.Push(0);
							expectOperand = false;
						} else {
							arity.Push(1);
							expectOperand = true;
						}
						break;
					case DelimeterToken lb when lb == DelimeterToken.LeftBrace:
						bool index = !(last is null || last.InnerToken is OperatorToken || last.InnerToken is DelimeterToken dlb && !(dlb.Name == ")" || dlb.Name == "]"));
						if(index && ExpectingOperand(cur)) break;
						if(!index && ExpectingOperator(cur)) break;
						// If this is an index, push a special array index delim to stack, otherwise push [
						stack.Push(index ? Delimeter.IndexBrace : Delimeter.LeftBrace);
						if(PeekMatch(DelimeterToken.RightBrace)) {
							arity.Push(0);
							expectOperand = false;
						} else {
							arity.Push(1);
							expectOperand = true;
						}
						break;
					case DelimeterToken comma when comma == DelimeterToken.Comma:
						if(ExpectingOperand(cur)) break;
						while (stack.Count > 0 && !(stack.Peek() is Delimeter d && (d.Name == "(" || d.Name == "["))) {
							ReduceOperator(output, stack);
						}
						if (stack.Count > 0) arity.Push(arity.Pop() + 1);
						else throw new OutletException("Cannot have a comma without being in a grouping or list expression");
						expectOperand = true;
						break;
					case DelimeterToken right when right == DelimeterToken.RightParen:
						if(ExpectingOperand(cur)) break;
						while (stack.Count > 0 && !(stack.Peek() is Delimeter d && d.Name == "(")) {
							ReduceOperator(output, stack);
						}
						if (stack.Count == 0) {
							Tokens.AddFirst(cur);
							if (output.Count == 1) return output.Pop();
							else throw new OutletException("invalid expression before )");
						} else {
							int tuplen = arity.Pop();
							Expression[] tuple = new Expression[tuplen];
							for (int i = 0; i < tuplen; i++) tuple[tuplen - 1 - i] = output.Pop();
							// If there is a func paren then this is a function call, otherwise it is a tuple literal if multiple values
							output.Push(stack.Pop() == Delimeter.FuncParen ? new Call(output.Pop(), tuple) : tuplen == 1 ? tuple[0] : new TupleLiteral(tuple));
						}
						expectOperand = false;
						break;
					case DelimeterToken rightb when rightb == DelimeterToken.RightBrace:
						if(ExpectingOperand(cur)) break;
						while (stack.Count > 0 && !(stack.Peek() is Delimeter d && d.Name == "[")) {
							ReduceOperator(output, stack);
						}
						int idxlen = arity.Pop();
						Expression[] list = new Expression[idxlen];
						for (int i = 0; i < idxlen; i++) {
							list[idxlen - 1 - i] = output.Pop();
						}
						// If there is an index brace this is a access expr, otherwise it is a list literal
						output.Push(stack.Pop() == Delimeter.IndexBrace ? new ArrayAccess(output.Pop(), list) as Expression : new ListLiteral(list));
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

		private static void ReduceOperator(Stack<Expression> output, Stack<IOperatorPrecedenceParsable> stack) {
			if (stack.Count > 0 && stack.Peek() is Operator op) {
				stack.Pop();
				if(op is TernaryElse) {
					if(output.Count < 3) throw new OutletException("Syntax Error: ternary operator expects 3 operands");
					output.Push(new Ternary(output.Pop(), output.Pop(), output.Pop()));
				} else if(op is BinaryOperator binop) {
					if(output.Count < 2) throw new OutletException("Syntax Error: binary operator " + binop.ToString() + " expects 2 operands");
					var right = output.Pop();
					var left = output.Pop();
					output.Push(binop.GenerateAstNode(left, right));
				} else if(op is UnaryOperator unop) {
					if(output.Count < 1) throw new OutletException("Syntax Error: unary operator " + unop.ToString() + " expects 1 operand");
					output.Push(unop.GenerateAstNode(output.Pop()));
				} else throw new OutletException("Syntax Error: Incomplete expression, tried to reduce");
			} else throw new OutletException("Expression invalid, more operators than needed operands");
		}
	}
}
