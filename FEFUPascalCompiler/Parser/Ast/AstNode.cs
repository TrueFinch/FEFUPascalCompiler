using System.Collections.Generic;
using System.Globalization;
using FEFUPascalCompiler.Parser.AstVisitor;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser
{
    public enum AstNodeType
    {
        // @formatter:off
        Program,
            MainBlock,
                ConstDeclsPart,
                    ConstDecl,
                TypeDeclsPart,
                    TypeDecl,
                VarDeclsPart,
                    VarDecl,
                ProcFuncDeclsPart,
                    ProcDecl,
                        ProcHeader,
                    FuncDecl,
                        FuncHeader,
        //Types
            SimpleType,
            ArrayType,
                IndexRange,
            RecordType,
                FieldSection,
            PointerType,
            ProcSignature,
            FuncSignature,
        //Other
        FormalParamSection,
            Modifier,
        IdentList,
            Ident,
        ConstIntegerLiteral,
        ConstDoubleLiteral,
        BinOperation,
        // @formatter:on
    }

    public abstract class AstNode
    {
        protected AstNode(AstNodeType type, Token token = null)
        {
            Token = token;
            Type = type;
            if (token != null)
            {
                Value = token.Value;
            }
            else
            {
                Value = type.ToString();
            }
        }

        public override string ToString() => Value;

        public abstract T Accept<T>(IAstVisitor<T> visitor);

        public Token Token { get; protected set; }
        public AstNodeType Type { get; }
        protected string Value { get; set; }
        protected List<AstNode> _children = new List<AstNode>();
    }

    public class Program : AstNode
    {
        public Program(AstNode header, AstNode mainBlock) : base(AstNodeType.Program, header.Token)
        {
            _children.Add(header);
            _children.Add(mainBlock);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Header => _children[0];
        public AstNode MainBlock => (MainBlock) _children[1];
    }

    public class MainBlock : AstNode
    {
        public MainBlock(List<AstNode> declParts, AstNode mainCompound) : base(AstNodeType.MainBlock)
        {
            _children.InsertRange(0, declParts);
            _children.Add(mainCompound);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> DeclsParts => _children.GetRange(0, _children.Count - 1);
        public AstNode MainCompound => _children[_children.Count - 1];
    }

    public class ConstIntegerLiteral : AstNode
    {
        public ConstIntegerLiteral(IntegerNumberToken token) : base(AstNodeType.ConstIntegerLiteral, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ConstDoubleLiteral : AstNode
    {
        public ConstDoubleLiteral(DoubleNumberToken token) : base(AstNodeType.ConstDoubleLiteral)
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

    public class BinOperation : AstNode
    {
        public BinOperation(Token operation, AstNode left, AstNode right) : this(left, right)
        {
            Operation = operation;
//            Value = string.Format("{0} {1} {2}", left.ToString(), operation.Value, right.ToString());
            Value = operation.Value;
        }

        protected BinOperation(AstNode left, AstNode right) : base(AstNodeType.BinOperation)
        {
            _children.Add(left);
            _children.Add(right);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token Operation { get; }
        public AstNode Left => _children[0];
        public AstNode Right => _children[1];
    }

    public class AssignStatement : BinOperation
    {
        public AssignStatement(AssignToken operation, AstNode left, AstNode right) : base(left, right)
        {
            Operation = operation;
//            Value = string.Format("{0} {1} {2}", left.ToString(), operation.Value, right.ToString());
            Value = operation.Value;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public new AssignToken Operation { get; }
    }
}