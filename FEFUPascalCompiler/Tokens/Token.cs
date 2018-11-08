using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FEFUPascalCompiler.Lexer;

namespace FEFUPascalCompiler.Tokens
{
    //TODO: refactor structure of Token class and it's children to make them usable in unit testing
    public abstract class Token
    {
        public int Line { get; }

        public int Column { get; }

        public TokenType TokenType { get; }

        public string Text { get; }

        public string StrValue { get; set; }

        protected Token(int line, int column, TokenType tokenType, string text)
        {
            Line = line;
            Column = column;
            TokenType = tokenType;
            Text = text;
        }
    }

    public class IntegerToken : Token
    {
        public int Value { get; }

        public IntegerToken(int line, int column, TokenType tokenType, string text)
            : base(line, column, tokenType, text)
        {
            int val;
            if (int.TryParse(text.ToCharArray(), out val))
            {
                //TODO: add cast from bin/oct/hex format to decimal
                Value = val;
            }
            else
            {
                throw new StrToIntConvertException(line, column, text);
                //TODO: throw exception of integer overflowing
            }

            StrValue = Value.ToString();
        }
    }

    public class DoubleToken : Token
    {
        private double _value;

        public double Value { get; }

        public DoubleToken(int line, int column, TokenType tokenType, string text)
            : base(line, column, tokenType, text)
        {
            double val;
            if (double.TryParse(text.ToCharArray(), out val))
            {
                _value = val;
            }
            else
            {
                //TODO: add throw exception of double overflowing
            }

            StrValue = _value.ToString();
        }
    }

    public class IdentToken : Token
    {
        public string Value { get; }

        public IdentToken(int line, int column, TokenType tokenType, string text)
            : base(line, column, tokenType, text)
        {
            Value = text.ToLower();
            StrValue = Value.ToString();
        }
    }

    public class KeyWordToken : Token
    {
        public string Value { get; }

        public KeyWordToken(int line, int column, TokenType tokenType, string text)
            : base(line, column, tokenType, text)
        {
            Value = text.ToLower();
            StrValue = Value.ToString();
        }
    }

    public class ArithmeticOperationToken : Token
    {
        public ArithmeticOperationToken(int line, int column, TokenType tokenType, string text)
            : base(line, column, tokenType, text)
        {
            StrValue = text;
        }
    }

    public class EOFToken : Token
    {
        public int Value { get; }
        public EOFToken(int line, int column, TokenType tokenType, string text)
            : base(line, column, tokenType, text)
        {
            Value = -1;
            StrValue = Value.ToString();
        }
    }

    public class SemiColonToken : Token
    {
        public SemiColonToken(int line, int column, TokenType tokenType, string text)
            : base(line, column, tokenType, text)
        {
            StrValue = ";";
        }
    }

    public class ColonToken : Token
    {
        public ColonToken(int line, int column, TokenType tokenType, string text)
            : base(line, column, tokenType, text)
        {
            StrValue = ":";
        }
    }

    public class AssignToken : Token
    {
        public AssignToken(int line, int column, TokenType tokenType, string text)
            : base(line, column, tokenType, text)
        {
            StrValue = text;
        }
    }

    public class DotToken : Token
    {
        public DotToken(int line, int column, TokenType tokenType, string text)
            : base(line, column, tokenType, text)
        {
            StrValue = ".";
        }
    }
}