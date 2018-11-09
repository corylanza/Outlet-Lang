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
		public static Stack<Scope> Scopes = new Stack<Scope>();

		static Checker() {
			Scopes.Push(new Scope(null));
		}

		public Scope Scope() => Scopes.Peek();

		public Scope EnterScope() {
			if(Scopes.Count == 0) Scopes.Push(new Scope(null));
			else Scopes.Push(new Scope(Scopes.Peek()));
			return Scopes.Peek();
		}

		public void ExitScope() => Scopes.Pop();

		public static void Cast(Type a, Type b, string message = "cannot convert type {0} to type {1}") {
			if(!a.Is(b)) throw new CheckerException(string.Format(message, a, b));
		}

		private readonly Type Bool = Primitive.Bool;

		#endregion

		public Type Visit(ClassDeclaration c) {
			Scope().Define(Primitive.MetaType, c.Name);
			EnterScope();
			foreach(Declaration d in c.StaticDecls) {
				d.Accept(this);
			}
			EnterScope();
			Scope().Define(Primitive.MetaType, "this");
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
			Scope().Define(ft, f.Decl.ID);
			// enter the function scope and define the args;
			EnterScope();
			Array.ForEach(args, arg => Scope().Define(arg.type, arg.id));
			// check the body now that its header and args have been defined
			// TODO will need a variable to track what return type is needed by this function and is used by conditionals to check that its a valid definition
			f.Body.Accept(this);
			ExitScope();
			return null;
		}

		public Type Visit(VariableDeclaration v) {
			Type decl = v.Decl.Accept(this);
			Scope().Declare(decl, v.Decl.ID);
			Type init = v.Initializer?.Accept(this);
			Cast(init, decl);
			Scope().Define(decl, v.Decl.ID);
			return null;
		}

		public Type Visit(Declarator d) {
			// THIS NEEDS MAJOR REWORK, CURRENTLY ACCEPTS NON TYPES AS TYPES....................................
			return d.Type.Accept(this);
		}

		public Type Visit(Constant c) => c.Type;

		public Type Visit(Assign a) {
			Type l = a.Left.Accept(this);
			Type r = a.Right.Accept(this);
			Cast(r, l);
			return l;
		}

		public Type Visit(Binary b) {
			Type l = b.Left.Accept(this);
			Type r = b.Right.Accept(this);
			if(l == r) return l;
			throw new NotImplementedException();
		}

		public Type Visit(Call c) {
			Type calltype = c.Caller.Accept(this);
			if(calltype is FunctionType functype) {		// doesnt work for other callable types
				Type[] argtypes = c.Args.Select(x => x.Accept(this)).ToArray();
				if(argtypes.Length != functype.Args.Length) throw new OutletException("not even of the same length");
				for(int i = 0; i < c.Args.Length; i++) {
					if(!argtypes[i].Is(functype.Args[i].type))
						throw new OutletException("cannot convert: "+c.ToString()+ " to: "+functype.ToString());
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
			//throw new NotImplementedException();
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
			throw new OutletException("types in branches of ternary statement are not compatible");
		}

		public Type Visit(TupleLiteral t) {
			if(t.Args.Length == 1) return t.Args[0].Accept(this);
			else return new TupleType(t.Args.Select(arg => arg.Accept(this)).ToArray());
		}

		public Type Visit(Unary u) {
			Type input = u.Expr.Accept(this);
			UnaryOperation op = null;
			foreach(UnaryOperation uo in u.Overloads.Cadidates()) {
				if(input.Is(uo.Input)) { op = uo; break; }
			}
			u.Oper = op ?? throw new OutletException("operator doesn't work on this type");
			return op.Result;
		}

		public Type Visit(Variable v) {
			(Type t, int l) = Scope().Find(v.Name);
			v.resolveLevel = l;
			if(l == -1) {
				if(ForeignFunctions.NativeTypes.ContainsKey(v.Name)) return ForeignFunctions.NativeTypes[v.Name];
				if(ForeignFunctions.NativeFunctions.ContainsKey(v.Name)) return ForeignFunctions.NativeFunctions[v.Name].Type;
				throw new OutletException("variable " + v.Name + " could not be resolved");
			}
			return t;
		}

		public Type Visit(Block b) {
			Scope exec = EnterScope();
			foreach(Declaration d in b.Lines) {
				d.Accept(this);
			}
			ExitScope();
			return null;
		}

		public Type Visit(ForLoop f) {
			throw new NotImplementedException();
		}

		public Type Visit(IfStatement i) {
			Type cond = i.Condition.Accept(this);
			Cast(cond, Bool, "if statement condition requires a boolean, found a {0}");
			i.Iftrue.Accept(this);
			i.Iffalse?.Accept(this);
			return null;
		}

		public Type Visit(ReturnStatement r) {
			throw new NotImplementedException();
		}

		public Type Visit(WhileLoop w) {
			Type cond = w.Condition.Accept(this);
			Cast(cond, Bool, "while loop condition requires a boolean, found a {0}");
			w.Body.Accept(this);
			return null;
		}
	}
}
