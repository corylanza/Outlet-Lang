using System;
using System.Collections.Generic;
using System.Linq;
using Outlet.AST;
using Outlet.Operands;
using Type = Outlet.Operands.Type;

namespace Outlet.Interpreting {
	public class Interpreter : IVisitor<Operand> {

		#region Helpers

		public static Stack<Scope> Scopes = new Stack<Scope>();

		public static Operand Interpret(IASTNode program) {
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

		#region Declarations

		public Operand Visit(ClassDeclaration c) {
			// Find super class, if none it will always be object
			Class super = c.SuperClass != null ? c.SuperClass.Accept(this) as Class : Primitive.Object;
			// Enter new scope and declare all statics there
			Scope closure = EnterScope();
			foreach(Declaration d in c.StaticDecls) d.Accept(this);
			// Hidden functions for getting and setting statics
			Operand Get(string s) => closure.Get(0, s);
			void Set(string s, Operand val) => closure.Assign(0, s, val);
			// Create class
			UserDefinedClass newclass = new UserDefinedClass(c.Name, super, Get, Set);
			// Hidden function for initializing instance variables/methods
			void Init() {
				foreach(Declaration d in c.InstanceDecls) d.Accept(this);
				if(newclass.Parent is UserDefinedClass udc) udc.Init();
			}
			// Give the constructor this hidden initializer then declare it
			newclass.Init = Init;
			c.Constructor.Accept(this);
			// leave the static scope
			ExitScope();
			// Define the class
			CurScope().Add(c.Name, Primitive.MetaType, newclass);
			return null;
		}

		public Operand Visit(ConstructorDeclaration c) {
			// Captures the static scope of the class in which the constructor is declared
			Scope staticscope = CurScope();
			Operand UnderlyingConstructor(params Operand[] args) {
				// Enter the instance scope
				Scope instancescope = new Scope(staticscope);
				Scopes.Push(instancescope);
				// Call the constructors hidden init function to initialize instance variables/methods
				UserDefinedClass type = staticscope.Get(1, c.Decl.Type.ToString()) as UserDefinedClass;
				type.Init();
				// Hidden functions that act as getters and setters for instance variables/methods
				void Set(string s, Operand val) => instancescope.Assign(0, s, val);
				Operand Get(string s) => instancescope.Get(0, s);
				// Create the new instance and define this
				Instance inst = new Instance(type, Get, Set);
				instancescope.Add("this", type, inst);
				// Enter the scope of the constructor
				Scope constructorscope = new Scope(instancescope);
				Scopes.Push(constructorscope);
				// Add all parameters to constructor scope 
				for(int i = 0; i < args.Length; i++) {
					constructorscope.Add(c.Type.Args[i].id, c.Type.Args[i].type, args[i]);
				}
				// Evaluate the body of the constructor
				c.Body.Accept(this);
				// Go back to the static scope
				ExitScope();
				ExitScope();
				return inst;
			}
			// Define the constructor as "" in the static scope (this is a special case
			// as it cannot be stored in the instance scope despite its being resolved 
			// at the instance level)
			var func = new Function(c.Decl.ID, c.Type, UnderlyingConstructor);
			staticscope.Add("", func.Type, func);
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
			Type type = (Type)v.Decl.Accept(this);
			Operand initial = v.Initializer?.Accept(this) ?? new Constant(type.Default());
			CurScope().Add(v.Decl.ID, type, initial);
			return null;
		}

		#endregion

		#region Expressions

		public Operand Visit(Declarator d) {
			Operand t = d.Type.Accept(this);
			if(t is Type type) return type;
			else throw new OutletException(d.Type.ToString() + " is not a valid type SHOULD NOT PRINT");
		}

		public Operand Visit(Literal c) {
			if(c.Value != null) return new Constant(c.Value);
			return new Constant();
		}

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

		public Operand Visit(As a) {
			Operand l = a.Left.Accept(this);
			Type t = (Type) a.Right.Accept(this);
			if(l.Type.Is(t)) return l;
			throw new RuntimeException("cannot implicitly cast " + l.Type.ToString() + " to " + t.ToString());
		}

		public Operand Visit(Assign a) {
			Operand val = a.Right.Accept(this);
			if(a.Left is Variable v) {
				CurScope().Assign(v.resolveLevel, v.Name, val);
				return val;
			} else if(a.Left is Deref d) {
				Operand temp = d.Left.Accept(this);
				if(temp is Instance i) {
					i.SetInstanceVar(d.Right, val);
					return val;
				} else if(temp is IRuntimeClass c) {
					c.SetStatic(d.Right, val);
					return val;
				}
			}
			throw new RuntimeException("cannot assign to the left side of this expression SHOULD NOT PRINT");
		}

		public Operand Visit(Binary b) => b.Oper.Perform(b.Left.Accept(this), b.Right.Accept(this));

		public Operand Visit(Call c) {
			Operand caller = c.Caller.Accept(this);
			var args = c.Args.Select(arg => arg.Accept(this)).ToArray();
			if(caller is UserDefinedClass cl) caller = cl.GetStatic("");
			if(caller is Function f) return f.Call(args);
			else throw new RuntimeException(caller.Type.ToString() + " is not callable SHOULD NOT PRINT");
		}

		public Operand Visit(Deref d) {
			Operand left = d.Left.Accept(this);
			if(left is Operands.Array a) return new Constant(a.Values().Length);
			if(left is IRuntimeClass c) return c.GetStatic(d.Right);
			if(left is Instance i) return i.GetInstanceVar(d.Right);
			if(left is Constant n && n.Value is null) throw new RuntimeException("null pointer exception");
			throw new RuntimeException("Illegal dereference THIS SHOULD NOT PRINT");
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
			throw new RuntimeException("lambda invalid SHOULD NOT PRINT");
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

		#endregion

		# region Statements

		public Operand Visit(Block b) {
			EnterScope();
			foreach(ClassDeclaration cd in b.Classes) {
				cd.Accept(this);
			}
			foreach(FunctionDeclaration fd in b.Functions) {
				fd.Accept(this);
			}
			foreach(IASTNode line in b.Lines) {
				if(line is FunctionDeclaration) continue;
				if(line is ClassDeclaration) continue;
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

		#endregion
	}
}
