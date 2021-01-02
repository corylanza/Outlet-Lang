using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Outlet.Types
{
    public class GenericFunctionType : Type, IGenericType
    {
        private readonly Func<List<Type>, FunctionType> GenericTemplate;

        public GenericFunctionType(Func<List<Type>, FunctionType> template)
        {
            GenericTemplate = template;
        }

        public override bool Is(Type t, out uint level)
        {
            throw new NotImplementedException();
        }

        public FunctionType WithTypeArguments(IEnumerable<Type> typeArgs) => GenericTemplate(typeArgs.ToList());

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        Type IGenericType.WithTypeArguments(IEnumerable<Type> typeArgs) => WithTypeArguments(typeArgs);
    }
}
