using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Lexer
{
    internal partial class LexerDfa
    {
        internal enum LexerState
        {
            Start,
            Ident,
            DecNumber,
            OctNumberStart,
            OctNumber,
            BinNumberStart,
            BinNumber,
            HexNumberStart,
            HexNumber,
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

            NotEqualOperator,
            EqualOperator,
            LessOperator,
            LessOrEqualOperator,
            GreaterOperator,
            GreaterOrEqualOperator,

//            DoubleNumberStart,
            DoubleNumber,
            ExpDoubleStart,
            ExpDoubleMinus,
            ExpDouble,
            LexemeEnd,
            Assign,
            OpenBracket,
            CloseBracket,
            OpenSquareBracket,
            CloseSquareBracket,
            MultiLineCommentStart,
            MultiLineCommentFinish,
            SingleLineComment,
            InvalidSign,

            SignCodeStart,
            SignCodeFinish,
            CharStringStart,
            CharStringFinish,
            CharStart,
            CharChar,
            CharFinish,
            ControlStringStart,
            ControlStringFinish,

            Carriage,
            AtSign,
        }


        private static Token GetToken(int line, int column, string lexeme, LexerState lexerState)
        {
            if (LexerStateTypeToTokenType.ContainsKey(lexerState))
            {
                return Token.TokenConstructors[LexerStateTypeToTokenType[lexerState]](line, column, lexeme);
            }

            return null;
        }

        private static readonly Dictionary<LexerState, TokenType> LexerStateTypeToTokenType
            = new Dictionary<LexerState, TokenType>
            {
                // @formatter:off
                {LexerState.MultiLineCommentFinish, TokenType.MultiLineComment  },
                {LexerState.CloseSquareBracket    , TokenType.Separator},
                {LexerState.SingleLineComment     , TokenType.SingleLineComment },
                {LexerState.SumArithmOperator     , TokenType.BinOperator       },
                {LexerState.DifArithmOperator     , TokenType.BinOperator       },
                {LexerState.MulArithmOperator     , TokenType.BinOperator       },
                {LexerState.DivArithmOperator     , TokenType.BinOperator       },
                {LexerState.PowArithmOperator     , TokenType.BinOperator       },
                {LexerState.DoubleDotOperator     , TokenType.BinOperator       },
                {LexerState.NotEqualOperator      , TokenType.BinOperator       },
                {LexerState.EqualOperator         , TokenType.BinOperator       },
                {LexerState.LessOperator          , TokenType.BinOperator       },
                {LexerState.LessOrEqualOperator   , TokenType.BinOperator       },
                {LexerState.GreaterOperator       , TokenType.BinOperator       },
                {LexerState.GreaterOrEqualOperator, TokenType.BinOperator       },
                {LexerState.OpenSquareBracket     , TokenType.Separator },         
//                {LexerState.SignCodeFinish        , TokenType.StringConst       },
                {LexerState.DoubleNumber          , TokenType.FloatNumber      },
                {LexerState.CloseBracket          , TokenType.Separator         },
                {LexerState.OpenBracket           , TokenType.Separator         },
                {LexerState.ExpDouble             , TokenType.FloatNumber      },
                {LexerState.BinNumber             , TokenType.IntegerNumber  },
                {LexerState.OctNumber             , TokenType.IntegerNumber  },
                {LexerState.DecNumber             , TokenType.IntegerNumber  },
                {LexerState.HexNumber             , TokenType.IntegerNumber  },
                {LexerState.SemiColon             , TokenType.Separator         },
                {LexerState.Assign                , TokenType.AssignOperator    },
                {LexerState.Colon                 , TokenType.Separator         },
                {LexerState.Ident                 , TokenType.Ident             },
                {LexerState.Comma                 , TokenType.Separator         },
                {LexerState.Dot                   , TokenType.Separator         },
                {LexerState.SignCodeFinish        , TokenType.CharConst         },
                {LexerState.CharFinish            , TokenType.CharConst         },
                {LexerState.ControlStringFinish   , TokenType.StringConst         },
                {LexerState.CharStringFinish      , TokenType.StringConst         },
                {LexerState.Carriage              , TokenType.Separator         },
                {LexerState.AtSign                , TokenType.Separator         },
//                {LexerState.                , TokenType.Separator         },
                // @formatter:on
            };

        private class Pair<T1, T2>
        {
            // @formatter:off
            internal T1 StateType { get; }
            internal T2 Shift     { get; }
            // @formatter:on

            public Pair(T1 type, T2 shift)
            {
                // @formatter:off
                StateType = type ;
                Shift     = shift;
                // @formatter:on
            }
        }

        private Token Parse()
        {
            StringBuilder lexeme = new StringBuilder();
            Node lastState = _statesList[(int) LexerState.Start],
                currState = _statesList[(int) LexerState.Start];
            int line = _line, column = _column;

            while ((currState.Type != LexerState.LexemeEnd))
            {
                if (!currState.Transitions.ContainsKey(_inputStream.Peek()))
                {
                    if ((_inputStream.Peek() != '\uffff')
                        && (((currState.Type == LexerState.CharStringStart) && (_inputStream.Peek() != '\n'))
                            || (currState.Type == LexerState.CharStart) && (_inputStream.Peek() != '\n')
                            || (currState.Type == LexerState.CharChar) && (_inputStream.Peek() != '\n')
                            || (currState.Type == LexerState.MultiLineCommentStart)
                            || ((currState.Type == LexerState.SingleLineComment) && (_inputStream.Peek() != '\n'))))
                    {

                        if ((currState.Type == LexerState.CharChar) && (_inputStream.Peek() != '\n'))
                        {
                            lastState = currState;
                            currState = new Node(LexerState.CharStringStart, TerminalStates[LexerState.CharStringStart],
                                new Dictionary<char, Pair<LexerState, int>>());
                            currState.Transitions.TryAdd('\'', new Pair<LexerState, int>(LexerState.CharStringFinish, 1));
                        }
                        
                        if ((currState.Type == LexerState.CharStart) && (_inputStream.Peek() != '\n'))
                        {
                            lastState = currState;
                            currState = new Node(LexerState.CharChar, TerminalStates[LexerState.CharChar],
                                new Dictionary<char, Pair<LexerState, int>>());
                            currState.Transitions['\''] = new Pair<LexerState, int>(LexerState.CharFinish, 1);
                        }

                        _line += _inputStream.Peek() == 10 ? 1 : 0;
                        _column = _inputStream.Peek() == 10 ? 1 : _column + 1;
                        var ch = _inputStream.Read();
                        if (ch == '\n')
                        {
                            lexeme.Append('\\');
                            lexeme.Append('n');
                        }
                        else
                        {
                            lexeme.Append(ch);
                        }

                        continue;
                    }

                    lastState = currState;
                    currState = new Node(LexerState.LexemeEnd, TerminalStates[LexerState.LexemeEnd],
                        new Dictionary<char, Pair<LexerState, int>>());
                    continue;
                }

                var transition = currState.Transitions[_inputStream.Peek()];

                if (transition.Shift == 1)
                {
                    lastState = currState;
                    currState = _statesList[transition.StateType];
                    if ((currState.Type == LexerState.Start) &&
                        ((_inputStream.Peek() == 10) || _inputStream.Peek() == 32))
                    {
                        line += _inputStream.Peek() == 10 ? 1 : 0;
                        column = _inputStream.Peek() == 10 ? 1 : _column + 1;
                        _line = line;
                        _column = column;
                        _inputStream.Read();
                    }
                    else
                    {
                        ++_column;
                        lexeme.Append(_inputStream.Read());
                    }
                }
                else if (transition.Shift == -1)
                {
                    --_column;
                    _inputStream.Kick(lexeme[lexeme.Length - 1]);
                    lexeme.Remove(lexeme.Length - 1, 1);
                    currState = new Node(LexerState.LexemeEnd, TerminalStates[LexerState.LexemeEnd],
                        new Dictionary<char, Pair<LexerState, int>>());
                }
            }

            if ((lastState.Type == LexerState.CharStringStart || lastState.Type == LexerState.CharStart ||
                 lastState.Type == LexerState.CharChar) && currState.Type == LexerState.LexemeEnd)
            {
                _tokenizeFinished = _gotError = true;
                throw new UnclosedStringConstException(
                    $"({line},{column}) Unclosed string constant lexeme {lexeme}");
            }

            if ((lastState.Type == LexerState.MultiLineCommentStart) && currState.Type == LexerState.LexemeEnd)
            {
                _tokenizeFinished = _gotError = true;
                throw new UnclosedMultilineCommentException(
                    $"({line},{column}) Unclosed multiline comment lexeme {lexeme}");
            }

            if ((currState.Type == LexerState.LexemeEnd) && (!lastState.Terminal))
            {
                _tokenizeFinished = _gotError = true;
                throw new UnexpectedSymbolException($"({_line},{_column - 1}) Unexpected symbol in lexeme {lexeme}");
            }


            var nextToken = (currState.Type == LexerState.SingleLineComment)
                            || (currState.Type == LexerState.MultiLineCommentFinish)
                            || (currState.Type == LexerState.ControlStringFinish)
                            || (currState.Type == LexerState.CharStringFinish)
                ? GetToken(line, column, lexeme.ToString(), currState.Type)
                : GetToken(line, column, lexeme.ToString(), lastState.Type);

            if ((_currentToken?.Type == TokenType.End) && (nextToken?.Type == TokenType.Dot))
            {
                _tokenizeFinished = true;
            }

            return nextToken;
        }

        public bool NextToken()
        {
            if ((_inputStream.EndOfStream()) || ((_currentToken == null) && (_inputStream == null)) ||
                _tokenizeFinished ||
                _gotError)
            {
                return false;
            }

            try
            {
                _currentToken = Parse();
                if (_currentToken == null)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                _gotError = true;
                _tokenizeFinished = true;
                throw;
            }

            return true;
        }


        public Token PeekToken()
        {
            return _currentToken;
        }

        public void InitLexer(in StreamReader input)
        {
            // @formatter:off
            _gotError         = false;
            _tokenizeFinished = false;
            _currentToken     = null;
            _line             = _column = 1;
            _inputStream      = new BufferedStreamReader(in input);
            // @formatter:on
        }

        public bool IsReady()
        {
            return !_gotError && !_tokenizeFinished && _currentToken == null && _inputStream != null;
        }

        public LexerDfa()
        {
            _statesList = TransitionsTable.InitTransitions();
        }

        // @formatter:off
        private BufferedStreamReader _inputStream;
        private Token                _currentToken;
        private int                  _line        ;
        private int                  _column      ;
        private bool                 _tokenizeFinished = true;
        private bool                 _gotError    ;
        // @formatter:on
    }
}