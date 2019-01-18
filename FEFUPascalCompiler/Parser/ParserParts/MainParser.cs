using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal delegate Token PeekAndNext();

    internal delegate Token NextAndPeek();

    internal delegate Token PeekToken();

    internal delegate bool NextToken();

    internal delegate AstNode DeclPartParser();

    internal delegate AstNode TypesParser();

    internal partial class PascalParser
    {
        public bool IsReady()
        {
            bool res = _isReady;
            _isReady = false;
            return res;
        }

        public AstNode ParseSingleExpression()
        {
            NextToken(); // skipping null token so next call will return not null token if it's exist
            return ParseExpression();
        }

        public AstNode Parse()
        {
            if (PeekToken() != null)
            {
                //throw exception wrong init of lexer
                return null;
            }
            _symbolTableStack.Push(new OrderedDictionary());
            NextToken();
            return ParseProgram();
        }

        public PascalParser(PeekToken peekToken, NextToken nextToken, PeekAndNext peekAndNext, NextAndPeek nextAndPeek)
        {
            PeekToken = peekToken;
            NextToken = nextToken;
            PeekAndNext = peekAndNext;
            NextAndPeek = nextAndPeek;
        }

        private bool _isReady = true;
        
        private PeekToken PeekToken { get; }
        private NextToken NextToken { get; }
        private PeekAndNext PeekAndNext { get; }
        private NextAndPeek NextAndPeek { get; }

        private Stack<OrderedDictionary> _symbolTableStack = new Stack<OrderedDictionary>();

        private void InitSymbolTableStack()
        {
            var mainTable = new OrderedDictionary();
            mainTable.Add("Integer".ToLower(), new IntegerType());
            mainTable.Add("Float".ToLower(), new FloatType());
            mainTable.Add("String".ToLower(), new StringType());
            mainTable.Add("Char".ToLower(), new CharType());
        }
        
        private void CheckToken(TokenType actual, List<TokenType> expected, string errMessage)
        {
            if (!expected.Contains(actual))
                throw new Exception(errMessage);
        }

        private bool CheckToken(TokenType actual, List<TokenType> expected)
        {
            return expected.Contains(actual);
        }

        private void CheckDuplicateIdentifier(Token ident)
        {
            if (_symbolTableStack.Peek().Contains(ident.Value))
            {
                throw new Exception(string.Format("{0}, {1} : Duplicate identifier {3}", 
                    ident.Line, ident.Column, ident.Lexeme));
            }
        }

        private void CheckTypeDeclared(Token type)
        {
            if (!_symbolTableStack.Peek().Contains(type.Value))
            {
                throw new Exception(string.Format("{0}, {1} : Undeclared type identifier {3}",
                    type.Line, type.Column, type.Lexeme));
            }
        }

        private bool CheckTypeDeclared(string typeIdent)
        {
            return _symbolTableStack.Peek().Contains(typeIdent);
        }
    }
}