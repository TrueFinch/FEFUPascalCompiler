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
        private (Type, AstNode) ParseType()
        {
            switch (PeekToken().Type)
            {
                case TokenType.Ident:
                {
                    return ParseSimpleType();
                }
                case TokenType.Array:
                {
                    return ParseArrayType();
                }
                case TokenType.Record:
                {
                    return ParseRecordType();
                }
                case TokenType.Carriage:
                {
                    return ParsePointerType();
                }
                case TokenType.Procedure:
                {
                    return ParseProcSignature();
                }
                case TokenType.Function:
                {
                    return ParseFuncSignature();
                }
            }

            throw new Exception(string.Format("{0}, {1} : syntax error, variable type expected, but {2} found",
                PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
        }

        private (Type, AstNode) ParseSimpleType()
        {
            var typeIdent = ParseIdent();

            CheckTypeDeclared(typeIdent.Token);

            return (_symbolTableStack.Peek()[typeIdent.Token.Value] as Type, typeIdent);
        }

        private (Type, AstNode) ParseArrayType()
        {
            var token = PeekAndNext();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.OpenSquareBracket},
                string.Format("{0} {1} : syntax error, '[' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var indexRanges = ParseIndexRanges();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.CloseSquareBracket},
                string.Format("{0} {1} : syntax error, ']' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Of},
                string.Format("{0} {1} : syntax error, '[' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            (Type, AstNode) type;
            type = ParseType();

            return (new ArrayType(indexRanges.Item1, type.Item1), new ArrayTypeAstNode(indexRanges.Item2, type.Item2));
        }

        private (List<IndexRange<int, int>>, List<AstNode>) ParseIndexRanges()
        {
            var indexRange = ParseIndexRange();
            var astNodesIndexRanges = new List<AstNode>();
            var symbolIndexRanges = new List<IndexRange<int, int>>();

            if (indexRange.Item1 == null || indexRange.Item2 == null)
            {
                throw new Exception(string.Format("{0} {1} : syntax error, index range expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            }


            while (PeekToken().Type == TokenType.Comma)
            {
                NextToken();
                indexRange = ParseIndexRange();
                if (indexRange.Item1 == null || indexRange.Item2 == null)
                {
                    throw new Exception(string.Format("{0} {1} : syntax error, index range expected, but {2} found",
                        PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
                }

                symbolIndexRanges.Add(indexRange.Item1);
                astNodesIndexRanges.Add(indexRange.Item2);
            }

            return (symbolIndexRanges, astNodesIndexRanges);
        }

        private (IndexRange<int, int>, AstNode) ParseIndexRange()
        {
            var leftBound = ParseConstIntegerLiteral();
            if (leftBound == null)
            {
                throw new Exception(string.Format("{0} {1} : syntax error, index range expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            }

            var token = PeekToken();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.DoubleDotOperator},
                string.Format("{0} {1} : syntax error, ':' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var rightBound = ParseConstIntegerLiteral();
            if (rightBound == null)
            {
                throw new Exception(string.Format("{0} {1} : syntax error, index range expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            }

            return (
                new IndexRange<int, int>((leftBound.Token as IntegerNumberToken).NumberValue,
                    (rightBound.Token as IntegerNumberToken).NumberValue),
                new IndexRangeAstNode(token, leftBound, rightBound));
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
            return new RecordTypeAstNode(fieldList);
        }

        private List<AstNode> ParseFieldsList()
        {
            var fieldsList = new List<AstNode>();
            var fieldSection = ParseFieldSection();

            if (fieldSection == null)
            {
                return fieldsList;
            }

            fieldsList.Add(fieldSection);
            while (PeekToken().Type == TokenType.Semicolon)
            {
                NextToken();
                fieldSection = ParseFieldSection();
                if (fieldSection == null)
                {
                    break;
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
            if (token.Type != TokenType.Function)
            {
                return null; // this is not func signature
            }

            NextToken();
            var formalParamList = ParseFormalParamList();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Colon},
                string.Format("{0} {1} : syntax error, ':' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var returnType = ParseSimpleType();
            return new FuncSignature(token, formalParamList, returnType);
        }

        private AstNode ParseProcSignature()
        {
            var token = PeekToken();
            if (token == null || token.Type != TokenType.Procedure)
            {
                return null; // this is not func signature
            }

            NextToken();
            var formalParamList = ParseFormalParamList();

            return new ProcSignature(token, formalParamList);
        }
    }
}