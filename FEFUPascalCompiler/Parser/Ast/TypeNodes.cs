using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstVisitor;
using FEFUPascalCompiler.Tokens;
using Microsoft.VisualBasic.CompilerServices;

namespace FEFUPascalCompiler.Parser
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

    public class ArrayType : AstNode
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

    public class RecordType : AstNode
    {
        public RecordType(List<AstNode> fieldsList) : base(AstNodeType.RecordType)
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
        public FieldSection(Token colon, AstNode identList, AstNode identsType) : base(AstNodeType.FieldSection)
        {
            Colon = colon;

            _children.Add(identList);
            _children.Add(identsType);
            Value = colon.Value;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token Colon { get; }
        public AstNode Idents => _children[0];
        public AstNode IdentsType => _children[1];
    }

    public class PointerType : AstNode
    {
        public PointerType(Token carriage, AstNode simpleType) : base(AstNodeType.PointerType)
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
            : base(AstNodeType.ProcSignature, token)
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