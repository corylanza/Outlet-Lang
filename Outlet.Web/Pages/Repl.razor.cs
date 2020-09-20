using Outlet.Operands;
using Outlet.StandardLib;
using Outlet.TreeViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Outlet.Web.Pages
{
    public partial class Repl
    {
        public string Input { get; set; } = "";

        public List<(string code, List<string> consoleOutput, Operand? result)> History = new List<(string, List<string>, Operand?)>();

        private ReplOutletProgram Program { get; set; }

        private Node? AST { get; set; }

        public Repl()
        {
            var browserInterface = new SystemInterface(
                stdin: GetInput,
                stdout: ShowOutput,
                stderr: ShowError
            );
            Program = new ReplOutletProgram(browserInterface);
        }

        protected List<string> Errors { get; set; } = new List<string>();

        public void RunOutlet()
        {
            Errors.Clear();
            History.Add((Input, new List<string>(), null));
            var result = Program.Run(Encoding.ASCII.GetBytes(Input));
            var last = History.Last();
            last.result = result;
            History.RemoveAt(History.Count - 1);
            History.Add(last);
        }

        public void ShowError(Exception ex)
        {
            Errors.Add(ex.Message);
        }

        public void ShowOutput(string output)
        {
            var last = History.Last();
            last.consoleOutput.Add(output);
        }

        public string GetInput()
        {
            return "";
        }

        public void ShowAST()
        {
            AST = Program.GenerateAST();
        }
    }
}
