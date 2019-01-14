using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal delegate Token PeekAndNext();

    internal delegate Token NextAndPeek();

    internal delegate Token PeekToken();

    internal delegate bool NextToken();

    internal delegate AstNode DeclPartParser();

    internal delegate AstNode TypesParser();

    internal partial class PascalParser
    {
        public bool IsReady()
        {
            return true;
        }

        public AstNode ParseSingleExpression()
        {
            NextToken(); // skipping null token so next call will return not null token if it's exist
            return ParseAssingStatement();
        }

        public AstNode Parse()
        {
            if (PeekToken() != null)
            {
                //throw exception wrong init of lexer
                return null;
            }

            return ParseProgram();
        }

        private AstNode ParseProgram()
        {
            var token = PeekToken();
            if (token.Type != TokenType.Program)
            {
                //some parser exception
                return null;
            }

            NextToken();
            var header = ParseIdent();

            token = PeekToken();
            if (token.Type != TokenType.Semicolon)
            {
                //some parser exception
                return null;
            }

            NextToken();
            var mainBlock = ParseMainBlock();

            token = PeekToken();
            if (token.Type != TokenType.Dot)
            {
                //some parser exception
                return null;
            }

            return new Program(header, mainBlock);
        }

        private AstNode ParseMainBlock()
        {
            var declParts = ParseDeclsParts();
            var mainCompound = ParseCompoundStatement();
            return new MainBlock(declParts, mainCompound);
        }

        private AstNode ParseConstDecl()
        {
            var constIdent = ParseIdent();
            var token = PeekToken();
            if (token == null || token.Type != TokenType.EqualOperator)
            {
                //some parser exception
                return null;
            }

            NextToken();
            var expression = ParseExpression();
            token = PeekToken();
            if (token == null || token.Type != TokenType.Semicolon)
            {
                //some parser exception
                return null;
            }

            NextToken();
            return new ConstDecl(constIdent, expression);
        }


        private AstNode ParseVarDeclsPart()
        {
            throw new NotImplementedException();
        }

        private AstNode ParseaProcFuncDeclsPart()
        {
            throw new NotImplementedException();
        }

        private AstNode ParseExpression()
        {
            var left = ParseTerm();

            while (true)
            {
                var operationToken = PeekToken();
                if (operationToken == null
                    || operationToken.Type != TokenType.SumOperator && operationToken.Type != TokenType.DifOperator
                                                                    && operationToken.Type != TokenType.Or &&
                                                                    operationToken.Type != TokenType.Xor)
                {
                    break;
                }

                NextToken();
                var right = ParseTerm();
                left = new BinOperation(operationToken, left, right);
            }

            return left;
        }

        private AstNode ParseTerm()
        {
            var left = ParseFactor();

            while (true)
            {
                var operationToken = PeekToken();
                if (operationToken == null
                    || operationToken.Type != TokenType.MulOperator
                    && operationToken.Type != TokenType.DivOperator && operationToken.Type != TokenType.And
                    && operationToken.Type != TokenType.Mod && operationToken.Type != TokenType.Div
                    && operationToken.Type != TokenType.Shl && operationToken.Type != TokenType.Shr)
                {
                    break;
                }

                NextToken();
                var right = ParseFactor();
                left = new BinOperation(operationToken, left, right);
            }

            return left;
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

        private AstNode ParseAssingStatement()
        {
            var left = ParseExpression();
            var assignToken = PeekToken();
            if (assignToken == null)
            {
                return left;
            }

            if (assignToken.Type != TokenType.SimpleAssignOperator
                && assignToken.Type != TokenType.SumAssignOperator && assignToken.Type != TokenType.DifAssignOperator
                && assignToken.Type != TokenType.MulAssignOperator && assignToken.Type != TokenType.DivAssignOperator)
            {
                //some parser exception
                return null;
            }

            NextToken();
            var right = ParseExpression();
            return new AssignStatement(assignToken as AssignToken, left, right);
        }

        public PascalParser(PeekToken peekToken, NextToken nextToken, PeekAndNext peekAndNext, NextAndPeek nextAndPeek)
        {
            PeekToken = peekToken;
            NextToken = nextToken;
            PeekAndNext = peekAndNext;
            NextAndPeek = nextAndPeek;
        }

        private PeekToken PeekToken { get; }
        private NextToken NextToken { get; }
        private PeekAndNext PeekAndNext { get; }
        private NextAndPeek NextAndPeek { get; }
    }
}