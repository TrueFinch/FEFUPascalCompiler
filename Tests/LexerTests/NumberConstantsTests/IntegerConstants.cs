using NUnit.Framework;

namespace Tests.LexerTests.NumberConstantsTests
{
    [TestFixture]
    public class NumbersConstantsTestFixture
    {
        private FEFUPascalCompiler.Compiler _compiler;

        [OneTimeSetUp]
        public void Setup()
        {
            _compiler = new FEFUPascalCompiler.Compiler();
        }

        [Test]
        public void CorrectNumbersTest()
        {
            const string inPathFile = @"LexerTests/NumberConstantsTests/CorrectNumbersTest.in";
            const string outPathFile = @"LexerTests/NumberConstantsTests/CorrectNumbersTest.out";
            const string resPathFile = @"LexerTests/NumberConstantsTests/CorrectNumbersTest.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);
            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);
            
            _compiler.Input.Close();
            
            TestFunctions.CheckResult(outPathFile, resPathFile);
            
            Assert.Pass();
        }

        
        [Test]
        public void IncorrectBinNumbersTest()
        {
            const string inPathFile = @"LexerTests/NumberConstantsTests/IncorrectBinNumbersTest.in";
            const string outPathFile = @"LexerTests/NumberConstantsTests/IncorrectBinNumbersTest.out";
            const string resPathFile = @"LexerTests/NumberConstantsTests/IncorrectBinNumbersTest.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);

            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);

            _compiler.Input.Close();
            
            TestFunctions.CheckResult(outPathFile, resPathFile);
            
            Assert.Pass();
        }
        
        [Test]
        public void IncorrectOctNumbersTest()
        {
            const string inPathFile = @"LexerTests/NumberConstantsTests/IncorrectOctNumbersTest.in";
            const string outPathFile = @"LexerTests/NumberConstantsTests/IncorrectOctNumbersTest.out";
            const string resPathFile = @"LexerTests/NumberConstantsTests/IncorrectOctNumbersTest.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);

            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);
            
            result.Close();
            _compiler.Input.Close();

            TestFunctions.CheckError(outPathFile, _compiler.LastException.Message);
            
            Assert.Pass();
        }
        
//        [Test]
//        public void IncorrectDecNumbersTest()
//        {
//            const string inPathFile = @"LexerTests/NumberConstantsTests/IncorrectDecNumbersTest.in";
//            const string outPathFile = @"LexerTests/NumberConstantsTests/IncorrectDecNumbersTest.out";
//            const string resPathFile = @"LexerTests/NumberConstantsTests/IncorrectDecNumbersTest.res";
//            
//            TestFunctions.InitStreamReader(out var input, inPathFile);
//            TestFunctions.InitStreamWriter(out var result, resPathFile);
//
//            _compiler.Input = input;
//            
//            TestFunctions.ParseAndPrint(ref _compiler, ref result);
//
//            result.Close();
//            _compiler.Input.Close();
//
//            TestFunctions.CheckError(outPathFile, _compiler.LastException.Message);
//            
//            Assert.Pass();
//        }
        
        [Test]
        public void IncorrectHexNumbersTest()
        {            
            const string inPathFile = @"LexerTests/NumberConstantsTests/IncorrectHexNumbersTest.in";
            const string outPathFile = @"LexerTests/NumberConstantsTests/IncorrectHexNumbersTest.out";
            const string resPathFile = @"LexerTests/NumberConstantsTests/IncorrectHexNumbersTest.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);

            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);

            result.Close();
            _compiler.Input.Close();

            TestFunctions.CheckError(outPathFile, _compiler.LastException.Message);
            
            Assert.Pass();
        }

        [Test]
        public void TooLongBinNumbersTest()
        {
            const string inPathFile = @"LexerTests/NumberConstantsTests/TooLongBinNumbersTest.in";
            const string outPathFile = @"LexerTests/NumberConstantsTests/TooLongBinNumbersTest.out";
            const string resPathFile = @"LexerTests/NumberConstantsTests/TooLongBinNumbersTest.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);

            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);
            
            result.Close();
            _compiler.Input.Close();

            TestFunctions.CheckError(outPathFile, _compiler.LastException.Message);
            
            Assert.Pass();
        }

        [Test]
        public void TooLongOctNumbersTest()
        {
            const string inPathFile = @"LexerTests/NumberConstantsTests/TooLongOctNumbersTest.in";
            const string outPathFile = @"LexerTests/NumberConstantsTests/TooLongOctNumbersTest.out";
            const string resPathFile = @"LexerTests/NumberConstantsTests/TooLongOctNumbersTest.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);

            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);
            
            result.Close();
            _compiler.Input.Close();

            TestFunctions.CheckError(outPathFile, _compiler.LastException.Message);
            
            Assert.Pass();
        }

        [Test]
        public void TooLongDecNumbersTest()
        {
            const string inPathFile = @"LexerTests/NumberConstantsTests/TooLongDecNumbersTest.in";
            const string outPathFile = @"LexerTests/NumberConstantsTests/TooLongDecNumbersTest.out";
            const string resPathFile = @"LexerTests/NumberConstantsTests/TooLongDecNumbersTest.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);

            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);
            
            result.Close();
            _compiler.Input.Close();

            TestFunctions.CheckError(outPathFile, _compiler.LastException.Message);
            
            Assert.Pass();
        }

        [Test]
        public void TooLongHexNumbersTest()
        {
            const string inPathFile = @"LexerTests/NumberConstantsTests/TooLongHexNumbersTest.in";
            const string outPathFile = @"LexerTests/NumberConstantsTests/TooLongHexNumbersTest.out";
            const string resPathFile = @"LexerTests/NumberConstantsTests/TooLongHexNumbersTest.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);

            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);
            
            result.Close();
            _compiler.Input.Close();

            TestFunctions.CheckError(outPathFile, _compiler.LastException.Message);
            
            Assert.Pass();
        }
    }
}