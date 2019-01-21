using System;
using System.Collections.Generic;

namespace FEFUPascalCompiler.Lexer
{
    internal partial class LexerDfa
    {
        private struct Node
        {
            // @formatter:off
            internal readonly LexerState                              Type;
            internal readonly bool                                    Terminal;
            internal readonly Dictionary<char, Pair<LexerState, int>> Transitions;
            // @formatter:on

            public Node(LexerState type, bool terminal, Dictionary<char, Pair<LexerState, int>> transitions)
            {
                // @formatter:off
                Type        = type;
                Terminal    = terminal;
                Transitions = transitions;
                // @formatter:on
            }
        }

        private static readonly Dictionary<LexerState, bool> TerminalStates = new Dictionary<LexerState, bool>
        {
            // @formatter:off
            {LexerState.MultiLineCommentStart , false},
//            {LexerState.DoubleNumberStart     , false},
            {LexerState.ExpDoubleStart        , false},
            {LexerState.ExpDoubleMinus        , false},
            {LexerState.BinNumberStart        , false},
            {LexerState.OctNumberStart        , false},
            {LexerState.HexNumberStart        , false},
            {LexerState.InvalidSign           , false},
            {LexerState.MultiLineCommentFinish, true },
            {LexerState.CloseSquareBracket    , true },
            {LexerState.OpenSquareBracket     , true },
            {LexerState.DoubleDotOperator     , true },
            {LexerState.SumArithmOperator     , true },
            {LexerState.DifArithmOperator     , true },
            {LexerState.MulArithmOperator     , true },
            {LexerState.DivArithmOperator     , true },
            {LexerState.PowArithmOperator     , true },
            {LexerState.SingleLineComment     , true },
            {LexerState.DoubleNumber          , true },
            {LexerState.ExpDouble             , true },
            {LexerState.CloseBracket          , true },
            {LexerState.OpenBracket           , true },
            {LexerState.BinNumber             , true },
            {LexerState.OctNumber             , true },
            {LexerState.HexNumber             , true },
            {LexerState.DecNumber             , true },
            {LexerState.SemiColon             , true },
            {LexerState.LexemeEnd             , true },
            {LexerState.Assign                , true },
            {LexerState.Start                 , true },
            {LexerState.Ident                 , true },
            {LexerState.Colon                 , true },
            {LexerState.Comma                 , true },
            {LexerState.Dot                   , true },
            {LexerState.NotEqualOperator      , true },
            {LexerState.EqualOperator         , true },
            {LexerState.LessOperator          , true },
            {LexerState.LessOrEqualOperator   , true },
            {LexerState.GreaterOperator       , true },
            {LexerState.GreaterOrEqualOperator, true },
            {LexerState.Carriage              , true },
            {LexerState.AtSign                , true },
            
            {LexerState.SignCodeStart         , false},
            {LexerState.SignCodeFinish        , true },
            {LexerState.ControlStringStart    , false},
            {LexerState.ControlStringFinish   , true},
            {LexerState.CharStringStart       , false},
            {LexerState.CharStringFinish      , true },
            {LexerState.CharStart             , false},
            {LexerState.CharChar              , false},
            {LexerState.CharFinish            , true },
            // @formatter:on
        };

        private readonly Dictionary<LexerState, Node> _statesList;

        //contain transitions for dfa
        private static class TransitionsTable
        {
            public static Dictionary<LexerState, Node> InitTransitions()
            {
                Dictionary<LexerState, Node> nodes = new Dictionary<LexerState, Node>();

                //create nodes and init space, new line and eof transitions for all states
                foreach (LexerState state in Enum.GetValues(typeof(LexerState)))
                {
                    nodes.TryAdd(state,
                        new Node(state, TerminalStates[state], new Dictionary<char, Pair<LexerState, int>>()));
                    // @formatter:off
                    var tmp      = InitFunctions[state](nodes[state]);
                    nodes[state] = tmp ?? nodes[state];
                    // @formatter:on
                }

                return nodes;
            }

            private static readonly Dictionary<LexerState, Func<Node, Node?>> InitFunctions =
                new Dictionary<LexerState, Func<Node, Node?>>
                {
                    // @formatter:off
                    {LexerState.MultiLineCommentFinish     , InitMultiLineCommentFinishStateNode     },
                    {LexerState.GreaterOrEqualOperator     , InitGreaterOrEqualOperatorStateNode     },
                    {LexerState.MultiLineCommentStart      , InitMultiLineCommentStartStateNode      },
                    {LexerState.LessOrEqualOperator        , InitLessOrEqualOperatorStateNode        },
                    {LexerState.CloseSquareBracket         , InitCloseSquareBracketStateNode         },
                    {LexerState.OpenSquareBracket          , InitOpenSquareBracketStateNode          },
//                    {LexerState.DoubleNumberStart          , InitDoubleNumberStartStateNode          },
                    {LexerState.SingleLineComment          , InitSingleLineCommentStateNode          },
                    {LexerState.DoubleDotOperator          , InitDoubleDotOperatorStateNode          },
                    {LexerState.SumArithmOperator          , InitSumArithOperatorStateNode           },
                    {LexerState.DifArithmOperator          , InitDifArithOperatorStateNode           },
                    {LexerState.MulArithmOperator          , InitMulArithOperatorStateNode           },
                    {LexerState.DivArithmOperator          , InitDivArithOperatorStateNode           },
                    {LexerState.PowArithmOperator          , InitPowArithOperatorStateNode           },
                    {LexerState.NotEqualOperator           , InitNotEqualOperatorStateNode           },
                    {LexerState.GreaterOperator            , InitGreaterOperatorStateNode            },
                    {LexerState.BinNumberStart             , InitBinNumberStartStateNode             },
                    {LexerState.OctNumberStart             , InitOctNumberStartStateNode             },
                    {LexerState.HexNumberStart             , InitHexNumberStartStateNode             },
                    {LexerState.ExpDoubleStart             , InitExpDoubleStartStateNode             },
                    {LexerState.ExpDoubleMinus             , InitExpDoubleMinusStateNode             },
                    {LexerState.EqualOperator              , InitEqualOperatorStateNode              },
                    {LexerState.LessOperator               , InitLessOperatorStateNode               },
                    {LexerState.DoubleNumber               , InitDoubleNumberStateNode               },
                    {LexerState.CloseBracket               , InitCloseBracketStateNode               },
                    {LexerState.OpenBracket                , InitOpenBracketStateNode                },
                    {LexerState.InvalidSign                , InitInvalidSignStateNode                },
                    {LexerState.ExpDouble                  , InitExpDoubleStateNode                  },
                    {LexerState.BinNumber                  , InitBinNumberStateNode                  },
                    {LexerState.OctNumber                  , InitOctNumberStateNode                  },
                    {LexerState.HexNumber                  , InitHexNumberStateNode                  },
                    {LexerState.DecNumber                  , InitDecNumberStateNode                  },
                    {LexerState.SemiColon                  , InitSemiColonStateNode                  },
                    {LexerState.LexemeEnd                  , InitLexemeEndStateNode                  },
                    {LexerState.Assign                     , InitAssignStateNode                     },
                    {LexerState.Start                      , InitStartStateNode                      },
                    {LexerState.Ident                      , InitIdentStateNode                      },
                    {LexerState.Colon                      , InitColonStateNode                      },
                    {LexerState.Comma                      , InitCommaStateNode                      },
                    {LexerState.Dot                        , InitDotStateNode                        },
                    {LexerState.Carriage                   , InitCarriageStateNode                   },
                    {LexerState.AtSign                     , InitAtSignStateNode                     },
                    
                    
                    {LexerState.CharStringStart            , InitCharStringStartStateNode            },
                    {LexerState.CharStringFinish           , InitCharStringFinishStateNode           },
                    {LexerState.SignCodeStart              , InitSignCodeStartStateNode              },
                    {LexerState.SignCodeFinish             , InitSignCodeFinishStateNode             },
                    {LexerState.ControlStringStart         , InitControlStringStartStateNode         },
                    {LexerState.ControlStringFinish        , InitControlStringFinishStateNode        },
                    {LexerState.CharStart                  , InitCharStartStateNode                  },
                    {LexerState.CharChar                   , InitCharCharStateNode                   },
                    {LexerState.CharFinish                 , InitCharFinishStateNode                 },
                    // @formatter:on
                };

            private static Node? InitControlStringStartStateNode(Node node)
            {
                if (node.Type != LexerState.ControlStringStart)
                {
                    return null;
                }

                string decDigits = "0123456789";
                foreach (char digit in decDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.ControlStringFinish, 1));
                }

                return node;
            }

            private static Node? InitControlStringFinishStateNode(Node node)
            {
                if (node.Type != LexerState.ControlStringFinish)
                {
                    return null;
                }

                string decDigits = "0123456789";
                foreach (char digit in decDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.ControlStringFinish, 1));
                }
                node.Transitions['#'] = new Pair<LexerState, int>(LexerState.ControlStringStart, 1);
                node.Transitions['\''] = new Pair<LexerState, int>(LexerState.CharStringStart, 1);
                return node;
            }

            private static Node? InitCharStartStateNode(Node node)
            {
                return node.Type != LexerState.CharStart ? (Node?) null : node;
            }

            private static Node? InitCharCharStateNode(Node node)
            {
                if (node.Type != LexerState.CharChar)
                {
                    return null;
                }

                node.Transitions['\''] = new Pair<LexerState, int>(LexerState.CharFinish, 1);

                return node;
            }

            private static Node? InitCharFinishStateNode(Node node)
            {
                if (node.Type != LexerState.CharChar)
                {
                    return null;
                }

                node.Transitions['#'] = new Pair<LexerState, int>(LexerState.ControlStringStart, 1);

                return node;
            }

            private static Node? InitAtSignStateNode(Node node)
            {
                return node.Type != LexerState.AtSign ? (Node?) null : node;
            }

            private static Node? InitCarriageStateNode(Node node)
            {
                return node.Type != LexerState.Carriage ? (Node?) null : node;
            }

            private static Node? InitNotEqualOperatorStateNode(Node node)
            {
                return node.Type != LexerState.NotEqualOperator ? (Node?) null : node;
            }

            private static Node? InitEqualOperatorStateNode(Node node)
            {
                return node.Type != LexerState.EqualOperator ? (Node?) null : node;
            }

            private static Node? InitLessOrEqualOperatorStateNode(Node node)
            {
                return node.Type != LexerState.LessOrEqualOperator ? (Node?) null : node;
            }

            private static Node? InitGreaterOrEqualOperatorStateNode(Node node)
            {
                return node.Type != LexerState.GreaterOrEqualOperator ? (Node?) null : node;
            }

            private static Node? InitLessOperatorStateNode(Node node)
            {
                if (node.Type != LexerState.LessOperator)
                {
                    return null;
                }

                node.Transitions['>'] = new Pair<LexerState, int>(LexerState.NotEqualOperator, 1);
                node.Transitions['='] = new Pair<LexerState, int>(LexerState.LessOrEqualOperator, 1);
                return node;
            }

            private static Node? InitGreaterOperatorStateNode(Node node)
            {
                if (node.Type != LexerState.GreaterOperator)
                {
                    return null;
                }

                node.Transitions['='] = new Pair<LexerState, int>(LexerState.GreaterOrEqualOperator, 1);
                return node;
            }

            private static readonly string IdentCharacters = "_ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            private static readonly string DecDigits = "0123456789";


            private static Node? InitStartStateNode(Node node)
            {
                if (node.Type != LexerState.Start)
                {
                    return null;
                }

                node.Transitions['\n'] = new Pair<LexerState, int>(LexerState.Start, 1);
                node.Transitions[' '] = new Pair<LexerState, int>(LexerState.Start, 1);

                foreach (char letter in IdentCharacters)
                {
                    node.Transitions.TryAdd(letter, new Pair<LexerState, int>(LexerState.Ident, 1));
                }

                foreach (char digit in DecDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.DecNumber, 1));
                }

                // @formatter:off
                node.Transitions.TryAdd('}',  new Pair<LexerState, int>(LexerState.MultiLineCommentFinish, 1));
                node.Transitions.TryAdd('{',  new Pair<LexerState, int>(LexerState.MultiLineCommentStart , 1));
                node.Transitions.TryAdd(']',  new Pair<LexerState, int>(LexerState.CloseSquareBracket    , 1));
                node.Transitions.TryAdd('[',  new Pair<LexerState, int>(LexerState.OpenSquareBracket     , 1));
                node.Transitions.TryAdd('+',  new Pair<LexerState, int>(LexerState.SumArithmOperator     , 1));
                node.Transitions.TryAdd('-',  new Pair<LexerState, int>(LexerState.DifArithmOperator     , 1));
                node.Transitions.TryAdd('*',  new Pair<LexerState, int>(LexerState.MulArithmOperator     , 1));
                node.Transitions.TryAdd('/',  new Pair<LexerState, int>(LexerState.DivArithmOperator     , 1));
                node.Transitions.TryAdd('\'', new Pair<LexerState, int>(LexerState.CharStart             , 1));
                node.Transitions.TryAdd('>',  new Pair<LexerState, int>(LexerState.GreaterOperator       , 1));
                node.Transitions.TryAdd('%',  new Pair<LexerState, int>(LexerState.BinNumberStart        , 1));
                node.Transitions.TryAdd('&',  new Pair<LexerState, int>(LexerState.OctNumberStart        , 1));
                node.Transitions.TryAdd('$',  new Pair<LexerState, int>(LexerState.HexNumberStart        , 1));
                node.Transitions.TryAdd('=',  new Pair<LexerState, int>(LexerState.EqualOperator         , 1));
                node.Transitions.TryAdd('#',  new Pair<LexerState, int>(LexerState.SignCodeStart         , 1));
                node.Transitions.TryAdd('<',  new Pair<LexerState, int>(LexerState.LessOperator          , 1));
                node.Transitions.TryAdd(')',  new Pair<LexerState, int>(LexerState.CloseBracket          , 1));
                node.Transitions.TryAdd('(',  new Pair<LexerState, int>(LexerState.OpenBracket           , 1));
                node.Transitions.TryAdd(';',  new Pair<LexerState, int>(LexerState.SemiColon             , 1));
                node.Transitions.TryAdd(':',  new Pair<LexerState, int>(LexerState.Colon                 , 1));
                node.Transitions.TryAdd(',',  new Pair<LexerState, int>(LexerState.Comma                 , 1));
                node.Transitions.TryAdd('.',  new Pair<LexerState, int>(LexerState.Dot                   , 1));
                node.Transitions.TryAdd('^',  new Pair<LexerState, int>(LexerState.Carriage              , 1));
                node.Transitions.TryAdd('@',  new Pair<LexerState, int>(LexerState.AtSign                , 1));
                // @formatter:on
                return node;
            }

            private static Node? InitIdentStateNode(Node node)
            {
                if (node.Type != LexerState.Ident)
                {
                    return null;
                }

                foreach (char ch in IdentCharacters + DecDigits)
                {
                    node.Transitions.TryAdd(ch, new Pair<LexerState, int>(LexerState.Ident, 1));
                }

                return node;
            }

            private static Node? InitSemiColonStateNode(Node node)
            {
                return node.Type != LexerState.SemiColon ? (Node?) null : node;
            }

            private static Node? InitColonStateNode(Node node)
            {
                if (node.Type != LexerState.Colon)
                {
                    return null;
                }

                node.Transitions.TryAdd('=', new Pair<LexerState, int>(LexerState.Assign, 1));

                return node;
            }

            private static Node? InitDotStateNode(Node node)
            {
                if (node.Type != LexerState.Dot)
                {
                    return null;
                }

                node.Transitions.TryAdd('.', new Pair<LexerState, int>(LexerState.DoubleDotOperator, 1));
                node.Transitions.TryAdd(')', new Pair<LexerState, int>(LexerState.CloseSquareBracket, 1));

                return node;
            }

            private static Node? InitDoubleDotOperatorStateNode(Node node)
            {
                return node.Type != LexerState.DoubleDotOperator ? (Node?) null : node;
            }

            private static Node? InitCommaStateNode(Node node)
            {
                return node.Type != LexerState.Comma ? (Node?) null : node;
            }

            private static Node? InitSumArithOperatorStateNode(Node node)
            {
                if (node.Type != LexerState.SumArithmOperator)
                {
                    return null;
                }

                node.Transitions.TryAdd('=', new Pair<LexerState, int>(LexerState.Assign, 1));

                return node;
            }

            private static Node? InitDifArithOperatorStateNode(Node node)
            {
                if (node.Type != LexerState.DifArithmOperator)
                {
                    return null;
                }

                node.Transitions.TryAdd('=', new Pair<LexerState, int>(LexerState.Assign, 1));

                return node;
            }

            private static Node? InitMulArithOperatorStateNode(Node node)
            {
                if (node.Type != LexerState.MulArithmOperator)
                {
                    return null;
                }

                node.Transitions.TryAdd('=', new Pair<LexerState, int>(LexerState.Assign, 1));
                node.Transitions.TryAdd('*', new Pair<LexerState, int>(LexerState.PowArithmOperator, 1));
                node.Transitions[')'] = new Pair<LexerState, int>(LexerState.MultiLineCommentFinish, 1);

                return node;
            }

            private static Node? InitDivArithOperatorStateNode(Node node)
            {
                if (node.Type != LexerState.DivArithmOperator)
                {
                    return null;
                }

                node.Transitions.TryAdd('=', new Pair<LexerState, int>(LexerState.Assign, 1));
                node.Transitions.TryAdd('/', new Pair<LexerState, int>(LexerState.SingleLineComment, 1));

                return node;
            }

            private static Node? InitPowArithOperatorStateNode(Node node)
            {
                return node.Type != LexerState.PowArithmOperator ? (Node?) null : node;
            }

//            private static Node? InitDoubleNumberStartStateNode(Node node)
//            {
//                if (node.Type != LexerState.DoubleNumberStart)
//                {
//                    return null;
//                }
//
//                foreach (char digit in DecDigits)
//                {
//                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.DoubleNumber, 1));
//                }
//
//                node.Transitions.TryAdd('.', new Pair<LexerState, int>(LexerState.DecNumber, -1));
//
//                return node;
//            }

            private static Node? InitDoubleNumberStateNode(Node node)
            {
                if (node.Type != LexerState.DoubleNumber)
                {
                    return null;
                }

                foreach (char digit in DecDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.DoubleNumber, 1));
                }

                node.Transitions.TryAdd('.', new Pair<LexerState, int>(LexerState.DecNumber, -1));
                node.Transitions.TryAdd('e', new Pair<LexerState, int>(LexerState.ExpDoubleStart, 1));
                node.Transitions.TryAdd('E', new Pair<LexerState, int>(LexerState.ExpDoubleStart, 1));

                return node;
            }

            private static Node? InitLexemeEndStateNode(Node node)
            {
                return node.Type != LexerState.LexemeEnd ? (Node?) null : node;
            }

            private static Node? InitAssignStateNode(Node node)
            {
                return node.Type != LexerState.Assign ? (Node?) null : node;
            }

            private static Node? InitCharStringStartStateNode(Node node)
            {
                if (node.Type != LexerState.CharStringStart)
                {
                    return null;
                }

                node.Transitions.TryAdd('\'', new Pair<LexerState, int>(LexerState.CharStringFinish, 1));

                return node;
            }

            private static Node? InitCharStringFinishStateNode(Node node)
            {
                if (node.Type != LexerState.CharStringFinish)
                {
                    return null;
                }

                node.Transitions.TryAdd('\'', new Pair<LexerState, int>(LexerState.CharStringStart, 1));
                node.Transitions.TryAdd('#', new Pair<LexerState, int>(LexerState.ControlStringStart, 1));

                return node;
            }

            private static Node? InitOpenBracketStateNode(Node node)
            {
                if (node.Type != LexerState.OpenBracket)
                {
                    return null;
                }

                node.Transitions.TryAdd('.', new Pair<LexerState, int>(LexerState.OpenSquareBracket, 1));
                node.Transitions.TryAdd('*', new Pair<LexerState, int>(LexerState.MultiLineCommentStart, 1));

                return node;
            }

            private static Node? InitCloseBracketStateNode(Node node)
            {
                return node.Type != LexerState.CloseBracket ? (Node?) null : node;
            }

            private static Node? InitOpenSquareBracketStateNode(Node node)
            {
                return node.Type != LexerState.OpenSquareBracket ? (Node?) null : node;
            }

            private static Node? InitCloseSquareBracketStateNode(Node node)
            {
                return node.Type != LexerState.CloseSquareBracket ? (Node?) null : node;
            }

            private static Node? InitMultiLineCommentStartStateNode(Node node)
            {
                if (node.Type != LexerState.MultiLineCommentStart)
                {
                    return null;
                }

                node.Transitions.TryAdd('}', new Pair<LexerState, int>(LexerState.MultiLineCommentFinish, 1));

                return node;
            }

            private static Node? InitMultiLineCommentFinishStateNode(Node node)
            {
                return node.Type != LexerState.MultiLineCommentFinish ? (Node?) null : node;
            }

            private static Node? InitSingleLineCommentStateNode(Node node)
            {
                return node.Type != LexerState.SingleLineComment ? (Node?) null : node;
            }

            private static Node? InitInvalidSignStateNode(Node node)
            {
                return node.Type != LexerState.InvalidSign ? (Node?) null : node;
            }

            private static Node InitBinNumber(Node node)
            {
                string binDigits = "01";
                string notBinDigits = "23456789" + IdentCharacters + '%' + '&' + '$';

                foreach (var ch in binDigits)
                {
                    node.Transitions.TryAdd(ch, new Pair<LexerState, int>(LexerState.BinNumber, 1));
                }

                foreach (var ch in notBinDigits)
                {
                    node.Transitions.TryAdd(ch, new Pair<LexerState, int>(LexerState.InvalidSign, 1));
                }

                return node;
            }

            private static Node? InitBinNumberStartStateNode(Node node)
            {
                return node.Type != LexerState.BinNumberStart ? (Node?) null : InitBinNumber(node);
            }

            private static Node? InitBinNumberStateNode(Node node)
            {
                return node.Type != LexerState.BinNumber ? (Node?) null : InitBinNumber(node);
            }

            private static Node InitOctNumber(Node node)
            {
                string octDigits = "01234567";
                string notOctDigits = "89" + IdentCharacters + '%' + '&' + '$';

                foreach (var ch in octDigits)
                {
                    node.Transitions.TryAdd(ch, new Pair<LexerState, int>(LexerState.OctNumber, 1));
                }

                foreach (var ch in notOctDigits)
                {
                    node.Transitions.TryAdd(ch, new Pair<LexerState, int>(LexerState.InvalidSign, 1));
                }

                return node;
            }

            private static Node? InitOctNumberStartStateNode(Node node)
            {
                if (node.Type != LexerState.OctNumberStart)
                {
                    return null;
                }

                foreach (var ch in IdentCharacters)
                {
                    node.Transitions.TryAdd(ch, new Pair<LexerState, int>(LexerState.Ident, 1));
                }

                return InitOctNumber(node);
            }

            private static Node? InitOctNumberStateNode(Node node)
            {
                return node.Type != LexerState.OctNumber ? (Node?) null : InitOctNumber(node);
            }

            private static Node? InitDecNumberStateNode(Node node)
            {
                if (node.Type != LexerState.DecNumber)
                {
                    return null;
                }

                string decDigits = "0123456789";
                string notDecDigits = IdentCharacters + '%' + '&' + '$';

                foreach (char digit in decDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.DecNumber, 1));
                }

                node.Transitions.TryAdd('e', new Pair<LexerState, int>(LexerState.ExpDoubleStart, 1));
                node.Transitions.TryAdd('E', new Pair<LexerState, int>(LexerState.ExpDoubleStart, 1));

                foreach (var ch in notDecDigits)
                {
                    node.Transitions.TryAdd(ch, new Pair<LexerState, int>(LexerState.InvalidSign, 1));
                }

                node.Transitions.TryAdd('.', new Pair<LexerState, int>(LexerState.DoubleNumber, 1));

                return node;
            }

            private static Node InitHexNumber(Node node)
            {
                string hexDigits = "0123456789ABCDEFabcdef";
                string notHexDigits = "_GHIJKLMNOPQRSTUVWXYZghijklmnopqrstuvwxyz" + '%' + '&' + '$';

                foreach (var ch in hexDigits)
                {
                    node.Transitions.TryAdd(ch, new Pair<LexerState, int>(LexerState.HexNumber, 1));
                }

                foreach (var ch in notHexDigits)
                {
                    node.Transitions.TryAdd(ch, new Pair<LexerState, int>(LexerState.InvalidSign, 1));
                }

                return node;
            }

            private static Node? InitHexNumberStartStateNode(Node node)
            {
                return node.Type != LexerState.HexNumberStart ? (Node?) null : InitHexNumber(node);
            }

            private static Node? InitHexNumberStateNode(Node node)
            {
                return node.Type != LexerState.HexNumber ? (Node?) null : InitHexNumber(node);
            }

            private static Node InitSignCode(Node node)
            {
                string decDigits = "0123456789";
                foreach (char digit in decDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.SignCodeFinish, 1));
                }

                return node;
            }

            private static Node? InitSignCodeStartStateNode(Node node)
            {
                return node.Type != LexerState.SignCodeStart ? (Node?) null : InitSignCode(node);
            }

            private static Node? InitSignCodeFinishStateNode(Node node)
            {
                if (node.Type != LexerState.SignCodeFinish)
                {
                    return null;
                }

                node.Transitions.TryAdd('\'', new Pair<LexerState, int>(LexerState.CharStringStart, 1));
                node.Transitions.TryAdd('#', new Pair<LexerState, int>(LexerState.ControlStringStart, 1));
                return InitSignCode(node);
            }

            private static Node? InitExpDoubleStartStateNode(Node node)
            {
                if (node.Type != LexerState.ExpDoubleStart)
                {
                    return null;
                }

                string decDigits = "0123456789";
                foreach (char digit in decDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.ExpDouble, 1));
                }

                node.Transitions.TryAdd('-', new Pair<LexerState, int>(LexerState.ExpDoubleMinus, 1));

                return node;
            }

            private static Node? InitExpDoubleMinusStateNode(Node node)
            {
                if (node.Type != LexerState.ExpDoubleMinus)
                {
                    return null;
                }

                string decDigits = "0123456789";
                foreach (char digit in decDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.ExpDouble, 1));
                }

                return node;
            }

            private static Node? InitExpDoubleStateNode(Node node)
            {
                if (node.Type != LexerState.ExpDouble)
                {
                    return null;
                }

                string decDigits = "0123456789";
                foreach (char digit in decDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.ExpDouble, 1));
                }

                return node;
            }
        }
    }
}