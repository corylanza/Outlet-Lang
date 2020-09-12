using NUnit.Framework;
using System;
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
        public void Test1()
        {
            OutletProgramFile program = LoadProgramFromFile("helloworld.outlet");
            var res = program.Run();
            Assert.Pass();
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
    }
}