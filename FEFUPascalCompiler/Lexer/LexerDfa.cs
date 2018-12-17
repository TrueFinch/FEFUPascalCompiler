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
            InvalidSign,
        }


        internal static Token GetToken(int line, int column, string lexeme, LexerState lexerState)
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
                {LexerState.CloseSquareBracket    , TokenType.Bracket           },
                {LexerState.SingleLineComment     , TokenType.SingleLineComment },
                {LexerState.SumArithmOperator     , TokenType.BinOperator       },
                {LexerState.DifArithmOperator     , TokenType.BinOperator       },
                {LexerState.MulArithmOperator     , TokenType.BinOperator       },
                {LexerState.DivArithmOperator     , TokenType.BinOperator       },
                {LexerState.PowArithmOperator     , TokenType.BinOperator       },
                {LexerState.DoubleDotOperator     , TokenType.BinOperator       },
                {LexerState.StringConstFinish     , TokenType.StringConst       },
                {LexerState.OpenSquareBracket     , TokenType.Bracket           },         
                {LexerState.DoubleNumber          , TokenType.DoubleNumber      },
                {LexerState.CloseBracket          , TokenType.Bracket           },
                {LexerState.OpenBracket           , TokenType.Bracket           },
                {LexerState.BinNumber             , TokenType.IntegerNumber     },
                {LexerState.OctNumber             , TokenType.IntegerNumber     },
                {LexerState.DecNumber             , TokenType.IntegerNumber     },
                {LexerState.HexNumber             , TokenType.IntegerNumber     },
                {LexerState.SemiColon             , TokenType.Separator         },
                {LexerState.Assign                , TokenType.AssignOperator    },
                {LexerState.Colon                 , TokenType.Separator         },
                {LexerState.Ident                 , TokenType.Ident             },
                {LexerState.Comma                 , TokenType.Separator         },
                {LexerState.Dot                   , TokenType.Separator         },
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
                // @formatter:off
            }
        }

        // @formatter:off
        private int                  _line        ;
        private int                  _column      ;
        private BufferedStreamReader _inputStream ;
        private Token                _currentToken;
        private bool                 _stopLexer   ;
        private bool                 _gotError    ;
        // @formatter:on

        public LexerDfa()
        {
            _statesList = TransitionsTable.InitTransitions();
        }

        public bool NextToken()
        {
            if ((_inputStream.EndOfStream()) || ((_currentToken == null) && (_inputStream == null)) || _stopLexer ||
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
                _stopLexer = true;
                throw;
            }

            return true;
        }

        private Token Parse()
        {
            StringBuilder lexeme = new StringBuilder();
            Node lastState = _statesList[(int) LexerState.Start],
                currState = _statesList[(int) LexerState.Start];
            int line = _line, column = _column;

            while ((currState.Type != LexerState.LexemeEnd) && (!_inputStream.EndOfStream()))
            {
                if (!currState.Transitions.ContainsKey((char) _inputStream.Peek()))
                {
                    if (((currState.Type == LexerState.StringConstStart) && (_inputStream.Peek() != '\n'))
                        || (currState.Type == LexerState.MultiLineCommentStart)
                        || ((currState.Type == LexerState.SingleLineComment) && (_inputStream.Peek() != '\n')))
                    {
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

                var transition = currState.Transitions[(char) _inputStream.Peek()];

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

            if (currState.Type == LexerState.StringConstStart)
            {
                _stopLexer = _gotError = true;
                throw new UnclosedStringConstException(
                    $"({line},{column}) Unclosed string constant lexeme {lexeme}");
            }
            
            if (currState.Type == LexerState.MultiLineCommentStart)
            {
                throw new UnclosedMultilineCommentException(
                    $"({line},{column}) Unclosed string constant lexeme {lexeme}");
            }
            
            if ((currState.Type == LexerState.LexemeEnd) && (!lastState.Terminal))
            {
//                lexeme.Append((char) _inputStream.Peek());
//                _inputStream.ReadLine();
//                ++_line;
//                _column = 1;
                _stopLexer = _gotError = true;
                throw new UnexpectedSymbolException($"({_line},{_column - 1}) Unexpected symbol in lexeme {lexeme}");
            }
            
            var nextToken = (currState.Type == LexerState.SingleLineComment) 
                            || (currState.Type == LexerState.MultiLineCommentFinish) 
                ? GetToken(line, column, lexeme.ToString(), currState.Type)
                : GetToken(line, column, lexeme.ToString(), lastState.Type);

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

        public void InitLexer(in StreamReader input)
        {
            // @formatter:off
            _gotError        = false;
            _stopLexer       = false;
            _currentToken    = null;
            _line            = _column = 1;
            _inputStream     = new BufferedStreamReader(in input);
            _currentToken    = null;
            // @formatter:on
        }
    }
}