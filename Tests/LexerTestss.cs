using System;
using System.IO;
using NUnit.Framework;
using FEFUPascalCompiler;

namespace Tests
{
    public static class TestFunctions
    {
        public static void InitStreamReader(out StreamReader sr, string path1)
        {
            try
            {
                sr = File.OpenText(path1);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
                sr = null;
            }
        }

        public static void InitStreamWriter(out StreamWriter sr, string path1)
        {
            try
            {
                sr = File.CreateText(path1);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
                sr = null;
            }
        }

        public static void ParseAndPrint(ref Compiler compiler, ref StreamWriter output)
        {
            while (compiler.Next())
            {                                                                
                output.WriteLine(compiler.Peek().ToString());
            } 
        }

        public static void CheckResult(in string filePathToExpected, in string filePathToActual)
        {
            InitStreamReader(out var expected, filePathToExpected);
            InitStreamReader(out var actual, filePathToActual);
            while (!expected.EndOfStream)
            {
                Assert.AreEqual(expected.ReadLine(), actual.ReadLine());
            }
        }

        public static void CheckError(in string filePathToExpected, in string exceptionMessage)
        {
            InitStreamReader(out var expected, filePathToExpected);
            Assert.AreEqual(expected.ReadLine(), exceptionMessage);
        }
    } // class TestFunctions
} // namespace Tests
