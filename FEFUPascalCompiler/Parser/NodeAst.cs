using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FEFUPascalCompiler.Parser.AstVisitor;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser
{
    public enum NodeAstType
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
        Ident,
        ConstIntegerLiteral,
        ConstDoubleLiteral,
        BinOperation,
    }

    public abstract class NodeAst
    {
        protected NodeAst(NodeAstType type)
        {
            Type = type;
        }

        public override string ToString() => Value;

        public abstract T Accept<T>(IAstVisitor<T> visitor);

        public NodeAstType Type { get; }
        protected string Value { get; set; }
        protected List<NodeAst> _children = new List<NodeAst>();
    }

    public abstract class DeclsPart : NodeAst
    {
        protected DeclsPart(List<NodeAst> decls, NodeAstType type) : base(type)
        {
            _children.InsertRange(0, decls);
            Value = Type.ToString();
        }
    }

    public class Program : NodeAst
    {
        public Program(NodeAst header, NodeAst mainBlock) : base(NodeAstType.Program)
        {
            Value = header.ToString();
            _children.Add(header);
            _children.Add(mainBlock);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public MainBlock Header => (MainBlock) _children[0];
        public MainBlock MainBlock => (MainBlock) _children[1];
    }

    public class MainBlock : NodeAst
    {
        public MainBlock(List<NodeAst> declParts, NodeAst mainCompound) : base(NodeAstType.MainBlock)
        {
            _children.InsertRange(0, declParts);
            _children.Add(mainCompound);
            Value = Type.ToString();
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<NodeAst> DeclParts => _children.GetRange(0, _children.Count - 1);
        public NodeAst MainCompound => _children[_children.Count - 1];
    }


    public class ConstDeclsPart : DeclsPart
    {
        public ConstDeclsPart(List<NodeAst> constDecls) : base(constDecls, NodeAstType.ConstDeclsPart)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ConstDecl : NodeAst
    {
        public ConstDecl(NodeAst ident, NodeAst expression) : base(NodeAstType.ConstDecl)
        {
            _children.Add(ident);
            _children.Add(expression);
            Value = "=";
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public NodeAst Constant => _children[0];
        public NodeAst Expression => _children[1];
    }

    public class Ident : NodeAst
    {
        public Ident(Token token) : this(token, NodeAstType.Ident)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        protected Ident(Token token, NodeAstType type) : base(type)
        {
            Token = token;
            Value = token.Value;
        }

        public Token Token { get; }
    }

    public class ConstIntegerLiteral : NodeAst
    {
        public ConstIntegerLiteral(IntegerNumberToken token) : base(NodeAstType.ConstIntegerLiteral)
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
        public ConstDoubleLiteral(DoubleNumberToken token) : base(NodeAstType.ConstDoubleLiteral)
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
        public BinOperation(Token operation, NodeAst left, NodeAst right) : this(left, right)
        {
            Operation = operation;
//            Value = string.Format("{0} {1} {2}", left.ToString(), operation.Value, right.ToString());
            Value = operation.Value;
        }

        protected BinOperation(NodeAst left, NodeAst right) : base(NodeAstType.BinOperation)
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