using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Internal;

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

        static Dictionary<int, string> TestFile = new Dictionary<int, string>
        {
            {0, @"ParserTests/01_VariableDeclarationTest"},
            {1, @"ParserTests/02_VariableDeclarationTest"},
            {2, @"ParserTests/03_VariableDeclarationTest"},
            {3, @"ParserTests/04_VariableDeclarationTest"},
            {4, @"ParserTests/05_AssignmentStatementTest"},
            {5, @"ParserTests/06_IfStatementTest"},
            {6, @"ParserTests/07_IfStatementTest"},
            {7, @"ParserTests/08_IfStatementTest"},
            {8, @"ParserTests/09_ForStatementTest"},
            {9, @"ParserTests/10_ForStatementTest"},
            {10, @"ParserTests/11_ComparingOperatorTest"},
            {11, @"ParserTests/12_ArithmExpressionsTest"},
            {12, @"ParserTests/13_ArithmExpressionsTest"},
            {13, @"ParserTests/14_FunctionDeclarationTest"},
            {14, @"ParserTests/15_ProcedureDeclarationTest"},
            {15, @"ParserTests/16_HelloWorldTest"},
            {16, @"ParserTests/17_TypeDeclarationTest"},
            {17, @"ParserTests/18_TypeDeclarationTest"},
            {18, @"ParserTests/19_UnaryOperatorTest"},
            {19, @"ParserTests/20_NilTest"},
        };
        
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

        }

        private void Test(int testNum)
        {
            testNum -= 1;
            Prepare(string.Concat(TestFile[testNum], ".in"), out var result, string.Concat(TestFile[testNum], ".res"));
            Run(result);
            Check(string.Concat(TestFile[testNum], ".out"), string.Concat(TestFile[testNum], ".res"));
            Assert.Pass();
        }

        [Test]
        public void VariableDeclarationTest1()
        {
            Test(1);
        }
        
        [Test]
        public void VariableDeclarationTest2()
        {
            Test(2);
        }

        [Test]
        public void VariableDeclarationTest3()
        {
            Test(3);
        }
        
        [Test]
        public void VariableDeclarationTest4()
        {
            Test(4);
        }
        
        [Test]
        public void AssignmentStatementTest()
        {
            Test(5);
        }
        
        [Test]
        public void IfStatementTest1()
        {
            Test(6);
        }

        [Test]
        public void IfStatementTest2()
        {
            Test(7);
        }
        
        [Test]
        public void IfStatementTest3()
        {
            Test(8);
        }
        
        [Test]
        public void ForStatementTest1()
        {
            Test(9);
        }
        
        [Test]
        public void ForStatementTest2()
        {
            Test(10);
        }
        
        [Test]
        public void ComparingOperatorTest()
        {
            Test(11);
        }
                
        [Test]
        public void ArithmExpressionsTest1()
        {
            Test(12);
        }
        
        [Test]
        public void ArithmExpressionsTest2()
        {
            Test(13);
        }
        // CorrectProgramsTest test
    } // ParserTestFixture class
} // Tests.ParserTests namespace