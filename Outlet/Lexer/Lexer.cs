using Outlet.ForeignFunctions;
using Outlet.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CharGroup = Outlet.Lexer.Lexer.CharGroup;

namespace Outlet.Lexer
{
    public delegate Token? Tokenizer(string input);

    public abstract partial class Lexer : ILexer
    {

        private LexingRule Rule { get; set; }

        public Lexer(LexingRule rule)
        {
            Rule = rule;
        }


        public LinkedList<Lexeme> Scan(byte[] charStream, StandardError errorHandler) => new(Lex(new List<char>(charStream.Select(b => (char)b))));

        protected IEnumerable<Lexeme> Lex(List<char> tokens)
        {
            int linePos = 1;
            int charPos = 0;
            var output = new List<Lexeme>();

            while (tokens.Any())
            {
                try
                {
                    if (Rule.TestRule(tokens, new LexState(0, "", null), out LexState result))
                    {
                        charPos += result.Length;
                        linePos += tokens.Take(result.Length).Count(c => NewLine.Contains(c));
                        tokens.RemoveRange(0, result.Length);

                        if (result.Tokenizer is Tokenizer tokenizer && tokenizer(result.CharBuffer) is Token token)
                        {
                            output.Add(new Lexeme(token, linePos, charPos));
                        }
                        else
                        {
                            // Whitespace / comments
                        }
                    }
                    else
                    {
                        throw new SyntaxException($"unexpected {string.Concat(tokens)}", linePos, new Range(charPos, charPos + result.Length));
                    }
                } catch (LexerException e)
                {
                    throw new SyntaxException(e.Message, linePos, new Range(charPos, charPos));
                }
            }
            return output;
        }
    }

    public class OutletLexer : Lexer
    {
        private OutletLexer(LexingRule rule) : base(rule) { }

        static Token? DiscardTokenizer(string text) => null;
        static Token IntTokenizer(string text) => new IntLiteral(int.Parse(text));
        static Token HexTokenizer(string text) => new IntLiteral(int.Parse(text, System.Globalization.NumberStyles.HexNumber));
        static Token FloatTokenizer(string text) => new FloatLiteral(float.Parse(text));
        static Token StringTokenizer(string text) => new StringLiteral(text);
        static Token OpTokenizer(string text) => Symbol.Get(text);
        static Token SymbolTokenizer(string text) {
            if (text == "true") return new BoolLiteral(bool.Parse(text));
            if (text == "false") return new BoolLiteral(bool.Parse(text));
            if (text == "null") return new NullLiteral();
            return Symbol.ContainsKey(text) ? Symbol.Get(text) : new Identifier(text);
        }

        static SequenceRule OneOrMoreRule(LexingRule rule) => new SequenceRule(new(rule), new(new ZeroOrMoreRule(rule)));

        /// <summary>
        /// Accepts one or more of the valid characters and allows underscores as discarded characters for spacing  (e.g. 2_000 == 2000)
        /// </summary>
        static SequenceRule NumberSequence(CharGroup validNumbers, Tokenizer tokenizer) =>
            OneOrMoreRule(new OrRule(KeepChar(validNumbers, tokenizer), DiscardChar(Underscore, tokenizer)));

        static LexingRule Operator(params CharGroup[] chars) =>
            new SequenceRule(chars.Select(charGroup => new SequenceStep(new CharacterRule(charGroup, keep: true, tokenizer: OpTokenizer))).ToArray());

        static CharacterRule KeepChar(CharGroup chars, Tokenizer tokenizer) => new(chars, keep: true, tokenizer: tokenizer);
        static CharacterRule DiscardChar(CharGroup chars, Tokenizer tokenizer) => new(chars, keep: false, tokenizer: tokenizer);

        public static OutletLexer CreateOutletLexer()
        {
            return new OutletLexer
            (
                new OrRule
                (
                    // Whitespace
                    OneOrMoreRule(KeepChar(WhiteSpace, DiscardTokenizer)),
                    // Identifiers and Keywords
                    OneOrMoreRule(KeepChar(Letter, SymbolTokenizer)),
                    // Ints and Floats
                    new SequenceRule
                    (
                        new SequenceStep(NumberSequence(Number, IntTokenizer)),
                        new SequenceStep
                        (
                            new CharacterRule(Dot, keep: true),
                            errorMessage: "expected number following ."
                        ),
                        new SequenceStep(NumberSequence(Number, FloatTokenizer))
                    ),
                    // Hexadecimal
                    new SequenceRule
                    (
                        new SequenceStep(new CharacterRule(Zero, keep: false, tokenizer: IntTokenizer)),
                        new SequenceStep
                        (
                            new CharacterRule(X, keep: false),
                            errorMessage: "expected hexadecimal number following 0x"
                        ),
                        new SequenceStep(NumberSequence(Hex, HexTokenizer))
                    ),
                    // Strings
                    new SequenceRule
                    (
                        new SequenceStep
                        (
                            new CharacterRule(DoubleQuote, keep: false),
                            errorMessage: "hanging quotes"
                        ),
                        new SequenceStep
                        (
                            new ZeroOrMoreRule
                            (
                                new CharacterRule(Not(DoubleQuote), keep: true)
                            ),
                            errorMessage: "expected quotes to close string"
                        ),
                        new SequenceStep
                        (
                            new CharacterRule(DoubleQuote, keep: false, tokenizer: StringTokenizer)
                        )
                    ),
                    // Operators
                    Operator(Delimeter),
                    Operator(OneCharOp),
                    Operator(PreEquals, EqualsSign),
                    Operator(EqualsSign, RAngleBracket),
                    // ++
                    Operator(Plus, Plus),
                    // --
                    Operator(Minus, Minus),
                    // <<
                    Operator(LAngleBracket, LAngleBracket),
                    // >>
                    Operator(RAngleBracket, RAngleBracket),
                    // &&
                    Operator(Ampsersand, Ampsersand),
                    // ||
                    Operator(Pipe, Pipe)
                    //// Comments
                    //new SequenceRule
                    //(
                    //    new CharacterRule(Slash, keep: false, errorMessage: ""),
                    //    new CharacterRule(Slash, keep: false, errorMessage: ""),
                    //)
                )
            );
        }
    }

    public record LexState(int Length, string CharBuffer, Tokenizer? Tokenizer);

    public abstract class LexingRule
    {
        public abstract bool TestRule(IReadOnlyList<char> tokens, LexState starting, out LexState result);


    }

    public class CharacterRule : LexingRule
    {
        private CharGroup Characters { get; init; }
        private bool KeepCharacter { get; init; }
        private Tokenizer? Tokenizer { get; init; }

        public CharacterRule(CharGroup chars, bool keep)
        {
            Characters = chars;
            KeepCharacter = keep;
        }

        public CharacterRule(CharGroup chars, bool keep, Tokenizer tokenizer)
        {
            Characters = chars;
            KeepCharacter = keep;
            Tokenizer = tokenizer;
        }

        public override bool TestRule(IReadOnlyList<char> tokens, LexState starting, out LexState result)
        {
            char currentChar;
            if (tokens.Count > starting.Length && Characters.Contains(currentChar = tokens[starting.Length]))
            {
                var newLength = starting.Length + 1;
                var newBuffer = KeepCharacter ? string.Concat(starting.CharBuffer.Append(currentChar)) : starting.CharBuffer;

                result = starting with
                {
                    Length = newLength,
                    CharBuffer = newBuffer,
                    Tokenizer = Tokenizer
                };
                return true;
            } else
            {
                result = starting;
                return false;
            }
        }
    }

    public class ZeroOrMoreRule : LexingRule
    {
        private LexingRule Rule { get; init; }

        public ZeroOrMoreRule(LexingRule rule) => Rule = rule;

        public override bool TestRule(IReadOnlyList<char> tokens, LexState starting, out LexState result)
        {
            result = starting;
            while (Rule.TestRule(tokens, result, out LexState iterResult))
            {
                result = iterResult;
            }
            return true;
        }
    }

    public class OrRule : LexingRule
    {
        private List<LexingRule> Rules { get; init; }

        public OrRule(params LexingRule[] rules)
        {
            Rules = rules.ToList();
        }

        public override bool TestRule(IReadOnlyList<char> tokens, LexState starting, out LexState result)
        {
            IEnumerable<(bool passed, LexState result)> results = Rules.Select(rule =>
            {
                return (rule.TestRule(tokens, starting, out LexState result), result);
            });

            (bool passed, LexState res) = results.OrderByDescending(r => r.result.Length).First();
            result = res;
            return passed;
        }
    }

    public class SequenceStep
    {
        public string? ErrorMessage { get; set; }

        public LexingRule Rule { get; init; }


        public SequenceStep(LexingRule rule, string? errorMessage = null)
        {
            Rule = rule;
            ErrorMessage = errorMessage;
        }
    }

    public class SequenceRule : LexingRule
    {
        private List<SequenceStep> Steps { get; init; }

        public SequenceRule(params SequenceStep[] steps)
        {
            Steps = steps.ToList();
        }

        public override bool TestRule(IReadOnlyList<char> tokens, LexState starting, out LexState result)
        {
            LexState current = starting;
            string? lastErrorMessage = null;

            foreach(var step in Steps)
            {
                bool passedStep = step.Rule.TestRule(tokens, current, out LexState stepResult);

                if (!passedStep)
                {
                    if (string.IsNullOrEmpty(lastErrorMessage))
                    {
                        current = stepResult;
                        break;
                    } else
                    {
                        throw new LexerException(lastErrorMessage);
                    }
                }
                else
                {
                    current = stepResult;
                    lastErrorMessage = step.ErrorMessage;
                }
            }

            result = current;
            return current.Tokenizer is not null;
        }
    }
}
