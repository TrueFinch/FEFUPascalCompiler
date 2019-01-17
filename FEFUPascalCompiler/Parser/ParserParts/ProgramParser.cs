using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal partial class PascalParser
    {
        private AstNode ParseProgram()
        {
            var token = PeekToken();
            if (token.Type != TokenType.Program)
            {
                //some parser exception
                return null;
            }

            NextToken();
            var header = ParseIdent();

            token = PeekToken();
            if (token.Type != TokenType.Semicolon)
            {
                //some parser exception
                return null;
            }

            NextToken();
            var mainBlock = ParseMainBlock();

            token = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Dot},
                string.Format("{0}, {1} : syntax error, '.' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            return new Program(header, mainBlock);
        }

        private AstNode ParseMainBlock()
        {
            var declParts = ParseDeclsParts();
            var mainCompound = ParseCompoundStatement();
            return new MainBlock(declParts, mainCompound);
        }
    }
}