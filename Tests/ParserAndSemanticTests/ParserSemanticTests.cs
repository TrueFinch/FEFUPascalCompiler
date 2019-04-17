using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Tests.ParserAndSemanticTests
{
    [TestFixture]
    public class ParserAndSemanticTestFixture
    {
        private FEFUPascalCompiler.Compiler _compiler;

        [OneTimeSetUp]
        public void Setup()
        {
            _compiler = new FEFUPascalCompiler.Compiler();
        }

        static Dictionary<int, string> TestFile = new Dictionary<int, string>
        {
            {1, @"ParserAndSemanticTests/01_VariableDeclarationTest"},
            {2, @"ParserAndSemanticTests/02_VariableDeclarationTest"},
            {3, @"ParserAndSemanticTests/03_VariableDeclarationTest"},
            {4, @"ParserAndSemanticTests/04_VariableDeclarationTest"},
            {5, @"ParserAndSemanticTests/05_AssignmentStatementTest"},
            {6, @"ParserAndSemanticTests/06_IfStatementTest"},
            {7, @"ParserAndSemanticTests/07_IfStatementTest"},
            {8, @"ParserAndSemanticTests/08_IfStatementTest"},
            {9, @"ParserAndSemanticTests/09_ForStatementTest"},
            {10, @"ParserAndSemanticTests/10_ForStatementTest"},
            {11, @"ParserAndSemanticTests/11_ComparingOperatorTest"},
            {12, @"ParserAndSemanticTests/12_ArithmExpressionsTest"},
            {13, @"ParserAndSemanticTests/13_ArithmExpressionsTest"},
            {14, @"ParserAndSemanticTests/14_FunctionDeclarationTest"},
            {15, @"ParserAndSemanticTests/15_ProcedureDeclarationTest"},
            {16, @"ParserAndSemanticTests/16_HelloWorldTest"},
            {17, @"ParserAndSemanticTests/17_TypeDeclarationTest"},
            {18, @"ParserAndSemanticTests/18_ConstDeclarationTest"},
            {19, @"ParserAndSemanticTests/19_UnaryOperatorTest"},
            {20, @"ParserAndSemanticTests/20_PointerTest"},
            {21, @"ParserAndSemanticTests/21_DoWhileTest"},
            {22, @"ParserAndSemanticTests/22_DoWhileTest"},
            {23, @"ParserAndSemanticTests/23_DoWhileTest"},
            {24, @"ParserAndSemanticTests/24_RecordAccessTest"},
            {25, @"ParserAndSemanticTests/25_RecordArrayAccessTest"},
            {26, @"ParserAndSemanticTests/26_ManyDeclTest"},
            {27, @"ParserAndSemanticTests/27_IncorrectAssignment"},
            {28, @"ParserAndSemanticTests/28_IncorrectAssignment"},
            {29, @"ParserAndSemanticTests/29_IncorrectAssignment"},
            {30, @"ParserAndSemanticTests/30_IncorrectAssignment"},
            {31, @"ParserAndSemanticTests/31_IncorrectAssignment"},
            {32, @"ParserAndSemanticTests/32_IncorrectAssignment"},
            {33, @"ParserAndSemanticTests/33_IncorrectAssignment"},
            {34, @"ParserAndSemanticTests/34_IncorrectAssignment"},
            {35, @"ParserAndSemanticTests/35_IncorrectAssignment"},
            {36, @"ParserAndSemanticTests/36_IncorrectAssignment"},
            {37, @"ParserAndSemanticTests/37_IncorrectAssignment"},
            {38, @"ParserAndSemanticTests/38_IncorrectAssignFloatPtrToIntPtr"},
            {39, @"ParserAndSemanticTests/39_IncorrectAssignment"},
            {40, @"ParserAndSemanticTests/40_IncorrectAssignment"},
            {41, @"ParserAndSemanticTests/41_IncorrectLoopTest"},
            {42, @"ParserAndSemanticTests/42_IncorrectLoopTest"},
            {43, @"ParserAndSemanticTests/43_IncorrectLoopTest"},
            {44, @"ParserAndSemanticTests/44_IncorrectLoopTest"},
            {45, @"ParserAndSemanticTests/45_IncorrectLoopTest"},
            {46, @"ParserAndSemanticTests/46_IncorrectLoopTest"},
            {47, @"ParserAndSemanticTests/47_IncorrectLoopTest"},
            {48, @"ParserAndSemanticTests/48_IncorrectLoopTest"},
            {49, @"ParserAndSemanticTests/49_IncorrectLoopTest"},
            {50, @"ParserAndSemanticTests/50_IncorrectLoopTest"},
            {51, @"ParserAndSemanticTests/51_IncorrectLoopTest"},
            {52, @"ParserAndSemanticTests/52_IncorrectLoopTest"},
            {53, @"ParserAndSemanticTests/53_IncorrectLoopTest"},
            {54, @"ParserAndSemanticTests/54_IncorrectLoopTest"},
            {55, @"ParserAndSemanticTests/55_IncorrectLoopTest"},
            {56, @"ParserAndSemanticTests/56_IncorrectLoopTest"},
            {57, @"ParserAndSemanticTests/57_IncorrectLoopTest"},
            {58, @"ParserAndSemanticTests/58_IncorrectLoopTest"},
            {59, @"ParserAndSemanticTests/59_IncorrectLoopTest"},
            {60, @"ParserAndSemanticTests/60_IncorrectLoopTest"},
            {61, @"ParserAndSemanticTests/61_IncorrectLoopTest"},
            {62, @"ParserAndSemanticTests/62_IncorrectVarDeclTest"},
            {63, @"ParserAndSemanticTests/63_IncorrectVarDeclTest"},
            {64, @"ParserAndSemanticTests/64_IncorrectVarDeclTest"},
            {65, @"ParserAndSemanticTests/65_IncorrectVarDeclTest"},
            {66, @"ParserAndSemanticTests/66_IncorrectVarDeclTest"},
            {67, @"ParserAndSemanticTests/67_IncorrectVarDeclTest"},
            {68, @"ParserAndSemanticTests/68_IncorrectVarDeclTest"},
            {69, @"ParserAndSemanticTests/69_IncorrectVarDeclTest"},
            {70, @"ParserAndSemanticTests/70_IncorrectVarDeclTest"},
            {71, @"ParserAndSemanticTests/71_IncorrectFunctionTest"},
            {72, @"ParserAndSemanticTests/72_IncorrectFunctionTest"},
            {73, @"ParserAndSemanticTests/73_IncorrectFunctionTest"},
            {74, @"ParserAndSemanticTests/74_IncorrectFunctionTest"},
            {75, @"ParserAndSemanticTests/75_IncorrectFunctionTest"},
            {76, @"ParserAndSemanticTests/76_IncorrectFunctionTest"},
            {77, @"ParserAndSemanticTests/77_IncorrectFunctionTest"},
            {78, @"ParserAndSemanticTests/78_IncorrectFunctionTest"},
            {79, @"ParserAndSemanticTests/79_IncorrectFunctionTest"},
            {80, @"ParserAndSemanticTests/80_IncorrectFunctionTest"},
            {81, @"ParserAndSemanticTests/81_IncorrectFunctionTest"},
            {82, @"ParserAndSemanticTests/82_IncorrectFunctionTest"},
            {83, @"ParserAndSemanticTests/83_IncorrectFunctionTest"},
            {84, @"ParserAndSemanticTests/84_IncorrectFunctionTest"},
            {85, @"ParserAndSemanticTests/85_IncorrectFunctionTest"},
            {86, @"ParserAndSemanticTests/86_IncorrectFunctionTest"},
            {87, @"ParserAndSemanticTests/87_IncorrectFunctionTest"},
            {88, @"ParserAndSemanticTests/88_IncorrectFunctionTest"},
            {89, @"ParserAndSemanticTests/89_IncorrectFunctionTest"},
            {90, @"ParserAndSemanticTests/90_IncorrectFunctionTest"},
            {91, @"ParserAndSemanticTests/91_IncorrectFunctionTest"},
            {92, @"ParserAndSemanticTests/92_IncorrectFunctionTest"},
            {93, @"ParserAndSemanticTests/93_IncorrectFunctionTest"},
            {94, @"ParserAndSemanticTests/94_IncorrectFunctionTest"},
        };

        private void Prepare(string inputPath, out StreamWriter output, string outputPath)
        {
            TestFunctions.InitStreamReader(out var input, inputPath);
            TestFunctions.InitStreamWriter(out output, outputPath);
            _compiler.LastException = null;
            _compiler.Input = input;
            _compiler.Output = output;
        }

        private void Run(in StreamWriter output)
        {
            _compiler.Parse();
            _compiler.CheckSemantics();

            if (_compiler.LastException == null)
            {
                _compiler.PrintAst(output);
            }

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
            Prepare(string.Concat(TestFile[testNum], ".in"), out var result, string.Concat(TestFile[testNum], ".res"));
            Run(result);
            Check(string.Concat(TestFile[testNum], ".out"), string.Concat(TestFile[testNum], ".res"));
            Assert.Pass();
        }

        [Test]
        public void VariableDeclarationTest1() => Test(1);

        [Test]
        public void VariableDeclarationTest2() => Test(2);

        [Test]
        public void VariableDeclarationTest3() => Test(3);

        [Test]
        public void VariableDeclarationTest4() => Test(4);

        [Test]
        public void AssignmentStatementTest() => Test(5);

        [Test]
        public void IfStatementTest1() => Test(6);

        [Test]
        public void IfStatementTest2() => Test(7);

        [Test]
        public void IfStatementTest3() => Test(8);

        [Test]
        public void ForStatementTest1() => Test(9);

        [Test]
        public void ForStatementTest2() => Test(10);

        [Test]
        public void ComparingOperatorTest() => Test(11);

        [Test]
        public void ArithmExpressionsTest1() => Test(12);

        [Test]
        public void ArithmExpressionsTest2() => Test(13);

        [Test]
        public void FunctionDeclaration() => Test(14);

        [Test]
        public void ProcedureDeclaration() => Test(15);

        [Test]
        public void HelloWorldTest() => Test(16);

        [Test]
        public void TypeDeclarationTest() => Test(17);

        [Test]
        public void ConstDeclarationTest() => Test(18);

        [Test]
        public void UnaryOperatorTest() => Test(19);

        [Test]
        public void PointerTest() => Test(20);

        [Test]
        public void DoWhileTest1() => Test(21);

        [Test]
        public void DoWhileTest2() => Test(22);

        [Test]
        public void DoWhileTest3() => Test(23);

        [Test]
        public void RecordAccess() => Test(24);

        [Test]
        public void RecordArrayAccessTest() => Test(25);

        [Test]
        public void ManyDeclParts() => Test(26);

        [Test]
        public void IncorrectAssignFloatToInt() => Test(27);

        [Test]
        public void IncorrectAssignCharToInt() => Test(28);

        [Test]
        public void IncorrectAssignCharToIntArray() => Test(29);

        [Test]
        public void IncorrectAssignIntToCharArray() => Test(30);

        [Test]
        public void IncorrectAssignCharArrayToIntArray() => Test(31);

        [Test]
        public void IncorrectAssignFloatToIntArray() => Test(32);

        [Test]
        public void IncorrectAssignCahrToFloatArray() => Test(33);

        [Test]
        public void IncorrectAssignFloatArrayToIntArray() => Test(34);

        [Test]
        public void IncorrectAssignFloatToRecordIntField() => Test(35);

        [Test]
        public void IncorrectAssignCharToFloat() => Test(36);

        [Test]
        public void IncorrectAssignIntPtrToInt() => Test(37);

        [Test]
        public void IncorrectAssignFloatPtrToIntPtr() => Test(38);

        [Test]
        public void IncorrectAssignDereferencedIntSumToChar() => Test(39);

        [Test]
        public void IncorrectAssignIntToChar() => Test(40);

        [Test]
        public void IncorrectLoopLostIterator() => Test(41);

        [Test]
        public void IncorrectLoopDoBeginMismatch() => Test(42);

        [Test]
        public void IncorrectLoopMistakeInWhile() => Test(43);

        [Test]
        public void IncorrectLoopWhileDoLostDo() => Test(44);

        [Test]
        public void IncorrectLoopInsideError() => Test(45);

        [Test]
        public void IncorrectLoopWhileConditionError() => Test(46);

        [Test]
        public void IncorrectLoopCloseBracketMisplace() => Test(47);

        [Test]
        public void IncorrectLoopBracketsInsteadOfBeginEnd() => Test(48);

        [Test]
        public void IncorrectLoopLostDoInDoWhile() => Test(49);

        [Test]
        public void IncorrectLoopDoWhileConditionError() => Test(50);

        [Test]
        public void IncorrectLoopDoWhileSemicolonBeforeWhile() => Test(51);

        [Test]
        public void IncorrectLoopForWrongDownTo() => Test(52);

        [Test]
        public void IncorrectLoopDoWhileWrongBeginEnd() => Test(53);

        [Test]
        public void IncorrectLoopDoWhileLostWhile() => Test(54);

        [Test]
        public void IncorrectLoopForBeginEndMisplace() => Test(55);

        [Test]
        public void IncorrectLoopForMistakeInFor() => Test(56);

        [Test]
        public void IncorrectLoopForLostBegin() => Test(57);

        [Test]
        public void IncorrectLoopForErrorBetweenBeginEnd() => Test(58);

        [Test]
        public void IncorrectLoopForLostDo() => Test(59);

        [Test]
        public void IncorrectLoopForWrongType() => Test(60);

        [Test]
        public void IncorrectLoopForWrongBoundInit() => Test(61);

        [Test]
        public void IncorrectVarDeclTest1() => Test(62);

        [Test]
        public void IncorrectVarDeclTest2() => Test(63);

        [Test]
        public void IncorrectVarDeclTest3() => Test(64);

        [Test]
        public void IncorrectVarDeclTest4() => Test(65);

        [Test]
        public void IncorrectVarDeclTest5() => Test(66);

        [Test]
        public void IncorrectVarDeclTest6() => Test(67);

        [Test]
        public void IncorrectVarDeclTest7() => Test(68);

        [Test]
        public void IncorrectVarDeclTest8() => Test(69);

        [Test]
        public void IncorrectVarDeclTest9() => Test(70);

        [Test]
        public void IncorrectFunctionTest1() => Test(71);

        [Test]
        public void IncorrectFunctionTest2() => Test(72);

        [Test]
        public void IncorrectFunctionTest3() => Test(73);

        [Test]
        public void IncorrectFunctionTest4() => Test(74);

        [Test]
        public void IncorrectFunctionTest5() => Test(75);

        [Test]
        public void IncorrectFunctionTest6() => Test(76);

        [Test]
        public void IncorrectFunctionTest7() => Test(77);

        [Test]
        public void IncorrectFunctionTest8() => Test(78);

        [Test]
        public void IncorrectFunctionTest9() => Test(79);

        [Test]
        public void IncorrectFunctionTest10() => Test(80);

        [Test]
        public void IncorrectFunctionTest11() => Test(81);

        [Test]
        public void IncorrectFunctionTest12() => Test(82);

        [Test]
        public void IncorrectFunctionTest13() => Test(83);

        [Test]
        public void IncorrectFunctionTest14() => Test(84);

        [Test]
        public void IncorrectFunctionTest15() => Test(85);

        [Test]
        public void IncorrectFunctionTest16() => Test(86);

        [Test]
        public void IncorrectFunctionTest17() => Test(87);

        [Test]
        public void IncorrectFunctionTest18() => Test(88);

        [Test]
        public void IncorrectFunctionTest19() => Test(89);

        [Test]
        public void IncorrectFunctionTest20() => Test(90);

        [Test]
        public void IncorrectFunctionTest21() => Test(91);

        [Test]
        public void IncorrectFunctionTest22() => Test(92);

        [Test]
        public void IncorrectFunctionTest23() => Test(93);

        [Test]
        public void IncorrectFunctionTest24() => Test(94);
    } // ParserTestFixture class
} // Tests.ParserTests namespace