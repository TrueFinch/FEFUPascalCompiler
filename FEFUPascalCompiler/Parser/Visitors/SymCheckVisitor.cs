using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.Visitors
{
    public class SymCheckVisitor : IAstVisitor<bool>
    {
        public bool Visit(Ident node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ConstIntegerLiteral node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ConstDoubleLiteral node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(BinOperator node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(AssignStatement node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(Program node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(MainBlock node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ConstDeclsPart node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(TypeDeclsPart node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(TypeDecl node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ConstDecl node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(VarDeclsPart node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(SimpleVarDecl node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(InitVarDecl node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ProcFuncDeclsPart node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ProcDecl node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ProcHeader node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(FuncDecl node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(FuncHeader node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(SubroutineBlock node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(Forward node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(UnaryOperator node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ArrayAccess node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(RecordAccess node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(FunctionCall node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(FormalParamSection node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(Modifier node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ConstCharLiteral node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ConstStringLiteral node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(Nil node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(CompoundStatement node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(EmptyStatement node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(IfStatement node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(WhileStatement node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ForStatement node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(SimpleType node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ArrayTypeAstNode node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(IndexRangeAstNode node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(RecordTypeAstNode node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(FieldSection node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(PointerType node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ProcSignature node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(FuncSignature node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ConformantArray node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(ForRange node)
        {
            throw new System.NotImplementedException();
        }

        public bool Visit(DereferenceOperator node)
        {
            throw new System.NotImplementedException();
        }
        
        public SymCheckVisitor (Stack<OrderedDictionary> symbolTableStack)
        {
            SymbolTableStack = symbolTableStack;
        }
        
        public Stack<OrderedDictionary> SymbolTableStack { get; }
    }
}