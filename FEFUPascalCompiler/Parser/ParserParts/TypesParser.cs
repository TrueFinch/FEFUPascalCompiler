using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal partial class PascalParser
    {
        private AstNode ParseType()
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

            throw new Exception(string.Format("{0}, {1} : syntax error, variable type expected, but '{2}' found",
                PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
        }

        private AstNode ParseSimpleType()
        {
            var typeIdent = ParseIdent();

//            var symType = CheckTypeDeclared(typeIdent.Token);

            return new SimpleTypeNode(typeIdent);
        }

        private AstNode ParseArrayType()
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

            return new ArrayTypeNode(indexRanges, type);
        }

        private List<IndexRangeNode> ParseIndexRanges()
        {
            var indexRange = ParseIndexRange();
            var astNodesIndexRanges = new List<IndexRangeNode>();
//            var symbolIndexRanges = new List<IndexRange<int, int>>();

            if (indexRange == null)
            {
                throw new Exception(string.Format("{0} {1} : syntax error, index range expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            }

//            symbolIndexRanges.Add(indexRange.Item1);
            astNodesIndexRanges.Add(indexRange);
            
            while (PeekToken().Type == TokenType.Comma)
            {
                NextToken();
                indexRange = ParseIndexRange();
                if (indexRange == null)
                {
                    throw new Exception(string.Format("{0} {1} : syntax error, index range expected, but {2} found",
                        PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
                }

//                symbolIndexRanges.Add(indexRange.Item1);
                astNodesIndexRanges.Add(indexRange);
            }

            return astNodesIndexRanges;
        }

        private IndexRangeNode ParseIndexRange()
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

            return new IndexRangeNode(token, leftBound, rightBound);
        }

        private AstNode ParseRecordType()
        {
            var token = PeekAndNext();
//            _symbolTableStack.Push();
            var fieldList = ParseFieldsList();
            token = PeekToken();
            
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.End},
                string.Format("{0} {1} : syntax error, 'end' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            
            return new RecordTypeNode(fieldList);
        }

        private List<FieldSectionNode> ParseFieldsList()
        {
            var fieldsList = new List<FieldSectionNode>();
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

        private FieldSectionNode ParseFieldSection()
        {
            var identList = ParseIdentList();
            if (identList == null || identList.Count == 0)
            {
                //this is empty ident list
                return null;
            }

            var token = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Colon},
                string.Format("{0} {1} : syntax error, ':' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            
            var fieldsType = ParseType();
            if (fieldsType== null)
            {
                throw new Exception(string.Format("{0} {1} : syntax error, field's type expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            }

//            foreach (var ident in identList)
//            {
//                CheckDuplicateIdentifier(ident.Token);
//                _symbolTableStack.AddVariable(ident.ToString(), new SymLocal(fieldsType.Item1));
//            }
            
            return new FieldSectionNode(token, identList, fieldsType);
        }

        private AstNode ParsePointerType()
        {
            var token = PeekAndNext();
            
            var simpleType = ParseSimpleType();
            if (simpleType == null)
            {
                //exception -- pointer must be on a simple type
                throw new Exception(string.Format("{0} {1} : syntax error, simple type expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            }

            return new PointerTypeNode(token, simpleType);
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