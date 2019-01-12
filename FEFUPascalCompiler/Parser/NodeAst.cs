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
        public BinOperation(BinOperatorToken oper, NodeAst left, NodeAst right)
        {
            Oper = oper;
            Value = string.Format("{0} {1} {2}", left.ToString(), oper.Value, right.ToString());
            _children.Add(left);
            _children.Add(right);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public BinOperatorToken Oper {get;}
        public NodeAst Left {get=>_children[0];}
        public NodeAst Right {get=>_children[1];}
    }
}