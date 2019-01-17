using System.Collections.Generic;
using System.Net.Http.Headers;
using FEFUPascalCompiler.Parser.AstNodes;
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
                                        || PeekToken().Type == TokenType.Const || PeekToken().Type == TokenType.Out))
            {
                modifier = new Modifier(PeekAndNext());
            }

            var identsList = ParseIdentList();

            if (PeekToken() == null || PeekToken().Type != TokenType.Colon)
            {
                return null; //section is empty
            }

            var paramType = ParseParamType();

            return new FormalParamSection(identsList, paramType, modifier);
        }

        private AstNode ParseParamType()
        {
            if (PeekToken() == null)
            {
                //exception unexpected end of file
                return null;
            }

            NextToken();
            switch (PeekToken().Type)
            {
                case TokenType.Ident:
                {
                    return new SimpleType(ParseIdent());
                }
                case TokenType.Array:
                {
                    return ParseConformatArray();
                }
                default:
                {
                    //exception -- syntax error
                    return null;
                }
            }
        }

        private AstNode ParseConformatArray()
        {
            if (PeekToken() == null)
            {
                //exception unexpected end of file
                return null;
            }

            var arrayToken = PeekAndNext();
            var ofToken = PeekAndNext();
            if (arrayToken.Type == TokenType.Array && ofToken.Type == TokenType.Of)
            {
                return new ConformantArray(arrayToken, ofToken, ParseSimpleType());
            }

            //exception -- syntax error
            return null;
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
    }
}