using FEFUPascalCompiler.Parser.Semantics;

namespace FEFUPascalCompiler.Parser.Sematics
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

    public abstract class SymConst : SymVar
    {
        public SymConst(string ident, SymType varSymType = null) : base(ident, varSymType)
        {
        }

        public object Value { get; } = null;
    }

    public class SymIntegerConst : SymConst
    {
        public SymIntegerConst(int value) : base("integer_const")
        {
            Value = value;
        }
        
        public int Value { get; }
    }
}