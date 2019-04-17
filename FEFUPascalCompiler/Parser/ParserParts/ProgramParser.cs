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

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Program},
                string.Format("({0}, {1}) syntax error: 'program' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var header = ParseIdent();
//            CheckDuplicateIdentifier(header.Token);
//            _symbolTableStack.Peek().Add();


            token = PeekToken();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("({0}, {1}) syntax error: ';' expected, but '{2}' found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var mainBlock = ParseMainBlock();

            token = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Dot},
                string.Format("({0}, {1}) syntax error: '.' expected, but '{2}' found",
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