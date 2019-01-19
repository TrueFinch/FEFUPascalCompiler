using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.Visitors
{
    public class SymCheckVisitor : ISymVisitor<bool>
    {
        public SymCheckVisitor (Stack<OrderedDictionary> symbolTableStack)
        {
            SymbolTableStack = symbolTableStack;
        }
        
        public Stack<OrderedDictionary> SymbolTableStack { get; }
        
        public bool Visit(ConstIntegerLiteral node)
        {
            if (node.SymbolType != null) return true;

            node.SymbolType = new IntegerSymbolType();
            return true;
        }

        public bool Visit(ConstDoubleLiteral node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ConstCharLiteral node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(ConstStringLiteral node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(Nil node)
        {
            throw new NotImplementedException();
        }

        public bool Visit(Ident node)
        {
            throw new NotImplementedException();
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