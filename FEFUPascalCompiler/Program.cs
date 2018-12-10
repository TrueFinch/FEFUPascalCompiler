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
            
            FEFUPascalCompiler compiler = new FEFUPascalCompiler();
            compiler.SetInput(inputFilePath);
            
            while (compiler.Next())
            {
                Console.WriteLine(compiler.Peek().ToString());
            }

            return 0;
        }
    }
}