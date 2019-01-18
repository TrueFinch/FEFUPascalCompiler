using System;
using System.IO;

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
            
            while (compiler.Next())
            {
                Console.WriteLine(compiler.Peek().ToString());
            }

            if (compiler.LastException != null)
            {
                Console.WriteLine(compiler.LastException.Message);
            }
            
            compiler.Input.Close();
            
            
            compiler.Input = new StreamReader(inputFilePath);
            compiler.Parse();
            if (compiler.LastException == null)
                compiler.PrintAst(output);
            
            output.Close();
            compiler.Input.Close();
            return 0;
        }
    }
}