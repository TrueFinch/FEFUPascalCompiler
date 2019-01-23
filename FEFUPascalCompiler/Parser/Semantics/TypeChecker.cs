using System;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.Semantics
{
    public class TypeChecker
    {
        private readonly SymbolStack _symStack;

        public TypeChecker(SymbolStack stack)
        {
            _symStack = stack;
        }

        //check if 
        public void Assignment(ref Expression left, ref Expression right, Token oper)
        {
            if (left.SymType.Equals(_symStack.SymFloat) && right.SymType.Equals(_symStack.SymInteger))
            {
                right = new Cast(right) {SymType = _symStack.SymFloat};
                return;
            }
            
            switch (oper.Type)
            {
                case TokenType.SimpleAssignOperator:
                {
                    if (left.SymType.Equals(right.SymType))
                    {
                        return;
                    }
                    
                    throw new Exception(string.Format(
                        "{0}, {1} : syntax error, Incompatible types: got '{2}' expected '{3}'",
                        oper.Line, oper.Column, right.SymType.ToString(), left.SymType.ToString()));
                }
                case TokenType.SumAssignOperator:
                case TokenType.DifAssignOperator:
                case TokenType.MulAssignOperator:
                case TokenType.DivAssignOperator:
                {
                    if (left.SymType.Equals(_symStack.SymInteger) && right.SymType.Equals(_symStack.SymInteger))
                    {
                        return;
                    }
                    
                    throw new Exception(string.Format(
                        "{0}, {1} : syntax error, Incompatible types: got '{2}' expected '{3}'",
                        oper.Line, oper.Column, right.SymType.ToString(), left.SymType.ToString()));
                }
                default:
                {
                    throw new Exception(string.Format(
                        "{0}, {1} : syntax error, unknown assignment operator '{2}'",
                        oper.Line, oper.Column, oper.Lexeme));
                }
            }
        }
    }
}