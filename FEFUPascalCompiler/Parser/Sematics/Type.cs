using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FEFUPascalCompiler.Parser.Sematics
{
    public abstract class Type : Symbol
    {
        protected Type(string ident) : base(ident)
        {
        }

        public bool Equals(ref Type type)
        {
            return false;
        }
    }

    public class IntegerType : Type
    {
        public IntegerType() : base("Integer")
        {
        }

        public bool Equals(ref IntegerType type)
        {
            return true;
        }
    }

    public class FloatType : Type
    {
        public FloatType() : base("Float")
        {
        }

        public bool Equals(ref FloatType type)
        {
            return true;
        }
    }

    public class CharType : Type
    {
        public CharType() : base("Char")
        {
        }

        public bool Equals(ref CharType type)
        {
            return true;
        }
    }

    public class StringType : Type
    {
        public StringType() : base("String")
        {
        }

        public bool Equals(ref StringType type)
        {
            return true;
        }
    }

    public class AliasType : Type
    {
        public AliasType(string ident, Type type) : base(ident)
        {
            Alias = type;
        }

        public new bool Equals(ref Type type)
        {
            return Alias.Equals(ref type);
        }

        public bool Equals(ref AliasType type)
        {
            var typeAlias = type.Alias;
            return Alias.Equals(ref typeAlias);
        }

        public Type Alias { get; }
    }

    public class RecordType : Type
    {
        public RecordType(OrderedDictionary table, string ident = "") : base(ident)
        {
            Table = table;
        }

        public bool Equals(ref RecordType type)
        {
            return string.Equals(Ident, type.Ident) && (Equals(type) || Ident.Length != 0);
        }

        public OrderedDictionary Table { get; }
    }

    public class ArrayTypeSymbol : Type
    {
        public ArrayTypeSymbol(List<IndexRange<int, int>> indexRanges, Type elemType, string ident = "") : base(ident)
        {
            IndexRanges = indexRanges;
            ElementType = elemType;
        }

        public bool Equals(ref ArrayTypeSymbol typeSymbol)
        {
            var typeElementType = typeSymbol.ElementType;
            bool res = Ident == typeSymbol.Ident && ElementType.Equals(ref typeElementType);
            for (int i = 0; i < Math.Max(IndexRanges.Count, typeSymbol.IndexRanges.Count); ++i)
                res = res && IndexRanges[i] == typeSymbol.IndexRanges[i];

            return res;
        }

        public List<IndexRange<int, int>> IndexRanges { get; }
        public Type ElementType { get; }
    }

    public class ConformatArrayType : Type
    {
        public ConformatArrayType(Type elemType) : base("")
        {
            ElementType = elemType;
        }
        
        public Type ElementType { get; }
    }

    public class PointerTypeSumbol : Type
    {
        public PointerTypeSumbol(Type referencedType, string ident = "") : base(ident)
        {
            ReferencedType = referencedType;
        }
        
        public Type ReferencedType { get; }
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