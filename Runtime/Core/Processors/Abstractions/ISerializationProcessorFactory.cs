using System;

namespace EasyToolKit.Serialization
{
    public interface ISerializationProcessorFactory
    {
        ISerializationProcessor<T> GetProcessor<T>();
    }
}
