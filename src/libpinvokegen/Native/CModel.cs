using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace PinvokeGen.Native
{
    public class CModel
    {
        private readonly ISet<CFunction> _functions = new HashSet<CFunction>(CSymbolNameComparer.Instance);
        private readonly ISet<CType> _namedTypes = new HashSet<CType>(CSymbolNameComparer.Instance);
        private readonly IList<CType> _anonTypes = new List<CType>();
        private readonly IDictionary<string, CTypedef> _typedefs = new Dictionary<string, CTypedef>();

        public CModel(string libraryName)
        {
            LibraryName = libraryName;
        }

        public string LibraryName { get; }

        public IEnumerable<CFunction> Functions => _functions;

        public IEnumerable<CType> AllTypes => _namedTypes.Union(_anonTypes);

        public IEnumerable<CType> NamedTypes => _namedTypes;

        public IEnumerable<CTypedef> Typedefs => _typedefs.Values;

        public CFunction AddFunction(CTypeReference returnType, string name, params CParameter[] parameters)
            => AddFunction(new CFunction(returnType, name, parameters));

        public CFunction AddFunction(CFunction f)
        {
            EnsureUnusedSymbol(f.Name);
            _functions.Add(f);

            return f;
        }

        public CType AddTypedef(CType type, string name) => AddTypedef(new CTypedef(type, name));

        public T AddTypedef<T>(CType type, string name)  where T : CType => AddTypedef<T>(new CTypedef(type, name));

        public CType AddTypedef(CTypedef td) => AddTypedef<CType>(td);

        public T AddTypedef<T>(CTypedef td) where T : CType
        {
            EnsureUnusedSymbol(td.Name);
            _typedefs.Add(td.Name, td);

            if (td.Type.IsAnonymous)
            {
                _anonTypes.Add(td.Type);
            }
            else if (!_namedTypes.Contains(td.Type))
            {
                AddType(td.Type);
            }

            return (T)td.Type;
        }

        public CType AddType(CType t) => AddType<CType>(t);

        public T AddType<T>(T t) where T : CType
        {
            EnsureUnusedSymbol(t.Name);

            if (t.IsAnonymous)
            {
                throw new ArgumentException("Type must have a name or be defined with a typedef.", nameof(t));
            }

            _namedTypes.Add(t);

            return t;
        }

        public IEnumerable<string> GetAllNames(CType type)
        {
            if (!type.IsAnonymous)
            {
                yield return type.Name;
            }

            foreach (CTypedef td in _typedefs.Values.Where(x => x.Type == type))
            {
                yield return td.Name;
            }
        }

        private void EnsureUnusedSymbol(string symbol)
        {
            if (symbol is null) return;

            if (Functions.Any(x => CSymbolNameComparer.Instance.Equals(x.Name, symbol)) ||
                AllTypes.Any(x => CSymbolNameComparer.Instance.Equals(x.Name, symbol)) ||
                _typedefs.ContainsKey(symbol))
            {
                throw new InvalidOperationException($"Symbol '{symbol}' has already been defined");
            }
        }

        private class CSymbolNameComparer : IEqualityComparer<ISymbol>, IEqualityComparer<string>
        {
            public static readonly CSymbolNameComparer Instance = new CSymbolNameComparer();

            private CSymbolNameComparer() { }

            public bool Equals(ISymbol x, ISymbol y)
            {
                if (x is null || y is null)
                {
                    return false;
                }

                return Equals(x.Name, y.Name);
            }

            public int GetHashCode(ISymbol obj)
            {
                return GetHashCode(obj.Name);
            }

            public bool Equals(string x, string y)
            {
                if (x is null || y is null)
                {
                    return false;
                }

                return x == y;
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }
    }

    public class CTypedef : ISymbol
    {
        public CTypedef(CType type, string name)
        {
            Type = type;
            Name = name;
        }

        public CType Type { get; }

        public string Name { get; }
    }

    public class CFunction : ISymbol
    {
        public CFunction(CTypeReference returnType, string name, params CParameter[] parameters)
        {
            ReturnType = returnType;
            Name = name;
            Parameters = (IList<CParameter>) parameters ?? new List<CParameter>();
        }

        public CTypeReference ReturnType { get; }

        public string Name { get; }

        public IList<CParameter> Parameters { get; set; }

        public CallingConvention CallingConvention { get; set; } = CallingConvention.Cdecl;
    }

    public abstract class CType : ISymbol
    {
        public static IEqualityComparer<CType> Comparer = new CTypeComparer();

        protected CType() {  }

        protected CType(string name)
        {
            Name = name;
        }

        public virtual string Name { get; }

        public bool IsAnonymous => Name is null;

        public CTypeReference Value(bool @const = false)
        {
            return new CTypeReference(this){IsConst = @const};
        }

        public CTypeReference Const() => Value(true);

        public CTypeReference ConstPointer(int p = 1) => Pointer(p, true);

        public CTypeReference Pointer(int p = 1, bool @const = false)
        {
            return new CTypeReference(this) {PointerDepth = p, IsConst = @const};
        }

        public CTypeReference Reference(bool @const)
        {
            return new CTypeReference(this) {IsReference = true, IsConst = @const};
        }

        private class CTypeComparer : IEqualityComparer<CType>
        {
            public bool Equals(CType x, CType y) => x?.Name == y?.Name;

            public int GetHashCode(CType obj) => obj.Name.GetHashCode();
        }

        #region Built-in Types

        public static CType Void = new BuiltInCType("void");
        public static CType SizeT = new BuiltInCType("size_t");
        public static CType Char = new BuiltInCType("char");
        public static CType SChar = new BuiltInCType("signed char");
        public static CType UChar = new BuiltInCType("unsigned char");
        public static CType Short = new BuiltInCType("short", "signed short int", "short int", "signed short");
        public static CType UShort = new BuiltInCType("short", "unsigned short", "unsigned short int");
        public static CType Int = new BuiltInCType("int", "signed", "signed int");
        public static CType UInt = new BuiltInCType("unsigned", "unsigned int");
        public static CType Long = new BuiltInCType("long", "signed long int", "long int", "signed long");
        public static CType ULong = new BuiltInCType("unsigned long", "unsigned long int");
        public static CType LongLong = new BuiltInCType("long long", "signed long long int", "long long int", "signed long long");
        public static CType ULongLong = new BuiltInCType("unsigned long long", "unsigned long long int");
        public static CType Float = new BuiltInCType("float");
        public static CType Double = new BuiltInCType("double");
        public static CType LongDouble = new BuiltInCType("long double");

        private class BuiltInCType : CType
        {
            public BuiltInCType(string name, params string[] aliases)
                : base(name)
            {
                Aliases = (IList<string>) aliases ?? new List<string>();
            }

            public IList<string> Aliases { get; }
        }
        #endregion
    }

    public class EnumCType : CType
    {
        public EnumCType(string name)
            : base(name) { }

        public IDictionary<string, int> Members { get; set; } = new Dictionary<string, int>();
    }

    public class CStructType : CType
    {
        public CStructType(params CField[] fields)
            : this(null, fields) { }

        public CStructType(string name, params CField[] fields)
        {
            StructName = name;
            Fields = fields;
        }

        public override string Name => StructName is null ? null : $"struct {StructName}";

        public string StructName { get; }

        public IList<CField> Fields { get; set; }
    }

    public class CTypeReference
    {
        public CTypeReference(CType type)
        {
            Definition = type;
        }

        public CType Definition { get; }

        public bool IsConst { get; set; }

        public bool IsReference { get; set; }

        public bool IsPointer => PointerDepth > 0;

        public int PointerDepth { get; set; }
    }

    public class CField
    {
        public CField(CTypeReference type, string name)
            : this(type, name, 0, 0) { }

        public CField(CTypeReference type, string name, int arrayRank, int arrayLength)
        {
            Type = type;
            Name = name;
            ArrayRank = arrayRank;
            ArrayLength = arrayLength;
        }

        public CTypeReference Type { get; }

        public string Name { get; }

        public int ArrayRank { get; set; }

        public int ArrayLength { get; set; }
    }

    public class CParameter : ISymbol
    {
        public CParameter(CTypeReference type, string name, int arrayRank = 0)
        {
            Type = type;
            Name = name;
            ArrayRank = arrayRank;
        }

        public CTypeReference Type { get; }

        public string Name { get; }

        public int ArrayRank { get; set; }
    }

    public interface ISymbol
    {
        string Name { get; }
    }
}
