using Outlet.StandardLib;
using Outlet.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Lexer
{
    public interface ILexer
    {
        LinkedList<Lexeme> Scan(byte[] charStream, StandardError errorHandler);
    }
}
