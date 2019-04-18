using System;
using System.Collections.Generic;
using System.Linq;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.ParserParts;
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

        private bool _inLastNamespace = false;

        static List<AstNodeType> WritableNodes = new List<AstNodeType>
        {
            AstNodeType.ConstIntegerLiteral, AstNodeType.ConstFloatLiteral,
            AstNodeType.ConstCharLiteral, AstNodeType.BooleanLiteral, AstNodeType.Ident
        };

        public bool Visit(ConstIntegerLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = SymbolStack.SymInt;
            return true;
        }

        public bool Visit(ConstFloatLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = SymbolStack.SymFloat;
            return true;
        }

        public bool Visit(ConstCharLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = SymbolStack.SymChar;
            return true;
        }

        public bool Visit(ConstStringLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = SymbolStack.SymString;
            return true;
        }

        public bool Visit(Nil node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = SymbolStack.SymNil;
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
                    "({0}, {1}) semantic error: left part of assignment '{2}' is not lvalue",
                    node.Left.Token.Line, node.Left.Token.Column, node.Left.ToString()));
            }

            if (node.Right is FunctionCall functionCall && functionCall.SymType == null)
            {
                throw new Exception(string.Format(
                    "({0}, {1}) semantic error: expression expected, but procedure '{2}' found",
                    node.Right.Token.Line, node.Right.Token.Column, node.Right.ToString()));
            }

            if (node.Left.SymType.Equals(SymbolStack.SymFloat) && node.Right.SymType.Equals(SymbolStack.SymInt))
            {
                node.Right = new Cast(node.Right) {SymType = SymbolStack.SymFloat, IsLValue = false};
                node.Right.Accept(this);
                return true;
            }

            switch (node.Token.Type)
            {
                case TokenType.SimpleAssignOperator:
                {
                    if (node.Left.SymType.IsEquivalent(node.Right.SymType) &&
                        !(node.Left.SymType.Equals(SymbolStack.SymInt) &&
                          node.Left.SymType.Equals(SymbolStack.SymFloat)))
                    {
                        return true;
                    }

                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: incompatible types: got '{2}' expected '{3}'",
                        node.Token.Line, node.Token.Column, node.Right.SymType.ToString(),
                        node.Left.SymType.ToString()));
                }
                case TokenType.SumAssignOperator:
                case TokenType.DifAssignOperator:
                case TokenType.MulAssignOperator:
                case TokenType.DivAssignOperator:
                {
                    if (node.Left.SymType.Equals(SymbolStack.SymInt) && node.Right.SymType.Equals(SymbolStack.SymInt) ||
                        node.Left.SymType.Equals(SymbolStack.SymFloat) &&
                        node.Right.SymType.Equals(SymbolStack.SymInt) ||
                        node.Left.SymType.Equals(SymbolStack.SymFloat) &&
                        node.Right.SymType.Equals(SymbolStack.SymFloat))
                    {
                        return true;
                    }

                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: incompatible types to assign: got '{2}' expected '{3}'",
                        node.Token.Line, node.Token.Column, node.Right.SymType, node.Left.SymType));
                }
                default:
                {
                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: unknown assignment operator '{2}'",
                        node.Token.Line, node.Token.Column, node.Token.Lexeme));
                }
            }
        }

        public bool Visit(Program node)
        {
            node.MainBlock.Accept(this);
            return true;
        }

        public bool Visit(MainBlock node)
        {
            foreach (var declarationsPart in node.DeclsParts)
            {
                declarationsPart.Accept(this);
            }

            foreach (var declarationsPart in node.DeclsParts)
            {
                if (!(declarationsPart is CallableDeclNode callableDecl) ||
                    !callableDecl.Header.CallableSymbol.IsForward) continue;
                foreach (var callable in _symStack.FindFunc(callableDecl.Header.Name.ToString()))
                    if (callable.IsForward)
                        throw new Exception(string.Format(
                            "({0}, {1}) semantic error: forward declaration is not solved",
                            callableDecl.Header.Name.Token.Line, callableDecl.Header.Name.Token.Column));
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
            //TODO: remove this code  (in cases to separate private functions
            CheckIdentifierDuplicateInScope(node.Ident.Token);
            switch (node.IdentType)
            {
                case SimpleTypeNode simpleTypeNode:
                {
                    var symType = _symStack.CheckTypeDeclared(simpleTypeNode.Token.Value);

                    _symStack.AddAlias(node.Ident.ToString(), symType);
                    break;
                }
                case ArrayTypeNode arrayTypeNode:
                {
                    arrayTypeNode.Accept(this);
                    _symStack.AddType(node.Ident.ToString(), arrayTypeNode.SymType);
                    break;
                }
                case RecordTypeNode recordTypeNode:
                {
                    recordTypeNode.Accept(this);
                    _symStack.AddType(node.Ident.ToString(), recordTypeNode.SymType);
                    break;
                }
                case PointerTypeNode pointerTypeNode:
                {
                    //TODO: this is something strange! Why alias here when AddPtr or smth must be
                    var ptrType = _symStack.CheckTypeDeclared(pointerTypeNode.SimpleType.Token.Value);

                    _symStack.AddAlias(node.Ident.ToString(), ptrType);
                    break;
                }
            }

            return true;
        }

        public bool Visit(SimpleTypeNode node)
        {
            node.SymType = _symStack.CheckTypeDeclared(node.ToString());
            if (node.SymType == null)
            {
                throw new Exception(string.Format("({0}, {1}) semantic error: the type '{2}' is not declared ",
                    node.Token.Line, node.Token.Column, node.ToString()));
            }

            return false;
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

            CheckIdentifierDuplicate(node.Ident.Token);

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
            //TODO: get types from functions we got after separate code in type declaration
            SymType identsType;
            switch (node.IdentsType)
            {
                case ArrayTypeNode arrayTypeNode:
                    arrayTypeNode.Accept(this);
                    identsType = arrayTypeNode.SymType;
                    break;
                case RecordTypeNode recordTypeNode:
                    recordTypeNode.Accept(this);
                    identsType = recordTypeNode.SymType;
                    break;
                case PointerTypeNode pointerTypeNode:
                    identsType = _symStack.CheckTypeDeclared(node.IdentsType.ToString());
                    break;
                default:
                    identsType = _symStack.CheckTypeDeclared(node.IdentsType.Token.Value);
                    break;
            }

            if (identsType == null)
            {
                throw new Exception(string.Format("({0}, {1}) semantic error: type '{2}' is not found",
                    node.IdentsType.Token.Line, node.IdentsType.Token.Column, node.IdentsType.ToString()));
            }

            foreach (var ident in node.IdentList)
            {
                CheckIdentifierDuplicate(ident.Token);
                _symStack.AddVariable(node.IsLocal, ident.ToString(), identsType);
                ident.SymType = identsType;
            }

            return true;
        }

        // TODO: at the end we need to add to stack new function
        public bool Visit(CallableDeclNode node)
        {
            node.Header.Accept(this);

            var existedCallable = _symStack.FindFunc(node.Header.CallableSymbol.Ident,
                new List<SymType>(from type in node.Header.CallableSymbol.ParametersTypes select type.Item2));

            if (existedCallable != null)
            {
                if (!existedCallable.IsForward)
                {
                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: overloaded functions have the same parameter list",
                        node.Header.Name.Token.Line, node.Header.Name.Token.Column));
                }

                var modifiersAreSame = true;
                for (var i = 0; i < existedCallable.ParametersTypes.Count; ++i)
                {
                    if (existedCallable.ParametersTypes[i].Item1 ==
                        node.Header.CallableSymbol.ParametersTypes[i].Item1) continue;
                    modifiersAreSame = false;
                    break;
                }

                if (!modifiersAreSame)
                {
                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: function does not match the previous declaration",
                        node.Header.Name.Token.Line, node.Header.Name.Token.Column));
                }
            }
            else
            {
                _symStack.PrepareFunction(node.Header.CallableSymbol);
            }

            if (node.Block != null)
            {
                _symStack.Push(node.Header.CallableSymbol.Local);
                node.Header.CallableSymbol.IsForward = false;
                _inLastNamespace = true;
                node.Block.Accept(this);
                _inLastNamespace = false;
                node.Header.CallableSymbol.Local = _symStack.Pop();
                _symStack.AddFunction(node.Header.CallableSymbol);
            }
            else if (existedCallable != null)
            {
                throw new Exception(string.Format(
                    "({0}, {1}) semantic error: function is already declared",
                    node.Header.Name.Token.Line, node.Header.Name.Token.Column));
            }

            return true;
        }

        public bool Visit(CallableHeader node)
        {
            _symStack.Push(); //this will be function local namespace
            var callable = new CallableSymbol(node.Name.ToString()) {ReturnSymType = null};
            if (node.ReturnType != null)
            {
                node.ReturnType.Accept(this);
                callable.ReturnSymType = _symStack.FindType(node.ReturnType.ToString());
                _symStack.AddVariable(true, "result", callable.ReturnSymType);
            }

            foreach (var paramSection in node.ParamList)
            {
                paramSection.Accept(this);
                foreach (var parameter in paramSection.ParamList)
                {
                    callable.ParametersTypes.Add(new Tuple<string, SymType>(paramSection.ParamModifier,
                        _symStack.FindIdentInScope(parameter.ToString()).VarSymType));
                }
            }

            callable.IsForward = true;
            callable.Local = _symStack.Pop();
            node.CallableSymbol = callable;
//            if (!_symStack.PrepareFunction(callable))
//            {
//                throw new Exception(string.Format("({0}, {1}) semantic error: duplicate declaration of {2} '{3}' ",
//                    node.Name.Token.Line, node.Name.Token.Column,
//                    callable.ReturnSymType == null ? "procedure" : "function",
//                    node.GetSignature()));
//            }

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
            if (paramSectionType == null)
            {
                throw new Exception(string.Format("({0}, {1}) semantic error: type '{2}' is not found",
                    node.ParamType.Token.Line, node.ParamType.Token.Column, node.ParamType.ToString()));
            }

            foreach (var ident in node.ParamList)
            {
                CheckIdentifierDuplicateInScope(ident.Token);
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
            if (!node.Expression.SymType.IsEquivalent(SymbolStack.SymBool))
            {
                throw new Exception(string.Format("({0}, {1}) semantic error: '{2}' expected, but '{3}' found",
                    node.IfToken.Line, node.IfToken.Column, SymbolStack.SymBool,
                    node.Expression.SymType));
            }

            node.ThenStatement.Accept(this);
            node.ElseStatement?.Accept(this);

            return true;
        }

        public bool Visit(WhileStatement node)
        {
            node.Expression.Accept(this);
            if (!node.Expression.SymType.Equals(SymbolStack.SymBool))
            {
                throw new Exception(string.Format("({0}, {1}) semantic error: '{2}' expected, but '{3}' found",
                    node.WhileToken.Line, node.WhileToken.Column, SymbolStack.SymBool,
                    node.Expression.SymType));
            }

            node.Statement.Accept(this);

            return true;
        }

        public bool Visit(ForStatement node)
        {
            node.Iterator.Accept(this);
            if (!node.Iterator.SymType.Equals(SymbolStack.SymInt))
            {
                throw new Exception(string.Format("({0}, {1}) semantic error: '{2}' expected, but '{3}' found",
                    node.Iterator.Token.Line, node.Iterator.Token.Column, SymbolStack.SymInt,
                    node.Iterator.SymType));
            }

            node.Range.Accept(this);
            if (!node.Range.Start.SymType.Equals(SymbolStack.SymInt))
            {
                throw new Exception(string.Format("({0}, {1}) semantic error: '{2}' expected, but '{3}' found",
                    node.ForToken.Line, node.ForToken.Column, SymbolStack.SymInt,
                    node.Range.Start.SymType));
            }

            if (!node.Range.Finish.SymType.Equals(SymbolStack.SymInt))
            {
                throw new Exception(string.Format("({0}, {1}) semantic error: '{2}' expected, but '{3}' found",
                    node.Range.Finish.Token.Line, node.Range.Finish.Token.Column, SymbolStack.SymInt,
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
            node.IndexRange.Accept(this);
            node.ArrayElemType.Accept(this);
            node.SymType = new SymArrayType(new IndexRangeSymbol<int, int>(
                    ((IntegerNumberToken) node.IndexRange.LeftBound.Token).NumberValue,
                    ((IntegerNumberToken) node.IndexRange.RightBound.Token).NumberValue),
                node.ArrayElemType.SymType);
//            _symStack.AddType(node.SymType);
            return true;
        }

        public bool Visit(IndexRangeNode node)
        {
            node.LeftBound.Accept(this);

            if (!node.LeftBound.SymType.Equals(SymbolStack.SymInt))
            {
                throw new Exception(string.Format(
                    "({0}, {1}) semantic error: index bound '{2}' is not integer",
                    node.LeftBound.Token.Line, node.LeftBound.Token.Line, node.LeftBound.ToString()));
            }

            node.RightBound.Accept(this);

            if (!node.RightBound.SymType.Equals(SymbolStack.SymInt))
            {
                throw new Exception(string.Format(
                    "({0}, {1}) semantic error: index bound '{2}' is not integer",
                    node.RightBound.Token.Line, node.RightBound.Token.Line, node.RightBound.ToString()));
            }

            return true;
        }

        public bool Visit(RecordTypeNode node)
        {
            _symStack.Push();
            foreach (var field in node.FieldsList)
            {
                field.Accept(this);
            }

            node.SymbolTable = _symStack.Pop();
            node.SymType = new SymRecordType(node.SymbolTable);
            return true;
        }

        public bool Visit(FieldSectionNode node)
        {
            node.IdentsType.Accept(this);
            foreach (var ident in node.Idents)
            {
                CheckIdentifierDuplicateInScope(ident.Token);
                _symStack.AddVariable(true, ident.ToString(), node.IdentsType.SymType);
            }

            return true;
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
            node.ExprToCast.Accept(this);
            return true;
        }

        public bool Visit(ConstBooleanLiteral node)
        {
            if (node.SymType != null) return true;

            node.IsLValue = false;
            node.SymType = SymbolStack.SymBool;
            return true;
        }

        public bool Visit(Ident node)
        {
            if (node.SymType != null) return true;

            var sym = !_inLastNamespace
                ? _symStack.FindIdent(node.ToString())
                : _symStack.FindIdentInScope(node.ToString()) ?? _symStack.FindIdent(node.ToString());

            if (sym == null)
                if (node.ToString() == "result" && _inLastNamespace)
                {
                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: procedure does not have return variable '{2}'",
                        node.Token.Line, node.Token.Column, node.Token.Lexeme));
                }
                else if (_inLastNamespace)
                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: field '{2}' is not defined in this scope",
                        node.Token.Line, node.Token.Column, node.Token.Lexeme));
                else
                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: identifier '{2}' is not defined",
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
                if (expression.SymType.GetType() != SymbolStack.SymInt.GetType())
                {
                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: index '{2}' is not integer",
                        expression.Token.Line, expression.Token.Line, expression.ToString()));
                }
            }

            if (node.ArrayIdent.SymType == null)
            {
                throw new Exception(string.Format(
                    "({0}, {1}) semantic error: array type of expression '{2}' is undeclared",
                    node.Token.Line, node.Token.Line, node.ToString()));
            }

            node.SymType = ((SymArrayType) node.ArrayIdent.SymType).ElementSymType;
            node.IsLValue = true;
            return true;
        }

        public bool Visit(DereferenceOperator node)
        {
            if (node.SymType != null) return true;

            node.Expr.Accept(this);

            if (!node.Expr.IsLValue)
            {
                throw new Exception(string.Format(
                    "({0}, {1}) semantic error: expression '{2}' is not lvalue",
                    node.Expr.Token.Line, node.Expr.Token.Column, node.Expr.ToString()));
            }

            if (!(node.Expr.SymType is SymPointerType))
            {
                throw new Exception(string.Format(
                    "({0}, {1}) semantic error: expression '{2}' is not a pointer",
                    node.Expr.Token.Line, node.Expr.Token.Column, node.Expr.ToString()));
            }

            node.SymType = ((SymPointerType) node.Expr.SymType).ReferencedSymType;
            return true;
        }

        public bool Visit(WriteFunctionCall node)
        {
            var paramTypes = new List<SymType>();
            foreach (var param in node.ParamList)
            {
                if (!WritableNodes.Contains(param.NodeType))
                {
                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: expression '{2}' is not writable",
                        param.Token.Line, param.Token.Column, param.ToString()));
                }

                param.Accept(this);
                paramTypes.Add(param.SymType);
            }

            node.ParamTypes = paramTypes;
            node.IsLValue = false;
            return true;
        }

        public bool Visit(UserFunctionCall node)
        {
            if (!_symStack.IsFuncExist(node.Ident.ToString()))
                throw new Exception(string.Format(
                    "({0}, {1}) semantic error: function '{2}' is not found",
                    node.Ident.Token.Line, node.Ident.Token.Column, node.Ident.ToString()));

            var paramTypes = new List<Tuple<bool, SymType>>();
            foreach (var param in node.ParamList)
            {
                if (!WritableNodes.Contains(param.NodeType))
                {
                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: expression '{2}' is not writable",
                        param.Token.Line, param.Token.Column, param.ToString()));
                }

                param.Accept(this);
//                paramTypes.Add(new Tuple<bool, SymType>(param.IsLValue, param.SymType));
                paramTypes.Add(new Tuple<bool, SymType>(param.IsLValue,
                    (param.SymType as SymAliasType)?.GetBase() ?? param.SymType));
            }

            var paramTypesString = paramTypes[0].Item2.ToString();
            for (var i = 1; i < paramTypes.Count; ++i)
            {
                paramTypesString = string.Concat(paramTypesString, ", ", paramTypes[i].Item2.ToString());
            }

            var possibleCallables = _symStack.FindFunc(node.Ident.ToString());

            if (possibleCallables == null)
            {
                throw new Exception(string.Format(
                    "({0}, {1}) semantic error: function '{2}({3})' is not found",
                    node.Ident.Token.Line, node.Ident.Token.Column, node.Ident.ToString(),
                    paramTypesString));
            }

            CallableSymbol calledCallable = null;

            foreach (var callable in possibleCallables)
            {
                if (callable == null || paramTypes.Count != callable.ParametersTypes.Count)
                {
                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: function '{2}({3})' is not found",
                        node.Ident.Token.Line, node.Ident.Token.Column, node.Ident.ToString(),
                        paramTypesString));
                }

                var callableFound = true;

                for (var i = 0; i < paramTypes.Count; ++i)
                {
                    if (!((callable.ParametersTypes[i].Item2 as SymAliasType)?.GetBase() ??
                          callable.ParametersTypes[i].Item2).IsEquivalent(paramTypes[i].Item2))
                    {
                        callableFound = false;
                    }
                    else
                        //if got parameter is not lvalue, but it has 'var' modifier
                    if (callable.ParametersTypes[i].Item1 == "var" && !paramTypes[i].Item1)
                    {
                        throw new Exception(string.Format(
                            "({0}, {1}) semantic error: parameter '{2}' is not lvalue",
                            node.ParamList[i].Token.Line, node.ParamList[i].Token.Column,
                            node.ParamList[i].ToString()));
                    }
                }

                if (callableFound)
                {
                    calledCallable = callable;
                    break;
                }
            }

            if (calledCallable == null)
            {
                throw new Exception(string.Format(
                    "({0}, {1}) semantic error: function '{2}({3})' is not found",
                    node.Ident.Token.Line, node.Ident.Token.Column, node.Ident.ToString(),
                    paramTypesString));
            }

            node.CallableSymbol = calledCallable;
            node.SymType = calledCallable.ReturnSymType;
            node.IsLValue = false;
            return true;
        }

        public bool Visit(RecordAccess node)
        {
            node.RecordIdent.Accept(this);
            _symStack.Push(((SymRecordType) node.RecordIdent.SymType).Table);
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
                    if (!(node.Expr.SymType.Equals(SymbolStack.SymInt)
                          || node.Expr.SymType.Equals(SymbolStack.SymFloat)))
                    {
                        throw new Exception(string.Format(
                            "({0}, {1}) semantic error: 'integer' or 'float' expected, but '{2}' found",
                            node.Expr.Token.Line, node.Expr.Token.Column, node.Expr.Token.Lexeme));
                    }

                    node.SymType = node.Expr.SymType;

                    break;
                }

                case TokenType.AtSign:
                {
                    if (!node.Expr.IsLValue)
                    {
                        throw new Exception(string.Format(
                            "({0}, {1}) semantic error: expression '{2}' is not lvalue",
                            node.Expr.Token.Line, node.Expr.Token.Column, node.Expr.ToString()));
                    }

                    node.SymType = _symStack.SymTypeToSymPointerType(node.Expr.SymType);
                    break;
                }
                case TokenType.Not:
                {
                    if (!node.Expr.SymType.Equals(SymbolStack.SymBool))
                    {
                        throw new Exception(string.Format(
                            "({0}, {1}) semantic error: boolean expected, but '{2}' found",
                            node.Expr.Token.Line, node.Expr.Token.Column, node.Expr.ToString()));
                    }

                    node.SymType = node.Expr.SymType;
                    break;
                }
                default:
                {
                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: unrecognized unary operator '{2}'",
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
            if (!(node.Left.SymType.Equals(SymbolStack.SymBool) && node.Right.SymType.Equals(SymbolStack.SymBool)
                  || (node.Left.SymType.Equals(SymbolStack.SymInt) || node.Left.SymType.Equals(SymbolStack.SymFloat))
                  && (node.Right.SymType.Equals(SymbolStack.SymInt) ||
                      node.Right.SymType.Equals(SymbolStack.SymFloat))))
            {
                throw new Exception(string.Format("({0}, {1}) semantic error: incompatible types: '{2}' {3} '{4}'",
                    node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value, node.Right.SymType));
            }

            //check if operator - and and operands are boolean type
            //give us ability to avoid this check in every case of switch
            if (node.Left.SymType.Equals(SymbolStack.SymBool) && node.Right.SymType.Equals(SymbolStack.SymBool) &&
                node.Token.Type != TokenType.And)
            {
                throw new Exception(string.Format(
                    "({0}, {1}) semantic error: incompatible types: '{2}' {3} '{4}'",
                    node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value,
                    node.Right.SymType));
            }

            switch (node.Token.Type)
            {
                case TokenType.MulOperator:
                {
                    node.SymType = node.Left.SymType.Equals(SymbolStack.SymInt) &&
                                   node.Right.SymType.Equals(SymbolStack.SymInt)
                        ? SymbolStack.SymInt
                        : SymbolStack.SymFloat;
                    break;
                }
                case TokenType.DivOperator:
                {
                    node.SymType = SymbolStack.SymFloat;
                    break;
                }
                case TokenType.Div:
                case TokenType.Mod:
                case TokenType.Shr:
                case TokenType.Shl:
                {
                    if (!node.Left.SymType.Equals(SymbolStack.SymInt) || !node.Right.SymType.Equals(SymbolStack.SymInt))
                    {
                        throw new Exception(string.Format(
                            "({0}, {1}) semantic error: incompatible types: '{2}' {3} '{4}'",
                            node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value,
                            node.Right.SymType));
                    }

                    node.SymType = SymbolStack.SymInt;
                    break;
                }
                case TokenType.And:
                {
                    if (!node.Left.SymType.Equals(SymbolStack.SymBool) ||
                        !node.Right.SymType.Equals(SymbolStack.SymBool))
                    {
                        throw new Exception(string.Format(
                            "({0}, {1}) semantic error: incompatible types: '{2}' {3} '{4}'",
                            node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value,
                            node.Right.SymType));
                    }

                    node.SymType = SymbolStack.SymBool;
                    break;
                }
                default:
                {
                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: unhandled multiplying operator '{2}'",
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
            if (!(node.Left.SymType.Equals(SymbolStack.SymBool) && node.Right.SymType.Equals(SymbolStack.SymBool)
                  || (node.Left.SymType.Equals(SymbolStack.SymInt) || node.Left.SymType.Equals(SymbolStack.SymFloat))
                  && (node.Right.SymType.Equals(SymbolStack.SymInt)
                      || node.Right.SymType.Equals(SymbolStack.SymFloat))))
            {
                throw new Exception(string.Format("({0}, {1}) semantic error: incompatible types: '{2}' {3} '{4}'",
                    node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value, node.Right.SymType));
            }

            switch (node.Token.Type)
            {
                case TokenType.SumOperator:
                case TokenType.DifOperator:
                {
                    if (node.Left.SymType.Equals(SymbolStack.SymBool) || node.Right.SymType.Equals(SymbolStack.SymBool))
                    {
                        throw new Exception(string.Format(
                            "({0}, {1}) semantic error: incompatible types: '{2}' {3} '{4}'",
                            node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value,
                            node.Right.SymType));
                    }

                    if (node.Left.SymType.Equals(SymbolStack.SymInt) && node.Right.SymType.Equals(SymbolStack.SymFloat))
                        node.Left = new Cast(node.Left) {SymType = SymbolStack.SymFloat, IsLValue = false};

                    if (node.Right.SymType.Equals(SymbolStack.SymInt) && node.Left.SymType.Equals(SymbolStack.SymFloat))
                        node.Right = new Cast(node.Right) {SymType = SymbolStack.SymFloat, IsLValue = false};

                    node.SymType = node.Left.SymType.Equals(SymbolStack.SymInt) &&
                                   node.Right.SymType.Equals(SymbolStack.SymInt)
                        ? SymbolStack.SymInt
                        : SymbolStack.SymFloat;
                    break;
                }
                case TokenType.Or:
                case TokenType.Xor:
                {
                    if (!node.Left.SymType.Equals(SymbolStack.SymBool) ||
                        !node.Right.SymType.Equals(SymbolStack.SymBool))
                    {
                        throw new Exception(string.Format(
                            "({0}, {1}) semantic error: incompatible types: '{2}' {3} '{4}'",
                            node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value,
                            node.Right.SymType));
                    }

                    node.SymType = SymbolStack.SymBool;
                    break;
                }
                default:
                {
                    throw new Exception(string.Format(
                        "({0}, {1}) semantic error: unhandled additive operator '{2}'",
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

            // we can use additive operators only with integer and float (all operators),
            // char and boolean and nil and any pointer ('=', '<>')   
            var lType = (node.Left.SymType as SymAliasType)?.GetBase() ?? node.Left.SymType;
            var rType = (node.Right.SymType as SymAliasType)?.GetBase() ?? node.Right.SymType;

            if (lType.GetType() == typeof(SymRecordType) || rType.GetType() == typeof(SymRecordType) ||
                lType.GetType() == typeof(SymArrayType) || rType.GetType() == typeof(SymArrayType) ||
                !((node.Token.Type == TokenType.EqualOperator || node.Token.Type == TokenType.NotEqualOperator) &&
                  (lType.Equals(SymbolStack.SymBool) && rType.Equals(SymbolStack.SymBool) ||
                   lType.Equals(SymbolStack.SymChar) && rType.Equals(SymbolStack.SymChar) ||
                   (lType.Equals(SymbolStack.SymNil) || rType.GetType() == typeof(SymPointerType)) &&
                   (lType.GetType() == typeof(SymPointerType) || rType.Equals(SymbolStack.SymNil))) ||
                  (lType.Equals(SymbolStack.SymInt) || lType.Equals(SymbolStack.SymFloat)) &&
                  (rType.Equals(SymbolStack.SymInt) || rType.Equals(SymbolStack.SymFloat))))
            {
                throw new Exception(string.Format("({0}, {1}) semantic error: incompatible types: '{2}' {3} '{4}'",
                    node.Token.Line, node.Token.Column, node.Left.SymType, node.Token.Value, node.Right.SymType));
            }

            node.SymType = SymbolStack.SymBool;
            node.IsLValue = false;
            return true;
        }

        private void CheckIdentifierDuplicate(Token identToken)
        {
            if (_symStack.ContainIdent(identToken.Value))
            {
                throw new Exception(string.Format("({0}, {1}) semantic error: duplicate identifier '{2}'",
                    identToken.Line, identToken.Column, identToken.Lexeme));
            }
        }

        private void CheckIdentifierDuplicateInScope(Token identToken)
        {
            if (_symStack.ContainIdentInScope(identToken.Value))
            {
                throw new Exception(string.Format("({0}, {1}) semantic error: duplicate identifier '{2}'",
                    identToken.Line, identToken.Column, identToken.Lexeme));
            }
        }
    }
}