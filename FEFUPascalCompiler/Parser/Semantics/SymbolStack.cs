using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.ParserParts;
using FEFUPascalCompiler.Parser.Semantics;
using FEFUPascalCompiler.Tokens;

namespace FEFUPascalCompiler.Parser.Sematics
{
    public class SymbolStack : IEnumerable<SymbolTable>
    {
        public SymbolStack()
        {
            _stack.Push(new SymbolTable()); // standard types namespace
            AddType(SymInt);
            AddType(SymString);
            AddType(SymFloat);
            AddType(SymChar);
            AddType(SymBool);
            AddType(SymNil);

//            AddFunction(WriteFunction);

//            SymIntPtr.ReferencedSymType = SymInt;
//            SymIntPtr.Ident = string.Concat("^", SymInt.Ident);
//            SymFloatPtr.ReferencedSymType = SymFloat;
//            SymFloatPtr.Ident = string.Concat("^", SymFloat.Ident);
//            SymCharPtr.ReferencedSymType = SymChar;
//            SymCharPtr.Ident = string.Concat("^", SymChar.Ident);
//            SymStringPtr.ReferencedSymType = SymString;
//            SymStringPtr.Ident = string.Concat("^", SymString.Ident);
//            SymBoolPtr.ReferencedSymType = SymBool;
//            SymBoolPtr.Ident = string.Concat("^", SymBool.Ident);
//            SymNilPtr.ReferencedSymType = SymNil;
//            SymNilPtr.Ident = string.Concat("^", SymNil.Ident);

            AddType(SymIntPtr);
            AddType(SymStringPtr);
            AddType(SymFloatPtr);
            AddType(SymCharPtr);
            AddType(SymBoolPtr);
            AddType(SymNilPtr);
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

        internal bool ContainIdent(string identName)
        {
            return Find(identName) != null;
        }

        internal bool ContainIdentInScope(string identName)
        {
            return FindInScope(identName) != null;
        }

        public Symbol FindInScope(string identifier)
        {
            return _stack.Peek().Find(identifier);
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

        internal SymType CheckTypeDeclared(String type)
        {
            var symType = FindType(type);

            if (FindType(type) == null)
            {
                return null;
//                throw new Exception(string.Format("{0}, {1} : Undeclared type identifier '{2}'",
//                    type.Line, type.Column, type.Lexeme));
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

        //TODO: rename with callable
        public CallableSymbol FindFunc(string identifier, List<SymType> parametersTypes)
        {
            foreach (var table in _stack)
            {
                if (table.FindCallable(identifier, parametersTypes) is CallableSymbol callable)
                {
                    return callable;
                }
            }

            return null;
        }

        public List<CallableSymbol> FindFunc(string identifier)
        {
            var callables = new List<CallableSymbol>();

            foreach (var table in _stack)
            {
                var c = table.FindCallable(identifier);
                if (c == null) continue;
                foreach (var callableSymbol in c)
                {
                    callables.Add(callableSymbol);
                }
            }

            return callables;
        }

        //TODO: rename with callable
        public bool IsFuncExist(string identifier)
        {
            foreach (var table in _stack)
            {
                if (table.IsCallableExist(identifier))
                    return true;
            }

            return false;
        }

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

        //TODO: rename with callable
        public bool PrepareFunction(CallableSymbol funcSym)
        {
            var existCallable = _stack.Peek().FindCallable(funcSym.Ident,
                new List<SymType>(from type in funcSym.ParametersTypes select type.Item2));
            if (existCallable != null && existCallable.IsForward)
            {
                return true; // function header already prepared by forward
            }

            return _stack.Peek().AddCallable(funcSym.Ident, funcSym);
        }

        //TODO: rename with callable
        public void AddFunction(CallableSymbol funcSym)
        {
//            funcSym.Local = Pop();
//            funcSym.Parameters = Pop();
            _stack.Peek().RemoveCallable(funcSym.Ident, funcSym); // remove prepared callable
            _stack.Peek().AddCallable(funcSym.Ident, funcSym); // add full callable
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

        // default types`
        public static readonly SymType SymInt = new SymIntegerType();
        public static readonly SymType SymFloat = new SymFloatType();
        public static readonly SymType SymChar = new SymCharType();
        public static readonly SymType SymString = new SymStringType();
        public static readonly SymType SymBool = new SymBoolType();
        public static readonly SymType SymNil = new SymNilConst();
        public SymPointerType SymIntPtr = new SymPointerType(SymInt);
        public SymPointerType SymFloatPtr = new SymPointerType(SymFloat);
        public SymPointerType SymCharPtr = new SymPointerType(SymChar);
        public SymPointerType SymStringPtr = new SymPointerType(SymString);
        public SymPointerType SymBoolPtr = new SymPointerType(SymBool);
        public SymPointerType SymNilPtr = new SymPointerType(SymNil);

        public SymPointerType SymTypeToSymPointerType(SymType symType)
        {
            switch (symType)
            {
                case SymAliasType symAliasType:
                    return SymTypeToSymPointerType(symAliasType.Alias);
                case SymArrayType symArrayType:
                    return new SymPointerType(symArrayType);
                case SymBoolType symBoolType:
                    return SymBoolPtr;
                case SymCharType symCharType:
                    return SymCharPtr;
                case SymConformatArrayType symConformatArrayType:
                    return new SymPointerType(symConformatArrayType);
                case SymFloatType symFloatType:
                    return SymFloatPtr;
                case SymIntegerType symIntegerType:
                    return SymIntPtr;
                case SymNilConst symNilConst:
                    return SymNilPtr;
                case SymPointerType symPointerType:
                    return new SymPointerType(symPointerType);
                case SymRecordType symRecordType:
                    return new SymPointerType(symRecordType);
                case SymStringType symStringType:
                    return SymStringPtr;
            }

            return null;
        }
    }
}