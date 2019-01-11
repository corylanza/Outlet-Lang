using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Tokens;
using Outlet.AST;

namespace Outlet.Parsing {
	public static partial class Parser {

		public static IASTNode NextDeclaration(LinkedList<Token> tokens) {
			#region helper
			bool Match(Token s) {
				if(tokens.Count > 0 && s.Equals(tokens.First())) {
					tokens.RemoveFirst(); return true;
				} else return false;
			}
			void Consume(Token s, string error) {
				if(tokens.Count == 0 || tokens.Dequeue() != s) throw new OutletException("Syntax Error: " + error);
			}
			T ConsumeType<T>(string error) where T : Token {
				if(tokens.Count > 0 && tokens.Dequeue() is T t) return t;
				else throw new OutletException("Syntax Error: " + error);
			}
			(List<Declarator>, Statement) ProtoType() {
				List<Declarator> argnames = new List<Declarator>();
				while(tokens.Count > 0 && tokens.First() != Delimeter.RightParen) {
					do {
						if(NextStatement(tokens) is Declarator paramdecl) {
							argnames.Add(paramdecl);
						} else throw new OutletException("function parameters expected in type id format");
					} while(Match(Delimeter.Comma));
				}
				Consume(Delimeter.RightParen, " expected ) after function args");
				Statement body;
				if(Match(Operator.Lambda)) {
					body = NextExpression(tokens);
					if(tokens.Count != 0)
						Consume(Delimeter.SemiC, "expected ; after inline function");
				} else body = NextStatement(tokens);
				return (argnames, body);
			}
			#endregion
			VariableDeclaration VarDeclaration(Declarator decl) {
				Expression initializer = null;
				if(Match(Operator.Equal)) initializer = NextExpression(tokens);
				if(tokens.Count != 0)
					Consume(Delimeter.SemiC, "expected ; after declaring a variable");
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
			ClassDeclaration ClassDef() {
				Identifier name = ConsumeType<Identifier>("Expected class identifier"); ;
				List<Declaration> instance = new List<Declaration>();
				List<Declaration> statics = new List<Declaration>();
				ConstructorDeclaration constructor = null;
				string superclass = "";
				if(Match(Keyword.Extends)) {
					 superclass = ConsumeType<Identifier>("expected name of super class after extends keyword").Name;
				}
				if(Match(Delimeter.LeftCurly)) {
					while(true) {
						if(Match(Delimeter.RightCurly)) break;
						if(tokens.Count == 0) throw new OutletException("expected } after class definition");
						if(Match(name)) {
							if(Match(Delimeter.LeftParen)) {
								Declarator constr = new Declarator(new Variable(name.Name), "");
								if(constructor != null) throw new OutletException("class cannot have two constructors");
								constructor = ConstructDef(constr);
								continue;
							} else tokens.AddFirst(name);
						}
						bool isstatic = Match(Keyword.Static);
						Statement nextfield = NextStatement(tokens);
						if(nextfield is Declarator df) {
							Declaration curdecl = Match(Delimeter.LeftParen) ? FunctionDef(df) as Declaration : VarDeclaration(df);
							(isstatic ? statics : instance).Add(curdecl);
						} else throw new OutletException("statement: " + nextfield.ToString() + " must be inside a function body");
					}
				}
				if(constructor == null) constructor = new ConstructorDeclaration(new Declarator(new Variable(name.Name), ""), new List<Declarator>(), new Block(new List<IASTNode>(), new List<FunctionDeclaration>(), new List<ClassDeclaration>()));
				return new ClassDeclaration(name.Name, superclass, constructor, instance, statics);
			}
			if(Match(Keyword.Class)) return ClassDef();
			Statement next = NextStatement(tokens);

			if(next is Declarator d) {
				if(Match(Delimeter.LeftParen)) return FunctionDef(d);
				else return VarDeclaration(d);
			}
			return next;
		}

		public static Statement NextStatement(LinkedList<Token> tokens) {
			#region helper
			bool Match(Token s) {
				if(tokens.Count > 0 && tokens.First() == s) {
					tokens.Dequeue(); return true;
				} else return false;
			}
			void Consume(Token s, string error) {
				if(tokens.Count == 0 || tokens.Dequeue() != s) throw new OutletException("Syntax Error: " + error);
			}
			T ConsumeType<T>(string error) where T : Token {
				if(tokens.Count > 0 && tokens.Dequeue() is T t) return t;
				else throw new OutletException("Syntax Error: " + error);
			}
			#endregion
			Statement Scope() {
				List<IASTNode> lines = new List<IASTNode>();
				List<FunctionDeclaration> funcs = new List<FunctionDeclaration>();
				List<ClassDeclaration> classes = new List<ClassDeclaration>();
				while(tokens.Count > 0 && tokens.First() != Delimeter.RightCurly) {
					var nextdecl = NextDeclaration(tokens);
					if(nextdecl is FunctionDeclaration fd) funcs.Add(fd);
					if(nextdecl is ClassDeclaration cd) classes.Add(cd);
					lines.Add(nextdecl);
				}
				Consume(Delimeter.RightCurly, "Expected } to close code block");
				return new Block(lines, funcs, classes);
			}
			Statement IfStatement() {
				Consume(Delimeter.LeftParen, "Expected ( after if");
				Expression condition = NextExpression(tokens);
				Consume(Delimeter.RightParen, "Expected ) after if condition");
				Statement iftrue = NextStatement(tokens);
				Statement ifelse = Match(Keyword.Else) ? NextStatement(tokens) : null;
				return new IfStatement(condition, iftrue, ifelse);
			}
			Statement WhileLoop() {
				Consume(Delimeter.LeftParen, "Expected ( after while");
				Expression condition = NextExpression(tokens);
				Consume(Delimeter.RightParen, "Expected ) after while condition");
				Statement iftrue = NextStatement(tokens);
				return new WhileLoop(condition, iftrue);
			}
			Statement ForLoop() {
				Consume(Delimeter.LeftParen, "Expected ( after for");
				Statement s = NextStatement(tokens);
				if(s is Declarator loopvar) {
					Consume(Keyword.In, "expected in after for loop variable");
					Expression collection = NextExpression(tokens);
					Consume(Delimeter.RightParen, "Expected ) after for loop collection");
					Statement body = NextStatement(tokens);
					return new ForLoop(loopvar, collection, body);
				}
				throw new OutletException("expected type followed by an identifier to use as a loop variable");
			}
			Statement Return() {
				Expression retexpr = NextExpression(tokens);
				Consume(Delimeter.SemiC, "expected ; after return statement");
				return new ReturnStatement(retexpr);
			}
			if(Match(Delimeter.LeftCurly)) return Scope();
			if(Match(Keyword.If)) return IfStatement();
			if(Match(Keyword.For)) return ForLoop();
			if(Match(Keyword.While)) return WhileLoop();
			if(Match(Keyword.Return)) return Return();
			Expression expr =  NextExpression(tokens);
			if(Match(Delimeter.SemiC)) return expr;
			if(tokens.Count == 0) return expr;
			Identifier id = ConsumeType<Identifier>("unexpected token: "+tokens.First().ToString()+", expected: ;");
			return new Declarator(expr, id.Name);
		}
	}
}
