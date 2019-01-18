using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FEFUPascalCompiler.Parser.AstNodes;
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

        private AstNode ParseConstDeclsPart()
        {
            var token = PeekAndNext();

            var constDecls = new List<AstNode>();
            var constDecl = ParseConstDecl();
            if (constDecl == null)
            {
                throw new Exception(string.Format("{0}, {1} : Wrong const declaratio",
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

        private AstNode ParseConstDecl()
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

            return new ConstDecl(token, constIdent, expression);
        }

        private AstNode ParseTypeDeclsPart()
        {
            var token = PeekAndNext();
            var typeDecls = new List<AstNode> {ParseTypeDecl()};
            if (typeDecls[0] == null)
            {
                throw new Exception(string.Format("{0}, {1} : Empty type block", PeekToken().Line, PeekToken().Column));
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

            CheckDuplicateIdentifier(typeIdent.Token);

            var token = PeekToken();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.EqualOperator},
                string.Format("{0} {1} : syntax error, '=' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var type = ParseType();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            if (type.Item1.Ident.Length != 0 && CheckTypeDeclared(type.Item1.Ident))
            {
                AliasType alias = new AliasType(typeIdent.ToString(), type.Item1);
                _symbolTableStack.Peek().Add(typeIdent.ToString(), alias);
            }
            else
            {
                type.Item1.Ident = typeIdent.ToString();
                _symbolTableStack.Peek().Add(typeIdent.ToString(), type.Item1);
            }


            return new TypeDecl(typeIdent, type.Item2);
        }

        private AstNode ParseVarDeclsPart(bool local = false)
        {
            var token = PeekAndNext();

            var varDecls = new List<AstNode> {ParseVarDecl(local)};
            if (varDecls[0] == null)
            {
                throw new Exception(string.Format("{0}, {1} : Empty var block", PeekToken().Line, PeekToken().Column));
            }

            do
            {
                var varDecl = ParseVarDecl(local);
                if (varDecl == null) break;
                varDecls.Add(varDecl);
            } while (true);

            return new VarDeclsPart(token, varDecls);
        }

        private AstNode ParseVarDecl(bool local = false)
        {
            if (PeekToken().Type != TokenType.Ident)
                return null;
            var varIdents = ParseIdentList();

            foreach (var varIdent in varIdents)
            {
                CheckDuplicateIdentifier(varIdent.Token);
            }

            var token = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Colon},
                string.Format("{0} {1} : syntax error, ':' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var type = ParseType();

            foreach (var varIdent in varIdents)
            {
                _symbolTableStack.Peek().Add(varIdent.ToString(),
                    local ? new Local(type.Item1) as object : new Global(type.Item1));
            }

            if (varIdents.Count == 1)
            {
                if (PeekToken().Type == TokenType.EqualOperator)
                {
                    NextToken();
                    var expr = ParseExpression();
                    CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                        string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                            PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

                    return new InitVarDecl(varIdents[0], type.Item2, expr);
                }
            }

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            return new SimpleVarDecl(varIdents, type.Item2);
        }

        private AstNode ParseProcFuncDeclsPart()
        {
            var declarations = new List<AstNode>();
            bool stopParse = false;
            while (!stopParse)
            {
                stopParse = true;
                switch (PeekToken().Type)
                {
                    case TokenType.Procedure:
                    {
                        stopParse = false;
                        var procDecl = ParseProcDecl();
                        declarations.Add(procDecl);
                        break;
                    }
                    case TokenType.Function:
                    {
                        stopParse = false;
                        var funcDecl = ParseFuncDecl();
                        declarations.Add(funcDecl);
                        break;
                    }
                }
            }

            return new ProcFuncDeclsPart(declarations);
        }

        private AstNode ParseFuncDecl()
        {
            _symbolTableStack.Push(new OrderedDictionary()); // this will be Parameters table
            var funcHeader = ParseFuncHeader();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            
            var parametersTable
            
            var funcSubroutineBlock = ParseSubroutineBlock();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            _symbolTableStack.Peek()

            return new FuncDecl(funcHeader, funcSubroutineBlock);
        }

        private AstNode ParseFuncHeader()
        {
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Function},
                string.Format("{0} {1} : syntax error, 'function' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var funcName = ParseIdent();
            var paramList = ParseFormalParamList();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Colon},
                string.Format("{0} {1} : syntax error, ':' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var returnType = ParseSimpleType();

            return new FuncHeader(funcName, paramList, returnType.);
        }

        private AstNode ParseSubroutineBlock()
        {
            if (PeekToken().Type == TokenType.Forward)
            {
                return new Forward(PeekAndNext());
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
                        var constDeclsPart = ParseConstDeclsPart();
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
                        var varDeclsPart = ParseVarDeclsPart();
                        declsParts.Add(varDeclsPart);
                        break;
                    }
                }
            }

            var compound = ParseCompoundStatement();

            return new SubroutineBlock(declsParts, compound);
        }

        private AstNode ParseProcDecl()
        {
            var procHeader = ParseProcHeader();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var procSubroutineBlock = ParseSubroutineBlock();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            return new ProcDecl(procHeader, procSubroutineBlock);
        }

        private AstNode ParseProcHeader()
        {
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Procedure},
                string.Format("{0} {1} : syntax error, 'procedure' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var funcName = ParseIdent();
            var paramList = ParseFormalParamList();

            return new ProcHeader(funcName, paramList);
        }
    }
}