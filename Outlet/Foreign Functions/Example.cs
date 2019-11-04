using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.FFI
{
    [ForeignClass(Name = "person")]
    public class Example
    {
        [ForeignField(Name = "Hey")]
        public int Hey;

        [ForeignConstructor]
        public Example(int hey)
        {
            Hey = hey;
        }
    }
}
