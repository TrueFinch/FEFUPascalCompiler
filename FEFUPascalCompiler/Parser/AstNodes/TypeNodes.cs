using System.Collections.Generic;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Parser.Visitors;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.AstNodes
{
    public abstract class TypeNode : AstNode
    {
        protected TypeNode(AstNodeType nodeType, Token token = null) : base(nodeType, token)
        {
        }

        public SymType SymType;
    }

    public class SimpleTypeNode : TypeNode
    {
        public SimpleTypeNode(AstNode typeIdent) : base(AstNodeType.SimpleType, typeIdent.Token)
        {
            TypeIdent = typeIdent;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode TypeIdent { get; set; }
    }

    public class ArrayTypeNode : TypeNode
    {
        public ArrayTypeNode(IndexRangeNode indexRange, TypeNode arrayNodeArrayElemType) : base(AstNodeType.ArrayType)
        {
            IndexRange = indexRange;
            ArrayElemType = arrayNodeArrayElemType;
            Value = NodeType.ToString();
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public IndexRangeNode IndexRange { get; set; }
        public TypeNode ArrayElemType { get; set; }
    }

    public class IndexRangeNode : AstNode
    {
        public IndexRangeNode(Token doubleDot, ConstIntegerLiteral leftBound, ConstIntegerLiteral rightBound) : base(
            AstNodeType.IndexRange)
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
        public ConstIntegerLiteral LeftBound { get; set; }
        public ConstIntegerLiteral RightBound { get; set; }
    }

    public class RecordTypeNode : TypeNode
    {
        public RecordTypeNode(List<FieldSectionNode> fieldsList) : base(AstNodeType.RecordType)
        {
            FieldsList = fieldsList;
            Value = NodeType.ToString();
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<FieldSectionNode> FieldsList { get; set; }
        public SymbolTable SymbolTable;
    }

    public class FieldSectionNode : TypeNode
    {
        public FieldSectionNode(Token colon, List<Ident> identList, TypeNode identsType) : base(
            AstNodeType.FieldSection,
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
        public List<Ident> Idents { get; set; }
        public TypeNode IdentsType { get; set; }
    }

    public class PointerTypeNode : TypeNode
    {
        public PointerTypeNode(Token carriage, TypeNode simpleType) : base(AstNodeType.PointerType, carriage)
        {
            Carriage = carriage;
            SimpleType = simpleType;
            Value = Carriage.Value + simpleType.ToString();
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token Carriage { get; }
        public TypeNode SimpleType { get; set; }
    }

    public class ConformantArray : TypeNode
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
        public AstNode ArrayType { get; set; }
    }
}