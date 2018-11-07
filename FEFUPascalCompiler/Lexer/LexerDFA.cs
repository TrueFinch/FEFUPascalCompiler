using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Lexer
{
    internal class LexerDFA
    {
        internal static Token GetToken(int line, int column, string text, LexerStateType lexerState)
        {
            Token token = null;
            switch (lexerState)
            {
                case LexerStateType.InvalidExpr:
                    break;
                case LexerStateType.Start:
                    break;
                case LexerStateType.Ident:
                    if (Dictionaries.KeyWords.ContainsKey(text.ToLower()))
                    {
                        token = new KeyWordToken(line, column, Dictionaries.KeyWords[text.ToLower()], text);
                    }
                    else
                    {
                        token = new IdentToken(line, column, TokenType.IDENT, text);
                    }

                    break;
                case LexerStateType.SemiColon:
                    token = new SemiColonToken(line, column, TokenType.SEMICOLON, text);
                    break;
                case LexerStateType.Colon:
                    token = new ColonToken(line, column, TokenType.COLON, text);
                    break;
                case LexerStateType.Dot:
                    token = new DotToken(line, column, TokenType.DOT, text);
                    break;
                case LexerStateType.Comma:
                    break;
                case LexerStateType.BinArithmeticOperator:
                    break;
                case LexerStateType.Percent:
                    break;
                case LexerStateType.BinNumber:
                    break;
                case LexerStateType.Ampersand:
                    break;
                case LexerStateType.OctNumber:
                    break;
                case LexerStateType.Dollar:
                    break;
                case LexerStateType.HexNumber:
                    break;
                case LexerStateType.DecNumber:
                    token = new IntegerToken(line, column, TokenType.TYPE_INTEGER, text);
                    break;
                case LexerStateType.DoubleNumberStart:
                    break;
                case LexerStateType.DoubleNumber:
                    break;
                case LexerStateType.LexemeEnd:
                    break;
                case LexerStateType.Assign:
                    token = new AssignToken(line, column, Dictionaries.Assigns[text], text);
                    break;
                case LexerStateType.EOF:
                    token = new EOFToken(line, column, TokenType.EOF, text);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lexerState), lexerState, null);
            }

            return token;
        }

        internal enum LexerStateType : int
        {
            InvalidExpr = -1,
            Start = 0,
            Ident,
            SemiColon,
            Colon,
            Dot,
            Comma,
            BinArithmeticOperator,
            Percent,
            BinNumber,
            Ampersand,
            OctNumber,
            Dollar,
            HexNumber,
            DecNumber,
            DoubleNumberStart,
            DoubleNumber,
            LexemeEnd,
            Assign,
            EOF,
        }

        private static readonly Dictionary<string, LexerStateType> StrToLexerStateType =
            new Dictionary<string, LexerStateType>
            {
                {"InvalidExpr", LexerStateType.InvalidExpr},
                {"Start", LexerStateType.Start},
                {"Ident", LexerStateType.Ident},
                {"SemiColon", LexerStateType.SemiColon},
                {"Colon", LexerStateType.Colon},
                {"Dot", LexerStateType.Dot},
                {"Comma", LexerStateType.Comma},
                {"BinArithmeticOperator", LexerStateType.BinArithmeticOperator},
                {"Percent", LexerStateType.Percent},
                {"BinNumber", LexerStateType.BinNumber},
                {"Ampersand", LexerStateType.Ampersand},
                {"OctNumber", LexerStateType.OctNumber},
                {"Dollar", LexerStateType.Dollar},
                {"HexNumber", LexerStateType.HexNumber},
                {"DecNumber", LexerStateType.DecNumber},
                {"DoubleNumberStart", LexerStateType.DoubleNumberStart},
                {"DoubleNumber", LexerStateType.DoubleNumber},
                {"LexemeEnd", LexerStateType.LexemeEnd},
                {"Assign", LexerStateType.Assign},
                {"EOF", LexerStateType.EOF},
            };

        private static readonly Dictionary<LexerStateType, bool> TerminalStates = new Dictionary<LexerStateType, bool>
        {
            {LexerStateType.InvalidExpr, true},
            {LexerStateType.Start, true},
            {LexerStateType.Ident, true},
            {LexerStateType.SemiColon, true},
            {LexerStateType.Colon, true},
            {LexerStateType.Dot, true},
            {LexerStateType.Comma, true},
            {LexerStateType.BinArithmeticOperator, true},
            {LexerStateType.Percent, false},
            {LexerStateType.BinNumber, true},
            {LexerStateType.Ampersand, false},
            {LexerStateType.OctNumber, true},
            {LexerStateType.Dollar, false},
            {LexerStateType.HexNumber, true},
            {LexerStateType.DecNumber, true},
            {LexerStateType.DoubleNumberStart, false},
            {LexerStateType.DoubleNumber, true},
            {LexerStateType.LexemeEnd, true},
            {LexerStateType.Assign, true},
            {LexerStateType.EOF, true},
        };

        private static readonly int StateNumber = Enum.GetValues(typeof(LexerStateType)).Length;

        private class Pair<T1, T2>
        {
            internal T1 StateType { get; }
            internal T2 Shift { get; }

            public Pair(T1 type, T2 shift)
            {
                StateType = type;
                Shift = shift;
            }
        }

        private struct Node
        {
            internal readonly LexerStateType Type;
            internal readonly bool Terminal;
            internal readonly Dictionary<char, Pair<LexerStateType, int>> Transitions;

            public Node(LexerStateType type, bool terminal)
            {
                Type = type;
                Terminal = terminal;
                Transitions = new Dictionary<char, Pair<LexerStateType, int>>();
            }
        }

        private List<Node> _statesList = new List<Node>(StateNumber);

        private int _line;
        private int _column;
        private LexerStateType _stateType;
        private StreamReader _input;
        private Token _currentToken = null;

        public LexerDFA(StreamReader input)
        {
            SetInput(input);
            _stateType = 0;
            InitTransitions();
            NextToken();
        }

        public LexerDFA()
        {
            _currentToken = null;
        }

        private void InitTransitions()
        {
            for (int i = 0; i < _statesList.Capacity - 1; ++i)
            {
                _statesList.Add(new Node((LexerStateType) i, TerminalStates[(LexerStateType) i]));
            }

            StreamReader sr = new StreamReader(@"Lexer/Transitions");
            string line = sr.ReadLine();
            while (line != null)
            {
                if (line.Length == 0)
                {
                    continue;
                }

//                Console.WriteLine(line);
                string[] words = line.Split(" ");
                /*
                 * replace \n with \\n because \\ is representation of character \
                 */
                if (words[1] == "\\n")
                {
                    _statesList[(int) StrToLexerStateType[words[0]]].Transitions.Add('\n',
                        new Pair<LexerStateType, int>(StrToLexerStateType[words[2]], int.Parse(words[3]))
                    );
                }
                else if (words[1] == "32")
                {
                    _statesList[(int) StrToLexerStateType[words[0]]].Transitions.Add(' ',
                        new Pair<LexerStateType, int>(StrToLexerStateType[words[2]], int.Parse(words[3]))
                    );
                }
                else
                {
                    foreach (char ch in words[1])
                    {
                        _statesList[(int) StrToLexerStateType[words[0]]].Transitions.Add(ch,
                            new Pair<LexerStateType, int>(StrToLexerStateType[words[2]], int.Parse(words[3]))
                        );
                    }
                }

                line = sr.ReadLine();
            }
        }

        public bool NextToken()
        {
            if ((_currentToken is EOFToken) || ((_currentToken == null) && (_input == null)))
            {
                return false;
            }
            _currentToken = Parse();
            return true;
        }

        private Token Parse()
        {
            StringBuilder text = new StringBuilder();
            _stateType = LexerStateType.Start;
            Node lastState = _statesList[(int) _stateType], currState = _statesList[(int) _stateType];
            int line = _line;
            int column = _column;

            while ((currState.Type != LexerStateType.LexemeEnd) && (!_input.EndOfStream))
            {
                if (!currState.Transitions.ContainsKey((char) _input.Peek()))
                {
                    //TODO: throw exception unexpected symbol
                    text.Append((char) _input.Peek());
                    throw new UnexpectedSymbol(line, column + 1, text.ToString());
                }

                lastState = currState;
//                Console.WriteLine((char) _input.Peek());
                var transition = lastState.Transitions[(char) _input.Peek()];
                currState = _statesList[(int) transition.StateType];

                if (_input.Peek() == '\n')
                {
                    ++_line;
                    _column = 1;
                }
                else
                {
                    _column += transition.Shift;
                }

                for (int i = 0; i < transition.Shift; ++i)
                {
                    if ((currState.Type == LexerStateType.LexemeEnd)
                        || ((currState.Type == LexerStateType.Start) && ((_input.Peek() == 10) || _input.Peek() == 32)))
                    {
                        _input.Read();
                    }
                    else
                    {
                        text.Append((char) _input.Read());
                    }
                }
            }

            if ((currState.Type == LexerStateType.LexemeEnd) && (!lastState.Terminal))
            {
                throw new UnexpectedSymbol(line, column, text.ToString());
                //TODO throw exception unexpected symbol
            }

            return _input.EndOfStream
                ? GetToken(line, column, "", LexerStateType.EOF)
                : GetToken(line, column, text.ToString(), lastState.Type);
        }

        public Token PeekToken()
        {
            return _currentToken;
        }

        public void SetInput(StreamReader input)
        {
            _line = 1;
            _column = 1;
            _input = input;
            _currentToken = null;
        }
    }
}