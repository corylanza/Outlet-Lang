using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.Parsing {
    public static partial class Parser {

        public static Declaration NextDeclaration(LinkedList<IToken> tokens) {
            bool Match(IToken s) { if(tokens.Count > 0 && tokens.First() == s) { tokens.RemoveFirst(); return true; } else return false; }
            void Consume(IToken s, string error) { if(tokens.Count == 0 || tokens.Dequeue() != s) throw new OutletException("Syntax Error: " + error); }
			T ConsumeType<T>(string error) { if (tokens.Count > 0 && tokens.Dequeue() is T t) return t; else throw new OutletException("Syntax Error: " + error); }
            Declaration VariableDeclaration() {
                Identifier name = ConsumeType<Identifier>("Expected variable identifier");
                Expression initializer = null;
                if(Match(Operator.Equal)) initializer = NextExpression(tokens);
                else Consume(Delimeter.SemiC, "expected either ; or an initializer after declaring a variable");
                return new VariableDeclaration(name, initializer);
            }
            Declaration FunctionDef() {
                Identifier name = ConsumeType<Identifier>("Expected function identifier"); ;
                Consume(Delimeter.LeftParen, "expected ( after function name");
                List <Identifier> argnames = new List<Identifier>();
                while(tokens.Count > 0 && tokens.First() != Delimeter.RightParen) {
                    do {
                        if(tokens.First() is Identifier argname) {
                            tokens.Dequeue();
                            argnames.Add(argname);
                        } else throw new OutletException("Only identifiers can be used as args");
                    } while(Match(Delimeter.Comma));
                }
                Consume(Delimeter.RightParen, " expected ) after function args");
                Consume(Operator.Equal, " expected = after function name");
				// TODO check for => and call nextexpression if true
                Statement body = NextStatement(tokens);
                return new FunctionDeclaration(name, argnames, body);
            }
			Declaration ClassDef(){
				Identifier name = ConsumeType<Identifier>("Expected class identifier"); ;
				Consume(Delimeter.LeftParen, "expected ( after function name");
				List<Identifier> argnames = new List<Identifier>();
				while (tokens.Count > 0 && tokens.First() != Delimeter.RightParen) {
					do {
						if (tokens.First() is Identifier argname) {
							tokens.Dequeue();
							argnames.Add(argname);
						} else throw new OutletException("Only identifiers can be used as args");
					} while (Match(Delimeter.Comma));
				}
				Consume(Delimeter.RightParen, " expected ) after function args");
				//TODO check for { after and use that scope as body
				return new ClassDeclaration(name, argnames);
			}
            if(Match(Keyword.Var)) return VariableDeclaration();
            if(Match(Keyword.Func)) return FunctionDef();
            if (Match(Keyword.Class)) return ClassDef();
            return NextStatement(tokens);
        }

        public static Statement NextStatement(LinkedList<IToken> tokens) {
            bool Match(IToken s) { if(tokens.Count > 0 && tokens.First() == s) { tokens.Dequeue(); return true; } else return false; }
            void Consume(IToken s, string error) { if(tokens.Count == 0 || tokens.Dequeue() != s) throw new OutletException("Syntax Error: " + error); }
			T ConsumeType<T>(string error) { if (tokens.Count > 0 && tokens.Dequeue() is T t) return t; else throw new OutletException("Syntax Error: " + error); }

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
