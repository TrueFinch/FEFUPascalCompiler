using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.Visitors
{
    public class SymCheckVisitor : IAstVisitor<bool>
    {
        public SymCheckVisitor(SymbolStack symStack, TypeChecker typeChecker)
        {
            _symStack = symStack;
            _typeChecker = typeChecker;
        }

        private readonly SymbolStack _symStack;
        private readonly TypeChecker _typeChecker;

        private bool _inLastNamespace = false;

        public bool Visit(ConstIntegerLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = _symStack.SymInt;
            return true;
        }

        public bool Visit(ConstFloatLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = _symStack.SymFloat;
            return true;
        }

        public bool Visit(ConstCharLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = _symStack.SymChar;
            return true;
        }

        public bool Visit(ConstStringLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = _symStack.SymString;
            return true;
        }

        public bool Visit(Nil node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = _symStack.SymNil;
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
//            node.MainBlock.Accept(this);
//            node.
            throw new NotImplementedException();
        }

        public bool Visit(MainBlock node)
        {
            foreach (var declarationsPart in node.DeclsParts)
            {
                declarationsPart.Accept(this);
            }

            node.MainCompound.Accept(this);
            return true;
        }

        public bool Visit(ConstDeclsPart node)
        {
            foreach (var nodeDecl in node.Decls)
            {
                nodeDecl.Accept(this);
            }

            return true;
        }

        public bool Visit(TypeDeclsPart node)
        {
            foreach (var nodeDecl in node.Decls)
            {
                nodeDecl.Accept(this);
            }

            return true;
        }

        public bool Visit(TypeDecl node)
        {
            switch (node.IdentType)
            {
                case SimpleTypeNode simpleTypeNode:
                {
                    var symType = _symStack.CheckTypeDeclared(simpleTypeNode.Token);

                    _symStack.AddAlias(node.Ident.ToString(), symType);
                    break;
                }
                case ArrayTypeNode arrayTypeNode:
                {
                    var indexRanges = new List<IndexRange<int, int>>();
                    foreach (var indexRange in arrayTypeNode.IndexRanges)
                    {
                        indexRange.LeftBound.Accept(this);
                        indexRange.RightBound.Accept(this);

                        if (!(indexRange.LeftBound.Token is IntegerNumberToken leftToken))
                        {
                            throw new Exception(string.Format("{0}, {1} : range bounds is not integer '{2}'",
                                indexRange.LeftBound.Token.Line, indexRange.LeftBound.Token.Column,
                                indexRange.LeftBound.Token.Lexeme));
                        }

                        if (!(indexRange.RightBound.Token is IntegerNumberToken rightToken))
                        {
                            throw new Exception(string.Format("{0}, {1} : range bounds is not integer '{2}'",
                                indexRange.RightBound.Token.Line, indexRange.RightBound.Token.Column,
                                indexRange.RightBound.Token.Lexeme));
                        }

                        indexRanges.Add(new IndexRange<int, int>(leftToken.NumberValue, rightToken.NumberValue));
                    }


                    var arrayElemType = _symStack.CheckTypeDeclared(arrayTypeNode.TypeOfArray.Token);

                    _symStack.CheckDuplicateIdentifier(node.Ident.Token);

                    _symStack.AddType(node.Ident.ToString(),
                        new SymArrayType(indexRanges, arrayElemType, node.Ident.ToString()));

                    break;
                }
                case RecordTypeNode recordTypeNode:
                {
                    // this will be record table for fields
                    _symStack.Push();
                    foreach (var field in recordTypeNode.FieldsList)
                    {
                        var fieldType = _symStack.CheckTypeDeclared(field.IdentsType.Token);
                        foreach (var fieldIdent in field.Idents)
                        {
                            _symStack.CheckDuplicateIdentifier(fieldIdent.Token);
                            _symStack.AddVariable(true, fieldIdent.ToString(), fieldType);
                        }
                    }

                    _symStack.AddType(node.Ident.ToString(), new SymRecordType(_symStack.Pop()));
                    break;
                }
                case PointerTypeNode pointerTypeNode:
                {
                    var ptrType = _symStack.CheckTypeDeclared(pointerTypeNode.SimpleType.Token);

                    _symStack.AddAlias(node.Ident.ToString(), ptrType);
                    break;
                }
            }

            return true;
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

//        public bool Visit(ProcFuncDeclsPart node)
//        {
//            throw new NotImplementedException();
//        }

        public bool Visit(CallableDeclNode node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(CallableHeader node)
        {
            throw new NotImplementedException();
        }

//        public bool Visit(ProcDecl node)
//        {
//            throw new NotImplementedException();
//        }
//
//        public bool Visit(ProcHeader node)
//        {
//            throw new NotImplementedException();
//        }
//
//        public bool Visit(FuncDecl node)
//        {
//            throw new NotImplementedException();
//        }
//
//        public bool Visit(FuncHeader node)
//        {
//            throw new NotImplementedException();
//        }

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

        public bool Visit(SimpleTypeNode node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ArrayTypeNode node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(IndexRangeNode node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(RecordTypeNode node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(FieldSectionNode node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(PointerTypeNode node)
        {
            throw new NotImplementedException();
        }

//        public bool Visit(ProcSignature node)
//        {
//            throw new NotImplementedException();
//        }
//
//        public bool Visit(FuncSignature node)
//        {
//            throw new NotImplementedException();
//        }

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

        public bool Visit(ConstBooleanLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = _symStack.SymBool;
            return true;
        }

        public bool Visit(Ident node)
        {
            if (node.SymType != null) return true;

            var sym = _inLastNamespace
                ? _symStack.FindIdentInScope(node.ToString())
                : _symStack.FindIdent(node.ToString());

            if (sym == null)
                if (_inLastNamespace)
                    throw new Exception(string.Format(
                        "{0}, {1} : syntax error, field '{2}' is not defined in record",
                        node.Token.Line, node.Token.Column, node.Token.Lexeme));
                else
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
            if (node.SymType != null) return true;

            node.ArrayIdent.Accept(this);
            foreach (var expression in node.AccessExpr)
            {
                expression.Accept(this);
            }

            node.SymType = (node.ArrayIdent.SymType as SymArrayType).ElementSymType;
            node.IsLValue = false;
            return true;
        }

        public bool Visit(DereferenceOperator node)
        {
            if (node.SymType != null) return true;

            node.Expr.Accept(this);

            if (!node.Expr.IsLValue)
            {
                throw new Exception(string.Format(
                    "{0}, {1} : syntax error, expression '{2}' is not lvalue",
                    node.Expr.Token.Line, node.Expr.Token.Column, node.Expr.ToString()));
            }


            return true;
        }

        public bool Visit(FunctionCall node)
        {
            //TODO: rewrite FunctionCall and symbol types of function and procedure
//            if (node.SymType != null) return true;
//            
//            node.FuncIdent.Accept(this);
//            
//            if (node.FuncIdent.SymType)
//            _symStack.Push((node.FuncIdent.SymType as FunctionSymbol).Table);
//            _inLastNamespace = true;
//            node.FieldToAccess.Accept(this);
//            _inLastNamespace = false;
//
//            node.SymType = node.FieldToAccess.SymType;
//            node.IsLValue = true;
//            return true;
            throw new NotImplementedException();
        }

        public bool Visit(FormalParamSection node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(RecordAccess node)
        {
            if (node.SymType != null) return true;

            node.RecordIdent.Accept(this);
            _symStack.Push((node.RecordIdent.SymType as SymRecordType).Table);
            _inLastNamespace = true;
            node.FieldToAccess.Accept(this);
            _inLastNamespace = false;
            _symStack.Pop();

            node.SymType = node.FieldToAccess.SymType;
            node.IsLValue = true;
            return true;
        }

        public bool Visit(UnaryOperator node)
        {
            if (node.SymType != null) return true;

            node.Expr.Accept(this);

            switch (node.Token.Type)
            {
                case TokenType.SumOperator:
                case TokenType.DifOperator:
                {
                    if (!(node.Expr.SymType.Equals(_symStack.SymInt)
                          || node.Expr.SymType.Equals(_symStack.SymFloat)))
                    {
                        throw new Exception(string.Format(
                            "{0}, {1} : syntax error, 'integer' or 'float' expected, but '{2}' found",
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
                    if (!node.Expr.SymType.Equals(_symStack.SymBool))
                    {
                        throw new Exception(string.Format(
                            "{0}, {1} : syntax error, boolean expected, but '{2}' found",
                            node.Expr.Token.Line, node.Expr.Token.Column, node.Expr.ToString()));
                    }

                    node.SymType = node.Expr.SymType;
                    break;
                }
                default:
                {
                    throw new Exception(string.Format(
                        "{0}, {1} : syntax error, unrecognized unary operator '{2}'",
                        node.Token.Line, node.Token.Column, node.ToString()));
                }
            }

            node.IsLValue = false;
            return true;
        }

        public bool Visit(MultiplyingOperator node)
        {
            if (node.SymType != null) return true;

            node.Left.Accept(this);
            node.Right.Accept(this);

            // we can use multiplication operators only with integer ('div', 'mod', 'shr', 'shl') and float ('*', '/')
            // and boolean ('and')
            if (!(node.Left.SymType.Equals(_symStack.SymBool) && node.Right.SymType.Equals(_symStack.SymBool)
                  || (node.Left.SymType.Equals(_symStack.SymInt) || node.Left.SymType.Equals(_symStack.SymFloat))
                  && (node.Right.SymType.Equals(_symStack.SymInt) || node.Right.SymType.Equals(_symStack.SymFloat))))
            {
                throw new Exception(string.Format("{0}, {1} : syntax error, incompatible types: '{2}' {3} '{4}'",
                    node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value, node.Right.SymType));
            }

            //check if operator - and and operands are boolean type
            //give us ability to avoid this check in every case of switch
            if (node.Left.SymType.Equals(_symStack.SymBool) && node.Right.SymType.Equals(_symStack.SymBool) &&
                node.Token.Type != TokenType.And)
            {
                throw new Exception(string.Format(
                    "{0}, {1} : syntax error, incompatible types: '{2}' {3} '{4}'",
                    node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value,
                    node.Right.SymType));
            }

            switch (node.Token.Type)
            {
                case TokenType.MulOperator:
                {
                    node.SymType = node.Left.SymType.Equals(_symStack.SymInt) &&
                                   node.Right.SymType.Equals(_symStack.SymInt)
                        ? _symStack.SymInt
                        : _symStack.SymFloat;
                    break;
                }
                case TokenType.DivOperator:
                {
                    node.SymType = _symStack.SymFloat;
                    break;
                }
                case TokenType.Div:
                case TokenType.Mod:
                case TokenType.Shr:
                case TokenType.Shl:
                {
                    if (!node.Left.SymType.Equals(_symStack.SymInt) || !node.Right.SymType.Equals(_symStack.SymInt))
                    {
                        throw new Exception(string.Format(
                            "{0}, {1} : syntax error, incompatible types: '{2}' {3} '{4}'",
                            node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value,
                            node.Right.SymType));
                    }

                    node.SymType = _symStack.SymInt;
                    break;
                }
                case TokenType.And:
                {
                    if (!node.Left.SymType.Equals(_symStack.SymBool) || !node.Right.SymType.Equals(_symStack.SymBool))
                    {
                        throw new Exception(string.Format(
                            "{0}, {1} : syntax error, incompatible types: '{2}' {3} '{4}'",
                            node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value,
                            node.Right.SymType));
                    }

                    node.SymType = _symStack.SymBool;
                    break;
                }
                default:
                {
                    throw new Exception(string.Format(
                        "{0}, {1} : syntax error, unhandled multiplying operator '{2}'",
                        node.Token.Line, node.Token.Column, node.Token.Value));
                }
            }

            node.IsLValue = false;
            return true;
        }

        public bool Visit(AdditiveOperator node)
        {
            if (node.SymType != null) return true;

            node.Left.Accept(this);
            node.Right.Accept(this);

            // we can use additive operators only with integer and float ('+', '-') and boolean ('or', 'xor')
            if (!(node.Left.SymType.Equals(_symStack.SymBool) && node.Right.SymType.Equals(_symStack.SymBool)
                  || (node.Left.SymType.Equals(_symStack.SymInt) || node.Left.SymType.Equals(_symStack.SymFloat))
                  && (node.Right.SymType.Equals(_symStack.SymInt) || node.Right.SymType.Equals(_symStack.SymFloat))))
            {
                throw new Exception(string.Format("{0}, {1} : syntax error, incompatible types: '{2}' {3} '{4}'",
                    node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value, node.Right.SymType));
            }

            switch (node.Token.Type)
            {
                case TokenType.SumOperator:
                case TokenType.DifOperator:
                {
                    if (node.Left.SymType.Equals(_symStack.SymBool) || node.Right.SymType.Equals(_symStack.SymBool))
                    {
                        throw new Exception(string.Format(
                            "{0}, {1} : syntax error, incompatible types: '{2}' {3} '{4}'",
                            node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value,
                            node.Right.SymType));
                    }

                    node.SymType = node.Left.SymType.Equals(_symStack.SymInt) &&
                                   node.Right.SymType.Equals(_symStack.SymInt)
                        ? _symStack.SymInt
                        : _symStack.SymFloat;
                    break;
                }
                case TokenType.Or:
                case TokenType.Xor:
                {
                    if (!node.Left.SymType.Equals(_symStack.SymBool) || !node.Right.SymType.Equals(_symStack.SymBool))
                    {
                        throw new Exception(string.Format(
                            "{0}, {1} : syntax error, incompatible types: '{2}' {3} '{4}'",
                            node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value,
                            node.Right.SymType));
                    }

                    node.SymType = _symStack.SymBool;
                    break;
                }
                default:
                {
                    throw new Exception(string.Format(
                        "{0}, {1} : syntax error, unhandled additive operator '{2}'",
                        node.Token.Line, node.Token.Column, node.Token.Value));
                }
            }

            node.IsLValue = false;
            return true;
        }

        public bool Visit(ComparingOperator node)
        {
            if (node.SymType != null) return true;

            node.Left.Accept(this);
            node.Right.Accept(this);

            // we can use additive operators only with integer and float (all operators) and char and boolean ('=', '<>')
            if (!(node.Left.SymType.Equals(_symStack.SymBool) && node.Token.Type == TokenType.EqualOperator
                  || node.Left.SymType.Equals(_symStack.SymBool) && node.Token.Type == TokenType.NotEqualOperator
                  || node.Left.SymType.Equals(_symStack.SymChar) && node.Token.Type == TokenType.EqualOperator
                  || node.Left.SymType.Equals(_symStack.SymChar) && node.Token.Type == TokenType.NotEqualOperator
                  || (node.Left.SymType.Equals(_symStack.SymInt) || node.Left.SymType.Equals(_symStack.SymFloat))
                  && (node.Right.SymType.Equals(_symStack.SymInt) || node.Right.SymType.Equals(_symStack.SymFloat))))
            {
                throw new Exception(string.Format("{0}, {1} : syntax error, incompatible types: '{2}' {3} '{4}'",
                    node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value, node.Right.SymType));
            }

            node.SymType = _symStack.SymBool;
            node.IsLValue = false;
            return true;
        }
    }
}