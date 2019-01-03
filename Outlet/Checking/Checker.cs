using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Outlet.Operands;
using Type = Outlet.Operands.Type;

namespace Outlet.Checking {
	public class Checker : IVisitor<Type> {

		#region Helpers

		public static readonly Stack<bool> DoImpl = new Stack<bool>();
		public static readonly Stack<Scope> Scopes = new Stack<Scope>();
		private static readonly Type ErrorType = new Class("error", null, null, null, null);
		public static int ErrorCount = 0;

		public static void Check(Declaration program) {
			ErrorCount = 0;
			if(program is FunctionDeclaration fd) {
				DoImpl.Push(false);
				fd.Accept(Hidden);
				DoImpl.Pop();
				DoImpl.Push(true);
				fd.Accept(Hidden);
				DoImpl.Pop();
			} else program.Accept(Hidden);
			if(ErrorCount > 0) throw new CheckerException(ErrorCount + " Checking errors encountered");
		}
		private static readonly Checker Hidden = new Checker();
		private Checker() => Scopes.Push(Scope.Global);

		public static void Declare(Type t, string s) => Scopes.Peek().Declare(t, s);
		public static void Define(Type t, string s) => Scopes.Peek().Define(t, s);
		public static void DefineType(Type t, string s) => Scopes.Peek().DefineType(t, s);
		public static (Type, int) Find(string s) => Scopes.Peek().Find(s);
		public static Type FindType(int level, string s) => Scopes.Peek().FindType(level, s);

		public static Scope EnterScope() {
			if(Scopes.Count == 0) Scopes.Push(new Scope(null));
			else Scopes.Push(new Scope(Scopes.Peek()));
			return Scopes.Peek();
		}

		public static void ExitScope() => Scopes.Pop();

		public static Type Error(string message) {
			ErrorCount++;
			Program.ThrowException(message);
			return ErrorType;
		}

		public static void Cast(Type from, Type to, string message = "cannot convert type {0} to type {1}") {
			if(from == ErrorType || to == ErrorType) return;
			if(!from.Is(to)) Error(string.Format(message, from, to));
		}

		private readonly Type Bool = Primitive.Bool;

		private Type TypeLiteral(Expression e) {
			if(e is Variable v) {
				v.Accept(this); // resolves the type literal so it can be found when interpreted
				return FindType(v.resolveLevel, v.Name);
			}
			if(e is TupleLiteral tl) return new TupleType(tl.Args.Select(arg => TypeLiteral(arg)).ToArray());
			if(e is Lambda l) return new FunctionType(((TupleType) TypeLiteral(l.Left)).Types.Select(x => (x, "")).ToArray(), TypeLiteral(l.Right));
			if(e is Access a) return new ArrayType(TypeLiteral(a.Collection));
			return Error("declaration requires valid type, found: "+e.ToString());
		}

		#endregion

		public Type Visit(AST.Program p) {
			return null;
		}

		public Type Visit(ClassDeclaration c) {
			EnterScope();
			foreach(Declaration d in c.StaticDecls) {
				d.Accept(this);
			}
			EnterScope();
			//Define(Primitive.MetaType, "this");
			foreach(Declaration d in c.InstanceDecls) {
				d.Accept(this);
			}
			ExitScope();
			ExitScope();
			if(!DoImpl.Peek()) {
				DefineType(new Class(c.Name, Scopes.Peek(), null, null, null), c.Name);
			}
			return null;
		}

		public Type Visit(FunctionDeclaration f) {
			if(!DoImpl.Peek()) {
				// Check decl and args first, needed to make function type
				EnterScope();
				(Type type, string id)[] args = f.Args.Select(arg => {
					Type curarg = arg.Accept(this);
					//if(curarg == Primitive.MetaType) DefineType(new Class(arg.ID, null, null, null), arg.ID);
					return (curarg, arg.ID);
				}).ToArray();
				Type returntype = f.Decl.Accept(this);
				ExitScope();
				FunctionType ft = new FunctionType(args, returntype);
				f.Type = ft;
				// define the header using the function type from above
				Define(ft, f.Decl.ID);
			} else {
				// enter the function scope and define the args;
				EnterScope();
				System.Array.ForEach(f.Type.Args, arg => Define(arg.type, arg.id));
				// check the body now that its header and args have been defined
				Type body = f.Body.Accept(this);
				if(body == null || body == Primitive.Void) {
					if(f.Type.ReturnType != Primitive.Void) return Error("function "+f.Decl.ID+"not all code paths return a value");
				} else Cast(body, f.Type.ReturnType, f.Decl.ID+"function definition invalid, expected {1}, returned {0}");
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

		public Type Visit(Declarator d) => TypeLiteral(d.Type);

		public Type Visit(Literal c) => c.Type;

		public Type Visit(Access a) {
			Type elem = a.Collection.Accept(this);
			if(elem == Primitive.MetaType) return Primitive.MetaType;
			Type idxType = a.Index[0].Accept(this);
			if(idxType != Primitive.Int) return Error("only ints can be used to index into an array, found: " + idxType.ToString());
			return ((ArrayType) elem).ElementType;
		}

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
			if(op == null) return Error("binary operator not defined for " + left.ToString() + " " + b.Op + " " + right.ToString());
			b.Oper = op;
			return op.Result;
		}

		public Type Visit(Call c) {
			Type calltype = c.Caller.Accept(this);
			Type[] argtypes = c.Args.Select(x => x.Accept(this)).ToArray();
			if(calltype is FunctionType functype) {     // doesnt work for other callable types
				var funcargs = functype.Args.ToArray();
				if(argtypes.Length != funcargs.Length)
					return Error("function " + c.Caller.ToString() + " expects (" + funcargs.Select(x => x.type).ToList().ToListString() + ") found: (" + argtypes.ToList().ToListString()+")");
				for(int i = 0; i < c.Args.Length; i++) {
					if(funcargs[i].type == Primitive.MetaType) {
						for(int j = i+1; j < funcargs.Length; j++) {
							if(funcargs[j].type.ToString() == funcargs[i].id) funcargs[j].type = TypeLiteral(c.Args[i]);
						}
					}
					if(!argtypes[i].Is(funcargs[i].type))
						return Error("function "+c.Caller.ToString()+" expects ("+ funcargs.Select(x => x.type).ToList().ToListString() + ") found: ("+argtypes.ToList().ToListString()+")");
				}
				return functype.ReturnType;
			}
			return Error("not callable");
		}

		public Type Visit(Deref d) {
			Type inst = d.Left.Accept(this);
			if(inst is ArrayType && d.Right == "length") return Primitive.Int;
			if(inst == Primitive.MetaType) {
				Type actual = TypeLiteral(d.Left);
				if(actual is NativeClass nc) {
					if(nc.Methods.ContainsKey(d.Right)) return nc.Methods[d.Right].Type;
					return Error("type ___ does not contain " + d.Right);
				} else if(actual is Class c) {
					return Error("classsss");
				}
				return Error("not implemented");
			}
			return Error("not implemented");
		}

		public Type Visit(Is i) {
			i.Left.Accept(this);
			Type r = i.Right.Accept(this);
			if(r != Primitive.MetaType) return Error("the right side of an is expression must be a type, found: "+r.ToString());
			return Primitive.Bool;
		}

		public Type Visit(Lambda l) {
			Type args = l.Left.Accept(this);
			Type res = l.Right.Accept(this);
			//if(args == Primitive.MetaType && res == Primitive.MetaType) return new FunctionType(ar)
			return Error("Lambdas currently only work for types");
		}

		public Type Visit(ListLiteral l) {
			if(l.Args.Length == 0) return new ArrayType(Primitive.Object);
			Type ancestor = l.Args[0].Accept(this);
			foreach(Expression cur in l.Args) {
				ancestor = Type.CommonAncestor(ancestor, cur.Accept(this));
			}
			return new ArrayType(ancestor);
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
			var op = u.Overloads.Best(input);
			if(op == null) return Error("unary operator " + u.Op + " is not defined for type " + input.ToString());
			u.Oper = op;
			return op.Result;
		}

		public Type Visit(Variable v) {
			(Type t, int l) = Find(v.Name);
			v.resolveLevel = l;
			if(l == -1) return Error("Line: "+v.Line+" variable " + v.Name + " could not be resolved");
			return t;
		}

		public Type Visit(Block b) {
			EnterScope();
			Type ret = null;
			// Forward Declaration of Classes
			DoImpl.Push(false);
			foreach(ClassDeclaration cd in b.Classes) {
				cd.Accept(this);
				//throw new NotImplementedException("classes need forward declaration");
				//DefineType(new Class(cd.Name, null, cd.InstanceDecls, cd.StaticDecls), cd.Name);
			}

			// Forward Declaration of Functions
			foreach(FunctionDeclaration fd in b.Functions) {
				fd.Accept(this);
			}

			DoImpl.Pop();
			DoImpl.Push(true);
			foreach(Declaration d in b.Lines) {
				if(!(d is ClassDeclaration)) {
					Type temp = d.Accept(this);
					if(ret != null) return Error("unreachable code detected");
					if(d is Statement && !(d is Expression) && temp != null) {
						ret = temp;
					}
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
	}
}
