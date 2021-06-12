using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Compiling.ByteCode
{
    public class CodeBlock : IEnumerable<byte>
    {
        private List<byte> Bytes { get; set; }

        public CodeBlock(IEnumerable<byte> bytes)
        {
            Bytes = bytes.ToList();
        }

        public CodeBlock(params IEnumerable<byte>[] bytes)
        {
            Bytes = bytes.SelectMany(b => b).ToList();
        }




        public IEnumerator GetEnumerator() => Bytes.GetEnumerator();

        IEnumerator<byte> IEnumerable<byte>.GetEnumerator() => Bytes.AsEnumerable().GetEnumerator();
    }
}
