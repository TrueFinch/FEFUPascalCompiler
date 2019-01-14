using System.Collections.Generic;
using System.Linq;
using FEFUPascalCompiler.Parser.AstVisitor;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser
{
    public abstract class DeclsPart : AstNode
    {
        protected DeclsPart(List<AstNode> decls, AstNodeType type, Token token = null) : base(type, token)
        {
            _children.InsertRange(0, decls);
        }

        public List<AstNode> Decls => _children;
    }

    public class ConstDeclsPart : DeclsPart
    {
        public ConstDeclsPart(Token token, List<AstNode> constDecls)
            : base(constDecls, AstNodeType.ConstDeclsPart, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ConstDecl : AstNode
    {
        public ConstDecl(Token token, AstNode ident, AstNode expression) : base(AstNodeType.ConstDecl, token)
        {
            _children.Add(ident);
            _children.Add(expression);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Ident => _children[0];
        public AstNode Expression => _children[1];
    }

    public class TypeDeclsPart : DeclsPart
    {
        public TypeDeclsPart(Token token, List<AstNode> decls) : base(decls, AstNodeType.TypeDeclsPart, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class TypeDecl : AstNode
    {
        public TypeDecl(Token token, AstNode ident, AstNode identType) : base(AstNodeType.TypeDecl, token)
        {
            _children.Add(ident);
            _children.Add(identType);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Ident => _children[0];
        public AstNode IdentType => _children[1];
    }

    public class VariableDeclsPart : DeclsPart
    {
        public VariableDeclsPart(Token token, List<AstNode> decls) : base(decls, AstNodeType.VarDeclsPart, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class VarDecl : AstNode
    {
        public VarDecl(Token token, List<AstNode> identList, AstNode identsType, AstNode expression = null)
            : base(AstNodeType.VarDecl, token)
        {
            _children.InsertRange(0, identList);
            _children.Add(identsType);
            if (expression != null)
            {
                _children.Add(expression);
            }
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        private bool _hasExpression = false;

        public List<AstNode> IdentList => _hasExpression
            ? _children.GetRange(0, _children.Count - 2)
            : _children.GetRange(0, _children.Count - 1);

        public AstNode IdentsType => _hasExpression ? _children[_children.Count - 2] : _children[_children.Count - 1];
        public AstNode Expression => _hasExpression ? _children[_children.Count - 1] : null;
    }

    public class ProcFuncDeclsPart : DeclsPart
    {
        public ProcFuncDeclsPart(List<AstNode> decls) : base(decls, AstNodeType.ProcFuncDeclsPart)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
    
    public class ProcDecl : AstNode
    {
        public ProcDecl(Token token, AstNode procHeader, AstNode block) : base(AstNodeType.ProcDecl, token)
        {
            _children.Add(procHeader);
            _children.Add(block);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode ProcHeader => _children[0];
        public AstNode Block => _children[1];
    }
    
    public class FuncDecl : AstNode
    {
        public FuncDecl(Token token, AstNode funcHeader, AstNode block) : base(AstNodeType.FuncDecl, token)
        {
            _children.Add(funcHeader);
            _children.Add(block);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode FuncHeader => _children[0];
        public AstNode Block => _children[1];
    }
}