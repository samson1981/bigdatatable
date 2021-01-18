using System.IO;
using System.Text;

namespace TreeZe.Sdk
{
    public sealed class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding _encoding;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="encoding"></param>
        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding) : base(sb)
        {
            _encoding = encoding;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Encoding Encoding => _encoding;
    }
}