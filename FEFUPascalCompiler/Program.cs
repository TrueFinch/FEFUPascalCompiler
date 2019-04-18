using System;
using System.Diagnostics;
using System.IO;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Parser.Sematics;

namespace FEFUPascalCompiler
{
    internal static class Application
    {
        private static void Help()
        {
            Console.WriteLine("-=|HELP|=-");
            Console.WriteLine("dotnet FEFUPascalCompiler.dll [OPTIONS] project.pas");
            Console.WriteLine("-=|options|=-");
            Console.WriteLine("-l \t\t run only lexer");
            Console.WriteLine("-c \t\t print comment (only for lexer)");
            Console.WriteLine("-o filename \t\t create output file");
        }

        public static int Main(string[] args)
        {
            string inputFilePath = @"project.txt";
            string outputFilePath = @"output.txt";
            var output = new StreamWriter(outputFilePath);
            Compiler compiler = new Compiler();
            compiler.Input = new StreamReader(inputFilePath);

            compiler.TokenizeComments = false;

            while (compiler.Next())
            {
                Console.WriteLine(compiler.Peek().ToString());
            }

            if (compiler.LastException != null)
            {
                return 0;
            }

            compiler.Input.Close();


            compiler.Input = new StreamReader(inputFilePath);
            compiler.Parse();
            if (compiler.LastException == null)
                compiler.CheckSemantics();
            if (compiler.LastException == null)
                compiler.PrintAst(output);

            if (compiler.LastException == null)
                compiler.GenerateAssembly();

            if (compiler.LastException == null)
                compiler.Assembly.SaveAssembly(output);

            if (compiler.LastException == null)
                compiler.Compile("project.asm");

            output.Close();
            compiler.Input.Close();

            if (compiler.LastException == null)
            {
                var programProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"project.exe",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                programProcess.Start();

                var res = File.CreateText(outputFilePath);
                while (!programProcess.StandardOutput.EndOfStream)
                {
                    res.WriteLine(programProcess.StandardOutput.ReadLine());
                }

                res.Close();
                programProcess.WaitForExit();
            }

            return 0;
        }
    }
}