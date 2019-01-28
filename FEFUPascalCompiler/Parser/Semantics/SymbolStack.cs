using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.Sematics
{
    public class SymbolStack : IEnumerable<SymbolTable>
    {
        public SymbolStack()
        {
            _stack.Push(new SymbolTable());
            AddType(SymInt);
            AddType(SymString);
            AddType(SymFloat);
            AddType(SymChar);
            AddType(SymBool);
            AddType(SymNil);
            
            SymIntPtr.ReferencedSymType = SymInt;
            SymIntPtr.Ident = string.Concat(SymInt.Ident, "_ptr");
            SymFloatPtr.ReferencedSymType = SymFloat;
            SymFloatPtr.Ident = string.Concat(SymInt.Ident, "_ptr");
            SymCharPtr.ReferencedSymType = SymChar;
            SymCharPtr.Ident = string.Concat(SymInt.Ident, "_ptr");
            SymStringPtr.ReferencedSymType = SymString;
            SymStringPtr.Ident = string.Concat(SymInt.Ident, "_ptr");
            SymBoolPtr.ReferencedSymType = SymBool;
            SymBoolPtr.Ident = string.Concat(SymInt.Ident, "_ptr");
            SymNilPtr.ReferencedSymType = SymNil;
            SymNilPtr.Ident = string.Concat(SymInt.Ident, "_ptr");
        }

        public void Push()
        {
            _stack.Push(new SymbolTable());
        }

        public void Push(SymbolTable table)
        {
            _stack.Push(table);
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

        internal void CheckIdentifierDuplicate(Token ident)
        {
            if (Find(ident.Value) != null)
            {
                throw new Exception(string.Format("{0}, {1} : Duplicate identifier '{2}'",
                    ident.Line, ident.Column, ident.Lexeme));
            }
        }
        
        public Symbol FindInScope(string identifier)
        {
            return _stack.Peek().Find(identifier);
        }

        internal void CheckIdentifierDuplicateInScope(Token ident)
        {
            if (FindInScope(ident.Value) != null)
            {
                throw new Exception(string.Format("{0}, {1} : Duplicate identifier '{2}'",
                    ident.Line, ident.Column, ident.Lexeme));
            }
        }
        
        public SymVar FindIdentInScope(string identifier)
        {
            return _stack.Peek().Find(identifier) as SymVar;
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

        internal SymType CheckTypeDeclared(Token type)
        {
            var symType = FindType(type.Value);

            if (FindType(type.Value) == null)
            {
                throw new Exception(string.Format("{0}, {1} : Undeclared type identifier '{2}'",
                    type.Line, type.Column, type.Lexeme));
            }

            return symType;
        }
        
        public SymVar FindIdent(string identifier)
        {
            if (Find(identifier) is SymVar variable)
            {
                return variable;
            }

            return null;
        }

        public CallableSymbol FindFunc(string identifier, List<SymType> parametersTypes)
        {
            if (Find(identifier) is CallableSymbol funcSymb)
            {
                return funcSymb;
            }

            return null;
        }

//        public CallableSymbol FindProc(string identifier)
//        {
//            if (Find(identifier) is CallableSymbol procSymb)
//            {
//                return procSymb;
//            }
//
//            return null;
//        }

        public void AddType(SymType type)
        {
            _stack.Peek().AddType(type);
        }

        public void AddType(string identifier, SymType type)
        {
            _stack.Peek().AddType(identifier, type);
        }

        public void AddVariable(bool local, string identifier, SymType type)
        {
            _stack.Peek().AddVariable(local, identifier, type);
        }

        public void AddVariable(string identifier, SymVar ident)
        {
            _stack.Peek().AddVariable(identifier, ident);
        }

        public void AddConstant(bool local, string identifier, SymType type)
        {
            _stack.Peek().AddVariable(local, identifier, type);
        }

        public void AddParameter(string modifier, string identifier, SymType type)
        {
            _stack.Peek().AddVariable(identifier, new SymParameter(type, modifier));
        }
        
        public void AddAlias(string aliasIdentifier, SymType typeToAias)
        {
            _stack.Peek().AddAlias(aliasIdentifier, new SymAliasType(aliasIdentifier, typeToAias));
        }

        public void PrepareFunction(string identifier, CallableSymbol funcSym)
        {
            _stack.Peek().AddFunction(identifier, funcSym);
        }
        
        public void AddFunction(string identifier, CallableSymbol funcSym)
        {
            funcSym.Local = Pop();
//            funcSym.Parameters = Pop();
            _stack.Peek().Remove(identifier);
            _stack.Peek().AddFunction(identifier, funcSym);
        }
        
//        public void PrepareProcedure(string identifier, CallableSymbol procSym)
//        {
//            _stack.Peek().AddProcedure(identifier, procSym);
//        }
//        
//        public void AddProcedure(string identifier, CallableSymbol procSym)
//        {
//            procSym.Local = Pop();
//            procSym.Parameters = Pop();
//            _stack.Peek().Remove(identifier);
//            _stack.Peek().AddProcedure(identifier, procSym);
//        }

//        public bool IsLvalue(Expression expr)
//        {
//            return expr is 
//        }
        
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
        public SymType SymInt = new SymIntegerType();
        public SymType SymFloat = new SymFloatType();
        public SymType SymChar = new SymCharType();
        public SymType SymString = new SymStringType();
        public SymType SymBool = new SymBoolType();
        public SymType SymNil = new SymNilConst();
        public SymPointerType SymIntPtr = new SymPointerType();
        public SymPointerType SymFloatPtr = new SymPointerType();
        public SymPointerType SymCharPtr = new SymPointerType();
        public SymPointerType SymStringPtr = new SymPointerType();
        public SymPointerType SymBoolPtr = new SymPointerType();
        public SymPointerType SymNilPtr = new SymPointerType();
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

        public void AddFunction(string identifier, CallableSymbol funcSym)
        {
            _table.Add(identifier.ToLower(), funcSym);
        }

        public void AddProcedure(string identifier, CallableSymbol funcSym)
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