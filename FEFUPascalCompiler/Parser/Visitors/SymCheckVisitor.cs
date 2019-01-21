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

        public bool Visit(Ident node)
        {
            if (node.SymType != null) return true;

            var sym = SymbolTableStack.FindIdent(node.ToString());
            if (sym == null)
                throw new Exception(string.Format(
                    "{0}, {1} : syntax error, identifier {2} is not defined",
                    node.Token.Line, node.Token.Column, node.Token.Lexeme));
            node.SymType = sym;
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