using System;
using System.IO;
using FEFUPascalCompiler.Lexer;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler
{
    public class FEFUPascalCompiler
    {
        private LexerDfa _lexer;

        public FEFUPascalCompiler()
        {
            _lexer = new LexerDfa();
        }

        public void SetInput(string inputFilePath)
        {
            try
            {
                var inputStream = File.OpenText(inputFilePath);
                _lexer.SetInput(ref inputStream);
//                Next();
            }
            catch (FileNotFoundException exception)
            {
                Console.WriteLine(exception);
            }
        }

        public bool Next()
        {
            return _lexer.NextToken();
        }

        public Token Peek()
        {
            return _lexer.PeekToken();
        }
    }
}