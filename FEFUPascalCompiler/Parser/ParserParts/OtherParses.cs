using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal partial class PascalParser
    {
        public AstNode ParseFormalParamList()
        {
            
        }
        
        public AstNode ParseIdentList()
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

            return new IdentList(identList);
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
    }
}