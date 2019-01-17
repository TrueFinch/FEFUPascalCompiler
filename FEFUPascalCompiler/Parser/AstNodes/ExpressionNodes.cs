using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstVisitor;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.AstNodes
{
    public abstract class BinOperator : AstNode
    {
        public BinOperator(AstNodeType type, Token token, AstNode left, AstNode right) : base(type, token)
        {
            _children.Add(left);
            _children.Add(right);
        }

        public AstNode Left => _children[0];
        public AstNode Right => _children[1];
    }

//    public abstract class Operand : AstNode
//    {
//        protected Operand(AstNodeType type, AstNode binOperator) : base(type)
//        {
//            _children.Add(binOperator);
//        }
//
//        public AstNode Operator => _children[0];
//    }
//    
//    public class Expression : Operand {
//        public Expression(AstNode comparingBinOperator) : base(AstNodeType.Expression, comparingBinOperator)
//        {
//        }
//
//        public override T Accept<T>(IAstVisitor<T> visitor)
//        {
//            return visitor.Visit(this);
//        }
//    }
    
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
    }
    
    public class UnaryOperator : AstNode{
        public UnaryOperator(Token token, AstNode right)
            : base(AstNodeType.UnaryOperator, token)
        {
            _children.Add(right);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Right => _children[0];
    }
    
    public class ArrayAccess : AstNode
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

        public AstNode Array => _children[0];
        public List<AstNode> AccessExpr => _children.GetRange(1, _children.Count - 1);
    }
    
    public class RecordAccess : AstNode {
        public RecordAccess(AstNode recordIdent, AstNode field) : base(AstNodeType.RecordAccess)
        {
            _children.Add(recordIdent);
            _children.Add(field);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode RecordIdent => _children[0];
        public AstNode Field => _children[1];
    }
    
    public class FunctionCall : AstNode{
        public FunctionCall(AstNode funcIdent, List<AstNode> paramList) : base(AstNodeType.FunctionCall)
        {
            _children.Add(funcIdent);
            _children.InsertRange(1, paramList);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode FuncIdent => _children[0];
        public List<AstNode> ParamList => _children.GetRange(1, _children.Count - 1);
    }
    
    public class DereferenceOperator : AstNode
    {
        public DereferenceOperator(Token token, AstNode leftExpr) : base(AstNodeType.DereferenceOperator, token)
        {
            _children.Add(leftExpr);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public AstNode Expr => _children[0];
    }
}