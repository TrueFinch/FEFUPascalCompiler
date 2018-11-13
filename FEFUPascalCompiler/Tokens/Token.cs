using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using FEFUPascalCompiler.Lexer;

namespace FEFUPascalCompiler.Tokens
{
    //TODO: refactor structure of Token class and it's children to make them usable in unit testing
    public abstract class Token
    {
        public int Line { get; }

        public int Column { get; }

        public TokenType TokenType { get; protected set; }

        public string Text { get; }

        public string StrValue { get; protected set; }

        protected Token(int line, int column, string text)
        {
            Line = line;
            Column = column;
            Text = text;
        }

        protected Token(int line, int column, TokenType tokenType, string text)
        {
            Line = line;
            Column = column;
            TokenType = tokenType;
            Text = text;
        }

        public static Token GetToken(int line, int column, TokenType tokenType, string text)
        {
            if (tokenType == TokenType.TYPE_INTEGER)
            {
                return new IntegerToken(line, column, text);
            }

            if (tokenType == TokenType.TYPE_DOUBLE)
            {
                return new DoubleToken(line, column, text);
            }

            if (tokenType == TokenType.TYPE_STRING)
            {
                return new StringConstToken(line, column, text);
            }

            if ((tokenType == TokenType.BIN_ARITHM_OPERATOR)
                || (tokenType == TokenType.BIN_ARITHM_SUM)
                || (tokenType == TokenType.BIN_ARITHM_DIF)
                || (tokenType == TokenType.BIN_ARITHM_MUL)
                || (tokenType == TokenType.BIN_ARITHM_DIV)
                || (tokenType == TokenType.BIN_ARITHM_POW))
            {
                return new BinArithmeticOperationToken(line, column, text);
            }

            if ((tokenType == TokenType.ASSIGNMENT)
                || (tokenType == TokenType.SMP_ASSIGN)
                || (tokenType == TokenType.SUM_ASSIGN)
                || (tokenType == TokenType.DIF_ASSIGN)
                || (tokenType == TokenType.MUL_ASSIGN)
                || (tokenType == TokenType.DIV_ASSIGN))
            {
                return new AssignToken(line, column, text);
            }

            if ((tokenType == TokenType.IDENT))
            {
                return new IdentToken(line, column, text);
            }

            if ((tokenType == TokenType.SEPARATOR)
                || (tokenType == TokenType.DOT)
                || (tokenType == TokenType.SEMICOLON)
                || (tokenType == TokenType.COLON)
                || (tokenType == TokenType.COMMA))
            {
                return new SeparatorToken(line, column, text);
            }

            throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null);
        }
    }

    public class IntegerToken : Token
    {
        //base if the number system
        private static Dictionary<char, int> basis = new Dictionary<char, int>
        {
            {'%', 2}, {'&', 8}, {'$', 16},
            {'0', 10}, {'1', 10}, {'2', 10}, {'3', 10}, {'4', 10}, {'5', 10}, {'6', 10}, {'7', 10}, {'8', 10}, {'9', 10}
        };

        private string ParseAndCheckLexeme(string lexeme)
        {
            string str;
            try
            {
                if (lexeme[0] == '-')
                {
                    str = (-1 * Convert.ToInt64(lexeme.Substring(2), basis[lexeme[1]])).ToString();
                }
                else
                {
                    //if number not in decimal basis then lexeme[0] == % or $ or &
                    if (basis[lexeme[0]] != 10)
                    {
                        str = Convert.ToInt64(lexeme.Substring(1), basis[lexeme[0]]).ToString();
                    }
                    else
                    {
                        str = Convert.ToInt64(lexeme, basis[lexeme[0]]).ToString();
                    }
                }
            }
            catch (FormatException exception)
            {
                throw new StrToIntConvertException($"Error on ({Line},{Column}) in {lexeme}: {exception.Message}");
            }
            catch (OverflowException exception)
            {
                throw new StrToIntConvertException($"Error on ({Line},{Column}) in {lexeme}: {exception.Message}");
            }

            return str;
        }

        public IntegerToken(int line, int column, string text)
            : base(line, column, text)
        {
            TokenType = TokenType.TYPE_INTEGER;
            StrValue = ParseAndCheckLexeme(text);
        }
    }

    public class DoubleToken : Token
    {
        private void CheckLexeme(string lexeme)
        {
            try
            {
                Convert.ToDouble(lexeme);
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

        public DoubleToken(int line, int column, string text)
            : base(line, column, text)
        {
            TokenType = TokenType.TYPE_DOUBLE;
            CheckLexeme(text);
            StrValue = text;
        }
    }

    public class IdentToken : Token
    {
        public IdentToken(int line, int column, string text)
            : base(line, column, text)
        {
            if (Dictionaries.KeyWords.ContainsKey(text.ToLower()))
            {
                TokenType = Dictionaries.KeyWords[text.ToLower()];
            }
            else
            {
                TokenType = TokenType.IDENT;
            }

            StrValue = text.ToLower();
        }
    }

    public class BinArithmeticOperationToken : Token
    {
        public BinArithmeticOperationToken(int line, int column, string text)
            : base(line, column, text)
        {
            TokenType = Dictionaries.BinArithmeticOperators[text];
            StrValue = text;
        }
    }

    public class AssignToken : Token
    {
        public AssignToken(int line, int column, string text)
            : base(line, column, text)
        {
            TokenType = Dictionaries.Assigns[text];
            StrValue = text;
        }
    }

    public class SeparatorToken : Token
    {
        public SeparatorToken(int line, int column, string text) : base(line, column, text)
        {
            StrValue = text;
            TokenType = Dictionaries.Separators[text];
        }
    }

    public class StringConstToken : Token
    {
        public StringConstToken(int line, int column, string text)
            : base(line, column, text)
        {
            TokenType = TokenType.TYPE_STRING;
            StrValue = text;
        }
    }
}