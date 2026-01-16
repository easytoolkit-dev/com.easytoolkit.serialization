using System;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Defines a contract for creating and retrieving serializers.
    /// </summary>
    public interface ISerializerFactory
    {
        IEasySerializer<T> GetSerializer<T>();
    }
}
