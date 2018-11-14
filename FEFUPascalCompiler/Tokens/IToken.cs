using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FEFUPascalCompiler.Tokens
{
    public enum TokenType : int
    {
        // arithmetic Operators
        BIN_ARITHM_OPERATOR,
        BIN_ARITHM_SUM,
        BIN_ARITHM_DIF,
        BIN_ARITHM_MUL,
        BIN_ARITHM_DIV,
        BIN_ARITHM_POW,

        //Assign operators
        ASSIGNMENT,
        SMP_ASSIGN,
        SUM_ASSIGN,
        DIF_ASSIGN,
        MUL_ASSIGN,
        DIV_ASSIGN,

        //Separator operators
        SEPARATOR,
        DOT,
        COMMA,
        SEMICOLON,
        COLON,

        //data types
        TYPE_INTEGER,
        TYPE_DOUBLE,
        TYPE_STRING,

        //Identifiers
        IDENT,

        // keywords
        AND,
        ARRAY,
        BEGIN,
        CASE,
        CONST,
        DIV,
        DO,
        DOWNTO,
        ELSE,
        END,
        FILE,
        FOR,
        FUNCTION,
        GOTO,
        IF,
        IN,
        LABEL,
        MOD,
        NIL,
        NOT,
        OF,
        OR,
        PACKED,
        PROCEDURE,
        PROGRAM,
        RECORD,
        REPEAT,
        SET,
        THEN,
        TO,
        TYPE,
        UNTIL,
        VAR,
        WHILE,
        WITH,
        
        //standard names
        ABS,
        ARCTAN,
        BOOLEAN,
        CHAR, 
        CHR,
        COS,
        DISPOSE,
        EOF,
        EOLN,
        EXP,
        FALSE,
        GET,
        INPUT,
        INTEGER,
        LN,
        MAXINT,
        MININT,
        NEW,
        ODD,
        ORD,
        OUTPUT,
        PACK,
        PAGE,
        PRED,
        PUT,
        READ,
        READLN,
        REAL,
        RESET,
        REWRITE,
        ROUND,
        SIN,
        SQR,
        SQRT,
        SUCC,
        TEXT,
        TRUE,
        TRUNC,
        UNPACK,
        WRITE,
        WRITELN,
        
    }

    internal static class Dictionaries
    {
        internal static readonly Dictionary<string, TokenType> KeyWords = new Dictionary<string, TokenType>
        {
            {"and", TokenType.AND},
            {"array", TokenType.ARRAY},
            {"begin", TokenType.BEGIN},
            {"case", TokenType.CASE},
            {"const", TokenType.CONST},
            {"div", TokenType.DIV},
            {"do", TokenType.DO},
            {"downto", TokenType.DOWNTO},
            {"else", TokenType.ELSE},
            {"end", TokenType.END},
            {"file", TokenType.FILE},
            {"for", TokenType.FOR},
            {"function", TokenType.FUNCTION},
            {"goto", TokenType.GOTO},
            {"if", TokenType.IF},
            {"in", TokenType.IN},
            {"label", TokenType.LABEL},
            {"mod", TokenType.MOD},
            {"nil", TokenType.NIL},
            {"not", TokenType.NOT},
            {"of", TokenType.OF},
            {"or", TokenType.OR},
            {"packed", TokenType.PACKED},
            {"procedure", TokenType.PROCEDURE},
            {"program", TokenType.PROGRAM},
            {"record", TokenType.RECORD},
            {"repeat", TokenType.REPEAT},
            {"set", TokenType.SET},
            {"then", TokenType.THEN},
            {"to", TokenType.TO},
            {"type", TokenType.TYPE},
            {"until", TokenType.UNTIL},
            {"var", TokenType.VAR},
            {"while", TokenType.WHILE},
            {"with", TokenType.WITH},
            {"abs", TokenType.ABS},
            {"arctan", TokenType.ARCTAN},
            {"boolean", TokenType.BOOLEAN},
            {"char", TokenType.CHAR},
            {"chr", TokenType.CHR},
            {"cos", TokenType.COS},
            {"dispose", TokenType.DISPOSE},
            {"eof", TokenType.EOF},
            {"eoln", TokenType.EOLN},
            {"exp", TokenType.EXP},
            {"false", TokenType.FALSE},
            {"get", TokenType.GET},
            {"input", TokenType.INPUT},
            {"integer", TokenType.INTEGER},
            {"ln", TokenType.LN},
            {"maxint", TokenType.MAXINT},
            {"minint", TokenType.MININT},
            {"new", TokenType.NEW},
            {"odd", TokenType.ODD},
            {"ord", TokenType.ORD},
            {"output", TokenType.OUTPUT},
            {"pack", TokenType.PACK},
            {"page", TokenType.PAGE},
            {"pred", TokenType.PRED},
            {"put", TokenType.PUT},
            {"read", TokenType.READ},
            {"readln", TokenType.READLN},
            {"real", TokenType.REAL},
            {"reset", TokenType.RESET},
            {"rewrite", TokenType.REWRITE},
            {"round", TokenType.ROUND},
            {"sin", TokenType.SIN},
            {"sqr", TokenType.SQR},
            {"sqrt", TokenType.SQRT},
            {"succ", TokenType.SUCC},
            {"text", TokenType.TEXT},
            {"true", TokenType.TRUE},
            {"trunc", TokenType.TRUNC},
            {"unpack", TokenType.UNPACK},
            {"write", TokenType.WRITE},
            {"writeln", TokenType.WRITELN},
        };

        internal static readonly Dictionary<string, TokenType> BinArithmeticOperators =
            new Dictionary<string, TokenType>
            {
                {"+", TokenType.BIN_ARITHM_SUM},
                {"-", TokenType.BIN_ARITHM_DIF},
                {"*", TokenType.BIN_ARITHM_MUL},
                {"/", TokenType.DIV},
                {"**", TokenType.BIN_ARITHM_POW}
            };
        
        internal static readonly Dictionary<string, TokenType> Assigns = new Dictionary<string, TokenType>
        {
            {":=", TokenType.SMP_ASSIGN},
            {"+=", TokenType.SUM_ASSIGN},
            {"-=", TokenType.DIF_ASSIGN},
            {"*=", TokenType.MUL_ASSIGN},
            {"/=", TokenType.DIV_ASSIGN},
        };
        
        internal static readonly Dictionary<string, TokenType> Separators = new Dictionary<string, TokenType>
        {
            {".", TokenType.DOT},
            {",", TokenType.COMMA},
            {":", TokenType.COLON},
            {";", TokenType.SEMICOLON},
        };
    }
}