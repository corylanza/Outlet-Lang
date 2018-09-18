using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Expressions;

namespace Outlet.Parsing {
    public static class Parser {
        public static Expression Parse(Queue<IToken> t) {
			Stack<Expression> stack = new Stack<Expression>();
            Stack<Operator> opstack = new Stack<Operator>();
			bool OperatorCondition(Operator op) => opstack.Count > 0 && opstack.Peek() is Operator o && o.Precedence <= op.Precedence && o.Asssoc == Side.Left;
			while (t.Count > 0) {
                IToken cur = t.Dequeue();
                switch(cur) {
                case Literal l:
                    stack.Push(l);
                    break;
                case Operator o:
						while (OperatorCondition(o)) {
							Expression temp = stack.Pop();
							stack.Push(new Binary(stack.Pop(), opstack.Pop(), temp));
						}
                    opstack.Push(o);
                    break;
                default:
                    throw new Exception("unrecognized token");
                }
                
            }
            while(opstack.Count > 0) {
                Expression temp = stack.Pop();
                stack.Push(new Binary(stack.Pop(), opstack.Pop(), temp));
            }
            return stack.Pop();
        }
    }
}
