using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;
using Type = System.Type;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal partial class PascalParser
    {
        private List<AstNode> ParseDeclsParts()
        {
            List<AstNode> declsParts = new List<AstNode>();
            bool stopParse = false;

            while (!stopParse)
            {
                AstNode declsPart = null;
                switch (PeekToken().Type)
                {
                    case TokenType.Const:
                    {
                        declsPart = ParseConstDeclsPart();
                        break;
                    }
                    case TokenType.Type:
                    {
                        declsPart = ParseTypeDeclsPart();
                        break;
                    }
                    case TokenType.Var:
                    {
                        declsPart = ParseVarDeclsPart();
                        break;
                    }
                    case TokenType.Procedure:
                    {
                        declsPart = ParseProcDecl();
                        break;
                    }
                    case TokenType.Function:
                    {
                        declsPart = ParseFuncDecl();
                        break;
                    }
                    default:
                    {
                        stopParse = true;
                        break;
                    }
                }

                if (declsPart == null)
                {
                    continue;
                }

                declsParts.Add(declsPart);
            }

            return declsParts;
        }

        private AstNode ParseConstDeclsPart(bool local = false)
        {
            var token = PeekAndNext();

            var constDecls = new List<AstNode>();
            var constDecl = ParseConstDecl();
            if (constDecl == null)
            {
                throw new Exception(string.Format("({0}, {1}) syntax error : Wrong const declaration",
                    PeekToken().Line, PeekToken().Column));
            }

            constDecls.Add(constDecl);
            do
            {
                constDecl = ParseConstDecl();
                if (constDecl == null) break;
                constDecls.Add(constDecl);
            } while (true);

            return new ConstDeclsPart(token, constDecls);
        }

        private AstNode ParseConstDecl(bool local = false)
        {
            var constIdent = ParseIdent();
            var token = PeekToken();
            if (token.Type != TokenType.EqualOperator)
            {
                //some parser exception
                return null;
            }

            NextToken();
            var expression = ParseExpression();
            var semicolonToken = PeekToken();
            if (semicolonToken == null || semicolonToken.Type != TokenType.Semicolon)
            {
                //some parser exception
                return null;
            }

            NextToken();

            return new ConstDecl(token, constIdent, expression, local);
        }

        private AstNode ParseTypeDeclsPart()
        {
            var token = PeekAndNext();
            var typeDecls = new List<AstNode> {ParseTypeDecl()};
            if (typeDecls[0] == null)
            {
                throw new Exception(string.Format("({0}, {1}) syntax error: Empty type block", PeekToken().Line,
                    PeekToken().Column));
            }

            do
            {
                var typeDecl = ParseTypeDecl();
                if (typeDecl == null) break;
                typeDecls.Add(typeDecl);
            } while (true);

            return new TypeDeclsPart(token, typeDecls);
        }

        private AstNode ParseTypeDecl()
        {
            var typeIdent = ParseIdent();

            if (typeIdent == null)
            {
                // this meants that we get to another part of declaration or compound statement and key word was met
                return null;
            }

            var token = PeekToken();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.EqualOperator},
                string.Format("({0}, {1}) syntax error: '=' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var type = ParseType();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("({0}, {1}) syntax error: ';' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            return new TypeDecl(typeIdent as Ident, type);
        }

        private AstNode ParseVarDeclsPart(bool local = false)
        {
            var token = PeekAndNext();

            var varDecls = new List<AstNode> {ParseVarDecl(local)};
            if (varDecls[0] == null)
            {
                throw new Exception(string.Format("({0}, {1}) syntax error: Empty var block", PeekToken().Line,
                    PeekToken().Column));
            }

            do
            {
                var varDecl = ParseVarDecl(local);
                if (varDecl == null) break;
                varDecls.Add(varDecl);
            } while (true);

            return new VarDeclsPart(token, varDecls, local);
        }

        private AstNode ParseVarDecl(bool local = false)
        {
            if (PeekToken().Type != TokenType.Ident)
                return null;
            var varIdents = ParseIdentList();

            var token = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Colon},
                string.Format("({0}, {1}) syntax error: ':' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var type = ParseType();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("({0}, {1}) syntax error: ';' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            return new SimpleVarDecl(varIdents, type, local);
        }

        private AstNode ParseFuncDecl()
        {
            var funcHeader = ParseFuncHeader();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("({0}, {1}) syntax error: ';' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

//            _symbolTableStack.PrepareFunction(functionSymbol.Ident, functionSymbol); // Add function to handle recursion 

//            _symbolTableStack.Push(); // this will be local table
            var funcSubroutineBlock = ParseSubroutineBlock();

//            functionSymbol.Body = funcSubroutineBlock as SubroutineBlock;

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("({0}, {1}) syntax error: ';' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            return new CallableDeclNode(funcHeader, funcSubroutineBlock);
        }

        private CallableHeader ParseFuncHeader()
        {
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Function},
                string.Format("({0}, {1}) syntax error: 'function' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var funcName = ParseIdent();

            var paramList = ParseFormalParamList();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Colon},
                string.Format("({0}, {1}) syntax error: ':' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var returnType = ParseSimpleType();

            return new CallableHeader(funcName, paramList, returnType);
        }

        private AstNode ParseProcDecl()
        {
            var procHeader = ParseProcHeader();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("({0}, {1}) syntax error: ';' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            _symbolTableStack.Push(); // this will be local table
            var procSubroutineBlock = ParseSubroutineBlock();
            if (procSubroutineBlock == null)
            {
            }

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("({0}, {1}) syntax error: ';' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            return new CallableDeclNode(procHeader, procSubroutineBlock);
        }

        private CallableHeader ParseProcHeader()
        {
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Procedure},
                string.Format("({0}, {1}) syntax error: 'procedure' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var procName = ParseIdent();
//            CheckDuplicateIdentifier(procName.Token);
//            var procSymbol = new CallableSymbol(procName.ToString());

//            _symbolTableStack.Push(); //this will be Parameters table
            var paramList = ParseFormalParamList();

//            procSymbol.Parameters = _symbolTableStack.Peek();

            return new CallableHeader(procName as Ident, paramList);
        }

        private AstNode ParseSubroutineBlock()
        {
            if (PeekToken().Type == TokenType.Forward)
            {
                NextToken(); //we do not need "forward" token, so just throw it away
                return null;
            }

            var declsParts = new List<AstNode>();
            bool stopParse = false;
            while (!stopParse)
            {
                stopParse = true;
                switch (PeekToken().Type)
                {
                    case TokenType.Const:
                    {
                        stopParse = false;
                        var constDeclsPart = ParseConstDeclsPart(true);
                        declsParts.Add(constDeclsPart);
                        break;
                    }
                    case TokenType.Type:
                    {
                        stopParse = false;
                        var typeDeclsPart = ParseTypeDeclsPart();
                        declsParts.Add(typeDeclsPart);
                        break;
                    }
                    case TokenType.Var:
                    {
                        stopParse = false;
                        var varDeclsPart = ParseVarDeclsPart(true);
                        declsParts.Add(varDeclsPart);
                        break;
                    }
                }
            }

            var compound = ParseCompoundStatement();

            return new SubroutineBlock(declsParts, compound);
        }
    }
}