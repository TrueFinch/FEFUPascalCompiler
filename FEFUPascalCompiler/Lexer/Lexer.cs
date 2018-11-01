using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Lexer
{
    internal static class Characters
    {
        private static string letterAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string digitAlphabet = "0123456789";
        private static string arithmeticOperators = "+-*/";

        public static readonly HashSet<char> IdentSymbols =
            new HashSet<char>((letterAlphabet + letterAlphabet.ToLower() + '_').ToCharArray());
    }

    public class Lexer : ILexer
    {
        public enum States
        {
            Start,
            Ident,
            Operator,
            Const
        }
        
        private readonly IdentDFA _identDFA;
        private int _line;
        private int _column;

        public Lexer()
        {
            _column = 1;
            _line = 1;
            _identDFA = new IdentDFA();
        }

        private States State { get; set; }

        public Token NextToken(StreamReader input)
        {
            State = States.Start;
            while (true)
            {
                var nextSign = (char) input.Peek();
                switch (State)
                {
                    case States.Start:
                        if (Characters.IdentSymbols.Contains(nextSign))
                            State = States.Ident;
                        else if (char.IsDigit(nextSign))
                            State = States.Const;
                        break;
                    case States.Ident:
                        return _identDFA.GetToken(input, ref _line, ref _column);
                        break;
                    case States.Operator:
                        break;
                    case States.Const:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        class IdentDFA
        {
            private enum CharCode
            {
                IdentSymbol = 0,
                Digit,
                Other
            }

            internal enum States
            {
                Start = 0,
                CharGot,
                End,
                Error
            }

            public States State { get; set; }

            private readonly List<List<States>> _stateTable = new List<List<States>>
            {
                // @formatter:off
                //                            a-z A-Z         0-9             other symbols
                /*Start*/   new List<States> {States.CharGot, States.Error,   States.End},
                /*CharGot*/ new List<States> {States.CharGot, States.CharGot, States.End},
                /*End*/     new List<States> {States.End,     States.End,     States.End},
                /*Error*/   new List<States> {States.Error,   States.Error,   States.Error}
                // @formatter:on
            };

            public Token GetToken(StreamReader input, ref int line, ref int column)
            {
                State = States.Start;
                StringBuilder text = new StringBuilder();

                while (true)
                {
                    char nextSign = (char) input.Peek();
                    switch (State)
                    {
                        case States.Start:
                            if (Characters.IdentSymbols.Contains(nextSign))
                                State = _stateTable[(int) State][(int) CharCode.IdentSymbol];
                            else if (char.IsDigit(nextSign))
                                State = _stateTable[(int) State][(int) CharCode.Digit];
                            else
                                State = _stateTable[(int) State][(int) CharCode.Other];
                            break;
                        case States.CharGot:
                            if (Characters.IdentSymbols.Contains(nextSign) || char.IsDigit(nextSign))
                            {
                                text.Append(nextSign);
                                input.Read();
                                State = _stateTable[(int) State][(int) CharCode.Digit];
                            }
                            else
                            {
                                State = _stateTable[(int) State][(int) CharCode.Other];
                            }

                            break;
                        case States.End:
                            var token = new IdentValueToken(line, column,
                                (Dictionaries.keyWords.ContainsKey(text.ToString()))
                                    ? (Dictionaries.keyWords[text.ToString()])
                                    : (TokenType.VARIABLE),
                                text.ToString());
                            column += text.Length;
                            return token;
                            break;
                        case States.Error:
                            //TODO: throw some exception
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        class ConstDFA
        {
            
        }
    }
}