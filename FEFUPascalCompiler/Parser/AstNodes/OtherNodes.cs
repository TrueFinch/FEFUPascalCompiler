using System.Collections.Generic;
using System.Globalization;
using FEFUPascalCompiler.Parser.Visitors;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.AstNodes
{
    public class FormalParamSection : AstNode
    {
        public FormalParamSection(List<AstNode> paramList, AstNode paramType, AstNode modifier = null)
            : base(AstNodeType.FormalParamSection)
        {
            ParamList = paramList;
            ParamType = paramType;
            ParamModifier = modifier;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> ParamList { get; set; }
        public AstNode ParamType { get; set; }
        public AstNode ParamModifier { get; set; }
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
}