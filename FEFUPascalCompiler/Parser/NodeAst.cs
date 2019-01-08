using System.Collections.Generic;
using System.Globalization;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser
{
    public abstract class NodeAst
    {
        public override string ToString() => Value;
        protected string Value { get; set; }
        protected List<NodeAst> _children = new List<NodeAst>();
    }

    public class Variable : NodeAst
    {
        public Variable(IdentToken token)
        {
            Token = token;
            Value = token.Value;
        }

        public IdentToken Token { get; }
    }

    public class ConstLiteral : NodeAst
    {
        public ConstLiteral(Token token)
        {
            Token = token;
            if (token.Type == TokenType.DoubleNumber)
                Value = ((DoubleNumberToken) token).Value.ToString(new NumberFormatInfo {NumberDecimalSeparator = "."});
            else if (token.Type == TokenType.IntegerNumber)
                Value = ((IntegerNumberToken) token).Value.ToString();
        }
        
        public Token Token { get; }
    }

    public class BinOperation : NodeAst
    {
        public BinOperation(BinOperatorToken oper, NodeAst left, NodeAst right)
        {
            Oper = oper;
            Value = oper.Value;
            _children[0] = left;
            _children[1] = right;
        }

        public Token Oper {get;}
        public NodeAst Left {get=>_children[0];}
        public NodeAst Right {get=>_children[1];}
    }
}