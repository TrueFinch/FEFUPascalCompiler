using System;
using System.Collections.Generic;
using System.Globalization;
using FEFUPascalCompiler.Lexer;

//all compilers are brothers, but you are adopted 

namespace FEFUPascalCompiler.Tokens
{
    public abstract class Token
    {
        public int Line { get; }

        public int Column { get; }

        public TokenType Type { get; }

        public string Lexeme { get; }

        public string Value { get; protected set; }

        protected Token(int line, int column, TokenType type, string lexeme)
        {
            Line = line;
            Column = column;
            Type = type;
            Lexeme = lexeme;
            Value = lexeme.ToLower();
        }

        public override string ToString()
        {
            return $"{(Line, Column),-10}| {Type,-20}| {Lexeme,-30}| ";
        }

        public static readonly Dictionary<TokenType, Func<int, int, string, Token>> TokenConstructors =
            new Dictionary<TokenType, Func<int, int, string, Token>>
            {
                {TokenType.IntegerNumber, (line, column, lexeme) => new IntegerNumberToken(line, column, lexeme)},
                {TokenType.FloatNumber, (line, column, lexeme) => new DoubleNumberToken(line, column, lexeme)},
                {TokenType.StringConst, (line, column, lexeme) => new StringConstToken(line, column, lexeme)},
                {TokenType.BinOperator, (line, column, lexeme) => new BinOperatorToken(line, column, lexeme)},
                {TokenType.AssignOperator, (line, column, lexeme) => new AssignToken(line, column, lexeme)},
                {TokenType.Separator, (line, column, lexeme) => new SeparatorToken(line, column, lexeme)},
                {TokenType.Ident, (line, column, lexeme) => new IdentToken(line, column, lexeme)},
                {TokenType.MultiLineComment, (line, column, lexeme) => new MultilineCommentToken(line, column, lexeme)},
                {
                    TokenType.SingleLineComment,
                    (line, column, lexeme) => new SingleLineCommentToken(line, column, lexeme)
                },
                {TokenType.Bracket, (line, column, lexeme) => new BracketToken(line, column, lexeme)},
                {TokenType.CharConst, (line, column, lexeme) => new CharConstToken(line, column, lexeme)},
            };
    }

    //TODO: change int64 ti int32
    public class IntegerNumberToken : Token
    {
        public IntegerNumberToken(int line, int column, string lexeme)
            : base(line, column, TokenType.IntegerNumber, lexeme)
        {
            NumberValue = ConvertToInteger(lexeme);
        }

        private int ConvertToInteger(string lexeme)
        {
            try
            {
                //if number not in decimal basis then lexeme[0] == % or $ or &
                if (basis[lexeme[0]] != 10)
                {
                    return Convert.ToInt32(lexeme.Substring(1), basis[lexeme[0]]);
                }

                return Convert.ToInt32(lexeme, basis[lexeme[0]]);
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
            return base.ToString() + $"{NumberValue,-30}" + '|';
        }

        public int NumberValue { get; }

        //base of the number system
        private static Dictionary<char, int> basis = new Dictionary<char, int>
        {
            {'%', 2}, {'&', 8}, {'$', 16},
            {'0', 10}, {'1', 10}, {'2', 10}, {'3', 10}, {'4', 10}, {'5', 10}, {'6', 10}, {'7', 10}, {'8', 10},
            {'9', 10},
        };
    }

    //TODO: change double to float
    public class DoubleNumberToken : Token
    {
        public DoubleNumberToken(int line, int column, string lexeme)
            : base(line, column, TokenType.FloatNumber, lexeme)
        {
            NumberValue = LexemeToDouble(lexeme);
        }

        private double LexemeToDouble(string lexeme)
        {
            try
            {
                NumberFormatInfo provider = new NumberFormatInfo {NumberDecimalSeparator = "."};
                return Convert.ToDouble(lexeme, provider);
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
            return base.ToString() + $"{NumberValue,-30}" + '|';
        }

        public double NumberValue { get; }
    }

    public class IdentToken : Token
    {
        public IdentToken(int line, int column, string lexeme)
            : base(line, column, Dictionaries.KeyWords.ContainsKey(lexeme.ToLower())
                    ? Dictionaries.KeyWords[lexeme.ToLower()]
                    : TokenType.Ident,
                lexeme)
        {
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value,-30}" + '|';
        }
    }

    public class BinOperatorToken : Token
    {
        public BinOperatorToken(int line, int column, string lexeme)
            : base(line, column, Dictionaries.LexemeToTokenType[lexeme.ToLower()], lexeme)
        {
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value,-30}" + '|';
        }
    }

    public class AssignToken : Token
    {
        public AssignToken(int line, int column, string lexeme)
            : base(line, column, Dictionaries.LexemeToTokenType[lexeme.ToLower()], lexeme)
        {
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value,-30}" + '|';
        }
    }

    public class SeparatorToken : Token
    {
        public SeparatorToken(int line, int column, string lexeme)
            : base(line, column, Dictionaries.LexemeToTokenType[lexeme.ToLower()], lexeme)
        {
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value,-30}" + '|';
        }
    }

    public class StringConstToken : Token
    {
        private string SetValue(string lexeme)
        {
            lexeme = lexeme[0] == '\'' ? lexeme.Substring(1) : lexeme;
            lexeme = lexeme[lexeme.Length - 1] == '\'' ? lexeme.Substring(0, lexeme.Length - 1) : lexeme;

            lexeme = lexeme.Replace("\'", "");
            lexeme = lexeme.Replace("''", "'");


            for (int nextSharpIndex = lexeme.IndexOf("#", StringComparison.Ordinal);
                nextSharpIndex >= 0;
                nextSharpIndex = lexeme.IndexOf("#", StringComparison.Ordinal))
            {
                string code = "";
                for (int i = nextSharpIndex + 1; i < lexeme.Length && "0123456789".Contains(lexeme[i]); ++i)
                {
                    code += lexeme[i];
                }

                lexeme = lexeme.Replace(String.Concat("#", code), ((char) Convert.ToUInt32(code, 10)).ToString());
            }

            lexeme = lexeme.Replace("''", "'");

            return lexeme;
        }

        public StringConstToken(int line, int column, string lexeme)
            : base(line, column, TokenType.StringConst, lexeme)
        {
            Value = SetValue(lexeme);
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value,-30}" + '|';
        }
    }

    //TODO: add charconst token realization and don't forget about lexer part and tests for this token
    public class CharConstToken : Token
    {
        public CharConstToken(int line, int column, string lexeme) : base(line, column, TokenType.CharConst, lexeme)
        {
            Value = SetValue(lexeme);
        }

        private string SetValue(string lexeme)
        {
            if (lexeme[0] == '\'')
            {
                return lexeme.Substring(1, 1);
            }

            return ((char) Convert.ToUInt32(lexeme.Substring(1))).ToString();
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value,-30}" + '|';
        }
    }

    public class MultilineCommentToken : Token
    {
        public MultilineCommentToken(int line, int column, string lexeme)
            : base(line, column, TokenType.MultiLineComment, lexeme)
        {
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value,-30}" + '|';
        }
    }

    public class SingleLineCommentToken : Token
    {
        public SingleLineCommentToken(int line, int column, string lexeme)
            : base(line, column, TokenType.SingleLineComment, lexeme)
        {
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value,-30}" + '|';
        }
    }

    public class BracketToken : Token
    {
        public BracketToken(int line, int column, string lexeme)
            : base(line, column, Dictionaries.LexemeToTokenType[lexeme.ToLower()], lexeme)
        {
        }

        public override string ToString()
        {
            return base.ToString() + $"{Value,-30}" + '|';
        }
    }

    public class BooleanToken : Token
    {
        public BooleanToken(int line, int column, string lexeme) : base(line, column,
            Dictionaries.KeyWords[lexeme.ToLower()], lexeme)
        {
            Value = lexeme.ToLower() == "true";
        }

        public new bool Value { get; }
    }
}