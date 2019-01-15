using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstVisitor;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.AstNodes
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
        public TypeDecl(AstNode ident, AstNode identType) : base(AstNodeType.TypeDecl)
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

    public class VarDeclsPart : DeclsPart
    {
        public VarDeclsPart(Token token, List<AstNode> decls) : base(decls, AstNodeType.VarDeclsPart, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class SimpleVarDecl : AstNode
    {
        public SimpleVarDecl(List<AstNode> identList, AstNode identsType)
            : base(AstNodeType.SimpleVarDecl)
        {
            _children.InsertRange(0, identList);
            _children.Add(identsType);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> IdentList => _children.GetRange(0, _children.Count - 1);

        public AstNode IdentsType => _children[_children.Count - 1];
    }

    public class InitVarDecl : AstNode
    {
        public InitVarDecl(AstNode ident, AstNode identsType, AstNode expression)
            : base(AstNodeType.InitVarDecl)
        {
            _children.Add(ident);
            _children.Add(identsType);
            _children.Add(expression);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode IdentList => _children[0];
        public AstNode IdentsType => _children[1];
        public AstNode Expression => _children[2];
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

    public class ProcHeader : AstNode
    {
        public ProcHeader(Token token, AstNode name, List<AstNode> paramList) : base(AstNodeType.ProcHeader, token)
        {
            _children.Add(name);
            _children.InsertRange(1, paramList);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Name => _children[0];
        public List<AstNode> ParamList => _children.GetRange(1, _children.Count - 1);
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
    
    public class FuncHeader : AstNode
    {
        public FuncHeader(Token token, AstNode name, List<AstNode> paramList, AstNode returnType) 
            : base(AstNodeType.FuncHeader, token)
        {
            _children.Add(name);
            _children.InsertRange(1, paramList);
            _children.Add(returnType);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Name => _children[0];
        public List<AstNode> ParamList => _children.GetRange(1, _children.Count - 2);
        public AstNode ReturnType => _children[_children.Count - 1];
    }
}