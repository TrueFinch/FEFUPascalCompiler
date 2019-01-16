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
            var beginToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Begin},
                string.Format("{0} {1} : syntax error, 'begin' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
            
            var statements = ParseStatementsPart();

            var endToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType>{TokenType.Colon},
                string.Format("{0} {1} : syntax error, 'end' expected, but {2} found", 
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
            
            return new CompoundStatement(beginToken, endToken, statements);
        }

        private List<AstNode> ParseStatementsPart()
        {
            var statements = new List<AstNode>();
            bool stopParseStatements = false;

            while (!stopParseStatements)
            {
                var stmt = ParseStatement();
                statements.Add(stmt);
                CheckToken(PeekToken().Type, new List<TokenType>{TokenType.Colon},
                    string.Format("{0} {1} : syntax error, ';' expected, but {2} found", 
                        PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
            }

            return statements;
        }

        private AstNode ParseStatement()
        {
            var stmtStartToken = PeekToken();
            var stmt = ParseSructuredStatement();
            if (stmt == null)
            {
                stmt = ParseSimpleStatement();
                if (stmt == null)
                {
                    throw new Exception(string.Format("{0} {1} : syntax error, statement expected", 
                        stmtStartToken.Line, stmtStartToken.Column));
                }
            }
            
            
        }

        private AstNode ParseSimpleStatement()
        {
            if (PeekToken().Type == TokenType.Pass)
            {
                return new EmptyStatement(PeekAndNext());
            }

            var stmt = ParseAssingStatement();
            if (stmt.Type == AstNodeType.AssignmentStatement
                || stmt.Type != AstNodeType.AssignmentStatement && stmt.Type == AstNodeType.FunctionCall)
            {
                return stmt;
            }

            return null; //this means that it is not simple statement
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
        
        private AstNode ParseSructuredStatement()
        {
            switch (PeekToken().Type)
            {
                case TokenType.If:
                {
                    return ParseIfStatement();
                }
                case TokenType.For:
                {
                    return ParseForStatement
                }
            }
        }

        private AstNode ParseIfStatement()
        {
            var ifToken = PeekAndNext();
            CheckToken(PeekToken().Type, new List<TokenType>{TokenType.OpenBracket},
                string.Format("{0} {1} : syntax error, '(' expected, but {2} found", 
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
                    
            var ifExpr = ParseExpression();
                    
            CheckToken(PeekToken().Type, new List<TokenType>{TokenType.CloseBracket},
                string.Format("{0} {1} : syntax error, ')' expected, but {2} found", 
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));

            var thenToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType>{TokenType.Then},
                string.Format("{0} {1} : syntax error, 'then' expected, but {2} found", 
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));

            var thenStmt = ParseStatement();

            Token elseToken = null;
            AstNode elseStmt = null;
                    
            if (PeekToken().Type == TokenType.Else)
            {
                elseToken = PeekAndNext();
                elseStmt = ParseStatement();
            }
                    
            return new IfStatement(ifToken, ifExpr, thenToken, thenStmt, elseToken, elseStmt);
        }
    }
}