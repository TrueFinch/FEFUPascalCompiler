using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FEFUPascalCompiler.Lexer;
using FEFUPascalCompiler.Tokens;
using FEFUPascalCompiler.Parser;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.ParserParts;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Parser.Visitors;

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
                if (WriteStackTrace)
                    Console.Out.WriteLine(e);
                else
                    Console.Out.WriteLine(e.Message);
                return null;
            }
        }

        public void Parse()
        {
            if (!_lexer.IsReady())
            {
                LastException = new Exception("Lexer is not ready or stopped. Don't panic and wait for help.");
            }

            if (!_pascalParser.IsReady())
            {
                LastException = new Exception("Parser is not ready or stopped. Don't panic and wait for help.");
            }

            try
            {
                _ast = _pascalParser.Parse();
            }
            catch (Exception e)
            {
                LastException = e;
                if (WriteStackTrace)
                    Console.Out.WriteLine(e);
                else
                    Console.Out.WriteLine(e.Message);
            }
        }

        public void ParseSingleExpression()
        {
            if (!_lexer.IsReady())
            {
                LastException = new Exception("Lexer is not ready or stopped. Don't panic and wait for help.");
            }

            if (!_pascalParser.IsReady())
            {
                LastException = new Exception("Parser is not ready or stopped. Don't panic and wait for help.");
            }

            try
            {
                _ast = _pascalParser.ParseSingleExpression();
            }
            catch (Exception e)
            {
                LastException = e;
                if (WriteStackTrace)
                    Console.Out.WriteLine(e);
                else
                    Console.Out.WriteLine(e.Message);
            }
        }

        public void PrintAst(StreamWriter output = null)
        {
            var astPrinter = _ast.Accept(new AstPrintVisitor());
            var canvas = new List<StringBuilder>();
            astPrinter.PrintTree(canvas);

            astPrinter.AlignBG(in canvas);

            foreach (var strBuilder in canvas)
            {
                if (output == null)
                {
                    Console.Out.WriteLine(strBuilder.ToString());
                }
                else
                {
                    output.WriteLine(strBuilder.ToString());
                }
            }
        }

        public void CheckSemantics()
        {
            try
            {
                _ast.Accept(new SymCheckVisitor(_pascalParser.SymbolStack));
            }
            catch (Exception e)
            {
                if (WriteStackTrace)
                    Console.Out.WriteLine(e);
                else
                    Console.Out.WriteLine(e.Message);
                LastException = e;
            }
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
                if (WriteStackTrace)
                    Console.Out.WriteLine(e);
                else
                    Console.Out.WriteLine(e.Message);
                return false;
            }
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

        public Token NextAndPeek()
        {
            Next();
            return Peek();
        }

        public StreamReader Input
        {
            get => _input;
            set
            {
                _input = value;
                _lexer.InitLexer(value);
                _pascalParser.InitParser();
            }
        }

        public Compiler()
        {
            _lexer = new LexerDfa();
            _pascalParser = new PascalParser(Peek, Next, PeekAndNext, NextAndPeek);
        }

        public Exception LastException = null;

        public bool TokenizeComments
        {
            get => _lexer.TokenizeComments;
            set => _lexer.TokenizeComments = value;
        }

        public bool WriteStackTrace { get; set; } = false;
        private AstNode _ast;
        private LexerDfa _lexer;
        private PascalParser _pascalParser;
        private StreamReader _input;
    } // FEFUPascalCompiler class
} // FEFUPascalCompiler namespace