using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST
{
    public abstract class ExpressionWrapper : Expression
    {
        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);
    }

    public abstract class ExpressionWrapper<E> : ExpressionWrapper where E : class
    {
        public E Wrapped { get; set; }

        protected ExpressionWrapper(E wrapped)
        {
            Wrapped = wrapped;
        }

        public override T Accept<T>(IASTVisitor<T> visitor) => visitor.Visit(this);

        public override string ToString() => $"{Wrapped}";
    }

    public class ParamListWrapper : ExpressionWrapper<ParameterList>
    {
        public ParamListWrapper(ParameterList wrapped) : base(wrapped)
        {

        }
    }
}
