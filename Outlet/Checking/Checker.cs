﻿using System.Collections.Generic;
using System.Linq;
using Outlet.AST;
using Outlet.Operands;
using Outlet.Types;
using Type = Outlet.Types.Type;

namespace Outlet.Checking
{
    public class Checker : IVisitor<Type>
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

            public override bool Is(Type t, out uint level) {
                level = 0;
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

        private void Define(Type type, IBindable decl) => CurrentStackFrame.Assign(decl, type);

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

        public Type Cast(Type from, Type to, string message = "cannot convert type {0} to type {1}")
        {
            if (from is Error) return from;
            if (to is Error) return to;
            if (!from.Is(to)) return new Error(string.Format(message, from, to));
            return to;
        }

        #endregion

        #region Declarations

        public Type Visit(ClassDeclaration c)
        {
            if (!DoImpl.Peek())
            {
                CheckStackFrame statics = new CheckStackFrame(CurrentStackFrame);
                CheckStackFrame instances = new CheckStackFrame(statics);
                Class parent = Primitive.Object;
                if (c.SuperClass != null)
                {
                    if (c.SuperClass.Accept(this) is MetaType to && to.Stored is Class super)
                    {
                        parent = super;
                    } else new Error("cannot extend anything other than a class");
                }

                ProtoClass proto = new ProtoClass(c.Name, parent, statics, instances);
                Define(new MetaType(proto), c.Decl);
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
                var proto = CurrentStackFrame.Get(c.Decl) is MetaType t && t.Stored is ProtoClass p ? p : throw new CheckerException("Expected protoclass");
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

        public Type Visit(ConstructorDeclaration c) => Visit(c as FunctionDeclaration);

        public Type Visit(FunctionDeclaration f)
        {
            if (!DoImpl.Peek())
            {
                // Check decl and args first, needed to make function type
                (Type type, string id)[] args = f.Args.Select(arg =>
                {
                    Type curArg = arg.Accept(this);
                    return (curArg, arg.Identifier);
                }).ToArray();
                Type returnType = f.Decl.Accept(this) is Type t ? t : throw new CheckerException("Expected Type");
                FunctionType ft = new FunctionType(args, returnType);
                // define the header using the function type from above
                Define(ft, f.Decl);
                return ft;
            }
            else if (CurrentStackFrame.Resolve(f.Decl, out Type type, out uint _, out uint _))
            {
                FunctionType ft = type is FunctionType fnt ? fnt : throw new CheckerException("Expected Function type");
                // enter the function scope and define the args;
                EnterStackFrame();
                ft.Args.Zip(f.Args).ToList().ForEach(arg => Define(arg.First.type, arg.Second));
                // check the body now that its header and args have been defined
                Type body = f.Body.Accept(this);
                if (f is ConstructorDeclaration)
                {
                    if (body != Primitive.Void) return new Error("constructor cannot return value");
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
            else throw new UnexpectedException("Could not resolve function type");
        }

        public Type Visit(VariableDeclaration v)
        {
            Type decl = v.Decl.Accept(this);
            Type? init = v.Initializer?.Accept(this);
            if (init != null) Cast(init, decl);
            if (init is MetaType meta) Define(meta, v.Decl);
            else Define(decl, v.Decl);
            return Primitive.Void;
        }

        #endregion

        #region Expressions

        public Type Visit(Access a)
        {
            Type elem = a.Collection.Accept(this);
            Type? idxType = a.Index.Length > 0 ? a.Index[0].Accept(this) : null;
            if (elem is MetaType meta && meta.Stored is Class c)
            {
                if (idxType is MetaType)
                    return new Error("Generics not supported yet");
                // array types are defined with empty braces []
                if (idxType == null)
                    return new MetaType(new ArrayType(meta.Stored));
            }
            if (a.Index.Length != 1)
                return new Error("array access requires exactly 1 index");
            if (idxType != Primitive.Int)
                return new Error("only ints can be used to index into an array, found: " + idxType?.ToString());
            if (elem is ArrayType at) return at.ElementType;
            return new Error("type " + elem.ToString() + " is not accessable by array access operator []");
        }

        public Type Visit(As a)
        {
            a.Left.Accept(this);
            Type r = a.Right.Accept(this);
            if (r is MetaType castedTo) return castedTo.Stored;
            return new Error("the right side of an is expression must be a type, found: " + r.ToString());
        }

        public Type Visit(Assign a) => a.Left switch
        {
            MemberAccess d when d.ArrayLength => new Error("cannot assign to an array length"),
            Expression left when left is Variable || left is MemberAccess =>
                (a.Left.Accept(this), a.Right.Accept(this)) switch
                {
                    (Error e, _) => e,
                    (_, Error e) => e,
                    (Type l, Type r) => Cast(r, l)
                },
            _ => new Error("illegal assignment, can only assign to variables and fields")
        };

        public Type Visit(Binary b) => (b.Left.Accept(this), b.Right.Accept(this)) switch
        {
            (Error e, _) => e,
            (_, Error e) => e,
            (Type left, Type right) => 
                b.Overloads.FindBestMatch(left, right) switch
                {
                    null => new Error("binary operator not defined for " + left.ToString() + " " + b.Op + " " + right.ToString()),
                    BinOp op => (b.Oper = op).GetResultType()
                }
        };

        public Type Visit(Call c)
        {
            Type calltype = c.Caller.Accept(this);
            if (calltype is Error) return calltype;
            if (calltype is MetaType t && t.Stored is ProtoClass proto)
            {
                // Rather than have to store the overload id within the call it is easier to turn a constructor call into
                // a dereference to static field "" where the constructor lives
                c.MakeConstructorCall();
                return Visit(c);
            }

            Type[] argtypes = c.Args.Select(x => x.Accept(this)).ToArray();
            if (calltype is FunctionType functype)
            {
                var funcargs = functype.Args.ToArray();
                bool argsMatch = funcargs.SameLengthAndAll(argtypes, (arg, argType) => !(Cast(argType, arg.type, "Could not cast {0} to {1}") is Error));
                return argsMatch ? functype.ReturnType :
                    new Error(c.Caller + " expects " + "(" + funcargs.Select(x => x.type).ToList().ToListString() + ") found: (" + argtypes.ToList().ToListString() + ")");
            }
            if (calltype is MethodGroupType mgt)
            {
                (FunctionType? bestMatch, uint? id) = mgt.FindBestMatch(argtypes);
                if(bestMatch is null || id is null) return new Error("No overload could be found for (" + argtypes.ToList().ToListString() + ")");
                if (c.Caller is Variable v && v.ResolveLevel.HasValue)
                {
                    v.Bind(id.Value, v.ResolveLevel.Value);
                }
                else if (c.Caller is MemberAccess ma && ma.Member.ResolveLevel.HasValue)
                {
                    ma.Member.Bind(id.Value, ma.Member.ResolveLevel.Value);
                }
                else throw new UnexpectedException("Caller is not able to be overloaded");
                return bestMatch.ReturnType;
            }
            return new Error("type " + calltype + " is not callable");
        }

        public Type Visit(Declarator d) => d.Type.Accept(this) switch
        {
            MetaType meta => meta.Stored,
            Primitive t when t == Primitive.MetaType => new Error("Declared type must be a check time constant"),
            Type invalid => new Error($"Declaration requires valid type, found: {invalid}")
        };

        public Type Visit(TupleAccess t) => t.Left.Accept(this) switch
        {
            Error e => e,
            TupleType tt when tt.Types.Length > t.Member => tt.Types[t.Member],
            TupleType tt => new Error($"cannot access element {t.Member} of tuple type {tt} which has {tt.Types.Length} elements"),
            Type type => new Error($"Cannot reference member {t.Member} of non tuple type {type}")
        };

        public Type Visit(MemberAccess d) => d.Left.Accept(this) switch
        {
            Error e => e,
            ArrayType _ when d.ArrayLengthAccess() => Primitive.Int,
            ProtoClass instances => instances.GetInstanceMemberType(d.Member),
            MetaType t when t.Stored is ProtoClass statics => statics.GetStaticMemberType(d.Member),
            Type other => new Error($"cannot dereference type: {other}")
        };

        public Type Visit(Is i)
        {
            i.Left.Accept(this);
            Type r = i.Right.Accept(this);
            if (r is MetaType || r == Primitive.MetaType) return Primitive.Bool;
            return new Error("the right side of an is expression must be a type, found: " + r.ToString());
        }

        public Type Visit(Lambda l) => (l.Left.Accept(this), l.Right.Accept(this)) switch
        {
            (MetaType args, MetaType result) => new Error("NOT IMPLEMENTED"),  //new TypeObject(new FunctionType())
            _ => new Error("Lambdas currently old work for types")
        };

        public Type Visit(ListLiteral l)
        {
            return new ArrayType(Type.CommonAncestor(l.Args.Select(x => x.Accept(this)).ToArray()));
        }

        public Type Visit<E>(Literal<E> c) => c.Type;

        public Type Visit(ShortCircuit s) => (s.Left.Accept(this), s.Right.Accept(this)) switch
        {
            (Error e, _) => e,
            (_, Error e) => e,
            (Type left, Type right) => Cast(right, Cast(left, Primitive.Bool))
        };

        public Type Visit(Ternary t) => (t.Condition.Accept(this), t.IfTrue.Accept(this), t.IfFalse.Accept(this)) switch
        {
            (Primitive condition, _, _) when condition != Primitive.Bool => 
                new Error($"Ternary condition requires a boolean, found a {condition}"),
            (_, Error e, _) => e,
            (_, _, Error e) => e,
            (_, Type iftrue, Type iffalse) => Type.CommonAncestor(iftrue, iffalse)
        };

        public Type Visit(TupleLiteral t)
        {
            if (t.Args.Length == 1) return t.Args[0].Accept(this);
            var types = t.Args.Select(arg => arg.Accept(this)).ToArray();
            if (types.All(type => type is MetaType)) return new MetaType(new TupleType(types));
            else return new TupleType(types);
        }

        public Type Visit(Unary u)
        {
            Type input = u.Expr.Accept(this);
            if (input is Error) return input;
            var op = u.Overloads.FindBestMatch(input);
            if (op == null) return new Error("unary operator " + u.Op + " is not defined for type " + input.ToString());
            u.Oper = op;
            return op.GetResultType();
        }

        public Type Visit(Variable v)
        {
            if(CurrentStackFrame.Resolve(v, out Type type, out uint level, out uint id))
            {
                v.Bind(id, level);
                return type;
            }
            return new Error("variable " + v.Identifier + " could not be resolved");
        }

        #endregion

        #region Statements

        public Type Visit(Block b)
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
            Type? ret = null;
            foreach (IASTNode d in b.Lines)
            {
                Type temp = d.Accept(this);
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

        public Type Visit(ForLoop f)
        {
            Type collection = f.Collection.Accept(this);
            if (collection is ArrayType at)
            {
                EnterScope();
                Type loopvar = f.LoopVar.Accept(this);
                Cast(at.ElementType, loopvar);
                Define(loopvar, f.LoopVar);
                Type body = f.Body.Accept(this);
                ExitScope();
                if (f.Body is Statement && !(f.Body is Expression) && body != null) return body;
                return Primitive.Void;
            }
            return new Error("only array types are iterable, found:" + collection.ToString());
        }

        public Type Visit(IfStatement i)
        {
            Type cond = i.Condition.Accept(this);
            Cast(cond, Primitive.Bool, "if statement condition requires a boolean, found a {0}");
            Type iftrue = i.Iftrue.Accept(this);
            Type? iffalse = i.Iffalse?.Accept(this);
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

        public Type Visit(ReturnStatement r)
        {
            return r.Expr.Accept(this);
        }

        public Type Visit(WhileLoop w)
        {
            Type cond = w.Condition.Accept(this);
            Cast(cond, Primitive.Bool, "while loop condition requires a boolean, found a {0}");
            Type body = w.Body.Accept(this);
            if (w.Body is Statement && !(w.Body is Expression) && body != null) return body;
            return Primitive.Void;
        }

        public Type Visit(UsingStatement u)
        {
            Type used = u.Used.Accept(this);
            if (used is MetaType t && t.Stored is ProtoClass staticClass)
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
