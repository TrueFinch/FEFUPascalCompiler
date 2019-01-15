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
                {ParseConstDeclsPart, ParseTypeDeclsPart, ParseVarDeclsPart, ParseaProcFuncDeclsPart};
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

        private AstNode ParseTypeDeclsPart()
        {
            var token = PeekToken();
            if (token.Type != TokenType.Type)
            {
                //it means this is not types declaration block so we are returning null and no exceptions
                return null;
            }

            NextToken();
            var typeDecls = new List<AstNode>();
            typeDecls.Add(ParseTypeDecl());
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
            token = PeekToken();
            if (token == null || token.Type != TokenType.Semicolon)
            {
                //some parser exception
                return null;
            }

            NextToken();
            return new TypeDecl(typeIdent, type);
        }
        
        private AstNode ParseVariableDeclsPart()
        {
            var token = PeekToken();
            if (token.Type != TokenType.Var)
            {
                  return null; //this is not var decl part
            }

            NextToken();
            var varDecls = new List<AstNode>();
            varDecls.Add(ParseVarDeclsPart());
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
    }
}