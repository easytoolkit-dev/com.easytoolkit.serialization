using System;
using JetBrains.Annotations;

namespace EasyToolKit.Serialization
{
    public interface ISerializationProcessorFactory
    {
        [CanBeNull] ISerializationProcessor GetProcessor(Type valueType);
    }
}
