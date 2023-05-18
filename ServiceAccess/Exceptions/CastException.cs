using System.Runtime.Serialization;

namespace ServiceAccess
{
    [Serializable]
    public class CastException : Exception
    {
        public CastException()
        {
        }

        public CastException(string message) : base(message)
        {
        }

        public CastException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CastException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}