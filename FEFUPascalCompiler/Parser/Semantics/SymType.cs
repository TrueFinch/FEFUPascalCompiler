using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Sematics;

namespace FEFUPascalCompiler.Parser.Semantics
{
    public abstract class SymType : Symbol
    {
        protected SymType(string ident, bool isTrivial) : base(ident)
        {
            this.isTrivial = isTrivial;
        }

        public bool IsEquivalent(SymType other)
        {
            switch (other)
            {
                case SymAliasType symAliasType:
                    return IsEquivalent(symAliasType);
                case SymArrayType symArrayType:
                    return IsEquivalent(symArrayType);
                case SymBoolType symBoolType:
                    return IsEquivalent(symBoolType);
                case SymCharType symCharType:
                    return IsEquivalent(symCharType);
                case SymConformatArrayType symConformatArrayType:
                    return IsEquivalent(symConformatArrayType);
                case SymFloatType symFloatType:
                    return IsEquivalent(symFloatType);
                case SymIntegerType symInteger:
                    return IsEquivalent(symInteger);
                case SymNilConst symNilConst:
                    return IsEquivalent(symNilConst);
                case SymPointerType symPointerType:
                    return IsEquivalent(symPointerType);
                case SymRecordType symRecordType:
                    return IsEquivalent(symRecordType);
                case SymStringType symStringType:
                    return IsEquivalent(symStringType);
            }

            return false;
        }

        protected virtual bool IsEquivalent(SymIntegerType other) => false;
        protected virtual bool IsEquivalent(SymFloatType other) => false;
        protected virtual bool IsEquivalent(SymCharType other) => false;
        protected virtual bool IsEquivalent(SymStringType other) => false;
        protected virtual bool IsEquivalent(SymBoolType other) => false;
        protected virtual bool IsEquivalent(SymNilConst other) => false;
        protected virtual bool IsEquivalent(SymAliasType other) => IsEquivalent(other.Alias);
        protected virtual bool IsEquivalent(SymRecordType other) => false;
        protected virtual bool IsEquivalent(SymArrayType other) => false;
        protected virtual bool IsEquivalent(SymConformatArrayType other) => false;
        protected virtual bool IsEquivalent(SymPointerType other) => false;

        public bool isTrivial;
    }

    public class SymIntegerType : SymType
    {
        public SymIntegerType() : base("Integer", true)
        {
        }

        protected override bool IsEquivalent(SymIntegerType other) => true;
        protected override bool IsEquivalent(SymFloatType other) => false;
    }

    public class SymFloatType : SymType
    {
        public SymFloatType() : base("Float", true)
        {
        }

        protected override bool IsEquivalent(SymIntegerType other) => true;
        protected override bool IsEquivalent(SymFloatType other) => true;
    }

    public class SymCharType : SymType
    {
        public SymCharType() : base("Char", true)
        {
        }

        protected override bool IsEquivalent(SymCharType other) => true;
    }

    public class SymStringType : SymType
    {
        public SymStringType() : base("String", false)
        {
        }

        protected override bool IsEquivalent(SymStringType other) => true;
    }

    public class SymBoolType : SymType
    {
        public SymBoolType() : base("Boolean", true)
        {
        }

        protected override bool IsEquivalent(SymBoolType other) => true;
    }

    public class SymNilConst : SymType
    {
        public SymNilConst() : base("nil", true)
        {
        }

        protected override bool IsEquivalent(SymNilConst other) => true;
        protected override bool IsEquivalent(SymPointerType other) => true;
    }

    public class SymAliasType : SymType
    {
        public SymAliasType(string ident, SymType symType) : base(ident, symType.isTrivial)
        {
            Alias = symType;
        }

        public SymType Alias { get; }

        public SymType GetBase()
        {
            var baseType = Alias;

            while (baseType != null && baseType.GetType() == typeof(SymAliasType))
            {
                baseType = (baseType as SymAliasType)?.Alias;
            }

            return baseType;
        }

        protected override bool IsEquivalent(SymIntegerType other) => Alias.IsEquivalent(other);
        protected override bool IsEquivalent(SymFloatType other) => Alias.IsEquivalent(other);
        protected override bool IsEquivalent(SymCharType other) => Alias.IsEquivalent(other);
        protected override bool IsEquivalent(SymStringType other) => Alias.IsEquivalent(other);
        protected override bool IsEquivalent(SymBoolType other) => Alias.IsEquivalent(other);
        protected override bool IsEquivalent(SymNilConst other) => Alias.IsEquivalent(other);
        protected override bool IsEquivalent(SymAliasType other) => IsEquivalent(other.Alias);
        protected override bool IsEquivalent(SymRecordType other) => Alias.Equals(other);
        protected override bool IsEquivalent(SymArrayType other) => Alias.Equals(other);
        protected override bool IsEquivalent(SymConformatArrayType other) => Alias.Equals(other);
        protected override bool IsEquivalent(SymPointerType other) => Alias.IsEquivalent(other);
    }

    public class SymRecordType : SymType
    {
        public SymRecordType(SymbolTable table, string ident = "") : base(ident, false)
        {
            Table = table;
        }

        public SymbolTable Table { get; }
        protected override bool IsEquivalent(SymRecordType other) => Equals(other);
    }

    public class SymArrayType : SymType
    {
        public SymArrayType(IndexRangeSymbol<int, int> indexRange, SymType elemSymType, string ident = "") :
            base(ident, false)
        {
            IndexRange = indexRange;
            ElementSymType = elemSymType;
        }

        public IndexRangeSymbol<int, int> IndexRange { get; }
        public SymType ElementSymType { get; }
        protected override bool IsEquivalent(SymArrayType other) => Equals(other);
    }

    public class SymConformatArrayType : SymType
    {
        public SymConformatArrayType(SymType elemSymType) : base("", false)
        {
            ElementSymType = elemSymType;
        }

        public SymType ElementSymType { get; }
        protected override bool IsEquivalent(SymConformatArrayType other) => Equals(other);
    }

    public class SymPointerType : SymType
    {
        public SymPointerType(SymType referencedSymType) : base("^" + referencedSymType.Ident, false)
        {
            ReferencedSymType = referencedSymType;
        }

        public SymType ReferencedSymType { get; set; }

        protected override bool IsEquivalent(SymPointerType other) =>
            ReferencedSymType.IsEquivalent(other.ReferencedSymType);

        protected override bool IsEquivalent(SymNilConst other) => true;
    }

    public class IndexRangeSymbol<T1, T2>
    {
            // @formatter:off
            public T1 From { get; }
            public T2 To     { get; }
        // @formatter:on

        public IndexRangeSymbol(T1 fromIndex, T2 toIndex)
        {
                // @formatter:off
                From = fromIndex ;
                To   = toIndex;
            // @formatter:on
        }
    }

    public class CallableSymbol : SymType
    {
        public CallableSymbol(string ident) : base(ident, false)
        {
        }

        public List<Tuple<string, SymType>> ParametersTypes { get; } = new List<Tuple<string, SymType>>();

        public bool IsForward { get; set; }

//        public SymbolTable Parameters { get; set; }
        public SymbolTable Local { get; set; }
        public SymType ReturnSymType { get; set; }
    }

//    public class SymConst : SymType
//    {
//        public SymConst(string ident, SymType symType) : base(ident)
//        {
//            Type = symType;
//        }
//
////        public Expression Value { get; } = null;
//        public SymType Type { get; set; }
//    }
}