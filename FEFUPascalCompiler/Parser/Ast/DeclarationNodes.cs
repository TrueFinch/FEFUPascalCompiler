using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstVisitor;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser
{
    public abstract class DeclsPart : AstNode
    {
        protected DeclsPart(Token token, List<AstNode> decls, AstNodeType type) : base(type)
        {
            Token = token;
            _children.InsertRange(0, decls);
            Value = token.Value;
        }
        public Token Token { get; }
        public List<AstNode> Decls => _children;
    }

    public class ConstDeclsPart : DeclsPart
    {
        public ConstDeclsPart(Token token, List<AstNode> constDecls) : base(token, constDecls, AstNodeType.ConstDeclsPart)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ConstDecl : AstNode
    {
        public ConstDecl(Token token, AstNode ident, AstNode expression) : base(AstNodeType.ConstDecl)
        {
            Value = token.Value;
            _children.Add(ident);
            _children.Add(expression);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public AstNode ConstIdent => _children[0];
        public AstNode Expression => _children[1];
    }

    public class VariableDeclsPart : DeclsPart
    {
        public VariableDeclsPart(Token token, List<AstNode> decls) : base(decls, AstNodeType.VarDeclsPart)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}