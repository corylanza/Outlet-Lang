using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Tokens;
using Outlet.AST;

namespace Outlet.Parsing {
	public partial class Parser {

		private IASTNode NextDeclaration() {
			#region helper
			(List<Declarator>, Statement) ProtoType() {
				List<Declarator> argnames = new List<Declarator>();
				while(Tokens.Count > 0 && Tokens.First() != DelimeterToken.RightParen) {
					do {
						if(NextStatement() is Declarator paramdecl) {
							argnames.Add(paramdecl);
						} else throw new OutletException("function parameters expected in type id format");
					} while(Match(DelimeterToken.Comma));
				}
				Consume(DelimeterToken.RightParen, " expected ) after function args");
				Statement body;
				if(Match(OperatorToken.Lambda)) {
					body = NextExpression();
					if(Tokens.Count != 0)
						Consume(DelimeterToken.SemiC, "expected ; after inline function");
				} else body = NextStatement();
				return (argnames, body);
			}
			#endregion
			VariableDeclaration VarDeclaration(Declarator decl) {
				Expression? initializer = null;
				if(Match(OperatorToken.Equal)) initializer = NextExpression();
				if(Tokens.Count != 0)
					Consume(DelimeterToken.SemiC, "expected ; after declaring a variable");
				return new VariableDeclaration(decl, initializer);
			}
			FunctionDeclaration FunctionDef(Declarator decl) {
				(List<Declarator> argnames, Statement body) = ProtoType();
				return new FunctionDeclaration(decl, argnames, body);
			}
			ConstructorDeclaration ConstructDef(Declarator decl) {
				(List<Declarator> argnames, Statement body) = ProtoType();
				return new ConstructorDeclaration(decl, argnames, body);
			}
			OperatorOverloadDeclaration OperatorOverloadDef(Declarator decl, OperatorToken op)
			{
				(List<Declarator> argnames, Statement body) = ProtoType();
				return new OperatorOverloadDeclaration(decl, op, argnames, body);
            }
			ClassDeclaration ClassDef() {
                List<(string id, Variable? constraint)> genericParameters = new List<(string, Variable?)>();
                List<Declaration> instance = new List<Declaration>();
				List<Declaration> statics = new List<Declaration>();
				List<ConstructorDeclaration> constructors = new List<ConstructorDeclaration>();
				Variable? superclass = null;
                Identifier name = ConsumeType<Identifier>("Expected class identifier");
                if (Match(DelimeterToken.LeftBrace))
                {
                    string genericId = ConsumeType<Identifier>("Generic class must have at least one identifier as a generic parameter").Name;
                    if (Match(Keyword.Extends)) genericParameters.Add((genericId, 
                        new Variable(ConsumeType<Identifier>("expected class constraint on generic parameter " + genericId).Name)));
                    else genericParameters.Add((genericId, null));
                    while(Tokens.Count > 0 && Tokens.First() != DelimeterToken.RightBrace)
                    {
                        Consume(DelimeterToken.Comma, "commas must be used between generic parameters");
                        genericId = ConsumeType<Identifier>("Generic class parameters must be identifiers").Name;

                        if (Match(Keyword.Extends)) genericParameters.Add((genericId,
                        new Variable(ConsumeType<Identifier>("expected class constraint on generic parameter " + genericId).Name)));
                        else genericParameters.Add((genericId, null));
                    }
                    Consume(DelimeterToken.RightBrace, "expected ] to close generic type definition");
                }
                if (Match(Keyword.Extends)) {
					 superclass = new Variable(ConsumeType<Identifier>("expected name of super class after extends keyword").Name);
				}
				if(Match(DelimeterToken.LeftCurly)) {
					while(true) {
						if(Match(DelimeterToken.RightCurly)) break;
						if(Tokens.Count == 0) throw new OutletException("expected } after class definition");
						if(Match(name)) {
							if(Match(DelimeterToken.LeftParen)) {
								Declarator constr = new Declarator(new Variable(name.Name), "");
								constructors.Add(ConstructDef(constr));
								continue;
							} else Tokens.AddFirst(name);
						}
						bool isstatic = Match(Keyword.Static);
						Statement nextfield = NextStatement();
						if(nextfield is Declarator df) {
							Declaration curdecl = Match(DelimeterToken.LeftParen) ? FunctionDef(df) as Declaration : VarDeclaration(df);
							(isstatic ? statics : instance).Add(curdecl);
						} else throw new OutletException("statement: " + nextfield.ToString() + " must be inside a function body");
					}
				}
				if(constructors.Count == 0) constructors.Add(new ConstructorDeclaration(new Declarator(new Variable(name.Name), ""), new List<Declarator>(), Block.Empty()));
				return new ClassDeclaration(name.Name, superclass, genericParameters, constructors, instance, statics);
			}
			if(Match(Keyword.Class)) return ClassDef();
			Statement next = NextStatement();

			if(next is Declarator d) {
				if(d.IsOperatorOverload)
                {
					var op = ConsumeType<OperatorToken>("Expected operator following overload");
					Consume(DelimeterToken.LeftParen, "Expected ( before operator overload args");
					return OperatorOverloadDef(d, op);
                }
				if(Match(DelimeterToken.LeftParen)) return FunctionDef(d);
				else return VarDeclaration(d);
			}
			return next;
		}

        private Statement NextStatement() {
            Statement Scope() {
				var block = ParseBlock();
				Consume(DelimeterToken.RightCurly, "Expected } to close code block");
                return block;
            }
            Statement IfStatement() {
                Consume(DelimeterToken.LeftParen, "Expected ( after if");
                Expression condition = NextExpression();
                Consume(DelimeterToken.RightParen, "Expected ) after if condition");
                Statement iftrue = NextStatement();
                Statement? ifelse = Match(Keyword.Else) ? NextStatement() : null;
                return new IfStatement(condition, iftrue, ifelse);
            }
            Statement WhileLoop() {
                Consume(DelimeterToken.LeftParen, "Expected ( after while");
                Expression condition = NextExpression();
                Consume(DelimeterToken.RightParen, "Expected ) after while condition");
                Statement iftrue = NextStatement();
                return new WhileLoop(condition, iftrue);
            }
            Statement ForLoop() {
                Consume(DelimeterToken.LeftParen, "Expected ( after for");
                Statement s = NextStatement();
                if (s is Declarator loopvar) {
                    Consume(Keyword.In, "expected in after for loop variable");
                    Expression collection = NextExpression();
                    Consume(DelimeterToken.RightParen, "Expected ) after for loop collection");
                    Statement body = NextStatement();
                    return new ForLoop(loopvar, collection, body);
                }
                throw new OutletException("expected type followed by an identifier to use as a loop variable");
            }
            Statement Return() {
                Expression retexpr = NextExpression();
                Consume(DelimeterToken.SemiC, "expected ; after return statement");
                return new ReturnStatement(retexpr);
            }
            Statement Using()
            {
                Expression used = NextExpression();
                Consume(DelimeterToken.SemiC, "expected ; after using statement");
                return new UsingStatement(used);
            }
			if(Match(DelimeterToken.LeftCurly)) return Scope();
			if(Match(Keyword.If)) return IfStatement();
			if(Match(Keyword.For)) return ForLoop();
			if(Match(Keyword.While)) return WhileLoop();
			if(Match(Keyword.Return)) return Return();
            if(Match(Keyword.Using)) return Using();
			Expression? expr = Match(Keyword.Var) ? null : NextExpression();
			if((Match(DelimeterToken.SemiC) || Tokens.Count == 0) && expr != null) return expr;
			Identifier id = ConsumeType<Identifier>($"unexpected token: {Tokens.First()}, expected: ;");
			return expr is null ? new Declarator(id.Name) : new Declarator(expr, id.Name);
		}
	}
}
