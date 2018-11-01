using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.Parsing {
    public static partial class Parser {

        public static Declaration NextDeclaration(LinkedList<IToken> tokens) {
            bool Match(IToken s) {
				if (tokens.Count > 0 && tokens.First() == s) {
					tokens.RemoveFirst(); return true;
				} else return false; }
            void Consume(IToken s, string error) {
				if (tokens.Count == 0 || tokens.Dequeue() != s) throw new OutletException("Syntax Error: " + error);
			}
			T ConsumeType<T>(string error) {
				if (tokens.Count > 0 && tokens.Dequeue() is T t) return t;
				else throw new OutletException("Syntax Error: " + error);
			}
			
            VariableDeclaration VarDeclaration(Declarator decl) {
                Expression initializer = null;
                if(Match(Operator.Equal)) initializer = NextExpression(tokens);
                else Consume(Delimeter.SemiC, "expected either ; or an initializer after declaring a variable");
                return new VariableDeclaration(decl, initializer);
            }
            FunctionDeclaration FunctionDef(Declarator decl) {
                List <Declarator> argnames = new List<Declarator>();
                while(tokens.Count > 0 && tokens.First() != Delimeter.RightParen) {
                    do {
						if (NextExpression(tokens) is Declarator paramdecl) {
							argnames.Add(paramdecl);
						} else throw new OutletException("function parameters expected in type id format");
                    } while(Match(Delimeter.Comma));
                }
                Consume(Delimeter.RightParen, " expected ) after function args");
                Consume(Operator.Lambda, " expected => after function name");
				// TODO check for => and call nextexpression if true
                Statement body = NextStatement(tokens);
                return new FunctionDeclaration(decl, argnames, body);
            }
			ClassDeclaration ClassDef(){
				Identifier name = ConsumeType<Identifier>("Expected class identifier"); ;
				List<Declaration> instance = new List<Declaration>();
				List<Declaration> statics = new List<Declaration>();
				if (Match(Delimeter.LeftCurly)) {
					while (true) {
						if (Match(Delimeter.RightCurly)) break;
						if (tokens.Count == 0) throw new OutletException("expected } after class definition");
						if (Match(Keyword.Static)) {
							Statement nextfield = NextStatement(tokens);
							if (nextfield is Declarator df) {
								if (Match(Delimeter.LeftParen)) statics.Add(FunctionDef(df));
								else statics.Add(VarDeclaration(df));
							} else throw new OutletException("statement: "+nextfield.ToString()+" must be inside a function body");
						} else {
							Statement nextfield = NextStatement(tokens);
							if (nextfield is Declarator df) {
								if (Match(Delimeter.LeftParen)) instance.Add(FunctionDef(df));
								else instance.Add(VarDeclaration(df));
							} else throw new OutletException("statement: " + nextfield.ToString() + " must be inside a function body");
						}
					}
				}
				return new ClassDeclaration(name, instance, statics);
			}
            if (Match(Keyword.Class)) return ClassDef();
			Statement next = NextStatement(tokens);
            if(next is Declarator d) {
				if (Match(Delimeter.LeftParen)) return FunctionDef(d);
				else return VarDeclaration(d);
			}
			return next;
        }

        public static Statement NextStatement(LinkedList<IToken> tokens) {
            bool Match(IToken s) {
				if (tokens.Count > 0 && tokens.First() == s) {
					tokens.Dequeue(); return true;
				} else return false;
			}
            void Consume(IToken s, string error) {
				if (tokens.Count == 0 || tokens.Dequeue() != s) throw new OutletException("Syntax Error: " + error);
			}
			T ConsumeType<T>(string error) {
				if (tokens.Count > 0 && tokens.Dequeue() is T t) return t;
				else throw new OutletException("Syntax Error: " + error);
			}
			Statement Scope() {
				List<Declaration> lines = new List<Declaration>();
                while(tokens.Count > 0 && tokens.First() != Delimeter.RightCurly) {
                    lines.Add(NextDeclaration(tokens));
                }
                Consume(Delimeter.RightCurly, "Expected } to close code block");
				return new Block(lines);
            }
            Statement IfStatement() {
                Consume(Delimeter.LeftParen, "Expected ( after if");
                Expression condition = NextExpression(tokens);
                Consume(Delimeter.RightParen, "Expected ) after if condition");
                Statement iftrue = NextStatement(tokens);
                Statement ifelse = null;
                if(Match(Keyword.Else)) ifelse = NextStatement(tokens);
                return new IfStatement(condition, iftrue, ifelse);
            }
            Statement WhileLoop() {
                Consume(Delimeter.LeftParen, "Expected ( after while");
                Expression condition = NextExpression(tokens);
                Consume(Delimeter.RightParen, "Expected ) after while condition");
                Statement iftrue = NextStatement( tokens);
                return new WhileLoop(condition, iftrue);
            }
            Statement ForLoop() {
                Consume(Delimeter.LeftParen, "Expected ( after for");
                Identifier loopvar = ConsumeType<Identifier>("expected interator identifer");
                Consume(Keyword.In, "expected in after for loop variable");
                Expression collection = NextExpression(tokens);
                Consume(Delimeter.RightParen, "Expected ) after for loop collection");
                Statement body = NextStatement(tokens);
                return new ForLoop(loopvar, collection, body);
            }
            Statement Return() {
                Expression e = NextExpression(tokens);
                return new ReturnStatement(e);
            }

            if(Match(Delimeter.LeftCurly)) return Scope();
            if(Match(Keyword.If)) return IfStatement();
            if(Match(Keyword.For)) return ForLoop();
            if(Match(Keyword.While)) return WhileLoop();
            if(Match(Keyword.Return)) return Return();
            return NextExpression(tokens);
        }
    }
}
