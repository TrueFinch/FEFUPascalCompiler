using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.Visitors
{
    public class SymCheckVisitor : IAstVisitor<bool>
    {
        public SymCheckVisitor(SymbolStack symStack)
        {
            _symStack = symStack;
        }

        private readonly SymbolStack _symStack;
//        private readonly TypeChecker _typeChecker;

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

            if (node.Right is FunctionCall functionCall && functionCall.SymType == null)
            {
                throw new Exception(string.Format(
                    "{0}, {1} : syntax error, expression expected, but procedure '{2}' found",
                    node.Right.Token.Line, node.Right.Token.Column, node.Right.ToString()));
            }
            
            if (node.Left.SymType.Equals(_symStack.SymFloat) && node.Right.SymType.Equals(_symStack.SymInt))
            {
                node.Left = new Cast(node.Right) {SymType = _symStack.SymFloat, IsLValue = false};
                return true;
            }
            
            switch (node.Token.Type)
            {
                case TokenType.SimpleAssignOperator:
                {
                    if (node.Left.SymType.Equals(node.Right.SymType))
                    {
                        return true;
                    }
                    
                    throw new Exception(string.Format(
                        "{0}, {1} : syntax error, incompatible types: got '{2}' expected '{3}'",
                        node.Token.Line, node.Token.Column, node.Right.SymType.ToString(), node.Left.SymType.ToString()));
                }
                case TokenType.SumAssignOperator:
                case TokenType.DifAssignOperator:
                case TokenType.MulAssignOperator:
                case TokenType.DivAssignOperator:
                {
                    if (node.Left.SymType.Equals(_symStack.SymInt) && node.Right.SymType.Equals(_symStack.SymInt))
                    {
                        return true;
                    }
                    
                    throw new Exception(string.Format(
                        "{0}, {1} : syntax error, incompatible types to assign: got '{2}' expected '{3}'",
                        node.Token.Line, node.Token.Column, node.Right.SymType, node.Left.SymType));
                }
                default:
                {
                    throw new Exception(string.Format(
                        "{0}, {1} : syntax error, unknown assignment operator '{2}'",
                        node.Token.Line, node.Token.Column, node.Token.Lexeme));
                }
            }
            
//            return true;
        }

        public bool Visit(Program node)
        {
            _symStack.Push(); // global namespace
            node.MainBlock.Accept(this);
            return true;
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
            _symStack.CheckIdentifierDuplicateInScope(node.Ident.Token);
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
                    arrayTypeNode.Accept(this);
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

                    _symStack.CheckIdentifierDuplicate(node.Ident.Token);

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
                            _symStack.CheckIdentifierDuplicateInScope(fieldIdent.Token);
                            _symStack.AddVariable(true, fieldIdent.ToString(), fieldType);
//                            fieldIdent.SymType = fieldType;
//                            fieldIdent.SymVar = _symStack.FindIdentInScope(fieldIdent.ToString());
//                            fieldIdent.IsLValue = true;
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

        public bool Visit(SimpleTypeNode node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ConstDeclsPart node)
        {
            foreach (var nodeDecl in node.Decls)
            {
                nodeDecl.Accept(this);
            }

            return true;
        }

        public bool Visit(ConstDecl node)
        {
            node.Expression.Accept(this);

            _symStack.CheckIdentifierDuplicate(node.Ident.Token);

            _symStack.AddConstant(node.IsLocal, node.Ident.ToString(), node.Expression.SymType);

            return true;
        }

        public bool Visit(VarDeclsPart node)
        {
            foreach (var nodeDecl in node.Decls)
            {
                nodeDecl.Accept(this);
            }

            return true;
        }

        public bool Visit(SimpleVarDecl node)
        {
            var identsType = _symStack.CheckTypeDeclared(node.IdentsType.Token);
            foreach (var ident in node.IdentList)
            {
                _symStack.CheckIdentifierDuplicate(ident.Token);
                _symStack.AddVariable(node.IsLocal, ident.ToString(), identsType);
            }

            return true;
        }

        // TODO: at the end we need to add to stack new function
        public bool Visit(CallableDeclNode node)
        {
            _symStack.Push(); //this will be function namespace
            node.Header.Accept(this);

            if (node.Block != null)
            {
                _symStack.Push(node.Header.CallableSymbol.Local);
                node.Header.CallableSymbol.IsForward = false;
                node.Block.Accept(this);
                node.Header.CallableSymbol.Local = _symStack.Pop();
                _symStack.AddFunction(node.Header.CallableSymbol);
            }

            return true;
        }

        public bool Visit(CallableHeader node)
        {
//            _symStack.CheckIdentifierDuplicate(node.Name.Token);
            var callable = new CallableSymbol(node.Name.ToString());

            foreach (var paramSection in node.ParamList)
            {
                paramSection.Accept(this);
                foreach (var parameter in paramSection.ParamList)
                {
                    callable.ParametersTypes.Add(_symStack.FindIdentInScope(parameter.ToString()).VarSymType);
                }
            }

            var retType = _symStack.FindType(node.ReturnType.ToString());
//            if (retType != null)
//                callable.ParametersTypes.Add(retType);
            callable.IsForward = true;
            callable.ReturnSymType = retType;
            // pop function's namespace because we want to push function in the scope upper
            callable.Local = _symStack.Pop();
            node.CallableSymbol = callable;
            if (!_symStack.PrepareFunction(callable))
            {
                throw new Exception(string.Format("{0}, {1} syntax error: duplicate declaration of {2} '{3}' ",
                    node.Name.Token.Line, node.Name.Token.Column,
                    retType == null ? "procedure" : "function",
                    node.GetSignature()));
            }

            return true;
        }

        public bool Visit(CallableCallStatement node)
        {
            node.Callable.Accept(this);
            return true;
        }

        public bool Visit(FormalParamSection node)
        {
            var paramSectionType = _symStack.FindType(node.ParamType.ToString());
            foreach (var ident in node.ParamList)
            {
                _symStack.CheckIdentifierDuplicateInScope(ident.Token);
                _symStack.AddParameter(node.ParamModifier, ident.ToString(), paramSectionType);
            }

            return true;
        }

        public bool Visit(SubroutineBlock node)
        {
            foreach (var nodeDeclPart in node.DeclParts)
            {
                nodeDeclPart.Accept(this);
            }

            node.CompoundStatement.Accept(this);

            return true;
        }

        public bool Visit(IfStatement node)
        {
            node.Expression.Accept(this);
            if (!node.Expression.SymType.Equals(_symStack.SymBool))
            {
                throw new Exception(string.Format("{0}, {1} error: '{2}' expected, but '{3}' found",
                    node.IfToken.Line, node.IfToken.Column, _symStack.SymBool,
                    node.Expression.SymType));
            }

            node.ThenStatement.Accept(this);
            node.ElseStatement?.Accept(this);

            return true;
        }

        public bool Visit(WhileStatement node)
        {
            node.Expression.Accept(this);
            if (!node.Expression.SymType.Equals(_symStack.SymBool))
            {
                throw new Exception(string.Format("{0}, {1} error: '{2}' expected, but '{3}' found",
                    node.WhileToken.Line, node.WhileToken.Column, _symStack.SymBool,
                    node.Expression.SymType));
            }

            node.Statement.Accept(this);

            return true;
        }

        public bool Visit(ForStatement node)
        {
            node.Iterator.Accept(this);
            if (!node.Iterator.SymType.Equals(_symStack.SymInt))
            {
                throw new Exception(string.Format("{0}, {1} error: '{2}' expected, but '{3}' found",
                    node.Iterator.Token.Line, node.Iterator.Token.Column, _symStack.SymInt,
                    node.Iterator.SymType));
            }

            node.Range.Accept(this);
            if (!node.Range.Start.SymType.Equals(_symStack.SymInt))
            {
                throw new Exception(string.Format("{0}, {1} error: '{2}' expected, but '{3}' found",
                    node.ForToken.Line, node.ForToken.Column, _symStack.SymInt,
                    node.Range.Start.SymType));
            }

            if (!node.Range.Finish.SymType.Equals(_symStack.SymInt))
            {
                throw new Exception(string.Format("{0}, {1} error: '{2}' expected, but '{3}' found",
                    node.ForToken.Line, node.ForToken.Column, _symStack.SymInt,
                    node.Range.Finish.SymType));
            }

            node.Statement.Accept(this);

            return true;
        }

        public bool Visit(ForRange node)
        {
            node.Start.Accept(this);
            node.Finish.Accept(this);

            return true;
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

        public bool Visit(ConformantArray node)
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
            node.IsLValue = !(sym is SymConstant);

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
//            TODO: rewrite FunctionCall and symbol types of function and procedure
            if (!_symStack.FindFunc(node.Ident.ToString()))
                throw new Exception(string.Format(
                    "{0}, {1} : function '{2}' is not found",
                    node.Ident.Token.Line, node.Ident.Token.Column, node.Ident.ToString()));
            
            var paramTypes = new List<SymType>();
            foreach (var param in node.ParamList)
            {
                param.Accept(this);
                paramTypes.Add(param.SymType);
            }

            node.SymCall= _symStack.FindFunc(node.Ident.ToString(), paramTypes);
            node.SymType = node.SymCall.ReturnSymType;
            node.IsLValue = false;
            
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