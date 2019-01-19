using System;
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
                //exception unexpected something TODO: deal with it
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

            switch (token.Type)
            {
                case TokenType.IntegerNumber:
                {
                    NextToken();
                    return new ConstIntegerLiteral(token);
                }
                case TokenType.FloatNumber:
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

            var left = ParseIdent();
            if (left == null)
            {
                return null; // this is not variable ref
            }

            bool breakWhile = false;
            while (!breakWhile)
            {
                token = PeekToken();
                switch (token.Type)
                {
                    case TokenType.OpenSquareBracket:
                    {
                        NextToken();
                        var paramList = ParseParamList();
                        if (paramList == null || paramList.Count == 0)
                        {
                            throw new Exception(string.Format(
                                "{0}, {1} : syntax error, indexes expected, but {2} found",
                                PeekToken().Line, PeekToken().Column, PeekToken().Lexeme));
                        }

                        token = PeekToken();
                        CheckToken(PeekToken().Type, new List<TokenType> {TokenType.CloseSquareBracket},
                            string.Format("{0}, {1} : syntax error, ']' expected, but {2} found",
                                PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

                        left = new ArrayAccess(left, paramList);
                        break;
                    }
                    case TokenType.Dot:
                    {
                        NextToken();
                        var field = ParseIdent();
                        if (field == null)
                        {
                            throw new Exception(string.Format(
                                "{0}, {1} : syntax error, field ident expected, but {2} found",
                                PeekToken().Line, PeekToken().Column, PeekToken().Lexeme));
                        }

                        left = new RecordAccess(left, field);
                        break;
                    }
                    case TokenType.OpenBracket:
                    {
                        token = NextAndPeek();
                        var paramList = ParseParamList();

                        if (paramList == null)
                        {
                            throw new Exception(string.Format(
                                "{0}, {1} : syntax error, parameters list expected, but {2} found",
                                PeekToken().Line, PeekToken().Column, PeekToken().Lexeme));
                        }

                        CheckToken(PeekToken().Type, new List<TokenType> {TokenType.CloseBracket},
                            string.Format("{0}, {1} : syntax error, ')' expected, but {2} found",
                                PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

                        left = new FunctionCall(left, paramList);
                        break;
                    }
                    case TokenType.Carriage:
                    {
                        if (left.NodeType == AstNodeType.DereferenceOperator)
                        {
                            throw new Exception(string.Format("{0}, {1} : syntax error, double carriage found",
                                PeekToken().Line, PeekToken().Column, PeekToken().Lexeme));
                        }

                        var carriageToken = PeekAndNext();
                        left = new DereferenceOperator(carriageToken, left);
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

            List<AstNode> paramList = new List<AstNode>();

            while (true)
            {
                paramList.Add(ParseExpression());
                if (paramList[paramList.Count - 1] == null)
                {
                    throw new Exception(string.Format(
                        "{0}, {1} : syntax error, invalid expression",
                        PeekToken().Line, PeekToken().Column, PeekToken().Lexeme));
                }

                token = PeekToken();
                if (token == null || token.Type != TokenType.Comma)
                {
                    break;
                }

                NextToken();
            }

            return paramList;
        }
    }
}