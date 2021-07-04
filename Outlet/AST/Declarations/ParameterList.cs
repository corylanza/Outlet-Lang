using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST
{
    public class ParameterList
    {
        public List<Declarator> Parameters { get; init; }

        public ParameterList(List<Declarator> parameters)
        {
            Parameters = parameters;
        }

        public override string ToString() => $"({string.Join(',', Parameters.Select(parameter => parameter.ToString()))})";
    }
}
