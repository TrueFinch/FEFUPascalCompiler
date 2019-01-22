using System.Collections.Generic;
using System.Globalization;
using FEFUPascalCompiler.Parser.Visitors;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.AstNodes
{
    public enum AstNodeType
    {
        // @formatter:off
        Program,
            MainBlock,
                ConstDeclsPart,
                    ConstDecl,
                TypeDeclsPart,
                    TypeDecl,
                VarDeclsPart,
                    SimpleVarDecl,
                    InitVarDecl,
                ProcFuncDeclsPart,
                    ProcDecl,
                        ProcHeader,
                    FuncDecl,
                        FuncHeader,
                    SubroutineBlock,
                    Forward,
        //Types
            SimpleType,
            ArrayType,
                IndexRange,
            RecordType,
                FieldSection,
            PointerType,
            ProcSignature,
            FuncSignature,
        //Statements
        CompoundStatement,
            EmptyStatement,
            AssignmentStatement,
            IfStatement,
            WhileStatement,
            DoWhileStatement,
            ForStatement,
                ForRange,
        //Expressions
        BinOperator,
            ComparingOperator,
            AdditiveOperator,
            MultiplyingOperator,
            UnaryOperator,
        ArrayAccess,
        RecordAccess,
        FunctionCall,
        DereferenceOperator,
        Cast,
        //Other
        FormalParamSection,
            Modifier,
            ConformantArray,
        IdentList,
            Ident,
        ConstIntegerLiteral,
        ConstDoubleLiteral,
        ConstCharLiteral,
        ConstStringLiteral,
        Nil,
        // @formatter:on
    }

    public abstract class AstNode
    {
        protected AstNode(AstNodeType nodeType, Token token = null)
        {
            Token = token;
            NodeType = nodeType;
            if (token != null)
            {
                if (token.Type == TokenType.FloatNumber)
                {
                    Value = (token as DoubleNumberToken)?.NumberValue.ToString(new NumberFormatInfo
                        {NumberDecimalSeparator = "."});
                }
                else
                {
                    Value = token.Value;
                }
            }
            else
            {
                Value = nodeType.ToString();
            }
        }

        public override string ToString() => Value;

        public abstract T Accept<T>(IAstVisitor<T> visitor);

        public Token Token { get; protected set; }
        public AstNodeType NodeType { get; set;  }
        protected string Value { get; set; }
//        protected List<AstNode> _children = new List<AstNode>();
    }

    public class Program : AstNode
    {
        public Program(AstNode header, AstNode mainBlock) : base(AstNodeType.Program, header.Token)
        {
            Header = header;
            MainBlock = mainBlock;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Header { get; set; }
        public AstNode MainBlock { get; set; }
    }

    public class MainBlock : AstNode
    {
        public MainBlock(List<AstNode> declParts, AstNode mainCompound) : base(AstNodeType.MainBlock)
        {
            DeclsParts = declParts;
            MainCompound = mainCompound;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> DeclsParts { get; set; }
        public AstNode MainCompound { get; set; }
    }
}