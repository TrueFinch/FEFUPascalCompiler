using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using FEFUPascalCompiler.Parser.Semantics;

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

        public SymType FindType(string symbolIdentifier)
        {
//            var symbol = Find(symbolIdentifier);
            switch (Find(symbolIdentifier))
            {
                case SymAliasType alias:
                    return alias.Alias;
                case SymType symType:
                    return symType;
                default:
                    return null;
            }
        }

        public SymVar FindIdent(string identifier)
        {
            if (Find(identifier) is SymVar variable)
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

        public void AddType(SymType type)
        {
            _stack.Peek().AddType(type);
        }

        public void AddType(string identifier, SymType type)
        {
            _stack.Peek().AddType(identifier, type);
        }

        public void AddIdent(bool local, string identifier, SymType type)
        {
            _stack.Peek().AddVariable(local, identifier, type);
        }

        public void AddIdent(string identifier, SymVar ident)
        {
            _stack.Peek().AddVariable(identifier, ident);
        }

        public void AddAlias(string aliasIdentifier, SymType typeToAias)
        {
            _stack.Peek().AddAlias(aliasIdentifier, new SymAliasType(aliasIdentifier, typeToAias));
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
        public SymType SymInteger = new SymIntegerType();
        public SymType SymString = new SymStringType();
        public SymType SymFloat = new SymFloatType();
        public SymType SymChar = new SymCharType();
        public SymType SymBool = new SymBoolType();
        public SymType SymNil = new SymNilConst();
    }

    public class SymbolTable: IEnumerable 
    {
        private OrderedDictionary _table = new OrderedDictionary();

        public Symbol Find(string symbolIdentifier)
        {
            return _table.Contains(symbolIdentifier) ? _table[symbolIdentifier] as Symbol : null;
        }

        public void AddVariable(bool local, string identifier, SymType type)
        {
            if (local)
                _table.Add(identifier.ToLower(), new SymLocal(type));
            else
                _table.Add(identifier.ToLower(), new SymGlobal(type));
        }
        
        public void AddVariable(string identifier, SymVar ident)
        {
            _table.Add(identifier.ToLower(), ident);
        }

        public void AddType(SymType sym)
        {
            _table.Add(sym.Ident.ToLower(), sym);
        }

        public void AddType(string identifier, SymType sym)
        {
            _table.Add(identifier.ToLower(), sym);
        }

        public void AddAlias(string identifier, SymAliasType symbol)
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