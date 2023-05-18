using System.Runtime.Serialization;

namespace ServiceAccess
{
    [Serializable]
    public class KeyVaultEception : Exception
    {
        public KeyVaultEception()
        {
        }

        public KeyVaultEception(string message) : base(message)
        {
        }

        public KeyVaultEception(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KeyVaultEception(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}