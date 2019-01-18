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
                string.Format("{0} {1} : syntax error, 'begin' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var statements = ParseStatementsPart();

            var endToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.End},
                string.Format("{0} {1} : syntax error, 'end' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            return new CompoundStatement(beginToken, endToken, statements);
        }

        private List<AstNode> ParseStatementsPart()
        {
            var statements = new List<AstNode>();

            var stmt = ParseStatement();
            if (stmt == null)
            {
                return statements;
            }
            
            statements.Add(stmt);
            
            while (true)
            {
                CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                    string.Format("{0}, {1} : syntax error, ';' expected, but '{2}' found",
                        PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
                stmt = ParseStatement();
                if (stmt == null)
                {
                    break;
                }
                statements.Add(stmt);
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
                    return null; // this is not statement
                }
            }

            return stmt;
        }

        private AstNode ParseSimpleStatement()
        {
            if (PeekToken().Type == TokenType.Pass)
            {
                return new EmptyStatement(PeekAndNext());
            }

            var stmt = ParseAssingStatement();
            if (stmt == null)
            {
                return null; //this is not simple statement
            }
            if (stmt.NodeType == AstNodeType.AssignmentStatement
                || stmt.NodeType != AstNodeType.AssignmentStatement && stmt.NodeType == AstNodeType.FunctionCall)
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
                return left; // this is not assign statement
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
                    return ParseForStatement();
                }
                case TokenType.While:
                {
                    return ParseWhileDoStatement();
                }
                case TokenType.Do:
                {
                    return ParseDoWhileStatement();
                }
                case TokenType.Begin:
                {
                    return ParseCompoundStatement();
                }
                default:
                {
                    return null; // this is not structured statement
                }
            }
        }

        private AstNode ParseIfStatement()
        {
            var ifToken = PeekAndNext();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.OpenBracket},
                string.Format("{0}, {1} : syntax error, '(' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var ifExpr = ParseExpression();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.CloseBracket},
                string.Format("{0}, {1} : syntax error, ')' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var thenToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Then},
                string.Format("{0}, {1} : syntax error, 'then' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

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

        private AstNode ParseForStatement()
        {
            var forToken = PeekAndNext();

            var iteratorToken = PeekToken();
            var iterator = ParseIdent();
            if (iterator == null)
            {
                throw new Exception(string.Format("{0}, {1} : syntax error, identifier expected, but '{2}' found",
                    iteratorToken.Line, iteratorToken.Column, iteratorToken.Lexeme));
            }

            var assignToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.SimpleAssignOperator},
                string.Format("{0}, {1} : syntax error, ':=' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var forRange = ParseForRange();
            
            var doToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Do},
                string.Format("{0}, {1} : syntax error, 'do' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var stmt = ParseStatement();
            
            return new ForStatement(forToken, iterator, assignToken, forRange, doToken, stmt);
        }

        private AstNode ParseForRange()
        {
            var fromExpr = ParseExpression();

            var directionToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.To, TokenType.Downto},
                string.Format("{0}, {1} : syntax error, 'to' or 'downto' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var toExpr = ParseExpression();

            return new ForRange(directionToken, fromExpr, toExpr);
        }
        
        private AstNode ParseWhileDoStatement()
        {
            var whileToken = PeekToken();
            
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.OpenBracket},
                string.Format("{0}, {1} : syntax error, '(' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var conditionExpr = ParseExpression();
            
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.CloseBracket},
                string.Format("{0}, {1} : syntax error, ')' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            
            var doToken = PeekToken();
            
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Do},
                string.Format("{0}, {1} : syntax error, 'do' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var stmt = ParseStatement();
            
            return new WhileStatement(whileToken, conditionExpr, doToken, stmt);
        }
        
        private AstNode ParseDoWhileStatement()
        {
            var doToken = PeekAndNext();
            
            var stmt = ParseStatement();
            
            var whileToken = PeekToken();
            
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.While},
                string.Format("{0}, {1} : syntax error, 'while' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.OpenBracket},
                string.Format("{0}, {1} : syntax error, '(' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var conditionExpr = ParseExpression();
            
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.CloseBracket},
                string.Format("{0}, {1} : syntax error, ')' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            
            return new DoWhileStatement(whileToken, conditionExpr, doToken, stmt);
        }
    }
}