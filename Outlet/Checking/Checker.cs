using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Type = Outlet.AST.Type;

namespace Outlet.Checking {
	public class Checker : IVisitor<Type> {

		#region Helpers

		public static Stack<FunctionType> CurrentFunctions = new Stack<FunctionType>();

		public static readonly Stack<Scope> Scopes = new Stack<Scope>();

		static Checker() {
			Scopes.Push(Scope.Global);
		}

		public static void Declare(Type t, string s) => Scopes.Peek().Declare(t, s);
		public static void Define(Type t, string s) => Scopes.Peek().Define(t, s);
		public static void DefineType(Type t, string s) => Scopes.Peek().DefineType(t, s);
		public static (Type, int) Find(string s) => Scopes.Peek().Find(s);
		public static Type FindType(int level, string s) => Scopes.Peek().FindType(level, s);

		public Scope EnterScope() {
			if(Scopes.Count == 0) Scopes.Push(new Scope(null));
			else Scopes.Push(new Scope(Scopes.Peek()));
			return Scopes.Peek();
		}

		public void ExitScope() => Scopes.Pop();

		public static void Cast(Type from, Type to, string message = "cannot convert type {0} to type {1}") {
			if(!from.Is(to)) throw new CheckerException(string.Format(message, from, to));
		}

		private readonly Type Bool = Primitive.Bool;

		private Type TypeLiteral(Expression e) {
			if(e is Variable v) {
				v.Accept(this);	// resolves the type literal so it can be found when interpreted
				return FindType(v.resolveLevel, v.Name);
			}
			if(e is TupleLiteral tl) return new TupleType(tl.Args.Select(arg => TypeLiteral(arg)).ToArray());
			throw new CheckerException("declaration requires valid type");
		}


		#endregion

		public Type Visit(ClassDeclaration c) {
			Define(Primitive.MetaType, c.Name);
			EnterScope();
			foreach(Declaration d in c.StaticDecls) {
				d.Accept(this);
			}
			EnterScope();
			Define(Primitive.MetaType, "this");
			foreach(Declaration d in c.InstanceDecls) {
				d.Accept(this);
			}
			ExitScope();
			ExitScope();
			return null;
		}

		public Type Visit(FunctionDeclaration f) {
			// Check decl and args first, needed to make function type
			Type returntype = f.Decl.Accept(this);
			(Type type, string id)[] args = f.Args.Select(arg => (arg.Accept(this), arg.ID)).ToArray();
			FunctionType ft = new FunctionType(args, returntype);
			// define the header using the function type from above
			Define(ft, f.Decl.ID);
			// enter the function scope and define the args;
			EnterScope();
			Array.ForEach(args, arg => Define(arg.type, arg.id));
			// check the body now that its header and args have been defined
			// TODO will need a variable to track what return type is needed by this function and is used by conditionals to check that its a valid definition
			Type body = f.Body.Accept(this);
			if(body == null || body == Primitive.Void) {
				if(ft.ReturnType != Primitive.Void) throw new CheckerException("not all code paths return a value");
			} else Cast(body, ft.ReturnType, "function definition invalid, expected {1}, returned {0}");
			/*else {
				CurrentFunctions.Push(ft);
				Type ret = f.Body.Accept(this);
				if(ret.)
			}*/
			ExitScope();
			return null;
		}

		public Type Visit(VariableDeclaration v) {
			Type decl = v.Decl.Accept(this);
			Declare(decl, v.Decl.ID);
			Type init = v.Initializer?.Accept(this);
			Cast(init, decl);
			if(decl == Primitive.MetaType) DefineType(TypeLiteral(v.Initializer), v.Decl.ID);
			else Define(decl, v.Decl.ID);
			return null;
		}

		public Type Visit(Declarator d) {
			return TypeLiteral(d.Type);
		}

		public Type Visit(Const c) => c.Type;

		public Type Visit(Assign a) {
			Type l = a.Left.Accept(this);
			Type r = a.Right.Accept(this);
			Cast(r, l);
			return l;
		}

		public Type Visit(Binary b) {
			Type left = b.Left.Accept(this);
			Type right = b.Right.Accept(this);
			var op = b.Overloads.Best(left, right);
			b.Oper = op ?? throw new OutletException("binary operator not defined for "+left.ToString()+ " "+b.Op+" "+right.ToString());
			return op.Result;
		}

		public Type Visit(Call c) {
			Type calltype = c.Caller.Accept(this);
			if(calltype is FunctionType functype) {		// doesnt work for other callable types
				Type[] argtypes = c.Args.Select(x => x.Accept(this)).ToArray();
				if(argtypes.Length != functype.Args.Length) throw new OutletException("not even of the same length");
				for(int i = 0; i < c.Args.Length; i++) {
					if(!argtypes[i].Is(functype.Args[i].type))
						throw new OutletException("cannot convert: "+argtypes.ToList().ToListString()+ " to: "+functype.ToString());
				}
				return functype.ReturnType;
			}
			throw new OutletException("not callable");
		}

		public Type Visit(Deref d) {
			Type inst = d.Left.Accept(this);
			throw new NotImplementedException();
		}

		public Type Visit(Lambda l) {
			throw new NotImplementedException();
		}

		public Type Visit(ListLiteral l) {
			return Primitive.List;
		}

		public Type Visit(ShortCircuit s) {
			Type l = s.Left.Accept(this);
			Type r = s.Right.Accept(this);
			Cast(l, Bool);
			Cast(r, Bool);
			return Primitive.Bool;
		}

		public Type Visit(Ternary t) {
			Type cond = t.Condition.Accept(this);
			Type iftrue = t.IfTrue.Accept(this);
			Type iffalse = t.IfFalse.Accept(this);
			Cast(cond, Bool, "Ternary condition requires a boolean, found a {0}");
			if(iftrue.Is(iffalse)) return iffalse;
			if(iffalse.Is(iftrue)) return iftrue;
			return Primitive.Object;
		}

		public Type Visit(TupleLiteral t) {
			if(t.Args.Length == 1) return t.Args[0].Accept(this);
			var types = t.Args.Select(arg => arg.Accept(this)).ToArray();
			if(types.All(type => type == Primitive.MetaType)) return Primitive.MetaType;
			else return new TupleType(types);
		}

		public Type Visit(Unary u) {
			Type input = u.Expr.Accept(this);
			var op = u.Overloads.Best(input);
			u.Oper = op ?? throw new OutletException("unary operator "+u.Op+" is not defined for type "+input.ToString());
			return op.Result;
		}

		public Type Visit(Variable v) {
			(Type t, int l) = Find(v.Name);
			v.resolveLevel = l;
			if(l == -1) {
				throw new OutletException("variable " + v.Name + " could not be resolved");
			}
			return t;
		}

		public Type Visit(Block b) {
			EnterScope();
			Type ret = null;
			foreach(Declaration d in b.Lines) {
				Type temp = d.Accept(this);
				if(ret != null) throw new CheckerException("unreachable code detected");
				if(d is Statement && !(d is Expression) && temp != null) {
					ret = temp;
				}
			}
			ExitScope();
			return ret;
		}

		public Type Visit(ForLoop f) {
			Type collection = f.Collection.Accept(this);
			Type loopvar = f.LoopVar.Accept(this);
			Type body = f.Accept(this);
			return null;
		}

		public Type Visit(IfStatement i) {
			Type cond = i.Condition.Accept(this);
			Cast(cond, Bool, "if statement condition requires a boolean, found a {0}");
			Type iftrue = i.Iftrue.Accept(this);
			Type iffalse = i.Iffalse?.Accept(this);
			if(i.Iftrue is Statement && !(i.Iftrue is Expression) && iftrue != null) {
				if(i.Iffalse is Statement && !(i.Iffalse is Expression) && iffalse != null) {
					return iftrue;
				}
			}
			return null;
		}

		public Type Visit(ReturnStatement r) {
			return r.Expr.Accept(this);
		}

		public Type Visit(WhileLoop w) {
			Type cond = w.Condition.Accept(this);
			Cast(cond, Bool, "while loop condition requires a boolean, found a {0}");
			w.Body.Accept(this);
			return null;
		}
	}
}
