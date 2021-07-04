using NUnit.Framework;
using Outlet.CLI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Outlet.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        private class CodeRunner
        {

            public List<string> Errors { get; init; }

            public int ErrorCount => Errors.Count;

            private ReplOutletProgram ReplProgram { get; set; }

            public CodeRunner()
            {
                Errors = new List<string>();
                ReplProgram = new ReplOutletProgram(Program.ConsoleInterface(OnException));
            }


            public string Run(string code) => ReplProgram.Run(Encoding.ASCII.GetBytes(code)).ToString();

            public string DumpErrors()
            {
                var output = string.Join('\n', Errors);
                Errors.Clear();
                return output;
            }

            private void OnException(Exception ex)
            {
                Errors.Add(ex.Message);
            }

        }

        //public static OutletProgramFile LoadProgramFromFile(string path)
        //{
        //    byte[] file = File.ReadAllBytes(path);
        //    byte[] bytes = file.Skip(3).ToArray();
        //    return new OutletProgramFile(bytes, Program.ConsoleInterface);
        //}

        [Test]
        public void TestRepl()
        {
            var code = new CodeRunner();
            code.Run("var a = 34;");
            Assert.AreEqual("34", code.Run("a"));
            code.Run("b");
            Assert.AreEqual(1, code.ErrorCount);
            code.Run("var b = a;");
            Assert.AreEqual(1, code.ErrorCount);
            Assert.AreEqual("34", code.Run("b"));
        }

        [Test]
        public void TestClasses()
        {
            var code = new CodeRunner();
            code.Run(@"
                class person {
                    static int Count = 0;
                    int Id = 0;
                    
                    person(int id) {
                        Id = id; Count += 1;
                    }
                }");
            Assert.AreEqual(0, code.ErrorCount, code.ErrorCount > 0 ? code.DumpErrors() : null);
            Assert.AreEqual("0", code.Run("person.Count"));
            code.Run("var p = person(5);");
            Assert.AreEqual("5", code.Run("p.Id"));
            Assert.AreEqual("1", code.Run("person.Count"));
            Assert.AreEqual("2", code.Run("person.Count = 2;"), code.ErrorCount > 0 ? code.DumpErrors() : null);
            Assert.AreEqual("3", code.Run("p.Id = 3;"));
        }

        [Test]
        public void TestOperators()
        {

            var code = new CodeRunner();
            Assert.AreEqual("7", code.Run("5+2"));
            Assert.AreEqual("10", code.Run("5*2"));
            Assert.AreEqual("5", code.Run("11 / 2"));
            Assert.AreEqual("10", code.Run("5 - -5"));
            Assert.AreEqual("10", code.Run("5 - -5"));
            Assert.AreEqual(0, code.ErrorCount, code.ErrorCount > 0 ? code.DumpErrors() : null);
        }

        [Test]
        public void TestFunctions()
        {
            var code = new CodeRunner();
            code.Run("int fac(int n) => n == 1 ? 1 : n * fac(n-1);");
            Assert.AreEqual(0, code.ErrorCount, code.ErrorCount > 0 ? code.DumpErrors() : null);
            Assert.AreEqual("120", code.Run("fac(5)"));
        }

        [Test]
        public void TestGenerics()
        {
            var code = new CodeRunner();
            code.Run("T noop[T](T t) => t;");
            Assert.AreEqual(0, code.ErrorCount, code.ErrorCount > 0 ? code.DumpErrors() : null);
            Assert.AreEqual("5", code.Run("noop(5)"));
            Assert.AreEqual("true", code.Run("noop(true)"));
        }
    }
}