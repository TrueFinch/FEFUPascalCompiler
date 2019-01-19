namespace FEFUPascalCompiler.Parser.Sematics
{
    public abstract class Var : Symbol
    {
        protected Var(string ident, SymbolType varSymbolType) : base(ident)
        {
            VarSymbolType = varSymbolType;
        }
        public SymbolType VarSymbolType { get; }
    }

    public class Local : Var
    {
        public Local(SymbolType varSymbolType) : base("Local", varSymbolType)
        {
        }
    }
    
    public class Global : Var
    {
        public Global(SymbolType varSymbolType) : base("Global", varSymbolType)
        {
        }
    }
    
    public class Parameter : Var
    {
        public Parameter(SymbolType varSymbolType, string modifier = "") : base("Parameter", varSymbolType)
        {
            Modifier = modifier;
        }

        public string Modifier { get; }
    }
}