namespace FEFUPascalCompiler.Parser.AstVisitor
{
    internal class AstPrintVisitor: IAstVisitor<AstNodePrinter>
    {
        public AstNodePrinter Visit(Variable node)
        {
            throw new System.NotImplementedException();
        }

        public AstNodePrinter Visit(ConstLiteral node)
        {
            throw new System.NotImplementedException();
        }

        public AstNodePrinter Visit(BinOperation node)
        {
            throw new System.NotImplementedException();
        }
    }

    public class AstNodePrinter
    {
        
    }
}