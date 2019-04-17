using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Tests.LexerTests.OtherTests
{
    [TestFixture]
    public class OtherLexerTests
    {
        private FEFUPascalCompiler.Compiler _compiler;

        [OneTimeSetUp]
        public void Setup()
        {
            _compiler = new FEFUPascalCompiler.Compiler();
        }

        static Dictionary<int, string> TestFile = new Dictionary<int, string>
        {
            {0, @"LexerTests/OtherTests/01_WrongLexeme"},
            {1, @"LexerTests/OtherTests/02_WrongLexeme"},
            {2, @"LexerTests/OtherTests/03_WrongLexeme"},
        };

        private void Prepare(string inputPath, out StreamWriter output, string outputPath)
        {
            TestFunctions.InitStreamReader(out var input, inputPath);
            TestFunctions.InitStreamWriter(out output, outputPath);

            _compiler.Input = input;
        }

        private void Run(ref StreamWriter output)
        {
//            _compiler.Parse();
//            _compiler.PrintAst(output);
            _compiler.Output = output;
            TestFunctions.ParseAndPrint(ref _compiler, ref output);
            output.Close();
            _compiler.Input.Close();
        }

        private void Check(string filePathToExpected, string filePathToActual)
        {
            TestFunctions.InitStreamReader(out var expected, filePathToExpected);
            TestFunctions.InitStreamReader(out var actual, filePathToActual);
            while (!expected.EndOfStream)
            {
                Assert.AreEqual(expected.ReadLine(), actual.ReadLine());
            }
        }

        private void Test(int testNum)
        {
            testNum -= 1;
            Prepare(string.Concat(TestFile[testNum], ".in"), out var result, string.Concat(TestFile[testNum], ".res"));
            Run(ref result);
            Check(string.Concat(TestFile[testNum], ".out"), string.Concat(TestFile[testNum], ".res"));
            Assert.Pass();
        }

        [Test]
        public void WrongLexemeTest1()
        {
            Test(1);
        }

        [Test]
        public void WrongLexemeTest2()
        {
            Test(2);
        }

        [Test]
        public void WrongLexemeTest3()
        {
            Test(3);
        }

        // CorrectProgramsTest test
    } // ParserTestFixture class
} // Tests.ParserTests namespace