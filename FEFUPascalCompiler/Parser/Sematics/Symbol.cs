using System.Collections.Generic;
using System.Collections.Specialized;
using FEFUPascalCompiler.Parser.AstNodes;

namespace FEFUPascalCompiler.Parser.Sematics
{
    public abstract class Symbol
    {
        protected Symbol(string ident)
        {
            Ident = ident.ToLower();
        }
        
        public string Ident { get; set; }
    }

    public abstract class CallableSymbol : Symbol
    {
        protected CallableSymbol(string ident) : base(ident)
        {
        }
        
        public SymbolTable Parameters { get; set; }
        public SymbolTable Local { get; set; }
        public AstNode Body { get; set; }
    } 
    
    public class FunctionSymbol : CallableSymbol
    {
        public FunctionSymbol(/*Type returnType, OrderedDictionary parameters, OrderedDictionary local, AstNode body,*/ string ident = "") : base(ident)
        {
//            Parameters = parameters;
//            Local = local;
//            ReturnType = returnType;
//            Body = body;
        }
        

        public SymbolType ReturnSymbolType { get; set; }
    }
    
    public class ProcedureSymbol : CallableSymbol
    {
        public ProcedureSymbol(/*Type returnType, OrderedDictionary parameters, OrderedDictionary local, AstNode body,*/ string ident = "") : base(ident)
        {
//            Parameters = parameters;
//            Local = local;
//            ReturnType = returnType;
//            Body = body;
        }
    }
}
