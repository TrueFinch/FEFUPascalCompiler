using System.Collections.Generic;
using System.Collections.Specialized;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Semantics;

namespace FEFUPascalCompiler.Parser.Semantics
{
    public abstract class Symbol
    {
        protected Symbol(string ident)
        {
            Ident = ident.ToLower();
        }

        public string Ident { get; }

        public override string ToString()
        {
            return Ident.ToString();
        }
    }

//    public class FunctionSymbol : CallableSymbol
//    {
//        public FunctionSymbol(/*Type returnType, OrderedDictionary parameters, OrderedDictionary local, AstNode body,*/ string ident = "") : base(ident)
//        {
////            Parameters = parameters;
////            Local = local;
////            ReturnType = returnType;
////            Body = body;
//        }
//        
//
//    }
//    
//    public class ProcedureSymbol : CallableSymbol
//    {
//        public ProcedureSymbol(/*Type returnType, OrderedDictionary parameters, OrderedDictionary local, AstNode body,*/ string ident = "") : base(ident)
//        {
////            Parameters = parameters;
////            Local = local;
////            ReturnType = returnType;
////            Body = body;
//        }
//    }
}