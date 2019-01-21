using System;
using System.Collections.Generic;
using FEFUPascalCompiler.Parser.Sematics;

namespace FEFUPascalCompiler.Parser.Semantics
{
    public abstract class SymType : Symbol
    {
        protected SymType(string ident) : base(ident)
        {
        }

        public bool Equals(ref SymType symType)
        {
            return false;
        }
    }
    
    public class SymIntegerType : SymType
    {
        public SymIntegerType() : base("Integer")
        {
        }

        public bool Equals(ref SymIntegerType type)
        {
            return true;
        }
        
        public bool Equals(ref SymIntegerConst type)
        {
            return true;
        }
    }

    public class SymFloatType : SymType
    {
        public SymFloatType() : base("Float")
        {
        }

        public bool Equals(ref SymFloatType type)
        {
            return true;
        }
    }

    public class SymSymFloatConst : SymFloatType
    {
        public SymSymFloatConst(float value) : base()
        {
            Value = value;
        }
        
        public float Value { get; }
    }
    
    public class SymCharType : SymType
    {
        public SymCharType() : base("Char")
        {
        }

        public bool Equals(ref SymCharType type)
        {
            return true;
        }
    }
    
    public class SymSymCharConst : SymCharType
    {
        public SymSymCharConst(char value) : base()
        {
            Value = value;
        }
        
        public char Value { get; }
    }

    public class SymStringType : SymType
    {
        public SymStringType() : base("String")
        {
        }

        public bool Equals(ref SymStringType type)
        {
            return true;
        }
    }
    
    public class SymSymStringConst : SymStringType
    {
        public SymSymStringConst(string value) : base()
        {
            Value = value;
        }
        
        public string Value { get; }
    }

    
    public class SymBoolType : SymType
    {
        public SymBoolType() : base("Bool")
        {
        }

        public bool Equals(ref SymBoolType type)
        {
            return true;
        }
    }

    public class SymNilConst : SymType
    {
        public SymNilConst() : base("nil")
        {
        }
        
        public bool Equals(ref Symbol symbolType)
        {
            return false;
        }
    }
    
    public class SymAliasType : SymType
    {
        public SymAliasType(string ident, SymType symType) : base(ident)
        {
            Alias = symType;
        }

        public new bool Equals(ref SymType symType)
        {
            return Alias.Equals(ref symType);
        }

        public bool Equals(ref SymAliasType type)
        {
            var typeAlias = type.Alias;
            return Alias.Equals(ref typeAlias);
        }

        public SymType Alias { get; }
    }

    public class SymRecordType : SymType
    {
        public SymRecordType(SymbolTable table, string ident = "") : base(ident)
        {
            Table = table;
        }

        public bool Equals(ref SymRecordType type)
        {
            return string.Equals(Ident, type.Ident) && (Equals(type) || Ident.Length != 0);
        }

        public SymbolTable Table { get; }
    }

    public class SymArrayType : SymType
    {
        public SymArrayType(List<IndexRange<int, int>> indexRanges, SymType elemSymType, string ident = "") : base(ident)
        {
            IndexRanges = indexRanges;
            ElementSymType = elemSymType;
        }

        public bool Equals(ref SymArrayType type)
        {
            var typeElementType = type.ElementSymType;
            bool res = Ident == type.Ident && ElementSymType.Equals(ref typeElementType);
            for (int i = 0; i < Math.Max(IndexRanges.Count, type.IndexRanges.Count); ++i)
                res = res && IndexRanges[i] == type.IndexRanges[i];

            return res;
        }

        public List<IndexRange<int, int>> IndexRanges { get; }
        public SymType ElementSymType { get; }
    }

    public class SymConformatArrayType : SymType
    {
        public SymConformatArrayType(SymType elemSymType) : base("")
        {
            ElementSymType = elemSymType;
        }
        
        public SymType ElementSymType { get; }
    }

    public class SymPointerType : SymType
    {
        public SymPointerType(SymType referencedSymType, string ident = "") : base(ident)
        {
            ReferencedSymType = referencedSymType;
        }
        
        public SymType ReferencedSymType { get; }
    }
    
    public class IndexRange<T1, T2>
    {
            // @formatter:off
            public T1 From { get; }
            public T2 To     { get; }
        // @formatter:on

        public IndexRange(T1 fromIndex, T2 toIndex)
        {
                // @formatter:off
                From = fromIndex ;
                To     = toIndex;
            // @formatter:on
        }
    }
}