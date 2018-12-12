using System;
using System.IO;
using FEFUPascalCompiler.Lexer;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler
{
    public class Compiler
    {
        private LexerDfa _lexer;
        private StreamReader _input;

        public Exception LastException = null;

        public Compiler()
        {
            _lexer = new LexerDfa();
        }
        
        public bool Next()
        {
            try
            {
                return _lexer.NextToken();
            }
            catch (Exception e)
            {
                LastException = e;
                return false;
            }
        }

        public Token Peek()
        {
            return _lexer.PeekToken();
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
    } // FEFUPascalCompiler class
} // FEFUPascalCompiler namespace