using Outlet.AST;
using Outlet.Checking;
using Outlet.FFI;
using Outlet.Interpreting.TreeWalk;
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
using Outlet.Interpreting.ByteCode;
using Outlet.Compiling;

namespace Outlet
{

    public abstract class OutletProgram
    {
        public Checker Checker { get; private set; }
        public Interpreter Interpreter { get; private set; }
        public VirtualMachine ByteCodeVM { get; private init; }
        public ByteCodeGenerator Compiler = new ByteCodeGenerator();
        public SystemInterface System { get; private set; }
        private List<IASTNode> Nodes { get; set; } = new List<IASTNode>();

        protected OutletProgram(SystemInterface sys)
        {
            System = sys;

            Checker = new Checker();
            var stdlib = AppDomain.CurrentDomain.Load("Outlet.StandardLib");
            new NativeInitializer(sys).Register(stdlib, Checker.GlobalScope, Checker.Error);

            Interpreter = new Interpreter();
            ByteCodeVM = new VirtualMachine();

        }

        protected Operand RunBytes(byte[] bytes)
        {
            try
            {
                LinkedList<Lexeme> lexout = Lexer.Scan(bytes, System.StdErr);
                IASTNode program = new Parser(lexout).Parse();
                Nodes.Add(program);
                Checker.Check(program);

                bool useByteCode = false;

                if (useByteCode)
                {
                    var byteCode = Compiler.GenerateByteCode(program);
                    string? result = ByteCodeVM.Interpret(byteCode.ToArray())?.ToString();
                    return result is null ? Value.Null : new Operands.String(result);
                }
                else
                {
                    Operand res = Interpreter.Interpret(program);
                    return res;
                }
            }
            catch (Exception e)
            {
                System.StdErr(e);
                return Value.Null;
            }
        }

        protected void CheckBytes(byte[] bytes)
        {
            try
            {
                LinkedList<Lexeme> lexout = Lexer.Scan(bytes, System.StdErr);
                IASTNode program = new Parser(lexout).Parse();
                Nodes.Add(program);
                Checker.Check(program);
            }
            catch (Exception e)
            {
                System.StdErr(e);
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
        public ReplOutletProgram(SystemInterface sys) : base(sys) { }

        public Operand Run(byte[] bytes) => RunBytes(bytes);

        public void Check(byte[] bytes) => CheckBytes(bytes);
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
