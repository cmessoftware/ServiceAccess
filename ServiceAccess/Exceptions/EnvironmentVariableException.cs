using System.Runtime.Serialization;

namespace ServiceAccess
{
    [Serializable]
    public class EnvironmentVariableException : Exception
    {
        public EnvironmentVariableException()
        {
        }

        public EnvironmentVariableException(string message) : base(message)
        {
        }

        public EnvironmentVariableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EnvironmentVariableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}