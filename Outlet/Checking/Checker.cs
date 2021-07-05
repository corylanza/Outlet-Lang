using System;
using System.Collections.Generic;
using System.Linq;
using Outlet.AST;
using Outlet.Operands;
using Outlet.Operators;
using Outlet.Tokens;
using Outlet.Types;
using Type = Outlet.Types.Type;

namespace Outlet.Checking
{
    public class Checker : IASTVisitor<Type>
    {

        #region Helpers

        //public readonly Stack<bool> DoImpl = new Stack<bool>();
        public readonly Stack<CheckStackFrame> StackFrames = new Stack<CheckStackFrame>();
        public CheckStackFrame CurrentStackFrame => StackFrames.Peek();

        private readonly List<Error> CheckingErrors = new List<Error>();

        public Checker()
        {
            StackFrames.Push(CheckStackFrame.Global(Error));
        }

        private void ErrorHandler(Error error)
        {
            CheckingErrors.Add(error);
        }

        public CheckStackFrame GlobalScope => StackFrames.Last();

        public Error Error(string message) => new Error(message, ErrorHandler);

        private void Define(Type type, IBindable decl) => CurrentStackFrame.Assign(decl, type);

        public void Check(IASTNode program)
        {
            CheckingErrors.Clear();
            program.Accept(this);

            if (CheckingErrors.Count > 0)
            {
                var errorMessage = $"{CheckingErrors.Count} Checking errors encountered\n";
                foreach(var error in CheckingErrors)
                {
                    errorMessage += error.Message + "\n";
                }

                throw new CheckerException(errorMessage);
            }

        }

        public void EnterScope() => CurrentStackFrame.EnterScope();
        public void ExitScope() => CurrentStackFrame.ExitScope();

        public CheckStackFrame EnterStackFrame(CheckStackFrame? toPush = null) {
            if (toPush is null)
            {
                var newFrame = new CheckStackFrame(CurrentStackFrame, Error);
                StackFrames.Push(newFrame);
                return newFrame;
            }
            StackFrames.Push(toPush);
            return toPush;

        }

        public void ExitStackFrame() => StackFrames.Pop();

        public Type Cast(Type from, Type to, string message = "cannot convert type {0} to type {1}")
        {
            if (from is Error) return from;
            if (to is Error) return to;
            if (!from.Is(to)) return Error(string.Format(message, from, to));
            return to;
        }

        #endregion

        #region Declarations

        public void DeclareClass(ClassDeclaration c)
        {
            CheckStackFrame statics = new CheckStackFrame(CurrentStackFrame, Error);
            CheckStackFrame instances = new CheckStackFrame(statics, Error);
            Class parent = Primitive.Object;
            if (c.SuperClass != null)
            {
                if (c.SuperClass.Accept(this) is MetaType to && to.Stored is Class super)
                {
                    parent = super;
                }
                else Error("cannot extend anything other than a class");
            }

            ProtoClass proto = new ProtoClass(c.Name, Error, parent, statics, instances);
            Define(new MetaType(proto), c.Decl);
            EnterStackFrame(statics);
            //foreach (var (id, classConstraint) in c.GenericParameters)
            //{
            //    Class constraint = classConstraint?.Accept(this) is TypeObject to && to.Encapsulated is Class co ? co : Primitive.Object;
            //    CurrentScope.Define(new TypeObject(constraint), id);
            //}

            foreach (Declaration d in c.StaticDecls) if (d is FunctionDeclaration f) DeclareFunction(f);
            foreach (var constructor in c.Constructors) DeclareFunction(constructor);

            EnterStackFrame(instances);
            Define(proto, "this".ToVariable());
            foreach (Declaration d in c.InstanceDecls) if (d is FunctionDeclaration f) DeclareFunction(f);

            ExitStackFrame();
            ExitStackFrame();
        }

        public Type Visit(ClassDeclaration c)
        {
            //ProtoClass? parent = c.SuperClass != null ? CurrentStackFrame.Get(c.SuperClass) as ProtoClass : null;

            var proto = CurrentStackFrame.Get(c.Decl) is MetaType t && t.Stored is ProtoClass p ? p : throw new CheckerException("Expected protoclass");
            EnterStackFrame(proto.StaticMembers);
            foreach (Declaration d in c.StaticDecls) d.Accept(this);

            EnterStackFrame(proto.InstanceMembers);
            foreach (Declaration d in c.InstanceDecls) d.Accept(this);

            //if (parent != null) foreach ((string id, Type type) in parent.InstanceMembers) CurrentScope.Define(type, id);
            foreach (var constructor in c.Constructors) constructor.Accept(this);
            ExitStackFrame();
            ExitStackFrame();

            return Primitive.Void;
        }

        public Type Visit(ConstructorDeclaration c) => Visit(c as FunctionDeclaration);

        public void DeclareFunction(FunctionDeclaration f)
        {
            if (f.TypeParameters.Any())
            {
                FunctionType FillInGenericArgs(List<Type> typeArgs)
                {
                    var valid = typeArgs.SameLengthAndAll(f.TypeParameters,
                        (type, typeParam) => typeParam.Constraint is null || !(Cast(type, typeParam.Constraint.Accept(this)) is Error));
                    //var types = f.TypeParameters.Zip(typeArgs).ToList().Select((param, type) => null);
                    return new FunctionType(new (Type, string)[] { (Primitive.Int, "t") }, Primitive.Int);
                    //throw new NotImplementedException();
                }
                var gen = new GenericFunctionType(FillInGenericArgs);
                Define(gen, f.Decl);
            }
            else
            {
                // Check decl and args first, needed to make function type
                (Type type, string id)[] parameterTypes = f.Parameters.Select(arg => (arg.Accept(this), arg.Identifier)).ToArray();
                Type returnType = f.Decl.Accept(this) is Type t ? t : throw new CheckerException("Expected Type");
                FunctionType ft = new FunctionType(parameterTypes, returnType);
                // define the header using the function type from above
                Define(ft, f.Decl);
            }
        }

        public Type Visit(FunctionDeclaration f)
        {
            if (CurrentStackFrame.Resolve(f.Decl, out Type type, out uint _, out uint _))
            {
                // enter the function scope and define the args and type args
                EnterStackFrame();
                if(f.TypeParameters.Any() && type is GenericFunctionType generic)
                {
                    foreach (var typeParam in f.TypeParameters)
                    {
                        if ((typeParam.Constraint?.Accept(this) ?? new MetaType(Primitive.Object)) is MetaType constraintType)
                        {
                            Define(constraintType, typeParam);
                        }
                        else Error($"Generic parameter {typeParam.Identifier} constraint was not a valid type");
                    }
                }
                FunctionType ft = type is FunctionType fnt ? fnt : throw new CheckerException("Expected Function type");

                foreach (var ((argType, _), argDecl) in ft.Parameters.Zip(f.Parameters)) Define(argType, argDecl);
                // check the body now that its header and args have been defined
                Type body = f.Body.Accept(this);
                if (f is ConstructorDeclaration)
                {
                    if (body != Primitive.Void) return Error("constructor cannot return value");
                }
                else if (body == Primitive.Void)
                {
                    if (ft.ReturnType != Primitive.Void) return Error("function " + f.Decl.Identifier + "not all code paths return a value");
                }
                else Cast(body, ft.ReturnType, f.Decl.Identifier + " function definition invalid, expected {1}, returned {0}");
                f.LocalCount = CurrentStackFrame.Count;
                ExitStackFrame();
                return ft;
            }
            else throw new UnexpectedException("Could not resolve function type");
        }

        public Type Visit(OperatorOverloadDeclaration o)
        {
            throw new NotImplementedException("In development");
            //if (!DoImpl.Peek())
            //{
            //    switch (o.Operator)
            //    {
            //        case BinaryOperator b:
            //            if (o.Parameters.Count != 2) return Error($"Binary operator {b.Name} requires two parameters to overload");
            //            break;
            //        case UnaryOperator u:
            //            if (o.Parameters.Count != 1) return Error($"Unary operator {u.Name} requires one parameter to overload");
            //            break;
            //        default:
            //            throw new UnexpectedException("No other operator types");
            //    }
            //    var res = Visit(o as FunctionDeclaration);
            //    return res;
            //}
            //else
            //{
            //    return Visit(o as FunctionDeclaration);
            //}
        }

        public Type Visit(VariableDeclaration v)
        {
            Type? init = v.Initializer?.Accept(this);
            if (v.Decl.IsVar)
            {
                if (init is null) return Error($"Cannot determine type of var {v.Name} without initializer");
                if (init is MetaType meta) Define(meta, v.Decl);
                else Define(init, v.Decl);
            }
            else
            {
                Type decl = v.Decl.Accept(this);
                if (init != null)
                {
                    var casted = Cast(init, decl);
                    if (casted is Error e) return e;
                    if (casted is MetaType meta) Define(meta, v.Decl);
                    else Define(casted, v.Decl);
                }
            }
            return Primitive.Void;
        }

        #endregion

        #region Expressions

        public Type Visit(Declarator d) => d.Type?.Accept(this) switch
        {
            MetaType meta => meta.Stored,
            Primitive t when t == Primitive.MetaType => Error("Declared type must be a check time constant"),
            Type invalid => Error($"Declaration requires valid type, found: {invalid}"),
            null => Error($"Cannot use var here")
        };

        public Type Visit(ArrayAccess a)
        {
            Type elem = a.Collection.Accept(this);
            var idxTypes = a.Index.Select(idx => idx.Accept(this));
            // array types are defined with empty braces []
            if (elem is MetaType m && !idxTypes.Any()) return new MetaType(new ArrayType(m.Stored));
            if (elem is IGenericType gt && idxTypes.All(type => type is MetaType))
            {
                var unwrappedTypes = idxTypes.OfType<MetaType>().Select(m => m.Stored);
                return gt.WithTypeArguments(unwrappedTypes);
            }
            //if (elem is GenericClass gt && idxTypes.All(type => type is MetaType))
            //{
            //    // TODO same here generate ProtoClass based on type parameters
            //    return Error("Generics not supported yet");
            //}
            if (elem is ArrayType at)
            {
                if (a.Index.Length != 1)
                    return Error("array access requires exactly 1 index");
                if (idxTypes.Count() != 1 || idxTypes.First() != Primitive.Int) return Error($"only ints can be used to index into an array, found: {(idxTypes is null ? "empty" : string.Join(",", idxTypes.Select(x => x.ToString())))}");
                else return at.ElementType;
            }
            return Error("type " + elem.ToString() + " is not accessable by array access operator []");
        }

        public Type Visit(ArrayAssign a) => (a.ArrayIdx.Collection.Accept(this), a.ArrayIdx.Index.Select(idx => idx.Accept(this)), a.Right.Accept(this)) switch
        {
            (Error e, _, _) => e,
            (_, Error e, _) => e,
            (_, _, Error e) => e,
            (ArrayType c, IEnumerable<Type> i, Type r) when i.Count() == 1 && i.First() == Primitive.Int => Cast(r, c.ElementType),
            (Type c, IEnumerable<Type> i, Type r) => Error($"type {c} cannot be accessed with an idx type of {string.Join(",", i.Select(idx => idx.ToString()))}")
        };

        public Type Visit(As a)
        {
            a.Left.Accept(this);
            Type r = a.Right.Accept(this);
            if (r is MetaType castedTo) return castedTo.Stored;
            return Error("the right side of an is expression must be a type, found: " + r.ToString());
        }

        public Type Visit(Binary b) => (b.Left.Accept(this), b.Right.Accept(this)) switch
        {
            (Error e, _) => e,
            (_, Error e) => e,
            (Type left, Type right) => 
                // Try to find built in operator first
                (b.Oper = b.Overloads.FindBestMatch(left, right))?.GetResultType() ??
                (GlobalScope.Has(b.Op) ? 
                    // If it's user defined it can be found in global scope
                    GlobalScope.Get(b.Op.ToVariable()) : 
                    Error($"binary operator not defined for {left} {b.Op} {right}"))
        };

        public Type Visit(Call c)
        {
            Type calltype = c.Caller.Accept(this);
            if (calltype is Error) return calltype;
            if (calltype is MetaType t && t.Stored is ProtoClass)
            {
                // Rather than have to store the overload id within the call it is easier to turn a constructor call into
                // a dereference to static field "" where the constructor lives
                c.MakeConstructorCall();
                return Visit(c);
            }

            Type[] argtypes = c.Args.Select(x => x.Accept(this)).ToArray();
            if (calltype is FunctionType functype)
            {
                return functype.Valid(out uint _, argtypes) ?
                    functype.ReturnType :
                    Error($"{c.Caller} expects ({string.Join(",", functype.Parameters.Select(x => x.type.ToString()))}) found: ({string.Join(",", argtypes.Select(x => x.ToString()))})");
            }
            if (calltype is MethodGroupType mgt)
            {
                (FunctionType? bestMatch, uint? id) = mgt.FindBestMatch(argtypes);
                if(bestMatch is null || id is null) return Error($"No overload could be found for ({string.Join(",", argtypes.Select(x => x.ToString()))})");
                IBindable caller = c.Caller is Variable v ? v : c.Caller is MemberAccess ma ? ma.Member : throw new UnexpectedException("Caller is not able to be overloaded");
                if(caller.ResolveLevel.HasValue)
                {
                    caller.Bind(id.Value, caller.ResolveLevel.Value);
                }
                else throw new UnexpectedException("Unresolved caller");
                return bestMatch.ReturnType;
            }
            return Error($"type {calltype} is not callable");
        }

        public Type Visit(ExpressionWrapper e) => Error($"{e} is not a complete statement");

        public Type Visit(TupleAccess t) => t.Left.Accept(this) switch
        {
            Error e => e,
            TupleType tt when tt.Types.Length > t.Member => tt.Types[t.Member],
            TupleType tt => Error($"cannot access element {t.Member} of tuple type {tt} which has {tt.Types.Length} elements"),
            Type type => Error($"Cannot reference member {t.Member} of non tuple type {type}")
        };

        public Type Visit(Lambda l)
        {
            if(l.Left is ParamListWrapper plw)
            {
                List<(Type paramType, string paramName)> args = new();

                EnterStackFrame();

                foreach(var parameter in plw.Wrapped.Parameters)
                {
                    var argType = parameter.Accept(this);
                    Define(argType, parameter);
                    args.Add((argType, parameter.Identifier));
                }

                Type returnType = l.Right.Accept(this);

                // TODO handle empty param list types e.g. () => int
                l.LocalCount = CurrentStackFrame.Count;
                ExitStackFrame();

                return new FunctionType(args.ToArray(), returnType);
            }
            else
            {
                return (l.Left.Accept(this), l.Right.Accept(this)) switch
                {
                    (MetaType args, MetaType result) when args.Stored is TupleType tt =>
                        new MetaType(new FunctionType(tt.Types.Select(t => (t, "arg")).ToArray(), result.Stored)),//Error("NOT IMPLEMENTED 
                    (MetaType arg, MetaType result) =>
                        new MetaType(new FunctionType(new (Type, string)[] { (arg, "arg")}, result.Stored)),
                    (Type args, Type result) => Error($"Lambdas currently only work for types, not {args} => {result}")
                };
            }
        }

        public Type Visit(ListLiteral l)
        {
            return new ArrayType(Type.CommonAncestor(l.Args.Select(x => x.Accept(this)).ToArray()));
        }

        public Type Visit(LocalAssign a) => (a.Variable.Accept(this), a.Right.Accept(this)) switch
        {
            (Error e, _) => e,
            (_, Error e) => e,
            (Type l, Type r) => Cast(r, l)
        };

        public Type Visit(MemberAccess d) => d.Left.Accept(this) switch
        {
            Error e => e,
            ArrayType _ when d.ArrayLengthAccess() => Primitive.Int,
            ProtoClass instances => instances.GetInstanceMemberType(d.Member),
            MetaType t when t.Stored is ProtoClass statics => statics.GetStaticMemberType(d.Member),
            Type other => Error($"cannot dereference type: {other}")
        };

        public Type Visit(MemberAssign a) => (a.Member.Accept(this), a.Right.Accept(this)) switch
        {
            (Error e, _) => e,
            (_, Error e) => e,
            (Type l, Type r) when a.Member.ArrayLength => Error("cannot assign to an array length"),
            (Type l, Type r) => Cast(r, l),
        };

        public Type Visit<E>(Literal<E> c) where E : struct => c.Type;

        public Type Visit(AST.StringLiteral s) => Primitive.String;

        public Type Visit(NullExpr n) => Primitive.Object;

        public Type Visit(ShortCircuit s) => (s.Left.Accept(this), s.Right.Accept(this)) switch
        {
            (Error e, _) => e,
            (_, Error e) => e,
            (Type left, Type right) => Cast(right, Cast(left, Primitive.Bool))
        };

        public Type Visit(Ternary t) => (t.Condition.Accept(this), t.IfTrue.Accept(this), t.IfFalse.Accept(this)) switch
        {
            (Primitive condition, _, _) when condition != Primitive.Bool => 
                Error($"Ternary condition requires a boolean, found a {condition}"),
            (_, Error e, _) => e,
            (_, _, Error e) => e,
            (_, Type iftrue, Type iffalse) => Type.CommonAncestor(iftrue, iffalse)
        };

        public Type Visit(TupleLiteral t)
        {
            if (t.Args.Length == 1) return t.Args.First().Accept(this);
            var types = t.Args.Select(arg => arg.Accept(this)).ToArray();
            if (types.All(type => type is MetaType)) return new MetaType(new TupleType(types));
            else return new TupleType(types);
        }

        public Type Visit(Unary u)
        {
            Type input = u.Expr.Accept(this);
            if (input is Error) return input;
            var op = u.Overloads.FindBestMatch(input);
            if (op == null) return Error("unary operator " + u.Op + " is not defined for type " + input.ToString());
            u.Oper = op;
            return op.GetResultType();
        }

        public Type Visit(Variable v)
        {
            if(CurrentStackFrame.Resolve(v, out Type type, out uint level, out uint id))
            {
                v.Bind(id, level);
            }
            return type;
        }

        #endregion

        #region Statements

        public Type Visit(Block b)
        {
            if(!b.IsProgram) EnterScope();
            // Forward Declaration of Classes
            foreach (ClassDeclaration cd in b.Classes) DeclareClass(cd);
            // Forward Declaration of Functions
            foreach (FunctionDeclaration func in b.Functions) DeclareFunction(func);
            foreach (var overload in b.OverloadedOperators)
            {
                overload.Accept(this);
            }
            Type? ret = null;
            foreach (IASTNode d in b.Lines)
            {
                Type temp = d.Accept(this);
                if (ret != null) return Error("unreachable code detected");
                if (d is Statement and not Expression && temp != null)
                {
                    ret = temp;
                }
            }
            if(!b.IsProgram) ExitScope();
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
                if (f.Body is Statement and not Expression && body != null) return body;
                return Primitive.Void;
            }
            return Error("only array types are iterable, found:" + collection.ToString());
        }

        public Type Visit(IfStatement i)
        {
            Type cond = i.Condition.Accept(this);
            Cast(cond, Primitive.Bool, "if statement condition requires a boolean, found a {0}");
            Type iftrue = i.Iftrue.Accept(this);
            Type? iffalse = i.Iffalse?.Accept(this);
            if (i.Iftrue is Statement and not Expression && iftrue != null)
            {
                if (i.Iffalse is Statement and not Expression && iffalse != null)
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
            if (w.Body is Statement and not Expression && body != null) return body;
            return Primitive.Void;
        }

        public Type Visit(UsingStatement u)
        {
            Type used = u.Used.Accept(this);
            if (used is MetaType t && t.Stored is ProtoClass staticClass)
                foreach (var (id, type) in staticClass.GetStaticMemberTypes())
                {
                    // TODO restore functionality
                    CurrentStackFrame.Assign(new Variable(id), type);
                }
            {
                return Primitive.Void;
            }
            throw new CheckerException("Only classes can be used, found " + used);
        }

        #endregion
    }
}
