using System.Collections.Generic;
using System.Linq;
using Outlet.AST;
using Outlet.Operands;
using Outlet.Types;
using Type = Outlet.Types.Type;

namespace Outlet.Checking
{
    public class Checker : IVisitor<ITyped>
    {

        #region Helpers

        public readonly Stack<bool> DoImpl = new Stack<bool>();
        public readonly Stack<CheckStackFrame> StackFrames = new Stack<CheckStackFrame>();
        public CheckStackFrame CurrentStackFrame => StackFrames.Peek();
        public class Error : Type
        {
            public readonly string Message;

            public Error(string message)
            {
                ErrorCount++;
                if (message != "") Program.ThrowException(message);
                Message = message;
            }

            public override bool Is(ITyped t, out int level) {
                level = -1;
                return false;
            }

            public override string ToString() => "error";
        }
        //private static readonly Type ErrorType = new ProtoClass("error", null, null, null);
        public static int ErrorCount = 0;

        public Checker()
        {
            StackFrames.Push(CheckStackFrame.Global);
        }

        private void Define(ITyped type, IBindable decl) => CurrentStackFrame.Assign(decl, type);

        public void Check(IASTNode program)
        {
            ErrorCount = 0;
            if (program is FunctionDeclaration || program is ClassDeclaration)
            {
                DoImpl.Push(false);
                program.Accept(this);
                DoImpl.Pop();
                DoImpl.Push(true);
                program.Accept(this);
                DoImpl.Pop();
            }
            else program.Accept(this);
            if (ErrorCount > 0) throw new CheckerException(ErrorCount + " Checking errors encountered");
        }

        public void EnterScope() => CurrentStackFrame.EnterScope();
        public void ExitScope() => CurrentStackFrame.ExitScope();

        public CheckStackFrame EnterStackFrame(CheckStackFrame? toPush = null) {
            if (toPush is null)
            {
                var newFrame = new CheckStackFrame(CurrentStackFrame);
                StackFrames.Push(newFrame);
                return newFrame;
            }
            StackFrames.Push(toPush);
            return toPush;

        }

        public void ExitStackFrame() => StackFrames.Pop();

        //public static Type Error(string message)
        //{
        //    ErrorCount++;
        //    if (message != "") Program.ThrowException(message);
        //    return ErrorType;
        //}

        public ITyped Cast(ITyped from, ITyped to, string message = "cannot convert type {0} to type {1}")
        {
            if (from is Error) return from;
            if (to is Error) return to;
            if (!from.Is(to)) return new Error(string.Format(message, from, to));
            return to;
        }

        #endregion

        #region Declarations

        public ITyped Visit(ClassDeclaration c)
        {
            if (!DoImpl.Peek())
            {
                CheckStackFrame statics = new CheckStackFrame(CurrentStackFrame);
                CheckStackFrame instances = new CheckStackFrame(statics);
                Class parent = Primitive.Object;
                if (c.SuperClass != null)
                {
                    if (c.SuperClass.Accept(this) is TypeObject to && to.Encapsulated is Class super)
                    {
                        parent = super;
                    } else new Error("cannot extend anything other than a class");
                }

                ProtoClass proto = new ProtoClass(c.Name, parent, statics, instances);
                Define(new TypeObject(proto), c.Decl);
                EnterStackFrame(statics);
                //foreach (var (id, classConstraint) in c.GenericParameters)
                //{
                //    Class constraint = classConstraint?.Accept(this) is TypeObject to && to.Encapsulated is Class co ? co : Primitive.Object;
                //    CurrentScope.Define(new TypeObject(constraint), id);
                //}

                foreach (Declaration d in c.StaticDecls) if (d is FunctionDeclaration) d.Accept(this);
                foreach (var constructor in c.Constructors) constructor.Accept(this);

                EnterStackFrame(instances);
                Define(proto, "this".ToVariable());
                foreach (Declaration d in c.InstanceDecls) if (d is FunctionDeclaration) d.Accept(this);

                ExitStackFrame();
                ExitStackFrame();
            }
            else
            {
                Class? parent;
                if (c.SuperClass != null) parent = CurrentStackFrame.Get(c.SuperClass) as Class;
                var proto = CurrentStackFrame.Get(c.Decl) is TypeObject t && t.Encapsulated is ProtoClass p ? p : throw new CheckerException("Expected protoclass");
                EnterStackFrame(proto.StaticMembers);
                foreach (Declaration d in c.StaticDecls) d.Accept(this);

                EnterStackFrame(proto.InstanceMembers);
                foreach (Declaration d in c.InstanceDecls) d.Accept(this);

                //if (parent != null) foreach ((string id, Type type) in parent.InstanceMembers) CurrentScope.Define(type, id);
                foreach (var constructor in c.Constructors) constructor.Accept(this);
                ExitStackFrame();
                ExitStackFrame();
            }
            return Primitive.Void;
        }

        public ITyped Visit(ConstructorDeclaration c) => Visit(c as FunctionDeclaration);

        public ITyped Visit(FunctionDeclaration f)
        {
            if (!DoImpl.Peek())
            {
                // Check decl and args first, needed to make function type
                (ITyped type, string id)[] args = f.Args.Select(arg =>
                {
                    ITyped curArg = arg.Accept(this);
                    return (curArg, arg.Identifier);
                }).ToArray();
                Type returnType = f.Decl.Accept(this) is Type t ? t : throw new CheckerException("Expected Type");
                FunctionType ft = new FunctionType(args, returnType);
                // define the header using the function type from above
                Define(ft, f.Decl);
                return ft;
            }
            else
            {
                (ITyped type, _, _) = CurrentStackFrame.Resolve(f.Decl);
                FunctionType ft = (FunctionType)type;
                // enter the function scope and define the args;
                EnterStackFrame();
                ft.Args.Zip(f.Args).ToList().ForEach(arg => Define(arg.First.type, arg.Second));
                // check the body now that its header and args have been defined
                ITyped body = f.Body.Accept(this);
                if (f is ConstructorDeclaration)
                {
                    if (body != null) return new Error("constructor cannot return value");
                }
                else if (body == null || body == Primitive.Void)
                {
                    if (ft.ReturnType != Primitive.Void) return new Error("function " + f.Decl.Identifier + "not all code paths return a value");
                }
                else Cast(body, ft.ReturnType, f.Decl.Identifier + " function definition invalid, expected {1}, returned {0}");
                f.LocalCount = CurrentStackFrame.Count;
                ExitStackFrame();
                return ft;
            }
        }

        public ITyped Visit(VariableDeclaration v)
        {
            ITyped decl = v.Decl.Accept(this);
            ITyped? init = v.Initializer?.Accept(this);
            if (init != null) Cast(init, decl);
            if (init is TypeObject meta) Define(meta, v.Decl);
            else Define(decl, v.Decl);
            return Primitive.Void;
        }

        #endregion

        #region Expressions

        public ITyped Visit(Access a)
        {
            ITyped elem = a.Collection.Accept(this);
            ITyped? idxType = a.Index.Length > 0 ? a.Index[0].Accept(this) : null;
            if (elem is TypeObject meta && meta.Encapsulated is Class c)
            {
                if (idxType is TypeObject)
                    return new Error("Generics not supported yet");
                // array types are defined with empty braces []
                if (idxType == null)
                    return new TypeObject(new ArrayType(meta.Encapsulated));
            }
            if (a.Index.Length != 1)
                return new Error("array access requires exactly 1 index");
            if (idxType != Primitive.Int)
                return new Error("only ints can be used to index into an array, found: " + idxType?.ToString());
            if (elem is ArrayType at) return at.ElementType;
            return new Error("type " + elem.ToString() + " is not accessable by array access operator []");
        }

        public ITyped Visit(As a)
        {
            a.Left.Accept(this);
            ITyped r = a.Right.Accept(this);
            if (r is TypeObject castedTo) return castedTo.Encapsulated;
            return new Error("the right side of an is expression must be a type, found: " + r.ToString());
        }

        public ITyped Visit(Assign a) => a.Left switch
        {
            MemberAccess d when d.ArrayLength => new Error("cannot assign to an array length"),
            Expression left when left is Variable || left is MemberAccess =>
                (a.Left.Accept(this), a.Right.Accept(this)) switch
                {
                    (Error e, _) => e,
                    (_, Error e) => e,
                    (ITyped l, ITyped r) => Cast(r, l)
                },
            _ => new Error("illegal assignment, can only assign to variables and fields")
        };

        public ITyped Visit(Binary b) => (b.Left.Accept(this), b.Right.Accept(this)) switch
        {
            (Error e, _) => e,
            (_, Error e) => e,
            (ITyped left, ITyped right) => 
                b.Overloads.FindBestMatch(left, right) switch
                {
                    null => new Error("binary operator not defined for " + left.ToString() + " " + b.Op + " " + right.ToString()),
                    BinOp op => (b.Oper = op).GetResultType()
                }
        };

        public ITyped Visit(Call c)
        {
            ITyped calltype = c.Caller.Accept(this);
            if (calltype is Error) return calltype;
            if (calltype is TypeObject t && t.Encapsulated is ProtoClass proto)
            {
                // Rather than have to store the overload id within the call it is easier to turn a constructor call into
                // a dereference to static field "" where the constructor lives
                c.MakeConstructorCall();
                return Visit(c);
            }

            ITyped[] argtypes = c.Args.Select(x => x.Accept(this)).ToArray();
            if (calltype is FunctionType functype)
            {
                var funcargs = functype.Args.ToArray();
                bool argsMatch = funcargs.SameLengthAndAll(argtypes, (arg, argType) => !(Cast(argType, arg.type, "Could not cast {0} to {1}") is Error));
                return argsMatch ? functype.ReturnType :
                    new Error(c.Caller + " expects " + "(" + funcargs.Select(x => x.type).ToList().ToListString() + ") found: (" + argtypes.ToList().ToListString() + ")");
            }
            if (calltype is MethodGroupType mgt)
            {
                (FunctionType? bestMatch, int id) = mgt.FindBestMatch(argtypes);
                if(c.Caller is Variable v) v.Bind(id, v.LocalId);
                if(c.Caller is MemberAccess ma) ma.Member.Bind(id, ma.Member.LocalId);
                if (bestMatch is null) return new Error("No overload could be found for (" + argtypes.ToList().ToListString() + ")");
                return bestMatch.ReturnType;
            }
            return new Error("type " + calltype + " is not callable");
        }

        public ITyped Visit(Declarator d) => d.Type.Accept(this) switch
        {
            TypeObject meta => meta.Encapsulated,
            Primitive t when t == Primitive.MetaType => new Error("Declared type must be a check time constant"),
            ITyped invalid => new Error($"Declaration requires valid type, found: {invalid}")
        };

        public ITyped Visit(TupleAccess t) => t.Left.Accept(this) switch
        {
            Error e => e,
            TupleType tt when tt.Types.Length > t.Member => tt.Types[t.Member],
            TupleType tt => new Error("cannot access element " + t.Member + " of tuple type " +
                        tt.ToString() + " which has only " + tt.Types.Length + " elements"),
            ITyped type => new Error($"Cannot reference member {t.Member} of non tuple type {type}")
        };


        public ITyped Visit(MemberAccess d)
        {
            ITyped inst = d.Left.Accept(this);
            if (inst is Error) return inst;
            if (inst is ArrayType && d.Member.Identifier == "length")
            {
                d.ArrayLength = true;
                return Primitive.Int;
            }
            if (inst is ProtoClass instances) 
                return instances.GetInstanceMemberType(d.Member);
            if (inst is TypeObject t && t.Encapsulated is ProtoClass statics) 
                return statics.GetStaticMemberType(d.Member);

            return new Error("cannot dereference type: " + inst.ToString());
        }

        public ITyped Visit(Is i)
        {
            i.Left.Accept(this);
            ITyped r = i.Right.Accept(this);
            if (r is TypeObject || r == Primitive.MetaType) return Primitive.Bool;
            return new Error("the right side of an is expression must be a type, found: " + r.ToString());
        }

        public ITyped Visit(Lambda l) => (l.Left.Accept(this), l.Right.Accept(this)) switch
        {
            (TypeObject args, TypeObject result) => new Error("NOT IMPLEMENTED"),  //new TypeObject(new FunctionType())
            _ => new Error("Lambdas currently old work for types")
        };

        public ITyped Visit(ListLiteral l)
        {
            return new ArrayType(Type.CommonAncestor(l.Args.Select(x => x.Accept(this)).ToArray()));
        }

        public ITyped Visit<E>(Literal<E> c) => c.Type;

        public ITyped Visit(ShortCircuit s) => (s.Left.Accept(this), s.Right.Accept(this)) switch
        {
            (Error e, _) => e,
            (_, Error e) => e,
            (ITyped left, ITyped right) => Cast(right, Cast(left, Primitive.Bool))
        };

        public ITyped Visit(Ternary t) => (t.Condition.Accept(this), t.IfTrue.Accept(this), t.IfFalse.Accept(this)) switch
        {
            (Primitive condition, _, _) when condition != Primitive.Bool => 
                new Error($"Ternary condition requires a boolean, found a {condition}"),
            (_, Error e, _) => e,
            (_, _, Error e) => e,
            (_, ITyped iftrue, ITyped iffalse) => Type.CommonAncestor(iftrue, iffalse)
        };

        public ITyped Visit(TupleLiteral t)
        {
            if (t.Args.Length == 1) return t.Args[0].Accept(this);
            var types = t.Args.Select(arg => arg.Accept(this)).ToArray();
            if (types.All(type => type is TypeObject)) return new TypeObject(new TupleType(types));
            else return new TupleType(types);
        }

        public ITyped Visit(Unary u)
        {
            ITyped input = u.Expr.Accept(this);
            if (input is Error) return input;
            var op = u.Overloads.FindBestMatch(input);
            if (op == null) return new Error("unary operator " + u.Op + " is not defined for type " + input.ToString());
            u.Oper = op;
            return op.GetResultType();
        }

        public ITyped Visit(Variable v)
        {
            (ITyped type, int level, int id) = CurrentStackFrame.Resolve(v);
            if (level == -1) return new Error("variable " + v.Identifier + " could not be resolved");
            v.Bind(id, level);
            return type;
        }

        #endregion

        #region Statements

        public ITyped Visit(Block b)
        {
            EnterScope();
            DoImpl.Push(false);
            // Forward Declaration of Classes
            foreach (ClassDeclaration cd in b.Classes)
            {
                cd.Accept(this);
            }
            // Forward Declaration of Functions
            foreach (FunctionDeclaration fd in b.Functions)
            {
                fd.Accept(this);
            }
            DoImpl.Pop();
            DoImpl.Push(true);
            ITyped? ret = null;
            foreach (IASTNode d in b.Lines)
            {
                ITyped temp = d.Accept(this);
                if (ret != null) return new Error("unreachable code detected");
                if (d is Statement && !(d is Expression) && temp != null)
                {
                    ret = temp;
                }
            }
            DoImpl.Pop();
            ExitScope();
            return ret is null ? Primitive.Void : ret;
        }

        public ITyped Visit(ForLoop f)
        {
            ITyped collection = f.Collection.Accept(this);
            if (collection is ArrayType at)
            {
                EnterScope();
                ITyped loopvar = f.LoopVar.Accept(this);
                Cast(at.ElementType, loopvar);
                Define(loopvar, f.LoopVar);
                ITyped body = f.Body.Accept(this);
                ExitScope();
                if (f.Body is Statement && !(f.Body is Expression) && body != null) return body;
                return Primitive.Void;
            }
            return new Error("only array types are iterable, found:" + collection.ToString());
        }

        public ITyped Visit(IfStatement i)
        {
            ITyped cond = i.Condition.Accept(this);
            Cast(cond, Primitive.Bool, "if statement condition requires a boolean, found a {0}");
            ITyped iftrue = i.Iftrue.Accept(this);
            ITyped? iffalse = i.Iffalse?.Accept(this);
            if (i.Iftrue is Statement && !(i.Iftrue is Expression) && iftrue != null)
            {
                if (i.Iffalse is Statement && !(i.Iffalse is Expression) && iffalse != null)
                {
                    // TODO should return common ancestor
                    return iftrue;
                }
            }
            return Primitive.Void;
        }

        public ITyped Visit(ReturnStatement r)
        {
            return r.Expr.Accept(this);
        }

        public ITyped Visit(WhileLoop w)
        {
            ITyped cond = w.Condition.Accept(this);
            Cast(cond, Primitive.Bool, "while loop condition requires a boolean, found a {0}");
            ITyped body = w.Body.Accept(this);
            if (w.Body is Statement && !(w.Body is Expression) && body != null) return body;
            return Primitive.Void;
        }

        public ITyped Visit(UsingStatement u)
        {
            ITyped used = u.Used.Accept(this);
            if (used is TypeObject t && t.Encapsulated is ProtoClass staticClass)
                foreach (var (id, type) in staticClass.GetStaticMemberTypes())
                {
                    // TODO restore functionality
                    //CurrentScope.Define(type, id);
                }
            {
                return Primitive.Void;
            }
            throw new CheckerException("Only classes can be used, found " + used);
        }

        #endregion
    }
}
