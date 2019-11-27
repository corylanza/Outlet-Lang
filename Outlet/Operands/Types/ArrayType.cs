using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public class ArrayType : Type, ICheckableClass {

		public Type ElementType;

		public ArrayType(Type elem) {
			ElementType = elem;
		}

		public override bool Equals(Operand b) => b is ArrayType at && ElementType.Equals(at.ElementType);

        public Type GetInstanceType(string s)
        {
            //return new FunctionType(new (Type, string)[] { }, Primitive.Bool);
            // TODO needed to create methods such as list.count()
            throw new NotImplementedException();
        }

        public Type GetStaticType(string s)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<(string id, Type type)> GetStaticMemberTypes()
        {
            throw new NotImplementedException();
        }
        
		public override bool Is(Type t, out int level) {
            if(t is ArrayType at && ElementType.Is(at.ElementType, out level))
            {
                return true;
            }
            if(t == Primitive.Object)
            {
                level = int.MaxValue;
                return true;
            }
            else
            {
                level = -1;
                return false;
            }
		}

		public override string ToString() => ElementType.ToString() + "[]";
	}
}
