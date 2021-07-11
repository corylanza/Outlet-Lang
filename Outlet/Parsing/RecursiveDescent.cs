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
			List<TypeParameter> ParseGenericParameters()
            {
				List<TypeParameter> parameters = new List<TypeParameter>();
				if (Match(DelimeterToken.LeftBrace))
				{
					do
					{
						string genericId = ConsumeType<Identifier>(expected: "generic parameter identifier").Name;
						Expression? genericConstraint = Match(Keyword.Extends) ? NextExpression() : null;
						parameters.Add(new TypeParameter(genericConstraint, genericId));
					} while (Match(DelimeterToken.Comma));
					Consume(DelimeterToken.RightBrace, expected: "] to close generic type definition");
				}
				return parameters;
			}
			(List<Declarator>, List<TypeParameter>, Statement) ProtoType() {
				var genericParameters = ParseGenericParameters();
				Consume(DelimeterToken.LeftParen, expected: "( after generic parameters");
				List<Declarator> argnames = new List<Declarator>();
				while(PeekNextTokenExistsAndIsnt(DelimeterToken.RightParen)) {
					do {
						if(NextStatement() is Declarator paramdecl) {
							argnames.Add(paramdecl);
						} else throw new OutletException("function parameters expected in type id format");
					} while(Match(DelimeterToken.Comma));
				}
				Consume(DelimeterToken.RightParen, expected: ") after function args");
				Statement body;
				if(Match(OperatorToken.Lambda)) {
					body = NextExpression();
					if(Tokens.Count != 0)
						Consume(DelimeterToken.SemiC, expected: "; after inline function");
				} else body = NextStatement();
				return (argnames, genericParameters, body);
			}
			#endregion
			VariableDeclaration VarDeclaration(Declarator decl) {
				Expression? initializer = null;
				if(Match(OperatorToken.Equal)) initializer = NextExpression();
				if(Tokens.Count != 0)
					Consume(DelimeterToken.SemiC, expected: "; after declaring a variable");
				return new VariableDeclaration(decl, initializer);
			}
			FunctionDeclaration FunctionDef(Declarator decl) {
				(List<Declarator> parameters, List<TypeParameter> typeParams, Statement body) = ProtoType();
				return new FunctionDeclaration(decl, parameters, typeParams, body);
			}
			ConstructorDeclaration ConstructDef(Declarator decl) {
				(List<Declarator> parameters, List<TypeParameter> typeParams, Statement body) = ProtoType();
				return new ConstructorDeclaration(decl, parameters, typeParams, body);
			}
			OperatorOverloadDeclaration OperatorOverloadDef(Declarator decl, OperatorToken op)
			{
				(List<Declarator> parameters, List<TypeParameter> typeParams, Statement body) = ProtoType();
				return new OperatorOverloadDeclaration(decl, op, parameters, typeParams, body);
            }
			ClassDeclaration ClassDef() {
				(List<Declaration> instance, List<Declaration> statics, List<ConstructorDeclaration> constructors, Variable? superclass) = (new(), new(), new(), null);
                (Lexeme nameLexeme, Identifier name) = ConsumeTypeGetLexeme<Identifier>(expected: "class identifier");
				var genericParameters = ParseGenericParameters();
                if (Match(Keyword.Extends)) {
					superclass = new Variable(ConsumeType<Identifier>(expected: "name of super class after extends keyword").Name);
				}
				if(Match(DelimeterToken.LeftCurly)) {
					while(true) {
						if(Match(DelimeterToken.RightCurly)) break;
						if(Tokens.Count == 0) throw new OutletException("expected } after class definition");
						if(Match(name)) {
							if(PeekMatch(DelimeterToken.LeftParen) || PeekMatch(DelimeterToken.LeftBrace)) {
								Declarator constr = new Declarator(new Variable(name.Name), "");
								constructors.Add(ConstructDef(constr));
								continue;
							} else Tokens.AddFirst(nameLexeme);
						}
						bool isstatic = Match(Keyword.Static);
						Statement nextfield = NextStatement();
						if(nextfield is Declarator df) {
							Declaration curdecl = PeekMatch(DelimeterToken.LeftParen) || PeekMatch(DelimeterToken.LeftBrace) ? FunctionDef(df) as Declaration : VarDeclaration(df);
							(isstatic ? statics : instance).Add(curdecl);
						} else throw new OutletException("statement: " + nextfield.ToString() + " must be inside a function body");
					}
				}
				if(constructors.Count == 0) constructors.Add(new ConstructorDeclaration(new Declarator(new Variable(name.Name), ""), new List<Declarator>(), new List<TypeParameter>(), Block.Empty()));
				return new ClassDeclaration(name.Name, superclass, genericParameters, constructors, instance, statics);
			}
			if(Match(Keyword.Class)) return ClassDef();
			Statement next = NextStatement();
			if(next is Declarator d && d.Type is not null) {
				if(d.IsOperatorOverload)
                {
					var op = ConsumeType<OperatorToken>(expected: "operator following overload");
					// TODO this should probably be a PeekMatch and also check for LeftBrace for generic case
					Consume(DelimeterToken.LeftParen, expected: "( before operator overload args");
					return OperatorOverloadDef(new Declarator(d.Type, op.ToString()), op);
                }
				if(PeekMatch(DelimeterToken.LeftParen) || PeekMatch(DelimeterToken.LeftBrace)) return FunctionDef(d);
				else return VarDeclaration(d);
			}
			return next;
		}

        private Statement NextStatement() {
            Statement Scope() {
				var block = ParseBlock();
				Consume(DelimeterToken.RightCurly, expected: "} to close code block");
                return block;
            }
            Statement IfStatement() {
                Consume(DelimeterToken.LeftParen, expected: "( after if");
                Expression condition = NextExpression();
                Consume(DelimeterToken.RightParen, expected: ") after if condition");
                Statement iftrue = NextStatement();
                Statement? ifelse = Match(Keyword.Else) ? NextStatement() : null;
                return new IfStatement(condition, iftrue, ifelse);
            }
            Statement WhileLoop() {
                Consume(DelimeterToken.LeftParen, expected: "( after while");
                Expression condition = NextExpression();
                Consume(DelimeterToken.RightParen, expected: ") after while condition");
                Statement iftrue = NextStatement();
                return new WhileLoop(condition, iftrue);
            }
            Statement ForLoop() {
                Consume(DelimeterToken.LeftParen, expected: "( after for");
                Statement s = NextStatement();
                if (s is Declarator loopvar) {
                    Consume(Keyword.In, expected: "'in' after for loop variable");
                    Expression collection = NextExpression();
                    Consume(DelimeterToken.RightParen, expected: ") after for loop collection");
                    Statement body = NextStatement();
                    return new ForLoop(loopvar, collection, body);
                }
				throw new OutletException("expected type followed by an identifier to use as a loop variable");
            }
            Statement Return() {
                Expression retexpr = NextExpression();
                Consume(DelimeterToken.SemiC, expected: "; after return statement");
                return new ReturnStatement(retexpr);
            }
            Statement Using()
            {
                Expression used = NextExpression();
                Consume(DelimeterToken.SemiC, expected: "; after using statement");
                return new UsingStatement(used);
            }
			if(Match(DelimeterToken.LeftCurly)) return Scope();
			if(Match(Keyword.If)) return IfStatement();
			if(Match(Keyword.For)) return ForLoop();
			if(Match(Keyword.While)) return WhileLoop();
			if(Match(Keyword.Return)) return Return();
            if(Match(Keyword.Using)) return Using();

			if(Match(Keyword.Var))
			{
				Identifier varId = ConsumeType<Identifier>(expected: "identifier after var");
				return new Declarator(varId.Name);
			}


			Expression expr = NextExpression();
			if(Match(DelimeterToken.SemiC) || Tokens.Count == 0) return expr;
			Identifier id = ConsumeType<Identifier>(expected: "; to close statement");
			return new Declarator(expr, id.Name);
		}
	}
}
