using System;
using System.Collections.Generic;

namespace FEFUPascalCompiler.Lexer
{
    internal partial class LexerDfa
    {
        private static readonly Dictionary<LexerState, bool> TerminalStates = new Dictionary<LexerState, bool>
        {
            {LexerState.Start, true},
            {LexerState.Ident, true},
            {LexerState.SemiColon, true},
            {LexerState.Colon, true},
            {LexerState.Dot, true},
            {LexerState.DoubleDotOperator, true},
            {LexerState.Comma, true},
            {LexerState.SumArithmOperator, true},
            {LexerState.DifArithmOperator, true},
            {LexerState.MulArithmOperator, true},
            {LexerState.DivArithmOperator, true},
            {LexerState.PowArithmOperator, true},
            {LexerState.Ampersand, false},
            {LexerState.IntNumber, true},
            {LexerState.ConstNumberStart, false},
            {LexerState.ConstNumber, true},
            {LexerState.DoubleNumberStart, false},
            {LexerState.DoubleNumber, true},
            {LexerState.LexemeEnd, true},
            {LexerState.Assign, true},
            {LexerState.StringConstStart, false},
            {LexerState.StringConstFinish, true},
            {LexerState.OpenBracket, true},
            {LexerState.CloseBracket, true},
            {LexerState.OpenSquareBracket, true},
            {LexerState.CloseSquareBracket, true},
            {LexerState.MultiLineCommentStart, false},
            {LexerState.MultiLineCommentFinish, true},
            {LexerState.SingleLineComment, true},
        };

        private struct Node
        {
            internal readonly LexerState Type;
            internal readonly bool Terminal;
            internal readonly Dictionary<char, Pair<LexerState, int>> Transitions;

            public Node(LexerState type, bool terminal, Dictionary<char, Pair<LexerState, int>> transitions)
            {
                Type = type;
                Terminal = terminal;
                Transitions = transitions;
            }
        }

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
                    nodes.TryAdd(state, new Node(state, TerminalStates[state],
                        new Dictionary<char, Pair<LexerState, int>>()
//                        {
//                        {'\n', new Pair<LexerState, int>(LexerState.LexemeEnd, 1)},
//                        {' ', new Pair<LexerState, int>(LexerState.LexemeEnd, 1)},
//                        {'\uffff', new Pair<LexerState, int>(LexerState.LexemeEnd, 1)},
//                        }
                    ));
                    var tmp = InitFunctions[state](nodes[state]);
                    nodes[state] = tmp ?? nodes[state];
                }

                return nodes;
            }

            private static readonly Dictionary<LexerState, Func<Node, Node?>> InitFunctions =
                new Dictionary<LexerState, Func<Node, Node?>>
                {
                    {LexerState.Start, InitStartStateNode},
                    {LexerState.Ident, InitIdentStateNode},
                    {LexerState.Ampersand, InitAmpersandStateNode},
                    {LexerState.IntNumber, InitIntNumberStateNode},
                    {LexerState.ConstNumberStart, InitConstNumberStartStateNode},
                    {LexerState.ConstNumber, InitConstNumberStateNode},
                    {LexerState.SemiColon, InitSemiColonStateNode},
                    {LexerState.Colon, InitColonStateNode},
                    {LexerState.Dot, InitDotStateNode},
                    {LexerState.DoubleDotOperator, InitDoubleDotStateNode},
                    {LexerState.Comma, InitCommaStateNode},
                    {LexerState.SumArithmOperator, InitSumArithOperatorStateNode},
                    {LexerState.DifArithmOperator, InitDifArithOperatorStateNode},
                    {LexerState.MulArithmOperator, InitMulArithOperatorStateNode},
                    {LexerState.DivArithmOperator, InitDivArithOperatorStateNode},
                    {LexerState.PowArithmOperator, InitPowArithOperatorStateNode},
                    {LexerState.DoubleNumberStart, InitDoubleNumberStartStateNode},
                    {LexerState.DoubleNumber, InitDoubleNumberStateMode},
                    {LexerState.LexemeEnd, InitLexemeEndStateNode},
                    {LexerState.Assign, InitAssignStateNode},
                    {LexerState.StringConstStart, InitStringConstStartStateNode},
                    {LexerState.StringConstFinish, InitStringConstFinishStateNode},
                    {LexerState.OpenBracket, InitOpenBracketStateNode},
                    {LexerState.CloseBracket, InitCloseBracketStateNode},
                    {LexerState.OpenSquareBracket, InitOpenSquareBracketStateNode},
                    {LexerState.CloseSquareBracket, InitCloseSquareBracketStateNode},
                    {LexerState.MultiLineCommentStart, InitMultiLineCommentStartStateNode},
                    {LexerState.MultiLineCommentFinish, InitMultiLineCommentFinishStateNode},
                    {LexerState.SingleLineComment, InitSingleLineCommentStateNode},
                };

            private static readonly string IdentCharacters = "_ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
//            private static readonly string Separators = ":;.,= +-*/$%&()[]{}'\"";
            private static readonly string DecDigits = "0123456789";
            private static readonly string AllDigits = "0123456789ABCDEFabcdef";

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
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.IntNumber, 1));
                }

                node.Transitions.TryAdd('+', new Pair<LexerState, int>(LexerState.SumArithmOperator, 1));
                node.Transitions.TryAdd('-', new Pair<LexerState, int>(LexerState.DifArithmOperator, 1));
                node.Transitions.TryAdd('*', new Pair<LexerState, int>(LexerState.MulArithmOperator, 1));
                node.Transitions.TryAdd('/', new Pair<LexerState, int>(LexerState.DivArithmOperator, 1));
                node.Transitions.TryAdd('\'', new Pair<LexerState, int>(LexerState.StringConstStart, 1));
                node.Transitions.TryAdd('%', new Pair<LexerState, int>(LexerState.ConstNumberStart, 1));
                node.Transitions.TryAdd('$', new Pair<LexerState, int>(LexerState.ConstNumberStart, 1));
                node.Transitions.TryAdd(';', new Pair<LexerState, int>(LexerState.SemiColon, 1));
                node.Transitions.TryAdd('&', new Pair<LexerState, int>(LexerState.Ampersand, 1));
                node.Transitions.TryAdd('=', new Pair<LexerState, int>(LexerState.Assign, 1));
                node.Transitions.TryAdd(':', new Pair<LexerState, int>(LexerState.Colon, 1));
                node.Transitions.TryAdd(',', new Pair<LexerState, int>(LexerState.Comma, 1));
                node.Transitions.TryAdd('.', new Pair<LexerState, int>(LexerState.Dot, 1));
                node.Transitions.TryAdd('(', new Pair<LexerState, int>(LexerState.OpenBracket, 1));
                node.Transitions.TryAdd(')', new Pair<LexerState, int>(LexerState.CloseBracket, 1));
                node.Transitions.TryAdd('[', new Pair<LexerState, int>(LexerState.OpenSquareBracket, 1));
                node.Transitions.TryAdd(']', new Pair<LexerState, int>(LexerState.CloseSquareBracket, 1));
                node.Transitions.TryAdd('{', new Pair<LexerState, int>(LexerState.MultiLineCommentStart, 1));
                node.Transitions.TryAdd('}', new Pair<LexerState, int>(LexerState.MultiLineCommentFinish, 1));

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

            private static Node? InitAmpersandStateNode(Node node)
            {
                if (node.Type != LexerState.Ampersand)
                {
                    return null;
                }

                foreach (char letter in IdentCharacters)
                {
                    node.Transitions.TryAdd(letter, new Pair<LexerState, int>(LexerState.Ident, 1));
                }

                foreach (char digit in "01234567")
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.ConstNumber, 1));
                }

                return node;
            }

            private static Node? InitIntNumberStateNode(Node node)
            {
                if (node.Type != LexerState.IntNumber)
                {
                    return null;
                }

                foreach (char digit in DecDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.IntNumber, 1));
                }

                return node;
            }

            private static Node? InitConstNumberStartStateNode(Node node)
            {
                if (node.Type != LexerState.ConstNumberStart)
                {
                    return null;
                }

                foreach (char digit in AllDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.ConstNumber, 1));
                }

                return node;
            }

            private static Node? InitConstNumberStateNode(Node node)
            {
                if (node.Type != LexerState.ConstNumber)
                {
                    return null;
                }

                foreach (char digit in AllDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.ConstNumber, 1));
                }

                return node;
            }

            private static Node? InitSemiColonStateNode(Node node)
            {
                return node.Type != LexerState.SemiColon ? (Node?) null : node;
            }

            private static Node? InitColonStateNode(Node node)
            {
                return node.Type != LexerState.Colon ? (Node?) null : node;
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

            private static Node? InitDoubleDotStateNode(Node node)
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

            private static Node? InitDoubleNumberStartStateNode(Node node)
            {
                if (node.Type != LexerState.DoubleNumberStart)
                {
                    return null;
                }

                foreach (char digit in DecDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.IntNumber, 1));
                }

                node.Transitions.TryAdd('.', new Pair<LexerState, int>(LexerState.IntNumber, -1));

                return node;
            }

            private static Node? InitDoubleNumberStateMode(Node node)
            {
                if (node.Type != LexerState.DoubleNumber)
                {
                    return null;
                }

                foreach (char digit in DecDigits)
                {
                    node.Transitions.TryAdd(digit, new Pair<LexerState, int>(LexerState.IntNumber, 1));
                }

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

            private static Node? InitStringConstStartStateNode(Node node)
            {
                if (node.Type != LexerState.StringConstStart)
                {
                    return null;
                }

                node.Transitions.TryAdd('\'', new Pair<LexerState, int>(LexerState.StringConstFinish, 1));

                return node;
            }

            private static Node? InitStringConstFinishStateNode(Node node)
            {
                return node.Type != LexerState.StringConstFinish ? (Node?) null : node;
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
                if (node.Type != LexerState.SingleLineComment)
                {
                    return null;
                }

                return node;
            }
        }
    }
}