using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.Tokens
{
    public class Symbol : Token
    {
        public string Name { get; private set; }

        protected Symbol(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
