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
                    stack.Push(new Literal(cur.Text));
                    break;
                default:
                    throw new Exception("unrecognized token");
                }
            }
            return null;//new Literal(t[0].)
        }
    }
}
