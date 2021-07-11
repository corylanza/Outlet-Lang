using Outlet.StandardLib;
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
    public abstract partial class Lexer : ILexer
    {
        private LexingRule Rule { get; set; }

        public Lexer(LexingRule rule)
        {
            Rule = rule;
        }


        public LinkedList<Lexeme> Scan(byte[] charStream, StandardError errorHandler) =>
            new LinkedList<Lexeme>(Lex(new List<char>(charStream.Select(b => (char)b))));

        protected IEnumerable<Lexeme> Lex(List<char> tokens)
        {
            int linePos = 1;
            int charPos = 0;

            while (tokens.Any())
            {
                if (Rule.TestRule(tokens, new LexState(0, "", null, null), out LexState result))
                {
                    charPos += result.Length;
                    linePos += tokens.Take(result.Length).Count(c => NewLine.Contains(c));
                    tokens.RemoveRange(0, result.Length);

                    if(result.Tokenizer is Tokenizer tokenizer && tokenizer(result.CharBuffer) is Token token)
                    {
                        yield return new Lexeme(token, linePos, charPos);
                    } else
                    {
                        // Whitespace / comments
                    }
                } else
                {
                    throw new SyntaxException(result.ErrorMessage ?? $"unexpected {string.Concat(tokens)}", linePos, new Range(charPos, charPos + result.Length));
                }
            }
        }
    }

    public class OutletLexer : Lexer
    {
        private OutletLexer(LexingRule rule) : base(rule) { }

        public static OutletLexer CreateOutletLexer()
        {
            Tokenizer DiscardTokenizer = s => null;
            Tokenizer IntTokenizer = s => new IntLiteral(int.Parse(s));
            Tokenizer HexTokenizer = s => new IntLiteral(int.Parse(s, System.Globalization.NumberStyles.HexNumber));
            Tokenizer FloatTokenizer = s => new FloatLiteral(float.Parse(s));
            Token SymbolTokenizer(string text)
            {
                if (text == "true") return new BoolLiteral(bool.Parse(text));
                if (text == "false") return new BoolLiteral(bool.Parse(text));
                if (text == "null") return new NullLiteral();
                return Symbol.ContainsKey(text) ? Symbol.Get(text) : new Identifier(text);
            }
            Tokenizer StringTokenizer = s => new StringLiteral(s);
            Tokenizer OpTokenizer = s => Symbol.Get(s);

            LexingRule Operator(params CharGroup[] chars) => new SequenceRule(chars.Select(charGroup => new CharacterRule(charGroup, keep: true, tokenizer: OpTokenizer)).ToArray());

            return new OutletLexer
            (
                new OrRule
                (
                    // Whitespace
                    new OneOrMoreRule
                    (
                        new CharacterRule(WhiteSpace, keep: false, tokenizer: DiscardTokenizer)
                    ),
                    // Identifiers and Keywords
                    new OneOrMoreRule
                    (
                        new CharacterRule(Letter, keep: true, tokenizer: SymbolTokenizer)
                    ),
                    // Ints and Floats
                    new SequenceRule
                    (
                        new OneOrMoreRule
                        (
                            new OrRule
                            (
                                new CharacterRule(Number, keep: true, tokenizer: IntTokenizer),
                                new CharacterRule(Underscore, keep: false, tokenizer: IntTokenizer)
                            )
                        ),
                        new CharacterRule(Dot, keep: true, errorMessage: "expected a number following decimal point"),
                        new OneOrMoreRule
                        (
                            new OrRule
                            (
                                new CharacterRule(Number, keep: true, tokenizer: FloatTokenizer),
                                new CharacterRule(Underscore, keep: false, tokenizer: FloatTokenizer)
                            )
                        )
                    ),
                    // Hexadecimal
                    new SequenceRule
                    (
                        new CharacterRule(Zero, keep: false, tokenizer: IntTokenizer),
                        new CharacterRule(X, keep: false, errorMessage: "expected hexadecimal number following 0x"),
                        new OneOrMoreRule
                        (
                            new OrRule
                            (
                                new CharacterRule(Underscore, keep: false, tokenizer: HexTokenizer),
                                new CharacterRule(Hex, keep: true, tokenizer: HexTokenizer)
                            )
                        )
                    ),
                    // Strings
                    new SequenceRule
                    (
                        new CharacterRule(DoubleQuote, keep: false, errorMessage: "hanging quotes"),
                        new ZeroOrMoreRule
                        (
                            new CharacterRule(Not(DoubleQuote), keep: true, errorMessage: "expected quotes to close string")
                        ),
                        new CharacterRule(DoubleQuote, keep: false, tokenizer: StringTokenizer)
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

    public record LexState(int Length, string CharBuffer, Tokenizer? Tokenizer, string? ErrorMessage);

    public abstract class LexingRule
    {
        public abstract bool TestRule(IReadOnlyList<char> tokens, LexState starting, out LexState result);
    }

    public class CharacterRule : LexingRule
    {
        private CharGroup Characters { get; init; }
        private bool KeepCharacter { get; init; }

        private string? ErrorMessage { get; init; }
        private Tokenizer? Tokenizer { get; init; }


        public CharacterRule(CharGroup chars, bool keep, string errorMessage)
        {
            Characters = chars;
            KeepCharacter = keep;
            ErrorMessage = errorMessage;
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
                    ErrorMessage = ErrorMessage,
                    Tokenizer = Tokenizer
                };
                return true;
            } else
            {
                result = starting with
                {
                    ErrorMessage = ErrorMessage
                };
                return false;
            }
        }
    }

    public class ZeroOrMoreRule : LexingRule
    {
        private LexingRule Rule { get; init; }

        public ZeroOrMoreRule(LexingRule rule)
        {
            Rule = rule;
        }

        public override bool TestRule(IReadOnlyList<char> tokens, LexState starting, out LexState result)
        {
            LexState current = starting;

            while (Rule.TestRule(tokens, current, out LexState? iterResult))
            {
                current = iterResult;
            }

            result = current;
            return true;
        }
    }

    public class OneOrMoreRule : LexingRule
    {
        private LexingRule Rule { get; init; }

        public OneOrMoreRule(LexingRule rule)
        {
            Rule = rule;
        }

        public override bool TestRule(IReadOnlyList<char> tokens, LexState starting, out LexState result)
        {
            LexState current = starting;

            int i = 0;
            while(Rule.TestRule(tokens, current, out LexState? iterResult))
            {
                current = iterResult;
                i++;
            }

            result = current;
            return i > 0 && current.Tokenizer is not null;
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

    public class SequenceRule : LexingRule
    {
        private List<LexingRule> Steps { get; init; }

        public SequenceRule(params LexingRule[] steps)
        {
            Steps = steps.ToList();
        }

        public override bool TestRule(IReadOnlyList<char> tokens, LexState starting, out LexState result)
        {
            LexState current = starting;

            foreach(var step in Steps)
            {
                bool passedStep = step.TestRule(tokens, current, out LexState stepResult);

                if (!passedStep)
                {
                    if(string.IsNullOrEmpty(current.ErrorMessage))
                    {
                        current = stepResult;
                    }
                    break;
                }
                else
                {
                    current = stepResult;
                }
            }

            result = current;
            return current.Tokenizer is not null;
        }
    }

    public delegate Token? Tokenizer(string input);
}
