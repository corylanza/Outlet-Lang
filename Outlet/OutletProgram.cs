using Outlet.AST;
using Outlet.Checking;
using Outlet.FFI;
using Outlet.Interpreting;
using Outlet.Lexing;
using Outlet.Operands;
using Outlet.Parsing;
using Outlet.Tokens;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Outlet.StandardLib;
using Outlet.TreeViewer;

namespace Outlet
{

    public abstract class OutletProgram
    {
        public Checker Checker { get; private set; }
        public Interpreter Interpreter { get; private set; }
        public SystemInterface System { get; private set; }
        private List<IASTNode> Nodes { get; set; } = new List<IASTNode>();

        protected OutletProgram(SystemInterface sys)
        {
            System = sys;

            Checker = new Checker();
            new NativeInitializer(sys).Register(AppDomain.CurrentDomain.Load("Outlet.StandardLib"), Checker.GlobalScope, Checker.ErrorHandler);

            Interpreter = new Interpreter();
        }

        protected Operand RunBytes(byte[] bytes)
        {
            try
            {
                LinkedList<Token> lexout = Lexer.Scan(bytes, System.StdErr);
                IASTNode program = new Parser(lexout).Parse();
                Nodes.Add(program);
                Checker.Check(program);
                Operand res = Interpreter.Interpret(program);
                return res;
            }
            catch (Exception e)
            {
                System.StdErr(e);
                return Value.Null;
            }
        }

        protected void Reset()
        {
            Checker = new Checker();
            Interpreter = new Interpreter();
        }

        public Node GenerateAST()
        {
            return new ASTViewer().BuildTree(Nodes.ToArray());
        }
    }

    public class ReplOutletProgram : OutletProgram
    {
        public ReplOutletProgram(SystemInterface sys) : base(sys) { }

        public Operand Run(byte[] bytes) => RunBytes(bytes);
    }

    public class OutletProgramFile : OutletProgram
    {
        private readonly byte[] _bytes;

        public OutletProgramFile(byte[] bytes, SystemInterface sys) : base(sys) {
            _bytes = bytes;
        }

        public Operand Run()
        {
            var res = RunBytes(_bytes);
            // reset variables after program run
            Reset();
            return res;
        }

    }
}
