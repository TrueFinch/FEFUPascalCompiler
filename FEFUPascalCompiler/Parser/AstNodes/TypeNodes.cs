using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstVisitor;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.AstNodes
{
    public class SimpleType : AstNode
    {
        public SimpleType(AstNode typeIdent) : base(AstNodeType.SimpleType)
        {
            _children.Add(typeIdent);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode TypeIdent => _children[0];
    }

    public class ArrayTypeAstNode : AstNode
    {
        public ArrayTypeAstNode(List<AstNode> indexRanges, AstNode arrayType) : base(AstNodeType.ArrayType)
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

    public class IndexRangeAstNode : AstNode
    {
        public IndexRangeAstNode(Token doubleDot, AstNode leftBound, AstNode rightBound) : base(AstNodeType.IndexRange)
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

    public class RecordTypeAstNode : AstNode
    {
        public RecordTypeAstNode(List<AstNode> fieldsList) : base(AstNodeType.RecordType)
        {
            _children.InsertRange(0, fieldsList);
            Value = Type.ToString();
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> FieldsList => _children;
    }

    public class FieldSection : AstNode
    {
        public FieldSection(Token colon, List<AstNode> identList, AstNode identsType) : base(AstNodeType.FieldSection, colon)
        {
            Colon = colon;

            _children.InsertRange(0, identList);
            _children.Add(identsType);
            
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token Colon { get; }
        public List<AstNode> Idents => _children.GetRange(0, _children.Count - 1);
        public AstNode IdentsType => _children[_children.Count - 1];
    }

    public class PointerType : AstNode
    {
        public PointerType(Token carriage, AstNode simpleType) : base(AstNodeType.PointerType, carriage)
        {
            Carriage = carriage;
            _children.Add(simpleType);
            Value = Carriage.Value;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token Carriage { get; }
        public AstNode SimpleType => _children[0];
    }

    public class ProcSignature : AstNode
    {
        public ProcSignature(Token token, List<AstNode> paramSections) : base(AstNodeType.ProcSignature, token)
        {
            _children.InsertRange(0, paramSections);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> ParamList => _children;
    }

    public class FuncSignature : AstNode
    {
        public FuncSignature(Token token, List<AstNode> paramSections, AstNode returnType)
            : base(AstNodeType.FuncSignature, token)
        {
            _children.InsertRange(0, paramSections);
            _children.Add(returnType);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> ParamList => _children.GetRange(0, _children.Count - 1);
        public AstNode ReturnType => _children[_children.Count - 1];
    }

    public class ConformantArray : AstNode
    {
        public ConformantArray(Token arrayToken, Token ofToken, AstNode arrayType) : base(AstNodeType.ConformantArray)
        {
            ArrayToken = arrayToken;
            OfToken = ofToken;
            _children.Add(arrayType);
            Value = string.Format("{0} {1}", arrayToken.Value, ofToken.Value);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public Token ArrayToken { get; }
        public Token OfToken { get; }
        public AstNode ArrayType => _children[0];
    }
}