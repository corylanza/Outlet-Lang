using System.Collections.Generic;
using System.Linq;
using Outlet.AST;
using Outlet.Operands;
using Outlet.Operators;
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

        public StackFrame EnterStackFrame(uint localCount, string call)
        {
            var newStackFrame = new StackFrame(CurrentStackFrame, localCount, call);
            CallStack.Push(newStackFrame);
            return newStackFrame;
        }

        private static T Cast<T>(Operand o) where T : Operand
        {
            if (o is T to) return to;
            throw new UnexpectedException($"Casting failed");
        }

        private class Empty : Operand
        {
            public static readonly Empty Value = new Empty();

            private Empty() { }

            public override bool Equals(Operand b) => ReferenceEquals(this, b);

            public override Type GetOutletType() => Primitive.Void;

            public override string ToString() => "";
        }

        #endregion

        #region Declarations

        public Operand Visit(ClassDeclaration c) {
			// Find super class, if none it will always be object
			Class? super = c.SuperClass?.Accept(this) is TypeObject t && t.Encapsulated is Class sc ? sc: Primitive.Object;

            StackFrame closure = CurrentStackFrame;

            var staticFields = new Field[c.StaticDecls.Count + c.Constructors.Count];
            StackFrame staticScope = new StackFrame(closure, (uint) staticFields.Length, $"{c.Name} static scope");

            // Hidden function for initializing instance variables/methods and defining this
            (Instance, StackFrame) Init(UserDefinedClass type)
            {
                // Enter the instance scope
                // add one to instance count for "this" which is not a member but lives in instance scope
                StackFrame instancescope = new StackFrame(staticScope, (uint) c.InstanceDecls.Count + 1, "class scope");
                CallStack.Push(instancescope);

                UserDefinedInstance instance = new UserDefinedInstance(type, instancescope);

                // Assign the value of "this"
                instancescope.LocalVariables[Class.This] = ("this", instance);

                foreach (Declaration d in c.InstanceDecls)
                {
                    instance.SetMember(d.Decl, d.Accept(this));
                }
                // TODO for super classes
                //if (super is UserDefinedClass udc) udc.Init();
                // TODO for native classes

                // Do NOT exit instance scope frame, the constructor method lives on top of it
                return (instance, instancescope);
            }


            // Define the class
            UserDefinedClass newClass = new UserDefinedClass(c.Name, super, staticScope, Init);
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
                UserDefinedClass type = c.Decl.Type is Variable v && staticscope.Get(v) is TypeObject t
                    && t.Encapsulated is UserDefinedClass udc ? udc : throw new UnexpectedException("Expected udc");

                // Call the constructors hidden init function to initialize instance variables/methods
                (Instance inst, StackFrame instancescope) = type.Initialize();

                // Enter the scope of the constructor
                if (!c.LocalCount.HasValue) throw new System.Exception("stack frame size not allocated at check time");
                StackFrame constructorscope = new StackFrame(instancescope, c.LocalCount.Value, "constructor scope");
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

			
            var funcType = new FunctionType(c.Args.Select(arg =>
                (arg.Accept(this) is TypeObject to ? to.Encapsulated :
                    throw new UnexpectedException("expected type"), arg.Identifier)).ToArray(),
                c.Decl.Accept(this) is TypeObject tr ? tr.Encapsulated :
                    throw new UnexpectedException("expected type"));
            var func = new UserDefinedFunction(c.Decl.Identifier, funcType, UnderlyingConstructor);
            staticscope.Assign(c.Decl, func);
            return func;
		}

		public Operand Visit(FunctionDeclaration f) {
            StackFrame closure = CurrentStackFrame;
			Operand HiddenFunc(params Operand[] args) {
                if (!f.LocalCount.HasValue) throw new UnexpectedException("stack frame size not allocated at check time");
                StackFrame stackFrame = new StackFrame(closure, f.LocalCount.Value, f.Name);
                CallStack.Push(stackFrame);
				for(int i = 0; i < args.Length; i++) {
                    stackFrame.Assign(f.Args[i], args[i]);
				}
                Operand returnval = f.Body.Accept(this);
                CallStack.Pop();
				return returnval;
			}
            var funcType = new FunctionType(f.Args.Select(arg =>
                (arg.Accept(this) is TypeObject to ? to.Encapsulated : 
                    throw new UnexpectedException("expected type"), arg.Identifier)).ToArray(),
                f.Decl.Accept(this) is TypeObject tr ? tr.Encapsulated : 
                    throw new UnexpectedException("expected type"));
            var func = new UserDefinedFunction(f.Decl.Identifier, funcType, HiddenFunc);
            CurrentStackFrame.Assign(f.Decl, func);
			return func;
		}

        public Operand Visit(OperatorOverloadDeclaration o)
        {
            return Visit(o as FunctionDeclaration);
        }

        public Operand Visit(VariableDeclaration v) {
			Operand initial = v.Initializer?.Accept(this) ?? ((TypeObject) v.Decl.Accept(this)).Encapsulated.Default();
            CurrentStackFrame.Assign(v.Decl, initial);
			return initial;
		}

		#endregion

		#region Expressions

		public Operand Visit(Declarator d) {
			Operand? t = d.Type?.Accept(this);
			if(t is TypeObject type) return type;
			else throw new UnexpectedException(d.Type?.ToString() ?? "invalid type" + " is not a valid type");
		}

		public Operand Visit<E>(Literal<E> c) where E : struct => new Value<E>(c.Type, c.Value);

        public Operand Visit(StringLiteral s) => new String(s.Value);

        public Operand Visit(NullExpr n) => Value.Null;

        public Operand Visit(Access a) {
			Operand col = a.Collection.Accept(this);
			if(col is TypeObject at && a.Index.Length == 0) return new TypeObject(new ArrayType(at.Encapsulated));
            if(col is Array c)
            {
                // Index is 0 because all array access is one-dimensional as of now
                // multi-dimensional arrays can be accessed through chained accesses
                int idx = Cast<Value<int>>(a.Index[0].Accept(this)).Underlying;
                int len = c.Values().Length;
                if (idx >= len) throw new RuntimeException("array index out of bounds exception: index was " + idx + " array only goes to " + (len - 1));
                return c.Values()[idx];
            }
            throw new UnexpectedException("cannot acccess this type");
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
			} else if(a.Left is MemberAccess m && m.Left.Accept(this) is IDereferenceable dereferenced) {
                dereferenced.SetMember(m.Member, val);
                return val;
			}
			throw new UnexpectedException("cannot assign to the left side of this expression");
		}

		public Operand Visit(Binary b) => b.Oper is BinOp bo ? bo.Perform(b.Left.Accept(this), b.Right.Accept(this)) : 
            throw new UnexpectedException("Operator never resolved");

		public Operand Visit(Call c) {
			Operand caller = c.Caller.Accept(this);
			var args = c.Args.Select(arg => arg.Accept(this)).ToArray();
            if (caller is ICallable f)
            {
                // TODO don't call left.accept
                if(c.Caller is MemberAccess m && m.Left.Accept(this) is Instance i)
                {
                    return f.Call(i, args);
                } else
                {
                    return f.Call(null, args);
                }
            }
            else throw new UnexpectedException(caller.GetOutletType().ToString() + " is not callable");
		}

        public Operand Visit(TupleAccess t)
        {
            Operand left = t.Left.Accept(this);
            if (left is OTuple tup) return tup.Values()[t.Member];
            throw new UnexpectedException("Illegal dereference");
        }

		public Operand Visit(MemberAccess m) {
			Operand left = m.Left.Accept(this);
			if(left is Array a && m.ArrayLength) return Value.Int(a.Values().Length);
            if (left is TypeObject to && to.Encapsulated is IDereferenceable statics) return statics.GetMember(m.Member); 
            if (left is IDereferenceable instances) return instances.GetMember(m.Member);
			if(left == Value.Null) throw new RuntimeException("null pointer exception");
			throw new UnexpectedException("Illegal dereference");
		}

		public Operand Visit(Is i) {
			bool val = i.Left.Accept(this).GetOutletType().Is(((TypeObject) i.Right.Accept(this)).Encapsulated);
			return Value.Bool(i.NotIsnt ? val : !val); 
		}

		public Operand Visit(Lambda l) {
			Operand left = l.Left.Accept(this);
			Operand right = l.Right.Accept(this);
			if(left is TypeObject lt && lt.Encapsulated is TupleType tt && right is TypeObject rt)
				return new TypeObject(new FunctionType(tt.Types.Select(x => (x, "")).ToArray(), rt.Encapsulated));
			throw new UnexpectedException("lambda invalid");
		}

		public Operand Visit(ListLiteral l) => new Operands.Array(l.Args.Select(arg => arg.Accept(this)).ToArray());

		public Operand Visit(ShortCircuit s) {
            bool left = Cast<Value<bool>>(s.Left.Accept(this)).Underlying;
			if(s.isand == left) {
                bool right = Cast<Value<bool>>(s.Right.Accept(this)).Underlying;
                return Value.Bool(right);
			} else return Value.Bool(!s.isand);
		}

		public Operand Visit(Ternary t) {
			if(t.Condition.Accept(this) is Value<bool> b) {
				return b.Underlying ? t.IfTrue.Accept(this) : t.IfFalse.Accept(this);
			}
			throw new UnexpectedException("expected boolean in ternary condition");
		}

		public Operand Visit(TupleLiteral t) {
			IEnumerable<Operand> evaled = t.Args.Select(arg => arg.Accept(this));
			if(evaled.All(elem => elem is TypeObject)) 
                return new TypeObject(new TupleType(evaled.Select(elem => Cast<TypeObject>(elem).Encapsulated).ToArray()));
			if(t.Args.Length == 1) return t.Args[0].Accept(this);
			else return new OTuple(evaled.ToArray());
		}

		public Operand Visit(Unary u) => u.Oper is UnOp uo ? uo.Perform(u.Expr.Accept(this)) : 
            throw new UnexpectedException("Unary operator was never resolved");

		public Operand Visit(Variable v) {
            if (!v.ResolveLevel.HasValue)
            {
                throw new UnexpectedException($"could not find variable {v}");
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
			return Empty.Value;
		}

		public Operand Visit(ForLoop f) {
			Operands.Array c = (Operands.Array) f.Collection.Accept(this);
			foreach(Operand o in c.Values()) {
				Cast<TypeObject>(f.LoopVar.Accept(this));
                CurrentStackFrame.Assign(f.LoopVar, o);
				Operand res = f.Body.Accept(this);
				if(f.Body is Statement s && !(s is Expression) && res != null) {
					return res;
				}
			}
			return Empty.Value;
		}

		public Operand Visit(IfStatement i) {
			if(i.Condition.Accept(this) is Value<bool> b && b.Underlying) {
				var ret = i.Iftrue.Accept(this);
				if(i.Iftrue is Statement s && !(s is Expression)) return ret;
			} else {
				var ret = i.Iffalse?.Accept(this);
                if (ret is null) return Empty.Value;
				if(i.Iffalse != null && i.Iffalse is Statement s && !(s is Expression)) return ret;
			}
			return Empty.Value;
		}

		public Operand Visit(ReturnStatement r) {
			return r.Expr.Accept(this);
		}

		public Operand Visit(WhileLoop w) {
			while(w.Condition.Accept(this) is Value<bool> b && b.Underlying) {
				var ret = w.Body.Accept(this);
                if (w.Body is Statement s && !(s is Expression) && ret != null) return ret;
            }
			return Empty.Value;
		}

        public Operand Visit(UsingStatement u)
        {
            Operand toUse = u.Used.Accept(this);
            if (toUse is TypeObject rc && rc.Encapsulated is IDereferenceable d) 
            {
                foreach(var (id, val) in d.GetMembers())
                {
                    // TODO fix
                    CurrentStackFrame.Assign(new Variable(id), val);
                }
                return Empty.Value;
            } 
            else throw new UnexpectedException(u + " is not a valid using statement");
        }

		#endregion
	}
}
