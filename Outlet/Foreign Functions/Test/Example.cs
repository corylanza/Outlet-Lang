using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.FFI
{
    [ForeignClass(Name = "example")]
    public class Example
    {
        [ForeignField(Name = "id")]
        public int Id;

        [ForeignConstructor]
        public Example(int id)
        {
            Id = id;
        }
    }
}
