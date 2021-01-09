using Outlet.StandardLib;
using Outlet.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Lexer
{
    public abstract partial class Lexer : ILexer
    {
        private IEnumerable<LexingRule> Rules { get; set; }

        public Lexer(params LexingRule[] rules)
        {
            Rules = rules;
        }


        public LinkedList<Lexeme> Scan(byte[] charStream, StandardError errorHandler) => 
            new LinkedList<Lexeme>(Lex(new LinkedList<char>(charStream.Select(b => (char) b))));

        protected IEnumerable<Lexeme> Lex(LinkedList<char> tokens)
        {
            int linePos = 1;
            int charPos = 0;


            while(tokens.Any())
            {
                if(NewLine(tokens.First())) linePos++;

                foreach (LexingRule rule in Rules)
                {
                    if (rule.TestRule(tokens, out Token? result))
                    {
                        yield return new Lexeme(result, linePos, charPos);
                        continue;
                    }
                }
            }
        }
    }

    public class OutletLexer : Lexer
    {
        private OutletLexer(params LexingRule[] rules) : base(rules) { }

        public static OutletLexer CreateOutletLexer() => new OutletLexer(
            new OneOrMoreRule(s => new Identifier(s), Letter),
            new OneOrMoreRule(s => new IntLiteral(s), Number),
            new SequenceRule(s => new StringLiteral(s.Trim('"')), DoubleQuote, Not(DoubleQuote), DoubleQuote),
            new SequenceRule(s => new FloatLiteral(s), Number, Dot, Number),
            new SequenceRule(s => Token.Get(s), PreEquals, EqualsSign),
            new SequenceRule(s => Token.Get(s), PreEquals)
        );
    }

    public abstract class LexingRule
    {
        protected Tokenizer Tokenize { get; private init; }
        protected IEnumerable<CharGroup> AllowedChars { get; private init; }

        protected LexingRule(Tokenizer func, params CharGroup[] chars)
        {
            Tokenize = func;
            AllowedChars = chars;
        }

        public abstract bool TestRule(LinkedList<char> tokens, [NotNullWhen(true)] out Token? result);
    }

    public class OneOrMoreRule : LexingRule
    {
        public OneOrMoreRule(Tokenizer func, params CharGroup[] chars) : base(func, chars) { }

        public override bool TestRule(LinkedList<char> tokens, [NotNullWhen(true)] out Token? result)
        {
            result = null;
            string output = "";

            while (tokens.Any() && AllowedChars.Any(charGroup => charGroup(tokens.First())))
            {
                output += tokens.First();
                tokens.RemoveFirst();
            }

            if (string.IsNullOrEmpty(output)) return false;
            result = Tokenize(output);
            return true;
        }
    }

    public class SequenceRule : LexingRule
    {
        public SequenceRule(Tokenizer func, params CharGroup[] chars) : base(func, chars) { }

        public override bool TestRule(LinkedList<char> tokens, [NotNullWhen(true)] out Token? result)
        {
            result = null;
            string output = "";

            while (tokens.Any() && AllowedChars.Any(charGroup => charGroup(tokens.First())))
            {
                output += tokens.First();
                tokens.RemoveFirst();
            }

            if (string.IsNullOrEmpty(output)) return false;
            result = Tokenize(output);
            return true;
        }
    }

    public delegate bool CharGroup(char c);

    public delegate Token Tokenizer(string input);
}
