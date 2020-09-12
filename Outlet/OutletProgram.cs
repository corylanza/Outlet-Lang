using Outlet.AST;
using Outlet.Checking;
using Outlet.FFI;
using Outlet.Interpreting;
using Outlet.Lexing;
using Outlet.Operands;
using Outlet.Parsing;
using Outlet.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Outlet
{
    public delegate string StandardIn();
    public delegate void StandardOut(string output);
    public delegate void StandardError(Exception error);

    public abstract class OutletProgram
    {
        public Checker Checker { get; private set; }
        public Interpreter Interpreter { get; private set; }
        public StandardIn StdIn { get; }
        public StandardOut StdOut { get; }
        public StandardError StdErr { get; }

        protected OutletProgram(StandardIn stdin, StandardOut stdout, StandardError stderror)
        {
            NativeInitializer.Register(AppDomain.CurrentDomain.Load("Outlet.StandardLib"));

            Checker = new Checker();
            Interpreter = new Interpreter();
            StdIn = stdin;
            StdOut = stdout;
            StdErr = stderror;
        }

        protected Operand RunBytes(byte[] bytes)
        {
            try
            {
                LinkedList<Token> lexout = Lexer.Scan(bytes, StdErr);
                IASTNode program = Parser.Parse(lexout);
                Checker.Check(program);
                Operand res = Interpreter.Interpret(program);
                return res;
            }
            catch (Exception e)
            {
                StdErr(e);
                return Value.Null;
            }
        }

        protected void Reset()
        {
            Checker = new Checker();
            Interpreter = new Interpreter();
        }

    }

    public class ReplOutletProgram : OutletProgram
    {
        public ReplOutletProgram(StandardIn stdin, StandardOut stdout, StandardError stderror) : base(stdin, stdout, stderror) { }

        public Operand Run(byte[] bytes) => RunBytes(bytes);
    }

    public class OutletProgramFile : OutletProgram
    {
        private readonly byte[] _bytes;

        public OutletProgramFile(byte[] bytes, StandardIn stdin, StandardOut stdout, StandardError stderror) : base(stdin, stdout, stderror) {
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
