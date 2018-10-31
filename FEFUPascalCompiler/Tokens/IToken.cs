using System.ComponentModel.DataAnnotations;

namespace FEFUPascalCompiler.Tokens
{
    public enum TokenType : int
    {
        // Operators
        SUM,
        DIF,
        MUL,
        DIV,
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
        //data types
        TYPE_INTEGER,
        
    }
    public interface IToken
    {
        int Line { get; set; }
        int Column { get; set; }
        TokenType TokenType { get; set; }
        string Text { get; set; }
    }
}