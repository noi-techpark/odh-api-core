using System;
using System.Runtime.Serialization;

namespace Helper
{
    [Serializable]
    public class JsonPathException : Exception
    {
        public string Path { get; } = "";

        public JsonPathException()
        {
        }

        public JsonPathException(string? message, string path) : base(message)
        {
            Path = path;
        }

        public JsonPathException(string? message, string path, Exception? innerException) : base(message, innerException)
        {
            Path = path;
        }

        protected JsonPathException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}