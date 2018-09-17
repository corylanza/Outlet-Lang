using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Expressions;

namespace Outlet.Parsing {
    public static class Parser {
        public static Expression Parse(Queue<Token> t) {
            Stack<Expression> stack = new Stack<Expression>();
            while(t.Count > 0) {
                Token cur = t.Dequeue();
                switch(cur.Type) {
                case TokenType.OInt:
                    stack.Push(new Literal((int) cur.Value));
                    break;
                case TokenType.OString:
                    stack.Push(new Literal((string) cur.Value));
                    break;
                case TokenType.OFloat:
                    stack.Push(new Literal((float) cur.Value));
                    break;
                case TokenType.True:
                    stack.Push(new Literal(true));
                    break;
                case TokenType.False:
                    stack.Push(new Literal(false));
                    break;

                default:
                    throw new Exception("unrecognized token");
                }
            }
            return stack.Pop();
        }
    }
}
