using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FEFUPascalCompiler.Parser.Sematics
{
    public abstract class SymbolType : Symbol
    {
        protected SymbolType(string ident) : base(ident)
        {
        }

        public bool Equals(ref SymbolType symbolType)
        {
            return false;
        }
    }
    
    public class IntegerSymbolType : SymbolType
    {
        public IntegerSymbolType() : base("Integer")
        {
        }

        public bool Equals(ref IntegerSymbolType symbolType)
        {
            return true;
        }
    }

    public class IntegerSymbolConst : IntegerSymbolType
    {
        public IntegerSymbolConst(int value) : base()
        {
            Value = value;
        }
        
        public int Value { get; }
    }

    public class FloatSymbolType : SymbolType
    {
        public FloatSymbolType() : base("Float")
        {
        }

        public bool Equals(ref FloatSymbolType symbolType)
        {
            return true;
        }
    }

    public class FloatSymbolConst : FloatSymbolType
    {
        public FloatSymbolConst(float value) : base()
        {
            Value = value;
        }
        
        public float Value { get; }
    }
    
    public class CharSymbolType : SymbolType
    {
        public CharSymbolType() : base("Char")
        {
        }

        public bool Equals(ref CharSymbolType symbolType)
        {
            return true;
        }
    }
    
    public class CharSymbolConst : CharSymbolType
    {
        public CharSymbolConst(char value) : base()
        {
            Value = value;
        }
        
        public char Value { get; }
    }

    public class StringSymbolType : SymbolType
    {
        public StringSymbolType() : base("String")
        {
        }

        public bool Equals(ref StringSymbolType symbolType)
        {
            return true;
        }
    }
    
    public class StringSymbolConst : StringSymbolType
    {
        public StringSymbolConst(string value) : base()
        {
            Value = value;
        }
        
        public string Value { get; }
    }

    
    public class BoolSymbolType : SymbolType
    {
        public BoolSymbolType() : base("Bool")
        {
        }

        public bool Equals(ref BoolSymbolType symbolType)
        {
            return true;
        }
    }

    public class BoolSymbolConst : BoolSymbolType
    {
        public BoolSymbolConst(bool value) : base()
        {
            Value = value;
        }
        
        public bool Value { get; }
    }
    
    public class AliasSymbolType : SymbolType
    {
        public AliasSymbolType(string ident, SymbolType symbolType) : base(ident)
        {
            Alias = symbolType;
        }

        public new bool Equals(ref SymbolType symbolType)
        {
            return Alias.Equals(ref symbolType);
        }

        public bool Equals(ref AliasSymbolType symbolType)
        {
            var typeAlias = symbolType.Alias;
            return Alias.Equals(ref typeAlias);
        }

        public SymbolType Alias { get; }
    }

    public class RecordSymbolType : SymbolType
    {
        public RecordSymbolType(SymbolTable table, string ident = "") : base(ident)
        {
            Table = table;
        }

        public bool Equals(ref RecordSymbolType symbolType)
        {
            return string.Equals(Ident, symbolType.Ident) && (Equals(symbolType) || Ident.Length != 0);
        }

        public SymbolTable Table { get; }
    }

    public class ArraySymbolTypeSymbol : SymbolType
    {
        public ArraySymbolTypeSymbol(List<IndexRange<int, int>> indexRanges, SymbolType elemSymbolType, string ident = "") : base(ident)
        {
            IndexRanges = indexRanges;
            ElementSymbolType = elemSymbolType;
        }

        public bool Equals(ref ArraySymbolTypeSymbol symbolTypeSymbol)
        {
            var typeElementType = symbolTypeSymbol.ElementSymbolType;
            bool res = Ident == symbolTypeSymbol.Ident && ElementSymbolType.Equals(ref typeElementType);
            for (int i = 0; i < Math.Max(IndexRanges.Count, symbolTypeSymbol.IndexRanges.Count); ++i)
                res = res && IndexRanges[i] == symbolTypeSymbol.IndexRanges[i];

            return res;
        }

        public List<IndexRange<int, int>> IndexRanges { get; }
        public SymbolType ElementSymbolType { get; }
    }

    public class ConformatArraySymbolType : SymbolType
    {
        public ConformatArraySymbolType(SymbolType elemSymbolType) : base("")
        {
            ElementSymbolType = elemSymbolType;
        }
        
        public SymbolType ElementSymbolType { get; }
    }

    public class PointerSymbolTypeSumbol : SymbolType
    {
        public PointerSymbolTypeSumbol(SymbolType referencedSymbolType, string ident = "") : base(ident)
        {
            ReferencedSymbolType = referencedSymbolType;
        }
        
        public SymbolType ReferencedSymbolType { get; }
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