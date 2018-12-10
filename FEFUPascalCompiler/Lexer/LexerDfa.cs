using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Lexer
{
    internal partial class LexerDfa
    {
        //TODO: And error state, that represents error incorrect numbers like &81, $HELLO
        //TODO: Don't forget rewrite tests after this
        internal enum LexerState
        {
            Start,
            Ident,
            Ampersand,
            IntNumber,
            ConstNumberStart,
            ConstNumber,
            SemiColon,
            Colon,
            Dot,
            Comma,
            SumArithmOperator,
            DifArithmOperator,
            MulArithmOperator,
            DivArithmOperator,
            PowArithmOperator,
            DoubleDotOperator,
            DoubleNumberStart,
            DoubleNumber,
            LexemeEnd,
            Assign,
            StringConstStart,
            StringConstFinish,
            OpenBracket,
            CloseBracket,
            OpenSquareBracket,
            CloseSquareBracket,
            MultiLineCommentStart,
            MultiLineCommentFinish,
            SingleLineComment,
            UnexpectedSymbol,
        }

        private static readonly Dictionary<LexerState, TokenType> LexerStateTypeToTokenType
            = new Dictionary<LexerState, TokenType>
            {
                {LexerState.Ident, TokenType.Ident},
                {LexerState.ConstNumber, TokenType.IntegerNumber},
                {LexerState.IntNumber, TokenType.IntegerNumber},
                {LexerState.DoubleNumber, TokenType.DoubleNumber},
                {LexerState.StringConstFinish, TokenType.StringConst},
                {LexerState.SemiColon, TokenType.Separator},
                {LexerState.Colon, TokenType.Separator},
                {LexerState.Dot, TokenType.Separator},
                {LexerState.Comma, TokenType.Separator},
                {LexerState.SumArithmOperator, TokenType.BinOperator},
                {LexerState.DifArithmOperator, TokenType.BinOperator},
                {LexerState.MulArithmOperator, TokenType.BinOperator},
                {LexerState.DivArithmOperator, TokenType.BinOperator},
                {LexerState.PowArithmOperator, TokenType.BinOperator},
                {LexerState.DoubleDotOperator, TokenType.BinOperator},
                {LexerState.Assign, TokenType.AssignOperator},
                {LexerState.MultiLineCommentFinish, TokenType.MultiLineComment},
                {LexerState.SingleLineComment, TokenType.SingleLineComment},
                {LexerState.OpenBracket, TokenType.Bracket},
                {LexerState.CloseBracket, TokenType.Bracket},
                {LexerState.OpenSquareBracket, TokenType.Bracket},
                {LexerState.CloseSquareBracket, TokenType.Bracket},
            };

        internal static Token GetToken(int line, int column, string lexeme, LexerState lexerState)
        {
            if (LexerStateTypeToTokenType.ContainsKey(lexerState))
            {
                return Token.TokenConstructors[LexerStateTypeToTokenType[lexerState]](line, column, lexeme);
            }

            return null;
        }

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

        private int _line;
        private int _column;
        private BufferedStreamReader _input;
        private Token _currentToken;
        private bool _stopLexer;
        private bool _gotError;

        public LexerDfa()
        {
            _statesList = TransitionsTable.InitTransitions();
        }

        public bool NextToken()
        {
            if ((_input.EndOfStream()) || ((_currentToken == null) && (_input == null)) || _stopLexer || _gotError)
            {
                return false;
            }

            try
            {
                _currentToken = Parse();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                _gotError = true;
                _stopLexer = true;
                return false;
            }

            return true;
        }

        private Token Parse()
        {
            StringBuilder lexeme = new StringBuilder();
            Node lastState = _statesList[(int) LexerState.Start],
                currState = _statesList[(int) LexerState.Start];
            int line = _line, column = _column;

            while (currState.Type != LexerState.LexemeEnd)
            {
                if (!currState.Transitions.ContainsKey((char) _input.Peek()))
                {
                    if (((currState.Type == LexerState.StringConstStart) && (_input.Peek() != '\n'))
                        || (currState.Type == LexerState.MultiLineCommentStart)
                        || ((currState.Type == LexerState.SingleLineComment) && (_input.Peek() != '\n')))
                    {
                        _line += _input.Peek() == 10 ? 1 : 0;
                        _column = _input.Peek() == 10 ? 1 : column + 1;
                        lexeme.Append(_input.Read());
                        continue;
                    }

                    lastState = currState;
                    currState = new Node(LexerState.LexemeEnd, TerminalStates[LexerState.LexemeEnd],
                        new Dictionary<char, Pair<LexerState, int>>());
                    continue;
                }

                var transition = currState.Transitions[(char) _input.Peek()];

                if (transition.Shift == 1)
                {
                    lastState = currState;
                    currState = _statesList[transition.StateType];
                    if ((currState.Type == LexerState.Start) && ((_input.Peek() == 10) || _input.Peek() == 32))
                    {
                        line += _input.Peek() == 10 ? 1 : 0;
                        column = _input.Peek() == 10 ? 1 : _column + 1;
                        _line = line;
                        _column = column;
                        _input.Read();
                    }
                    else
                    {
                        ++_column;
                        lexeme.Append(_input.Read());
                    }
                }
                else if (transition.Shift == -1)
                {
                    --_column;
                    _input.Kick(lexeme[lexeme.Length - 1]);
                    lexeme.Remove(lexeme.Length - 1, 1);
                    currState = new Node(LexerState.LexemeEnd, TerminalStates[LexerState.LexemeEnd],
                        new Dictionary<char, Pair<LexerState, int>>());
                }
            }

            if ((currState.Type == LexerState.LexemeEnd) && (!lastState.Terminal))
            {
                lexeme.Append((char) _input.Peek());
                _input.ReadLine();
                ++_line;
                _column = 1;
                _stopLexer = true;
                _gotError = true;
                if (lastState.Type == LexerState.StringConstStart)
                {
                    throw new UnclosedStringConstException(
                        $"({line},{column}) Unclosed string constant lexeme {lexeme}");
                }

                throw new UnexpectedSymbolException($"({line},{column + 1}) Unexpected symbol in lexeme {lexeme}");
            }

            var nextToken = lexeme.Length == 0 ? null : GetToken(line, column, lexeme.ToString(), lastState.Type);
            
            if ((_currentToken?.TokenType == TokenType.End) && (nextToken?.TokenType == TokenType.Dot))
            {
                _stopLexer = true;
            }

            return nextToken;
        }

        public Token PeekToken()
        {
            return _currentToken;
        }

        public void SetInput(ref StreamReader input)
        {
            _gotError = false;
            _stopLexer = false;
            _currentToken = null;
            _line = _column = 1;
            _input = new BufferedStreamReader(ref input);
            _currentToken = null;
        }
    }
}