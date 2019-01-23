using System.Collections.Generic;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Parser.Visitors;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.AstNodes
{
    public abstract class Expression : AstNode
    {
        protected Expression(AstNodeType nodeType, Token token = null) : base(nodeType, token)
        {
        }
        
        public SymType SymType { get; set; }
        public bool IsLValue { get; set; }= false;
    }

    public class Cast : Expression
    {
        public Cast(AstNode expressionToCast) : base(AstNodeType.Cast)
        {
            ExprToCast = expressionToCast;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode ExprToCast { get; set; }
    }
    
    public abstract class BinOperator : Expression
    {
        protected BinOperator(AstNodeType nodeType, Token token, Expression left, Expression right) : base(nodeType, token)
        {
            Left = left;
            Right = right;
        }

        public Expression Left { get; set; }
        public Expression Right { get; set; }
    }
    
    public class ComparingOperator : BinOperator
    {
        public ComparingOperator(Token token, Expression left, Expression right) 
            : base(AstNodeType.ComparingOperator, token, left, right)
        {
        }
        
        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
    
    public class AdditiveOperator : BinOperator
    {
        public AdditiveOperator(Token token, Expression left, Expression right)
            : base(AstNodeType.AdditiveOperator, token, left, right)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class MultiplyingOperator : BinOperator{
        public MultiplyingOperator(Token token, Expression left, Expression right)
            : base(AstNodeType.MultiplyingOperator, token, left, right)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
    
    public class UnaryOperator : Expression {
        public UnaryOperator(Token token, Expression expr)
            : base(AstNodeType.UnaryOperator, token)
        {
            Expr = expr;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Expression Expr { get; set; }
    }
    
    public class ArrayAccess : Expression
    {
        public ArrayAccess(Expression arrayIdent, List<Expression> accessExpressions) : base(AstNodeType.ArrayAccess)
        {
            Array = arrayIdent;
            AccessExpr = accessExpressions;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public Expression Array { get; set; }
        public List<Expression> AccessExpr { get; set; }
    }
    
    public class RecordAccess : Expression {
        public RecordAccess(AstNode recordIdent, AstNode field) : base(AstNodeType.RecordAccess)
        {
            RecordIdent = recordIdent;
            Field = field;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode RecordIdent { get; set; }
        public AstNode Field { get; set; }
    }
    
    public class FunctionCall : Expression {
        public FunctionCall(Expression funcIdent, List<Expression> paramList) : base(AstNodeType.FunctionCall)
        {
            FuncIdent = funcIdent;
            ParamList = paramList;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public Expression FuncIdent { get; set; }
        public List<Expression> ParamList { get; set; }
    }
    
    public class DereferenceOperator : Expression
    {
        public DereferenceOperator(Token token, Expression leftExpr) : base(AstNodeType.DereferenceOperator, token)
        {
            Expr = leftExpr;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public Expression Expr { get; set; }
    }
    
    public class Ident : Expression
    {
        public Ident(Token token) : this(token, AstNodeType.Ident)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        protected Ident(Token token, AstNodeType nodeType) : base(nodeType, token)
        {
        }
        
        public SymVar SymVar { get; set; }
    }
    
    public class ConstIntegerLiteral : Expression
    {
        public ConstIntegerLiteral(Token token) : base(AstNodeType.ConstIntegerLiteral, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ConstFloatLiteral : Expression
    {
        public ConstFloatLiteral(Token token) : base(AstNodeType.ConstDoubleLiteral, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ConstCharLiteral : Expression
    {
        public ConstCharLiteral(Token token) : base(AstNodeType.ConstCharLiteral, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ConstStringLiteral : Expression
    {
        public ConstStringLiteral(Token token) : base(AstNodeType.ConstStringLiteral, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class Nil : Expression
    {
        public Nil(Token token) : base(AstNodeType.Nil, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class BooleanLiteral : Expression
    {
        public BooleanLiteral(Token token) : base(AstNodeType.BooleanLiteral, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}