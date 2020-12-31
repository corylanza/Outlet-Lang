using Outlet.Operands;
using Outlet.StandardLib;
using Outlet.TreeViewer;
using Outlet.Web.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Web.Pages
{
    public partial class Repl
    {

        public OutletEditor Editor;

        public List<(string code, List<string> consoleOutput, Operand? result)> History = new List<(string, List<string>, Operand?)>();

        private const string success = "text-success";
        private const string failure = "text-danger";

        private string CheckCssClass = "";

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

        public async Task RunOutlet()
        {
            var input = await Editor.GetValue();
            Errors.Clear();
            History.Add((input, new List<string>(), null));
            var result = Program.Run(Encoding.ASCII.GetBytes(input));
            var last = History.Last();
            last.result = result;
            History.RemoveAt(History.Count - 1);
            History.Add(last);
        }

        public async Task CheckOutlet()
        {
            var input = await Editor.GetValue();
            Errors.Clear();
            Program.Check(Encoding.ASCII.GetBytes(input));

            CheckCssClass = Errors.Count > 0 ? failure : success;
        }

        public void ShowError(Exception ex)
        {
            Errors.Add(ex.Message);
        }

        public void ShowOutput(string output)
        {
            var (code, consoleOutput, result) = History.Last();
            consoleOutput.Add(output);
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
