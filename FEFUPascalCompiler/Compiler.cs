using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using FEFUPascalCompiler.Lexer;
using FEFUPascalCompiler.Tokens;
using FEFUPascalCompiler.Parser;

//TODO: change logic of Peek Next - we want that on start we have valid token got by Peek and that already use Next

namespace FEFUPascalCompiler
{
    public class Compiler
    {
        public List<Token> Tokenize()
        {
            if (!_lexer.IsReady())
            {
                LastException = new Exception("Lexer is not ready or stopped. Don't panic and wait for help.");
                return null;
            }
            List<Token> tokens = new List<Token>();
            try
            {
                while (Next())
                {
                    tokens.Add(Peek());
                }
                return tokens;
            }
            catch (Exception e)
            {
                LastException = e;
                return null;
            }
        }

        public NodeAst Parse()
        {
            if (!_lexer.IsReady())
            {
                LastException = new Exception("Lexer is not ready or stopped. Don't panic and wait for help.");
                return null;
            }

            if (!_parser.IsReady())
            {
                LastException = new Exception("Parser is not ready or stopped. Don't panic and wait for help.");
                return null;
            }
            
            try
            {
                return _parser.Parse();
            }
            catch (Exception e)
            {
                LastException = e;
                return null;
            }
        }
        
        public NodeAst ParseSingleExpression()
         {
            if (!_lexer.IsReady())
            {
                LastException = new Exception("Lexer is not ready or stopped. Don't panic and wait for help.");
                return null;
            }

            if (!_parser.IsReady())
            {
                LastException = new Exception("Parser is not ready or stopped. Don't panic and wait for help.");
                return null;
            }
            
            try
            {
                return _parser.ParseSingleExpression();
            }
            catch (Exception e)
            {
                LastException = e;
                return null;
            }
        }
        
        public bool Next()
        {
//            try
//            {
                return _lexer.NextToken();
//            }
//            catch (Exception e)
//            {
//                LastException = e;
//                return false;
//            }
        }

        public Token Peek()
        {
            return _lexer.PeekToken();
        }

        public Token PeekAndNext()
        {
            var t = Peek();
            Next();
            return t;
        }

        public StreamReader Input
        {
            get => _input;
            set
            {
                _input = value;
                _lexer.InitLexer(value);
            }
                
        }

        public Compiler()
        {
            _lexer = new LexerDfa();
            _parser = new Parser.Parser(Peek, Next, PeekAndNext);
        }
        
        public Exception LastException = null;
        
        private LexerDfa _lexer;
        private Parser.Parser _parser;
        private StreamReader _input;
    } // FEFUPascalCompiler class
} // FEFUPascalCompiler namespace