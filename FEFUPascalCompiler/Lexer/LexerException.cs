using System;

namespace FEFUPascalCompiler.Lexer
{
    public abstract class LexerException : Exception
    {
        public int Line { get; }
        public int Column { get; }
        public string Lexeme { get; }
        public override abstract string Message { get; }

        protected LexerException(int line, int column, string lexeme)
        {
            Line = line;
            Column = column;
            Lexeme = lexeme;
        }
    }

    public class UnexpectedSymbol : LexerException
    {
        public UnexpectedSymbol(int line, int column, string lexeme) : base(line, column, lexeme)
        {
            Message = string.Format("({0},{1}) Unexpected symbol: {2}",
                line.ToString(),
                column.ToString(),
                lexeme);
        }

        public override string Message { get; }
    }
}