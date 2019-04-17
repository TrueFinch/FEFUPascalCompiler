namespace FEFUPascalCompiler.Parser.Semantics
{
    public abstract class SymVar : Symbol
    {
        protected SymVar(string ident, SymType varSymType = null) : base(ident)
        {
            VarSymType = varSymType;
        }

        public SymType VarSymType { get; }
    }

    public class SymLocal : SymVar
    {
        public SymLocal(SymType varSymType) : base("Local", varSymType)
        {
        }
    }

    public class SymGlobal : SymVar
    {
        public SymGlobal(SymType varSymType) : base("Global", varSymType)
        {
        }
    }

    public class SymParameter : SymVar
    {
        public SymParameter(SymType varSymType, string modifier = "") : base("Parameter", varSymType)
        {
            Modifier = modifier;
        }

        public string Modifier { get; }
    }

    public class SymConstant : SymVar
    {
        public SymConstant(string ident, SymType varSymType) : base(ident, varSymType)
        {
        }
    }
}