using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.Operands
{
    public class Field
    {
        public bool IsPublic = true;
        public bool IsReadonly = false;
        public bool IsConstant = false;

        public Operand Value;
        public string Name;

        public Field() { }

        public Field(string name, Operand value)
        {
            Name = name;
            Value = value;
        }



    }
}
