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
        [ForeignConstructor]
        public Example()
        {

        }
    }
}
