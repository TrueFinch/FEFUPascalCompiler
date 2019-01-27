using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Visitors;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.AstNodes
{
    public abstract class DeclsPart : AstNode
    {
        protected DeclsPart(List<AstNode> decls, AstNodeType nodeType, Token token = null) : base(nodeType, token)
        {
            Decls = decls;
        }

        public List<AstNode> Decls { get; set; }
    }
}

public class ConstDeclsPart : DeclsPart
{
    public ConstDeclsPart(Token token, List<AstNode> constDecls)
        : base(constDecls, AstNodeType.ConstDeclsPart, token)
    {
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class ConstDecl : AstNode
{
    public ConstDecl(Token token, AstNode ident, AstNode expression) : base(AstNodeType.ConstDecl, token)
    {
        Ident = ident;
        Expression = expression;
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public AstNode Ident { get; set; }
    public AstNode Expression { get; set; }
}

public class TypeDeclsPart : DeclsPart
{
    public TypeDeclsPart(Token token, List<AstNode> decls) : base(decls, AstNodeType.TypeDeclsPart, token)
    {
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class TypeDecl : AstNode
{
    public TypeDecl(Ident ident, AstNode identType) : base(AstNodeType.TypeDecl)
    {
        Ident = ident;
        IdentType = identType;
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public Ident Ident { get; set; }
    public AstNode IdentType { get; set; }
}

public class VarDeclsPart : DeclsPart
{
    public VarDeclsPart(Token token, List<AstNode> decls) : base(decls, AstNodeType.VarDeclsPart, token)
    {
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class SimpleVarDecl : AstNode
{
    public SimpleVarDecl(List<Ident> identList, AstNode identsType)
        : base(AstNodeType.SimpleVarDecl)
    {
        IdentList = identList;
        IdentsType = identsType;
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public List<Ident> IdentList { get; set; }
    public AstNode IdentsType { get; set; }
}

public class InitVarDecl : AstNode
{
    public InitVarDecl(AstNode ident, AstNode identsType, AstNode expression)
        : base(AstNodeType.InitVarDecl)
    {
        Ident = ident;
        IdentType = identsType;
        Expression = expression;
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public AstNode Ident { get; set; }
    public AstNode IdentType { get; set; }
    public AstNode Expression { get; set; }
}


//public class ProcFuncDeclsPart : DeclsPart
//{
//    public ProcFuncDeclsPart(List<AstNode> decls) : base(decls, AstNodeType.ProcFuncDeclsPart)
//    {
//    }
//
//    public override T Accept<T>(IAstVisitor<T> visitor)
//    {
//        return visitor.Visit(this);
//    }
//}

public class CallableDeclNode : AstNode
{
    public CallableDeclNode(CallableHeader header, AstNode block = null) : base(AstNodeType.ProcDecl)
    {
        Header = header;
        Block = block;
    }
    
    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
    
    public CallableHeader Header { get; set; }
    public AstNode Block { get; set; }
}

public class CallableHeader : AstNode
{
    public CallableHeader(AstNode name, List<AstNode> paramList, AstNode returnType = null)
        : base(AstNodeType.FuncHeader)
    {
        Name = name;
        ParamList = paramList;
        ReturnType = returnType;
    }
    
    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
    
    public AstNode Name {get; set;}
    public List<AstNode> ParamList {get; set;}
    public AstNode ReturnType {get; set;}
}

//public class ProcDecl : AstNode
//{
//    public ProcDecl(AstNode procHeader, AstNode block) : base(AstNodeType.ProcDecl)
//    {
//        ProcHeader = procHeader;
//        Block = block;
//    }
//
//    public override T Accept<T>(IAstVisitor<T> visitor)
//    {
//        return visitor.Visit(this);
//    }
//
//    public AstNode ProcHeader { get; set; }
//    public AstNode Block { get; set; }
//}
//
//public class ProcHeader : AstNode
//{
//    public ProcHeader(AstNode name, List<AstNode> paramList) : base(AstNodeType.ProcHeader)
//    {
//        Name = name;
//        ParamList = paramList;
//    }
//
//    public override T Accept<T>(IAstVisitor<T> visitor)
//    {
//        return visitor.Visit(this);
//    }
//
//    public AstNode Name { get; set; }
//    public List<AstNode> ParamList { get; set; }
//}
//
//public class FuncDecl : AstNode
//{
//    public FuncDecl(AstNode funcHeader, AstNode block) : base(AstNodeType.FuncDecl)
//    {
//        FuncHeader = funcHeader;
//        Block = block;
//    }
//
//    public override T Accept<T>(IAstVisitor<T> visitor)
//    {
//        return visitor.Visit(this);
//    }
//
//    public AstNode FuncHeader { get; set; }
//    public AstNode Block { get; set; }
//}
//
//public class FuncHeader : AstNode
//{
//    public FuncHeader(AstNode name, List<AstNode> paramList, AstNode returnType)
//        : base(AstNodeType.FuncHeader)
//    {
//        Name = name;
//        ParamList = paramList;
//        ReturnType = returnType;
//    }
//
//    public override T Accept<T>(IAstVisitor<T> visitor)
//    {
//        return visitor.Visit(this);
//    }
//
//    public AstNode Name {get; set;}
//    public List<AstNode> ParamList {get; set;}
//    public AstNode ReturnType {get; set;}
//}

public class SubroutineBlock : AstNode
{
    public SubroutineBlock(List<AstNode> declParts, AstNode compoundStatement)
        : base(AstNodeType.SubroutineBlock)
    {
        DeclParts = declParts;
        CompoundStatement = compoundStatement;
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public List<AstNode> DeclParts {get; set;}
    public AstNode CompoundStatement {get; set;}
}

public class Forward : AstNode
{
    public Forward(Token token) : base(AstNodeType.Forward, token)
    {
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

