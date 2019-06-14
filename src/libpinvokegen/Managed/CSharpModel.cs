using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PinvokeGen.Managed
{
    public class CSharpModel
    {
        public CSharpModel(string libraryName)
        {
            LibraryName = libraryName;
        }

        public string LibraryName { get; }

        public IList<CSharpMethod> Methods { get; set; } = new List<CSharpMethod>();

        public IList<CSharpType> Types { get; set; } = new List<CSharpType>();
    }

    public class CSharpMethod
    {
        public CSharpMethod(CSharpTypeReference returnType, string name)
        {
            ReturnType = returnType;
            Name = name;
        }

        public CSharpTypeReference ReturnType { get; }

        public string Name { get; }

        public IList<CSharpParameter> Parameters { get; set; } = new List<CSharpParameter>();
    }

    public class CSharpPInvokeMethod : CSharpMethod
    {
        public CSharpPInvokeMethod(CSharpTypeReference returnType, string name)
            : base(returnType, name)
        {
        }

        public CallingConvention CallingConvention { get; set; } = CallingConvention.Cdecl;
    }

    public abstract class CSharpType
    {
        protected CSharpType(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public CSharpTypeReference Value()
        {
            return new CSharpTypeReference(this);
        }

        public CSharpTypeReference Pointer(int p = 0)
        {
            return new CSharpTypeReference(this) {PointerDepth = p};
        }
    }

    public class CSharpTypeReference
    {
        public CSharpTypeReference(CSharpType definition)
        {
            Definition = definition;
        }

        public CSharpType Definition { get; }

        public bool IsUnsafe => IsPointer;

        public bool IsArray => ArrayDepth > 0;

        public int ArrayDepth { get; set; }

        public bool IsPointer => PointerDepth > 0;

        public int PointerDepth { get; set; }
    }

    public class CSharpEnumType<T> : CSharpType where T : CSharpType
    {
        public CSharpEnumType(string name)
            : base(name)
        {
        }

        public T BackingType { get; set; }

        public IDictionary<string, T> Members { get; set; }
    }

    public class CSharpStructType : CSharpType
    {
        public CSharpStructType(string name, bool isClass)
            : base(name)
        {
            IsClass = isClass;
        }

        public bool IsClass { get; }

        public IList<CSharpField> Fields { get; set; }

        public bool IsPacked { get; set; }
    }

    public class CSharpParameter
    {
        public CSharpParameter(CSharpTypeReference type, string name)
        {
            Type = type;
            Name = name;
        }

        public CSharpTypeReference Type { get; }

        public string Name { get; }

        public bool IsOut { get; set; }

        public bool IsRef { get; set; }

        public bool IsIn { get; set; }
    }

    public class CSharpField
    {
        public CSharpField(CSharpTypeReference type, string name)
        {
            Type = type;
            Name = name;
        }

        public CSharpTypeReference Type { get; }

        public string Name { get; }

        public bool IsUnsafe => Type.IsUnsafe;
    }
}
