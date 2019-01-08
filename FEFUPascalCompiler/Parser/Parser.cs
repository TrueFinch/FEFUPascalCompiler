using FEFUPascalCompiler.Lexer;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser
{
    internal delegate Token PeekAndNext();

    internal delegate Token PeekToken();

    internal delegate bool NextToken();

    internal class Parser
    {
        public bool IsReady()
        {
            return true;
        }

        public NodeAst ParseSingleExpression()
        {
            NextToken(); // skipping null token so next call will return not null token if it's exist
            return ParseExpression();
        }

        public NodeAst Parse()
        {
            return null;
        }

        private NodeAst ParseExpression()
        {
            var left = ParseTerm();

            while (true)
            {
                var operationToken = PeekToken();
                if (operationToken == null
                    || operationToken.Type != TokenType.SumOperator && operationToken.Type != TokenType.DifOperator)
                {
                    break;
                }

                NextToken();
                var right = ParseTerm();
                left = new BinOperation(operationToken as BinOperatorToken, left, right);
            }

            return left;
        }

        private NodeAst ParseTerm()
        {
            var left = ParseFactor();

            while (true)
            {
                var operationToken = PeekToken();
                if (operationToken == null
                    || operationToken.Type != TokenType.MulOperator && operationToken.Type != TokenType.DivOperator)
                {
                    break;
                }

                NextToken();
                var right = ParseFactor();
                left = new BinOperation(operationToken as BinOperatorToken, left, right);
            }

            return left;
        }

        private NodeAst ParseFactor()
        {
            var token = PeekAndNext();

            switch (token.Type)
            {
                case TokenType.Ident:
                {
                    return new Variable(token as IdentToken);
                }
                case TokenType.IntegerNumber:
                case TokenType.DoubleNumber:
                {
                    return new ConstLiteral(token);
                }
                case TokenType.OpenBracket:
                {
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

        public Parser(PeekToken peekToken, NextToken nextToken, PeekAndNext peekAndNext)
        {
            PeekToken = peekToken;
            NextToken = nextToken;
            PeekAndNext = peekAndNext;
        }

        private PeekToken PeekToken { get; }
        private NextToken NextToken { get; }
        private PeekAndNext PeekAndNext { get; }
    }
}