using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser
{
    internal partial class Parser
    {
        private AstNode ParseTypeDeclsPart()
        {
            var token = PeekToken();
            if (token.Type != TokenType.Type)
            {
                //it means this is not types declaration block so we are returning null and no exceptions
                return null;
            }

            var constDecls = new List<AstNode>();
            NextToken();
            var constDecl = ParseTypeDecl();
            if (constDecl == null)
            {
                //some parser exception
                return null;
            }

            constDecls.Add(constDecl);
            do
            {
                constDecl = ParseTypeDecl();
                if (constDecl == null) break;
                constDecls.Add(constDecl);
            } while (true);

            return new ConstDeclsPart(constDecls);
        }

        private AstNode ParseTypeDecl()
        {
            var constIdent = ParseIdent();
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
            return new ConstDecl(constIdent, type);
        }

        private AstNode ParseType()
        {
            var typesParsers = new List<TypesParser>
                {ParseSimpleTest, ParseArrayType, ParseRecordType, ParsePointerType, ParseProcedureType};
            foreach (var tp in typesParsers)
            {
                var type = tp();
                if (type != null)
                {
                    return type;
                }
            }

            //some parser exception
            return null;
        }

        private AstNode ParseSimpleTest()
        {
            var ident = ParseIdent();
            if (ident == null)
            {
                return null; // this is not simple type
            }

            return ident;
        }

        private AstNode ParseArrayType()
        {
            var token = PeekToken();
            if (token.Type != TokenType.Array)
            {
                return null; // this is not array type
            }

            token = NextAndPeek();
            if (token.Type != TokenType.OpenSquareBracket)
            {
                return null; //some parser exception -- this already mistake
            }

            NextToken();
            var index_ranges = ParseIndexRanges();
            token = PeekToken();
            if (token.Type != TokenType.OpenSquareBracket)
            {
                return null; //some parser exception -- this already mistake
            }

            token = NextAndPeek();
            if (token.Type != TokenType.Of)
            {
                return null; //some parser exception -- this already mistake
            }

            var type = ParseType();
            if (type == null)
            {
                //some parser exception
            }

            return new ArrayType(index_ranges, type);
        }

        private List<AstNode> ParseIndexRanges()
        {
            var indexRange = ParseIndexRange();

            if (indexRange == null)
            {
                //some parser exception -- need at list one list range here
                return null;
            }

            var indexRanges = new List<AstNode>();
            indexRanges.Add(indexRange);

            do
            {
                var token = PeekToken();
                if (token.Type != TokenType.Comma)
                {
                    //some parser exception -- no comma (and no hommo)
                    return null;
                }

                indexRange = ParseIndexRange();
                if (indexRange == null) break;
                indexRanges.Add(indexRange);
            } while (true);

            return indexRanges;
        }

        private AstNode ParseIndexRange()
        {
            var leftBound = ParseDecInt();
            if (leftBound == null)
            {
                //exception -- wrong range bounds 
                return null;
            }

            var token = PeekToken();
            if (token.Type != TokenType.DoubleDotOperator)
            {
                //exception -- no double dot
                return null;
            }

            NextToken();
            var rightBound = ParseDecInt();
            if (rightBound == null)
            {
                //exception -- wrong range bounds 
                return null;
            }

            return new IndexRange(token, leftBound, rightBound);
        }

        private AstNode ParseRecordType()
        {
            var token = PeekToken();
            if (token.Type != TokenType.Record)
            {
                return null; //this is not record type
            }

            NextToken();
            var fieldList = ParseFieldsList();
            token = PeekToken();
            if (token.Type != TokenType.End)
            {
                return null; //this is not record type
            }

            NextToken();
            return RecordType(fieldList);
        }

        private AstNode ParseFieldsList()
        {
            
        }

        private AstNode ParsePointerType()
        {
            throw new NotImplementedException();
        }

        private AstNode ParseProcedureType()
        {
            throw new NotImplementedException();
        }
    }
}