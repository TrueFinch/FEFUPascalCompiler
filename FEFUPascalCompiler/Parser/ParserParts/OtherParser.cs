using System.Collections.Generic;
using System.Net.Http.Headers;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal partial class PascalParser
    {
        public AstNode ParseFormalParamList()
        {
            var token = PeekToken();
            if (token == null)
            {
                //exception
                return null;
            }

            var paramSections = new List<AstNode>();

            while (true)
            {
                paramSections.Add(ParseFormalParamSection());
                if (paramSections[0] == null)
                {
                    return null; // param list is empty
                }
            }
        }

        private AstNode ParseFormalParamSection()
        {
            var token = PeekToken();
            if (token == null)
            {
                //exception
                return null;
            }


            AstNode modifier = null;
            if (PeekToken() != null && (PeekToken().Type == TokenType.Var
                || PeekToken().Type == TokenType.Const || PeekToken().Type == TokenType.Out))
            {
                modifier = new Modifier(PeekToken());
            }

            NextToken();
            var identsList = ParseIdentList();

            if (PeekToken() == null || PeekToken().Type != TokenType.Colon)
            {
                //exception -- syntax error
                return null;
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
        }

        public List<AstNode> ParseIdentList()
        {
            var identList = new List<AstNode> {ParseIdent()};
            if (identList[0] == null)
            {
                //exception -- this is not ident list
                return null;
            }

            while (PeekToken().Type == TokenType.Comma)
            {
                var ident = ParseIdent();
                if (ident == null)
                {
                    //exception unexpected lexeme
                    return null;
                }

                identList.Add(ident);
                NextToken();
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
            return new ConstIntegerLiteral(PeekAndNext());
        }
    }
}