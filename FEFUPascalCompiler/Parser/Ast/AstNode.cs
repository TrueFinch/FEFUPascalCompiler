using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FEFUPascalCompiler.Parser.AstVisitor;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser
{
    public enum AstNodeType
    {
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
        FuncDecl,
        ArrayType,
        IndexRange,
        Ident,
        ConstIntegerLiteral,
        ConstDoubleLiteral,
        BinOperation,
    }

    public abstract class AstNode
    {
        protected AstNode(AstNodeType type)
        {
            Type = type;
        }

        public override string ToString() => Value;

        public abstract T Accept<T>(IAstVisitor<T> visitor);

        public AstNodeType Type { get; }
        protected string Value { get; set; }
        protected List<AstNode> _children = new List<AstNode>();
    }

    public abstract class DeclsPart : AstNode
    {
        protected DeclsPart(List<AstNode> decls, AstNodeType type) : base(type)
        {
            _children.InsertRange(0, decls);
            Value = Type.ToString();
        }
    }

    public class Program : AstNode
    {
        public Program(AstNode header, AstNode mainBlock) : base(AstNodeType.Program)
        {
            Value = header.ToString();
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
            Value = Type.ToString();
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> DeclParts => _children.GetRange(0, _children.Count - 1);
        public AstNode MainCompound => _children[_children.Count - 1];
    }


    public class ConstDeclsPart : DeclsPart
    {
        public ConstDeclsPart(List<AstNode> constDecls) : base(constDecls, AstNodeType.ConstDeclsPart)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ConstDecl : AstNode
    {
        public ConstDecl(AstNode ident, AstNode expression) : base(AstNodeType.ConstDecl)
        {
            _children.Add(ident);
            _children.Add(expression);
            Value = "=";
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode ConstIdent => _children[0];
        public AstNode Expression => _children[1];
    }

    public class Ident : AstNode
    {
        public Ident(Token token) : this(token, AstNodeType.Ident)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        protected Ident(Token token, AstNodeType type) : base(type)
        {
            Token = token;
            Value = token.Value;
        }

        public Token Token { get; }
    }

    public class ConstIntegerLiteral : AstNode
    {
        public ConstIntegerLiteral(IntegerNumberToken token) : base(AstNodeType.ConstIntegerLiteral)
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