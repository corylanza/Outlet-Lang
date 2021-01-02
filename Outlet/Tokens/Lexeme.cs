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
    }
}
