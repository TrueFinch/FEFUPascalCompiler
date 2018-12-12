using System;
using System.Diagnostics.Tracing;
using System.IO;
using NUnit.Framework;
using FEFUPascalCompiler;
using FEFUPascalCompiler.Lexer;
using FEFUPascalCompiler.Tokens;

namespace Tests
{
    public static class TestFunctions
    {
        public static void InitStreamReader(out StreamReader sr, string path1)
        {
            try
            {
                sr = File.OpenText(path1);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
                sr = null;
            }
        }

        public static void InitStreamWriter(out StreamWriter sr, string path1)
        {
            try
            {
                sr = File.CreateText(path1);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
                sr = null;
            }
        }

        public static void ParseAndPrint(ref FEFUPascalCompiler.Compiler compiler, ref StreamWriter output)
        {
//            InitStreamWriter(out var result, resultFilePath);
            while (compiler.Next())
            {                                                                
                output.WriteLine(compiler.Peek().ToString());
            } 
//            result.Close();
        }

        public static void CheckResult(in string filePathToExpected, in string filePathToActual)
        {
            InitStreamReader(out var expected, filePathToExpected);
            InitStreamReader(out var actual, filePathToActual);
            while (!expected.EndOfStream)
            {
                Assert.AreEqual(expected.ReadLine(), actual.ReadLine());
            }
        }

        public static void CheckError(in string filePathToExpected, in string exceptionMessage)
        {
            InitStreamReader(out var expected, filePathToExpected);
            Assert.AreEqual(expected.ReadLine(), exceptionMessage);
        }
    } // class TestFunctions
} // namespace Tests

//    
//    [TestFixture]
//    public class LexerTestss
//    {
//        private void WriteTokenToConsole(Token t)
//        {
//            Console.WriteLine("{0},{1}\t{2}\t\t{3}\t\t{4}",
//                t.Line.ToString(),
//                t.Column.ToString(),
//                t.TokenType.ToString(),
//                t.StrValue,
//                t.Lexeme);
//        }
//
//        private struct Row
//        {
//            public int Line, Column;
//            public TokenType Type;
//            public string StrValue, Text;
//
//            public Row(string s)
//            {
//                if (s == null)
//                {
//                    this = default(Row);
//                    return;
//                }
//
//                string[] words = s.Split(' ');
//                Line = int.Parse(words[0].Split(',')[0]);
//                Column = int.Parse(words[0].Split(',')[1]);
//                Type = Enum.Parse<TokenType>(words[1]);
//                StrValue = words[2];
//                Text = words[3];
//            }
//
//            public bool Empty()
//            {
//                return StrValue == null;
//            }
//        }
//
//        private void InitStreams(out StreamReader sr1, out StreamReader sr2, string path1, string path2)
//        {
//            InitStream(out sr1, path1);
//            InitStream(out sr2, path2);
//        }
//
//        private void AreEqual(Token t, Row expectedToken)
//        {
//            Assert.AreEqual(t.Line, expectedToken.Line);
//            Assert.AreEqual(t.Column, expectedToken.Column);
//            Assert.AreEqual(t.TokenType, expectedToken.Type);
//            Assert.AreEqual(t.StrValue, expectedToken.StrValue);
//            Assert.AreEqual(t.Lexeme, expectedToken.Text);
//        }
//
//        private void TryFail(StreamReader fileIn, StreamReader fileOut)
//        {
//            Token token;
//            do
//            {
//                token = _compiler.Peek();
//                var l = fileOut.ReadLine();
//                Row expectedToken = new Row(l);
//                if (expectedToken.Empty())
//                {
//                    Assert.Fail(".out file does not contain information about token");
//                }
//
//                AreEqual(token, expectedToken);
//            } while (_compiler.Next());
//        }
//
//        private FEFUPascalCompiler.FEFUPascalCompiler _compiler;
//
//        
//
//        [Test]
//        public void SimpleTest()
//        {
//            InitStreams(out var fileIn, out var fileOut,
//                @"LexerTests/SimpleProgramTests/test_00.in", @"LexerTests/SimpleProgramTests/test_00.out");
//            _compiler.SetInput(ref fileIn);
//            TryFail(fileIn, fileOut);
//
//            Assert.Pass();
//        }
//
//        [Test]
//        public void DecimalIntegerConstantsTest()
//        {
//            InitStreams(out var fileIn, out var fileOut,
//                @"LexerTests/DecimalIntegerTests/test_01.in", @"LexerTests/DecimalIntegerTests/test_01.out");
//            _compiler.SetInput(ref fileIn);
//            TryFail(fileIn, fileOut);
//            Assert.Pass();
//        }
//
//        [Test]
//        public void TooBigDecimalIntegerConstantsTest()
//        {
//            InitStream(out var fileIn, @"LexerTests/DecimalIntegerTests/test_02.in");
//            Assert.Throws<StrToIntConvertException>(() => { _compiler.SetInput(ref fileIn); });
//            while (!fileIn.EndOfStream)
//            {
//                Assert.Throws<StrToIntConvertException>(() => { _compiler.Next(); });
//            }
//
//            Assert.Pass();
//        }
//
//        [Test]
//        public void InvalidDecimalIntegerConstantsTest()
//        {
//            InitStream(out var fileIn, @"LexerTests/DecimalIntegerTests/test_03.in");
//            Assert.Throws<UnexpectedSymbolException>(() => { _compiler.SetInput(ref fileIn); });
//            while (fileIn.EndOfStream)
//            {
//                Assert.Throws<UnexpectedSymbolException>(() => { _compiler.Next(); });
//            }
//
//            Assert.Pass();
//        }
//
//        [Test]
//        public void ManySpacesAndNewLinesDecimalIntegerConstantsTest()
//        {
//            InitStreams(out var fileIn, out var fileOut,
//                @"LexerTests/DecimalIntegerTests/test_04.in", @"LexerTests/DecimalIntegerTests/test_04.out");
//            _compiler.SetInput(ref fileIn);
//            TryFail(fileIn, fileOut);
//            Assert.Pass();
//        }
//
//        [Test]
//        public void BinaryIntegerConstantsTest()
//        {
//            InitStreams(out var fileIn, out var fileOut,
//                @"LexerTests/BinaryIntegerTests/test_01.in", @"LexerTests/BinaryIntegerTests/test_01.out");
//            _compiler.SetInput(ref fileIn);
//            TryFail(fileIn, fileOut);
//            Assert.Pass();
//        }
//
//        [Test]
//        public void TooBigBinaryIntegerConstantsTest()
//        {
//            InitStream(out var fileIn, @"LexerTests/BinaryIntegerTests/test_02.in");
//            Assert.Throws<StrToIntConvertException>(() => { _compiler.SetInput(ref fileIn); });
//            while (!fileIn.EndOfStream)
//            {
//                Assert.Throws<StrToIntConvertException>(() => { _compiler.Next(); });
//            }
//
//            Assert.Pass();
//        }
//
//        [Test]
//        public void InvalidBinaryIntegerConstantsTest()
//        {
//            InitStream(out var fileIn, @"LexerTests/BinaryIntegerTests/test_03.in");
//            Assert.Throws<UnexpectedSymbolException>(() => { _compiler.SetInput(ref fileIn); });
//            while (!fileIn.EndOfStream)
//            {
//                Assert.Throws<UnexpectedSymbolException>(() => { _compiler.Next(); });
//            }
//
//            Assert.Pass();
//        }
//
//        [Test]
//        public void ManySpacesAndNewLinesBinaryIntegerConstantsTest()
//        {
//            InitStreams(out var fileIn, out var fileOut, 
//                @"LexerTests/BinaryIntegerTests/test_04.in", @"LexerTests/BinaryIntegerTests/test_04.out");
//            _compiler.SetInput(ref fileIn);
//            TryFail(fileIn, fileOut);
//            Assert.Pass();
//        }
//    }
//}