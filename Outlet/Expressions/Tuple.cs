using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Expressions {
	public class Tuple : Operand {

		public Tuple(params Operand[] vals) {
			Value = vals;
		}
	}
}
