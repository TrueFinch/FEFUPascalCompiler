using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FEFUPascalCompiler.Tokens
{
    public enum TokenType : int
    {
        // arithmetic Operators
        SUM,
        DIF,
        MUL,
        DIV,
        POW,

        //Assign operators
        ASSIGN,
        SUM_ASSIGN,
        DIF_ASSIGN,
        MUL_ASSIGN,
        DIV_ASSIGN,

        //Separator operators
        DOT,
        COMMA,
        SEMICOLON,
        COLON,
        SPACE,

        //data types
        TYPE_INTEGER,

        //Identifiers
        IDENT,

        // keywords
        PROGRAM,
        VAR,
        BEGIN,
        END,
        
        //special tokenTypes
        NEWLINE,
        EOF,
    }

    internal static class Dictionaries
    {
        internal static readonly Dictionary<string, TokenType> KeyWords = new Dictionary<string, TokenType>
        {
            {"begin", TokenType.BEGIN},
            {"end", TokenType.END},
            {"program", TokenType.PROGRAM},
            {"var", TokenType.VAR}
        };

        internal static readonly Dictionary<string, TokenType> BinArithmeticOperators =
            new Dictionary<string, TokenType>
            {
                {"+", TokenType.SUM},
                {"-", TokenType.DIF},
                {"*", TokenType.MUL},
                {"/", TokenType.DIV},
                {"**", TokenType.POW}
            };
        
        internal static readonly Dictionary<string, TokenType> Assigns = new Dictionary<string, TokenType>
        {
            {":=", TokenType.ASSIGN},
            {"+=", TokenType.SUM_ASSIGN},
            {"-=", TokenType.DIF_ASSIGN},
            {"*=", TokenType.MUL_ASSIGN},
            {"/=", TokenType.DIV_ASSIGN},
        };
    }
}