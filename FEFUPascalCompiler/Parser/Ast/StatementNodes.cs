using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstVisitor;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser
{
    public class CompoundStatement : AstNode
    {
        public CompoundStatement(Token beginToken, Token endToken, List<AstNode> statements) 
            : base(AstNodeType.CompoundStatement)
        {
            BeginToken = beginToken;
            EndToken = endToken;
            _children.InsertRange(0, statements);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public Token BeginToken { get; }
        public Token EndToken { get; }
        public List<AstNode> Statements => _children;
    }

    public class EmptyStatement : AstNode
    {
        public EmptyStatement(Token token) : base(AstNodeType.EmptyStatement, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
    
    
}