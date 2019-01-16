using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Tokens;

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
            var token = PeekToken();
            if (token.Type != TokenType.Const)
            {
                //it means this is not constants declaration block so we are returning null and no exceptions
                return null;
            }

            var constDecls = new List<AstNode>();
            NextToken();
            var constDecl = ParseConstDecl();
            if (constDecl == null)
            {
                //some parser exception
                return null;
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
            if (token == null || token.Type != TokenType.EqualOperator)
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
            var token = PeekToken();
            if (token.Type != TokenType.Type)
            {
                //it means this is not types declaration block so we are returning null and no exceptions
                return null;
            }

            NextToken();
            var typeDecls = new List<AstNode> {ParseTypeDecl()};
            if (typeDecls[0] == null)
            {
                //some parser exception
                return null;
            }

            do
            {
                var typeDecl = ParseTypeDecl();
                if (typeDecl == null) break;
                typeDecls.Add(typeDecl);
            } while (true);

            return new ConstDeclsPart(token, typeDecls);
        }

        private AstNode ParseTypeDecl()
        {
            var typeIdent = ParseIdent();
            var token = PeekToken();
            if (token == null || token.Type != TokenType.EqualOperator)
            {
                //some parser exception
                return null;
            }

            NextToken();
            var type = ParseType();
            var ttoken = PeekToken();
            if (ttoken == null || ttoken.Type != TokenType.Semicolon)
            {
                //some parser exception
                return null;
            }

            NextToken();
            return new TypeDecl(typeIdent, type);
        }

        private AstNode ParseVarDeclsPart()
        {
            var token = PeekToken();
            if (token.Type != TokenType.Var)
            {
                return null; //this is not var decl part
            }

            NextToken();
            var varDecls = new List<AstNode> {ParseVarDecl()};
            if (varDecls[0] == null)
            {
                throw new Exception(string.Format("{0}, {1} : Empty var block", PeekToken().Line, PeekToken().Column));
            }

            do
            {
                var varDecl = ParseVarDecl();
                if (varDecl == null) break;
                varDecls.Add(varDecl);
            } while (true);

            return new VarDeclsPart(token, varDecls);
        }

        private AstNode ParseVarDecl()
        {
            var varIdents = ParseIdentList();
            var token = PeekToken();
            if (token == null || token.Type != TokenType.Colon)
            {
                //some parser exception
                return null;
            }

            NextToken();
            var type = ParseType();

            if (varIdents.Count == 1)
            {
                if (PeekToken().Type == TokenType.EqualOperator)
                {
                    NextToken();
                    var expr = ParseExpression();
                    if (PeekToken() == null || PeekToken().Type != TokenType.Semicolon)
                    {
                        //some parser exception
                        return null;
                    }

                    NextToken();
                    return new InitVarDecl(varIdents[0], type, expr);
                }
            }

            if (PeekToken() == null || PeekToken().Type != TokenType.Semicolon)
            {
                //some parser exception
                return null;
            }

            NextToken();
            return new SimpleVarDecl(varIdents, type);
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
            var funcHeader = ParseFuncHeader();
            CheckToken(PeekToken().Type, new List<TokenType>{TokenType.Semicolon}, 
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found", 
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));

            var funcSubroutineBlock = ParseSubroutineBlock();
            CheckToken(PeekToken().Type, new List<TokenType>{TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found", 
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
            
            return new FuncDecl(funcHeader, funcSubroutineBlock);
        }

        private AstNode ParseFuncHeader()
        {
            CheckToken(PeekToken().Type, new List<TokenType>{TokenType.Function}, 
                string.Format("{0} {1} : syntax error, 'function' expected, but {2} found", 
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
            
            var funcName = ParseIdent();
            var paramList = ParseFormalParamList();
            
            CheckToken(PeekToken().Type, new List<TokenType>{TokenType.Colon},
                string.Format("{0} {1} : syntax error, ':' expected, but {2} found", 
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));

            var returnType = ParseSimpleType();
            
            return new FuncHeader(funcName, paramList, returnType);
        }

        private AstNode ParseSubroutineBlock()
        {
            if (PeekToken().Type == TokenType.Forward)
            {
                return new Forward(PeekAndNext());
            }

            var declsParts = new List<AstNode>();
            bool stopParse = false;
            while(!stopParse)
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
            
            CheckToken(PeekToken().Type, new List<TokenType>{TokenType.Semicolon}, 
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found", 
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));

            var procSubroutineBlock = ParseSubroutineBlock();
            
            CheckToken(PeekToken().Type, new List<TokenType>{TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found", 
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
            
            return new ProcDecl(procHeader, procSubroutineBlock);
        }

        private AstNode ParseProcHeader()
        {
            CheckToken(PeekToken().Type, new List<TokenType>{TokenType.Function}, 
                string.Format("{0} {1} : syntax error, 'procedure' expected, but {2} found", 
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
            
            var funcName = ParseIdent();
            var paramList = ParseFormalParamList();
            
            CheckToken(PeekToken().Type, new List<TokenType>{TokenType.Colon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found", 
                    PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
            
            return new ProcHeader(funcName, paramList);
        }
    }
}