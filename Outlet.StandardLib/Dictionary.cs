using Outlet.ForeignFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.StandardLib
{
    [ForeignClass(Name = "dict")]
    public class ODictionary
    {
        private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

        [ForeignConstructor]
        public ODictionary()
        {

        }

        [ForeignFunction(Name = "get")]
        public object Get(string s) => dictionary[s];

        [ForeignFunction(Name = "set")]
        public void Set(string s, object value) => dictionary[s] = value;

        [ForeignFunction(Name = "keys")]
        public IEnumerable<string> Keys() => dictionary.Keys;

        [ForeignFunction(Name = "values")]
        public IEnumerable<object> Values() => dictionary.Values;
    }
}
