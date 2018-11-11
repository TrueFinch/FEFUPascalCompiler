using System;

namespace FEFUPascalCompiler.Lexer
{
    [Serializable]
    public abstract class LexerException : ApplicationException
    {
        private int Line { get; }
        private int Column { get; }
        private string Lexeme { get; }
        public abstract override string Message { get; }

        protected LexerException(): base() {}
        protected LexerException(string message) : base(message) { }
        protected LexerException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected LexerException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        
        protected LexerException(int line, int column, string lexeme)
        {
            Line = line;
            Column = column;
            Lexeme = lexeme;
        }
    }
    
    [Serializable]
    public class UnexpectedSymbolException : LexerException
    {
        public UnexpectedSymbolException() {}
        public UnexpectedSymbolException(string message) : base(message)
        {
        }
        public UnexpectedSymbolException(int line, int column, string lexeme) : base(line, column, lexeme)
        {
            Message = string.Format("({0},{1}) Unexpected symbol: {2}",
                line.ToString(), column.ToString(), lexeme);
        }

        public override string Message { get; }
    }
       
    [Serializable]
    public class StrToIntConvertException : LexerException
    {
        public StrToIntConvertException()
        {
        }

        public StrToIntConvertException(string message) : base(message)
        {
            Message = message;
        }

        public override string Message { get; }
    }
}