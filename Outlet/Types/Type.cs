using System;
using Outlet.Operands;

namespace Outlet.Types {

	public abstract class Type {

        public bool Is(Type t) => NewIs(this, t); //Is(t, out int _);
		public abstract bool Is(Type t, out uint level);

        public static bool NewIs(Type from, Type to)
        {
            return (from, to) switch
            {
                (Type other, UnionType unionTo) => NewIs(other, unionTo.First) || NewIs(other, unionTo.Second),
                (ArrayType arrayFrom, ArrayType arrayTo) => NewIs(arrayFrom.ElementType, arrayTo.ElementType),
                (TupleType ttFrom, TupleType ttTo) =>ttFrom.Types.SameLengthAndAll(ttTo.Types, (fromElementType, toElementType) => NewIs(fromElementType, toElementType)),
                (FunctionType funcFrom, FunctionType funcTo) => true,
                (Class classFrom, Class classTo) => (classFrom.Equals(classTo) || (classFrom.Parent != null && NewIs(classFrom.Parent, classTo))),
                (MetaType meta, Primitive type) => type == Primitive.MetaType,
                (Type any, Primitive obj) => obj == Primitive.Object,
                _ => throw new NotImplementedException()
            };
        }

		public virtual Operand Default() => Constant.Null;

		private static Type ClosestAncestor(Type ca, Type cb) {
            if(ca is Class a && cb is Class b)
            {
                Class cur = a;
                while (cur != Primitive.Object)
                {
                    if (cur.Is(b) && b.Is(cur)) return cur;
                    if (cur.Parent is null) break;
                    cur = cur.Parent;
                }
                cur = b;
                while (cur != Primitive.Object)
                {
                    if (cur.Is(a) && a.Is(cur)) return cur;
                    if (cur.Parent is null) break;
                    cur = cur.Parent;
                }
            }
			return Primitive.Object;
		}

		public static Type CommonAncestor(params Type[] types) {
			if(types.Length == 0) return Primitive.Object;
			Type ancestor = types[0];
			foreach(Type cur in types) {
                if(cur is MetaType)
                {
                    ancestor = Primitive.MetaType;
                    break;
                }
                else ancestor = ClosestAncestor(ancestor, cur);
			}
			return ancestor as Type;
		}

        public abstract override string ToString();

    }

}