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
        public bool IsReady() => _isReady;

        public AstNode ParseSingleExpression()
        {
            NextToken(); // skipping null token so next call will return not null token if it's exist
            return ParseExpression();
        }

        public AstNode Parse()
        {
            _isReady = false;
            if (PeekToken() != null)
            {
                //throw exception wrong init of lexer
                return null;
            }

            NextToken(); // skipping null token so next call will return not null token if it's exist
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
        public SymbolStack SymbolStack => _symbolTableStack;

        private SymbolStack _symbolTableStack = new SymbolStack();

        private void CheckToken(TokenType actual, List<TokenType> expected, string errMessage)
        {
            if (!expected.Contains(actual))
                throw new Exception(errMessage);
        }

        public void InitParser()
        {
            _symbolTableStack = new SymbolStack();
            _isReady = true;
        }
    }
}