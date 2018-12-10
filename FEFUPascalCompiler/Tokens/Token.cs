using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Lexer;

namespace FEFUPascalCompiler.Tokens
{
    public abstract class Token
    {
        public int Line { get; }

        public int Column { get; }

        public TokenType TokenType { get; protected set; }

        public string Lexeme { get; }

        protected Token(int line, int column, string lexeme)
        {
            Line = line;
            Column = column;
            Lexeme = lexeme;
        }

        protected Token(int line, int column, TokenType tokenType, string lexeme)
        {
            Line = line;
            Column = column;
            TokenType = tokenType;
            Lexeme = lexeme;
        }

        public override string ToString()
        {
            return $"{(Line, Column), -10}| {TokenType, -20}| {Lexeme, -30}| ";
        }

        public static readonly Dictionary<TokenType, Func<int, int, string, Token>> TokenConstructors =
            new Dictionary<TokenType, Func<int, int, string, Token>>
            {
                {TokenType.IntegerNumber, (line, column, lexeme) => new IntegerNumberToken(line, column, lexeme)},
                {TokenType.DoubleNumber, (line, column, lexeme) => new DoubleNumberToken(line, column, lexeme)},
                {TokenType.StringConst, (line, column, lexeme) => new StringConstToken(line, column, lexeme)},
                {TokenType.BinOperator, (line, column, lexeme) => new BinOperatorToken(line, column, lexeme)},
                {TokenType.AssignOperator, (line, column, lexeme) => new AssignToken(line, column, lexeme)},
                {TokenType.Separator, (line, column, lexeme) => new SeparatorToken(line, column, lexeme)},
                {TokenType.Ident, (line, column, lexeme) => new IdentToken(line, column, lexeme)},
                {TokenType.MultiLineComment, (line, column, lexeme) => new MultilineCommentToken(line, column, lexeme)},
                {TokenType.SingleLineComment, (line, column, lexeme) => new SingleLineCommentToken(line, column, lexeme)},
                {TokenType.Bracket, (line, column, lexeme) => new BracketToken(line, column, lexeme)}
            };
    }

    public class IntegerNumberToken : Token
    {
        public IntegerNumberToken(int line, int column, string lexeme)
            : base(line, column, TokenType.IntegerNumber, lexeme)
        {
            Value = ParseAndCheckLexeme(lexeme);
        }

        private long ParseAndCheckLexeme(string lexeme)
        {
            try
            {
                if (lexeme[0] == '-')
                {
                    return -1 * Convert.ToInt64(lexeme.Substring(2), basis[lexeme[1]]);
                }

                //if number not in decimal basis then lexeme[0] == % or $ or &
                if (basis[lexeme[0]] != 10)
                {
                    return Convert.ToInt64(lexeme.Substring(1), basis[lexeme[0]]);
                }

                return Convert.ToInt64(lexeme, basis[lexeme[0]]);
            }
            catch (FormatException exception)
            {
                throw new StrToIntConvertException($"Error on ({Line},{Column}) in {lexeme}: {exception.Message}");
            }
            catch (OverflowException exception)
            {
                throw new StrToIntConvertException($"Error on ({Line},{Column}) in {lexeme}: {exception.Message}");
            }
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value, -30}" + '|';
        }

        public long Value { get; }

        //base of the number system
        private static Dictionary<char, int> basis = new Dictionary<char, int>
        {
            {'%', 2}, {'&', 8}, {'$', 16},
            {'0', 10}, {'1', 10}, {'2', 10}, {'3', 10}, {'4', 10}, {'5', 10}, {'6', 10}, {'7', 10}, {'8', 10},
            {'9', 10}
        };
    }

    public class DoubleNumberToken : Token
    {
        public DoubleNumberToken(int line, int column, string lexeme)
            : base(line, column, TokenType.DoubleNumber, lexeme)
        {
            Value = LexemeToDouble(lexeme);
        }

        private double LexemeToDouble(string lexeme)
        {
            try
            {
                return Convert.ToDouble(lexeme);
            }
            catch (FormatException exception)
            {
                throw new StrToIntConvertException($"Error on ({Line},{Column}) in {lexeme}: {exception.Message}");
            }
            catch (OverflowException exception)
            {
                throw new StrToIntConvertException($"Error on ({Line},{Column}) in {lexeme}: {exception.Message}");
            }
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value, -30}" + '|';
        }

        public double Value { get; }
    }

    public class IdentToken : Token
    {
        public IdentToken(int line, int column, string lexeme)
            : base(line, column, Dictionaries.KeyWords.ContainsKey(lexeme.ToLower())
                    ? Dictionaries.KeyWords[lexeme.ToLower()]
                    : TokenType.Ident,
                lexeme)
        {
            Value = lexeme.ToLower();
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value, -30}" + '|';
        }

        public string Value { get; }
    }

    public class BinOperatorToken : Token
    {
        public BinOperatorToken(int line, int column, string lexeme)
            : base(line, column, Dictionaries.LexemeToTokenType[lexeme.ToLower()], lexeme)
        {
            Value = lexeme.ToLower();
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value, -30}" + '|';
        }

        public string Value { get; }
    }

    public class AssignToken : Token
    {
        public AssignToken(int line, int column, string lexeme)
            : base(line, column, Dictionaries.LexemeToTokenType[lexeme.ToLower()], lexeme)
        {
            Value = lexeme.ToLower();
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value, -30}" + '|';
        }

        public string Value { get; }
    }

    public class SeparatorToken : Token
    {
        public SeparatorToken(int line, int column, string lexeme)
            : base(line, column, Dictionaries.LexemeToTokenType[lexeme.ToLower()], lexeme)
        {
            Value = lexeme.ToLower();
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value, -30}" + '|';
        }

        public string Value { get; }
    }

    public class StringConstToken : Token
    {
        public StringConstToken(int line, int column, string lexeme)
            : base(line, column, TokenType.StringConst, lexeme)
        {
            Value = lexeme;
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value, -30}" + '|';
        }

        public string Value { get; }
    }

    public class MultilineCommentToken : Token
    {
        public MultilineCommentToken(int line, int column, string lexeme)
            : base(line, column, TokenType.MultiLineComment, lexeme)
        {
            Value = lexeme;
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value, -30}" + '|';
        }

        public string Value { get; }
    }
    
    public class SingleLineCommentToken : Token
    {
        public SingleLineCommentToken(int line, int column, string lexeme)
            : base(line, column, TokenType.SingleLineComment, lexeme)
        {
            Value = lexeme;
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value, -30}" + '|';
        }

        public string Value { get; }
    }
    
    public class BracketToken: Token{
        public BracketToken(int line, int column, string lexeme)
            : base(line, column, Dictionaries.LexemeToTokenType[lexeme.ToLower()], lexeme)
        {
            Value = lexeme.ToLower();
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value, -30}" + '|';
        }

        public string Value { get; }
    }
}