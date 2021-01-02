using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.Types
{
    public class Error : Type
    {
        public readonly string Message;

        public Error(string message, Action<Error> errorHandler)
        {
            Message = message;

            errorHandler(this);
        }

        public override bool Is(Type t, out uint level)
        {
            level = 0;
            return false;
        }

        public override string ToString() => "error";
    }
}
