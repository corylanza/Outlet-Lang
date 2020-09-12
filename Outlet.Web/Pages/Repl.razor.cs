using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Outlet.Web.Pages
{
    public partial class Repl
    {
        public string Input { get; set; } = "";

        public string Output { get; private set; } = "";

        private ReplOutletProgram Program { get; set; }

        public Repl()
        {
            Program = new ReplOutletProgram(GetInput, ShowOutput, ShowError);
        }

        protected List<string> Errors { get; set; } = new List<string>();

        public void RunOutlet()
        {
            Errors.Clear();
            Output = Program.Run(Encoding.ASCII.GetBytes(Input)).ToString();
        }

        public void ShowError(Exception ex)
        {
            Errors.Add(ex.Message);
        }

        public void ShowOutput(string output)
        {

        }

        public string GetInput()
        {
            return "";
        }
    }
}
