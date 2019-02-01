using NUnit.Framework;

namespace Tests.LexerTests.IdentifiersTests
{
    [TestFixture]
    public class ConstStringsTestFixture
    {
        private FEFUPascalCompiler.Compiler _compiler;

        [OneTimeSetUp]
        public void Setup()
        {
            _compiler = new FEFUPascalCompiler.Compiler();
        }

        [Test]
        public void CorrectConstStringsTest()
        {
            const string inPathFile = @"LexerTests/ConstStringsTests/CorrectConstStringsTest.in";
            const string outPathFile = @"LexerTests/ConstStringsTests/CorrectConstStringsTest.out";
            const string resPathFile = @"LexerTests/ConstStringsTests/CorrectConstStringsTest.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);
            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);
            
            result.Close();
            _compiler.Input.Close();
            
            TestFunctions.CheckResult(outPathFile, resPathFile);
            
            Assert.Pass();
        } // CorrectConstStringsTest test
        
        [Test]
        public void UnclosedConstStringsTest()
        {
            const string inPathFile = @"LexerTests/ConstStringsTests/UnclosedConstStringsTest.in";
            const string outPathFile = @"LexerTests/ConstStringsTests/UnclosedConstStringsTest.out";
            const string resPathFile = @"LexerTests/ConstStringsTests/UnclosedConstStringsTest.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);
            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);
            
            result.Close();
            _compiler.Input.Close();
            
            TestFunctions.CheckError(outPathFile, _compiler.LastException.Message);
            
            Assert.Pass();
        } // UnclosedConstStringsTest test
        
        [Test]
        public void UnclosedConstStringsTest2()
        {
            const string inPathFile = @"LexerTests/ConstStringsTests/UnclosedConstStringsTest2.in";
            const string outPathFile = @"LexerTests/ConstStringsTests/UnclosedConstStringsTest2.out";
            const string resPathFile = @"LexerTests/ConstStringsTests/UnclosedConstStringsTest2.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);
            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);
            
            result.Close();
            _compiler.Input.Close();
            
            TestFunctions.CheckError(outPathFile, _compiler.LastException.Message);
            
            Assert.Pass();
        } // UnclosedConstStringsTest test
    } // ConstStringsTestFixture class
} // Tests.LexerTests.IdentifiersTests namespace