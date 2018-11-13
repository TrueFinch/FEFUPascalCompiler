using System;
using System.Diagnostics.Tracing;
using System.IO;
using NUnit.Framework;
using FEFUPascalCompiler;
using FEFUPascalCompiler.Lexer;
using FEFUPascalCompiler.Tokens;

namespace Tests
{
    [TestFixture]
    public class LexerTests
    {
        private void WriteTokenToConsole(Token t)
        {
            Console.WriteLine("{0},{1}\t{2}\t\t{3}\t\t{4}",
                t.Line.ToString(),
                t.Column.ToString(),
                t.TokenType.ToString(),
                t.StrValue,
                t.Text);
        }

        private struct Row
        {
            public int Line, Column;
            public TokenType Type;
            public string StrValue, Text;

            public Row(string s)
            {
                if (s == null)
                {
                    this = default(Row);
                    return;
                }

                string[] words = s.Split(' ');
                Line = int.Parse(words[0].Split(',')[0]);
                Column = int.Parse(words[0].Split(',')[1]);
                Type = Enum.Parse<TokenType>(words[1]);
                StrValue = words[2];
                Text = words[3];
            }

            public bool Empty()
            {
                return StrValue == null;
            }
        }

        private void InitStream(out StreamReader sr1, string path1)
        {
            sr1 = File.OpenText(path1);
            if (sr1 == null)
            {
                Assert.Fail($"File by path {path1} does not exist");
            }
        }

        private void InitStreams(out StreamReader sr1, out StreamReader sr2, string path1, string path2)
        {
            InitStream(out sr1, path1);
            InitStream(out sr2, path2);
        }

        private void AreEqual(Token t, Row expectedToken)
        {
            Assert.AreEqual(t.Line, expectedToken.Line);
            Assert.AreEqual(t.Column, expectedToken.Column);
            Assert.AreEqual(t.TokenType, expectedToken.Type);
            Assert.AreEqual(t.StrValue, expectedToken.StrValue);
            Assert.AreEqual(t.Text, expectedToken.Text);
        }

        private void TryFail(StreamReader fileIn, StreamReader fileOut)
        {
            Token token;
            do
            {
                token = _compiler.Peek();
                var l = fileOut.ReadLine();
                Row expectedToken = new Row(l);
                if (expectedToken.Empty())
                {
                    Assert.Fail(".out file does not contain information about token");
                }

                AreEqual(token, expectedToken);
            } while (_compiler.Next());
        }

        private FEFUPascalCompiler.FEFUPascalCompiler _compiler;

        [OneTimeSetUp]
        public void Setup()
        {
            _compiler = new FEFUPascalCompiler.FEFUPascalCompiler();
        }

        [Test]
        public void SimpleTest()
        {
            InitStreams(out var fileIn, out var fileOut, @"LexerTests/test_00.in", @"LexerTests/test_00.out");
            _compiler.SetInput(fileIn);
            TryFail(fileIn, fileOut);

            Assert.Pass();
        }

        [Test]
        public void DecimalIntegerConstantsTest()
        {
            InitStreams(out var fileIn, out var fileOut, @"LexerTests/test_01.in", @"LexerTests/test_01.out");
            _compiler.SetInput(fileIn);
            TryFail(fileIn, fileOut);
            Assert.Pass();
        }

        [Test]
        public void TooLongDecimalIntegerConstantsTest()
        {
            InitStream(out var fileIn, @"LexerTests/test_02.in");
            _compiler.SetInput(fileIn);
            while (fileIn.EndOfStream)
            {
                Assert.Throws<StrToIntConvertException>(() => { _compiler.Next(); });
            }

            Assert.Pass();
        }

        [Test]
        public void InvalidDecimalIntegerConstantsTest()
        {
            InitStream(out var fileIn, @"LexerTests/test_03.in");
            _compiler.SetInput(fileIn);
            while (fileIn.EndOfStream)
            {
                Assert.Throws<UnexpectedSymbolException>(() => { _compiler.Next(); });
            }

            Assert.Pass();
        }

        [Test]
        public void BinaryIntegerConstantsTest()
        {
            InitStreams(out var fileIn, out var fileOut, @"LexerTests/test_04.in", @"LexerTests/test_04.out");
            _compiler.SetInput(fileIn);
            TryFail(fileIn, fileOut);
            Assert.Pass();
        }
        
//        [Test]
//        public void 
    }
}