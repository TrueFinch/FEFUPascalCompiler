using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Semantics;
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
    public ConstDecl(Token token, AstNode ident, Expression expression, bool isLocal) : base(AstNodeType.ConstDecl,
        token)
    {
        Ident = ident;
        Expression = expression;
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public bool IsLocal;
    public AstNode Ident { get; set; }
    public Expression Expression { get; set; }
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
    public VarDeclsPart(Token token, List<AstNode> decls, bool isLocal) : base(decls, AstNodeType.VarDeclsPart, token)
    {
        IsLocal = isLocal;
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public bool IsLocal;
}

public class SimpleVarDecl : AstNode
{
    public SimpleVarDecl(List<Ident> identList, AstNode identsType, bool isLocal)
        : base(AstNodeType.SimpleVarDecl)
    {
        IsLocal = isLocal;
        IdentList = identList;
        IdentsType = identsType;
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public bool IsLocal;
    public List<Ident> IdentList { get; set; }
    public AstNode IdentsType { get; set; }
}

public class CallableDeclNode : AstNode
{
    public CallableDeclNode(CallableHeader header, AstNode block = null) : base(header.ReturnType == null
        ? AstNodeType.ProcDecl
        : AstNodeType.FuncDecl)
    {
        Header = header;
        Block = block;
        if (block == null)
        {
            IsForward = true;
        }
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public bool IsForward { get; }
    public CallableHeader Header { get; }
    public AstNode Block { get; }
}

public class CallableHeader : AstNode
{
    public CallableHeader(Ident name, List<FormalParamSection> paramList, AstNode returnType = null)
        : base(AstNodeType.CallableHeader)
    {
        Name = name;
        ParamList = paramList;
        ReturnType = returnType;
    }

    public override T Accept<T>(IAstVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }

    public string GetSignature()
    {
        var signature = Name.ToString() + "(";
        for (int i = 0; i < CallableSymbol.ParametersTypes.Count - 1; ++i)
        {
            signature += CallableSymbol.ParametersTypes[i].ToString() + ", ";
        }

        if (CallableSymbol.ParametersTypes.Count > 0)
        {
            signature += CallableSymbol.ParametersTypes[CallableSymbol.ParametersTypes.Count - 1].ToString();
        }

        signature += ");";
        return signature;
    }

    public CallableSymbol CallableSymbol { get; set; }
    public Ident Name { get; set; }
    public List<FormalParamSection> ParamList { get; set; }
    public AstNode ReturnType { get; set; }
}

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

    public List<AstNode> DeclParts { get; set; }
    public AstNode CompoundStatement { get; set; }
}