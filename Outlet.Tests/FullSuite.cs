using NUnit.Framework;
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

        public static OutletProgramFile LoadProgramFromFile(string path)
        {
            byte[] file = File.ReadAllBytes(path);
            byte[] bytes = file.Skip(3).ToArray();
            return new OutletProgramFile(bytes, () => "", s => { }, ex => { });
        }

        [Test]
        public void TestRepl()
        {
            int errorCount = 0;
            ReplOutletProgram program = new ReplOutletProgram(() => Console.ReadLine(), text => Console.WriteLine(text), OnException);
            string RunCode(string code) => program.Run(Encoding.ASCII.GetBytes(code)).ToString();
            void OnException(Exception ex)
            {
                errorCount++;
            }
            RunCode("var a = 34;");
            Assert.AreEqual("34", RunCode("a"));
            RunCode("b");
            Assert.AreEqual(1, errorCount);
            RunCode("var b = a;");
            Assert.AreEqual(1, errorCount);
            Assert.AreEqual("34", RunCode("b"));
        }

        [Test]
        public void TestClasses()
        {
            List<string> errors = new List<string>();
            ReplOutletProgram program = new ReplOutletProgram(() => Console.ReadLine(), text => Console.WriteLine(text), OnException);
            string RunCode(string code) => program.Run(Encoding.ASCII.GetBytes(code)).ToString();
            void OnException(Exception ex)
            {
                errors.Add(ex.Message);
            }
            RunCode("class person { static int Count = 0; int Id = 0; person(int id) { Id = id; Count += 1; } }");
            Assert.AreEqual(0, errors.Count, errors.Count > 0 ? errors.Last() : null);
            errors.Clear();
            Assert.AreEqual("0", RunCode("person.Count"));
            RunCode("var p = person(5);");
            Assert.AreEqual("5", RunCode("p.Id"));
            Assert.AreEqual("1", RunCode("person.Count"));
            Assert.AreEqual("2", RunCode("person.Count = 2;"), errors.Count > 0 ? errors.Last() : null);
            errors.Clear();
            Assert.AreEqual("3", RunCode("p.Id = 3;"));
        }
    }
}