using NUnit.Framework;

namespace Tests.LexerTests.NumberConstantsTests
{
    [TestFixture]
    public class NumbersConstantsTestFixture
    {
        private FEFUPascalCompiler.FEFUPascalCompiler _compiler;

        [OneTimeSetUp]
        public void Setup()
        {
            _compiler = new FEFUPascalCompiler.FEFUPascalCompiler();
        }

        [Test]
        public void CorrectNumbersTest()
        {
            _compiler.SetInput(@"LexerTests/NumberConstantsTests/CorrectNumbersTest.in");
            TestFunctions.ParseFile(_compiler, @"LexerTests/NumberConstantsTests/CorrectNumbersTest.res");

            TestFunctions.CheckResult(@"LexerTests/NumberConstantsTests/CorrectNumbersTest.out",
                @"LexerTests/NumberConstantsTests/CorrectNumbersTest.res");
            
            Assert.Pass();
        }

        
//        [Test]
        public void IncorrectBinNumbersTest()
        {
            _compiler.SetInput(@"LexerTests/NumberConstantsTests/IncorrectBinNumbersTest.in");
            TestFunctions.ParseFile(_compiler, @"LexerTests/NumberConstantsTests/IncorrectBinNumbersTest.res");

            TestFunctions.CheckResult(@"LexerTests/NumberConstantsTests/IncorrectBinNumbersTest.out",
                @"LexerTests/NumberConstantsTests/IncorrectBinNumbersTest.res");
            
            Assert.Pass();
        }
        
//        [Test]
        public void IncorrectOctNumbersTest()
        {
            _compiler.SetInput(@"LexerTests/NumberConstantsTests/IncorrectOctNumbersTest.in");
            TestFunctions.ParseFile(_compiler, @"LexerTests/NumberConstantsTests/IncorrectOctNumbersTest.res");

            TestFunctions.CheckResult(@"LexerTests/NumberConstantsTests/IncorrectOctNumbersTest.out",
                @"LexerTests/NumberConstantsTests/IncorrectOctNumbersTest.res");
            
            Assert.Pass();
        }
        
//        [Test]
        public void IncorrectDecNumbersTest()
        {
            _compiler.SetInput(@"LexerTests/NumberConstantsTests/IncorrectDecNumbersTest.in");
            TestFunctions.ParseFile(_compiler, @"LexerTests/NumberConstantsTests/IncorrectDecNumbersTest.res");

            TestFunctions.CheckResult(@"LexerTests/NumberConstantsTests/IncorrectDecNumbersTest.out",
                @"LexerTests/NumberConstantsTests/IncorrectDecNumbersTest.res");
            
            Assert.Pass();
        }
        
//        [Test]
        public void IncorrectHexNumbersTest()
        {
            _compiler.SetInput(@"LexerTests/NumberConstantsTests/IncorrectHexNumbersTest.in");
            TestFunctions.ParseFile(_compiler, @"LexerTests/NumberConstantsTests/IncorrectHexNumbersTest.res");

            TestFunctions.CheckResult(@"LexerTests/NumberConstantsTests/IncorrectHexNumbersTest.out",
                @"LexerTests/NumberConstantsTests/IncorrectHexNumbersTest.res");
            
            Assert.Pass();
        }

        [Test]
        public void TooLongBinNumbersTest()
        {
            //TODO:
        }

        [Test]
        public void TooLongOctNumbersTest()
        {
            //TODO:
        }

        [Test]
        public void TooLongDecNumbersTest()
        {
            //TODO:
        }

        [Test]
        public void TooLongHexNumbersTest()
        {
            //TODO:
        }
    }
}