using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Microsoft.VisualBasic.CompilerServices;

namespace FEFUPascalCompiler.Lexer
{
    internal class LexerDFA
    {
//        internal enum CharacterType
//        {
//            InvalidCharacter,
//            FromAtoFLetter,
//            FromGtoZLetter,
//            BinDigit,
//            OctDigit,
//            DecDigit,
//            ArithmeticOperator
//        }
//
//        private static class Characters
//        {
//            private static string fromAToFLetters = "ABCDEF";
//            private static string fromGToZLetters = "GHIJKLMNOPQRSTUVWXYZ";
//            private static string binDigits = "01";
//            private static string octDigits = "2345678";
//            private static string decDigits = "9";
//            private static string arithmOps = "+-*/";
//
//            private static readonly HashSet<char> FromAToFLetters =
//                new HashSet<char>((fromAToFLetters + fromAToFLetters.ToLower() + '_').ToCharArray());
//
//            private static readonly HashSet<char> FromGToZLetters =
//                new HashSet<char>((fromGToZLetters + fromGToZLetters.ToLower() + '_').ToCharArray());
//
//            private static readonly HashSet<char> BinaryDigits =
//                new HashSet<char>(binDigits.ToCharArray());
//
//            private static readonly HashSet<char> OctalDigits =
//                new HashSet<char>(octDigits.ToCharArray());
//
//            private static readonly HashSet<char> DecimalDigits =
//                new HashSet<char>(decDigits.ToCharArray());
//
//            private static readonly HashSet<char> ArithmeticOperators =
//                new HashSet<char>(arithmOps.ToCharArray());
//
//            internal static CharacterType GetCharacterType(char ch)
//            {
//                if (FromAToFLetters.Contains(ch))
//                    return CharacterType.FromAtoFLetter;
//                if (FromGToZLetters.Contains(ch))
//                    return CharacterType.FromGtoZLetter;
//                if (BinaryDigits.Contains(ch))
//                    return CharacterType.BinDigit;
//                if (OctalDigits.Contains(ch))
//                    return CharacterType.OctDigit;
//                if (DecimalDigits.Contains(ch))
//                    return CharacterType.DecDigit;
//                if (ArithmeticOperators.Contains(ch))
//                    return CharacterType.ArithmeticOperator;
//                return CharacterType.InvalidCharacter;
//            }
//        }

        private int _row;
        private int _column;
        private int _state;

        private enum LexerStateType : int
        {
            InvalidExpration = -1,
            LexerStart = 0,
            Ident,
            Separator,
            ArithmeticOperator,

            BinNumberStart,
            BinNumber,
            OctNumberStart,
            OctNumber,
            HexNumberStart,
            HexNumber,
            DecNumber,
        }

        private static readonly int StateNumber = Enum.GetValues(typeof(LexerStateType)).Length;

        private List<Hashtable> _transitions = new List<Hashtable>(StateNumber);

        public LexerDFA()
        {
            _row = 1;
            _column = 1;
            _state = 0;
            InitTransitions();
        }

        private bool InitTransitions()
        {
            StreamReader sr = new StreamReader("../FEFUPascalCompiler/FEFUPascalCompiler/Lexer/StateTable");
            string line;
            
            //Get codes of valid characters
            line = sr.ReadLine();
            if (line == null)
            {
                return false;
            }
            string[] words = line.Split(' ');
            List<char> characters = new List<char>();
            foreach (var word in words)
            {
                int char_code;
                characters.Add((char)(int.TryParse(word, out char_code) ? char_code : -1));
            }
            
            int new_state = 0, state = 0;
            line = sr.ReadLine();
            while (line != null)
            {
                words = line.Split(' ');
                for (int i = 0; i < words.Length; ++i)
                {
                    _transitions[state].Add(characters[i], int.Parse(words[i]));
                }
                ++state;
                line = sr.ReadLine();
            }

            return true;
        }
        
        
    }
}