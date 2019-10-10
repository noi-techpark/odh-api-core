using System;
using System.Runtime.Serialization;

namespace Helper
{
    [Serializable]
    public class JsonPathException : Exception
    {
        public JsonPathException()
        {
        }

        public JsonPathException(string? message) : base(message)
        {
        }

        public JsonPathException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected JsonPathException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}