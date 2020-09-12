using Outlet.ForeignFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Outlet.StandardLib
{
    [ForeignClass(Name = "list")]
    public class OList
    {
        private readonly List<object> list = new List<object>();

        [ForeignConstructor]
        public OList()
        {

        }

        [ForeignFunction(Name = "get")]
        public object Get(int i) => list[i];

        [ForeignFunction(Name = "push")]
        public void Push(object o) => list.Add(o);

        [ForeignFunction(Name = "peek")]
        public object Peek() => list.Last();

        [ForeignFunction(Name = "pop")]
        public object Pop()
        {
            var temp = list.Last();
            list.RemoveAt(list.Count - 1);
            return temp;
        }

        [ForeignFunction(Name = "toArray")]
        public IEnumerable<object> ToArray() => list;
    }
}
