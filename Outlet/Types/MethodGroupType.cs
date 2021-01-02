using System;
using System.Collections.Generic;
using System.Linq;

namespace Outlet.Types
{
    public class MethodGroupType : Type
    {
        // Hidden class so that not all FunctionTypes need an overload ID
        private class MethodWrapper : IOverloadable
        {
            public MethodWrapper(FunctionType type, uint id)
            {
                OverloadId = id;
                MethodType = type;
            }

            public uint OverloadId { get; private set; }
            public FunctionType MethodType { get; private set; }

            public bool Valid(out uint level, params Type[] inputs) => MethodType.Valid(out level, inputs);
        }

        private Overload<MethodWrapper> Methods { get; set; }

        public MethodGroupType(params (FunctionType type, uint id)[] functions)
        {
            Methods = new Overload<MethodWrapper>(functions.Select(method => new MethodWrapper(method.type, method.id)).ToArray());
        }

        public void AddMethod(FunctionType type, uint id) => Methods.Add(new MethodWrapper(type, id));

        public (FunctionType? type, uint? id) FindBestMatch(params Type[] inputs)
        {
            var method = Methods.FindBestMatch(inputs);
            if(method != null)
            {
                return (method.MethodType, method.OverloadId);
            }
            return (null, null);
        }

        //public override bool Equals(Operand b) => ReferenceEquals(this, b);

        public override bool Is(Type t, out uint level)
        {
            level = 0;
            return false;
        }

        public override string ToString()
        {
            return "MethodGroup";
        }
    }
}
