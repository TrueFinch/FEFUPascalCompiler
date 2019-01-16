using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal partial class PascalParser
    {
        private AstNode ParseCompoundStatement()
        {
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Begin},
                string.Format("{0} {1} : syntax error, 'begin' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
            
            var stmtPart = ParseStatementsPart();
            
            CheckToken(PeekToken().Type, new List<TokenType>{TokenType.Colon},
                string.Format("{0} {1} : syntax error, 'end' expected, but {2} found", 
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));

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
    }
}