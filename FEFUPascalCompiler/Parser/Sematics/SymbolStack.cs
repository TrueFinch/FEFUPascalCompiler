using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FEFUPascalCompiler.Parser.Sematics
{
    public class SymbolStack : IEnumerable<SymbolTable>
    {
        public SymbolStack()
        {
            _stack.Push(new SymbolTable());
            AddType(SymInteger);
            AddType(SymString);
            AddType(SymFloat);
            AddType(SymChar);
            AddType(SymBool);
        }

        public void Push()
        {
            _stack.Push(new SymbolTable());
        }

        public SymbolTable Peek()
        {
            return _stack.Peek();
        }

        public SymbolTable Pop()
        {
            return _stack.Pop();
        }

        public Symbol Find(string symbolIdentifier)
        {
            foreach (var symbolTable in _stack)
            {
                var symbol = symbolTable.Find(symbolIdentifier);
                if (symbol != null)
                    return symbol;
            }

            return null;
        }

        public Symbol FindInScope(string identifier)
        {
            return _stack.Peek().Find(identifier);
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

        public void AddType(SymbolType type)
        {
            _stack.Peek().AddType(type);
        }

        public void AddType(string identifier, SymbolType type)
        {
            _stack.Peek().AddType(identifier, type);
        }

        public void AddIdent(bool local, string identifier, SymbolType type)
        {
            _stack.Peek().AddVariable(local, identifier, type);
        }

        public void AddIdent(string identifier, Var ident)
        {
            _stack.Peek().AddVariable(identifier, ident);
        }

        public void AddAlias(string aliasIdentifier, SymbolType typeToAias)
        {
            _stack.Peek().AddAlias(aliasIdentifier, new AliasSymbolType(aliasIdentifier, typeToAias));
        }

        public void PrepareFunction(string identifier, FunctionSymbol funcSym)
        {
            _stack.Peek().AddFunction(identifier, funcSym);
        }
        
        public void AddFunction(string identifier, FunctionSymbol funcSym)
        {
            funcSym.Local = Pop();
            funcSym.Parameters = Pop();
            _stack.Peek().Remove(identifier);
            _stack.Peek().AddFunction(identifier, funcSym);
        }
        
        public void PrepareProcedure(string identifier, ProcedureSymbol procSym)
        {
            _stack.Peek().AddProcedure(identifier, procSym);
        }
        
        public void AddProcedure(string identifier, ProcedureSymbol procSym)
        {
            procSym.Local = Pop();
            procSym.Parameters = Pop();
            _stack.Peek().Remove(identifier);
            _stack.Peek().AddProcedure(identifier, procSym);
        }
        
        public IEnumerator<SymbolTable> GetEnumerator()
        {
            return _stack.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Stack<SymbolTable> _stack = new Stack<SymbolTable>();

        // default types
        public SymbolType SymInteger = new IntegerSymbolType();
        public SymbolType SymString = new StringSymbolType();
        public SymbolType SymFloat = new FloatSymbolType();
        public SymbolType SymChar = new CharSymbolType();
        public SymbolType SymBool = new BoolSymbolType();
        public SymbolType SymNil = new NilSymbolConst();
    }

    public class SymbolTable: IEnumerable 
    {
        private OrderedDictionary _table = new OrderedDictionary();

        public Symbol Find(string symbolIdentifier)
        {
            return _table.Contains(symbolIdentifier) ? _table[symbolIdentifier] as Symbol : null;
        }

        public void AddVariable(bool local, string identifier, SymbolType type)
        {
            if (local)
                _table.Add(identifier.ToLower(), new Local(type));
            else
                _table.Add(identifier.ToLower(), new Global(type));
        }
        
        public void AddVariable(string identifier, Var ident)
        {
            _table.Add(identifier.ToLower(), ident);
        }

        public void AddType(SymbolType symbol)
        {
            _table.Add(symbol.Ident.ToLower(), symbol);
        }

        public void AddType(string identifier, SymbolType symbol)
        {
            _table.Add(identifier.ToLower(), symbol);
        }

        public void AddAlias(string identifier, AliasSymbolType symbol)
        {
            _table.Add(identifier.ToLower(), symbol);
        }

        public void AddFunction(string identifier, FunctionSymbol funcSym)
        {
            _table.Add(identifier.ToLower(), funcSym);
        }

        public void AddProcedure(string identifier, ProcedureSymbol funcSym)
        {
            _table.Add(identifier.ToLower(), funcSym);
        }
        
        public void Remove(string identifier)
        {
            _table.Remove(identifier.ToLower());
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