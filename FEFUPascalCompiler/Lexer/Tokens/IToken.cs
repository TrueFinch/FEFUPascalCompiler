using System.Collections.Generic;

namespace FEFUPascalCompiler.Tokens
{
    public enum TokenType
    {
        //TODO: add tests for carriage and at_sign
        AtSign,
        Carriage,
        //TODO: add tests for comparing_operators operators
        //logic operators
        NotEqualOperator,
        EqualOperator,
        LessOperator,
        LessOrEqualOperator,
        GreaterOperator,
        GreaterOrEqualOperator,
        
        
        // arithmetic Operators
        BinOperator,
        SumOperator,
        DifOperator,
        MulOperator,
        DivOperator,
        DoubleDotOperator,

        //Assign operators
        AssignOperator,
        SimpleAssignOperator,
        SumAssignOperator,
        DifAssignOperator,
        MulAssignOperator,
        DivAssignOperator,

        //Separator operators
        Separator,
        Dot,
        Comma,
        Semicolon,
        Colon,

        //data types
        BinIntegerNumber,
        DecIntegerNumber,
        OctIntegerNumber,
        HexIntegerNumber,
        DoubleNumber,
        StringConst,
        CharConst,

        //Identifiers
        Ident,
        
        //Comments
        MultiLineComment,
        SingleLineComment,
        
        //Brackets
        Bracket,
        OpenBracket,
        CloseBracket,
        OpenSquareBracket,
        CloseSquareBracket,
        
        // keywords
        And,
        Array,
        Begin,
        Case,
        Const,
        Div,
        Do,
        Downto,
        Else,
        End,
        File,
        For,
        Function,
        Goto,
        If,
        In,
        Label,
        Mod,
        Nil,
        Not,
        Of,
        Or,
        Packed,
        Pass,
        Procedure,
        Program,
        Record,
        Repeat,
        Set,
        Shl,
        Shr,
        Then,
        To,
        Type,
        Until,
        Var,
        While,
        With,
        Xor,

        //standard names
        Abs,
        Arctan,
        Boolean,
        Char,
        Chr,
        Cos,
        Dispose,
        Eof,
        Eoln,
        Exp,
        False,
        Get,
        Input,
        Integer,
        Ln,
        Maxint,
        Minint,
        New,
        Odd,
        Ord,
        Output,
        Pack,
        Page,
        Pred,
        Put,
        Read,
        Readln,
        Real,
        Reset,
        Rewrite,
        Round,
        Sin,
        Sqr,
        Sqrt,
        Succ,
        Text,
        True,
        Trunc,
        Unpack,
        Write,
        Writeln,

    }

    internal static class Dictionaries
    {
        internal static readonly Dictionary<string, TokenType> KeyWords = new Dictionary<string, TokenType>
        {
            {"and", TokenType.And},
            {"array", TokenType.Array},
            {"begin", TokenType.Begin},
            {"case", TokenType.Case},
            {"const", TokenType.Const},
            {"div", TokenType.Div},
            {"do", TokenType.Do},
            {"downto", TokenType.Downto},
            {"else", TokenType.Else},
            {"end", TokenType.End},
            {"file", TokenType.File},
            {"for", TokenType.For},
            {"function", TokenType.Function},
            {"goto", TokenType.Goto},
            {"if", TokenType.If},
            {"in", TokenType.In},
            {"label", TokenType.Label},
            {"mod", TokenType.Mod},
            {"nil", TokenType.Nil},
            {"not", TokenType.Not},
            {"of", TokenType.Of},
            {"or", TokenType.Or},
            {"packed", TokenType.Packed},
            {"pass", TokenType.Pass},
            {"procedure", TokenType.Procedure},
            {"program", TokenType.Program},
            {"record", TokenType.Record},
            {"repeat", TokenType.Repeat},
            {"set", TokenType.Set},
            {"shl", TokenType.Shl},
            {"shr", TokenType.Shr},
            {"then", TokenType.Then},
            {"to", TokenType.To},
            {"type", TokenType.Type},
            {"until", TokenType.Until},
            {"var", TokenType.Var},
            {"while", TokenType.While},
            {"with", TokenType.With},
            {"abs", TokenType.Abs},
            {"arctan", TokenType.Arctan},
            {"boolean", TokenType.Boolean},
            {"char", TokenType.Char},
            {"chr", TokenType.Chr},
            {"cos", TokenType.Cos},
            {"dispose", TokenType.Dispose},
            {"eof", TokenType.Eof},
            {"eoln", TokenType.Eoln},
            {"exp", TokenType.Exp},
            {"false", TokenType.False},
            {"get", TokenType.Get},
            {"input", TokenType.Input},
            {"integer", TokenType.Integer},
            {"ln", TokenType.Ln},
            {"maxint", TokenType.Maxint},
            {"minint", TokenType.Minint},
            {"new", TokenType.New},
            {"odd", TokenType.Odd},
            {"ord", TokenType.Ord},
            {"output", TokenType.Output},
            {"pack", TokenType.Pack},
            {"page", TokenType.Page},
            {"pred", TokenType.Pred},
            {"put", TokenType.Put},
            {"read", TokenType.Read},
            {"readln", TokenType.Readln},
            {"real", TokenType.Real},
            {"reset", TokenType.Reset},
            {"rewrite", TokenType.Rewrite},
            {"round", TokenType.Round},
            {"sin", TokenType.Sin},
            {"sqr", TokenType.Sqr},
            {"sqrt", TokenType.Sqrt},
            {"succ", TokenType.Succ},
            {"text", TokenType.Text},
            {"true", TokenType.True},
            {"trunc", TokenType.Trunc},
            {"unpack", TokenType.Unpack},
            {"write", TokenType.Write},
            {"writeln", TokenType.Writeln},
        };

        internal static readonly Dictionary<string, TokenType> LexemeToTokenType = new Dictionary<string, TokenType>
        {
            {"+", TokenType.SumOperator},
            {"-", TokenType.DifOperator},
            {"*", TokenType.MulOperator},
            {"/", TokenType.DivOperator},
            {"..", TokenType.DoubleDotOperator},
            {":=", TokenType.SimpleAssignOperator},
            {"+=", TokenType.SumAssignOperator},
            {"-=", TokenType.DifAssignOperator},
            {"*=", TokenType.MulAssignOperator},
            {"/=", TokenType.DivAssignOperator},
            {".", TokenType.Dot},
            {",", TokenType.Comma},
            {":", TokenType.Colon},
            {";", TokenType.Semicolon},
            {"(", TokenType.OpenBracket},
            {")", TokenType.CloseBracket},
            {"[", TokenType.OpenSquareBracket},
            {"]", TokenType.CloseSquareBracket},
            {"<>", TokenType.NotEqualOperator},
            {"<", TokenType.LessOperator},
            {"<=", TokenType.LessOrEqualOperator},
            {">=", TokenType.GreaterOrEqualOperator},
            {">", TokenType.GreaterOperator},
            {"=", TokenType.EqualOperator},
            {"^", TokenType.Carriage},
            {"@", TokenType.AtSign},
        };
    }
}