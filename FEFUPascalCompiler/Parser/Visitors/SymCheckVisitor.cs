using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Sematics;

namespace FEFUPascalCompiler.Parser.Visitors
{
    public class SymCheckVisitor : IAstVisitor<bool>
    {
        public SymCheckVisitor(SymbolStack symbolTableStack)
        {
            SymbolTableStack = symbolTableStack;
        }

        public SymbolStack SymbolTableStack { get; }

        public bool Visit(ConstIntegerLiteral node)
        {
            if (node.SymType != null) return true;

            node.SymType = SymbolTableStack.SymInteger;
            return true;
        }

        public bool Visit(ConstFloatLiteral node)
        {
            if (node.SymType != null) return true;

            node.SymType = SymbolTableStack.SymFloat;
            return true;
        }

        public bool Visit(BinOperator node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(Modifier node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ConstCharLiteral node)
        {
            if (node.SymType != null) return true;

            node.SymType = SymbolTableStack.SymChar;
            return true;
        }

        public bool Visit(ConstStringLiteral node)
        {
            if (node.SymType != null) return true;

            node.SymType = SymbolTableStack.SymString;
            return true;
        }

        public bool Visit(Nil node)
        {
            if (node.SymType != null) return true;

            node.SymType = SymbolTableStack.SymNil;
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
                    "{0}, {1} : syntax error, left part of assignment {2} is not lvalue",
                    node.Left.Token.Line, node.Left.Token.Column, node.Left.ToString()));
            }

            _typeChecker.Assignment(ref node.Left, ref node.Right, node.NodeType);
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

        public bool Visit(Ident node)
        {
            if (node.SymType != null) return true;

            var sym = SymbolTableStack.FindIdent(node.ToString());
            if (sym == null)
                throw new Exception(string.Format(
                    "{0}, {1} : syntax error, identifier {2} is not defined",
                    node.Token.Line, node.Token.Column, node.Token.Lexeme));
            node.SymVar = sym;
            node.SymType = sym.VarSymType;

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
            throw new NotImplementedException();
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
    }
}