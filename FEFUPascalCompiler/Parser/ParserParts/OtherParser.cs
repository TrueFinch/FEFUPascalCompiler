using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;

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

        private (SymbolType, AstNode) ParseParamType()
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
    }
}