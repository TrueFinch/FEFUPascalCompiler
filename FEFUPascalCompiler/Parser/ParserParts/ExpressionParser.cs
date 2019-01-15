using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal partial class PascalParser
    {
        private static readonly List<TokenType> ComparingOperators = new List<TokenType>
        {
            TokenType.NotEqualOperator, TokenType.LessOperator, TokenType.LessOrEqualOperator,
            TokenType.EqualOperator, TokenType.GreaterOperator, TokenType.GreaterOrEqualOperator,
        };

        private static readonly List<TokenType> AdditiveOperators = new List<TokenType>
        {
            TokenType.SumOperator, TokenType.DifOperator, TokenType.Or, TokenType.Xor,
        };

        private static readonly List<TokenType> MultiplyingOperators = new List<TokenType>
        {
            TokenType.MulOperator, TokenType.DivOperator, TokenType.Div, 
            TokenType.Mod, TokenType.And, TokenType.Shr, TokenType.Shl,
        };

        private static readonly List<TokenType> UnaryOperators = new List<TokenType>
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
            var token = PeekToken();

            if (token == null)
            {
                //exception -- unexpected end of file
                return null;
            }

            switch (token.Type)
            {
                case TokenType.IntegerNumber:
                {
                    NextToken();
                    return new ConstIntegerLiteral(token);
                }
                case TokenType.DoubleNumber:
                {
                    NextToken();
                    return new ConstDoubleLiteral(token);
                }
                case TokenType.Char:
                {
                    NextToken();
                    return new ConstCharLiteral(token);
                }
                case TokenType.StringConst:
                {
                    NextToken();
                    return new ConstStringLiteral(token);
                }
                case TokenType.Nil:
                {
                    NextToken();
                    return new Nil(token);
                }
                case TokenType.OpenBracket:
                {
                    NextToken();
                    var expression = ParseExpression();
                    token = PeekToken();
                    if (token.Type != TokenType.CloseBracket)
                    {
                        //some parser exception
                        return null;
                    }

                    NextToken();
                    return expression;
                }
                default:
                {
                    return ParseVariableReference();
                }
            }
        }

        private AstNode ParseVariableReference()
        {
            var token = PeekToken();

            if (token == null)
            {
                //exception -- unexpected end of file
                return null;
            }

            var left = ParseIdent();
            if (left == null)
            {
                //exception - unexpected lexeme
                return null;
            }

            bool breakWhile = false;
            while (!breakWhile)
            {
                token = NextAndPeek();
                switch (token.Type)
                {
                    case TokenType.OpenSquareBracket:
                    {
                        NextToken();
                        var paramList = ParseParamList();
                        if (paramList == null || paramList.Count == 0)
                        {
                            //exception -- indexes expected
                        }

                        token = PeekToken();
                        if (token == null || token.Type != TokenType.CloseSquareBracket)
                        {
                            //exception -- an unclosed bracket
                        }

                        left = new ArrayAccess(left, paramList);
                        break;
                    }
                    case TokenType.Dot:
                    {
                        NextToken();
                        var field = ParseIdent();
                        if (field == null)
                        {
                            //exception -- indexes expected
                        }

                        left = new RecordAccess(left, field);
                        break;
                    }
                    case TokenType.OpenBracket:
                    {
                        NextToken();
                        var paramList = ParseParamList();
                        
                        if (paramList == null)
                        {
                            //exception -- params (even empty) expected
                        }

                        left = new FunctionCall(left, paramList);
                        break;
                    }
                    default:
                    {
                        breakWhile = true;
                        break;
                    }
                }
            }

            return left;
        }

        private List<AstNode> ParseParamList()
        {
            var token = PeekToken();
            if (token == null)
            {
                //exception -- 
            }
            
            List<AstNode> paramList = new List<AstNode>();
            
            while (true)
            {
                paramList.Add(ParseExpression());
                if (paramList[paramList.Count - 1] == null)
                {
                    //exception -- invalid expression
                    return null;
                }

                token = PeekToken();
                if (token == null || token.Type != TokenType.Comma)
                {
                    break;
                }
            }

            return paramList;
        }
    }
    
    
}