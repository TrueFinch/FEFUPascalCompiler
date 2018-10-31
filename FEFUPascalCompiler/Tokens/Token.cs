using System.Linq.Expressions;

namespace FEFUPascalCompiler.Tokens
{
    public abstract class Token : IToken
    {
        private int _line;
        private int _column;
        private TokenType _tokenType;
        private string _text;

        public virtual int Line { get; set; }

        public virtual int Column { get; set; }

        public virtual TokenType TokenType { get; set; }

        public string Text { get; set; }

        public Token(int line, int column, TokenType tokenType, string text)
        {
            _line = line;
            _column = column;
            _tokenType = tokenType;
            _text = text;
        }
    }

    public class IntegerValueToken : Token
    {
        public IntegerValueToken(int line, int column, TokenType tokenType, string text)
            : base(line, column, tokenType, text)
        {
        }
    }
}