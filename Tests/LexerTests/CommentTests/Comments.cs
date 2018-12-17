using NUnit.Framework;

namespace Tests.LexerTests.CommentTests
{
    [TestFixture]
    public class CommentTestFixture
    {
        private FEFUPascalCompiler.Compiler _compiler;

        [OneTimeSetUp]
        public void Setup()
        {
            _compiler = new FEFUPascalCompiler.Compiler();
        }

        [Test]
        public void CorrectCommentsTest()
        {
            const string inPathFile = @"LexerTests/CommentTests/CorrectCommentsTest.in";
            const string outPathFile = @"LexerTests/CommentTests/CorrectCommentsTest.out";
            const string resPathFile = @"LexerTests/CommentTests/CorrectCommentsTest.res";
            
            TestFunctions.InitStreamReader(out var input, inPathFile);
            TestFunctions.InitStreamWriter(out var result, resPathFile);
            _compiler.Input = input;
            
            TestFunctions.ParseAndPrint(ref _compiler, ref result);
            
            result.Close();
            _compiler.Input.Close();
            
            TestFunctions.CheckResult(outPathFile, resPathFile);
            
            Assert.Pass();
        } // CorrectCommentsTest test
    } // CommentTestFixture class
} // Tests.LexerTests.CommentTests namespace