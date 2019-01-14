using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal partial class PascalParser
    {
        private AstNode ParseType()
        {
            var typesParsers = new List<TypesParser>
                {ParseSimpleType, ParseArrayType, ParseRecordType, ParsePointerType, ParseProcedureType};
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

        private AstNode ParseSimpleType()
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
            var indexRanges = new List<AstNode> {ParseIndexRange()};

            if (indexRanges[0] == null)
            {
                //some parser exception -- need at list one list range here
                return null;
            }
            
            while (PeekToken().Type == TokenType.Comma)
            {
                var indexRange = ParseIndexRange();
                if (indexRange == null)
                {
                    //exception unexpected lexeme
                }
                indexRanges.Add(indexRange);
                NextToken();
            }

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
            return new RecordType(fieldList);
        }

        private List<AstNode> ParseFieldsList()
        {
            var fieldsList = new List<AstNode> {ParseFieldSection()};

            if (fieldsList[0] == null)
            {
                //some parser exception -- need at list one list range here
                return null;
            }
            
            while (PeekToken().Type == TokenType.Comma)
            {
                var fieldSection = ParseFieldSection();
                if (fieldSection == null)
                {
                    //exception unexpected lexeme
                }
                fieldsList.Add(fieldSection);
                NextToken();
            }

            return fieldsList;
        }

        private AstNode ParseFieldSection()
        {
            var identList = ParseIdentList();
            if (identList == null)
            {
                //exception -- empty ident list
                return null;
            }

            var token = PeekToken();
            if (token.Type != TokenType.Colon)
            {
                //exception -- no double dot
                return null;
            }

            NextToken();
            var fieldsType = ParseType();
            if (fieldsType == null)
            {
                //exception -- wrong range bounds 
                return null;
            }

            return new FieldSection(token, identList, fieldsType);
        }

        private AstNode ParsePointerType()
        {
            var token = PeekToken();
            if (token.Type != TokenType.Carriage)
            {
                return null; //this is not pointer type
            }

            NextToken();
            var simpleType = ParseSimpleType();
            if (simpleType == null)
            {
                //exception -- pointer must be on a simple type
                return null;
            }

            return new PointerType(token, simpleType);
        }

        private AstNode ParseProcedureType()
        {
            var funcSignature = ParseFuncSignature();
            if (funcSignature != null)
            {
                return funcSignature;
            }

            var procSignature = ParseProcSignature();
            if (procSignature != null)
            {
                return procSignature;
            }

            return null; // this is not func or proc signature
        }

        private AstNode ParseFuncSignature()
        {
            var token = PeekToken();
            if (token == null || token.Type != TokenType.Function)
            {
                return null; // this is not func signature
            }

            var formalParamList = ParseFormalParamList();
            
        }

        private AstNode ParseProcSignature()
        {
            throw new NotImplementedException();
        }
    }
}