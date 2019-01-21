using System.Linq.Expressions;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.Semantics
{
    public class TypeChecker
    {
        private readonly SymbolStack _symStack;

        public TypeChecker(SymbolStack stack)
        {
            _symStack = stack;
        }
        
        //check if 
        public void Assignment(ref Expression left, ref Expression rigth, Token op)
        {
            
        }
    }
}