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
        public CompoundStatement(Token beginToken, Token endToken, List<Statement> statements)
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
    
    public class CallableCallStatement : Statement
    {
        public CallableCallStatement(FunctionCall callable) : base(AstNodeType.CallableCallStatement)
        {
            Callable = callable;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public Expression Callable { get; set; }
    }

    public class IfStatement : Statement
    {
        public IfStatement(Token ifToken, Expression expression, Token thenToken, Statement thenStatement,
            Token elseToken = null, Statement elseStatement = null) : base(AstNodeType.IfStatement)
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
        public Expression Expression { get; set; }
        public Statement ThenStatement { get; set; }
        public Statement ElseStatement { get; set; }
    }

    public class WhileStatement : Statement
    {
        public WhileStatement(Token whileToken, Expression expression, Token doToken, Statement statement)
            : this(whileToken, expression, doToken, statement, AstNodeType.WhileStatement)
        {
        }

        protected WhileStatement(Token whileToken, Expression expression, Token doToken, Statement statement,
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
        public Expression Expression { get; set; }
        public Statement Statement { get; set; }
    }

    public class DoWhileStatement : WhileStatement
    {
        public DoWhileStatement(Token whileToken, Expression expression, Token doToken, Statement statement)
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
        public ForStatement(Token forToken, Ident iterator, Token assignToken, ForRange range,
            Token doToken, Statement statement)
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

        public Ident Iterator { get; set; }
        public ForRange Range { get; set; }
        public Statement Statement { get; set; }
    }

    public class ForRange : Statement
    {
        public ForRange(Token token, Expression start, Expression finish) : base(AstNodeType.ForRange, token)
        {
            Start = start;
            Finish = finish;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Expression Start { get; set; }
        public Expression Finish { get; set; }
    }
}