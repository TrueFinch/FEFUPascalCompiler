using System.Collections.Generic;
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
        
        public Type SymType { get; set; }
        
        public abstract T Accept<T>(ISymVisitor<T> visitor);
    }
    
    public abstract class BinOperator : Expression
    {
        public BinOperator(AstNodeType nodeType, Token token, AstNode left, AstNode right) : base(nodeType, token)
        {
            _children.Add(left);
            _children.Add(right);
        }

        public AstNode Left => _children[0];
        public AstNode Right => _children[1];
    }
    
    public class ComparingOperator : BinOperator
    {
        public ComparingOperator(Token token, AstNode left, AstNode right) 
            : base(AstNodeType.ComparingOperator, token, left, right)
        {
        }
        
        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override T Accept<T>(ISymVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

//    public class SimpleExpretion : Operand
//    {
//        public SimpleExpretion(AstNode additiveOp) : base(AstNodeType.SimpleExpression, additiveOp)
//        {
//        }
//
//        public override T Accept<T>(IAstVisitor<T> visitor)
//        {
//            return visitor.Visit(this);
//        }
//    }
    
    public class AdditiveOperator : BinOperator
    {
        public AdditiveOperator(Token token, AstNode left, AstNode right)
            : base(AstNodeType.AdditiveOperator, token, left, right)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override T Accept<T>(ISymVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
    
//    public class Term: Operand
//    {
//        public Term(AstNode multiplyingOperator) : base(AstNodeType.Term, multiplyingOperator)
//        {
//        }
//
//        public override T Accept<T>(IAstVisitor<T> visitor)
//        {
//            return visitor.Visit(this);
//        }
//    }

    public class MultiplyingOperator : BinOperator{
        public MultiplyingOperator(Token token, AstNode left, AstNode right)
            : base(AstNodeType.MultiplyingOperator, token, left, right)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override T Accept<T>(ISymVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
    
    public class UnaryOperator : Expression {
        public UnaryOperator(Token token, AstNode right)
            : base(AstNodeType.UnaryOperator, token)
        {
            _children.Add(right);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        
        public override T Accept<T>(ISymVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Right => _children[0];
    }
    
    public class ArrayAccess : Expression
    {
        public ArrayAccess(AstNode arrayIdent, List<AstNode> accessExpressions) : base(AstNodeType.ArrayAccess)
        {
            _children.Add(arrayIdent);
            _children.InsertRange(1, accessExpressions);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public override T Accept<T>(ISymVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Array => _children[0];
        public List<AstNode> AccessExpr => _children.GetRange(1, _children.Count - 1);
    }
    
    public class RecordAccess : Expression {
        public RecordAccess(AstNode recordIdent, AstNode field) : base(AstNodeType.RecordAccess)
        {
            _children.Add(recordIdent);
            _children.Add(field);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public override T Accept<T>(ISymVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode RecordIdent => _children[0];
        public AstNode Field => _children[1];
    }
    
    public class FunctionCall : Expression {
        public FunctionCall(AstNode funcIdent, List<AstNode> paramList) : base(AstNodeType.FunctionCall)
        {
            _children.Add(funcIdent);
            _children.InsertRange(1, paramList);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override T Accept<T>(ISymVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public AstNode FuncIdent => _children[0];
        public List<AstNode> ParamList => _children.GetRange(1, _children.Count - 1);
    }
    
    public class DereferenceOperator : Expression
    {
        public DereferenceOperator(Token token, AstNode leftExpr) : base(AstNodeType.DereferenceOperator, token)
        {
            _children.Add(leftExpr);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public override T Accept<T>(ISymVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public AstNode Expr => _children[0];
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

        public override T Accept<T>(ISymVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        protected Ident(Token token, AstNodeType nodeType) : base(nodeType, token)
        {
        }
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

        public override T Accept<T>(ISymVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ConstDoubleLiteral : Expression
    {
        public ConstDoubleLiteral(Token token) : base(AstNodeType.ConstDoubleLiteral, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override T Accept<T>(ISymVisitor<T> visitor)
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

        public override T Accept<T>(ISymVisitor<T> visitor)
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

        public override T Accept<T>(ISymVisitor<T> visitor)
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

        public override T Accept<T>(ISymVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}