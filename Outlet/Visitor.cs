using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet {
	public interface IVisitor<T> {

		T Visit(AST.Program p);
		T Visit(ClassDeclaration c);
		T Visit(ConstructorDeclaration c);
		T Visit(FunctionDeclaration f);
		T Visit(VariableDeclaration v);

		T Visit(Access a);
		T Visit(Assign a);
		T Visit(Binary b);
		T Visit(Call c);
		T Visit(Literal c);
		T Visit(Declarator d);
		T Visit(Deref d);
		T Visit(Is i);
		T Visit(Lambda l);
		T Visit(ListLiteral l);
		T Visit(ShortCircuit s);
		T Visit(Ternary t);
		T Visit(TupleLiteral t);
		T Visit(Unary u);
		T Visit(Variable v);

		T Visit(Block b);
		T Visit(ForLoop f);
		T Visit(IfStatement i);
		T Visit(ReturnStatement r);
		T Visit(WhileLoop w);
	}
}
