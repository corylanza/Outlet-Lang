using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.Types
{
    public class GenericFunctionType : Type
    {
        public GenericFunctionType()
        {

        }

        public override bool Is(Type t, out uint level)
        {
            throw new NotImplementedException();
        }

        //public FunctionType Generic(params Type[] typeArgs)
        //{
        //    return new FunctionType();
        //}

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
