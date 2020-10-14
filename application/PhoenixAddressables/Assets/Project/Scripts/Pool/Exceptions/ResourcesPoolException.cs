using System;

namespace Phoenix.Pool
{
    public class ResourcesPoolException : Exception
    {
        public ResourcesPoolException()
        {
        }
        public ResourcesPoolException(string message) : base(message)
        {
        }
        public ResourcesPoolException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
