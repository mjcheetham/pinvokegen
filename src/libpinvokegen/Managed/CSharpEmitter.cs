using System.Linq;
using System.Text;
using PinvokeGen.Native;

namespace PinvokeGen.Managed
{
    public class CSharpEmitter
    {
        private readonly CModel _model;

        public CSharpEmitter(CModel model)
        {
            _model = model;
        }

        public string Emit()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"public static class {_model.LibraryName}");
            sb.AppendLine("{");

            EmitLibraryName(sb);

            foreach (CFunction function in _model.Functions)
            {
                sb.AppendLine();
                EmitFunction(sb, function);
            }

            foreach (CType type in _model.AllTypes)
            {
                sb.AppendLine();
                EmitTypeDefinition(sb, type);
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        private void EmitLibraryName(StringBuilder sb)
        {
            EmitIndent(sb);
            sb.AppendLine($"private const string LibraryName = \"{_model.LibraryName}\";");
        }

        private void EmitFunction(StringBuilder sb, CFunction function)
        {
            EmitDllImport(sb);

            EmitIndent(sb);
            sb.Append("public static extern ");

            if (function.ReturnType.IsPointer || function.Parameters.Any(x => x.Type.IsPointer))
            {
                sb.Append("unsafe ");
            }

            EmitTypeReference(sb, function.ReturnType);

            sb.Append($" {function.Name}(");

            for (var i = 0; i < function.Parameters.Count; i++)
            {
                EmitParameter(sb, function.Parameters[i]);
                if (i + 1 != function.Parameters.Count)
                {
                    if (function.Parameters.Count < 4)
                    {
                        sb.Append(", ");
                    }
                    else
                    {
                        sb.Append(",\n        ");
                    }
                }
            }
            sb.AppendLine(");");
        }

        private void EmitDllImport(StringBuilder sb)
        {
            EmitIndent(sb);
            sb.AppendLine("[DllImport(LibraryName)]");
        }

        private void EmitParameter(StringBuilder sb, CParameter parameter)
        {
            EmitTypeReference(sb, parameter.Type);
            sb.Append($" {parameter.Name}");
        }

        private void EmitTypeReference(StringBuilder sb, CTypeReference reference)
        {
            if (reference.IsReference) sb.Append("ref ");
            sb.Append(reference.Definition.Name);
            sb.Append(new string('*', reference.PointerDepth));
        }

        private void EmitTypeDefinition(StringBuilder sb, CType type)
        {
            switch (type)
            {
                case CStructType structType:
                    sb.AppendLine("    [StructLayout(LayoutKind.Sequential)]");
                    sb.Append($"    public struct {structType.Name}");
                    if (structType.Fields.Any())
                    {
                        sb.AppendLine("\n    {");
                        foreach (CField field in structType.Fields)
                        {
                            sb.Append("        ");
                            EmitField(sb, field);
                        }
                        sb.AppendLine("    }");
                    }
                    else
                    {
                        sb.AppendLine(" { /* opaque */ }");
                    }
                    break;
                default:
                    break;
            }
        }

        private void EmitField(StringBuilder sb, CField field)
        {
            sb.Append("public ");
            if (field.Type.IsPointer) sb.Append("unsafe ");
            sb.Append(field.Type.Definition.Name);
            sb.Append(new string('*', field.Type.PointerDepth));
            sb.AppendLine($" {field.Name};");
        }

        private void EmitIndent(StringBuilder sb)
        {
            sb.Append(new string(' ', 4));
        }
    }
}
