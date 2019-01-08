namespace FEFUPascalCompiler.Parser.AstVisitor
{
    internal interface IAstVisitor <T>
    {
        T Visit(Variable node);
        T Visit(ConstLiteral node);
        T Visit(BinOperation node);
    }
}