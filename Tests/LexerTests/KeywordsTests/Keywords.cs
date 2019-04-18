using NUnit.Framework;

namespace Tests.LexerTests.KeywordsTests
{
    [TestFixture]
    public class KeywordsTestFixture
    {
        private FEFUPascalCompiler.Compiler _compiler;

        [OneTimeSetUp]
        public void Setup()
        {
            _compiler = new FEFUPascalCompiler.Compiler();
        }

        [Test]
        public void AllKeywordsTest()
        {
            const string inPathFile = @"LexerTests/KeywordsTests/AllKeywordsTest.in";
            const string outPathFile = @"LexerTests/KeywordsTests/AllKeywordsTest.out";
            const string resPathFile = @"LexerTests/KeywordsTests/AllKeywordsTest.res";

            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);
            _compiler.Input = input;

            TestFunctions.ParseAndPrint(ref _compiler, ref result);

            result.Close();
            _compiler.Input.Close();

            TestFunctions.CheckResult(outPathFile, resPathFile);

            Assert.Pass();
        }
    }
}