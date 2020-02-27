using System;
using Outlet.Operands;

namespace Outlet.Types {

    public interface ITyped
    {
        bool Is(ITyped t);

        bool Is(ITyped t, out int level);
    }

	public abstract class Type : ITyped {

        public bool Is(ITyped t) => NewIs(this, t); //Is(t, out int _);
		public abstract bool Is(ITyped t, out int level);

        public static bool NewIs(ITyped from, ITyped to)
        {
            return (from, to) switch
            {
                (Type other, UnionType unionTo) => NewIs(other, unionTo.First) || NewIs(other, unionTo.Second),
                (ArrayType arrayFrom, ArrayType arrayTo) => NewIs(arrayFrom.ElementType, arrayTo.ElementType),
                (TupleType ttFrom, TupleType ttTo) =>ttFrom.Types.SameLengthAndAll(ttTo.Types, (fromElementType, toElementType) => NewIs(fromElementType, toElementType)),
                (FunctionType funcFrom, FunctionType funcTo) => true,
                (Class classFrom, Class classTo) => (classFrom.Equals(classTo) || (classFrom.Parent != null && NewIs(classFrom.Parent, classTo))),
                (TypeObject meta, Primitive type) => type == Primitive.MetaType,
                (Type any, Primitive obj) => obj == Primitive.Object,
                _ => throw new NotImplementedException()
            };
        }

		public virtual Operand Default() => Constant.Null();

		private static ITyped ClosestAncestor(ITyped a, ITyped b) {
            if (!(a is Class && b is Class)) return Primitive.Object;//throw new NotImplementedException("common ancestor only currently works for classes");
			Class cur = a as Class;
			while(cur != Primitive.Object) {
				if(cur.Is(b) && b.Is(cur)) return cur;
				cur = cur.Parent;
			}
			cur = b as Class;
			while(cur != Primitive.Object) {
				if(cur.Is(a) && a.Is(cur)) return cur;
				cur = cur.Parent;
			}
			return Primitive.Object;
		}

		public static Type CommonAncestor(params ITyped[] types) {
			if(types.Length == 0) return Primitive.Object;
			ITyped ancestor = types[0];
			foreach(ITyped cur in types) {
                if(cur is TypeObject)
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