namespace Outlet.Tokens
{
    public abstract class Lexeme
    {
        public int Line { get; init; }
        public int Character { get; init; }

        public abstract Token InnerToken { get; }
    }


    public class Lexeme<T> : Lexeme where T : Token
    {

        public override T InnerToken { get; }

        public Lexeme(T token, int line, int character)
        {
            InnerToken = token;
            Line = line;
            Character = character;
        }
    }
}
