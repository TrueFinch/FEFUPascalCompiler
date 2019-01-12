﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using FEFUPascalCompiler.Lexer;
using FEFUPascalCompiler.Tokens;
using FEFUPascalCompiler.Parser;
using FEFUPascalCompiler.Parser.AstVisitor;

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

        public void Parse()
        {
            if (!_lexer.IsReady())
            {
                LastException = new Exception("Lexer is not ready or stopped. Don't panic and wait for help.");
            }

            if (!_parser.IsReady())
            {
                LastException = new Exception("Parser is not ready or stopped. Don't panic and wait for help.");
            }

            try
            {
                _ast = _parser.Parse();
            }
            catch (Exception e)
            {
                LastException = e;
            }
        }

        public void ParseSingleExpression()
        {
            if (!_lexer.IsReady())
            {
                LastException = new Exception("Lexer is not ready or stopped. Don't panic and wait for help.");
            }

            if (!_parser.IsReady())
            {
                LastException = new Exception("Parser is not ready or stopped. Don't panic and wait for help.");
            }

            try
            {
                _ast = _parser.ParseSingleExpression();
            }
            catch (Exception e)
            {
                LastException = e;
            }
        }

        public void PrintAst(StreamWriter output = null)
        {
            var astPrinter = _ast.Accept(new AstPrintVisitor());
            var canvas = new List<StringBuilder>();
            astPrinter.PrintTree(canvas);

            var maxWidth = 0;
            foreach (var strBuilder in canvas)
            {
                maxWidth = maxWidth > strBuilder.Length ? maxWidth : strBuilder.Length;
            }

            foreach (var stringBuilder in canvas)
            {
                stringBuilder.Insert(stringBuilder.Length, ".", maxWidth - stringBuilder.Length);
            }
            
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

        public bool Next()
        {
            return _lexer.NextToken();
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

        private NodeAst _ast;
        private LexerDfa _lexer;
        private Parser.Parser _parser;
        private StreamReader _input;
    } // FEFUPascalCompiler class
} // FEFUPascalCompiler namespace