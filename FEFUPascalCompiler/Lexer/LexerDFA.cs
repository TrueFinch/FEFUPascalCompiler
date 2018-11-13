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
            Ampersand,
            Number,
            DoubleNumberStart,
            DoubleNumber,
            LexemeEnd,
            Assign,
            StartStringConst,
            FinishStringConst,
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
                {"Ampersand", LexerStateType.Ampersand},
                {"Number", LexerStateType.Number},
                {"DoubleNumberStart", LexerStateType.DoubleNumberStart},
                {"DoubleNumber", LexerStateType.DoubleNumber},
                {"LexemeEnd", LexerStateType.LexemeEnd},
                {"Assign", LexerStateType.Assign},
                {"StartStringConst", LexerStateType.StartStringConst},
                {"FinishStringConst", LexerStateType.FinishStringConst},
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
            {LexerStateType.Ampersand, false},
            {LexerStateType.Number, true},
            {LexerStateType.DoubleNumberStart, false},
            {LexerStateType.DoubleNumber, true},
            {LexerStateType.LexemeEnd, true},
            {LexerStateType.Assign, true},
            {LexerStateType.StartStringConst, false},
            {LexerStateType.FinishStringConst, true},
        };

        private static readonly Dictionary<LexerStateType, TokenType> LexerStateTypeToTokenType
            = new Dictionary<LexerStateType, TokenType>
            {
                {LexerStateType.Ident, TokenType.IDENT},
                {LexerStateType.Number, TokenType.TYPE_INTEGER},
                {LexerStateType.DoubleNumber, TokenType.TYPE_DOUBLE},
                {LexerStateType.SemiColon, TokenType.SEPARATOR},
                {LexerStateType.Colon, TokenType.SEPARATOR},
                {LexerStateType.Dot, TokenType.SEPARATOR},
                {LexerStateType.Comma, TokenType.SEPARATOR},
                {LexerStateType.BinArithmeticOperator, TokenType.BIN_ARITHM_OPERATOR},
                {LexerStateType.Assign, TokenType.SMP_ASSIGN},
                {LexerStateType.FinishStringConst, TokenType.TYPE_STRING},
            };

        internal static Token GetToken(int line, int column, string text, LexerStateType lexerState)
        {
            if (LexerStateTypeToTokenType.ContainsKey(lexerState))
            {
                return Token.GetToken(line, column, LexerStateTypeToTokenType[lexerState], text);
            }

            return null;
        }

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

        public LexerDFA()
        {
            InitTransitions();
            _currentToken = null;
        }

        private void InitTransitions()
        {
            _statesList.Add(new Node(LexerStateType.Start, TerminalStates[LexerStateType.Start]));
            _statesList[0].Transitions.Add('\n', new Pair<LexerStateType, int>(LexerStateType.Start, 1));
            _statesList[0].Transitions.Add(' ', new Pair<LexerStateType, int>(LexerStateType.Start, 1));
            _statesList[0].Transitions.Add('\uffff', new Pair<LexerStateType, int>(LexerStateType.LexemeEnd, 1));

            for (int i = 1; i < _statesList.Capacity - 1; ++i)
            {
                _statesList.Add(new Node((LexerStateType) i, TerminalStates[(LexerStateType) i]));
                _statesList[_statesList.Count - 1].Transitions.Add('\n',
                    new Pair<LexerStateType, int>(LexerStateType.LexemeEnd, 1));
                _statesList[_statesList.Count - 1].Transitions.Add(' ',
                    new Pair<LexerStateType, int>(LexerStateType.LexemeEnd, 1));
                _statesList[_statesList.Count - 1].Transitions.Add('\uffff',
                    new Pair<LexerStateType, int>(LexerStateType.LexemeEnd, 1));
            }

            StreamReader sr = new StreamReader(@"Lexer/Transitions");
            string line = sr.ReadLine();
            while (line != null)
            {
                if (line.Length == 0)
                {
                    continue;
                }

                string[] words = line.Split(" ");
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
            if ((_input.EndOfStream) || ((_currentToken == null) && (_input == null)))
            {
                return false;
            }

            _currentToken = Parse();
            return true;
        }

        private Token Parse()
        {
            StringBuilder text = new StringBuilder();
            Node lastState = _statesList[(int) LexerStateType.Start],
                currState = _statesList[(int) LexerStateType.Start];
            int line = _line, column = _column;

            while (currState.Type != LexerStateType.LexemeEnd)
            {
                if (!currState.Transitions.ContainsKey((char) _input.Peek()))
                {
                    text.Append((char) _input.Peek());
                    _input.ReadLine();
                    ++_line;
                    _column = 1;
                    throw new UnexpectedSymbolException($"({line},{column + 1}) Unexpected symbol in lexeme {text}");
                }

                lastState = currState;
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
                if (lastState.Type == LexerStateType.StartStringConst)
                {
                    throw new UnclosedStringConstException($"({line},{column}) Unclosed string constant lexeme {text}");
                }

                throw new UnexpectedSymbolException($"({line},{column}) Unexpected symbol in lexeme {text}");
            }

            return text.Length == 0
                ? null
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
            _stateType = 0;
            NextToken();
        }
    }
}