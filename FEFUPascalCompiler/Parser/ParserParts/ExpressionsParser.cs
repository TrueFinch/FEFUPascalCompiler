using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal partial class PascalParser
    {
        static List<TokenType> ComparingOperators = new List<TokenType>
        {
            TokenType.NotEqualOperator, TokenType.LessOperator, TokenType.LessOrEqualOperator,
            TokenType.EqualOperator, TokenType.GreaterOperator, TokenType.GreaterOrEqualOperator,
        };

        static List<TokenType> AdditiveOperators = new List<TokenType>
        {
            TokenType.SumOperator, TokenType.DifOperator, TokenType.Or, TokenType.Xor,
        };

        static List<TokenType> MultiplyingOperators = new List<TokenType>
        {
            TokenType.MulOperator, TokenType.DivOperator, TokenType.Div, TokenType.Mod,
            TokenType.And, TokenType.Shr, TokenType.Shl,
        };

        static List<TokenType> UnaryOperators = new List<TokenType>
        {
            TokenType.SumOperator, TokenType.DifOperator, TokenType.Not, TokenType.AtSign,
        };

        private AstNode ParseExpression()
        {
            var left = ParseSimpleExpression();

            if (left == null)
            {
                //exception unexpected something
                return null;
            }

            while (true)
            {
                var operatorToken = PeekToken();
                if (operatorToken == null || !ComparingOperators.Contains(operatorToken.Type))
                {
                    break;
                }

                NextToken();
                var right = ParseSimpleExpression();
                left = new ComparingOperator(operatorToken, left, right);
            }

            return left;
        }

        private AstNode ParseSimpleExpression()
        {
            var left = ParseTerm();

            if (left == null)
            {
                //exception unexpected something
                return null;
            }

            while (true)
            {
                var operatorToken = PeekToken();
                if (operatorToken == null || !AdditiveOperators.Contains(operatorToken.Type))
                {
                    break;
                }

                NextToken();
                var right = ParseTerm();
                left = new AdditiveOperator(operatorToken, left, right);
            }

            return left;
        }

        private AstNode ParseTerm()
        {
            var left = ParseSimpleTerm();

            if (left == null)
            {
                //exception unexpected something
                return null;
            }

            while (true)
            {
                var operatorToken = PeekToken();
                if (operatorToken == null || !MultiplyingOperators.Contains(operatorToken.Type))
                {
                    break;
                }

                NextToken();
                var right = ParseSimpleTerm();
                left = new MultiplyingOperator(operatorToken, left, right);
            }

            return left;
        }

        private AstNode ParseSimpleTerm()
        {
            var operatorToken = PeekToken();

            if (operatorToken != null && UnaryOperators.Contains(operatorToken.Type))
            {
                NextToken();
                var factor = ParseFactor();
                return new UnaryOperator(operatorToken, factor);
            }

            return ParseFactor();
        }

        private AstNode ParseFactor()
        {
//            var token = PeekAndNext();
            var token = PeekToken();

            switch (token.Type)
            {
                case TokenType.Ident:
                {
                    return ParseIdent();
                }
                case TokenType.DecIntegerNumber:
                {
                    NextToken();
                    return new ConstIntegerLiteral(token as IntegerNumberToken);
                }
                case TokenType.DoubleNumber:
                {
                    NextToken();
                    return new ConstDoubleLiteral(token as DoubleNumberToken);
                }
                case TokenType.OpenBracket:
                {
                    NextToken();
                    var expression = ParseExpression();
                    token = PeekAndNext();
                    if (token.Type != TokenType.CloseBracket)
                    {
                        //some parser exception
                    }

                    return expression;
                }
                default:
                {
                    //some parser exception

                    return null;
                }
            }
        }
    }
}