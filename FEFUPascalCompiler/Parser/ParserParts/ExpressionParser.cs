using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;
using Type = System.Type;

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

        private Expression ParseExpression()
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

        private Expression ParseSimpleExpression()
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

        private Expression ParseTerm()
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

        private Expression ParseSimpleTerm()
        {
            var operatorToken = PeekToken();

            if (operatorToken != null && UnaryOperators.Contains(operatorToken.Type))
            {
                NextToken();
                var factor = ParseFactor();
                if (operatorToken.Type == TokenType.AtSign &&
                    !(factor is Ident || factor is ArrayAccess || factor is RecordAccess))
                {
                }

                return new UnaryOperator(operatorToken, factor);
            }

            return ParseFactor();
        }

        private Expression ParseFactor()
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
                    return new ConstFloatLiteral(token);
                }
                case TokenType.CharConst:
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
                case TokenType.True:
                case TokenType.False:
                {
                    NextToken();
                    return new ConstBooleanLiteral(token);
                }
                case TokenType.OpenBracket:
                {
                    NextToken();
                    var expression = ParseExpression();
                    token = PeekToken();

                    CheckToken(PeekToken().Type, new List<TokenType> {TokenType.CloseBracket},
                        string.Format("({0}, {1}) syntax error: ')' expected, but {2} found",
                            PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

                    return expression;
                }
                default:
                {
                    return ParseVariableReference();
                }
            }
        }

        private Expression ParseVariableReference()
        {
            var token = PeekToken();

            AstNode left = ParseIdent();
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
                                "({0}, {1}) syntax error: indexes expected, but {2} found",
                                PeekToken().Line, PeekToken().Column, PeekToken().Lexeme));
                        }

                        token = PeekToken();
                        CheckToken(PeekToken().Type, new List<TokenType> {TokenType.CloseSquareBracket},
                            string.Format("({0}, {1}) syntax error: ']' expected, but {2} found",
                                PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

                        if (!(left is Ident || left is RecordAccess || left is ArrayAccess))
                        {
                            throw new Exception(string.Format(
                                "({0}, {1}) syntax error: accessing array must be identifier",
                                left.Token.Line, left.Token.Column, left.Token.Lexeme));
                        }

                        left = new ArrayAccess(left as Expression, paramList);
                        break;
                    }
                    case TokenType.Dot:
                    {
                        NextToken();
                        var field = ParseIdent();
                        if (field == null)
                        {
                            throw new Exception(string.Format(
                                "({0}, {1}) syntax error: field ident expected, but {2} found",
                                PeekToken().Line, PeekToken().Column, PeekToken().Lexeme));
                        }

//                        if (!(field is Ident))
//                        {
//                            throw new Exception(string.Format(
//                                "{0}, {1} : syntax error, accessing field must be identifier",
//                                field.Token.Line, field.Token.Column, field.Token.Lexeme));
//                        }

                        left = new RecordAccess(left as Expression, field as Ident);
                        break;
                    }
                    case TokenType.OpenBracket:
                    {
                        token = NextAndPeek();
                        var paramList = ParseParamList();

                        if (paramList == null)
                        {
                            throw new Exception(string.Format(
                                "({0}, {1}) syntax error: parameters list expected, but {2} found",
                                PeekToken().Line, PeekToken().Column, PeekToken().Lexeme));
                        }

                        CheckToken(PeekToken().Type, new List<TokenType> {TokenType.CloseBracket},
                            string.Format("({0}, {1}) syntax error: ')' expected, but {2} found",
                                PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

                        if (!(left is Ident))
                        {
                            throw new Exception(string.Format(
                                "({0}, {1}) syntax error: accessing field must be identifier",
                                left.Token.Line, left.Token.Column, left.Token.Lexeme));
                        }

                        left = left.Token.Type == TokenType.Write || left.Token.Type == TokenType.WriteLn
                            ? (AstNode) new WriteFunctionCall((Ident) left, paramList,
                                left.Token.Type == TokenType.WriteLn)
                            : new UserFunctionCall((Ident) left, paramList);
                        break;
                    }
                    case TokenType.Carriage:
                    {
                        if (left.NodeType == AstNodeType.DereferenceOperator)
                        {
                            throw new Exception(string.Format("({0}, {1}) syntax error: double carriage found",
                                PeekToken().Line, PeekToken().Column, PeekToken().Lexeme));
                        }

                        var carriageToken = PeekAndNext();
                        left = new DereferenceOperator(carriageToken, left as Expression);
                        break;
                    }
                    default:
                    {
                        breakWhile = true;
                        break;
                    }
                }
            }

            return left as Expression;
        }

        private List<Expression> ParseParamList()
        {
            var token = PeekToken();

            List<Expression> paramList = new List<Expression>();

            var expr = ParseExpression();
            if (expr == null)
            {
                return paramList; // list is empty - no parameters given
            }

            paramList.Add(expr);
            while (true)
            {
                token = PeekToken();
                if (token.Type != TokenType.Comma)
                {
                    break;
                }

                NextToken();

                expr = ParseExpression();
                if (expr == null)
                {
                    break; //
                }

                paramList.Add(expr);
            }

            return paramList;
        }

        private AstNode ParseConformatArray()
        {
            var arrayToken = PeekAndNext();
            var ofToken = PeekAndNext();
            if (arrayToken.Type == TokenType.Array && ofToken.Type == TokenType.Of)
            {
                var simpleType = ParseSimpleType();
                return new ConformantArray(arrayToken, ofToken, simpleType);
            }

            throw new Exception(string.Format("({0}, {1}) syntax error: conformat array type expected, but {2} found",
                PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
        }

        public List<Ident> ParseIdentList()
        {
            var identList = new List<Ident>();
            if (!(ParseIdent() is Ident ident))
            {
                //exception -- this is not ident list
                return identList;
            }

            identList.Add(ident);
            while (true)
            {
                if (PeekToken().Type != TokenType.Comma)
                {
                    break;
                }

                NextToken();
                ident = ParseIdent() as Ident;
                if (ident == null)
                {
                    //exception unexpected lexeme
                    return null;
                }

                identList.Add(ident);
            }

            return identList;
        }

        private Ident ParseIdent()
        {
            var token = PeekToken();
            if (!new List<TokenType> {TokenType.Ident, TokenType.WriteLn, TokenType.Write}.Contains(token.Type))
            {
                return null;
            }
//            if (token.Type != TokenType.Ident)
//            {
//                //this is not ident, may be this is key word? Think about it
//            }

            return new Ident(PeekAndNext());
        }

        private ConstIntegerLiteral ParseConstIntegerLiteral()
        {
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.IntegerNumber},
                string.Format("({0} {1}) syntax error: integer expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekToken().Lexeme));
            return new ConstIntegerLiteral(PeekAndNext());
        }

        private Expression ParseConstFloatLiteral()
        {
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.FloatNumber},
                string.Format("({0} {1}) syntax error: float expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekToken().Lexeme));
            return new ConstFloatLiteral(PeekAndNext());
        }
    }
}