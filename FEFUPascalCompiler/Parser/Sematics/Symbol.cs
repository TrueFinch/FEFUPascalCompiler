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

    public class FunctionSymbol : Symbol
    {
        public FunctionSymbol(/*Type returnType, OrderedDictionary parameters, OrderedDictionary local, AstNode body,*/ string ident = "") : base(ident)
        {
//            Parameters = parameters;
//            Local = local;
//            ReturnType = returnType;
//            Body = body;
        }
        
        public OrderedDictionary Parameters { get; set; }
        public OrderedDictionary Local { get; set; }
        public Type ReturnType { get; set; }
        public AstNode Body { get; set; }
    }
    
    public class ProcedureSymbol : Symbol
    {
        public ProcedureSymbol(/*Type returnType, OrderedDictionary parameters, OrderedDictionary local, AstNode body,*/ string ident = "") : base(ident)
        {
//            Parameters = parameters;
//            Local = local;
//            ReturnType = returnType;
//            Body = body;
        }
        
        public OrderedDictionary Parameters { get; set; }
        public OrderedDictionary Local { get; set; }
        public AstNode Body { get; set; }
    }
}
