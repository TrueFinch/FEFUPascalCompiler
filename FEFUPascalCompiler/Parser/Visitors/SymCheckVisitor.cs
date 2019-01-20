using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Sematics;

namespace FEFUPascalCompiler.Parser.Visitors
{
    public class SymCheckVisitor : ISymVisitor<bool>
    {
        public SymCheckVisitor (SymbolStack symbolTableStack)
        {
            SymbolTableStack = symbolTableStack;
        }
        
        public SymbolStack SymbolTableStack { get; }
        
        public bool Visit(ConstIntegerLiteral node)
        {
            if (node.SymbolType != null) return true;

            node.SymbolType = SymbolTableStack.SymInteger;
            return true;
        }

        public bool Visit(ConstFloatLiteral node)
        {
            if (node.SymbolType != null) return true;

            node.SymbolType = SymbolTableStack.SymFloat;
            return true;
        }

        public bool Visit(ConstCharLiteral node)
        {
            if (node.SymbolType != null) return true;

            node.SymbolType = SymbolTableStack.SymChar;
            return true;
        }

        public bool Visit(ConstStringLiteral node)
        {
            if (node.SymbolType != null) return true;

            node.SymbolType = SymbolTableStack.SymString;
            return true;
        }

        public bool Visit(Nil node)
        {
            if (node.SymbolType != null) return true;

            node.SymbolType = SymbolTableStack.SymNil;
            return true;
        }

        public bool Visit(Ident node)
        {
            return true;
//            if (node.SymbolType != null) return true;

//            var sym = SymbolTableStack.FindIdent();
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