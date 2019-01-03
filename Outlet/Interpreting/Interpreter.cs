using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Outlet.Operands;
using Type = Outlet.Operands.Type;

namespace Outlet.Interpreting {
	public class Interpreter : IVisitor<Operand> {

		#region Helpers

		public static Stack<Scope> Scopes = new Stack<Scope>();

		public static Operand Interpret(Declaration program) {
			while(Scopes.Count != 1) Scopes.Pop();
			return program.Accept(Hidden);
		}
		private static readonly Interpreter Hidden = new Interpreter();
		private Interpreter() {
			Scopes.Push(Scope.Global);
		}

		public static Scope CurScope() => Scopes.Peek();

		public static Scope EnterScope() {
			if(Scopes.Count == 0) Scopes.Push(new Scope(null));
			else Scopes.Push(new Scope(Scopes.Peek()));
			return Scopes.Peek();
		}

		public static void ExitScope() => Scopes.Pop();

		#endregion

		public Operand Visit(AST.Program p) {
			return null;
		}

		public Operand Visit(ClassDeclaration c) {
			Scope closure = CurScope();
			Operand HiddenFunc(string s) {
				return closure.Get(0, s);
			}
			EnterScope();
			Class newclass = new Class(c.Name, CurScope(), c.InstanceDecls, c.StaticDecls, HiddenFunc);
			foreach(Declaration d in c.StaticDecls) {
				d.Accept(this);
			}
			ExitScope();
			CurScope().Add(c.Name, Primitive.MetaType, newclass);
			return null;
		}

		public Operand Visit(FunctionDeclaration f) {
			Scope closure = CurScope();
			Operand HiddenFunc(params Operand[] args) {
				Scope exec = new Scope(closure);
				Scopes.Push(exec);
				Operand returnval = null;
				for(int i = 0; i < args.Length; i++) {
					exec.Add(f.Type.Args[i].id, f.Type.Args[i].type, args[i]);
				}
				returnval = f.Body.Accept(this);
				ExitScope();
				return returnval;
			}
			var func = new Function(f.Decl.ID, f.Type, HiddenFunc);
			CurScope().Add(f.Decl.ID, func.Type, func);
			return null;
		}

		public Operand Visit(VariableDeclaration v) {
			Type type = (Type) v.Decl.Accept(this);
			Operand initial = v.Initializer?.Accept(this) ?? new Constant(type.Default);
			CurScope().Add(v.Decl.ID, type, initial);
			return null;
		}

		public Operand Visit(Declarator d) {
			Operand t = d.Type.Accept(this);
			if(t is Type type) return type;
			else throw new OutletException(d.Type.ToString() + " is not a valid type SHOULD NOT PRINT");
		}

		public Operand Visit(Literal c) => new Constant(c.Value);

		public Operand Visit(Access a) {
			Operand col = a.Collection.Accept(this);
			if(col is Type at && a.Index.Length == 0) return new ArrayType(at);
			Operands.Array c = (Operands.Array) a.Collection.Accept(this);
			// Index is 0 because all array access is one-dimensional as of now
			// multi-dimensional arrays can be accessed through chained accesses
			int idx = a.Index[0].Accept(this).Value;
			int len = c.Values().Length;
			if(idx >= len) throw new RuntimeException("array index out of bounds exception: index was " + idx + " array only goes to " + (len - 1));
			return c.Values()[idx];
		}

		public Operand Visit(Assign a) {
			Operand val = a.Right.Accept(this);
			if(a.Left is Variable v) {
				CurScope().Assign(v.resolveLevel, v.Name, val);
				return val;
			} else if(a.Left is Deref d && d.Accept(this) is Instance i) {
				i.Assign(d.Right, val);
				return val;
			} else throw new RuntimeException("cannot assign to the left side of this expression SHOULD NOT PRINT");
		}

		public Operand Visit(Binary b) => b.Oper.Perform(b.Left.Accept(this), b.Right.Accept(this));

		public Operand Visit(Call c) {
			Operand caller = c.Caller.Accept(this);
			var args = c.Args.Select(arg => arg.Accept(this)).ToArray();
			if(caller is Function f) return f.Call(args);
			else throw new RuntimeException(caller.Type.ToString() + " is not callable SHOULD NOT PRINT");
		}

		public Operand Visit(Deref d) {
			Operand left = d.Left.Accept(this);
			if(left is Operands.Array a) return new Constant(a.Values().Length);
			if(left is NativeClass nc) return nc.Methods[d.Right];
			throw new NotImplementedException();
		}

		public Operand Visit(Is i) {
			bool val = i.Left.Accept(this).Type.Is((Type)i.Right.Accept(this));
			return new Constant(i.NotIsnt ? val : !val); 
		}

		public Operand Visit(Lambda l) {
			Operand left = l.Left.Accept(this);
			Operand right = l.Right.Accept(this);
			if(left is TupleType tt && right is Type r)
				return new FunctionType(tt.Types.Select(x => (x, "")).ToArray(), r);
			throw new NotImplementedException();
		}

		public Operand Visit(ListLiteral l) => new Operands.Array(l.Args.Select(arg => arg.Accept(this)).ToArray());

		public Operand Visit(ShortCircuit s) {
			bool b = s.Left.Accept(this).Value;
			if(s.isand == b) {
				return new Constant(s.Right.Accept(this).Value);
			} else return new Constant(!s.isand);
		}

		public Operand Visit(Ternary t) {
			if(t.Condition.Accept(this).Value is bool b) {
				return b ? t.IfTrue.Accept(this) : t.IfFalse.Accept(this);
			}
			throw new RuntimeException("expected boolean in ternary condition SHOULD NOT PRINT");
		}

		public Operand Visit(TupleLiteral t) {
			IEnumerable<Operand> evaled = t.Args.Select(arg => arg.Accept(this));
			if(evaled.All(elem => elem is Type)) return new TupleType(evaled.Select(elem => elem as Type).ToArray());
			if(t.Args.Length == 1) return t.Args[0].Accept(this);
			else return new OTuple(evaled.ToArray());
		}

		public Operand Visit(Unary u) => u.Oper.Perform(u.Expr.Accept(this));

		public Operand Visit(Variable v) {
			if(v.resolveLevel == -1) {
				throw new RuntimeException("could not find variable, THIS SHOULD NEVER PRINT");
			} else return CurScope().Get(v.resolveLevel, v.Name);
		}

		public Operand Visit(Block b) {
			EnterScope();
			foreach(FunctionDeclaration fd in b.Functions) {
				fd.Accept(this);
			}
			foreach(Declaration line in b.Lines) {
				if(line is FunctionDeclaration) continue;
				var ret = line.Accept(this);
				if(line is Statement s && !(s is Expression) && ret != null) {
					ExitScope();
					return ret;
				}
			}
			ExitScope();
			return null;
		}

		public Operand Visit(ForLoop f) {
			Operands.Array c = (Operands.Array) f.Collection.Accept(this);
			foreach(Operand o in c.Values()) {
				EnterScope();
				Type looptype = (Type)f.LoopVar.Accept(this);
				CurScope().Add(f.LoopVar.ID, looptype, o);
				Operand res = f.Body.Accept(this);
				ExitScope();
				if(f.Body is Statement s && !(s is Expression) && res != null) {
					return res;
				}
			}
			return null;
		}

		public Operand Visit(IfStatement i) {
			if(i.Condition.Accept(this).Value is bool b && b) {
				var ret = i.Iftrue.Accept(this);
				if(i.Iftrue is Statement s && !(s is Expression)) return ret;
			} else {
				var ret = i.Iffalse?.Accept(this);
				if(i.Iffalse != null && i.Iffalse is Statement s && !(s is Expression)) return ret;
			}
			return null;
		}

		public Operand Visit(ReturnStatement r) {
			return r.Expr.Accept(this);
		}

		public Operand Visit(WhileLoop w) {
			while(w.Condition.Accept(this).Value is bool b && b) {
				w.Body.Accept(this);
			}
			return null;
		}
	}
}
