using System;
using System.Diagnostics.Tracing;
using System.IO;
using NUnit.Framework;
using FEFUPascalCompiler;
using FEFUPascalCompiler.Lexer;
using FEFUPascalCompiler.Tokens;

namespace Tests
{
    public class Tests
    {
        private FEFUPascalCompiler.FEFUPascalCompiler _compiler;

        [SetUp]
        public void Setup()
        {
            _compiler = new FEFUPascalCompiler.FEFUPascalCompiler();
        }

        [Test]
        public void SimpleTest()
        {
            const string testFileInPath = @"LexerTests/test_01.in";
            const string testFileOutPath = @"LexerTests/test_01.out";
            var fileInStream = File.OpenText(testFileInPath);
            var fileOutStream = File.OpenText(testFileOutPath);
            if ((fileInStream == null) || (fileOutStream == null))
            {
                Assert.Fail();
            }

            _compiler.SetInput(fileInStream);
            Token token = null;
            while (!(token is EOFToken))
            {
                try
                {
                    token = _compiler.Peek();
                    _compiler.Next();
                    string row = fileOutStream.ReadLine();
                    if (row == null)
                    {
                        Assert.Fail();
                    }
                    Console.WriteLine("{0},{1}\t{2}\t\t{3}\t\t{4}", 
                        token.Line.ToString(), 
                        token.Column.ToString(),
                        token.TokenType.ToString(),
                        token.StrValue,
                        token.Text);
                    string[] words = row.Split(' ');
                    int line = int.Parse(words[0].Split(',')[0]);
                    int column = int.Parse(words[0].Split(',')[1]);
                    Assert.AreEqual(token.Line, line);
                    Assert.AreEqual(token.Column, column);
                    Assert.AreEqual(token.TokenType, Enum.Parse<TokenType>(words[1]));
                    Assert.AreEqual(token.StrValue, words[2]);
                    Assert.AreEqual(token.Text, words[3]);
                }
                catch (LexerException exception)
                {
                    Console.WriteLine(exception.Message);
                    break;
                }
            }
            Assert.Pass();
        }
    }
}