using System.Collections.Generic;
using System.Linq;
using Outlet.AST;
using Outlet.Operands;
using Outlet.Types;
using Type = Outlet.Types.Type;

namespace Outlet.Interpreting {
	public class Interpreter : IVisitor<Operand> {

		#region Helpers

        public readonly Stack<StackFrame> CallStack = new Stack<StackFrame>();
        public StackFrame CurrentStackFrame => CallStack.Peek();

        public Interpreter()
        {
            CallStack.Push(StackFrame.Global);
        }

        public Operand Interpret(IASTNode program) {
            while(CallStack.Count > 1) CallStack.Pop();
            try
            {
                if (CallStack.Count == 0) CallStack.Push(StackFrame.Global); 
                return program.Accept(this);
            } catch (RuntimeException r)
            {
                throw new RuntimeException(r.Message, this);
            }
		}

        public StackFrame EnterStackFrame(int localCount, string call)
        {
            var newStackFrame = new StackFrame(CurrentStackFrame, localCount, call);
            CallStack.Push(newStackFrame);
            return newStackFrame;
        }

		#endregion

		#region Declarations

		public Operand Visit(ClassDeclaration c) {
			// Find super class, if none it will always be object
			Class super = c.SuperClass != null ? (c.SuperClass.Accept(this) as TypeObject).Encapsulated as Class : Primitive.Object;

            StackFrame closure = CurrentStackFrame;

            var staticFields = new Field[c.StaticDecls.Count + c.Constructors.Count];
            StackFrame staticScope = new StackFrame(closure, staticFields.Length, $"{c.Name} static scope");

            // Hidden function for initializing instance variables/methods and defining this
            (Instance, StackFrame) Init(UserDefinedClass type)
            {
                // Enter the instance scope
                // add one to instance count for "this" which is not a member but lives in instance scope
                StackFrame instancescope = new StackFrame(staticScope, c.InstanceDecls.Count + 1, "class scope");
                CallStack.Push(instancescope);

                UserDefinedInstance instance = new UserDefinedInstance(type, c.InstanceDecls.Count);
                foreach (Declaration d in c.InstanceDecls)
                {
                    instance.SetMember(d.Decl, d.Accept(this));
                }
                // TODO for super classes
                //if (super is UserDefinedClass udc) udc.Init();
                // TODO for native classes


                // Assign the value of "this"
                instancescope.LocalVariables[Class.This] = instance;
                // Do NOT exit instance scope frame, the constructor method lives on top of it
                return (instance, instancescope);
            }


            // Define the class
            UserDefinedClass newClass = new UserDefinedClass(c.Name, super, staticFields, Init);
            var newType = new TypeObject(newClass);
            CurrentStackFrame.Assign(c.Decl, newType);

            // if there are any generic parameters define their value as their class constraint
            foreach (var (id, classConstraint) in c.GenericParameters)
            {
                //Class constraint = classConstraint?.Accept(this) is TypeObject to && to.Encapsulated is Class co ? co : Primitive.Object;
                // TODO reimplement
                //CurrentStackFrame.Initialize(id, new TypeObject(constraint));
            }

            int fieldNum = 0;
            CallStack.Push(staticScope);
            foreach (Declaration d in c.StaticDecls)
            {
                staticFields[fieldNum++] = new Field(d.Name, d.Accept(this));
            }
            foreach(ConstructorDeclaration constructor in c.Constructors)
            {
                staticFields[fieldNum++] = new Field("", constructor.Accept(this));
            }
            CallStack.Pop();
			return newType;
		}

		public Operand Visit(ConstructorDeclaration c) {
			// Captures the static scope of the class in which the constructor is declared
            StackFrame staticscope = CurrentStackFrame;
			Operand UnderlyingConstructor(params Operand[] args) {
                UserDefinedClass type = (staticscope.Get(c.Decl.Type as Variable) as TypeObject).Encapsulated as UserDefinedClass;

                // Call the constructors hidden init function to initialize instance variables/methods
                (Instance inst, StackFrame instancescope) = type.Initialize();

                // Enter the scope of the constructor
                StackFrame constructorscope = new StackFrame(instancescope, c.LocalCount, "constructor scope");
                CallStack.Push(constructorscope);
				// Add all parameters to constructor scope 
				for(int i = 0; i < args.Length; i++) {
                    constructorscope.Assign(c.Args[i], args[i]);
				}
				// Evaluate the body of the constructor
				c.Body.Accept(this);
				// Go back to the static scope
                CallStack.Pop();
                CallStack.Pop();
				return inst;
			}
            // Define the constructor as "" in the static scope (this is a special case
            // as it cannot be stored in the instance scope despite its being resolved 
            // at the instance level)

			var func = new UserDefinedFunction(c.Decl.Identifier, null, UnderlyingConstructor);
			staticscope.Assign(c.Decl, func);
            var funcType = c.Type;/*new FunctionType(c.Args.Select(arg =>
                (arg.Accept(this) is TypeObject to ? to.Encapsulated as ITyped :
                    throw new OutletException("expected type SHOULD NOT PRINT"), arg.Identifier)).ToArray(),
                c.Decl.Accept(this) is TypeObject tr ? tr.Encapsulated :
                    throw new OutletException("expected type SHOULD NOT PRINT"));*/
            func.RuntimeType = funcType;
            return func;
		}

		public Operand Visit(FunctionDeclaration f) {
            StackFrame closure = CurrentStackFrame;
			Operand HiddenFunc(params Operand[] args) {
                StackFrame stackFrame = new StackFrame(closure, f.LocalCount, f.Name);
                CallStack.Push(stackFrame);
				Operand returnval = null;
				for(int i = 0; i < args.Length; i++) {
                    stackFrame.Assign(f.Args[i], args[i]);
				}
				returnval = f.Body.Accept(this);
                CallStack.Pop();
				return returnval;
			}
            var funcType = new FunctionType(f.Args.Select(arg =>
                (arg.Accept(this) is TypeObject to ? to.Encapsulated as ITyped : 
                    throw new OutletException("expected type SHOULD NOT PRINT"), arg.Identifier)).ToArray(),
                f.Decl.Accept(this) is TypeObject tr ? tr.Encapsulated : 
                    throw new OutletException("expected type SHOULD NOT PRINT"));
            var func = new UserDefinedFunction(f.Decl.Identifier, funcType, HiddenFunc);
            CurrentStackFrame.Assign(f.Decl, func);
			return func;
		}

		public Operand Visit(VariableDeclaration v) {
			TypeObject type = (TypeObject) v.Decl.Accept(this);
			Operand initial = v.Initializer?.Accept(this) ?? type.Encapsulated.Default();
            CurrentStackFrame.Assign(v.Decl, initial);
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
                CurrentStackFrame.Assign(v, val);
				return val;
			} else if(a.Left is Deref d && d.Left.Accept(this) is IDereferenceable dereferenced) {
                dereferenced.SetMember(d.Referenced, val);
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
                var returned = f.Call(args);
                return returned;
            }
            else throw new RuntimeException(caller.GetOutletType().ToString() + " is not callable SHOULD NOT PRINT");
		}

		public Operand Visit(Deref d) {
			Operand left = d.Left.Accept(this);
			if(left is Array a && d.ArrayLength) return Constant.Int(a.Values().Length);
            if (left is OTuple t && int.TryParse(d.Identifier, out int idx)) return t.Values()[idx];
            if (left is IDereferenceable dereferenceable) return dereferenceable.GetMember(d.Referenced);
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
            if (v.ResolveLevel == -1)
            {
                throw new RuntimeException("could not find variable, THIS SHOULD NEVER PRINT");
            }
            return CurrentStackFrame.Get(v);
		}

		#endregion

		# region Statements

		public Operand Visit(Block b) {
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
					return ret;
				}
			}
			return null;
		}

		public Operand Visit(ForLoop f) {
			Operands.Array c = (Operands.Array) f.Collection.Accept(this);
			foreach(Operand o in c.Values()) {
				Type looptype = (f.LoopVar.Accept(this) as TypeObject).Encapsulated;
                CurrentStackFrame.Assign(f.LoopVar, o);
				Operand res = f.Body.Accept(this);
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
                    // TODO fix CurScope.Add(id, val.GetOutletType(), val);
                }
                return null;
            } 
            else throw new OutletException(u + " is not a valid using statement");
        }

		#endregion
	}
}
