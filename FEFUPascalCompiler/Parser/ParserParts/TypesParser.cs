using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
//                case TokenType.Procedure:
//                {
//                    return ParseProcSignature();
//                }
//                case TokenType.Function:
//                {
//                    return ParseFuncSignature();
//                }
            }

            throw new Exception(string.Format("{0}, {1} : syntax error, variable type expected, but {2} found",
                PeekToken().Line, PeekToken().Column, NextAndPeek().Lexeme));
        }

        private (Type, AstNode) ParseSimpleType()
        {
            var typeIdent = ParseIdent();

            CheckTypeDeclared(typeIdent.Token);

            return (_symbolTableStack.Peek()[typeIdent.Token.Value] as Type, new SimpleType(typeIdent));
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

            var type = ParseType();

            return (new ArrayTypeSymbol(indexRanges.Item1, type.Item1), new ArrayTypeAstNode(indexRanges.Item2, type.Item2));
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

        private (Type, AstNode) ParseRecordType()
        {
            var token = PeekAndNext();
            _symbolTableStack.Push(new OrderedDictionary());
            var fieldList = ParseFieldsList();
            token = PeekToken();
            
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.End},
                string.Format("{0} {1} : syntax error, 'end' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            
            return (new RecordType(_symbolTableStack.Pop()), new RecordTypeAstNode(fieldList));
        }

        private List<AstNode> ParseFieldsList()
        {
            var fieldsList = new List<AstNode>();
            var fieldSection = ParseFieldSection();

            if (fieldSection == null)
            {
                //this is means that record does not have any fields
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
                //this is empty ident list
                return null;
            }

            var token = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Colon},
                string.Format("{0} {1} : syntax error, ':' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            
            var fieldsType = ParseType();
            if (fieldsType.Item1 == null && fieldsType.Item2 == null)
            {
                throw new Exception(string.Format("{0} {1} : syntax error, field's type expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            }

            foreach (var ident in identList)
            {
                CheckDuplicateIdentifier(ident.Token);
                _symbolTableStack.Peek().Add(ident.ToString(), new Local(fieldsType.Item1));
            }
            
            return new FieldSection(token, identList, fieldsType.Item2);
        }

        private (Type, AstNode) ParsePointerType()
        {
            var token = PeekAndNext();
            
            var simpleType = ParseSimpleType();
            if (simpleType.Item1 == null && simpleType.Item2 == null)
            {
                //exception -- pointer must be on a simple type
                throw new Exception(string.Format("{0} {1} : syntax error, simple type expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            }

            return (new PointerTypeSumbol(simpleType.Item1), new PointerType(token, simpleType.Item2));
        }

//        private AstNode ParseProcedureType()
//        {
//            var funcSignature = ParseFuncSignature();
//            if (funcSignature != null)
//            {
//                return funcSignature;
//            }
//
//            var procSignature = ParseProcSignature();
//            if (procSignature != null)
//            {
//                return procSignature;
//            }
//
//            return null; // this is not func or proc signature
//        }

//        private AstNode ParseFuncSignature()
//        {
//            var token = PeekToken();
//            if (token.Type != TokenType.Function)
//            {
//                return null; // this is not func signature
//            }
//
//            NextToken();
//            var formalParamList = ParseFormalParamList();
//
//            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Colon},
//                string.Format("{0} {1} : syntax error, ':' expected, but {2} found",
//                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
//
//            var returnType = ParseSimpleType();
//            return new FuncSignature(token, formalParamList, returnType);
//        }

//        private AstNode ParseProcSignature()
//        {
//            var token = PeekToken();
//            if (token == null || token.Type != TokenType.Procedure)
//            {
//                return null; // this is not func signature
//            }
//
//            NextToken();
//            var formalParamList = ParseFormalParamList();
//
//            return new ProcSignature(token, formalParamList);
//        }
    }
}