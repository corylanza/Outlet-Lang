using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public abstract class Type : Operand {

        public override Type GetOutletType()
        {
            return new MetaType(this);
        }

        public bool Is(Type t) => NewIs(this, t); //Is(t, out int _);
		public abstract bool Is(Type t, out int level);

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

		public virtual Operand Default() => Constant.Null();

		private static Type ClosestAncestor(Type a, Type b) {
			if(!(a is Class && b is Class)) throw new NotImplementedException("common ancestor only currently works for classes");
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

		public static Type CommonAncestor(params Type[] types) {
			if(types.Length == 0) return Primitive.Object;
			Type ancestor = types[0];
			foreach(Type cur in types) {
				ancestor = ClosestAncestor(ancestor, cur);
			}
			return ancestor;
		}
		
	}

}