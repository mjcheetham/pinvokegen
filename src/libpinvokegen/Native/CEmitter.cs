using System;
using System.Linq;

namespace PinvokeGen.Native
{
    public class CEmitter
    {
        private readonly CModel _model;

        public CEmitter(CModel model)
        {
            _model = model;
        }

        public string Emit()
        {
            var sb = new IndentedStringBuilder(4);

            sb.AppendLine("#pragma once");

            foreach (CTypedef td in _model.Typedefs)
            {
                sb.AppendLine();
                EmitTypeDef(sb, td);
            }

            foreach (CType type in _model.NamedTypes)
            {
                sb.AppendLine();
                EmitType(sb, type);
            }

            foreach (CFunction function in _model.Functions)
            {
                sb.AppendLine();
                EmitFunction(sb, function);
            }

            return sb.ToString();
        }

        private void EmitFunction(IndentedStringBuilder sb, CFunction function)
        {
            EmitTypeReference(sb, function.ReturnType);
            sb.Append($"{function.Name}(");

            if (function.Parameters.Count == 0)
            {
                sb.AppendLine("void);");
                return;
            }

            int paramCount = function.Parameters.Count;
            bool lineBreaks = function.Parameters.Count > 4;

            IDisposable indentScope = lineBreaks ? sb.WithIndent() : null;

            for (var i = 0; i < paramCount; i++)
            {
                EmitParameter(sb, function.Parameters[i]);

                if (i + 1 < paramCount)
                {
                    if (lineBreaks)
                    {
                        sb.AppendLine(",");
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                }
            }

            indentScope?.Dispose();

            sb.AppendLine(");");
        }

        private void EmitTypeDef(IndentedStringBuilder sb, CTypedef td)
        {
            sb.Append("typedef ");
            EmitTypeInternal(sb, td.Type);
            sb.AppendFormat(" {0};", td.Name);
            sb.AppendLine();
        }

        private void EmitType(IndentedStringBuilder sb, CType t)
        {
            EmitTypeInternal(sb, t);
            sb.AppendLine(";");
        }

        private void EmitTypeInternal(IndentedStringBuilder sb, CType t)
        {
            switch (t)
            {
                case CStructType structType:
                    EmitStruct(sb, structType);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(t), $"Unknown {nameof(CType)}: '{t.GetType().Name}'");
            }
        }

        private void EmitStruct(IndentedStringBuilder sb, CStructType t)
        {
            sb.Append("struct");
            sb.AppendIf(!t.IsAnonymous, $" {t.StructName}");
            sb.Append(" {");
            if (t.Fields.Any())
            {
                sb.AppendLine();
                using (sb.WithIndent())
                {
                    foreach (CField field in t.Fields)
                    {
                        EmitField(sb, field);
                        sb.AppendLine();
                    }
                }
            }
            else
            {
                sb.Append(" ");
            }
            sb.Append("}");
        }

        private void EmitParameter(IndentedStringBuilder sb, CParameter p)
        {
            EmitNamedTypeRefInternal(sb, p.Type, p.Name);
            for (int i = 0; i < p.ArrayRank; i++)
            {
                sb.Append("[]");
            }
        }

        private void EmitField(IndentedStringBuilder sb, CField f)
        {
            EmitNamedTypeRefInternal(sb, f.Type, f.Name);
            if (f.ArrayRank > 0)
            {
                sb.Append($"[{f.ArrayLength}]");
                for (int i = 1; i < f.ArrayRank; i++)
                {
                    sb.Append("[]");
                }
            }
            sb.Append(";");
        }

        private void EmitNamedTypeRefInternal(IndentedStringBuilder sb, CTypeReference tr, string name)
        {
            EmitTypeReference(sb, tr);
            sb.Append(name);
        }

        private void EmitTypeReference(IndentedStringBuilder sb, CTypeReference tr)
        {
            sb.AppendIf(tr.IsConst, "const ");
            if (tr.Definition.IsAnonymous)
            {
                string name = _model.GetAllNames(tr.Definition).FirstOrDefault();
                if (name is null)
                {
                    throw new InvalidOperationException("Unknown name for type");
                }
                sb.Append(name);
            }
            else
            {
                sb.Append(tr.Definition.Name);
            }
            sb.Append(" ");
            sb.Append(new string('*', tr.PointerDepth));
            sb.AppendIf(tr.IsReference, "&");
        }
    }
}
