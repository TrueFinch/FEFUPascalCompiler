using System.Collections.Generic;
using System.Globalization;
using FEFUPascalCompiler.Parser.AstVisitor;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser
{
    public abstract class NodeAst
    {
        public override string ToString() => Value;

        public abstract T Accept<T>(IAstVisitor<T> visitor);

        protected string Value { get; set; }
        protected List<NodeAst> _children = new List<NodeAst>();
    }

    public class Identifier : NodeAst
    {
        public Identifier(IdentToken token)
        {
            Token = token;
            Value = token.Value;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public IdentToken Token { get; }
    }

    public class ConstIntegerLiteral : NodeAst
    {
        public ConstIntegerLiteral(IntegerNumberToken token)
        {
            Token = token;
            Value = token.Value.ToString();
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public IntegerNumberToken Token { get; }
    }

    public class ConstDoubleLiteral : NodeAst
    {
        public ConstDoubleLiteral(DoubleNumberToken token)
        {
            Token = token;
            Value = token.Value.ToString(new NumberFormatInfo {NumberDecimalSeparator = "."});
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public DoubleNumberToken Token { get; }
    }

    public class BinOperation : NodeAst
    {
        public BinOperation(Token operation, NodeAst left, NodeAst right)
        {
            Operation = operation;
            Value = string.Format("{0} {1} {2}", left.ToString(), operation.Value, right.ToString());
            _children.Add(left);
            _children.Add(right);
        }

        protected BinOperation(NodeAst left, NodeAst right)
        {
            _children.Add(left);
            _children.Add(right);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token Operation { get; }
        public NodeAst Left => _children[0];
        public NodeAst Right => _children[1];
    }

    public class AssignStatement : BinOperation
    {
        public AssignStatement(AssignToken operation, NodeAst left, NodeAst right) : base(left, right)
        {
            Operation = operation;
            Value = string.Format("{0} {1} {2}", left.ToString(), operation.Value, right.ToString());
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        public new AssignToken Operation { get; }
    }
}