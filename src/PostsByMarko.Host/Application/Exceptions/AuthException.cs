using System.Runtime.Serialization;

namespace PostsByMarko.Host.Application.Exceptions
{
    [Serializable]
    public class AuthException : Exception
    {
        public AuthException() { }
        public AuthException(string message) : base(message) { }
        public AuthException(string message, Exception inner) : base(message, inner) { }
#pragma warning disable SYSLIB0051 // Type or member is obsolete
        protected AuthException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051 // Type or member is obsolete
    }
}
