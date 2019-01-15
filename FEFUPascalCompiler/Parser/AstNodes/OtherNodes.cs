using System.Collections.Generic;
using System.Globalization;
using FEFUPascalCompiler.Parser.AstVisitor;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.AstNodes
{
    public class FormalParamSection : AstNode
    {
        public FormalParamSection(AstNode paramList, AstNode paramType, AstNode modifier = null) : base(AstNodeType.FormalParamSection)
        {
            _children.Add(paramList);
            _children.Add(paramType);
            _children.Add(modifier);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        public AstNode ParamList => _children[0];
        public AstNode ParamType => _children[1];
        public AstNode ParamModifier => _children[2];
    }

    public class Modifier : AstNode
    {
        public Modifier(Token token) : base(AstNodeType.Modifier, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
    
//    public class IdentList : AstNode
//    {
//        public IdentList(List<AstNode> identList) : base(AstNodeType.IdentList)
//        {
//            _children.InsertRange(0, identList);
//        }
//
//        public override T Accept<T>(IAstVisitor<T> visitor)
//        {
//            return visitor.Visit(this);
//        }
//
//        public List<AstNode> Idents => _children;
//    }

    public class Ident : AstNode
    {
        public Ident(Token token) : this(token, AstNodeType.Ident)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        protected Ident(Token token, AstNodeType type) : base(type, token)
        {
        }
    }
    
    public class ConstIntegerLiteral : AstNode
    {
        public ConstIntegerLiteral(Token token) : base(AstNodeType.ConstIntegerLiteral, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ConstDoubleLiteral : AstNode
    {
        public ConstDoubleLiteral(Token token) : base(AstNodeType.ConstDoubleLiteral, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ConstCharLiteral : AstNode
    {
        public ConstCharLiteral(Token token) : base(AstNodeType.ConstCharLiteral, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ConstStringLiteral : AstNode
    {
        public ConstStringLiteral(Token token) : base(AstNodeType.ConstStringLiteral, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
    
    public class Nil : AstNode
    {
        public Nil(Token token) : base(AstNodeType.Nil, token)
        {
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}