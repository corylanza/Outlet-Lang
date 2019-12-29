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

        public static readonly Stack<bool> DoImpl = new Stack<bool>();
        public static readonly Stack<SymbolTable> Scopes = new Stack<SymbolTable>();
        private static readonly Type ErrorType = new ProtoClass("error", null, null, null);
        public static int ErrorCount = 0;

        public static void Check(IASTNode program)
        {
            ErrorCount = 0;
            if (program is FunctionDeclaration || program is ClassDeclaration)
            {
                DoImpl.Push(false);
                program.Accept(Hidden);
                DoImpl.Pop();
                DoImpl.Push(true);
                program.Accept(Hidden);
                DoImpl.Pop();
            }
            else program.Accept(Hidden);
            if (ErrorCount > 0) throw new CheckerException(ErrorCount + " Checking errors encountered");
        }
        private static readonly Checker Hidden = new Checker();
        private Checker() => Scopes.Push(SymbolTable.Global);
        public static void Define(ITyped t, string s) => Scopes.Peek().Define(t, s);
        public static ITyped Get(int level, string s) => Scopes.Peek().GetType(level, s);
        public static (ITyped type, int level, int id) Find(string s) => Scopes.Peek().Bind(s);

        public static SymbolTable EnterScope()
        {
            if (Scopes.Count == 0) Scopes.Push(SymbolTable.Global);
            else Scopes.Push(new SymbolTable(Scopes.Peek()));
            return Scopes.Peek();
        }

        public static void ExitScope() => Scopes.Pop();

        public static Type Error(string message)
        {
            ErrorCount++;
            if (message != "") Program.ThrowException(message);
            return ErrorType;
        }

        public static ITyped Cast(ITyped from, ITyped to, string message = "cannot convert type {0} to type {1}")
        {
            if (from == ErrorType || to == ErrorType) return ErrorType;
            if (!from.Is(to)) return Error(string.Format(message, from, to));
            return to;
        }

        #endregion

        #region Declarations

        public ITyped Visit(ClassDeclaration c)
        {
            if (!DoImpl.Peek())
            {
                var statics = new Dictionary<string, Type>();
                var instances = new Dictionary<string, Type>();
                Class parent = Primitive.Object;
                if (c.SuperClass != null)
                {
                    if (!(c.SuperClass.Accept(this) is TypeObject)) Error("cannot extend anything other than a class");
                    else
                    {
                        if (Get(c.SuperClass.resolveLevel, c.SuperClass.Name) is TypeObject meta &&
                            meta.Encapsulated is Class pt) parent = pt;
                        else Error("cannot extend anything other than a class");
                    }
                }
                Define(new TypeObject(new ProtoClass(c.Name, parent, instances, statics)), c.Name);
                EnterScope();
                foreach (Declaration d in c.StaticDecls)
                {
                    d.Accept(this);
                    if (Find(d.Name).type is Type declType)
                        statics.Add(d.Name, declType);
                    else Error("not supported");
                }
                EnterScope();
                foreach (Declaration d in c.InstanceDecls)
                {
                    d.Accept(this);
                    if (Find(d.Name).type is Type declType)
                        instances.Add(d.Name, declType);
                    else Error("not supported");
                }
                c.Constructor.Accept(this);
                statics.Add("", c.Constructor.Type);
                ExitScope();
                ExitScope();
            }
            else
            {
                ProtoClass parent = null;
                if (c.SuperClass != null)
                {
                    if (Get(c.SuperClass.resolveLevel, c.SuperClass.Name) is TypeObject meta &&
                        meta.Encapsulated is ProtoClass pt) parent = pt;
                    else Error("unexpected parent type");
                }
                EnterScope();
                foreach (Declaration d in c.StaticDecls) d.Accept(this);
                foreach (Declaration d in c.StaticDecls) if (d is FunctionDeclaration fd) Define(fd.Type, fd.Name);
                // if (parent != null) foreach (KeyValuePair<string, Type> d in parent.Statics) Define(d.Value, d.Key);
                EnterScope();
                Define(Get(2, c.Name), "this");
                foreach (Declaration d in c.InstanceDecls) d.Accept(this);
                foreach (Declaration d in c.InstanceDecls) if (d is FunctionDeclaration fd) Define(fd.Type, fd.Name);
                if (parent != null) foreach (KeyValuePair<string, Type> d in parent.InstanceMembers) Define(d.Value, d.Key);
                c.Constructor.Accept(this);
                ExitScope();
                ExitScope();
            }
            return null;
        }

        public ITyped Visit(ConstructorDeclaration c)
        {
            Visit(c as FunctionDeclaration);
            return null;
        }

        public ITyped Visit(FunctionDeclaration f)
        {
            if (!DoImpl.Peek())
            {
                // Check decl and args first, needed to make function type
                EnterScope();
                (ITyped type, string id)[] args = f.Args.Select(arg =>
                {
                    ITyped curArg = arg.Accept(this);
                    return (curArg, arg.ID);
                }).ToArray();
                ExitScope();
                ITyped returnType = f.Decl.Accept(this);
                FunctionType ft = new FunctionType(args, returnType as Type);
                f.Type = ft;
                // define the header using the function type from above
                Define(ft, f.Decl.ID);
            }
            else
            {
                FunctionType ft = f.Type;
                // enter the function scope and define the args;
                EnterScope();
                System.Array.ForEach(ft.Args, arg => Define(arg.type, arg.id));
                // check the body now that its header and args have been defined
                ITyped body = f.Body.Accept(this);
                if (f is ConstructorDeclaration)
                {
                    if (body != null) return Error("constructor cannot return value");
                }
                else if (body == null || body == Primitive.Void)
                {
                    if (ft.ReturnType != Primitive.Void) return Error("function " + f.Decl.ID + "not all code paths return a value");
                }
                else Cast(body, ft.ReturnType, f.Decl.ID + "function definition invalid, expected {1}, returned {0}");

                ExitScope();

            }
            return null;
        }

        public ITyped Visit(VariableDeclaration v)
        {
            ITyped decl = v.Decl.Accept(this);
            ITyped init = v.Initializer?.Accept(this);
            if (init != null) Cast(init, decl);
            if (init is TypeObject meta) Define(meta, v.Decl.ID);
            else Define(decl, v.Decl.ID);
            return null;
        }

        #endregion

        #region Expressions

        public ITyped Visit(Access a)
        {
            ITyped elem = a.Collection.Accept(this);
            ITyped idxType = a.Index.Length > 0 ? a.Index[0].Accept(this) : null;
            if (elem is TypeObject meta && meta.Encapsulated is Class c)
            {
                if (idxType is TypeObject)
                    return Error("Generics not supported yet");
                // array types are defined with empty braces []
                if(idxType == null)
                    return new TypeObject(new ArrayType(meta.Encapsulated));
            }
            if (a.Index.Length != 1)
                return Error("array access requires exactly 1 index");
            if (idxType != Primitive.Int)
                return Error("only ints can be used to index into an array, found: " + idxType.ToString());
            if (elem is ArrayType at) return at.ElementType;
            return Error("type " + elem.ToString() + " is not accessable by array access operator []");
        }

        public ITyped Visit(As a)
        {
            a.Left.Accept(this);
            ITyped r = a.Right.Accept(this);
            if (r is TypeObject castedTo) return castedTo.Encapsulated;
            return Error("the right side of an is expression must be a type, found: " + r.ToString());

        }

        public ITyped Visit(Assign a)
        {
            if (a.Left is Variable || a.Left is Deref)
            {
                ITyped l = a.Left.Accept(this);
                ITyped r = a.Right.Accept(this);
                if (l == ErrorType || r == ErrorType) return ErrorType;
                if (a.Left is Deref d && d.ArrayLength) return Error("cannot assign to an array length");
                Cast(r, l);
                return l;
            }
            return Error("illegal assignment, can only assign to variables and fields");
        }

        public ITyped Visit(Binary b)
        {
            ITyped left = b.Left.Accept(this);
            ITyped right = b.Right.Accept(this);
            if (left == ErrorType || right == ErrorType) return ErrorType;
            var op = b.Overloads.FindBestMatch(left, right);
            if (op == null) return Error("binary operator not defined for " + left.ToString() + " " + b.Op + " " + right.ToString());
            b.Oper = op;
            return op.GetResultType();
        }

        public ITyped Visit(Call c)
        {
            ITyped calltype = c.Caller.Accept(this);
            if (calltype == ErrorType) return ErrorType;
            if (calltype is TypeObject t && t.Encapsulated is ProtoClass containsConstructor) 
                calltype = containsConstructor.GetStaticMemberType("");

            ITyped[] argtypes = c.Args.Select(x => x.Accept(this)).ToArray();
            if (calltype is FunctionType functype)
            {
                var funcargs = functype.Args.ToArray();
                bool argsMatch = funcargs.SameLengthAndAll(argtypes, (arg, argType) => Cast(argType, arg.type, "Could not cast {0} to {1}") != ErrorType);
                return argsMatch ? functype.ReturnType :
                    Error(c.Caller + " expects " + "(" + funcargs.Select(x => x.type).ToList().ToListString() + ") found: (" + argtypes.ToList().ToListString() + ")");
            }
            if (calltype is MethodGroupType mgt)
            {
                if(c.Caller is Variable v)
                {
                    FunctionType bestMatch = mgt.FindBestMatch(argtypes);
                    if (bestMatch is null) return Error("No overload could be found for (" + argtypes.ToList().ToListString() + ")");
                    return bestMatch.ReturnType;
                }
                else
                {
                    throw new System.Exception("Should not be able to have method group for non variable");
                }
            }
            return Error("type " + calltype + " is not callable");
        }

        public ITyped Visit(Declarator d)
        {
            ITyped decl = d.Type.Accept(this);
            if (decl is TypeObject meta) return meta.Encapsulated;
            if (decl == Primitive.MetaType) return Error("Declared type must be a check time constant");
            return Error("declaration requires valid type, found: " + decl.ToString());
        }


        public ITyped Visit(Deref d)
        {
            ITyped inst = d.Left.Accept(this);
            if (inst == ErrorType) return ErrorType;
            if (inst is ArrayType && d.Identifier == "length")
            {
                d.ArrayLength = true;
                return Primitive.Int;
            }
            if (inst is TupleType tt && int.TryParse(d.Identifier, out int result))
            {
                if (result >= tt.Types.Length)
                    return Error("cannot access element " + result + " of tuple type " +
                        tt.ToString() + " which has only " + tt.Types.Length + " elements");
                return tt.Types[result];
            }
            if (inst is ProtoClass instances) 
                return instances.GetInstanceMemberType(d.Identifier);
            if (inst is TypeObject t && t.Encapsulated is ProtoClass statics) 
                return statics.GetStaticMemberType(d.Identifier);

            return Error("cannot dereference type: " + inst.ToString());
        }

        public ITyped Visit(Is i)
        {
            i.Left.Accept(this);
            ITyped r = i.Right.Accept(this);
            if (r is TypeObject || r == Primitive.MetaType) return Primitive.Bool;
            return Error("the right side of an is expression must be a type, found: " + r.ToString());
        }

        public ITyped Visit(Lambda l)
        {
            ITyped args = l.Left.Accept(this);
            ITyped res = l.Right.Accept(this);
            if (args is TypeObject a && res is TypeObject r)
            {
                return Error("NOT IMPLEMENTED");//new MetaType(new FunctionType());
            }
            return Error("Lambdas currently only work for types");
        }

        public ITyped Visit(ListLiteral l)
        {
            return new ArrayType(Type.CommonAncestor(l.Args.Select(x => x.Accept(this)).ToArray()));
        }

        public ITyped Visit<E>(Literal<E> c) => c.Type;

        public ITyped Visit(ShortCircuit s)
        {
            ITyped l = s.Left.Accept(this);
            ITyped r = s.Right.Accept(this);
            Cast(l, Primitive.Bool);
            Cast(r, Primitive.Bool);
            return Primitive.Bool;
        }

        public ITyped Visit(Ternary t)
        {
            ITyped cond = t.Condition.Accept(this);
            ITyped iftrue = t.IfTrue.Accept(this);
            ITyped iffalse = t.IfFalse.Accept(this);
            Cast(cond, Primitive.Bool, "Ternary condition requires a boolean, found a {0}");
            return Type.CommonAncestor(iftrue, iffalse);
        }

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
            if (input == ErrorType) return ErrorType;
            var op = u.Overloads.FindBestMatch(input);
            if (op == null) return Error("unary operator " + u.Op + " is not defined for type " + input.ToString());
            u.Oper = op;
            return op.GetResultType();
        }

        public ITyped Visit(Variable v)
        {
            (ITyped type, int level, int id) = Find(v.Name);
            v.resolveLevel = level;
            v.id = id;
            if (level == -1) return Error("variable " + v.Name + " could not be resolved");
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
            ITyped ret = null;
            foreach (IASTNode d in b.Lines)
            {
                ITyped temp = d.Accept(this);
                if (ret != null) return Error("unreachable code detected");
                if (d is Statement && !(d is Expression) && temp != null)
                {
                    ret = temp;
                }
            }
            DoImpl.Pop();
            ExitScope();
            return ret;
        }

        public ITyped Visit(ForLoop f)
        {
            ITyped collection = f.Collection.Accept(this);
            if (collection is ArrayType at)
            {
                EnterScope();
                ITyped loopvar = f.LoopVar.Accept(this);
                Cast(at.ElementType, loopvar);
                Define(loopvar, f.LoopVar.ID);
                ITyped body = f.Body.Accept(this);
                ExitScope();
                if (f.Body is Statement && !(f.Body is Expression) && body != null) return body;
                return null;
            }
            return Error("only array types are iterable, found:" + collection.ToString());
        }

        public ITyped Visit(IfStatement i)
        {
            ITyped cond = i.Condition.Accept(this);
            Cast(cond, Primitive.Bool, "if statement condition requires a boolean, found a {0}");
            ITyped iftrue = i.Iftrue.Accept(this);
            ITyped iffalse = i.Iffalse?.Accept(this);
            if (i.Iftrue is Statement && !(i.Iftrue is Expression) && iftrue != null)
            {
                if (i.Iffalse is Statement && !(i.Iffalse is Expression) && iffalse != null)
                {
                    // TODO should return common ancestor
                    return iftrue;
                }
            }
            return null;
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
            return null;
        }

        public ITyped Visit(UsingStatement u)
        {
            ITyped used = u.Used.Accept(this);
            if (used is TypeObject t && t.Encapsulated is ProtoClass staticClass)
                foreach (var (id, type) in staticClass.GetStaticMemberTypes())
                {
                    Define(type, id);
                }
            {
                return null;
            }
            throw new CheckerException("Only classes can be used, found " + used);
        }

        #endregion
    }
}
