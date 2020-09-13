﻿using Outlet.StandardLib;
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
            Output += Program.Run(Encoding.ASCII.GetBytes(Input)).ToString();
        }

        public void ShowError(Exception ex)
        {
            Errors.Add(ex.Message);
        }

        public void ShowOutput(string output)
        {
            Output += output;
        }

        public string GetInput()
        {
            return "";
        }
    }
}