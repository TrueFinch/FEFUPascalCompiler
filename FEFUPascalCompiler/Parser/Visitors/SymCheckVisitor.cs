using System;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.Visitors
{
    public class SymCheckVisitor : IAstVisitor<bool>
    {
        public SymCheckVisitor(SymbolStack symbolTableStack, TypeChecker typeChecker)
        {
            _symbolTableStack = symbolTableStack;
            _typeChecker = typeChecker;
        }

        private readonly SymbolStack _symbolTableStack;
        private readonly TypeChecker _typeChecker;


        public bool Visit(ConstIntegerLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = _symbolTableStack.SymInteger;
            return true;
        }

        public bool Visit(ConstFloatLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = _symbolTableStack.SymFloat;
            return true;
        }

        public bool Visit(BinOperator node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ConstCharLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = _symbolTableStack.SymChar;
            return true;
        }

        public bool Visit(ConstStringLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = _symbolTableStack.SymString;
            return true;
        }

        public bool Visit(Nil node)
        {
            if (node.SymType != null) return true;
            
            node.IsLValue = false;
            node.SymType = _symbolTableStack.SymNil;
            return true;
        }

        public bool Visit(CompoundStatement node)
        {
            foreach (var nodeStatement in node.Statements)
            {
                nodeStatement.Accept(this);
            }

            return true;
        }

        public bool Visit(EmptyStatement node)
        {
            return true;
        }

        public bool Visit(AssignStatement node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);

            if (!node.Left.IsLValue)
            {
                throw new Exception(string.Format(
                    "{0}, {1} : syntax error, left part of assignment '{2}' is not lvalue",
                    node.Left.Token.Line, node.Left.Token.Column, node.Left.ToString()));
            }

            Expression nodeLeft = node.Left;
            Expression nodeRight = node.Right;
            _typeChecker.Assignment(ref nodeLeft, ref nodeRight, node.Token);

            return true;
        }

        public bool Visit(Program node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(MainBlock node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ConstDeclsPart node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(TypeDeclsPart node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(TypeDecl node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ConstDecl node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(VarDeclsPart node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(SimpleVarDecl node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(InitVarDecl node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ProcFuncDeclsPart node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ProcDecl node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ProcHeader node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(FuncDecl node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(FuncHeader node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(SubroutineBlock node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(Forward node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(IfStatement node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(WhileStatement node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ForStatement node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(SimpleType node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ArrayTypeAstNode node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(IndexRangeAstNode node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(RecordTypeAstNode node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(FieldSection node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(PointerType node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ProcSignature node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(FuncSignature node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ConformantArray node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ForRange node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(Cast node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(BooleanLiteral node)
        {
            
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = _symbolTableStack.SymBool;
            return true;
        }

        public bool Visit(Ident node)
        {
            if (node.SymType != null) return true;

            var sym = _symbolTableStack.FindIdent(node.ToString());
            if (sym == null)
                throw new Exception(string.Format(
                    "{0}, {1} : syntax error, identifier '{2}' is not defined",
                    node.Token.Line, node.Token.Column, node.Token.Lexeme));
            node.SymVar = sym;
            node.SymType = sym.VarSymType;
            node.IsLValue = true;

            return true;
        }

        public bool Visit(ArrayAccess node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(DereferenceOperator node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(FunctionCall node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(FormalParamSection node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(RecordAccess node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(UnaryOperator node)
        {
            node.Expr.Accept(this);
            
            switch (node.Token.Type)
            {
                case TokenType.SumOperator:
                case TokenType.DifOperator:
                {
                    if (!(node.Expr.SymType.Equals(_symbolTableStack.SymInteger)
                         || node.Expr.SymType.Equals(_symbolTableStack.SymFloat)))
                    {
                        throw new Exception(string.Format(
                            "{0}, {1} : syntax error, integer or float expected, but '{2}' found",
                            node.Expr.Token.Line, node.Expr.Token.Column, node.Expr.Token.Lexeme));
                    }
                    break;
                }

                case TokenType.AtSign:
                {
                    if (!node.Expr.IsLValue)
                    {
                        throw new Exception(string.Format(
                            "{0}, {1} : syntax error, expression '{2}' is not lvalue",
                            node.Expr.Token.Line, node.Expr.Token.Column, node.Expr.ToString()));
                    }

                    break;
                }
                case TokenType.Not:
                {
                    if (!node.SymType.Equals(_symbolTableStack.SymBool))
                    {
                        throw new Exception(string.Format(
                            "{0}, {1} : syntax error, boolean expected, but '{2}' found",
                            node.Expr.Token.Line, node.Expr.Token.Column, node.Expr.ToString()));
                    }

                    break;
                }
                default:
                {
                    throw new Exception(string.Format(
                        "{0}, {1} : syntax error, unrecognized unary operator '{2}'",
                        node.Token.Line, node.Token.Column, node.ToString()));
                }
            }

            return true;
        }

        public bool Visit(MultiplyingOperator node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(AdditiveOperator node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ComparingOperator node)
        {
            throw new NotImplementedException();
        }

//        private bool IsLvalue(Expression expr)
//        {
//            exprexpr is Ident || expr is ArrayAccess || expr is RecordAccess;
//        }
    }
}