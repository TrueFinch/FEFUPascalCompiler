namespace FEFUPascalCompiler.Parser.Sematics
{
    public abstract class Var : Symbol
    {
        protected Var(string ident, Type varType) : base(ident)
        {
            VarType = varType;
        }
        public Type VarType { get; }
    }

    public class Local : Var
    {
        public Local(Type varType) : base("Local", varType)
        {
        }
    }
    
    public class Global : Var
    {
        public Global(Type varType) : base("Global", varType)
        {
        }
    }
    
    public class Parameter : Var
    {
        public Parameter(string modifier, Type varType) : base("Parameter", varType)
        {
            Modifier = modifier;
        }

        public string Modifier { get; }
    }
}