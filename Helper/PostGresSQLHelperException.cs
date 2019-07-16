using System;
using System.Runtime.Serialization;

namespace Helper
{
    public class PostGresSQLHelperException : Exception
    {
        public PostGresSQLHelperException()
        {
        }

        public PostGresSQLHelperException(string message) : base(message)
        {
        }

        public PostGresSQLHelperException(Exception innerException) : base("Error executing SQL query.", innerException)
        {
        }

        public PostGresSQLHelperException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PostGresSQLHelperException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
