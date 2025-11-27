using System.Runtime.Serialization;

namespace PostsByMarko.Host.Application.Exceptions
{
    [Serializable]
    public class AuthException : Exception
    {
        public AuthException() { }
        public AuthException(string message) : base(message) { }
        public AuthException(string message, Exception inner) : base(message, inner) { }
        protected AuthException(SerializationInfo info, StreamingContext context) { }
    }
}
