using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet {
	public class OutletException : Exception {
		public OutletException() { }
		public OutletException(string s) : base(s) { }
	}
}
