using System;
using EasyToolKit.Core.Patterns;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Environment for serialization operations with dependency injection support.
    /// </summary>
    public sealed class SerializationEnvironment : Singleton<SerializationEnvironment>
    {
        private readonly IServiceContainer _factoryContainer;

        private SerializationEnvironment()
        {
            var descriptors = GetServiceDescriptors();
            _factoryContainer = ServiceContainerBuilder.Build(descriptors);
        }

        public T GetFactory<T>() where T : class
        {
            return _factoryContainer.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Gets the default service descriptors for serialization.
        /// </summary>
        /// <returns>Collection of default service descriptors.</returns>
        private static ServiceDescriptor[] GetServiceDescriptors()
        {
            return new ServiceDescriptor[]
            {
                ServiceDescriptor.Singleton<
                    ISerializationNodeFactory,
                    Implementations.SerializationNodeFactory>(),

                ServiceDescriptor.Singleton<
                    ISerializationStructureResolverFactory,
                    Implementations.DefaultSerializationStructureResolverFactory>(),

                ServiceDescriptor.Singleton<
                    IFormatterFactory,
                    Implementations.FormatterFactory>(),

                ServiceDescriptor.Singleton<
                    ISerializationProcessorFactory,
                    Implementations.SerializationProcessorFactory>(),
            };
        }
    }
}
