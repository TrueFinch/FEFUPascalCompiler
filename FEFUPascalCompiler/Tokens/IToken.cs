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
        UNAR_MINUS,
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
        SPACE,
        //data types
        TYPE_INTEGER,
        
        //Identifiers
        VARIABLE,
       // keywords
        BEGIN,
        END,
        PROGRAM,
        VAR,
        
        
    }

    internal static class Dictionaries
    {
        internal static readonly Dictionary<string, TokenType> keyWords = new Dictionary<string, TokenType>
        {
            {"begin", TokenType.BEGIN},
            {"end", TokenType.END},
            {"program", TokenType.PROGRAM},
            {"var", TokenType.VAR}
        };
    }
    
    public interface IToken
    {
        int Line { get; set; }
        int Column { get; set; }
        TokenType TokenType { get; set; }
        string Text { get; set; }
    }
}