using System;
using System.Collections.Generic;

namespace Outlet.Tokens
{
    public class Lexeme
    {
        public int Line { get; private init; }
        public int Character { get; private init; }
        public Token InnerToken { get; private init; }

        public Lexeme(Token token, int line, int character)
        {
            InnerToken = token;
            Line = line;
            Character = character;
        }

        public override string ToString() => $"{InnerToken.GetType().Name}:{InnerToken} line: {Line}, character: {Character}";

        public IEnumerable<(ConsoleColor, object s)> PrettyPrint()
        {
            yield return (ConsoleColor.Cyan, InnerToken.GetType().Name);
            yield return (ConsoleColor.White, ":");
            yield return (ConsoleColor.Red, InnerToken);
            yield return (ConsoleColor.White, "[");
            yield return (ConsoleColor.Magenta, Line);
            yield return (ConsoleColor.White, ":");
            yield return (ConsoleColor.Magenta, Character);
            yield return (ConsoleColor.White, "]");
        }
    }
}
