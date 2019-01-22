using System.Collections.Generic;
using FEFUPascalCompiler.Parser.Visitors;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.AstNodes
{
    public abstract class Statement : AstNode
    {
        protected Statement(AstNodeType nodeType, Token token = null) : base(nodeType, token)
        {
        }
    }

    public class CompoundStatement : Statement
    {
        public CompoundStatement(Token beginToken, Token endToken, List<AstNode> statements)
            : base(AstNodeType.CompoundStatement)
        {
            BeginToken = beginToken;
            EndToken = endToken;
        Statements =  statements;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token BeginToken { get; }
        public Token EndToken { get; }
        public List<AstNode> Statements { get; set; }
    }

    public class EmptyStatement : Statement
    {
        public EmptyStatement(Token token) : base(AstNodeType.EmptyStatement, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class AssignStatement : Statement
    {
        public AssignStatement(Token assignToken, Expression left, Expression right)
            : base(AstNodeType.AssignmentStatement, assignToken)
        {
            Left = left;
            Right = right;
//            Value = string.Format("{0} {1} {2}", left.ToString(), operation.Value, right.ToString());
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Expression Left { get; set; }
        public Expression Right { get; set; }
    }

    public class IfStatement : Statement
    {
        public IfStatement(Token ifToken, AstNode expression, Token thenToken, AstNode thenStatement,
            Token elseToken = null, AstNode elseStatement = null) : base(AstNodeType.IfStatement)
        {
            IfToken = ifToken;
            Expression = expression;
            ThenToken = thenToken;
            ThenStatement = thenStatement;
            ElseToken = elseToken;
            ElseStatement = elseStatement;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token IfToken { get; }
        public Token ThenToken { get; }
        public Token ElseToken { get; }
        public AstNode Expression { get; set; }
        public AstNode ThenStatement { get; set; }
        public AstNode ElseStatement { get; set; }
    }

    public class WhileStatement : Statement
    {
        public WhileStatement(Token whileToken, AstNode expression, Token doToken, AstNode statement)
            : this(whileToken, expression, doToken, statement, AstNodeType.WhileStatement)
        {
        }

        protected WhileStatement(Token whileToken, AstNode expression, Token doToken, AstNode statement,
            AstNodeType nodeType)
            : base(nodeType)
        {
            WhileToken = whileToken;
            DoToken = doToken;
            Expression = expression;
            Statement = statement;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token WhileToken { get; }
        public Token DoToken { get; }
        public AstNode Expression { get; set; }
        public AstNode Statement { get; set; }
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

    public class ForStatement : Statement
    {
        public ForStatement(Token forToken, AstNode iterator, Token assignToken, AstNode range,
            Token doToken, AstNode statement)
            : base(AstNodeType.ForStatement)
        {
            ForToken = forToken;
            Iterator = iterator;
            AssignToken = assignToken;
            Range = range;
            DoToken = doToken;
            Statement = statement;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Token ForToken { get; }
        public Token AssignToken { get; }
        public Token DoToken { get; }

        public AstNode Iterator { get; set; }
        public AstNode Range { get; set; }
        public AstNode Statement { get; set; }
    }

    public class ForRange : Statement
    {
        public ForRange(Token token, AstNode start, AstNode finish) : base(AstNodeType.ForRange, token)
        {
            Start = start;
            Finish = finish;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Start { get; set; }
        public AstNode Finish { get; set; }
    }
}