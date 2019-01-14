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

    public class WhileStatement : AstNode
    {
        public WhileStatement(Token whileToken, AstNode expression, Token doToken, AstNode statement)
            : this(whileToken, expression, doToken, statement, AstNodeType.WhileStatement)
        {
        }

        protected WhileStatement(Token whileToken, AstNode expression, Token doToken, AstNode statement,
            AstNodeType type)
            : base(type)
        {
            WhileToken = whileToken;
            DoToken = doToken;
            _children.Add(expression);
            _children.Add(statement);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token WhileToken { get; }
        public Token DoToken { get; }
        public AstNode Expression => _children[0];
        public AstNode Statement => _children[1];
    }

    public class DoWhileStatement : WhileStatement
    {
        public DoWhileStatement(Token whileToken, AstNode expression, Token doToken, AstNode statement)
            : base(whileToken, expression, doToken, statement, AstNodeType.DoWhileStatement)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ForStatement : AstNode
    {
        public ForStatement(Token forToken, AstNode iterator, Token assignToken, AstNode range,
            Token doToken, AstNode expression)
            : base(AstNodeType.ForStatement)
        {
            ForToken = forToken;
            _children.Add(iterator);
            AssignToken = assignToken;
            _children.Add(range);
            DoToken = doToken;
            _children.Add(expression);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token ForToken { get; }
        public Token AssignToken { get; }
        public Token DoToken { get; }

        public AstNode Iterator => _children[0];
        public AstNode Range => _children[1];
        public AstNode Expression => _children[2];
    }

    public class ForRange : AstNode
    {
        public ForRange(Token token, AstNode start, AstNode finish) : base(AstNodeType.ForRange, token)
        {
            _children.Add(start);
            _children.Add(finish);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public AstNode Start => _children[0];
        public AstNode Finish => _children[1];
    }
}