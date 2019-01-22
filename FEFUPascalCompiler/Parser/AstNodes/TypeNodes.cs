using System.Collections.Generic;
using FEFUPascalCompiler.Parser.Visitors;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.AstNodes
{
    public class SimpleType : AstNode
    {
        public SimpleType(AstNode typeIdent) : base(AstNodeType.SimpleType)
        {
            TypeIdent = typeIdent;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode TypeIdent { get; set; }
    }

    public class ArrayTypeAstNode : AstNode
    {
        public ArrayTypeAstNode(List<AstNode> indexRanges, AstNode arrayType) : base(AstNodeType.ArrayType)
        {
            IndexRanges = indexRanges;
            TypeOfArray = arrayType;
            Value = NodeType.ToString();
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> IndexRanges { get; set; }
        public AstNode TypeOfArray { get; set; }
    }

    public class IndexRangeAstNode : AstNode
    {
        public IndexRangeAstNode(Token doubleDot, AstNode leftBound, AstNode rightBound) : base(AstNodeType.IndexRange)
        {
            DoubleDot = doubleDot;
            LeftBound = leftBound;
            RightBound = rightBound;
            Value = doubleDot.Value;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token DoubleDot { get; }
        public AstNode LeftBound { get; set; }
        public AstNode RightBound { get; set; }
    }

    public class RecordTypeAstNode : AstNode
    {
        public RecordTypeAstNode(List<AstNode> fieldsList) : base(AstNodeType.RecordType)
        {
            FieldsList = fieldsList;
            Value = NodeType.ToString();
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> FieldsList { get; set; }
    }

    public class FieldSection : AstNode
    {
        public FieldSection(Token colon, List<AstNode> identList, AstNode identsType) : base(AstNodeType.FieldSection,
            colon)
        {
            Colon = colon;

            Idents = identList;
            IdentsType = identsType;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token Colon { get; }
        public List<AstNode> Idents { get; set;}
        public AstNode IdentsType { get; set;}
    }

    public class PointerType : AstNode
    {
        public PointerType(Token carriage, AstNode simpleType) : base(AstNodeType.PointerType, carriage)
        {
            Carriage = carriage;
            SimpleType = simpleType;
            Value = Carriage.Value;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token Carriage { get; }
        public AstNode SimpleType { get; set;}
    }

    public class ProcSignature : AstNode
    {
        public ProcSignature(Token token, List<AstNode> paramSections) : base(AstNodeType.ProcSignature, token)
        {
            ParamList = paramSections;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> ParamList  { get; set;}
    }

    public class FuncSignature : AstNode
    {
        public FuncSignature(Token token, List<AstNode> paramSections, AstNode returnType)
            : base(AstNodeType.FuncSignature, token)
        {
            ParamList = paramSections;
            ReturnType = returnType;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> ParamList { get; set;}
        public AstNode ReturnType { get; set;}
    }

    public class ConformantArray : AstNode
    {
        public ConformantArray(Token arrayToken, Token ofToken, AstNode arrayType) : base(AstNodeType.ConformantArray)
        {
            ArrayToken = arrayToken;
            OfToken = ofToken;
            ArrayType = arrayType;
            Value = string.Format("{0} {1}", arrayToken.Value, ofToken.Value);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token ArrayToken { get; }
        public Token OfToken { get; }
        public AstNode ArrayType { get; set;}
    }
}