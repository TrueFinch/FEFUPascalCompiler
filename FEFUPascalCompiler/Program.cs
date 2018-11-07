using System;
using System.IO;
using System.Runtime;
using FEFUPascalCompiler.Lexer;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler
{
    public class FEFUPascalCompiler
    {
        private LexerDFA _lexer;

        public FEFUPascalCompiler(StreamReader input)
        {
            _lexer = new LexerDFA();
            _lexer.SetInput(input);
        }

        public FEFUPascalCompiler()
        {
            _lexer = new LexerDFA();
        }
        
        public void SetInput(StreamReader input)
        {
            _lexer.SetInput(input);
        }

        public void Next()
        {
            _lexer.NextToken();
        }

        public Token Peek()
        {
            return _lexer.PeekToken();
        }
    }
    internal static class Application
    {
        private static void Help()
        {
            Console.WriteLine("-=|HELP|=-");
            Console.WriteLine("dotnet FEFUPascalCompiler.dll [OPTIONS] project.pas");
            Console.WriteLine("-=|options|=-");
            Console.WriteLine("-l             run only lexer");
            Console.WriteLine("-o filename    create output file");
        }
        
        public static int Main(string[] args)
        {
//            if (args.Length == 0)
//            {
//                Help();
//                return 0;
//            }

//            var inputFilePath = args[args.Length - 1];
            string inputFilePath = @"project.pas";
            try
            {
                var inputStream = File.OpenText(inputFilePath);
//                while (!inputStream.EndOfStream)
//                {
//                    Console.Write((char)inputStream.Read());
//                }
                FEFUPascalCompiler compiler = new FEFUPascalCompiler(inputStream);

                Token token = null;
                while (!(token is EOFToken))
                {
                    try
                    {
                        token = compiler.Peek();
                        compiler.Next();
                        Console.WriteLine("{0},{1}\t{2}\t\t{3}\t\t{4}", 
                            token.Line.ToString(), 
                            token.Column.ToString(),
                            token.TokenType.ToString(),
                            token.StrValue,
                            token.Text);
                    }
                    catch (LexerException exception) 
                    {
                        Console.WriteLine(exception.Message);
                        break;
                    }
                }
            }
            catch (FileNotFoundException exception)
            {
                Console.WriteLine($"{exception.FileName} not found");
            }

            return 0;
        }
    }
}