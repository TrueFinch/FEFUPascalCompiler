using System.IO;
using NUnit.Framework;

namespace Tests.LexerTests.IdentifiersTests
{
    [TestFixture]
    public class IdentifiersTestFixture
    {
        private FEFUPascalCompiler.Compiler _compiler;

        [OneTimeSetUp]
        public void Setup()
        {
            _compiler = new FEFUPascalCompiler.Compiler();
        }

        [Test]
        public void IdentifiersLikeKeywordsTest()
        {
            const string inPathFile = @"LexerTests/IdentifiersTests/IdentifiersLikeKeywordsTest.in";
            const string outPathFile = @"LexerTests/IdentifiersTests/IdentifiersLikeKeywordsTest.out";
            const string resPathFile = @"LexerTests/IdentifiersTests/IdentifiersLikeKeywordsTest.res";

            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);
            _compiler.Input = input;

            TestFunctions.ParseAndPrint(ref _compiler, ref result);

            result.Close();
            _compiler.Input.Close();

            TestFunctions.CheckResult(outPathFile, resPathFile);

            Assert.Pass();
        }

        [Test]
        public void DifferentIdentifiersTest()
        {
            const string inPathFile = @"LexerTests/IdentifiersTests/DifferentIdentifiersTest.in";
            const string outPathFile = @"LexerTests/IdentifiersTests/DifferentIdentifiersTest.out";
            const string resPathFile = @"LexerTests/IdentifiersTests/DifferentIdentifiersTest.res";

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