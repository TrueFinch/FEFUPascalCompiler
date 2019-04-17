using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace FEFUPascalCompiler.Parser.Semantics
{
    public class SymbolTable : IEnumerable
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

        public CallableSymbol FindCallable(string identifier, List<SymType> parametersTypes)
        {
            if (!_table.Contains(identifier) || !(_table[identifier] is List<CallableSymbol> callables)) return null;
            if (parametersTypes == null)
            {
                return callables[0];
            }

            foreach (var callableSymbol in callables)
            {
                if (parametersTypes.Count != callableSymbol.ParametersTypes.Count)
                    continue;
                bool found = true;
                for (int i = 0; i < parametersTypes.Count; ++i)
                {
                    if (parametersTypes[i] == (callableSymbol.ParametersTypes[i].Item2)) continue;
                    found = false;
                    break;
                }

                if (found)
                    return callableSymbol;
            }

            return null;
        }

        public List<CallableSymbol> FindCallable(string identifier)
        {
            if (!_table.Contains(identifier) || !(_table[identifier] is List<CallableSymbol> callables)) return null;
            return callables;
        }

        public bool IsCallableExist(string identifier)
        {
            return FindCallable(identifier) != null;
        }

        public bool AddCallable(string identifier, CallableSymbol callable)
        {
            var paramTypes = new List<SymType>(from type in callable.ParametersTypes select type.Item2);

            foreach (var parametersType in callable.ParametersTypes)
            {
                paramTypes.Add(parametersType.Item2);
            }

            if (FindCallable(identifier, paramTypes) != null)
            {
                return false;
            }

            if (_table.Contains(identifier))
            {
                if (_table[identifier] is List<CallableSymbol> callableSymbols && !callableSymbols.Contains(callable))
                    callableSymbols.Add(callable);
            }
            else
                _table.Add(identifier.ToLower(), new List<CallableSymbol> {callable});

            return true;
        }

        public void RemoveCallable(string identifier, CallableSymbol callable)
        {
            if (!_table.Contains(identifier) || !(_table[identifier] is List<CallableSymbol> callables)) return;
            foreach (var callableSymbol in callables)
            {
                if (callable.ParametersTypes.Count != callableSymbol.ParametersTypes.Count)
                    continue;
                bool found = true;
                for (int i = 0; i < callable.ParametersTypes.Count; ++i)
                {
                    if (!callable.ParametersTypes[i].Equals(callableSymbol.ParametersTypes[i]))
                    {
                        found = false;
                    }
                }

                if (found)
                {
                    callables.Remove(callableSymbol);
                    break;
                }
            }
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