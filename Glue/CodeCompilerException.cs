using System;
using System.Collections.Generic;

using System.Text;

namespace Glue
{
    public class CodeCompilerException : Exception
    {
        public CodeCompilerException()
            : base()
        {
        }
        public CodeCompilerException(string message)
            : base(message)
        {
        }
        public CodeCompilerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public CodeCompilerException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
