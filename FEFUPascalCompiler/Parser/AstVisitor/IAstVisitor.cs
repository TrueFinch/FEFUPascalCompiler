namespace FEFUPascalCompiler.Parser.AstVisitor
{
    public interface IAstVisitor <T>
    {
        T Visit(Identifier node);
        T Visit(ConstIntegerLiteral node);
        T Visit(ConstDoubleLiteral node);
        T Visit(BinOperation node);
    }
}