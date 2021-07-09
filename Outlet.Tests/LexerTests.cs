using NUnit.Framework;
using Outlet.Lexer;
using Outlet.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tests
{
    public class LexerTests
    {
        private ILexer Lexer { get; set; }

        [SetUp]
        public void SetUp()
        {
            Lexer = OutletLexer.CreateOutletLexer();
        }

        [Test]
        public void TestTokens()
        {
            void ErrHandler(Exception e)
            {
                Assert.Fail(e.Message);
            }

            foreach(var (key, token) in Token.AllTokens)
            {
                var byteArray = key.Select(c => (byte)c).ToArray();
                var output = Lexer.Scan(byteArray, ErrHandler);

                var outputTokenString = string.Join(',', output.Select(lexeme => lexeme.InnerToken.ToString()));

                var errMessage = $"expected '{key}' but was '{outputTokenString}'";
                Assert.AreEqual(1, output.Count, errMessage);
                Assert.AreEqual(token, output.First().InnerToken, errMessage);
            }
        }

        [Test]
        public void TestNumbers()
        {
            void ErrHandler(Exception e)
            {
                Assert.Fail(e.Message);
            }

            
        }

        [Test]
        public void TestStrings()
        {
            void ErrHandler(Exception e)
            {
                Assert.Fail(e.Message);
            }


        }
    }
}
