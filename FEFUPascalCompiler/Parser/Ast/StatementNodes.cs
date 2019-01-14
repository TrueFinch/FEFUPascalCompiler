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
    
    public class AssignStatement : AstNode
    {
        public AssignStatement(Token assignToken, AstNode left, AstNode right) 
            : base(AstNodeType.AssignmentStatement, assignToken)
        {
            _children.Add(left);
            _children.Add(right);
//            Value = string.Format("{0} {1} {2}", left.ToString(), operation.Value, right.ToString());
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Left => _children[0];
        public AstNode Right => _children[1];
    }

    public class IfStatement : AstNode
    {
        public IfStatement(Token ifToken, AstNode expression, Token thenToken, AstNode thenStatement, 
                           Token elseToken = null, AstNode elseStatement = null) : base(AstNodeType.IfStatement)
        {
            IfToken = ifToken;
            _children.Add(expression);
            ThenToken = thenToken;
            _children.Add(thenStatement);
            ElseToken = elseToken;
            _children.Add(elseStatement);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public Token IfToken { get; }
        public Token ThenToken { get; }
        public Token ElseToken { get; }
        public AstNode Expression => _children[0];
        public AstNode ThenStatement => _children[1];
        public AstNode ElseStatement => _children[2];
    }

}