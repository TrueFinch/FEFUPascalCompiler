using System.Collections.Generic;
using System.Globalization;
using FEFUPascalCompiler.Parser.AstVisitor;
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
        protected AstNode(AstNodeType type, Token token = null)
        {
            Token = token;
            Type = type;
            if (token != null)
            {
                if (token.Type == TokenType.DoubleNumber)
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
                Value = type.ToString();
            }
        }

        public override string ToString() => Value;

        public abstract T Accept<T>(IAstVisitor<T> visitor);

        public Token Token { get; protected set; }
        public AstNodeType Type { get; }
        protected string Value { get; set; }
        protected List<AstNode> _children = new List<AstNode>();
    }

    public class Program : AstNode
    {
        public Program(AstNode header, AstNode mainBlock) : base(AstNodeType.Program, header.Token)
        {
            _children.Add(header);
            _children.Add(mainBlock);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode Header => _children[0];
        public AstNode MainBlock => _children[1];
    }

    public class MainBlock : AstNode
    {
        public MainBlock(List<AstNode> declParts, AstNode mainCompound) : base(AstNodeType.MainBlock)
        {
            _children.InsertRange(0, declParts);
            _children.Add(mainCompound);
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public List<AstNode> DeclsParts => _children.GetRange(0, _children.Count - 1);
        public AstNode MainCompound => _children[_children.Count - 1];
    }
}