using System;
using System.Runtime.Serialization;

namespace CleanArchitecture.Persistence.DataAccess.ElasticSearch
{
    [Serializable]
    public class ElasticSearchException : Exception
    {
        public ElasticSearchException()
        {
        }

        public ElasticSearchException(SerializationInfo serializationInfo, StreamingContext context) : base(
            serializationInfo, context)
        {
        }

        public ElasticSearchException(string message) : base(message)
        {
        }

        public ElasticSearchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}