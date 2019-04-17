using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Parser.Visitors;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal partial class PascalParser
    {
        private Statement ParseCompoundStatement()
        {
            var beginToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Begin},
                string.Format("({0}, {1}) syntax error: 'begin' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var statements = ParseStatementsPart();

            var endToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.End},
                string.Format("({0}, {1}) syntax error: 'end' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var compoundStatement = new CompoundStatement(beginToken, endToken, statements);
            return compoundStatement;
        }

        private List<Statement> ParseStatementsPart()
        {
            var statements = new List<Statement>();

            var stmt = ParseStatement();
            if (stmt == null)
            {
                return statements;
            }

            statements.Add(stmt);

            while (PeekToken().Type == TokenType.Semicolon)
            {
                CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                    string.Format("({0}, {1}) syntax error: ';' expected, but '{2}' found",
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

        private Statement ParseStatement()
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

        private Statement ParseSimpleStatement()
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

            if (stmt.NodeType != AstNodeType.AssignmentStatement && stmt.NodeType == AstNodeType.FunctionCall)
            {
                return new CallableCallStatement(stmt as FunctionCall);
            }

            return stmt as Statement; //
        }

        private AstNode ParseAssingStatement()
        {
            var left = ParseExpression();
            var assignToken = PeekToken();

            if (assignToken.Type != TokenType.SimpleAssignOperator
                && assignToken.Type != TokenType.SumAssignOperator && assignToken.Type != TokenType.DifAssignOperator
                && assignToken.Type != TokenType.MulAssignOperator && assignToken.Type != TokenType.DivAssignOperator)
            {
                return left; // this is not assign statement
            }

            if (left == null)
            {
                throw new Exception(string.Format("({0}, {1}) syntax error: expression expected",
                    assignToken.Line, assignToken.Column));
            }

            NextToken();
            var right = ParseExpression();

            if (right == null)
            {
                throw new Exception(string.Format("({0}, {1}) syntax error: expression expected",
                    assignToken.Line, assignToken.Column + 2));
            }

            return new AssignStatement(assignToken as AssignToken, left as Expression, right as Expression);
        }

        private Statement ParseSructuredStatement()
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

        private Statement ParseIfStatement()
        {
            var ifToken = PeekAndNext();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.OpenBracket},
                string.Format("({0}, {1}) syntax error: '(' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var ifExpr = ParseExpression();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.CloseBracket},
                string.Format("({0}, {1}) syntax error: ')' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var thenToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Then},
                string.Format("({0}, {1}) syntax error: 'then' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var thenStmt = ParseStatement();

            Token elseToken = null;
            Statement elseStmt = null;

            if (PeekToken().Type == TokenType.Else)
            {
                elseToken = PeekAndNext();
                elseStmt = ParseStatement();
            }

            return new IfStatement(ifToken, ifExpr, thenToken, thenStmt, elseToken, elseStmt);
        }

        private Statement ParseForStatement()
        {
            var forToken = PeekAndNext();

            var iteratorToken = PeekToken();
            var iterator = ParseIdent();
            if (iterator == null)
            {
                throw new Exception(string.Format("({0}, {1}) syntax error: iterator expected, but '{2}' found",
                    iteratorToken.Line, iteratorToken.Column, iteratorToken.Lexeme));
            }

            var assignToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.SimpleAssignOperator},
                string.Format("({0}, {1}) syntax error: ':=' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var forRange = ParseForRange();

            var doToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Do},
                string.Format("({0}, {1}) syntax error: 'do' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var stmt = ParseStatement();

            return new ForStatement(forToken, iterator, assignToken, forRange, doToken, stmt);
        }

        private ForRange ParseForRange()
        {
            var fromExpr = ParseExpression();

            var directionToken = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.To, TokenType.Downto},
                string.Format("({0}, {1}) syntax error: 'to' or 'downto' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var toExpr = ParseExpression();

            return new ForRange(directionToken, fromExpr, toExpr);
        }

        private Statement ParseWhileDoStatement()
        {
            var whileToken = PeekAndNext();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.OpenBracket},
                string.Format("({0}, {1}) syntax error: '(' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var conditionExpr = ParseExpression();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.CloseBracket},
                string.Format("({0}, {1}) syntax error: ')' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var doToken = PeekToken();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Do},
                string.Format("({0}, {1}) syntax error: 'do' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var stmt = ParseStatement();

            if (stmt == null)
            {
                throw new Exception(string.Format("({0}, {1}) syntax error: illegal expression",
                    PeekToken().Line, PeekToken().Column));
            }

            return new WhileStatement(whileToken, conditionExpr, doToken, stmt);
        }

        private Statement ParseDoWhileStatement()
        {
            var doToken = PeekAndNext();

            var stmt = ParseStatement();

            var whileToken = PeekToken();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.While},
                string.Format("({0}, {1}) syntax error: 'while' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.OpenBracket},
                string.Format("({0}, {1}) syntax error: '(' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var conditionExpr = ParseExpression();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.CloseBracket},
                string.Format("({0}, {1}) syntax error: ')' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            return new DoWhileStatement(whileToken, conditionExpr, doToken, stmt);
        }
    }
}