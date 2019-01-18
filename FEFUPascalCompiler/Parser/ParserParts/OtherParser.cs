using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;
using Type = FEFUPascalCompiler.Parser.Sematics.Type;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal partial class PascalParser
    {
        public List<AstNode> ParseFormalParamList()
        {
            var token = PeekToken();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.OpenBracket},
                string.Format("{0} {1} : syntax error, '(' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var paramSections = new List<AstNode>();

            while (true)
            {
                var section = ParseFormalParamSection();
                if (section == null)
                {
                    break; // param list is empty
                }

                paramSections.Add(section);

                if (PeekToken().Type != TokenType.Semicolon)
                {
                    break;
                }

                NextToken();
            }

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.CloseBracket},
                string.Format("{0} {1} : syntax error, ')' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            return paramSections;
        }

        private AstNode ParseFormalParamSection()
        {
            var token = PeekToken();

            AstNode modifier = null;
            if ((PeekToken().Type == TokenType.Var
                 || PeekToken().Type == TokenType.Const
                 || PeekToken().Type == TokenType.Out))
            {
                modifier = new Modifier(PeekAndNext());
            }

            var identsList = ParseIdentList();
            foreach (var ident in identsList)
            {
                CheckDuplicateIdentifier(ident.Token);
            }

            if (PeekToken() == null || PeekToken().Type != TokenType.Colon)
            {
                return null; //section is empty
            }

            var paramType = ParseParamType();

            foreach (var ident in identsList)
            {
                _symbolTableStack.Peek().Add(ident.ToString(), new Parameter(paramType.Item1, modifier?.ToString()));
            }

            return new FormalParamSection(identsList, paramType.Item2, modifier);
        }

        private (Type, AstNode) ParseParamType()
        {
            NextToken();
            switch (PeekToken().Type)
            {
                case TokenType.Ident:
                {
                    return ParseSimpleType();
                }
                case TokenType.Array:
                {
                    return ParseConformatArray();
                }
                default:
                {
                    throw new Exception(string.Format("{0}, {1} : syntax error, parameter type expected, but {2} found",
                        PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
                }
            }
        }

        private (Type, AstNode) ParseConformatArray()
        {
            var arrayToken = PeekAndNext();
            var ofToken = PeekAndNext();
            if (arrayToken.Type == TokenType.Array && ofToken.Type == TokenType.Of)
            {
                var simpleType = ParseSimpleType();
                return (new ConformatArrayType(simpleType.Item1),
                    new ConformantArray(arrayToken, ofToken, simpleType.Item2));
            }

            throw new Exception(string.Format("{0}, {1} : syntax error, conformat array type expected, but {2} found",
                PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
        }

        public List<AstNode> ParseIdentList()
        {
            var identList = new List<AstNode>();
            var ident = ParseIdent();
            if (ident == null)
            {
                //exception -- this is not ident list
                return identList;
            }

            identList.Add(ident);
            while (true)
            {
                if (PeekToken().Type != TokenType.Comma)
                {
                    break;
                }

                NextToken();
                ident = ParseIdent();
                if (ident == null)
                {
                    //exception unexpected lexeme
                    return null;
                }

                identList.Add(ident);
            }

            return identList;
        }

        private AstNode ParseIdent()
        {
            var token = PeekToken();
            if (token.Type != TokenType.Ident)
            {
                //this is not ident, may be this is key word? Think about it
                return null;
            }

            return new Ident(PeekAndNext());
        }

        private AstNode ParseConstIntegerLiteral()
        {
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.IntegerNumber},
                string.Format("{0} {1} : syntax error, integer expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekToken().Lexeme));
            return new ConstIntegerLiteral(PeekAndNext());
        }

        private AstNode ParseConstFloatLiteral()
        {
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.FloatNumber},
                string.Format("{0} {1} : syntax error, float expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekToken().Lexeme));
            return new ConstIntegerLiteral(PeekAndNext());
        }
    }
}