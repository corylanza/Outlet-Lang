using System.Collections.Generic;
using System.Linq;
using Outlet.AST;
using Outlet.Operands;
using Outlet.Types;
using Type = Outlet.Types.Type;

namespace Outlet.Interpreting {
	public class Interpreter : IVisitor<Operand> {

		#region Helpers

		public Stack<Scope> Scopes = new Stack<Scope>();
        public Stack<StackFrame> CallStack = new Stack<StackFrame>();

        public Interpreter()
        {
            Scopes.Push(Scope.Global);
        }

        public Operand Interpret(IASTNode program) {
			while(Scopes.Count != 1) Scopes.Pop();
            try
            {
                return program.Accept(this);
            } catch (RuntimeException r)
            {
                throw new RuntimeException(r.Message, this);
            }
		}

		public Scope CurScope() => Scopes.Peek();
        //public Operand GetLocalVariable(Variable v) => CallStack.ElementAt(v.resolveLevel).LocalVariables.ElementAt(v.id);

		public Scope EnterScope() {
			if(Scopes.Count == 0) Scopes.Push(new Scope(null));
			else Scopes.Push(new Scope(Scopes.Peek()));
			return Scopes.Peek();
		}

		public void ExitScope() => Scopes.Pop();

		#endregion

		#region Declarations

		public Operand Visit(ClassDeclaration c) {
			// Find super class, if none it will always be object
			Class super = c.SuperClass != null ? (c.SuperClass.Accept(this) as TypeObject).Encapsulated as Class : Primitive.Object;

            // Enter new scope and declare all statics there
            Scope closure = EnterScope();

            // if there are any generic parameters define their value as their class constraint
            foreach (var (id, classConstraint) in c.GenericParameters)
            {
                Class constraint = classConstraint?.Accept(this) is TypeObject to && to.Encapsulated is Class co ? co : Primitive.Object;
                CurScope().Add(id, Primitive.MetaType, new TypeObject(constraint));
            }

            var staticFields = new Dictionary<string, Field>();
            foreach (Declaration d in c.StaticDecls) d.Accept(this);
            foreach (var constructor in c.Constructors) constructor.Accept(this);
            foreach (var (id, value) in CurScope().List()) staticFields.Add(id, new Field { Value = value });

			// Hidden function for initializing instance variables/methods
			void Init(Instance i) {
				foreach(Declaration d in c.InstanceDecls) d.Accept(this);
                foreach(var (id, value) in CurScope().List()) i.SetMember(id, value);
				if(super is UserDefinedClass udc) udc.Init(i);
			}
            // Create class
            UserDefinedClass newclass = new UserDefinedClass(c.Name, super, staticFields, Init);
			// leave the static scope
			ExitScope();
			// Define the class
			CurScope().Add(c.Name, Primitive.MetaType, new TypeObject(newclass));
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
                UserDefinedClass type = (staticscope.Get(1, c.Decl.Type.ToString()) as TypeObject).Encapsulated as UserDefinedClass;

                // Create the new instance, initialize all fields, and define this
                Instance inst = new UserDefinedInstance(type);
                type.Init(inst);
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
			var func = new UserDefinedFunction(c.Decl.ID, c.Type, UnderlyingConstructor);
			staticscope.Add("", func.RuntimeType, func);
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
			var func = new UserDefinedFunction(f.Decl.ID, f.Type, HiddenFunc);
			CurScope().Add(f.Decl.ID, func.RuntimeType, func);
			return null;
		}

		public Operand Visit(VariableDeclaration v) {
			TypeObject type = (TypeObject) v.Decl.Accept(this);
			Operand initial = v.Initializer?.Accept(this) ?? type.Encapsulated.Default();
			CurScope().Add(v.Decl.ID, type.Encapsulated, initial);
			return initial;
		}

		#endregion

		#region Expressions

		public Operand Visit(Declarator d) {
			Operand t = d.Type.Accept(this);
			if(t is TypeObject type) return type;
			else throw new OutletException(d.Type.ToString() + " is not a valid type SHOULD NOT PRINT");
		}

		public Operand Visit<E>(Literal<E> c) {
			if(c.Value != null) return new Constant<E>(c.Type, c.Value);
			return Constant.Null();
		}

		public Operand Visit(Access a) {
			Operand col = a.Collection.Accept(this);
			if(col is TypeObject at && a.Index.Length == 0) return new TypeObject(new ArrayType(at.Encapsulated));
            if(col is Array c)
            {
                // Index is 0 because all array access is one-dimensional as of now
                // multi-dimensional arrays can be accessed through chained accesses
                int idx = (a.Index[0].Accept(this) as Constant<int>).Value;
                int len = c.Values().Length;
                if (idx >= len) throw new RuntimeException("array index out of bounds exception: index was " + idx + " array only goes to " + (len - 1));
                return c.Values()[idx];
            }
            throw new OutletException("cannot acccess this type SHOULD NOT PRINT");
		}

		public Operand Visit(As a) {
			Operand l = a.Left.Accept(this);
			TypeObject t = (TypeObject) a.Right.Accept(this);
			if(l.GetOutletType().Is(t.Encapsulated)) return l;
			throw new RuntimeException("cannot implicitly cast " + l.GetOutletType().ToString() + " to " + t.ToString());
		}

		public Operand Visit(Assign a) {
			Operand val = a.Right.Accept(this);
			if(a.Left is Variable v) {
				CurScope().Assign(v.resolveLevel, v.Name, val);
				return val;
			} else if(a.Left is Deref d && d.Left.Accept(this) is IDereferenceable dereferenced) {
                dereferenced.SetMember(d.Identifier, val);
                return val;
			}
			throw new RuntimeException("cannot assign to the left side of this expression SHOULD NOT PRINT");
		}

		public Operand Visit(Binary b) => b.Oper.Perform(b.Left.Accept(this), b.Right.Accept(this));

		public Operand Visit(Call c) {
			Operand caller = c.Caller.Accept(this);
			var args = c.Args.Select(arg => arg.Accept(this)).ToArray();
            if (caller is ICallable f)
            {
                CallStack.Push(new StackFrame("called " + caller.ToString()));
                var returned = f.Call(args);
                CallStack.Pop();
                return returned;
            }
            else throw new RuntimeException(caller.GetOutletType().ToString() + " is not callable SHOULD NOT PRINT");
		}

		public Operand Visit(Deref d) {
			Operand left = d.Left.Accept(this);
			if(left is Array a && d.ArrayLength) return Constant.Int(a.Values().Length);
            if (left is OTuple t && int.TryParse(d.Identifier, out int idx)) return t.Values()[idx];
            if (left is IDereferenceable dereferenceable) return dereferenceable.GetMember(d.Identifier);
			if(left is Constant<object> n && n.Value is null) throw new RuntimeException("null pointer exception");
			throw new RuntimeException("Illegal dereference THIS SHOULD NOT PRINT");
		}

		public Operand Visit(Is i) {
			bool val = i.Left.Accept(this).GetOutletType().Is(((TypeObject) i.Right.Accept(this)).Encapsulated);
			return Constant.Bool(i.NotIsnt ? val : !val); 
		}

		public Operand Visit(Lambda l) {
			Operand left = l.Left.Accept(this);
			Operand right = l.Right.Accept(this);
			if(left is TypeObject lt && lt.Encapsulated is TupleType tt && right is TypeObject rt)
				return new TypeObject(new FunctionType(tt.Types.Select(x => (x, "")).ToArray(), rt.Encapsulated));
			throw new RuntimeException("lambda invalid SHOULD NOT PRINT");
		}

		public Operand Visit(ListLiteral l) => new Operands.Array(l.Args.Select(arg => arg.Accept(this)).ToArray());

		public Operand Visit(ShortCircuit s) {
			bool left= (s.Left.Accept(this) as Constant<bool>).Value;
			if(s.isand == left) {
                bool right = (s.Right.Accept(this) as Constant<bool>).Value;
                return Constant.Bool(right);
			} else return Constant.Bool(!s.isand);
		}

		public Operand Visit(Ternary t) {
			if(t.Condition.Accept(this) is Constant<bool> b) {
				return b.Value ? t.IfTrue.Accept(this) : t.IfFalse.Accept(this);
			}
			throw new RuntimeException("expected boolean in ternary condition SHOULD NOT PRINT");
		}

		public Operand Visit(TupleLiteral t) {
			IEnumerable<Operand> evaled = t.Args.Select(arg => arg.Accept(this));
			if(evaled.All(elem => elem is TypeObject)) 
                return new TypeObject(new TupleType(evaled.Select(elem => (elem as TypeObject).Encapsulated).ToArray()));
			if(t.Args.Length == 1) return t.Args[0].Accept(this);
			else return new OTuple(evaled.ToArray());
		}

		public Operand Visit(Unary u) => u.Oper.Perform(u.Expr.Accept(this));

		public Operand Visit(Variable v) {
            if (v.resolveLevel == -1)
            {
                throw new RuntimeException("could not find variable, THIS SHOULD NEVER PRINT");
            }
            //if (Scope.Global.Has(v.Name)) 
                return CurScope().Get(v.resolveLevel, v.Name);
            //else return GetLocalVariable(v);
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
				Type looptype = (f.LoopVar.Accept(this) as TypeObject).Encapsulated;
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
			if(i.Condition.Accept(this) is Constant<bool> b && b.Value) {
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
			while(w.Condition.Accept(this) is Constant<bool> b && b.Value) {
				var ret = w.Body.Accept(this);
                if (w.Body is Statement s && !(s is Expression) && ret != null) return ret;
            }
			return null;
		}

        public Operand Visit(UsingStatement u)
        {
            Operand toUse = u.Used.Accept(this);
            if (toUse is TypeObject rc) 
            {
                foreach(var (id, val) in rc.GetMembers())
                {
                    CurScope().Add(id, val.GetOutletType(), val);
                }
                return null;
            } 
            else throw new OutletException(u + " is not a valid using statement");
        }

		#endregion
	}
}
