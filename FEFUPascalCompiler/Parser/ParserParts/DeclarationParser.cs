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
            List<AstNode> declParts = new List<AstNode>();
            var declParsers = new List<DeclPartParser>
                {ParseConstDeclsPart, ParseTypeDeclsPart, ParseVarDeclsPart, ParseProcFuncDeclsPart};
            bool partsExist;
            do
            {
                partsExist = false;
                foreach (var declParser in declParsers)
                {
                    var tmp = declParser();
                    if (tmp == null) continue;
                    declParts.Add(tmp);
                    partsExist = true;
                }
            } while (partsExist);

            return declParts;
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
                //exception - variable declaration expected but not found
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
                var funcDecl = ParseFuncDecl();
                if (funcDecl != null)
                {
                    declarations.Add(funcDecl);
                    stopParse = false;
                }

                var procDecl = ParseProcDecl();
                if (procDecl == null) continue;
                declarations.Add(procDecl);
                stopParse = false;
            }
        }

        private AstNode ParseFuncDecl()
        {
            throw new NotImplementedException();
        }

        private AstNode ParseProcDecl()
        {
            throw new NotImplementedException();
        }
    }
}