using System;
using System.Collections.Generic;
using System.Linq;
using Outlet.AST;
using Outlet.Operands;
using Type = Outlet.Operands.Type;

namespace Outlet.Checking {
	public class Checker : IVisitor<Type> {

		#region Helpers

		public static readonly Stack<bool> DoImpl = new Stack<bool>();
		public static readonly Stack<Scope> Scopes = new Stack<Scope>();
		private static readonly Type ErrorType = new ProtoClass("error", null, null, null);
		public static int ErrorCount = 0;

		public static void Check(IASTNode program) {
			ErrorCount = 0;
			if(program is FunctionDeclaration || program is ClassDeclaration) {
				DoImpl.Push(false);
				program.Accept(Hidden);
				DoImpl.Pop();
				DoImpl.Push(true);
				program.Accept(Hidden);
				DoImpl.Pop();
			} else program.Accept(Hidden);
			if(ErrorCount > 0) throw new CheckerException(ErrorCount + " Checking errors encountered");
		}
		private static readonly Checker Hidden = new Checker();
		private Checker() => Scopes.Push(Scope.Global);

		public static void Declare(Type t, string s) => Scopes.Peek().Declare(t, s);
		public static void Define(Type t, string s) => Scopes.Peek().Define(t, s);
		public static void DefineType(Type t, string s) => Scopes.Peek().DefineType(t, s);
		public static (Type type, int level) Find(string s) => Scopes.Peek().Find(s);
		public static Type FindType(int level, string s) => Scopes.Peek().FindType(level, s);

		public static Scope EnterScope() {
			if(Scopes.Count == 0) Scopes.Push(new Scope(null));
			else Scopes.Push(new Scope(Scopes.Peek()));
			return Scopes.Peek();
		}

		public static void ExitScope() => Scopes.Pop();

		public static Type Error(string message) {
			ErrorCount++;
			if(message != "") Program.ThrowException(message);
			return ErrorType;
		}

		public static void Cast(Type from, Type to, string message = "cannot convert type {0} to type {1}") {
			if(from == ErrorType || to == ErrorType) return;
			if(from == Primitive.Null) {
				if(to is Primitive) Error("cannot assign null to non nullable type: "+to.ToString());
			} else  if(!from.Is(to)) Error(string.Format(message, from, to));
		}

		private readonly Type Bool = Primitive.Bool;

		private Type TypeLiteral(Expression e) {
			if(e is Variable v) {
				v.Accept(this); // resolves the type literal so it can be found when interpreted
				return FindType(v.resolveLevel, v.Name);
			}
			if(e is TupleLiteral tl) return new TupleType(tl.Args.Select(arg => TypeLiteral(arg)).ToArray());
			if(e is Lambda l) return new FunctionType(((TupleType)TypeLiteral(l.Left)).Types.Select(x => (x, "")).ToArray(), TypeLiteral(l.Right));
			if(e is Access a) return new ArrayType(TypeLiteral(a.Collection));
            if (e is Binary b && b.Op == "/") return new UnionType(TypeLiteral(b.Left), TypeLiteral(b.Right));
			return Error("declaration requires valid type, found: " + e.ToString());
		}

		#endregion

		#region Declarations

		public Type Visit(ClassDeclaration c) {
			if(!DoImpl.Peek()) {
				var statics = new Dictionary<string, Type>();
				var instances = new Dictionary<string, Type>();
				Class parent = Primitive.Object;
				if(c.SuperClass != null) {
					if(c.SuperClass.Accept(this) != Primitive.MetaType) Error("cannot extend anything other than a class");
					else {
						Type parenttype = FindType(c.SuperClass.resolveLevel, c.SuperClass.Name);
						if(parenttype is Class pt) parent = pt;
						else Error("cannot extend anything other than a class");
					}
				}
				DefineType(new ProtoClass(c.Name, parent, instances, statics), c.Name);
				EnterScope();
				foreach(Declaration d in c.StaticDecls) {
					d.Accept(this);
					statics.Add(d.Name, Find(d.Name).type);
				}
				EnterScope();
				foreach(Declaration d in c.InstanceDecls) {
					d.Accept(this);
					instances.Add(d.Name, Find(d.Name).type);
				}
				c.Constructor.Accept(this);
				statics.Add("", c.Constructor.Type);
				ExitScope();
				ExitScope();
			} else {
				ProtoClass parent = null;
				if(c.SuperClass != null) {
					parent = FindType(c.SuperClass.resolveLevel, c.SuperClass.Name) as ProtoClass;
				}
				EnterScope();
				foreach(Declaration d in c.StaticDecls) d.Accept(this);
				foreach(Declaration d in c.StaticDecls) if(d is FunctionDeclaration fd) Define(fd.Type, fd.Name);
				if(parent != null) foreach(KeyValuePair<string, Type> d in parent.Statics) Define(d.Value, d.Key);
				EnterScope();
				Define(FindType(2, c.Name), "this");
				foreach(Declaration d in c.InstanceDecls) d.Accept(this);
				foreach(Declaration d in c.InstanceDecls) if(d is FunctionDeclaration fd) Define(fd.Type, fd.Name);
				if(parent != null) foreach(KeyValuePair<string, Type> d in parent.Instances) Define(d.Value, d.Key);
				c.Constructor.Accept(this);
				ExitScope();
				ExitScope();
			}
			return null;
		}

		public Type Visit(ConstructorDeclaration c) {
			Visit(c as FunctionDeclaration);
			return null;
		}

		public Type Visit(FunctionDeclaration f) {
			if(!DoImpl.Peek()) {
				// Check decl and args first, needed to make function type
				EnterScope();
				(Type type, string id)[] args = f.Args.Select(arg => {
					Type curarg = arg.Accept(this);
					//if(curarg == Primitive.MetaType) DefineType(new ProtoClass(arg.ID, null, null), arg.ID);
					return (curarg, arg.ID);
				}).ToArray();
				Type returntype = f.Decl.Accept(this);
				ExitScope();
				FunctionType ft = new FunctionType(args, returntype);
				f.Type = ft;
				// define the header using the function type from above
				Define(ft, f.Decl.ID);
			} else {
				FunctionType ft = f.Type;//(FunctionType) Find(f.Decl.ID).Item1;
										 //f.Type = null;
										 // enter the function scope and define the args;
				EnterScope();
				System.Array.ForEach(ft.Args, arg => Define(arg.type, arg.id));
				// check the body now that its header and args have been defined
				Type body = f.Body.Accept(this);
				if(f.Name == "") {
					if(body != null) return Error("constructor cannot return value");
				} else if(body == null || body == Primitive.Void) {
					if(ft.ReturnType != Primitive.Void) return Error("function " + f.Decl.ID + "not all code paths return a value");
				} else Cast(body, ft.ReturnType, f.Decl.ID + "function definition invalid, expected {1}, returned {0}");
				ExitScope();
			}
			return null;
		}

		public Type Visit(VariableDeclaration v) {
			Type decl = v.Decl.Accept(this);
			Declare(decl, v.Decl.ID);
			Type init = v.Initializer?.Accept(this);
			if(init != null) Cast(init, decl);
			if(decl == Primitive.MetaType) DefineType(TypeLiteral(v.Initializer), v.Decl.ID);
			else Define(decl, v.Decl.ID);
			return null;
		}

		#endregion

		#region Expressions

		public Type Visit(Access a) {
			Type elem = a.Collection.Accept(this);
			if(elem == Primitive.MetaType) return Primitive.MetaType;
			if(a.Index.Length != 1) return Error("array access requires exactly 1 index");
			Type idxType = a.Index[0].Accept(this);
			if(idxType != Primitive.Int) return Error("only ints can be used to index into an array, found: " + idxType.ToString());
			if(elem is ArrayType at) return at.ElementType;
			return Error("invalid array type");
		}

		public Type Visit(As a) {
			a.Left.Accept(this);
			Type r = a.Right.Accept(this);
			if(r != Primitive.MetaType) return Error("the right side of an is expression must be a type, found: " + r.ToString());
			return TypeLiteral(a.Right);
		}

		public Type Visit(Assign a) {
			if(a.Left is Variable || a.Left is Deref) {
				Type l = a.Left.Accept(this);
				Type r = a.Right.Accept(this);
				if(l == ErrorType || r == ErrorType) return ErrorType;
				if(a.Left is Deref d && d.ArrayLength) return Error("cannot assign to an array length");
				Cast(r, l);
				return l;
			}
			return Error("illegal assignment, can only assign to variables and fields");
		}

		public Type Visit(Binary b) {
			Type left = b.Left.Accept(this);
			Type right = b.Right.Accept(this);
			if(left == ErrorType || right == ErrorType) return ErrorType;
			var op = b.Overloads.Best(left, right);
			if(op == null) return Error("binary operator not defined for " + left.ToString() + " " + b.Op + " " + right.ToString());
			b.Oper = op;
			return op.Result;
		}

		public Type Visit(Call c) {
			Type calltype = c.Caller.Accept(this);
			if(calltype == ErrorType) return ErrorType;
			if(calltype == Primitive.MetaType) {
				Type literal = TypeLiteral(c.Caller);
				if(literal is ICheckableClass cl) calltype = cl.GetStaticType("");
				else return Error("type " + literal + " is not instantiable");
			}
			Type[] argtypes = c.Args.Select(x => x.Accept(this)).ToArray();
			if(calltype is FunctionType functype) {
				var funcargs = functype.Args.ToArray();
				if(argtypes.Length == funcargs.Length) {
					int ec = ErrorCount;
					for(int i = 0; i < c.Args.Length; i++) {
						Cast(argtypes[i], funcargs[i].type, "");
					}
					if(ErrorCount == ec) return functype.ReturnType;
				} return Error(c.Caller+" expects "+"(" + funcargs.Select(x => x.type).ToList().ToListString() + ") found: (" + argtypes.ToList().ToListString() + ")");
			}
			return Error("type " + calltype + " is not callable");
		}

		public Type Visit(Declarator d) => TypeLiteral(d.Type);

		public Type Visit(Deref d) {
			Type inst = d.Left.Accept(this);
			if(inst == ErrorType) return ErrorType;
			if(inst is ArrayType && d.Right == "length") {
				d.ArrayLength = true;
				return Primitive.Int;
			}
			if(inst is ICheckableClass t) return t.GetInstanceType(d.Right);
			if(inst == Primitive.MetaType) {
				Type actual = TypeLiteral(d.Left);
				if(actual is ICheckableClass c) return c.GetStaticType(d.Right);
				return Error("type " + actual.ToString() + " does not contain static field: " + d.Right);
			}
			return Error("cannot dereference type: " + inst.ToString());
		}

		public Type Visit(Is i) {
			i.Left.Accept(this);
			Type r = i.Right.Accept(this);
			if(r != Primitive.MetaType) return Error("the right side of an is expression must be a type, found: " + r.ToString());
			return Primitive.Bool;
		}

		public Type Visit(Lambda l) {
			Type args = l.Left.Accept(this);
			Type res = l.Right.Accept(this);
			if(args == Primitive.MetaType && res == Primitive.MetaType) {
				Type a = TypeLiteral(l.Left);
				Type r = TypeLiteral(l.Right);
				return Primitive.MetaType;
			}
			return Error("Lambdas currently only work for types");
		}

		public Type Visit(ListLiteral l) {
			return new ArrayType(Type.CommonAncestor(l.Args.Select(x => x.Accept(this)).ToArray()));
		}

		public Type Visit(Literal c) => c.Type;

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
			return Type.CommonAncestor(iftrue, iffalse);
		}

		public Type Visit(TupleLiteral t) {
			if(t.Args.Length == 1) return t.Args[0].Accept(this);
			var types = t.Args.Select(arg => arg.Accept(this)).ToArray();
			if(types.All(type => type == Primitive.MetaType)) return Primitive.MetaType;
			else return new TupleType(types);
		}

		public Type Visit(Unary u) {
			Type input = u.Expr.Accept(this);
			if(input == ErrorType) return ErrorType;
			var op = u.Overloads.Best(input);
			if(op == null) return Error("unary operator " + u.Op + " is not defined for type " + input.ToString());
			u.Oper = op;
			return op.Result;
		}

		public Type Visit(Variable v) {
			(Type t, int l) = Find(v.Name);
			v.resolveLevel = l;
			if(l == -1) return Error("variable " + v.Name + " could not be resolved");
			return t;
		}

		#endregion

		#region Statements

		public Type Visit(Block b) {
			EnterScope();
			DoImpl.Push(false);
			// Forward Declaration of Classes
			foreach(ClassDeclaration cd in b.Classes) {
				cd.Accept(this);
			}
			// Forward Declaration of Functions
			foreach(FunctionDeclaration fd in b.Functions) {
				fd.Accept(this);
			}
			DoImpl.Pop();
			DoImpl.Push(true);
			Type ret = null;
			foreach(IASTNode d in b.Lines) {
				Type temp = d.Accept(this);
				if(ret != null) return Error("unreachable code detected");
				if(d is Statement && !(d is Expression) && temp != null) {
					ret = temp;
				}
			}
			DoImpl.Pop();
			ExitScope();
			return ret;
		}

		public Type Visit(ForLoop f) {
			Type collection = f.Collection.Accept(this);
			if(collection is ArrayType at) {
				EnterScope();
				Type loopvar = f.LoopVar.Accept(this);
				Cast(at.ElementType, loopvar);
				Define(loopvar, f.LoopVar.ID);
				Type body = f.Body.Accept(this);
				ExitScope();
				if(f.Body is Statement && !(f.Body is Expression) && body != null) return body;
				return null;
			}
			return Error("only array types are iterable, found:" + collection.ToString());
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

		#endregion
	}
}
