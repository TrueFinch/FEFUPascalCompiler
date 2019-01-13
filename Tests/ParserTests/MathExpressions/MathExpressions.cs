using NUnit.Framework;

namespace Tests.ParserTests.MathExpressions
{
    [TestFixture]
    public class MathExpressions
    {
        private FEFUPascalCompiler.Compiler _compiler;

        [OneTimeSetUp]
        public void Setup()
        {
            _compiler = new FEFUPascalCompiler.Compiler();
        }

        [Test]
        public void AllBinOperatorsTest()
        {
            const string inPathFile = @"LexerTests/SeparatorsTests/AllBinOperatorsTest.in";
            const string outPathFile = @"LexerTests/SeparatorsTests/AllBinOperatorsTest.out";
            const string resPathFile = @"LexerTests/SeparatorsTests/AllBinOperatorsTest.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);
            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);
            
            result.Close();
            _compiler.Input.Close();
            
            TestFunctions.CheckResult(outPathFile, resPathFile);
            
            Assert.Pass();
        } // AllBinOperatorsTest test
        
        [Test]
        public void AllAssignOperatorsTest()
        {
            const string inPathFile = @"LexerTests/SeparatorsTests/AllAssignOperatorsTest.in";
            const string outPathFile = @"LexerTests/SeparatorsTests/AllAssignOperatorsTest.out";
            const string resPathFile = @"LexerTests/SeparatorsTests/AllAssignOperatorsTest.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);
            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);
            
            result.Close();
            _compiler.Input.Close();
            
            TestFunctions.CheckResult(outPathFile, resPathFile);
            
            Assert.Pass();
        } // AllAssignOperatorsTest test
    } // BinOperatorsTestFixture class
} // Tests.LexerTests.BinOperatorsTests namespace