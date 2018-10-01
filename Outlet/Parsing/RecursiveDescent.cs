using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet.Parsing {
    public static partial class Parser {

        public static Declaration NextDeclaration(Scope block, LinkedList<IToken> tokens) {
            bool Match(IToken s) { if(tokens.Count > 0 && tokens.First() == s) { tokens.RemoveFirst(); return true; } else return false; }
            void Consume(IToken s, string error) { if(tokens.Count == 0 || tokens.Dequeue() != s) throw new OutletException("Syntax Error: " + error); }
            Declaration VariableDeclaration() {
                Identifier name = tokens.Dequeue() as Identifier;
                Expression initializer = null;
                if(Match(Operator.Equal)) initializer = NextExpression(block, tokens);
                else Consume(Delimeter.SemiC, "expected either ; or an initializer after declaring a variable");
                return new VariableDeclaration(name, initializer);
            }
            Declaration FunctionDef() {
                Identifier name = tokens.Dequeue() as Identifier;
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
                Statement body = NextStatement(block, tokens);
                return new FunctionDeclaration(name, argnames, body);
            }
			Declaration ClassDef(){
				Identifier name = tokens.Dequeue() as Identifier;
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
            return NextStatement(block, tokens);
        }

        public static Statement NextStatement(Scope block, LinkedList<IToken> tokens) {
            bool Match(IToken s) { if(tokens.Count > 0 && tokens.First() == s) { tokens.Dequeue(); return true; } else return false; }
            void Consume(IToken s, string error) { if(tokens.Count == 0 || tokens.Dequeue() != s) throw new OutletException("Syntax Error: " + error); }

            Statement Scope() {
                Scope newscope = new Scope(block);
                while(tokens.Count > 0 && tokens.First() != Delimeter.RightCurly) {
                    newscope.Lines.Add(NextDeclaration(newscope, tokens));
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
                if(Match(Keyword.Else)) ifelse = NextStatement(block, tokens);
                return new IfStatement(condition, iftrue, ifelse);
            }
            Statement WhileLoop() {
                Consume(Delimeter.LeftParen, "Expected ( after while");
                Expression condition = NextExpression(block, tokens);
                Consume(Delimeter.RightParen, "Expected ) after while condition");
                Statement iftrue = NextStatement(block, tokens);
                return new WhileLoop(condition, iftrue);
            }
            Statement ForLoop() {
                Consume(Delimeter.LeftParen, "Expected ( after for");
                Identifier loopvar = tokens.Dequeue() as Identifier;
                Consume(Keyword.In, "expected in after for loop variable");
                Expression collection = NextExpression(block, tokens);
                Consume(Delimeter.RightParen, "Expected ) after for loop collection");
                Statement body = NextStatement(block, tokens);
                return new ForLoop(block, loopvar, collection, body);
            }
            Statement Return() {
                Expression e = NextExpression(block, tokens);
                return new ReturnStatement(e);
            }

            if(Match(Delimeter.LeftCurly)) return Scope();
            if(Match(Keyword.If)) return IfStatement();
            if(Match(Keyword.For)) return ForLoop();
            if(Match(Keyword.While)) return WhileLoop();
            if(Match(Keyword.Return)) return Return();
            return NextExpression(block, tokens);
        }
    }
}
