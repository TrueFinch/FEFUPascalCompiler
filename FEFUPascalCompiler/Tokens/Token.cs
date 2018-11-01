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

        protected Token(int line, int column, TokenType tokenType, string text)
        {
            _line = line;
            _column = column;
            _tokenType = tokenType;
            _text = text;
        }
    }

    public class IntegerValueToken : Token
    {
        private int _value;

        public int Value { get; set; }

        public IntegerValueToken(int line, int column, TokenType tokenType, int value, string text)
            : base(line, column, tokenType, text)
        {
            _value = value;
        }
    }

    public class DoubleValueToken : Token
    {
        private double _value;

        public double Value { get; set; }
        
        public DoubleValueToken(int line, int column, TokenType tokenType, double value, string text)
            : base(line, column, tokenType, text)
        {
            _value = value;
        }
    }

    public class IdentValueToken : Token
    {
        public IdentValueToken(int line, int column, TokenType tokenType, string text)
            : base(line, column, tokenType, text)
        {
        }
    }
}