using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstVisitor;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser
{
    public abstract class TypeNode : AstNode
    {
        protected TypeNode(AstNodeType type) : base(type)
        {
        }
    }

    public class ArrayType : TypeNode
    {
        public ArrayType(List<AstNode> indexRanges, AstNode arrayType) : base(AstNodeType.ArrayType)
        {           
            _children.InsertRange(0, indexRanges);
            _children.Add(arrayType);
            Value = Type.ToString();
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> IndexRanges => _children.GetRange(0, _children.Count - 1);
        public AstNode TypeOfArray => _children[_children.Count - 1];
    }

    public class IndexRange : AstNode
    {
        public IndexRange(Token doubleDot, AstNode leftBound, AstNode rightBound) : base(AstNodeType.IndexRange)
        {
            DoubleDot = doubleDot;
            _children.Add(leftBound);
            _children.Add(rightBound);
            Value = doubleDot.Value;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public Token DoubleDot { get; }
        public AstNode LeftBound => _children[0];
        public AstNode RightBound => _children[1];
    }
}