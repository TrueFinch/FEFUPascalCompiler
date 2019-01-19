using FEFUPascalCompiler.Parser.AstNodes;

namespace FEFUPascalCompiler.Parser.Visitors
{
    public interface ISymVisitor <T>
    {
        T Visit(ArrayAccess node);
        T Visit(Ident node);
        T Visit(DereferenceOperator node);
        T Visit(FunctionCall node);
        T Visit(RecordAccess node);
        T Visit(UnaryOperator node);
        T Visit(MultiplyingOperator node);
        T Visit(AdditiveOperator node);
        T Visit(ComparingOperator node);
        T Visit(ConstIntegerLiteral node);
        T Visit(ConstDoubleLiteral node);
        T Visit(ConstCharLiteral node);
        T Visit(ConstStringLiteral node);
        T Visit(Nil node);
    }
}