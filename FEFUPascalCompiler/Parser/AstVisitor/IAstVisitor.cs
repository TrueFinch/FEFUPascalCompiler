using FEFUPascalCompiler.Parser.AstNodes;

namespace FEFUPascalCompiler.Parser.AstVisitor
{
    public interface IAstVisitor <T>
    {
        T Visit(Ident node);
        T Visit(ConstIntegerLiteral node);
        T Visit(ConstDoubleLiteral node);
        T Visit(BinOperator node);
        T Visit(AssignStatement node);
        T Visit(Program node);
//        T Visit(ProgramHeader node);
        T Visit(MainBlock node);
    }
}