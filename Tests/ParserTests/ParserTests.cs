using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Tests.ParserTests.VariableDeclaration
{
    [TestFixture]
    public class VariableDeclarationTestFixture
    {
        private FEFUPascalCompiler.Compiler _compiler;

        [OneTimeSetUp]
        public void Setup()
        {
            _compiler = new FEFUPascalCompiler.Compiler();
        }

        private void Prepare(string inputPath, out StreamWriter output, string outputPath)
        {
            TestFunctions.InitStreamReader(out var input, inputPath);
            TestFunctions.InitStreamWriter(out output, outputPath);

            _compiler.Input = input;
        }

        private void Run(in StreamWriter output)
        {
            _compiler.Parse();
            _compiler.PrintAst(output);

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

            Assert.Pass();
        }

        static Dictionary<int, string> TestFile = new Dictionary<int, string>
        {
            {0, @"ParserTests/1_VariableDeclarationTest"},
            {1, @"ParserTests/2_VariableDeclarationTest"},
        };

        [Test]
        public void VariableDeclarationTest1()
        {
            Prepare(string.Concat(TestFile[0], ".in"), out var result, string.Concat(TestFile[0], ".res"));
            Run(result);
            Check(string.Concat(TestFile[0], ".out"), string.Concat(TestFile[0], ".res"));
        }
        
        [Test]
        public void VariableDeclarationTest2()
        {
            Prepare(string.Concat(TestFile[1], ".in"), out var result, string.Concat(TestFile[1], ".res"));
            Run(result);
            Check(string.Concat(TestFile[1], ".out"), string.Concat(TestFile[1], ".res"));
        }

        // CorrectProgramsTest test
    } // ParserTestFixture class
} // Tests.ParserTests namespace