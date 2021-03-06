﻿using System;
using System.Collections.Generic;

namespace Outlet.Types {
	public class ArrayType : Type {

		public Type ElementType;

		public ArrayType(Type elem) {
			ElementType = elem;
		}

		//public override bool Equals(Operand b) => b is ArrayType at && ElementType.Equals(at.ElementType);

        public Type GetStaticType(string s)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<(string id, Type type)> GetStaticMemberTypes()
        {
            throw new NotImplementedException();
        }
        
		public override bool Is(Type t, out uint level) {
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
                level = 0;
                return false;
            }
		}

		public override string ToString() => ElementType.ToString() + "[]";
	}
}
