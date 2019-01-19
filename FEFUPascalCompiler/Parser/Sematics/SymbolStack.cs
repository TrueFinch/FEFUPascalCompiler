using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using FEFUPascalCompiler.Parser.ParserParts;

namespace FEFUPascalCompiler.Parser.Sematics
{
    public class SymbolStack : IEnumerable<SymbolTable<string, Symbol>>
    {
        public SymbolStack()
        {
            _stack.Push(new SymbolTable<string, Symbol>());
        }

        public void Push()
        {
            _stack.Push(new SymbolTable<string, Symbol>());
        }

        public SymbolTable<string, Symbol> Peek()
        {
            return _stack.Peek();
        }

        public SymbolTable<string, Symbol> Pop()
        {
            return _stack.Pop();
        }

        public Symbol Find(string symbolIdentifier)
        {
            foreach (var symbolTable in _stack)
            {
                var symbol = symbolTable.Find(symbolIdentifier);
                if (symbol != null)
                {
                    return symbol;
                }
            }

            return null;
        }

        public SymbolType FindType(string symbolIdentifier)
        {
//            var symbol = Find(symbolIdentifier);
            switch (Find(symbolIdentifier))
            {
                case AliasSymbolType alias:
                    return alias.Alias;
                case SymbolType symType:
                    return symType;
                default:
                    return null;
            }
        }

        public Var FindIdent(string identifier)
        {
            if (Find(identifier) is Var variable)
            {
                return variable;
            }

            return null;
        }

        public FunctionSymbol FindFunc(string identifier)
        {
            if (Find(identifier) is FunctionSymbol funcSymb)
            {
                return funcSymb;
            }

            return null;
        }

        public ProcedureSymbol FindProc(string identifier)
        {
            if (Find(identifier) is ProcedureSymbol procSymb)
            {
                return procSymb;
            }

            return null;
        }
        
        public IEnumerator<SymbolTable<string, Symbol>> GetEnumerator()
        {
            return _stack.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Stack<SymbolTable<string, Symbol>> _stack = new Stack<SymbolTable<string, Symbol>>();

        // default types
        public Symbol SymInteger = new IntegerSymbolType();
        public Symbol SymString = new StringSymbolType();
        public Symbol SymFloat = new FloatSymbolType();
        public Symbol SymChar = new CharSymbolType();
        public Symbol SymBool = new BoolSymbolType();
    }

    public class SymbolTable<TKey, TValue> : IEnumerable where TValue : Symbol
    {
        private OrderedDictionary _table = new OrderedDictionary();

        public TValue Find(TKey symbolIdentifier)
        {
            return _table.Contains(symbolIdentifier) ? _table[symbolIdentifier] as TValue : null;
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return _table.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}