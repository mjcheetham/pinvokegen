using System;
using System.Text;

namespace PinvokeGen
{
    public class IndentedStringBuilder
    {
        private readonly string _indentStr;
        private readonly StringBuilder _sb = new StringBuilder();

        private int _indentLevel = 0;
        private bool _shouldIndent = true;

        public IndentedStringBuilder()
        {
            _indentStr = "\t";
        }

        public IndentedStringBuilder(int spaces)
        {
            _indentStr = new string(' ', spaces);
        }

        public IndentedStringBuilder(string indentStr)
        {
            _indentStr = indentStr;
        }

        public int IndentLevel
        {
            get => _indentLevel;
            private set
            {
                _shouldIndent = true;
                _indentLevel = value;
            }
        }

        public void Indent()
        {
            IndentLevel = IndentLevel + 1;
        }

        public void Unindent()
        {
            IndentLevel = Math.Min(0, IndentLevel - 1);
        }

        public IDisposable WithIndent()
        {
            return new IndentScope(this, IndentLevel + 1);
        }

        public IDisposable WithIndent(int level)
        {
            return new IndentScope(this, level);
        }

        private void AppendIndent()
        {
            if (!_shouldIndent) return;

            for (int i = 0; i < IndentLevel; i++)
            {
                _sb.Append(_indentStr);
            }

            _shouldIndent = false;
        }

        public void Append(string text)
        {
            AppendIndent();
            _sb.Append(text);
        }

        public void AppendLine()
        {
            AppendIndent();
            _sb.AppendLine();
            _shouldIndent = true;
        }

        public void AppendLine(string text)
        {
            AppendIndent();
            _sb.AppendLine(text);
            _shouldIndent = true;
        }

        public void AppendFormat(string format, params object[] args)
        {
            AppendIndent();
            _sb.AppendFormat(format, args);
        }

        public void AppendIf(bool b, string text)
        {
            if (b) Append(text);
        }

        public void AppendLineIf(bool b)
        {
            if (b) AppendLine();
        }

        public void AppendLineIf(bool b, string text)
        {
            if (b) AppendLine(text);
        }

        public void AppendFormatIf(bool b, string format, params object[] args)
        {
            if (b) AppendFormat(format, args);
        }

        public override string ToString()
        {
            return _sb.ToString();
        }

        private class IndentScope : IDisposable
        {
            private readonly IndentedStringBuilder _sb;
            private readonly int _originalLevel;

            public IndentScope(IndentedStringBuilder sb, int level)
            {
                _sb = sb;
                _originalLevel = _sb.IndentLevel;
                _sb.IndentLevel = level;
            }

            public void Dispose()
            {
                _sb.IndentLevel = _originalLevel;
            }
        }
    }
}
