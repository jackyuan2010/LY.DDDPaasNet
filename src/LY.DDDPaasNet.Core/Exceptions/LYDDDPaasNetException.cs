using System.Runtime.Serialization;

namespace LY.DDDPaasNet.Core.Exceptions;

public class LYDDDPaasNetException : Exception
{
    public LYDDDPaasNetException()
    {
    }

    public LYDDDPaasNetException(string message)
        : base(message)
    {
    }

    public LYDDDPaasNetException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public LYDDDPaasNetException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }
}