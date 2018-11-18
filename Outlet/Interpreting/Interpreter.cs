using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Type = Outlet.AST.Type;

namespace Outlet.Interpreting {
	public class Interpreter : IVisitor<Operand> {

		#region Helpers

		public static Stack<Scope> Scopes = new Stack<Scope>();

		static Interpreter() {
			Scopes.Push(Scope.Global);
		}

		public Scope CurScope() => Scopes.Peek();

		public Scope EnterScope() {
			if(Scopes.Count == 0) Scopes.Push(new Scope(null));
			else Scopes.Push(new Scope(Scopes.Peek()));
			return Scopes.Peek();
		}

		public void ExitScope() => Scopes.Pop();

		#endregion

		public Operand Visit(ClassDeclaration c) {
			EnterScope();
			Class newclass = new Class(c.Name, CurScope(), c.InstanceDecls, c.StaticDecls);
			foreach(Declaration d in c.StaticDecls) {
				d.Accept(this);
			}
			ExitScope();
			CurScope().Add(c.Name, Primitive.MetaType, newclass);
			return null;
		}

		public Operand Visit(FunctionDeclaration f) {
			var args = f.Args.Select(x => ((Type)x.Accept(this), x.ID)).ToList();
			var func = new Function(CurScope(), f.Decl.ID, (Type) f.Decl.Accept(this), args, f.Body);
			CurScope().Add(f.Decl.ID, null, func);
			return null;
		}

		public Operand Visit(VariableDeclaration v) {
			Operand initial = v.Initializer?.Accept(this);
			Type type = (Type) v.Decl.Accept(this);
			Type initType = initial?.Type;
			CurScope().Add(v.Decl.ID, type, initial);
			return null;
		}

		public Operand Visit(Declarator d) {
			Operand t = d.Type.Accept(this);
			if(t is Type type) return type;
			else throw new OutletException(d.Type.ToString() + " is not a valid type SHOULD NOT PRINT");
		}

		public Operand Visit(Const c) => c;

		public Operand Visit(Assign a) {
			Operand val = a.Right.Accept(this);
			if(a.Left is Variable v) {
				CurScope().Assign(v.resolveLevel, v.Name, val);
				return val;
			} else if(a.Left is Deref d && d.Accept(this) is Instance i) {
				i.Assign(d.Right, val);
				return val;
			} else throw new OutletException("cannot assign to the left side of this expression SHOULD NOT PRINT");
		}

		public Operand Visit(Binary b) => b.Oper.Perform(b.Left.Accept(this), b.Right.Accept(this));

		public Operand Visit(Call c) {
			Operand Left = c.Caller.Accept(this);
			var args = c.Args.Select(arg => arg.Accept(this)).ToArray();
			if(Left is Native n) return n.Call(args);
			if(Left is Function f) {
				Scope exec = new Scope(f.Closure);
				Scopes.Push(exec);
				Operand returnval = null;
				for(int i = 0; i < args.Length; i++) {
					exec.Add(f.ArgNames[i].ID, f.ArgNames[i].Type, args[i]);
				}
				try {
					if(f.Body is Expression e) returnval = e.Accept(this);
					else f.Body.Accept(this);
				} catch(Return r) {
					returnval = r.Value;
				}
				ExitScope();
				return returnval;
			}
			else throw new OutletException(Left.Type.ToString() + " is not callable SHOULD NOT PRINT");
		}

		public Operand Visit(Deref d) {
			throw new NotImplementedException();
		}

		public Operand Visit(Lambda l) {
			throw new NotImplementedException();
		}

		public Operand Visit(ListLiteral l) => new OList(l.Args.Select(arg => arg.Accept(this)).ToArray());

		public Operand Visit(ShortCircuit s) {
			bool b = s.Left.Accept(this).Value;
			if(s.isand == b) {
				return new Const(s.Right.Accept(this).Value);
			} else return new Const(!s.isand);
		}

		public Operand Visit(Ternary t) {
			if(t.Condition.Accept(this).Value is bool b) {
				return b ? t.IfTrue.Accept(this) : t.IfFalse.Accept(this);
			}
			throw new OutletException("expected boolean in ternary condition SHOULD NOT PRINT");
		}

		public Operand Visit(TupleLiteral t) {
			if(t.Args.Length == 1) return t.Args[0].Accept(this);
			IEnumerable<Operand> evaled = t.Args.Select(arg => arg.Accept(this));
			if(evaled.All(elem => elem is Type)) return new TupleType(evaled.Select(elem => elem as Type).ToArray());
			else return new OTuple(evaled.ToArray());
		}

		public Operand Visit(Unary u) => u.Oper.Perform(u.Expr.Accept(this));

		public Operand Visit(Variable v) {
			if(v.resolveLevel == -1) {
				throw new OutletException("could not find variable, THIS SHOULD NEVER PRINT");
			} else return CurScope().Get(v.resolveLevel, v.Name);
		}

		public Operand Visit(Block b) {
			EnterScope();
			b.Lines.ForEach(line => line.Accept(this));
			ExitScope();
			return null;
		}

		public Operand Visit(ForLoop f) {
			/*
			EnterScope();
			Operand c = f.Collection.Accept(this);
			Type looptype = f.LoopVar.GetType(Scope());
			if(c is ICollection collect) {
				foreach(Operand o in collect.Values()) {
					o.Cast(looptype);
					Scope().Add(f.LoopVar.ID, looptype, o);
					f.Body.Accept(this);
					ExitScope();
					EnterScope();
				}
			} else throw new OutletException("for loops can only loop over collections");*/
			return null;
		}

		public Operand Visit(IfStatement i) {
			if(i.Condition.Accept(this).Value is bool b && b) i.Iftrue.Accept(this);
			else i.Iffalse?.Accept(this);
			return null;
		}

		public Operand Visit(ReturnStatement r) {
			throw new Return(r.Expr.Accept(this));
		}

		public Operand Visit(WhileLoop w) {
			while(w.Condition.Accept(this).Value is bool b && b) {
				w.Body.Accept(this);
			}
			return null;
		}
	}
}
