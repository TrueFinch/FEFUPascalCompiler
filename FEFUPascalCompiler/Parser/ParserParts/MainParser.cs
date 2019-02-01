using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal delegate Token PeekAndNext();

    internal delegate Token NextAndPeek();

    internal delegate Token PeekToken();

    internal delegate bool NextToken();

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

//            _symbolTableStack.Push(new OrderedDictionary());
            NextToken();
            return ParseProgram();
        }

        public PascalParser(PeekToken peekToken, NextToken nextToken, PeekAndNext peekAndNext, NextAndPeek nextAndPeek)
        {
            PeekToken = peekToken;
            NextToken = nextToken;
            PeekAndNext = peekAndNext;
            NextAndPeek = nextAndPeek;

//            InitSymbolTableStack();
        }

        private bool _isReady = true;

        private PeekToken PeekToken { get; }
        private NextToken NextToken { get; }
        private PeekAndNext PeekAndNext { get; }
        private NextAndPeek NextAndPeek { get; }
        
//        private Stack<OrderedDictionary> _symbolTableStack = new Stack<OrderedDictionary>();
        public SymbolStack SymbolStack => _symbolTableStack;

        private SymbolStack _symbolTableStack = new SymbolStack();
//        private void InitSymbolTableStack()
//        {
//            var mainTable = new OrderedDictionary();
//            mainTable.Add("Integer".ToLower(), new IntegerSymbolType());
//            mainTable.Add("Float".ToLower(), new FloatSymbolType());
//            mainTable.Add("String".ToLower(), new StringSymbolType());
//            mainTable.Add("Char".ToLower(), new CharSymbolType());
//            _symbolTableStack.Push(mainTable);
//        }

        private void CheckToken(TokenType actual, List<TokenType> expected, string errMessage)
        {
            if (!expected.Contains(actual))
                throw new Exception(errMessage);
        }

        private void CheckDuplicateIdentifier(Token ident)
        {
            if (_symbolTableStack.Find(ident.Value) != null)
            {
                throw new Exception(string.Format("{0}, {1} : Duplicate identifier '{2}'",
                    ident.Line, ident.Column, ident.Lexeme));
            }
        }

        private void CheckDuplicateIdentifierInScope(Token ident)
        {
            if (_symbolTableStack.FindInScope(ident.Value) != null)
            {
                throw new Exception(string.Format("{0}, {1} : Duplicate identifier '{2}'",
                    ident.Line, ident.Column, ident.Lexeme));
            }
        }
        
        private SymType CheckTypeDeclared(Token type)
        {
            var symType = _symbolTableStack.FindType(type.Value);

            if (_symbolTableStack.FindType(type.Value) == null)
            {
                throw new Exception(string.Format("{0}, {1} : Undeclared type identifier '{2}'",
                    type.Line, type.Column, type.Lexeme));
            }

            return symType;
        }

//        private bool CheckTypeDeclared(string typeIdent)
//        {
//            return _symbolTableStack.fin.Contains(typeIdent);
//        }

        private SymVar FindIdent(Token identToken)
        {
            var ident = _symbolTableStack.FindIdent(identToken.Value);
            if (ident != null)
            {
                return ident;
            }

            throw new Exception(string.Format("{0}, {1} : Undeclared variable identifier '{2}'",
                identToken.Line, identToken.Column, identToken.Lexeme));
        }
        
        public void InitParser()
        {
            _symbolTableStack = new SymbolStack();
//            InitSymbolTableStack();
            _isReady = true;
        }
    }
}